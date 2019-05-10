using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Expenses.Domain;
using FluentValidation;
using Guards;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Expenses.Api.Middleware {
    public class ExceptionMiddleware {
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) {
            Guard.NotNull(next, nameof(next));
            Guard.NotNull(logger, nameof(logger));

            _next = next;
            _logger = logger;
        }

        readonly RequestDelegate _next;
        readonly ILogger<ExceptionMiddleware> _logger;

        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);

            } catch(ValidationException e) {
                string cause = "Validation";

                LogException(e, cause);

                await HandleServerError(context, new ExceptionResponse(
                    HttpStatusCode.BadRequest,
                    cause,
                    e.Errors.Select(it => it.ErrorMessage).ToArray()));

            } catch(DomainException e) {
                string cause = Enum.GetName(typeof(DomainExceptionCause), e.Cause);

                LogException(e, cause);

                await HandleServerError(context, new ExceptionResponse(
                    HttpStatusCode.BadRequest,
                    cause,
                    new[] { e.Message }));
            }
        }

        void LogException(Exception e, string cause) {
            _logger.LogWarning(e, "Cause {cause}", cause);
        }
        
        async Task HandleServerError(HttpContext context, ExceptionResponse exceptionResponse) {
            context.Response.StatusCode = (int)exceptionResponse.Status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(exceptionResponse.ToString());
        }
    }
}