using AperturePlus.PortfolioService.Application.Interfaces;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.PortfolioService.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IMinioClient minioClient;

        public FileStorageService(IMinioClient minioClient)
        {
            this.minioClient = minioClient;
        }

        public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType,CancellationToken cancellationToken)
        {
            var result = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket("portfolio"), cancellationToken);
            if (!result)
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket("portfolio"), cancellationToken);
            }
            var uniqueFileName = Guid.NewGuid().ToString();//使用修改文件名为唯一的Guid以防冲突
            var putObjectArgs = new PutObjectArgs()
                .WithBucket("protfolio")
                .WithObject(uniqueFileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType);
            await minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
            return $"http://localhost:9000/protfolio/{uniqueFileName}";//返回文件的访问URL

        }
    }
}
