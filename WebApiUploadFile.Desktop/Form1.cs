using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebApiFileUpload.Infrastructure;

namespace WebApiUploadFile.Desktop
{
    public partial class Form1 : Form
    {
        const string RequestUrl = "http://localhost:53700/api/fileupload";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            // Set the file dialog to filter for graphics files.
            this.openFileDialog1.Filter =
                "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "Browse files to upload.";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                HttpClient client = new HttpClient();
                try
                {
                    foreach (string file in openFileDialog1.FileNames)
                    {
                        var fileStream = File.Open(file, FileMode.Open);
                        var fileInfo = new FileInfo(file);
                        FileUploadResult uploadResult = null;
                        bool fileUploaded = false;
                        var content = new MultipartFormDataContent();
                        content.Add(new StreamContent(fileStream), "\"file\"", string.Format("\"{0}\"", fileInfo.Name));
                        Task taskUpload = client.PostAsync(RequestUrl, content)
                            .ContinueWith(task =>
                            {
                                if (task.Status == TaskStatus.RanToCompletion)
                                {
                                    var response = task.Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        uploadResult = response.Content.ReadAsAsync<FileUploadResult>().Result;
                                        if (uploadResult != null)
                                        {
                                            fileUploaded = true;
                                        }
                                        foreach (var header in response.Content.Headers)
                                        {
                                            Debug.WriteLine("{0}: {1}", header.Key, string.Join(",", header.Value));
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Status Code: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                                        Debug.WriteLine("Response Body: {0}", response.Content.ReadAsStringAsync().Result);
                                    }
                                }
                                fileStream.Close();
                            });
                        taskUpload.Wait();
                        if (fileUploaded)
                        {
                            AddMessage(uploadResult.FileName + " with length " + uploadResult.FileLength
                                                    + " has been uploaded at " + uploadResult.LocalFilePath);
                        }
                    }
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    AddMessage(ex.Message);
                }
            }
        }

        private void AddMessage(string message)
        {
            textBox1.AppendText(message);
            textBox1.AppendText(Environment.NewLine);
        }


    }
}
