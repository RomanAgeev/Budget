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

namespace Expenses.Api.Tests {
    public class DeleteCategoryCommandTests {
        public DeleteCategoryCommandTests() {
            _fakeRepository = A.Fake<IExpenseRepository>(); 
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();

            A.CallTo(() => _fakeRepository.UnitOfWork).Returns(_fakeUnitOfWork);

            _saveAsync = A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>._)); 

            _commandHandler = new DeleteCategoryCommandHandler(_fakeRepository);

            _defaultCategory = new Category("defaultCategory", null).WithId(DefaultCategoryId);
            _category = new Category("category", null).WithId(CategoryId);

            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(DefaultCategoryId, default(CancellationToken)))
                .Returns(_defaultCategory);
            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(CategoryId, default(CancellationToken)))
                .Returns(_category);

            _loadDefaultCategoryExpenses = A.CallTo(() => _fakeRepository.LoadExpensesAsync(_defaultCategory, default(CancellationToken)));
            _loadCategoryExpenses = A.CallTo(() => _fakeRepository.LoadExpensesAsync(_category, default(CancellationToken)));
            _removeCategory = A.CallTo(() => _fakeRepository.RemoveCategory(_category));
        }

        const int DefaultCategoryId = Constants.DefaultCategoryId;
        const int CategoryId = 100;

        readonly IExpenseRepository _fakeRepository;
        readonly IUnitOfWork _fakeUnitOfWork;
        readonly DeleteCategoryCommandHandler _commandHandler;
        readonly IReturnValueArgumentValidationConfiguration<Task> _saveAsync;
        readonly Category _defaultCategory;
        readonly Category _category;
        readonly IReturnValueArgumentValidationConfiguration<Task> _loadDefaultCategoryExpenses;
        readonly IReturnValueArgumentValidationConfiguration<Task> _loadCategoryExpenses;
        readonly IVoidArgumentValidationConfiguration _removeCategory;

        [Fact]
        public async Task DeleteCategoryTest() {
            Expense expense1 = _category.AddExpense(new DateTime(2019, 1, 1), 20.5m, null).WithId(1);
            Expense expense2 = _category.AddExpense(new DateTime(2019, 1, 2), 50, null).WithId(2);

            _category.Expenses.Should().BeEquivalentTo(new[] { expense1, expense2 });
            _defaultCategory.Expenses.Should().BeEmpty();

            var command = new DeleteCategoryCommand {
                CategoryId = CategoryId
            };

            bool success = await _commandHandler.Handle(command, default(CancellationToken));

            success.Should().BeTrue();

            _category.Expenses.Should().BeEmpty();
            _defaultCategory.Expenses.Should().BeEquivalentTo(new[] { expense1, expense2 });

            _loadDefaultCategoryExpenses.MustHaveHappenedOnceExactly();
            _loadCategoryExpenses.MustHaveHappenedOnceExactly();
            _removeCategory.MustHaveHappenedOnceExactly();
            _saveAsync.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void DeleteNotExistedCategoryTest() {
            var command = new DeleteCategoryCommand {
                CategoryId = 150
            };

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.CategoryNotFound);

            _loadDefaultCategoryExpenses.MustNotHaveHappened();
            _loadCategoryExpenses.MustNotHaveHappened();
            _removeCategory.MustNotHaveHappened();
            _saveAsync.MustNotHaveHappened();
        }

        [Fact]
        public void DeleteDefaultCategoryTest() {
            var command = new DeleteCategoryCommand {
                CategoryId = DefaultCategoryId
            };

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.DefaultCategoryUpdateOrDelete);

            _loadDefaultCategoryExpenses.MustNotHaveHappened();
            _loadCategoryExpenses.MustNotHaveHappened();
            _removeCategory.MustNotHaveHappened();
            _saveAsync.MustNotHaveHappened();
        }
    }
}