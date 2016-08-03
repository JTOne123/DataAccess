using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace VIC.DataAccess.Abstraction
{
    public interface IDataCommand : IDisposable
    {
        string ConnectionString { get; set; }

        string Text { get; set; }

        int Timeout { get; set; }

        CommandType Type { get; set; }

        Task<List<T>> ExecuteEntityListAsync<T>(dynamic paramter = null);

        Task<List<T>> ExecuteEntityListAsync<T>(CancellationToken cancellationToken, dynamic paramter = null);

        Task<T> ExecuteEntityAsync<T>(dynamic parameter = null);

        Task<T> ExecuteEntityAsync<T>(CancellationToken cancellationToken, dynamic paramter = null);

        Task<T> ExecuteScalarAsync<T>(dynamic parameter = null);

        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, dynamic parameter = null);

        Task<DbDataReader> ExecuteDataReaderAsync(dynamic parameter = null, CommandBehavior behavior = CommandBehavior.Default);

        Task<DbDataReader> ExecuteDataReaderAsync(CancellationToken cancellationToken, dynamic parameter = null, CommandBehavior behavior = CommandBehavior.Default);

        Task<IMultipleReader> ExecuteMultipleAsync(dynamic parameter = null);

        Task<IMultipleReader> ExecuteMultipleAsync(CancellationToken cancellationToken, dynamic parameter = null);

        Task<int> ExecuteNonQueryAsync(dynamic parameter = null);

        Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken, dynamic parameter = null);

        IDbTransaction BeginTransaction(IsolationLevel level = IsolationLevel.ReadUncommitted);
    }
}