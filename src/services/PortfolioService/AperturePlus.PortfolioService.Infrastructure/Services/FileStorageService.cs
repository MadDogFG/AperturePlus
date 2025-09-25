using AperturePlus.PortfolioService.Application.Interfaces;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
            var bucketName = "portfolio";
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
            var result = await minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken).ConfigureAwait(false);
            if (!result)
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), cancellationToken);
            }
            //创建并设置公共只读策略
            var policy = new
            {
                Version = "2012-10-17",
                Statement = new[]
                {
                        new
                        {
                            Effect = "Allow",
                            Principal = new { AWS = new[] { "*" } },
                            Action = new[] { "s3:GetObject" },
                            Resource = new[] { $"arn:aws:s3:::{bucketName}/*" }
                        }
                    }
            };
            var policyJson = JsonSerializer.Serialize(policy);
            var setPolicyArgs = new SetPolicyArgs()
                .WithBucket(bucketName)
                .WithPolicy(policyJson);
            await minioClient.SetPolicyAsync(setPolicyArgs, cancellationToken);
        
        var uniqueFileName = Guid.NewGuid().ToString();//使用修改文件名为唯一的Guid以防冲突
            var putObjectArgs = new PutObjectArgs()
                .WithBucket("portfolio")
                .WithObject(uniqueFileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType);
            await minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
            return $"http://localhost:9000/{bucketName}/{uniqueFileName}";//返回文件的访问URL

        }
    }
}
