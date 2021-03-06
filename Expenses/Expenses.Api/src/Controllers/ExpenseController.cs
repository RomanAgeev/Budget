using System;
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

        public ExpenseController(IMediator mediator) {
            Guard.NotNull(mediator, nameof(mediator));

            _mediator = mediator;
        }

        readonly IMediator _mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExpenseViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExpensesAsync() {
            IEnumerable<ExpenseViewModel> expenses = await _mediator.Send(new GetExpensesQuery());

            return Ok(expenses);
        }

        [HttpGet]
        [Route("{expenseId}", Name = RouteExpense)]
        [ProducesResponseType(typeof(ExpenseViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetExpenseAsync(int expenseId) {
            ExpenseViewModel expense = await _mediator.Send(new GetExpenseQuery(expenseId));

            if(expense == null)
                return NotFound();

            return Ok(expense);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ExpenseViewModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateExpenseAsync(CreateExpenseCommand command) {
            int expenseId = await _mediator.Send(command);

            ExpenseViewModel expense = await _mediator.Send(new GetExpenseQuery(expenseId));
            if(expense == null)
                throw new Exception("Failed to get the newly created expense");

            return CreatedAtRoute(RouteExpense, new { expenseId }, expense);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ExpenseViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateExpenseAsync(UpdateExpenseCommand command) {
            await _mediator.Send(command);

            ExpenseViewModel expense = await _mediator.Send(new GetExpenseQuery(command.ExpenseId));
            if(expense == null)
                throw new Exception("Failed to get the updated expense");

            return Ok(expense);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(IEnumerable<ExpenseViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteExpenseAsync(DeleteExpenseCommand command) {
            await _mediator.Send(command);

            return Ok();
        }
    }
}