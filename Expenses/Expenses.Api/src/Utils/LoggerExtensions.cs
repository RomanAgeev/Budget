using Microsoft.Extensions.Logging;

namespace Expenses.Api.Utils {
    public static class LoggerExtensions {
        public static void DebugSql(this ILogger logger, string sql) {
            logger.LogDebug($"Execute SQL {sql}");
        }
    }
}