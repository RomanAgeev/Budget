using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public abstract class CommandHandlerBase<TCommand> : IRequestHandler<TCommand, bool> where TCommand : CommandBase {
        protected CommandHandlerBase(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            Repository = repository;
        }

        protected IExpenseRepository Repository { get; private set; }

        public abstract Task<bool> Handle(TCommand command, CancellationToken ct);
    }
}