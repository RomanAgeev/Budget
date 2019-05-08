using System.Collections.Generic;
using System.Threading.Tasks;

namespace Expenses.Api.Queries {
    public interface ICategoryQueries {
        Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync();
        Task<CategoryViewModel> GetCategoryAsync(int categoryId);
    }
}