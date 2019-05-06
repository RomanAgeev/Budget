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
    public class UpdateCategoryCommandTests {
        public UpdateCategoryCommandTests() {
            _fakeRepository = A.Fake<IExpenseRepository>(); 
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();

            A.CallTo(() => _fakeRepository.UnitOfWork).Returns(_fakeUnitOfWork);

            _saveAsync = A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>._)); 

            _commandHandler = new UpdateCategoryCommandHandler(_fakeRepository);
        }

        readonly IExpenseRepository _fakeRepository;
        readonly IUnitOfWork _fakeUnitOfWork;
        readonly UpdateCategoryCommandHandler _commandHandler;
        readonly IReturnValueArgumentValidationConfiguration<Task> _saveAsync;

        [Fact]
        public async Task UpdateCategoryTest() {
            const int categoryId = 100;
            const string expectedName = "new_category";
            const string expectedDescription = "new_category_description";

            var category = new Category("category", "category_description");

            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(categoryId, default(CancellationToken)))
                .Returns(category);

            var command = new UpdateCategoryCommand {
                CategoryId = categoryId,
                Name = expectedName,
                Description = expectedDescription
            };

            bool success = await _commandHandler.Handle(command, default(CancellationToken));

            success.Should().BeTrue();

            category.Name.Should().Be(expectedName);
            category.Description.Should().Be(expectedDescription);

            _saveAsync.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void UpdateNotExistedCategoryTest() {
            const int categoryId = 100;

            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(categoryId, default(CancellationToken)))
                .Returns<Category>(null);

            var command = new UpdateCategoryCommand {
                CategoryId = categoryId,
                Name = "new_name",
                Description = null
            };

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.CategoryNotFound);

            _saveAsync.MustNotHaveHappened();
        }

        [Fact]
        public void UpdateDefaultCategoryTest() {
            const int defaultCategoryId = Constants.DefaultCategoryId;

            var defaultCategory = new Category("category", "category_description").WithId(defaultCategoryId);

            A.CallTo(() => _fakeRepository.GetCategoryByIdAsync(defaultCategoryId, default(CancellationToken)))
                .Returns(defaultCategory);

            var command = new UpdateCategoryCommand {
                CategoryId = defaultCategoryId,
                Name = "new_name",
                Description = null
            };

            Func<Task<bool>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.DefaultCategoryUpdateOrDelete);

            _saveAsync.MustNotHaveHappened();
        }
    }
}