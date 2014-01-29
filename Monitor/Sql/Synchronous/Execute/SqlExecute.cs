using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlMagic.Monitor.Extensions;
using System.Data;
using System.Data.SqlClient;
using SqlMagic.Monitor.Items.Results;
using SqlMagic.Monitor.Items.Logging;

namespace SqlMagic.Monitor.Sql
{

    /// <summary>
    /// A fully managed SQL Server class that takes care of handling connections.
    /// </summary>
    public sealed partial class Sql : IDisposable
    {

        #region Execute

        /// <summary>
        /// Executes a SQL statement.
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        public SqlResult Execute(String aSqlStatement)
        {
            return Execute(aSqlStatement, CommandType.Text);
        }

        /// <summary>
        /// Executes a SQL statement.
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <seealso cref="System.Data.CommandType"/>
        public SqlResult Execute(String aSqlStatement,
            CommandType aCommandType)
        {
            return Execute(aSqlStatement, aCommandType, null);
        }

        /// <summary>
        /// Executes a SQL statement.
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public SqlResult Execute(String aSqlStatement,
            CommandType aCommandType,
            params SqlParameter[] aParameters)
        {
            return Execute(aSqlStatement, aCommandType, aParameters == null ? null : aParameters.AsEnumerable());
        }

        /// <summary>
        /// Executes a SQL statement.
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public SqlResult Execute(String aSqlStatement,
            CommandType aCommandType,
            IEnumerable<SqlParameter> aParameters)
        {
            return Execute(new Statement { Sql = aSqlStatement, Type = aCommandType, Parameters = aParameters });
        }

        /// <summary>
        /// Executes a SQL statement.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <seealso cref="Sql.Statement"/>
        public SqlResult Execute(Statement aStatement)
        {
            return Execute(aStatement, CreateConnection());
        }

        /// <summary>
        /// Executes a SQL statement.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public SqlResult Execute(Statement aStatement, SqlConnection aConnection)
        {
            // Execute<Object>(aStatement, aConnection);
            return Execute(aStatement, aConnection, true);
        }

        /// <summary>
        /// Executes a SQL statement.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">If true, the connection will be closed after execution</param>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public SqlResult Execute(Statement aStatement, SqlConnection aConnection, Boolean aCloseConnection)
        {
            // Execute<Object>(aStatement, aConnection);
            return Execute(aStatement, aConnection, aCloseConnection, null);
        }

        /// <summary>
        /// Executes a SQL statement.
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">If true, the connection will be closed after execution</param>
        /// <param name="aTransaction">The SqlTransaction object to use during execution</param>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public SqlResult Execute(Statement aStatement, SqlConnection aConnection, Boolean aCloseConnection, SqlTransaction aTransaction)
        {
            // Execute<Object>(aStatement, aConnection);
            return new SqlResult { Exception = null, Statistics = Execute<Object>(aStatement, aConnection, aCloseConnection, aTransaction).Statistics };
        }

        #endregion Execute

        #region Execute<T>

        /// <summary>
        /// Executes a SQL statement and returns the value of the statement
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <returns>An object of type <typeparam name="T">T</typeparam></returns>
        public SqlResultWithValue<T> Execute<T>(String aSqlStatement)
        {
            return Execute<T>(aSqlStatement, CommandType.Text);
        }

        /// <summary>
        /// Executes a SQL statement and returns the value of the statement
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <returns>An object of type <typeparam name="T">T</typeparam></returns>
        /// <seealso cref="System.Data.CommandType"/>
        public SqlResultWithValue<T> Execute<T>(String aSqlStatement,
            CommandType aCommandType)
        {
            return Execute<T>(aSqlStatement, aCommandType, null);
        }

        /// <summary>
        /// Executes a SQL statement and returns the value of the statement
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <returns>An object of type <typeparam name="T">T</typeparam></returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public SqlResultWithValue<T> Execute<T>(String aSqlStatement,
            CommandType aCommandType,
            params SqlParameter[] aParameters)
        {
            return Execute<T>(aSqlStatement, aCommandType, aParameters == null ? null : aParameters.AsEnumerable());
        }

        /// <summary>
        /// Executes a SQL statement and returns the value of the statement
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <returns>An object of type <typeparam name="T">T</typeparam></returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public SqlResultWithValue<T> Execute<T>(String aSqlStatement,
            CommandType aCommandType,
            IEnumerable<SqlParameter> aParameters)
        {
            return Execute<T>(new Statement { Sql = aSqlStatement, Type = aCommandType, Parameters = aParameters });
        }

        /// <summary>
        /// Executes a SQL statement and returns the value of the statement
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <returns>An object of type <typeparam name="T">T</typeparam></returns>
        /// <seealso cref="Sql.Statement"/>
        public SqlResultWithValue<T> Execute<T>(Statement aStatement)
        {
            return Execute<T>(aStatement, CreateConnection());
        }

        /// <summary>
        /// Executes a SQL statement and returns the value of the statement
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <returns>An object of type <typeparam name="T">T</typeparam></returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public SqlResultWithValue<T> Execute<T>(Statement aStatement, SqlConnection aConnection)
        {
            return Execute<T>(aStatement, aConnection, true);
        }

        /// <summary>
        /// Executes a SQL statement and returns the value of the statement
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">If true, the connection will be closed after execution</param>
        /// <returns>An object of type <typeparam name="T">T</typeparam></returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public SqlResultWithValue<T> Execute<T>(Statement aStatement, SqlConnection aConnection, Boolean aCloseConnection)
        {
            return Execute<T>(aStatement, aConnection, aCloseConnection, null);
        }

        /// <summary>
        /// Executes a SQL statement and returns the value of the statement
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">If true, the connection will be closed after execution</param>
        /// <param name="aTransaction">The SqlTransaction object to use during execution</param>
        /// <returns>An object of type <typeparam name="T">T</typeparam></returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public SqlResultWithValue<T> Execute<T>(Statement aStatement, SqlConnection aConnection, Boolean aCloseConnection, SqlTransaction aTransaction)
        {

            // Call .Open() and return the first row of the first column.
            SqlResultWithDataSet oResults = Open(aStatement, aConnection, aCloseConnection, aTransaction);
            DataTable oTable = oResults.Results.Tables[0]; // Open(aStatement).Tables[0];

            // Create our T variable
            T oRetval = default(T);

            // Now that we have the table, grab the first row and first value
            if (oTable.Rows.Count > 0 && oTable.Rows[0].ItemArray.Any())
                oRetval = (T)Convert.ChangeType(oTable.Rows[0][0], typeof(T));

            // Return!
            return new SqlResultWithValue<T>() { Exception = null, Value = oRetval, Statistics = oResults.Statistics };

        }

        #endregion Execute<T>

        #region TryExecute

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully.
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aResult">The results of the Sql execution</param>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="Items.Results.SqlResultBase"/>
        public Boolean TryExecute(String aSqlStatement,
            out SqlResult aResult)
        {
            return TryExecute(aSqlStatement, CommandType.Text, out aResult);
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aResult">The results of the Sql execution</param>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="Items.Results.SqlResultBase"/>
        public Boolean TryExecute(String aSqlStatement,
            CommandType aCommandType,
            out SqlResult aResult)
        {
            return TryExecute(aSqlStatement, aCommandType, null, out aResult);
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <param name="aResult">The results of the Sql execution</param>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="Items.Results.SqlResultBase"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public Boolean TryExecute(String aSqlStatement,
            CommandType aCommandType,
            out SqlResult aResult,
            params SqlParameter[] aParameters)
        {

            // Perform the try execute
            return TryExecute
            (
                aSqlStatement,
                aCommandType,
                aParameters == null ? null : aParameters.AsEnumerable(),
                out aResult
            );

        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <param name="aResult">The results of the Sql execution</param>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="Items.Results.SqlResultBase"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public Boolean TryExecute(String aSqlStatement,
            CommandType aCommandType,
            IEnumerable<SqlParameter> aParameters,
            out SqlResult aResult)
        {

            // Perform the try execute
            return TryExecute
            (
                new Statement
                {
                    Sql = aSqlStatement,
                    Type = aCommandType,
                    Parameters = aParameters
                },
                out aResult
            );

        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aResult">The results of the Sql execution</param>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="Items.Results.SqlResultBase"/>
        public Boolean TryExecute(Statement aStatement,
            out SqlResult aResult)
        {
            return TryExecute
            (
                aStatement,
                CreateConnection(),
                out aResult
            );
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aResult">The results of the Sql execution</param>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        /// <seealso cref="Items.Results.SqlResultBase"/>
        public Boolean TryExecute(Statement aStatement,
            SqlConnection aConnection,
            out SqlResult aResult)
        {
            return TryExecute(aStatement, aConnection, true, out aResult);
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aResult">The results of the Sql execution</param>
        /// <param name="aCloseConnection">If true, closes the connection after execution</param>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        /// <seealso cref="Items.Results.SqlResultBase"/>
        public Boolean TryExecute(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection,
            out SqlResult aResult)
        {
            return TryExecute(aStatement, aConnection, aCloseConnection, null, out aResult);
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aResult">The results of the Sql execution</param>
        /// <param name="aCloseConnection">If true, closes the connection after execution</param>
        /// <param name="aTransaction">The SqlTransaction object to use during execution</param>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        /// <seealso cref="Items.Results.SqlResultBase"/>
        public Boolean TryExecute(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection,
            SqlTransaction aTransaction,
            out SqlResult aResult)
        {

            // Dummy variable to hold the result
            bool oSqlResult = false;
            Items.Results.SqlResultWithValue<object> oResult = null;
            aResult = new SqlResult();

            // Perform the TryExecute<T>
            oSqlResult = TryExecute<object>(aStatement, aConnection, aCloseConnection, aTransaction, out oResult);

            // Assign the exception result
            aResult.Exception = oResult.Exception;
            aResult.Statistics = oResult.Statistics;

            // Leave
            return oSqlResult;

        }

        #endregion TryExecute

        #region TryExecute<T>

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aResult">The resulting structure that contains the return value
        /// and an exception object if the SQL statement failed to execute properly</param>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <typeparam name="T">The data type the return value should be</typeparam>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="Items.Results.SqlResultWithValue{T}"/>
        public Boolean TryExecute<T>(String aSqlStatement,
            out SqlResultWithValue<T> aResult)
        {
            return TryExecute<T>(aSqlStatement, CommandType.Text, out aResult);
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aResult">The resulting structure that contains the return value
        /// and an exception object if the SQL statement failed to execute properly</param>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>        
        /// <typeparam name="T">The data type the return value should be</typeparam>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="Items.Results.SqlResultWithValue{T}"/>
        public Boolean TryExecute<T>(String aSqlStatement,
            CommandType aCommandType,
            out SqlResultWithValue<T> aResult)
        {
            return TryExecute<T>(aSqlStatement, aCommandType, null, out aResult);
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value contains whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aResult">The resulting structure that contains the return value
        /// and an exception object if the SQL statement failed to execute properly</param>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <typeparam name="T">The data type the return value should be</typeparam>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        /// <seealso cref="Items.Results.SqlResultWithValue{T}"/>
        public Boolean TryExecute<T>(String aSqlStatement,
            CommandType aCommandType,
            out SqlResultWithValue<T> aResult,
            params SqlParameter[] aParameters)
        {
            return TryExecute<T>
            (
                aSqlStatement,
                aCommandType,
                aParameters == null ? null : aParameters.AsEnumerable(),
                out aResult
            );
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value contains whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aResult">The resulting structure that contains the return value
        /// and an exception object if the SQL statement failed to execute properly</param>
        /// <param name="aSqlStatement">The SQL statement to execute</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <typeparam name="T">The data type the return value should be</typeparam>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        /// <seealso cref="Items.Results.SqlResultWithValue{T}"/>
        public Boolean TryExecute<T>(String aSqlStatement,
            CommandType aCommandType,
            IEnumerable<SqlParameter> aParameters,
            out SqlResultWithValue<T> aResult)
        {
            return TryExecute<T>
            (
                new Statement
                {
                    Sql = aSqlStatement,
                    Type = aCommandType,
                    Parameters = aParameters
                },
                out aResult
            );
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aResult">The resulting structure that contains the return value
        /// and an exception object if the SQL statement failed to execute properly</param>
        /// <typeparam name="T">The data type the return value should be</typeparam>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="Items.Results.SqlResultWithValue{T}"/>
        public Boolean TryExecute<T>(Statement aStatement,
            out SqlResultWithValue<T> aResult)
        {
            return TryExecute<T>
            (
                aStatement,
                CreateConnection(),
                out aResult
            );
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aResult">The resulting structure that contains the return value
        /// and an exception object if the SQL statement failed to execute properly</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <typeparam name="T">The data type the return value should be</typeparam>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        /// <seealso cref="Items.Results.SqlResultWithValue{T}"/>
        public Boolean TryExecute<T>(Statement aStatement,
            SqlConnection aConnection,
            out SqlResultWithValue<T> aResult)
        {
            return TryExecute<T>(aStatement, aConnection, true, out aResult);
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aResult">The resulting structure that contains the return value
        /// and an exception object if the SQL statement failed to execute properly</param>
        /// <param name="aCloseConnection">If true, closes the connection after execution</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <typeparam name="T">The data type the return value should be</typeparam>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        /// <seealso cref="Items.Results.SqlResultWithValue{T}"/>
        public Boolean TryExecute<T>(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection,
            out SqlResultWithValue<T> aResult)
        {
            return TryExecute<T>(aStatement, aConnection, aCloseConnection, null, out aResult);
        }

        /// <summary>
        /// Tries to execute an SQL statement.
        /// The return value indicates whether or not the statement executed successfully. 
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute</param>
        /// <param name="aResult">The resulting structure that contains the return value
        /// and an exception object if the SQL statement failed to execute properly</param>
        /// <param name="aCloseConnection">If true, closes the connection after execution</param>
        /// <param name="aTransaction">The SqlTransaction object to use during execution</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <typeparam name="T">The data type the return value should be</typeparam>
        /// <returns>A boolean value that indicates whether or not the Sql Statement was executed successfully</returns>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        /// <seealso cref="Items.Results.SqlResultWithValue{T}"/>
        public Boolean TryExecute<T>(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection,
            SqlTransaction aTransaction,
            out SqlResultWithValue<T> aResult)
        {

            // Declare the defaulted value
            aResult = new SqlResultWithValue<T>();
            aResult.Value = default(T);

            try
            {

                // Execute the query to get the proper return value
                aResult = Execute<T>(aStatement, aConnection, aCloseConnection, aTransaction);

            }
            catch (Exception e)
            {

                // Some error happened
                aResult.Exception = e;
                fAddLogEntry(new SqlLogError(new Items.Logging.SqlLogItem(e.Message, aStatement, e.StackTrace), e));

            }

            // Return
            return aResult.Success;

        }

        #endregion TryExecute<T>

    }

}
