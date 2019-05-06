using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands { 
    public class DeleteExpenseCommandHandler : CommandHandlerBase<DeleteExpenseCommand> {
        public DeleteExpenseCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(DeleteExpenseCommand command, CancellationToken ct) {
            Expense expense = await Repository.EnsureExpenseByIdAsync(command.ExpenseId, ct);

            Repository.RemoveExpense(expense);

            await Repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}