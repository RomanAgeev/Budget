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
    public class GetExpenseQuery : IRequest<ExpenseViewModel> {
        public class Validator : AbstractValidator<GetExpenseQuery> {
            public Validator() =>
                RuleFor(it => it.ExpenseId)
                    .GreaterThan(0);
        }

        public GetExpenseQuery(int expenseId) {
            ExpenseId = expenseId;
        }

        public int ExpenseId { get; private set; }
    }

    public class GetExpenseQueryHandler : IRequestHandler<GetExpenseQuery, ExpenseViewModel> {
        public GetExpenseQueryHandler(DbConnectionFactory connectionFactory, ILogger<GetExpenseQueryHandler> logger) {
            Guard.NotNull(connectionFactory, nameof(connectionFactory));
            Guard.NotNull(logger, nameof(logger));

            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        readonly DbConnectionFactory _connectionFactory;
        readonly ILogger<GetExpenseQueryHandler> _logger;

        public async Task<ExpenseViewModel> Handle(GetExpenseQuery query, CancellationToken ct) {
            using(DbConnection connection = _connectionFactory()) {
                connection.Open();

                string sql = $@"
                    SELECT e.Id, c.Id CategoryId, c.Name CategoryName, e.Description, e.Date, e.Amount
                    FROM Expense e
                    INNER JOIN Category c
                    ON e.CategoryId = c.Id
                    WHERE e.Id = {query.ExpenseId}";

                _logger.DebugSql(sql);

                var expenses = await connection.QueryAsync<ExpenseViewModel>(sql);
                
                return expenses.FirstOrDefault();
            }
        }
    }
}