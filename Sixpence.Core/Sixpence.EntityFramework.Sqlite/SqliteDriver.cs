using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework.Sqlite
{
    public class SqliteDriver : IDbDriver
    {
        public string Name => "Sqlite";

        public IFieldMapping FieldMapping => new SqliteFieldMapping();

        public ISqlBuilder SqlBuilder => new SqliteSqlBuilder();

        public IDbOperator Operator => new SqliteOperator();

        public DbConnection GetDbConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}
