using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands { 
    public class DeleteExpenseCommandHandler : CommandHandlerBase<DeleteExpenseCommand> {
        public DeleteExpenseCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(DeleteExpenseCommand command, CancellationToken cancellationToken) {
            Expense expense = await Repository.GetExpenseAsync(command.ExpenseId, cancellationToken);
             if(expense == null)
                throw new DomainException(DomainExceptionCause.ExpenseNotFound, $"Expense with {command.ExpenseId} ID is not found"); 

            Repository.DeleteExpense(expense);

            await Repository.UnitOfWork.SaveAsync(cancellationToken);

            return true;
        }
    }
}