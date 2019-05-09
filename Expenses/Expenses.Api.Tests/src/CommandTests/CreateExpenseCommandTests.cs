using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Commands;
using Expenses.Api.Utils;
using Expenses.Domain;
using Expenses.Domain.Models;
using FakeItEasy;
using FakeItEasy.Configuration;
using FluentAssertions;
using Xunit;

namespace Expenses.Api.Tests.CommandTests {
    public class CreateExpenseCommandTests {
        public CreateExpenseCommandTests() {
            _fakeRepository = A.Fake<IExpenseRepository>(); 
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();

            A.CallTo(() => _fakeRepository.UnitOfWork).Returns(_fakeUnitOfWork); 

            _commandHandler =  new CreateExpenseCommandHandler(_fakeRepository);

            _category = new Category("category", null).WithId(CategoryId);            

            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(CategoryId, default(CancellationToken)))
                .Returns(_category);
        }

        const int CategoryId = 100;

        readonly IExpenseRepository _fakeRepository;
        readonly IUnitOfWork _fakeUnitOfWork;
        readonly CreateExpenseCommandHandler _commandHandler;
        readonly Category _category;

        [Fact]
        public async Task CreateExpenseTest() {
            const int expectedExpenseId = 111;

            var command = new CreateExpenseCommand {
                CategoryId = CategoryId,
                Date = new DateTime(2019, 1, 1),
                Amount = 10.2m,
                Description = "expense_description"
            };

            var saveAsync = A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>._))
                .Invokes(() => _category.Expenses.First().WithId(expectedExpenseId));

            int newExpenseId = await _commandHandler.Handle(command, default(CancellationToken));

            newExpenseId.Should().Be(expectedExpenseId);

            _category.Expenses.Select(it => new {
                it.Date,
                it.Amount,
                it.Description
            }).Should().BeEquivalentTo(new[] {
                new {
                    Date = new DateTime(2019, 1, 1),
                    Amount = 10.2m,
                    Description = "expense_description"
                }
            });

            saveAsync.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void CreateExpenseWithNotExistedCategory() {
            var saveAsync = A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>._));
            
            var command = new CreateExpenseCommand {
                CategoryId = 50,
                Date = new DateTime(2019, 1, 1),
                Amount = 10.2m,
                Description = "expense_description"
            };            

            Func<Task<int>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.CategoryNotFound);

            _category.Expenses.Should().BeEmpty();

            saveAsync.MustNotHaveHappened();
        }

        [Fact]
        public void CreateExpenseWithDefaultCategory() {
            const int defaultCategoryId = Constants.DefaultCategoryId;

            var defaultCategory = new Category("default_cagetory", null).WithId(defaultCategoryId);

            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(defaultCategoryId, default(CancellationToken)))
                .Returns(defaultCategory);

            var saveAsync = A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>._));

            var command = new CreateExpenseCommand {
                CategoryId = defaultCategoryId,
                Date = new DateTime(2019, 1, 1),
                Amount = 10.2m,
                Description = "expense_description"
            };

            Func<Task<int>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.DefaultCategoryUpdateOrDelete);

            _category.Expenses.Should().BeEmpty();

            saveAsync.MustNotHaveHappened();
        }
    }
}