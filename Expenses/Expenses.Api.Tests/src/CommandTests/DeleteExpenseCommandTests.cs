using System;
using System.Threading;
using System.Threading.Tasks;
using Expenses.Api.Commands;
using Expenses.Domain;
using Expenses.Domain.Models;
using FakeItEasy;
using FakeItEasy.Configuration;
using FluentAssertions;
using Xunit;

namespace Expenses.Api.Tests.CommandTests {
    public class DeleteExpenseCommandTests {
        public DeleteExpenseCommandTests() {
            _fakeRepository = A.Fake<IExpenseRepository>(); 
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();

            A.CallTo(() => _fakeRepository.UnitOfWork).Returns(_fakeUnitOfWork);

            _saveAsync = A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>._)); 

            _commandHandler = new DeleteExpenseCommandHandler(_fakeRepository);

            _expense = new Expense(new DateTime(2019, 1, 1), 11.4m, null).WithId(ExpenseId);

            A.CallTo(() => _fakeRepository.GetExpenseByIdAsync(ExpenseId, default(CancellationToken)))
                .Returns(_expense);

            _removeExpense = A.CallTo(() => _fakeRepository.RemoveExpense(_expense));
        }

        const int ExpenseId = 5;
        const int NotExistedExpenseId = 10;

        readonly IExpenseRepository _fakeRepository;
        readonly IUnitOfWork _fakeUnitOfWork;
        readonly DeleteExpenseCommandHandler _commandHandler;
        readonly IReturnValueArgumentValidationConfiguration<Task> _saveAsync;
        readonly Expense _expense;
        readonly IVoidArgumentValidationConfiguration _removeExpense;

        [Fact]
        public async Task DeleteExpenseTest() {
            var command = new DeleteExpenseCommand {
                ExpenseId = ExpenseId
            };

            bool success = await _commandHandler.Handle(command, default(CancellationToken));

            success.Should().BeTrue();

            _removeExpense.MustHaveHappenedOnceExactly();
            _saveAsync.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void DeleteNotExistedExpenseTest() {
            var command = new DeleteExpenseCommand {
                ExpenseId = NotExistedExpenseId
            };

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.ExpenseNotFound);

            _removeExpense.MustNotHaveHappened();
            _saveAsync.MustNotHaveHappened();
        }
    }
}