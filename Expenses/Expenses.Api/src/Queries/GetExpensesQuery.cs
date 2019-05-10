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
    public class GetExpensesQuery : IRequest<IEnumerable<ExpenseViewModel>> {
        public class Validator : AbstractValidator<GetExpensesQuery> {
        }
    }

    public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, IEnumerable<ExpenseViewModel>> {
         public GetExpensesQueryHandler(DbConnectionFactory connectionFactory, ILogger<GetExpensesQueryHandler> logger) {
            Guard.NotNull(connectionFactory, nameof(connectionFactory));
            Guard.NotNull(logger, nameof(logger));

            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        readonly DbConnectionFactory _connectionFactory;
        readonly ILogger<GetExpensesQueryHandler> _logger;

        public async Task<IEnumerable<ExpenseViewModel>> Handle(GetExpensesQuery query, CancellationToken ct) {
            using(DbConnection connection = _connectionFactory()) {
                connection.Open();

                string sql = @"
                    SELECT e.Id, c.Id CategoryId, c.Name CategoryName, e.Description, e.Date, e.Amount
                    FROM Expense e
                    INNER JOIN Category c
                    ON e.CategoryId = c.Id";

                _logger.DebugSql(sql);

                return await connection.QueryAsync<ExpenseViewModel>(sql);
            }
        }
    }
}