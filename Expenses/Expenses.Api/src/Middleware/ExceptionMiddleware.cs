using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Expenses.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Expenses.Api.Middleware {
    public class ExceptionMiddleware {
        public ExceptionMiddleware(RequestDelegate next) {
            _next = next;
        }

        readonly RequestDelegate _next;

        // TODO: Log Error
        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);
            } catch(ValidationException e) {
                await HandleServerError(context, new ExceptionResponse(
                    status: HttpStatusCode.BadRequest,
                    cause: "Validation",
                    errors: e.Errors.Select(it => it.ErrorMessage).ToArray()
                ));
            } catch(DomainException e) {
                await HandleServerError(context, new ExceptionResponse(
                    status: HttpStatusCode.BadRequest,
                    cause: System.Enum.GetName(typeof(DomainExceptionCause), e.Cause),
                    errors: new[] { e.Message }
                ));
            }
        }

        async Task HandleServerError(HttpContext context, ExceptionResponse exceptionResponse) {
            context.Response.StatusCode = (int)exceptionResponse.Status;
            context.Response.ContentType = "application/json";            
            await context.Response.WriteAsync(exceptionResponse.ToString());
        }
    }
}