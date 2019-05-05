using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool> {
        public DeleteCategoryCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;

        public async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken) {
            Category defaultCategory = _repository.GetCategory(1);

            Category category = _repository.GetCategory(command.CategoryId);
            if(category == null)
                throw new DomainException(DomainExceptionCause.CategoryNotFound, $"Category with {command.CategoryId} ID is not found");            

            category.MoveExpenses(defaultCategory);

            _repository.DeleteCategory(category);

            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);


            return true;
        }
    }
}