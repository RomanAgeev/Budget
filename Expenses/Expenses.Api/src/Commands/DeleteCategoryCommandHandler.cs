using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class DeleteCategoryCommandHandler : CommandHandlerBase<DeleteCategoryCommand> {
        public DeleteCategoryCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(DeleteCategoryCommand command, CancellationToken ct) {
            if(command.CategoryId == Constants.DefaultCategoryId)
                throw new DomainException(
                    cause: DomainExceptionCause.DefaultCategoryUpdateOrDelete,
                    message: "The default category cannot be deleted"
                );

            Category category = await Repository.EnsureCategoryByIdAsync(command.CategoryId, ct);
            Category defaultCategory = await Repository.GetCategoryByIdAsync(Constants.DefaultCategoryId, ct);

            await Repository.LoadExpensesAsync(category, ct);
            await Repository.LoadExpensesAsync(defaultCategory, ct);

            category.MoveExpenses(defaultCategory);

            Repository.RemoveCategory(category);

            await Repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}