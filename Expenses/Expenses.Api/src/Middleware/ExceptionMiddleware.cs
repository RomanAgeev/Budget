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
                var messages = e.Errors.Select(it => it.ErrorMessage).ToArray();

                _logger.LogWarning(1000, e, $"Validation: {string.Join("\n", messages)}");

                await HandleServerError(context, new ExceptionResponse(
                    status: HttpStatusCode.BadRequest,
                    cause: "Validation",
                    errors: messages
                ));
            } catch(DomainException e) {
                _logger.LogWarning(1001, e, e.Message);

                await HandleServerError(context, new ExceptionResponse(
                    status: HttpStatusCode.BadRequest,
                    cause: System.Enum.GetName(typeof(DomainExceptionCause), e.Cause),
                    errors: new[] { e.Message }
                ));
            } catch(Exception e) {
                _logger.LogError(1002, "Unhandled Server Error", e.Message);

                throw;
            }
        }

        async Task HandleServerError(HttpContext context, ExceptionResponse exceptionResponse) {
            context.Response.StatusCode = (int)exceptionResponse.Status;
            context.Response.ContentType = "application/json";            
            await context.Response.WriteAsync(exceptionResponse.ToString());
        }
    }
}