using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class DeleteCategoryCommandHandler : CommandHandlerBase<DeleteCategoryCommand> {
        public DeleteCategoryCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken ct) {
            Category defaultCategory = await Repository.GetCategoryByIdAsync(1, ct);

            Category category = await Repository.GetCategoryByIdAsync(command.CategoryId, ct);
            if(category == null)
                throw new DomainException(DomainExceptionCause.CategoryNotFound, $"Category with {command.CategoryId} ID is not found");

            await Repository.LoadExpenses(defaultCategory, ct);
            await Repository.LoadExpenses(category, ct);

            category.MoveExpenses(defaultCategory);

            Repository.DeleteCategory(category);

            await Repository.UnitOfWork.SaveAsync(ct);


            return true;
        }
    }
}