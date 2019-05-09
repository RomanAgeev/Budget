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
    public class ExpenseControllerTests {
        public ExpenseControllerTests() {
            _fakeQueries = A.Fake<IExpenseQueries>();
            _fakeMediator = A.Fake<IMediator>();
            _controller = new ExpenseController(_fakeQueries, _fakeMediator);
        }

        readonly IExpenseQueries _fakeQueries;
        readonly IMediator _fakeMediator;
        readonly ExpenseController _controller;

        [Fact]
        public async Task GetExpensesTest() {
            var expenses = new[] {
                new ExpenseViewModel {
                    Id = 1,
                    CategoryId = 1,
                    CategoryName = "First",
                    Date = new DateTime(2019, 1, 1),
                    Amount = 100,
                    Description = "Expense 1"
                },
                new ExpenseViewModel {
                    Id = 2,
                    CategoryId = 2,
                    CategoryName = "Second",
                    Date = new DateTime(2019, 1, 2),
                    Amount = 50,
                    Description = "Expense 2"
                }
            };

            A.CallTo(() => _fakeQueries.GetExpensesAsync())
                .Returns(expenses);

            IActionResult result = await _controller.GetExpensesAsync();

            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(expenses);
        }

        [Fact]
        public async Task GetExpenseTest() {
            const int expenseId = 1;
            const int expenseIdWrong = 111;

            var expense = new ExpenseViewModel {
                Id = expenseId,
                CategoryId = 1,
                CategoryName = "First",
                Date = new DateTime(2019, 1, 1),
                Amount = 100,
                Description = "Expense 1"
            };

            A.CallTo(() => _fakeQueries.GetExpenseAsync(expenseId))
                .Returns(expense);
            A.CallTo(() => _fakeQueries.GetExpenseAsync(expenseIdWrong))
                .Returns<ExpenseViewModel>(null);

            var okResult = await _controller.GetExpenseAsync(expenseId);
            okResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(expense);

            var notFoundResult = await _controller.GetExpenseAsync(expenseIdWrong);
            notFoundResult.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateExpenseTest() {
            const int expenseId = 3;

            var command = new CreateExpenseCommand {
                CategoryId = 2,
                Date = new DateTime(2019, 1, 3),
                Amount = 1.5m,
                Description = "Expense 3"
            };

            var expense = new ExpenseViewModel {
                Id = expenseId,
                CategoryId = 2,
                CategoryName = "Second",
                Date = new DateTime(2019, 1, 3),
                Amount = 1.5m,
                Description = "Expense 3"
            };

            var send = A.CallTo(() => _fakeMediator.Send(command, default(CancellationToken)));
            send.Returns(expenseId);

            A.CallTo(() => _fakeQueries.GetExpenseAsync(expenseId))
                .Returns(expense);

            IActionResult result = await _controller.CreateExpenseAsync(command);
            
            var createdResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdResult.RouteValues.Should().BeEquivalentTo(new Dictionary<string, int> { { "expenseId", expenseId } });
            createdResult.Value.Should().Be(expense);

            send.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void CreateExpenseExceptionTest() {
            const int expenseId = 3;

            var command = new CreateExpenseCommand {
                CategoryId = 2,
                Date = new DateTime(2019, 1, 3),
                Amount = 1.5m,
                Description = "Expense 3"
            };

            A.CallTo(() => _fakeMediator.Send(command, default(CancellationToken)))
                .Returns(expenseId);
            A.CallTo(() => _fakeQueries.GetExpenseAsync(expenseId))
                .Returns<ExpenseViewModel>(null);

            Func<Task<IActionResult>> action = async () => await _controller.CreateExpenseAsync(command);

            action.Should().ThrowExactly<Exception>();
        }

        [Fact]
        public async Task UpdateExpenseTest() {
            const int expenseId = 3;

            var command = new UpdateExpenseCommand {
                ExpenseId = expenseId,
                CategoryId = 2,
                Amount = 1.5m,
                Description = "Expense 3"                
            };

            var expense = new ExpenseViewModel {
                Id = expenseId,
                CategoryId = 2,
                CategoryName = "Second",
                Date = new DateTime(2019, 1, 3),
                Amount = 1.5m,
                Description = "Expense 3"
            };

            var send = A.CallTo(() => _fakeMediator.Send(command, default(CancellationToken)));

            A.CallTo(() => _fakeQueries.GetExpenseAsync(expenseId))
                .Returns(expense);

            IActionResult result = await _controller.UpdateExpenseAsync(command);

            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(expense);

            send.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void UpdateExpenseExceptionTest() {
            const int expenseId = 3;

            var command = new UpdateExpenseCommand {
                ExpenseId = expenseId,
                CategoryId = 2,
                Amount = 1.5m,
                Description = "Expense 3"                
            };

            A.CallTo(() => _fakeQueries.GetExpenseAsync(expenseId))
                .Returns<ExpenseViewModel>(null);

            Func<Task<IActionResult>> action = async () => await _controller.UpdateExpenseAsync(command);

            action.Should().ThrowExactly<Exception>();
        }

        [Fact]
        public async Task DeleteExpenseTest() {
            const int expenseId = 3;

            var command = new DeleteExpenseCommand {
                ExpenseId = expenseId
            };

            var send = A.CallTo(() => _fakeMediator.Send(command, default(CancellationToken)));

            IActionResult result = await _controller.DeleteExpenseAsync(command);

            result.Should().BeOfType<OkResult>();

            send.MustHaveHappenedOnceExactly();
        }
    }
}