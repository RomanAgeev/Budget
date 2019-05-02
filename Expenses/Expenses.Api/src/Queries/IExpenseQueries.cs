using System.Collections.Generic;
using System.Threading.Tasks;

namespace Expenses.Api.Queries {
    public interface IExpenseQueries {
        Task<IEnumerable<ExpenseCategoryViewModel>> GetExpenseCategoriesAsync();
        Task<IEnumerable<ExpenseViewModel>> GetExpensesAsync();
    }
}