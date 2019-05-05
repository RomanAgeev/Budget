using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class DeleteCategoryCommandHandler : CommandHandlerBase<DeleteCategoryCommand> {
        public DeleteCategoryCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken) {
            Category defaultCategory = await Repository.GetCategoryAsync(1, cancellationToken);

            Category category = await Repository.GetCategoryAsync(command.CategoryId, cancellationToken);
            if(category == null)
                throw new DomainException(DomainExceptionCause.CategoryNotFound, $"Category with {command.CategoryId} ID is not found");            

            category.MoveExpenses(defaultCategory);

            Repository.DeleteCategory(category);

            await Repository.UnitOfWork.SaveChangesAsync(cancellationToken);


            return true;
        }
    }
}