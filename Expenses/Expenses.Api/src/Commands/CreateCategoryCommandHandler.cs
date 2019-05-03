using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, bool> {
        public CreateCategoryCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;

        public async Task<bool> Handle(CreateCategoryCommand command, CancellationToken cancellationToken) {
            bool duplicatedCategoryName = _repository.GetCategories().Any(it => it.Name == command.Name);
            if(duplicatedCategoryName)
                throw new DomainException(
                    cause: DomainExceptionCause.DuplicatedCategoryName, 
                    message: $"Category with the name '{command.Name}' already exists"
                );

            
            var category = new Category(command.Name, command.Description);

            _repository.AddCategory(category);

            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}