using System.Threading;
using System.Threading.Tasks;
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

        public async Task<bool> Handle(DeleteExpenseCommand command, CancellationToken cancellationToken) {
            Expense expense = await _repository.GetExpenseAsync(command.ExpenseId, cancellationToken);
             if(expense == null)
                throw new DomainException(DomainExceptionCause.ExpenseNotFound, $"Expense with {command.ExpenseId} ID is not found"); 

            _repository.DeleteExpense(expense);

            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}