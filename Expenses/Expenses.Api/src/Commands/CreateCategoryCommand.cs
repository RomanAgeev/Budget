using MediatR;
using Newtonsoft.Json;

namespace Expenses.Api.Commands {
    public class CreateCategoryCommand : IRequest<bool> {
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}