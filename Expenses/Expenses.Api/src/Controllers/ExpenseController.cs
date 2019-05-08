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
        const string RouteExpense = "expense";

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
            IEnumerable<ExpenseViewModel> expenses = await _expenseQueries.GetExpensesAsync();

            return Ok(expenses);
        }

        [HttpGet]
        [Route("{expenseId}", Name = RouteExpense)]
        [ProducesResponseType(typeof(ExpenseViewModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExpense(int expenseId) {
            ExpenseViewModel expense = await _expenseQueries.GetExpenseAsync(expenseId);

            return Ok(expense);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ExpenseViewModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateExpense(CreateExpenseCommand command) {
            int expenseId = await _mediator.Send(command);

            ExpenseViewModel expense = await _expenseQueries.GetExpenseAsync(expenseId);

            return CreatedAtRoute(RouteExpense, new { expenseId }, expense);
        }

        [HttpPut]
        [ProducesResponseType(typeof(IEnumerable<ExpenseViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateExpense(UpdateExpenseCommand command) {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(typeof(IEnumerable<ExpenseViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteExpense(DeleteExpenseCommand command) {
            await _mediator.Send(command);

            return Ok();
        }
    }
}