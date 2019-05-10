using System;
using Microsoft.Extensions.Logging;

namespace Expenses.Api.Utils {
    public static class LoggerExtensions {
        public static void DebugSql(this ILogger logger, string sql) {
            logger.LogDebug($"Execute SQL {sql}");
        }

        public static void WarningException(this ILogger logger, Exception e, string cause) {
            logger.LogWarning(e, "Cause {cause}", cause);
        }
    }
}