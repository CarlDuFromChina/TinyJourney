﻿using Npgsql;
using System.Data.Common;

namespace Sixpence.ORM.Postgres
{
    public class PostgresDriver : IDbDriver
    {
        public string Name => "Postgres";
        public IFieldMapping FieldMapping => new PostgresFieldMapping();
        public IDbDialect Dialect => new PostgresDialect();
        public IDbBatch Batch => new PostgresBatch();

        public DbConnection GetDbConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
