using System;
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
    public class UpdateExpenseCommandTests {
        public UpdateExpenseCommandTests() {
            _fakeRepository = A.Fake<IExpenseRepository>(); 
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();

            A.CallTo(() => _fakeRepository.UnitOfWork).Returns(_fakeUnitOfWork);

            _saveAsync = A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>._)); 

            _commandHandler = new UpdateExpenseCommandHandler(_fakeRepository);

            _fromCategory = new Category("from_category", null).WithId(FromCategoryId);
            _toCategory = new Category("to_category", null).WithId(ToCategoryId);
            _expense = _fromCategory.AddExpense(new DateTime(2019, 1, 1), 10.1m, null).WithId(5);
            
            A.CallTo(() => _fakeRepository.GetExpenseCategoryId(_expense))
                .Returns(FromCategoryId);
            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(FromCategoryId, default(CancellationToken)))
                .Returns(_fromCategory);
            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(ToCategoryId, default(CancellationToken)))
                .Returns(_toCategory);
            A.CallTo(() => _fakeRepository.GetExpenseByIdAsync(ExpenseId, default(CancellationToken)))
                .Returns(_expense);
            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(DefaultCategoryId, default(CancellationToken)))
                .Returns(new Category("default", null).WithId(DefaultCategoryId));
        }

        const int FromCategoryId = 100;
        const int ToCategoryId = 101;
        const int ExpenseId = 5;
        const int NotExistedExpenseId = 10;
        const int NotExistedCategoryId = 200;
        const int DefaultCategoryId = Constants.DefaultCategoryId;

        readonly IExpenseRepository _fakeRepository;
        readonly IUnitOfWork _fakeUnitOfWork;
        readonly UpdateExpenseCommandHandler _commandHandler;
        readonly IReturnValueArgumentValidationConfiguration<Task> _saveAsync;
        readonly Category _fromCategory;
        readonly Category _toCategory;
        readonly Expense _expense;

        [Fact]
        public async Task UpdateExpenseTest() {
            var command = new UpdateExpenseCommand {
                ExpenseId = ExpenseId,
                CategoryId = ToCategoryId,
                Amount = 20,
                Description = "new_description"
            };

            bool success = await _commandHandler.Handle(command, default(CancellationToken));

            success.Should().BeTrue();

            _fromCategory.Expenses.Should().BeEmpty();
            _toCategory.Expenses.Should().BeEquivalentTo(new[] { _expense });

            (_expense.Date, _expense.Amount, _expense.Description).Should().Be((new DateTime(2019, 1, 1), 20, "new_description"));

            _saveAsync.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateExpenseDoNotChangeCategoryTest() {
            var command = new UpdateExpenseCommand {
                ExpenseId = ExpenseId,
                CategoryId = FromCategoryId,
                Amount = 20,
                Description = "new_description"
            };

            bool success = await _commandHandler.Handle(command, default(CancellationToken));

            success.Should().BeTrue();

            _fromCategory.Expenses.Should().BeEquivalentTo(new[] { _expense });
            _toCategory.Expenses.Should().BeEmpty();

            (_expense.Date, _expense.Amount, _expense.Description).Should().Be((new DateTime(2019, 1, 1), 20, "new_description"));

            _saveAsync.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void UpdateNotExistedExpense() {
            var command = new UpdateExpenseCommand {
                ExpenseId = NotExistedExpenseId,
                CategoryId = ToCategoryId,
                Amount = 20,
                Description = "new_description"
            };

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.ExpenseNotFound);

            _fromCategory.Expenses.Should().BeEquivalentTo(new[] { _expense });
            _toCategory.Expenses.Should().BeEmpty();

            (_expense.Date, _expense.Amount, _expense.Description).Should().Be((new DateTime(2019, 1, 1), 10.1m, null));

            _saveAsync.MustNotHaveHappened();
        }

        [Fact]
        public void UpdateNotExistedFromCategory() {
            var command = new UpdateExpenseCommand {
                ExpenseId = ExpenseId,
                CategoryId = ToCategoryId,
                Amount = 20,
                Description = "new_description"
            };

            A.CallTo(() => _fakeRepository.GetExpenseCategoryId(_expense))
                .Returns(NotExistedCategoryId);

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.CategoryNotFound);

            _fromCategory.Expenses.Should().BeEquivalentTo(new[] { _expense });
            _toCategory.Expenses.Should().BeEmpty();

            (_expense.Date, _expense.Amount, _expense.Description).Should().Be((new DateTime(2019, 1, 1), 10.1m, null));

            _saveAsync.MustNotHaveHappened();
        }

        [Fact]
        public void UpdateNotExistedToCategory() {
            var command = new UpdateExpenseCommand {
                ExpenseId = ExpenseId,
                CategoryId = NotExistedCategoryId,
                Amount = 20,
                Description = "new_description"
            };

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.CategoryNotFound);

            _fromCategory.Expenses.Should().BeEquivalentTo(new[] { _expense });
            _toCategory.Expenses.Should().BeEmpty();

            (_expense.Date, _expense.Amount, _expense.Description).Should().Be((new DateTime(2019, 1, 1), 10.1m, null));

            _saveAsync.MustNotHaveHappened();
        }

        [Fact]
        public void MoveExpenseToDefaultCategoryTest() {
             var command = new UpdateExpenseCommand {
                ExpenseId = ExpenseId,
                CategoryId = DefaultCategoryId,
                Amount = 20,
                Description = "new_description"
            };

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.DefaultCategoryUpdateOrDelete);

            _fromCategory.Expenses.Should().BeEquivalentTo(new[] { _expense });
            _toCategory.Expenses.Should().BeEmpty();

            (_expense.Date, _expense.Amount, _expense.Description).Should().Be((new DateTime(2019, 1, 1), 10.1m, null));

            _saveAsync.MustNotHaveHappened();
        }
    }
}