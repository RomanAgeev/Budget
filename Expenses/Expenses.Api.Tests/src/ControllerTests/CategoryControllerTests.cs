using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Commands;
using Expenses.Api.Controllers;
using Expenses.Api.Queries;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Expenses.Api.Tests.ControllerTests {
    public class CategoryControllerTests {
        public CategoryControllerTests() {
            _fakeQueries = A.Fake<ICategoryQueries>();
            _fakeMediator = A.Fake<IMediator>();
            _controller = new CategoryController(_fakeQueries, _fakeMediator);
        }

        readonly ICategoryQueries _fakeQueries;
        readonly IMediator _fakeMediator;
        readonly CategoryController _controller;

        [Fact]
        public async Task GetCategoriesTest() {
            var categories = new[] { 
                new CategoryViewModel {
                    Id = 1,
                    Name = "First",
                    Description = "First_Description"
                },
                new CategoryViewModel {
                    Id = 2,
                    Name = "Second",
                    Description = "Second_Description"
                }
            };

            var getCategoies = A.CallTo(() => _fakeQueries.GetCategoriesAsync())
                .Returns(categories);            

            IActionResult result = await _controller.GetCategiesAsync();

            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(categories);
        }

        [Fact]
        public async Task GetCategoryTest() {
            const int categoryId = 1;
            const int categoryIdWrong = 111;

            var category = new CategoryViewModel {
                Id = categoryId,
                Name = "First",
                Description = "First_Description"
            };

            A.CallTo(() => _fakeQueries.GetCategoryAsync(categoryId)).Returns(category);
            A.CallTo(() => _fakeQueries.GetCategoryAsync(categoryIdWrong)).Returns<CategoryViewModel>(null);

            IActionResult okResult = await _controller.GetCategoryAsync(categoryId);
            okResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(category);

            IActionResult notFoundResult = await _controller.GetCategoryAsync(categoryIdWrong);
            notFoundResult.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateCategoryTest() {
            const int categoryId = 1;

            var command = new CreateCategoryCommand {
                Name = "First",
                Description = "First_Description"
            };

            var category = new CategoryViewModel {
                Id = categoryId,
                Name = "First",
                Description = "First_Description"                
            };

            var send = A.CallTo(() => _fakeMediator.Send(command, default(CancellationToken)));
            send.Returns(categoryId);

            A.CallTo(() => _fakeQueries.GetCategoryAsync(categoryId))
                .Returns(category);

            IActionResult result = await _controller.CreateCategoryAsync(command);

            var createdResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int> { { "categoryId", 1 } });
            createdResult.Value.Should().Be(category);

            send.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void CreateCategoryExeptionTest() {
            const int categoryId = 1;

            var command = new CreateCategoryCommand {
                Name = "First",
                Description = "First_Description"
            };

            A.CallTo(() => _fakeMediator.Send(command, default(CancellationToken)))
                .Returns(categoryId);

            A.CallTo(() => _fakeQueries.GetCategoryAsync(categoryId))
                .Returns<CategoryViewModel>(null);

            Func<Task<IActionResult>> action = async () => await _controller.CreateCategoryAsync(command);

            action.Should().ThrowExactly<Exception>();
        }

        [Fact]
        public async Task UpdateCategoryTest() {
            const int categoryId = 1;

            var command = new UpdateCategoryCommand {
                CategoryId = categoryId,
                Name = "First",
                Description = "First_Description"
            };

            var category = new CategoryViewModel {
                Id = categoryId,
                Name = "First",
                Description = "First_Description"
            };

            var send = A.CallTo(() => _fakeMediator.Send(command, default(CancellationToken)));

            A.CallTo(() => _fakeQueries.GetCategoryAsync(categoryId))
                .Returns(category);

            IActionResult result = await _controller.UpdateCategoryAsync(command);

            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(category);

            send.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void UpdateCategoryExceptionTest() {
            const int categoryId = 1;

            var command = new UpdateCategoryCommand {
                CategoryId = categoryId,
                Name = "First",
                Description = "First_Description"
            };

            A.CallTo(() => _fakeQueries.GetCategoryAsync(categoryId))
                .Returns<CategoryViewModel>(null);

            Func<Task<IActionResult>> action = async () => await _controller.UpdateCategoryAsync(command);

            action.Should().ThrowExactly<Exception>();
        }

        [Fact]
        public async Task DeleteCategoryTest() {
            const int categoryId = 1;

            var command = new DeleteCategoryCommand {
                CategoryId = categoryId
            };

            var send = A.CallTo(() => _fakeMediator.Send(command, default(CancellationToken)));

            IActionResult result = await _controller.DeleteCategoryAsync(command);

            result.Should().BeOfType<OkResult>();
        }
    }
}