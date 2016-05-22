using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApiFileUpload.Infrastructure;

namespace WebApiFileUpload.Controllers
{
    public class FileUploadController : ApiController
    {
        [MimeMultipart]
        public async Task<FileUploadResult> Post()
        {
            var uploadPath = HttpContext.Current.Server.MapPath("~/Storage");
            var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);
            await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);
            string localFileName = multipartFormDataStreamProvider.FileData.Select(m => m.LocalFileName).FirstOrDefault();

            return new FileUploadResult
            {
                LocalFilePath = localFileName,
                FileName = Path.GetFileName(localFileName),
                FileLength = new FileInfo(localFileName).Length
            };
        }
    }
}
