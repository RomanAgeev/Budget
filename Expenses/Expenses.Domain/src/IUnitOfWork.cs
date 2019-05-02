using System.Threading;
using System.Threading.Tasks;

namespace Expenses.Domain {
    public interface IUnitOfWork {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}