using System.Collections.Generic;
using System.Threading.Tasks;
using Expenses.Api.Queries;
using Guards;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.Api.Controllers {
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase {
        readonly IExpenseQueries _expenseQueries;

        public ExpenseController(IExpenseQueries expenseQueries) {
            Guard.NotNull(expenseQueries, nameof(expenseQueries));

            _expenseQueries = expenseQueries;
        }

        [HttpGet]
        public async Task<IActionResult> GetExpensesAsync() {
            IEnumerable<ExpenseViewModel> expenses = await _expenseQueries.GetExpensesAsync();

            return Ok(expenses);
        }
    }
}