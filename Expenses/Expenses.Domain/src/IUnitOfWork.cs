using System.Threading;
using System.Threading.Tasks;

namespace Expenses.Domain {
    public interface IUnitOfWork {
        Task SaveAsync(CancellationToken ct);
    }
}