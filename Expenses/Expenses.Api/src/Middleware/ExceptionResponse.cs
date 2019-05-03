using System.Collections.Generic;
using System.Net;
using Guards;
using Newtonsoft.Json;

namespace Expenses.Api.Middleware {
    public class ExceptionResponse {
        public ExceptionResponse(HttpStatusCode status, string cause, string[] errors) {
            Guard.NotNullOrWhiteSpace(cause, nameof(cause));
            Guard.NotNullOrEmpty(errors, nameof(errors));

            Status = status;
            Cause = cause;
            Errors = errors;
        }

        public HttpStatusCode Status { get; private set; }        
        public string Cause { get; private set; }
        public IEnumerable<string> Errors { get; private set; }

        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }
}