using AperturePlus.PortfolioService.Api.DTOs;
using AperturePlus.PortfolioService.Application.Commands;
using AperturePlus.PortfolioService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AperturePlus.PortfolioService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
        private readonly IMediator mediator;
        public PortfoliosController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize]
        [HttpPost("UploadPhotos/{galleryId}")]
        public async Task<IActionResult> UploadPhotos(Guid galleryId, [FromForm] UploadPhotosRequest request)
        {
            if (request.Files == null || !request.Files.Any())
            {
                return BadRequest("上传至少一个文件");
            }
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            //将IFormFile列表转换为FileToUpload列表
            var filesToUpload = request.Files.Select(file => new FileToUpload
            (
                file.OpenReadStream(),
                file.FileName,
                file.ContentType
            )).ToList();
            var command = new AddPhotoToGalleryCommand(userId, galleryId, filesToUpload);
            var photoIds = await mediator.Send(command);
            return Ok(new { PhotoIds = photoIds });
        }
    }
}
