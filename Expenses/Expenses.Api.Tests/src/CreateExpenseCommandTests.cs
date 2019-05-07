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

namespace Expenses.Api.Tests {
    public class CreateExpenseCommandTests {
        public CreateExpenseCommandTests() {
            _fakeRepository = A.Fake<IExpenseRepository>(); 
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();

            A.CallTo(() => _fakeRepository.UnitOfWork).Returns(_fakeUnitOfWork); 

            _saveAsync = A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>._));

            _commandHandler =  new CreateExpenseCommandHandler(_fakeRepository);

            _category = new Category("category", null).WithId(CategoryId);

            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(CategoryId, default(CancellationToken)))
                .Returns(_category);

            _loadCategoryExpenses = A.CallTo(() => _fakeRepository.LoadExpensesAsync(_category, default(CancellationToken)));
            
        }

        const int CategoryId = 100;        

        readonly IExpenseRepository _fakeRepository;
        readonly IUnitOfWork _fakeUnitOfWork;
        readonly CreateExpenseCommandHandler _commandHandler;
        readonly IReturnValueArgumentValidationConfiguration<Task> _saveAsync;
        readonly Category _category;
        readonly IReturnValueArgumentValidationConfiguration<Task> _loadCategoryExpenses;

        [Fact]
        public async Task CreateExpenseTest() {
            var command = new CreateExpenseCommand {
                CategoryId = CategoryId,
                Date = new DateTime(2019, 1, 1),
                Amount = 10.2m,
                Description = "expense_description"
            };            

            bool success = await _commandHandler.Handle(command, default(CancellationToken));

            success.Should().BeTrue();

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

            _loadCategoryExpenses.MustHaveHappenedOnceExactly();
            _saveAsync.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void CreateExpenseWithNotExistedCategory() {
            var command = new CreateExpenseCommand {
                CategoryId = 50,
                Date = new DateTime(2019, 1, 1),
                Amount = 10.2m,
                Description = "expense_description"
            };

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.CategoryNotFound);

            _category.Expenses.Should().BeEmpty();

            _loadCategoryExpenses.MustNotHaveHappened();
            _saveAsync.MustNotHaveHappened();
        }

        [Fact]
        public void CreateExpenseWithDefaultCategory() {
            const int defaultCategoryId = Constants.DefaultCategoryId;

            var defaultCategory = new Category("default_cagetory", null).WithId(defaultCategoryId);

            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(defaultCategoryId, default(CancellationToken)))
                .Returns(defaultCategory);

            var command = new CreateExpenseCommand {
                CategoryId = defaultCategoryId,
                Date = new DateTime(2019, 1, 1),
                Amount = 10.2m,
                Description = "expense_description"
            };

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.DefaultCategoryUpdateOrDelete);

            _category.Expenses.Should().BeEmpty();

            _loadCategoryExpenses.MustNotHaveHappened();
            _saveAsync.MustNotHaveHappened();
        }
    }
}