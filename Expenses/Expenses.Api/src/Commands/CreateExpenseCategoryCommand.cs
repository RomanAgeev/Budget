using MediatR;

namespace Expenses.Api.Commands {
    public class CreateExpenseCategoryCommand : IRequest<bool> {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}