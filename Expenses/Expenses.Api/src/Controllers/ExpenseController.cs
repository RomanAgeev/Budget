using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Expenses.Api.Commands;
using Expenses.Api.Middleware;
using Expenses.Api.Queries;
using Guards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.Api.Controllers {
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase {
        public ExpenseController(IExpenseQueries expenseQueries, IMediator mediator) {
            Guard.NotNull(expenseQueries, nameof(expenseQueries));
            Guard.NotNull(mediator, nameof(mediator));

            _expenseQueries = expenseQueries;
            _mediator = mediator;
        }

        readonly IExpenseQueries _expenseQueries;
        readonly IMediator _mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExpenseViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExpenses() {
            var expenses = await _expenseQueries.GetExpensesAsync();

            return Ok(expenses);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateExpense(CreateExpenseCommand command) {
            await _mediator.Send(command);

            return Created("", "");
        }
    }
}