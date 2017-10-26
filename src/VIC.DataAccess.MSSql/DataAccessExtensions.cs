﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;
using VIC.DataAccess.MSSql.Core;
using VIC.DataAccess.MSSql.Core.Converter;

namespace VIC.DataAccess.MSSql
{
    public static partial class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service)
        {
            return service.AddSingleton<IDbFuncNameConverter, DbFuncNameConverter>()
                .AddSingleton<IDbTypeConverter, DbTypeConverter>()
                .AddSingleton<IScalarConverter, ScalarConverter>()
                .AddSingleton<IEntityConverter, EntityConverter>()
                .AddSingleton<IParamConverter, MSSqlParamConverter>()
                .AddTransient<IDataCommand, MSSqlDataCommand>();
        }
    }
}