using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool> {
        public UpdateCategoryCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;
        
        public async Task<bool> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken) {
            Category category = _repository.GetCategory(command.CategoryId);
             if(category == null)
                throw new DomainException(DomainExceptionCause.CategoryNotFound, $"Category with {command.CategoryId} ID is not found"); 

            category.Update(command.Name, command.Description);

            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}