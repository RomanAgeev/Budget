using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Guards;
using MediatR;

namespace Expenses.Api.Behaviors {
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> {
        public ValidationBehavior(AbstractValidator<TRequest> validator) {
            Guard.NotNull(validator, nameof(validator));

            _validator = validator;
        }

        readonly AbstractValidator<TRequest> _validator;
        
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {
            await _validator.ValidateAndThrowAsync(request);
            
            return await next();
        }
    }
}