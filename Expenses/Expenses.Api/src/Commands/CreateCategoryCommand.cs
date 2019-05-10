using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;
using FluentValidation;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class CreateCategoryCommand : IRequest<int> {
        public class Validator : AbstractValidator<CreateCategoryCommand> {
            public Validator() {
                RuleFor(it => it.Name)
                    .NotNull()
                    .NotEmpty();
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int> {
        public CreateCategoryCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;

        public async Task<int> Handle(CreateCategoryCommand command, CancellationToken ct) {
            var category = await _repository.GetCategoryByNameAsync(command.Name, ct);
            if(category != null)
                throw new DomainException(
                    cause: DomainExceptionCause.DuplicatedCategoryName, 
                    message: $"Category '{command.Name}' already exists"
                );            

            var newCategory = new Category(command.Name, command.Description);
            
            _repository.AddCategory(newCategory);

            await _repository.UnitOfWork.SaveAsync(ct);

            return newCategory.Id;
        }
    }
}