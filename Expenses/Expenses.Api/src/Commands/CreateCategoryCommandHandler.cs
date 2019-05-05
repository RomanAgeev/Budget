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

        public override async Task<bool> Handle(CreateCategoryCommand command, CancellationToken cancellationToken) {
            bool duplicatedCategoryName = Repository.GetCategories().Any(it => it.Name == command.Name);
            if(duplicatedCategoryName)
                throw new DomainException(
                    cause: DomainExceptionCause.DuplicatedCategoryName, 
                    message: $"Category with the name '{command.Name}' already exists"
                );

            
            var category = new Category(command.Name, command.Description);

            Repository.AddCategory(category);

            await Repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}