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
    public class CategoryController : ControllerBase {
        const string RouteCategory = "category";

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
        public async Task<IActionResult> GetCategies() {
            IEnumerable<CategoryViewModel> categories = await _categoryQueries.GetCategoriesAsync();

            return Ok(categories);
        }

        [HttpGet]
        [Route("{categoryId}", Name = RouteCategory)]
        [ProducesResponseType(typeof(CategoryViewModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCategory(int categoryId) {
            CategoryViewModel category = await _categoryQueries.GetCategoryAsync(categoryId);

            return Ok(category);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryViewModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateCategory(CreateCategoryCommand command) {
            int categoryId = await _mediator.Send(command);

            CategoryViewModel category = await _categoryQueries.GetCategoryAsync(categoryId);

            return CreatedAtRoute(RouteCategory, new { categoryId }, category);
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryCommand command) {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteCategory(DeleteCategoryCommand command) {
            await _mediator.Send(command);

            return Ok();
        }
    }
}