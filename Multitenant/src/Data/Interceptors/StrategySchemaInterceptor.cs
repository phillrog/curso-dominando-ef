using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using src.Provider;

namespace src.Data.Interceptors
{
    public class StrategySchemaInterceptor : DbCommandInterceptor
    {
        private readonly TenantData _tenantData;

        public StrategySchemaInterceptor(TenantData tenantData)
        {
            _tenantData = tenantData;
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command, 
            CommandEventData eventData, 
            InterceptionResult<DbDataReader> result)
        {
            ReplaceSchema(command);
            
            return base.ReaderExecuting(command, eventData, result);
        }

        private void ReplaceSchema(DbCommand command)
        {
            // FROM PRODUCTS -> FROM [tenant-1].PRODUCTS
            command.CommandText = command.CommandText
                .Replace("FROM ", $" FROM [{_tenantData.TenantId}].")
                .Replace("JOIN ", $" JOIN [{_tenantData.TenantId}].");
        }
    }
}