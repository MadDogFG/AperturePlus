using AperturePlus.PortfolioService.Application.Commands;
using AperturePlus.PortfolioService.Application.DTOs;
using AperturePlus.PortfolioService.Application.Queries;
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
        [HttpPost("CreateGallery/{galleryName}")]
        public async Task<IActionResult> CreateGallery(string galleryName)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            var command = new CreateGalleryCommand(userId, galleryName);
            try
            {
                var result = await mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {

                return BadRequest("未能创建相册");
            }
        }

        [Authorize]
        [HttpPost("UploadPhotos/{galleryId}")]
        public async Task<IActionResult> UploadPhotos(Guid galleryId, [FromForm] List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                return BadRequest("上传至少一个文件");
            }
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            try
            {
                //将IFormFile列表转换为FileToUpload列表
                var filesToUpload = files.Select(file => new FileToUpload
                (
                    file.OpenReadStream(),
                    file.FileName,
                    file.ContentType
                )).ToList();
                var command = new AddPhotoToGalleryCommand(userId, galleryId, filesToUpload);
                var photoIds = await mediator.Send(command);
                return Ok(new { PhotoIds = photoIds });
            }
            catch (Exception)
            {

                return BadRequest("上传照片失败");
            }
        }

        [Authorize]
        [HttpGet("GetMyPortfolio")]
        public async Task<IActionResult> GetMyPortfolio()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }

            var query = new GetPortfolioByUserIdQuery(userId);
            try
            {
                var result = await mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = "未能找到您的作品集", Error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("GetPortfolioByUserId/{userId}")]
        public async Task<IActionResult> GetPortfolioByUserId(Guid userId)
        {
            var query = new GetPortfolioByUserIdQuery(userId);
            try
            {
                var result = await mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = "未能找到该用户的作品集", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("DeleteGallery/{galleryId}")]
        public async Task<IActionResult> DeleteGallery(Guid galleryId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            var command = new DeleteGalleryCommand(galleryId,userId);
            var result = await mediator.Send(command);
            if (result == false)
            {
                return BadRequest("删除失败");
            }
            return NoContent();//204 No Content是DELETE成功的标准响应
        }

        [Authorize]
        [HttpDelete("DeletePhoto/{galleryId}")]
        public async Task<IActionResult> DeletePhoto(Guid galleryId, [FromQuery] List<Guid> photoIds)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (String.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "无效的用户ID" });
            }
            var command = new DeletePhotoCommand(userId, galleryId, photoIds);
            var result = await mediator.Send(command);
            if (result == false)
            {
                return BadRequest("删除失败");
            }
            return NoContent();
        }
    }
}
