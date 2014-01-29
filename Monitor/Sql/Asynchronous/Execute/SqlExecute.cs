using SqlMagic.Monitor.Items.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlMagic.Monitor.Sql
{

    /// <summary>
    /// A fully managed SQL Server class that takes care of handling connections.
    /// </summary>
    public sealed partial class Sql : IDisposable
    {
        
        #region ExecuteAsync

        /// <summary>
        /// Performs an asynchronous request against a database
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResult"/>
        public async Task<SqlResult> ExecuteAsync(String aSqlStatement)
        {
            return await ExecuteAsync(aSqlStatement, CommandType.Text);
        }

        /// <summary>
        /// Performs an asynchronous request against a database
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="System.Data.CommandType" />
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResult"/>
        public async Task<SqlResult> ExecuteAsync(String aSqlStatement,
            CommandType aCommandType)
        {
            return await ExecuteAsync(aSqlStatement, aCommandType, null);
        }

        /// <summary>
        /// Performs an asynchronous request against a database
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A collection of parameters that will be used by the statement</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResult"/>
        /// <seealso cref="System.Data.CommandType" />
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        public async Task<SqlResult> ExecuteAsync(String aSqlStatement,
            CommandType aCommandType,
            params SqlParameter[] aParameters)
        {
            return await ExecuteAsync(aSqlStatement, aCommandType, aParameters.AsEnumerable());
        }

        /// <summary>
        /// Performs an asynchronous request against a database
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A collection of parameters that will be used by the statement</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResult"/>
        /// <seealso cref="System.Data.CommandType" />
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public async Task<SqlResult> ExecuteAsync(String aSqlStatement,
            CommandType aCommandType,
            IEnumerable<SqlParameter> aParameters)
        {
            return await ExecuteAsync
            (
                new Statement { Sql = aSqlStatement, Type = aCommandType, Parameters = aParameters }
            );
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResult"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        public async Task<SqlResult> ExecuteAsync(Statement aStatement)
        {
            return await ExecuteAsync
            (
                aStatement,
                await CreateConnectionAsync()
            );
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResult"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public async Task<SqlResult> ExecuteAsync(Statement aStatement,
            SqlConnection aConnection)
        {
            return await ExecuteAsync(aStatement, aConnection, true);
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">A boolean flag that indicates whether or not the
        /// connection should be closed after the statement is executed</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResult"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public async Task<SqlResult> ExecuteAsync(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection)
        {
            SqlResult oResult = new SqlResult();
            oResult.Exception = (await ExecuteAsync<Object>(aStatement, aConnection, aCloseConnection)).Exception;
            return oResult;
        }

        /// <summary>
        /// Performs an asynchronous request against a database.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">A boolean flag that indicates whether or not the
        /// connection should be closed after the statement is executed</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <param name="aTransaction">The current transaction that this command should be executed on</param>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResult"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        /// <seealso cref="System.Data.SqlClient.SqlTransaction"/>
        public async Task<SqlResult> ExecuteAsync(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection,
            SqlTransaction aTransaction)
        {
            SqlResult oResult = new SqlResult();
            oResult.Exception = (await ExecuteAsync<Object>(aStatement, aConnection, aCloseConnection, aTransaction)).Exception;
            return oResult;
        }

        #endregion ExecuteAsync

        #region ExecuteAsync<T>

        /// <summary>
        /// Performs an asynchronous query with a single return value against a database
        /// </summary>
        /// <typeparam name="T">The return type expected as a result of the query</typeparam>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithValue{T}"/>
        public async Task<SqlResultWithValue<T>> ExecuteAsync<T>(String aSqlStatement)
        {
            return await ExecuteAsync<T>(aSqlStatement, CommandType.Text);
        }

        /// <summary>
        /// Performs an asynchronous query with a single return value against a database
        /// </summary>
        /// <typeparam name="T">The return type expected as a result of the query</typeparam>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithValue{T}"/>
        /// <seealso cref="System.Data.CommandType"/>
        public async Task<SqlResultWithValue<T>> ExecuteAsync<T>(String aSqlStatement,
            CommandType aCommandType)
        {
            return await ExecuteAsync<T>(aSqlStatement, aCommandType, null);
        }

        /// <summary>
        /// Performs an asynchronous query with a single return value against a database
        /// </summary>
        /// <typeparam name="T">The return type expected as a result of the query</typeparam>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A collection of parameters that will be used by the statement</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithValue{T}"/>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        public async Task<SqlResultWithValue<T>> ExecuteAsync<T>(String aSqlStatement,
            CommandType aCommandType,
            params SqlParameter[] aParameters)
        {
            return await ExecuteAsync<T>(aSqlStatement, aCommandType, aParameters.AsEnumerable());
        }

        /// <summary>
        /// Performs an asynchronous query with a single return value against a database
        /// </summary>
        /// <typeparam name="T">The return type expected as a result of the query</typeparam>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A collection of parameters that will be used by the statement</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithValue{T}"/>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public async Task<SqlResultWithValue<T>> ExecuteAsync<T>(String aSqlStatement,
            CommandType aCommandType,
            IEnumerable<SqlParameter> aParameters)
        {
            return await ExecuteAsync<T>
            (
                new Statement { Sql = aSqlStatement, Type = aCommandType, Parameters = aParameters }
            );
        }

        /// <summary>
        /// Performs an asynchronous query with a single return value against a database
        /// </summary>
        /// <typeparam name="T">The return type expected as a result of the query</typeparam>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithValue{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        public async Task<SqlResultWithValue<T>> ExecuteAsync<T>(Statement aStatement)
        {
            return await ExecuteAsync<T>
            (
                aStatement,
                await CreateConnectionAsync()
            );
        }

        /// <summary>
        /// Performs an asynchronous query with a single return value against a database
        /// </summary>
        /// <typeparam name="T">The return type expected as a result of the query</typeparam>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithValue{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public async Task<SqlResultWithValue<T>> ExecuteAsync<T>(Statement aStatement,
            SqlConnection aConnection)
        {
            return await ExecuteAsync<T>(aStatement, aConnection, true);
        }

        /// <summary>
        /// Performs an asynchronous query with a single return value against a database
        /// </summary>
        /// <typeparam name="T">The return type expected as a result of the query</typeparam>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">A boolean flag that indicates whether or not the
        /// connection should be closed after the statement is executed</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithValue{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public async Task<SqlResultWithValue<T>> ExecuteAsync<T>(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection)
        {
            return await ExecuteAsync<T>(aStatement, aConnection, aCloseConnection, null);
        }

        /// <summary>
        /// Performs an asynchronous query with a single return value against a database
        /// </summary>
        /// <typeparam name="T">The return type expected as a result of the query</typeparam>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">A boolean flag that indicates whether or not the
        /// connection should be closed after the statement is executed</param>
        /// <param name="aTransaction">The current transaction that this command should be executed on</param>
        /// <returns>A Task object that represents the asynchronous request</returns>
        /// <seealso cref="System.Threading.Tasks.Task{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Items.Results.SqlResultWithValue{T}"/>
        /// <seealso cref="SqlMagic.Monitor.Sql.Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        /// <seealso cref="System.Data.SqlClient.SqlTransaction"/>
        public async Task<SqlResultWithValue<T>> ExecuteAsync<T>(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection,
            SqlTransaction aTransaction)
        {

            // Make a call to OpenAsync to return a dataset.
            SqlResultWithDataSet oDataSet = await OpenAsync(aStatement, aConnection, aCloseConnection, aTransaction);

            // Create a result w/ value opbject
            SqlResultWithValue<T> oResult = new SqlResultWithValue<T>();

            // Assign stuff manually
            oResult.Exception = oDataSet.Exception;
            oResult.Statistics = oDataSet.Statistics;

            // Check for success+actual results
            if (oDataSet.Success && oDataSet.Results.Tables[0].Rows.Count > 0)
                oResult.Value = (T)Convert.ChangeType(oDataSet.Results.Tables[0].Rows[0][0], typeof(T));

            // All done
            return oResult;

        }

        #endregion ExecuteAsync<T>

    }
}
