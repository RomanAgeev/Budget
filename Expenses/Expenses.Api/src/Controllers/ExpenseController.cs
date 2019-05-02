using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Expenses.Api.Commands;
using Expenses.Api.Queries;
using Guards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Expenses.Api.Controllers {
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase {
        readonly IExpenseQueries _expenseQueries;
        readonly IMediator _mediator;

        public ExpenseController(IExpenseQueries expenseQueries, IMediator mediator) {
            Guard.NotNull(expenseQueries, nameof(expenseQueries));
            Guard.NotNull(mediator, nameof(mediator));

            _expenseQueries = expenseQueries;
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExpenseViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExpensesAsync() {
            IEnumerable<ExpenseViewModel> expenses = await _expenseQueries.GetExpensesAsync();

            return Ok(expenses);
        }

        [HttpGet]
        [Route("category")]
        [ProducesResponseType(typeof(IEnumerable<ExpenseCategoryViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExpenseCategiesAsync() {
            IEnumerable<ExpenseCategoryViewModel> categories = await _expenseQueries.GetExpenseCategoriesAsync();

            return Ok(categories);
        }

        [HttpPost]
        [Route("category")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateExpenseCategory([FromBody]CreateExpenseCategoryCommand command) {
            bool success = await _mediator.Send(command);
            if(success)
                return Created("", "");
            return BadRequest();
        }
    }
}