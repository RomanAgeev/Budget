using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;
using FluentValidation;
using Guards;
using MediatR;

namespace Expenses.Api.Commands {
    public class UpdateCategoryCommand : IRequest<bool> {
        public class Validator : AbstractValidator<UpdateCategoryCommand> {
            public Validator() {
                RuleFor(it => it.CategoryId)
                    .GreaterThan(0);

                RuleFor(it => it.Name)
                    .NotNull()
                    .NotEmpty();
            }
        }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool> {
        public UpdateCategoryCommandHandler(IExpenseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IExpenseRepository _repository;

        public async Task<bool> Handle(UpdateCategoryCommand command, CancellationToken ct) {
            if(command.CategoryId == Constants.DefaultCategoryId)
                throw new DomainException(
                    cause: DomainExceptionCause.DefaultCategoryUpdateOrDelete,
                    message: "It is forbidden to update the default category"
                );

            Category category = await _repository.EnsureCategoryByIdAsync(command.CategoryId, ct);

            category.Update(command.Name, command.Description);

            await _repository.UnitOfWork.SaveAsync(ct);

            return true;
        }
    }
}