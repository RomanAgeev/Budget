using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Expenses.Api.Utils;
using FluentValidation;
using Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Expenses.Api.Queries {
    public class GetCategoriesQuery : IRequest<IEnumerable<CategoryViewModel>> {
        public class Validator : AbstractValidator<GetCategoriesQuery> {
        }
    }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryViewModel>> {
        public GetCategoriesQueryHandler(DbConnectionFactory connectionFactory, ILogger<GetCategoriesQueryHandler> logger) {
            Guard.NotNull(connectionFactory, nameof(connectionFactory));
            Guard.NotNull(logger, nameof(logger));

            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        readonly DbConnectionFactory _connectionFactory;
        readonly ILogger<GetCategoriesQueryHandler> _logger;

        public async Task<IEnumerable<CategoryViewModel>> Handle(GetCategoriesQuery query, CancellationToken ct) {
            using(DbConnection connection = _connectionFactory()) {
                connection.Open();

                string sql = @"
                    SELECT Id, Name, Description
                    FROM Category";

                _logger.DebugSql(sql);

                return await connection.QueryAsync<CategoryViewModel>(sql);
            }
        }
    }
}