using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class UpdateExpenseCommandHandler : CommandHandlerBase<UpdateExpenseCommand> {
        public UpdateExpenseCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(UpdateExpenseCommand command, CancellationToken cancellationToken) {
            Expense expense = await Repository.GetExpenseAsync(command.ExpenseId, cancellationToken);
             if(expense == null)
                throw new DomainException(DomainExceptionCause.ExpenseNotFound, $"Expense with {command.ExpenseId} ID is not found"); 

            expense.Update(command.Amount, command.Description);

            Category fromCategory = await Repository.GetContainingCategoryAsync(expense, cancellationToken);
            Category toCategory = await Repository.GetCategoryAsync(command.CategoryId, cancellationToken);
            if(toCategory != fromCategory)
                fromCategory.MoveExense(toCategory, expense);
 
            await Repository.UnitOfWork.SaveChangesAsync();

            return true;
        }
    }
}