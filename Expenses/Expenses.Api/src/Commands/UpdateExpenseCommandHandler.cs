using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class UpdateExpenseCommandHandler : CommandHandlerBase<UpdateExpenseCommand> {
        public UpdateExpenseCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(UpdateExpenseCommand command, CancellationToken ct) {
            Expense expense = await Repository.GetExpenseAsync(command.ExpenseId, ct);
             if(expense == null)
                throw new DomainException(DomainExceptionCause.ExpenseNotFound, $"Expense with {command.ExpenseId} ID is not found"); 

            expense.Update(command.Amount, command.Description);

            Category fromCategory = await Repository.GetContainingCategoryAsync(expense, ct);
            Category toCategory = await Repository.GetCategoryByIdAsync(command.CategoryId, ct);

            await Repository.LoadExpenses(fromCategory, ct);
            await Repository.LoadExpenses(toCategory, ct);

            if(toCategory != fromCategory)
                fromCategory.MoveExpense(toCategory, expense);
 
            await Repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}