using MediatR;
using Newtonsoft.Json;

namespace Expenses.Api.Commands {
    public class DeleteCategoryCommand : IRequest<bool> {
        public int CategoryId { get; set; }

        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}