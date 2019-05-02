using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Guards;

namespace Expenses.Api.Queries {
    public class CategoryQueries : ICategoryQueries {
        public CategoryQueries(DbConnectionFactory connectionFactory) {
            Guard.NotNull(connectionFactory, nameof(connectionFactory));

            _connectionFactory = connectionFactory;
        }
        readonly DbConnectionFactory _connectionFactory;

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync() {
            using(DbConnection connection = _connectionFactory()) {
                connection.Open();

                return await connection.QueryAsync<CategoryViewModel>(@"
                    SELECT Name, Description
                    FROM Category");
            }
        }
    }
}