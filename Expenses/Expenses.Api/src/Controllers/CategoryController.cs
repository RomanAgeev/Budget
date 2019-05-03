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
    public class CategoryController : ControllerBase {
        public CategoryController(ICategoryQueries categoryQueries, IMediator mediator) {
            Guard.NotNull(categoryQueries, nameof(categoryQueries));
            Guard.NotNull(mediator, nameof(mediator));

            _categoryQueries = categoryQueries;
            _mediator = mediator;
        }

        readonly ICategoryQueries _categoryQueries;
        readonly IMediator _mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetExpenseCategiesAsync() {
            IEnumerable<CategoryViewModel> categories = await _categoryQueries.GetCategoriesAsync();

            return Ok(categories);
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateExpenseCategory(CreateCategoryCommand command) {
            bool success = await _mediator.Send(command);
            if(success)
                return Created("", "");
            return BadRequest();
        }
    }
}