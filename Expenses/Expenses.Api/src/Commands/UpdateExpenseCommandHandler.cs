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
            if(command.CategoryId == Constants.DefaultCategoryId)
                throw new DomainException(
                    cause: DomainExceptionCause.DefaultCategoryUpdateOrDelete,
                    message: "It is forbidden to move an expense to the default category"
                );

            Expense expense = await Repository.EnsureExpenseByIdAsync(command.ExpenseId, ct);

            int expenseCategoryId = Repository.GetExpenseCategoryId(expense);
            
            Category fromCategory = await Repository.EnsureCategoryByIdAsync(expenseCategoryId, ct);
            Category toCategory = await Repository.EnsureCategoryByIdAsync(command.CategoryId, ct);

            await Repository.LoadExpensesAsync(fromCategory, ct);
            await Repository.LoadExpensesAsync(toCategory, ct);

            if(toCategory != fromCategory)
                fromCategory.MoveExpense(toCategory, expense);

            expense.Update(command.Amount, command.Description);
 
            await Repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}