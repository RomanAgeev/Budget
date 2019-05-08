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

namespace Expenses.Api.Tests {
    public class CreateCategoryCommandTests {
        public CreateCategoryCommandTests() {
            _fakeRepository = A.Fake<IExpenseRepository>(); 
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();

            A.CallTo(() => _fakeRepository.UnitOfWork).Returns(_fakeUnitOfWork);            

            _commandHandler =  new CreateCategoryCommandHandler(_fakeRepository);
        }

        readonly IExpenseRepository _fakeRepository;
        readonly IUnitOfWork _fakeUnitOfWork;
        readonly CreateCategoryCommandHandler _commandHandler;

        [Fact]
        public async Task CreateCategoryTest() {
            const string expectedName = "category_3";
            const string expectedDescription = "category_3_description";
            const int expectedCategoryId = 111;

            Category newCategory = null;

            A.CallTo(() => _fakeRepository.GetCategoryByNameAsync(expectedName, default(CancellationToken)))
                .Returns<Category>(null);

            var addCategory = A.CallTo(() => _fakeRepository.AddCategory(A<Category>.That
                .Matches((Category category) => new {
                    category.Name,
                    category.Description
                }.Equals(new {
                    Name = expectedName,
                    Description = expectedDescription
                }))))
                .Invokes((Category category) => newCategory = category);                

            var saveAsync = A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>._))
                .Invokes(() => newCategory.WithId(expectedCategoryId));

            var command = new CreateCategoryCommand {
                Name = expectedName,
                Description = expectedDescription
            };

            int newCategoryId = await _commandHandler.Handle(command, default(CancellationToken));

            newCategoryId.Should().Be(expectedCategoryId);

            addCategory.MustHaveHappenedOnceExactly();
            saveAsync.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void CreateDublicatedNameCategory() {
            const string expectedName = "category";

            A.CallTo(() => _fakeRepository.GetCategoryByNameAsync(expectedName, default(CancellationToken)))
                .Returns(new Category(expectedName, null));

            var addCategory = A.CallTo(() => _fakeRepository.AddCategory(A<Category>._));
            var saveAsync = A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>._));

            var command = new CreateCategoryCommand {
                Name = expectedName,
                Description = null
            };

            Func<Task<int>> handle = async () => await _commandHandler.Handle(command, default(CancellationToken));

            handle.Should().Throw<DomainException>()
                .Which.Cause.Should().Be(DomainExceptionCause.DuplicatedCategoryName);

            addCategory.MustNotHaveHappened();
            saveAsync.MustNotHaveHappened();
        }
    }
}