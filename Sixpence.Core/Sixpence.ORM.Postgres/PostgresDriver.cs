using Npgsql;
using System.Data.Common;

namespace Sixpence.ORM.Postgres
{
    public class PostgresDriver : IDbDriver
    {
        public string Name => "Postgres";
        public IFieldMapping FieldMapping => new PostgresFieldMapping();
        public ISqlBuilder SqlBuilder => new PostgresSqlBuilder();
        public IDbOperator Operator => new PostgresOperator();

        public DbConnection GetDbConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
