using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class UpdateCategoryCommandHandler : CommandHandlerBase<UpdateCategoryCommand> {
        public UpdateCategoryCommandHandler(IExpenseRepository repository)
            : base(repository) {
        }

        public override async Task<bool> Handle(UpdateCategoryCommand command, CancellationToken ct) {
            if(command.CategoryId == Constants.DefaultCategoryId)
                throw new DomainException(
                    cause: DomainExceptionCause.DefaultCategoryUpdateOrDelete,
                    message: "It is forbidden to update the default category"
                );

            Category category = await Repository.EnsureCategoryByIdAsync(command.CategoryId, ct);

            category.Update(command.Name, command.Description);

            await Repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}