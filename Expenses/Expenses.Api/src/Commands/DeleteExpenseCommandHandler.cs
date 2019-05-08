using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;
using MediatR;

namespace Expenses.Api.Commands { 
    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, bool> {
        public DeleteExpenseCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;

        public async Task<bool> Handle(DeleteExpenseCommand command, CancellationToken ct) {
            Expense expense = await _repository.EnsureExpenseByIdAsync(command.ExpenseId, ct);

            _repository.RemoveExpense(expense);

            await _repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}