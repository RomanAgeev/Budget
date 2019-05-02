using MediatR;

namespace Expenses.Api.Commands {
    public class CreateCategoryCommand : IRequest<bool> {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}