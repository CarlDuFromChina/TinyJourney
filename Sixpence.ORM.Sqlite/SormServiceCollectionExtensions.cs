﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.ORM.Sqlite
{
    public static class SormServiceCollectionExtensions
    {
        public static ServiceCollectionOptions UseSqlite(this ServiceCollectionOptions options, string connectionString, int commandTimeout)
        {
            options.DbSetting = new DbSetting()
            {
                ConnectionString = connectionString,
                CommandTimeout = commandTimeout,
                Driver = new SqliteDriver()
            };

            return options;
        }
    }
}
