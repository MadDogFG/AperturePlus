using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken);//接收文件流、文件名和内容类型，返回上传后的访问的URL
    }
}
