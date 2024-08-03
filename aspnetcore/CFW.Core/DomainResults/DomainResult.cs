
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CFW.Core.DomainResults
{
    public class DomainResult
    {
        public bool IsSuccess { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string? ErrorMessage { get; set; }

        private object? Result { get; set; }

        public static DomainResult NotFound()
        {
            return new DomainResult { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };
        }

        public static DomainResult<T> NotFound<T>()
        {
            return new DomainResult<T> { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };
        }

        public static DomainResult Success()
        {
            return new DomainResult { IsSuccess = true, StatusCode = HttpStatusCode.OK };
        }

        public static DomainResult<T> Success<T>(T result)
        {
            return new DomainResult<T> { IsSuccess = true, StatusCode = HttpStatusCode.OK };
        }

        public IActionResult ToActionResult()
        {
            if (IsSuccess)
            {
                return Result is not null ? new OkObjectResult(Result) : new OkResult();
            }

            if (StatusCode == HttpStatusCode.NotFound)
                return new NotFoundResult();

            if (StatusCode == HttpStatusCode.BadRequest)
                return new BadRequestObjectResult(ErrorMessage);

            return new StatusCodeResult((int)StatusCode);
        }
    }

    public class DomainResult<T> : DomainResult
    {

    }
}
