using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class UpdateExpenseCommandHandler : CommandHandlerBase<UpdateExpenseCommand> {
        public UpdateExpenseCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(UpdateExpenseCommand command, CancellationToken ct) {
            Expense expense = await Repository.EnsureExpenseByIdAsync(command.ExpenseId, ct);

            expense.Update(command.Amount, command.Description);

            Category fromCategory = await Repository.GetContainingCategoryAsync(expense, ct);
            Category toCategory = await Repository.GetCategoryByIdAsync(command.CategoryId, ct);

            await Repository.LoadExpensesAsync(fromCategory, ct);
            await Repository.LoadExpensesAsync(toCategory, ct);

            if(toCategory != fromCategory)
                fromCategory.MoveExpense(toCategory, expense);
 
            await Repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}