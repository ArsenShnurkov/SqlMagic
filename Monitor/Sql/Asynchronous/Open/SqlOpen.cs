using SqlMagic.Monitor.Items.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlMagic.Monitor.Extensions;

namespace SqlMagic.Monitor.Sql
{

    /// <summary>
    /// A fully managed SQL Server class that takes care of handling connections.
    /// </summary>
    public sealed partial class Sql : IDisposable
    {

        #region OpenAsync

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithDataSet"/>
        public async Task<SqlResultWithDataSet> OpenAsync(String aSqlStatement)
        {
            return await OpenAsync(aSqlStatement, CommandType.Text);
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="System.Data.CommandType"/>
        public async Task<SqlResultWithDataSet> OpenAsync(String aSqlStatement,
            CommandType aCommandType)
        {
            return await OpenAsync(aSqlStatement, aCommandType, null);
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A collection of parameters that will be used by the statement</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="System.Data.CommandType" />
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        public async Task<SqlResultWithDataSet> OpenAsync(String aSqlStatement,
            CommandType aCommandType,
            params SqlParameter[] aParameters)
        {
            return await OpenAsync(aSqlStatement, aCommandType, aParameters == null ? null : aParameters.AsEnumerable());
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A collection of parameters that will be used by the statement</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="System.Data.CommandType" />
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public async Task<SqlResultWithDataSet> OpenAsync(String aSqlStatement,
            CommandType aCommandType,
            IEnumerable<SqlParameter> aParameters)
        {
            return await OpenAsync(new Statement
                {
                    Sql = aSqlStatement,
                    Type = aCommandType,
                    Parameters = aParameters
                }
            );
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to open</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        public async Task<SqlResultWithDataSet> OpenAsync(Statement aStatement)
        {
            return await OpenAsync(aStatement, await CreateConnectionAsync());
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to open</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        public async Task<SqlResultWithDataSet> OpenAsync(Statement aStatement,
            SqlConnection aConnection)
        {
            return await OpenAsync(aStatement, aConnection, true);
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to open</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">A boolean flag that indicates whether or not the
        /// connection should be closed after the statement is executed</param>
        /// /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        public async Task<SqlResultWithDataSet> OpenAsync(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection)
        {
            return await OpenAsync(aStatement, aConnection, aCloseConnection, null);
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to open</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">A boolean flag that indicates whether or not the
        /// connection should be closed after the statement is executed</param>
        /// <param name="aTransaction">The current transaction that this command should be executed on</param>
        /// /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public async Task<SqlResultWithDataSet> OpenAsync(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection,
            SqlTransaction aTransaction)
        {
            return await Task.Factory.StartNew<SqlResultWithDataSet>(() => Open(aStatement, aConnection, aCloseConnection, aTransaction));
        }

        #endregion OpenAsync

    }

}
