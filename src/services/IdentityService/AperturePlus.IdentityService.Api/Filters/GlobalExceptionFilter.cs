using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace AperturePlus.IdentityService.Api.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> logger;
        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            this.logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, "发生未处理的异常: {ErrorMessage}", context.Exception.Message);

            //向客户端返回一个标准化的错误响应
            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "服务器内部错误。",
                Detail = "处理您的请求时发生了一个意外错误，我们已经记录了该问题。"
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            //标记异常已被处理
            context.ExceptionHandled = true;
        }
    }
}
