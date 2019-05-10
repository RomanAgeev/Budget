using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Expenses.Api.Utils;
using FluentValidation;
using Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Expenses.Api.Queries {
    public class GetCategoryQuery : IRequest<CategoryViewModel> {
        public class Validator : AbstractValidator<GetCategoryQuery> {
            public Validator() =>
                RuleFor(it => it.CategoryId)
                    .GreaterThan(0);
        }

        public GetCategoryQuery(int categoryId) {
            CategoryId = categoryId;
        }

        public int CategoryId { get; private set; }
    }

    public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, CategoryViewModel> {
        public GetCategoryQueryHandler(DbConnectionFactory connectionFactory, ILogger<GetCategoryQueryHandler> logger) {
            Guard.NotNull(connectionFactory, nameof(connectionFactory));
            Guard.NotNull(logger, nameof(logger));

            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        readonly DbConnectionFactory _connectionFactory;
        readonly ILogger<GetCategoryQueryHandler> _logger;

         public async Task<CategoryViewModel> Handle(GetCategoryQuery query, CancellationToken ct) {
            using(DbConnection connection = _connectionFactory()) {
                connection.Open();

                string sql = $@"
                    SELECT Id, Name, Description
                    FROM Category
                    WHERE Id = {query.CategoryId}";

                _logger.DebugSql(sql);

                var categories = await connection.QueryAsync<CategoryViewModel>(sql);
                
                return categories.FirstOrDefault();
            }
         }
    }
}