using System.Threading;
using System.Threading.Tasks;
using Expenses.Domain;
using Expenses.Domain.Models;

namespace Expenses.Api.Utils {
    public static class ExpenseRepositoryExtensions {
        public static async Task<Category> EnsureCategoryByIdAsync(this IExpenseRepository repository, int categoryId, CancellationToken ct) {
            Category category = await repository.GetCategoryByIdAsync(categoryId, ct);
            if(category == null)
                throw new DomainException(DomainExceptionCause.CategoryNotFound, $"Category with ID {categoryId} doesn't exist"); 
            return category;
        }
    }
}