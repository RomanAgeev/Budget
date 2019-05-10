using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Guards;
using Microsoft.Extensions.Logging;

namespace Expenses.Api.Queries {
    public class CategoryQueries : ICategoryQueries {
        public CategoryQueries(DbConnectionFactory connectionFactory, ILogger<CategoryQueries> logger) {
            Guard.NotNull(connectionFactory, nameof(connectionFactory));
            Guard.NotNull(logger, nameof(logger));

            _connectionFactory = connectionFactory;
            _logger = logger;
        }
        readonly DbConnectionFactory _connectionFactory;
        readonly ILogger<CategoryQueries> _logger;

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync() {
            _logger.LogInformation("Querying categories from storage");

            using(DbConnection connection = _connectionFactory()) {
                connection.Open();

                return await connection.QueryAsync<CategoryViewModel>(@"
                    SELECT Id, Name, Description
                    FROM Category");
            }
        }

        public async Task<CategoryViewModel> GetCategoryAsync(int categoryId) {
            Guard.NotZeroOrNegative(categoryId, nameof(categoryId));

            _logger.LogInformation("Querying category {categoryId} from storage", categoryId);

            using(DbConnection connection = _connectionFactory()) {
                connection.Open();

                var categories = await connection.QueryAsync<CategoryViewModel>($@"
                    SELECT Id, Name, Description
                    FROM Category
                    WHERE Id = {categoryId}");
                
                return categories.FirstOrDefault();
            }
        }
    }
}