using MediatR;

namespace Expenses.Api.Commands {
    public class DeleteCategoryCommand : IRequest<bool> {
        public int CategoryId { get; set; }
    }
}