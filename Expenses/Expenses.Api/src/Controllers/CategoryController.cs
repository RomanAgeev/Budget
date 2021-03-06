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
    public class CategoryController : ControllerBase {
        const string RouteCategory = "category";

        public CategoryController(IMediator mediator) {
            Guard.NotNull(mediator, nameof(mediator));

            _mediator = mediator;
        }

        readonly IMediator _mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCategiesAsync() {
            IEnumerable<CategoryViewModel> categories = await _mediator.Send(new GetCategoriesQuery());

            return Ok(categories);
        }

        [HttpGet]
        [Route("{categoryId}", Name = RouteCategory)]
        [ProducesResponseType(typeof(CategoryViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCategoryAsync(int categoryId) {
            CategoryViewModel category = await _mediator.Send(new GetCategoryQuery(categoryId));

            if(category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryViewModel), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateCategoryAsync(CreateCategoryCommand command) {
            int categoryId = await _mediator.Send(command);

            CategoryViewModel category = await _mediator.Send(new GetCategoryQuery(categoryId));
            if(category == null)
                throw new Exception("Failed to get the newly created category");

            return CreatedAtRoute(RouteCategory, new { categoryId }, category);
        }

        [HttpPut]
        [ProducesResponseType(typeof(CategoryViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateCategoryAsync(UpdateCategoryCommand command) {
            await _mediator.Send(command);

            CategoryViewModel category = await _mediator.Send(new GetCategoryQuery(command.CategoryId));
            if(category == null)
                throw new Exception("Failed to get the updated category");

            return Ok(category);
        }

        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExceptionResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteCategoryAsync(DeleteCategoryCommand command) {
            await _mediator.Send(command);

            return Ok();
        }
    }
}