using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Curso.Interceptadores
{
    public class InterceptadorDeConexao : DbConnectionInterceptor
    {
        public override InterceptionResult ConnectionOpening(
            DbConnection connection, 
            ConnectionEventData eventData, 
            InterceptionResult result)
        {
            System.Console.WriteLine("Entrei no metodo ConnectionOpening");

            var connectionString = ((SqlConnection)connection).ConnectionString;

            System.Console.WriteLine(connectionString);

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                //DataSource="IP Segundo Servidor",
                ApplicationName = "CursoEFCore"
            };

            connection.ConnectionString = connectionStringBuilder.ToString();

            System.Console.WriteLine(connectionStringBuilder.ToString());

            return result;
        }
    }
}