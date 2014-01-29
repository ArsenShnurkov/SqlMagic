using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SqlMagic.Monitor.Extensions;
using SqlMagic.Monitor.Items.Results;
using System.Threading.Tasks;

namespace SqlMagic.Monitor.Sql
{

    /// <summary>
    /// A fully managed SQL Server class that takes care of handling connections.
    /// </summary>
    public sealed partial class Sql : IDisposable
    {

        #region Open

        /// <summary>
        /// Creates a DataSet that contains zero or more DataTables based on the SQL Statement passed
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <returns>A DataSet that contains a set of DataTables</returns>
        /// <seealso cref="System.Data.DataSet"/>
        /// <seealso cref="System.Data.SqlClient.SqlDataReader"/>
        public SqlResultWithDataSet Open(String aSqlStatement)
        {
            return Open(aSqlStatement, System.Data.CommandType.Text);
        }

        /// <summary>
        /// Creates a DataSet that contains zero or more DataTables based on the SQL Statement passed
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <returns>A DataSet that contains a set of DataTables</returns>
        /// <seealso cref="System.Data.DataSet"/>
        /// <seealso cref="System.Data.CommandType"/>
        public SqlResultWithDataSet Open(String aSqlStatement,
            CommandType aCommandType)
        {
            return Open(aSqlStatement, System.Data.CommandType.Text, null);
        }
        
        /// <summary>
        /// Creates a DataSet that contains zero or more DataTables based on the SQL Statement passed
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <returns>A DataSet that contains a set of DataTables</returns>
        /// <seealso cref="System.Data.DataSet"/>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public SqlResultWithDataSet Open(String aSqlStatement,
            CommandType aCommandType,
            params SqlParameter[] aParameters)
        {
            return Open
            (
                aSqlStatement, aCommandType, aParameters == null ? null : aParameters.AsEnumerable()
            );
        }
        
        /// <summary>
        /// Creates a DataSet that contains zero or more DataTables based on the SQL Statement passed
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <returns>A DataSet that contains a set of DataTables</returns>
        /// <seealso cref="System.Data.DataSet"/>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public SqlResultWithDataSet Open(String aSqlStatement,
            CommandType aCommandType,
            IEnumerable<SqlParameter> aParameters)
        {
            return Open(new Statement { Sql = aSqlStatement, Type = aCommandType, Parameters = aParameters });
        }

        /// <summary>
        /// Creates a DataSet that contains zero or more DataTables based on the SQL Statement passed
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to open</param>
        /// <returns>A DataSet that contains a set of DataTables</returns>
        /// <seealso cref="System.Data.DataSet"/>
        /// <seealso cref="Sql.Statement"/>
        public SqlResultWithDataSet Open(Statement aStatement)
        {
            return Open(aStatement, CreateConnection());
        }

        /// <summary>
        /// Creates a DataSet that contains zero or more DataTables based on the SQL Statement passed
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to open</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <returns>A DataSet that contains a set of DataTables</returns>
        /// <seealso cref="System.Data.DataSet"/>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public SqlResultWithDataSet Open(Statement aStatement, SqlConnection aConnection)
        {
            return Open(aStatement, aConnection, true);
        }

        /// <summary>
        /// Creates a DataSet that contains zero or more DataTables based on the SQL Statement passed
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to open</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">A boolean flag that indicates whether or not the
        /// connection should be closed after the statement is executed</param>
        /// <returns>A DataSet that contains a set of DataTables</returns>
        /// <seealso cref="System.Data.DataSet"/>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public SqlResultWithDataSet Open(Statement aStatement, SqlConnection aConnection, Boolean aCloseConnection)
        {
            return Open(aStatement, aConnection, aCloseConnection, null);
        }

        /// <summary>
        /// Creates a DataSet that contains zero or more DataTables based on the SQL Statement passed
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to open</param>
        /// <param name="aConnection">The connection to use for this SQL statement</param>
        /// <param name="aCloseConnection">A boolean flag that indicates whether or not the
        /// connection should be closed after the statement is executed</param>
        /// <param name="aTransaction">The SqlTransaction object that this connection is part of</param>
        /// <returns>A DataSet that contains a set of DataTables</returns>
        /// <seealso cref="System.Data.DataSet"/>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        /// <seealso cref="System.Data.SqlClient.SqlTransaction"/>
        public SqlResultWithDataSet Open(Statement aStatement, SqlConnection aConnection, Boolean aCloseConnection, SqlTransaction aTransaction)
        {

            // Create the connection
            SqlDataReader oReader = null;
            DataSet oDataSet = null;
            SqlResultWithDataSet oResult = new SqlResultWithDataSet();

            // Create a command
            SqlCommand oCommand = new SqlCommand
            {
                CommandText = aStatement.Sql,
                CommandType = aStatement.Type,
                CommandTimeout = iTimeout,
                Connection = aConnection,
                Transaction = aTransaction
            };

            // Add the parameters
            if (aStatement.Parameters != null) oCommand.Parameters.AddRange(aStatement.Parameters.ToArray());

            // Do a log of the command being executed
            if (iLogging) fAddLogEntry(new Items.Logging.SqlLogItem(string.Format("Beginning Query Execution: {0}", aStatement.Sql.Substring(0, aStatement.Sql.Length / 4)), aStatement, Environment.StackTrace, DateTime.Now));
            
            // Store the reader
            oReader = oCommand.ExecuteReader(aCloseConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default);

            // Set the statistics
            oResult.Statistics = aConnection.RetrieveStatistics();

            // Post-Execution
            if (iLogging) fAddLogEntry(new Items.Logging.SqlLogItem(string.Format("Completed Query Execution: {0}", aStatement.Sql.Substring(0, aStatement.Sql.Length / 4)), aStatement, Environment.StackTrace, DateTime.Now));
            
            // Convert
            oDataSet = oReader.ToDataSet();

            // Set
            oResult.Results = oDataSet;

            // Close!
            oReader.Close();

            // Give back the result set
            return oResult;
            
        }

        #endregion Open

        #region TryOpen

        /// <summary>
        /// Tries to create a DataSet object.
        /// The resulting boolean indicates whether or not the query execution was successful
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <param name="aResult">The result object that contains the DataSet and potential Exception object</param>
        /// <returns>A boolean result that indicates whether or not the query was able to be successfully executed</returns>
        /// <seealso cref="Items.Results.SqlResultWithDataSet"/>
        public Boolean TryOpen(String aSqlStatement,
            out SqlResultWithDataSet aResult)
        {
            return TryOpen(aSqlStatement, CommandType.Text, out aResult);
        }

        /// <summary>
        /// Tries to create a DataSet object.
        /// The resulting boolean indicates whether or not the query execution was successful
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aResult">The result object that contains the DataSet and potential Exception object</param>
        /// <returns>A boolean result that indicates whether or not the query was able to be successfully executed</returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="Items.Results.SqlResultWithDataSet"/>
        public Boolean TryOpen(String aSqlStatement,
            CommandType aCommandType,
            out SqlResultWithDataSet aResult)
        {
            return TryOpen(aSqlStatement, aCommandType, null, out aResult);
        }

        /// <summary>
        /// Tries to create a DataSet object.
        /// The resulting boolean indicates whether or not the query execution was successful
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <param name="aResult">The result object that contains the DataSet and potential Exception object</param>
        /// <returns>A boolean result that indicates whether or not the query was able to be successfully executed</returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public Boolean TryOpen(String aSqlStatement,
            CommandType aCommandType,
            out SqlResultWithDataSet aResult,
            params SqlParameter[] aParameters)
        {
            return TryOpen
            (
                aSqlStatement,
                aCommandType,
                aParameters == null ? null : aParameters.AsEnumerable(),
                out aResult
            );
        }

        /// <summary>
        /// Tries to create a DataSet object.
        /// The resulting boolean indicates whether or not the query execution was successful
        /// </summary>
        /// <param name="aSqlStatement">The SQL statement to execute and return a DataSet on</param>
        /// <param name="aCommandType">The type of SQL statement being passed</param>
        /// <param name="aParameters">A list of parameters that will be used by the statement</param>
        /// <param name="aResult">The result object that contains the DataSet and potential Exception object</param>
        /// <returns>A boolean result that indicates whether or not the query was able to be successfully executed</returns>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        /// <seealso cref="System.Collections.Generic.IEnumerable{T}"/>
        public Boolean TryOpen(String aSqlStatement,
            CommandType aCommandType,
            IEnumerable<SqlParameter> aParameters,
            out SqlResultWithDataSet aResult)
        {
            return TryOpen
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
        /// Tries to create a DataSet object.
        /// The resulting boolean indicates whether or not the query execution was successful
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute and return a DataSet on</param>
        /// <param name="aResult">The result object that contains the DataSet and potential Exception object</param>
        /// <returns>A boolean result that indicates whether or not the query was able to be successfully executed</returns>
        /// <seealso cref="Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="Sql.Statement"/>
        public Boolean TryOpen(Statement aStatement,
            out SqlResultWithDataSet aResult)
        {
            return TryOpen(aStatement, CreateConnection(), out aResult);
        }

        /// <summary>
        /// Tries to create a DataSet object.
        /// The resulting boolean indicates whether or not the query execution was successful
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute and return a DataSet on</param>
        /// <param name="aConnection">A SQL Connection object</param>
        /// <param name="aResult">The result object that contains the DataSet and potential Exception object</param>
        /// <returns>A boolean result that indicates whether or not the query was able to be successfully executed</returns>
        /// <seealso cref="Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public Boolean TryOpen(Statement aStatement,
            SqlConnection aConnection,
            out SqlResultWithDataSet aResult)
        {
            return TryOpen(aStatement, aConnection, true, out aResult);
        }

        /// <summary>
        /// Tries to create a DataSet object.
        /// The resulting boolean indicates whether or not the query execution was successful
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute and return a DataSet on</param>
        /// <param name="aConnection">A SQL Connection object</param>
        /// <param name="aCloseConnection">A boolean flag that indicates whether or not the
        /// connection should be closed after the statement is executed</param>
        /// <param name="aResult">The result object that contains the DataSet and potential Exception object</param>
        /// <returns>A boolean result that indicates whether or not the query was able to be successfully executed</returns>
        /// <seealso cref="Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public Boolean TryOpen(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection,
            out SqlResultWithDataSet aResult)
        {
            return TryOpen(aStatement, aConnection, aCloseConnection, null, out aResult);
        }

        /// <summary>
        /// Tries to create a DataSet object.
        /// The resulting boolean indicates whether or not the query execution was successful
        /// </summary>
        /// <param name="aStatement">The SQL statement structure to execute and return a DataSet on</param>
        /// <param name="aConnection">A SQL Connection object</param>
        /// <param name="aCloseConnection">A boolean flag that indicates whether or not the
        /// connection should be closed after the statement is executed</param>
        /// <param name="aResult">The result object that contains the DataSet and potential Exception object</param>
        /// <param name="aTransaction">The SqlTransaction object that this connection is part of</param>
        /// <returns>A boolean result that indicates whether or not the query was able to be successfully executed</returns>
        /// <seealso cref="Items.Results.SqlResultWithDataSet"/>
        /// <seealso cref="Sql.Statement"/>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        /// <seealso cref="System.Data.SqlClient.SqlTransaction"/>
        public Boolean TryOpen(Statement aStatement,
            SqlConnection aConnection,
            Boolean aCloseConnection,
            SqlTransaction aTransaction,
            out SqlResultWithDataSet aResult)
        {

            // Assign default values
            aResult = new Items.Results.SqlResultWithDataSet();
            bool bCompleted;

            try
            {
                // Open and execute
                aResult = Open(aStatement, aConnection, aCloseConnection, aTransaction);
                bCompleted = true;
            }
            catch (Exception e)
            {
                bCompleted = false;
                aResult.Exception = e;
                fAddLogEntry(new Items.Logging.SqlLogError(new Items.Logging.SqlLogItem(e.Message, aStatement, e.StackTrace), e));
            }

            // Done
            return bCompleted;

        }

        #endregion TryOpen

    }

}
