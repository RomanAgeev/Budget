using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Commands {
    public class CreateCategoryCommandHandler : CommandHandlerBase<CreateCategoryCommand> {
        public CreateCategoryCommandHandler(IExpenseRepository repository)
            : base(repository) {            
        }

        public override async Task<bool> Handle(CreateCategoryCommand command, CancellationToken ct) {
            var category = await Repository.GetCategoryByNameAsync(command.Name, ct);
            if(category != null)
                throw new DomainException(
                    cause: DomainExceptionCause.DuplicatedCategoryName, 
                    message: $"Category '{command.Name}' already exists"
                );            

            Repository.AddCategory(new Category(command.Name, command.Description));

            await Repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}