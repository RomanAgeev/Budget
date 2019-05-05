using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, bool> {
        public CreateExpenseCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;
        public async Task<bool> Handle(CreateExpenseCommand command, CancellationToken cancellationToken) {
             Category category = await _repository.GetCategoryAsync(command.CategoryId, cancellationToken);
             if(category == null)
                throw new DomainException(DomainExceptionCause.CategoryNotFound, $"Category with {command.CategoryId} ID is not found"); 

            category.AddExpense(command.Date, command.Amount, command.Description);

            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }        
    }
}