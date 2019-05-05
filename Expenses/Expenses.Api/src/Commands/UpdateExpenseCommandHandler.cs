using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, bool> {
        public UpdateExpenseCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;
        public async Task<bool> Handle(UpdateExpenseCommand command, CancellationToken cancellationToken) {
            Expense expense = await _repository.GetExpenseAsync(command.ExpenseId, cancellationToken);
             if(expense == null)
                throw new DomainException(DomainExceptionCause.ExpenseNotFound, $"Expense with {command.ExpenseId} ID is not found"); 

            expense.Update(command.Amount, command.Description);
 
            await _repository.UnitOfWork.SaveChangesAsync();

            return true;
        }
    }
}