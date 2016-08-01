﻿using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Abstratiion;
using VIC.DataAccess.PostgreSQL.Core;

namespace VIC.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service, DbConfig config)
        {
            return service.AddSingleton<IDbManager>(new PostgreSQLDbManager(config));
        }
    }
}