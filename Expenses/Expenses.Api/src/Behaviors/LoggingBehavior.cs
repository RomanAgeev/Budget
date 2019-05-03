using System.Threading;
using System.Threading.Tasks;
using Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Expenses.Api.Behaviors {
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> {
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) {
            Guard.NotNull(logger, nameof(logger));

            _logger = logger;
        }

        readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {            
            string commandName = request.GetType().Name;

            _logger.LogInformation($"----- Handling command \"{commandName}\"\n----- Request: {request}");

            var response = await next();

            _logger.LogInformation($"----- Command \"{commandName}\" handled\n----- Response: {response}");

            return response;
        }
    }
}