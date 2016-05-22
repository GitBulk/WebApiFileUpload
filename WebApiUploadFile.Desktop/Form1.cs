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
        const string RequestUrl = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
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

        private void AddMessage(string v)
        {
            throw new NotImplementedException();
        }
    }
}
