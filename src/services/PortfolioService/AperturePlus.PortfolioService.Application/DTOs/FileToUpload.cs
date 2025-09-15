using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.DTOs
{
    public class FileToUpload
    {
        public Stream FileStream { get; private set; }
        public string FileName { get; private set; }
        public string ContentType { get; private set; }

        public FileToUpload(Stream fileStream, string fileName, string contentType)
        {
            FileStream = fileStream;
            FileName = fileName;
            ContentType = contentType;
        }
    }
}
