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
            Category defaultCategory = await _repository.GetCategoryAsync(1, cancellationToken);

            Category category = await _repository.GetCategoryAsync(command.CategoryId, cancellationToken);
            if(category == null)
                throw new DomainException(DomainExceptionCause.CategoryNotFound, $"Category with {command.CategoryId} ID is not found");            

            category.MoveExpenses(defaultCategory);

            _repository.DeleteCategory(category);

            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);


            return true;
        }
    }
}