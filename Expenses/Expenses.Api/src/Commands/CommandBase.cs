using MediatR;
using Newtonsoft.Json;

namespace Expenses.Api.Commands {
    public abstract class CommandBase : IRequest<bool> {
        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}