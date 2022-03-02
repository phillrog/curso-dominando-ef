using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Curso.Interceptadores
{
    public class InterceptadorDeComandos : DbCommandInterceptor
    {
        private static readonly Regex _tableRegex =
            new Regex(@"(?<tableAlias>FROM +(\[.*\]\.)?(\[.*\]) AS (\[.*\])(?! WITH \(NOLOCK\)))",
                RegexOptions.Multiline | 
                RegexOptions.IgnoreCase | 
                RegexOptions.Compiled);

        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command, 
            CommandEventData eventData, 
            InterceptionResult<DbDataReader> result)
        {
            UsarNoLock(command);

            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command, 
            CommandEventData eventData, 
            InterceptionResult<DbDataReader> result, 
            CancellationToken cancellationToken = default)
        {
            UsarNoLock(command);

            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }

        private static void UsarNoLock(DbCommand command)
        {
            if (!command.CommandText.Contains("WITH (NOLOCK)")
                && command.CommandText.StartsWith("-- Use NOLOCK"))
            {
                command.CommandText = _tableRegex.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
            }
        }
    }
}