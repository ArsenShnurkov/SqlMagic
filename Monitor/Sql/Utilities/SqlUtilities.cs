using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SqlMagic.Monitor.Extensions;
using System.Data;
using SqlMagic.Monitor.Items.Logging;
using SqlMagic.Monitor.Items.Results;
using System.Threading.Tasks;

namespace SqlMagic.Monitor.Sql
{

    /// <summary>
    /// A fully managed SQL Server class that takes care of handling connections.
    /// </summary>
    public sealed partial class Sql : IDisposable
    {

        #region Utilities

        /// <summary>
        /// Returns an opened connection object
        /// </summary>
        /// <returns>A SqlConnection object</returns>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public SqlConnection CreateConnection()
        {
            return CreateConnection(true);
        }

        /// <summary>
        /// Returns a connection object
        /// </summary>
        /// <param name="aOpened">If true, the connection will be opened before it is returned</param>
        /// <returns>A SqlConnection object</returns>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public SqlConnection CreateConnection(Boolean aOpened)
        {

            //return oConnection;
            Task<SqlConnection> oConnection = fCreateConnectionAsync(aOpened, false);

            // All done
            SqlConnection oSqlConnection = oConnection.Result;

            // Return
            return oSqlConnection;

        }

        

        /// <summary>
        /// Returns an opened connection object asynchronously
        /// </summary>
        /// <returns>A SqlConnection object</returns>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public async Task<SqlConnection> CreateConnectionAsync()
        {
            return await CreateConnectionAsync(true);
        }

        /// <summary>
        /// Returns a connection object asynchronously
        /// </summary>
        /// <param name="aOpened">If true, the connection will be opened before it is returned</param>
        /// <returns>A SqlConnection object</returns>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        public async Task<SqlConnection> CreateConnectionAsync(Boolean aOpened)
        {
            return await fCreateConnectionAsync(aOpened, true);
        }

        /// <summary>
        /// Returns a connection object asynchronously
        /// </summary>
        /// <param name="aOpened">If true, the connection will be opened before it is returned</param>
        /// <param name="aIsAsync">If true, this is a synchronous call and should be handled as such</param>
        /// <returns>A SqlConnection object</returns>
        /// <seealso cref="System.Data.SqlClient.SqlConnection"/>
        private async Task<SqlConnection> fCreateConnectionAsync(Boolean aOpened, Boolean aIsAsync)
        {

            // Create the connection and open it
            SqlConnection oConnection = new SqlConnection(iConnectionString);

            // Create the Task
            Task oConnectionTask = oConnection.OpenAsync();

            // Configure the task
            await oConnectionTask.ConfigureAwait(aIsAsync);

            // Open if true via await command
            if (aOpened) await oConnectionTask;

            // Add the opened connection to the collection of connections
            iOpenConnections.TryAdd(oConnection, null);

            // Monitor the connection
            oConnection.StateChange += fStateChange;

            // Gathering time.
            oConnection.StatisticsEnabled = true;

            // Give it back
            return oConnection;

        }



        /// <summary>
        /// Checks the state of the connection.
        /// If the connection state is closed or broken,
        /// remove the connection from the tracking list
        /// and attempt to do some clean up.
        /// </summary>
        /// <param name="aSender">The SqlConnection object that raised the event</param>
        /// <param name="aEventArguments">This object represents the current state of the SqlConnection Object.</param>
        private void fStateChange(Object aSender, StateChangeEventArgs aEventArguments)
        {
            SqlConnection oConnection = aSender as SqlConnection;
            object oHelperObject = null;
            switch (aEventArguments.CurrentState)
            {
                case ConnectionState.Executing:
                case ConnectionState.Fetching:
                    if (iLogging) fAddLogEntry(new SqlLogItem(String.Format("Connection state is {0}", aEventArguments.CurrentState), Statement.Empty, Environment.StackTrace));
                    break;
                case ConnectionState.Broken:
                case ConnectionState.Closed:
                    if (oConnection != null)
                    {
                        iOpenConnections.TryRemove(oConnection, out oHelperObject);
                        oConnection.StateChange -= fStateChange;
                        if (iLogging) fAddLogEntry(new SqlLogItem(String.Format("Connection cleaned up and removed (State: {0})", aEventArguments.CurrentState), Statement.Empty, Environment.StackTrace));
                    }
                    break;
            }
        }

        /// <summary>
        /// Adds a new record to the internal log
        /// </summary>
        /// <param name="aLogItem">The item to log</param>
        private void fAddLogEntry(SqlLogItem aLogItem)
        {

            // Add an item to the log
            iLogItems.Enqueue(aLogItem);

            // Now, raise the event IF it has an attachment
            if (ItemLogged != null) ItemLogged(this, new EventArgs.SqlLogEventArgs(aLogItem));

        }

        /// <summary>
        /// A structure that contains a SQL statement
        /// </summary>
        /// <seealso cref="System.Data.CommandType"/>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        public struct Statement
        {

            /// <summary>
            /// The SQL statement to execute
            /// </summary>
            public string Sql { get; set; }

            /// <summary>
            /// The type of SQL statement being passed
            /// </summary>
            public CommandType Type { get; set; }

            /// <summary>
            /// A list of parameters that will be used by the statement
            /// </summary>
            public IEnumerable<SqlParameter> Parameters { get; set; }

            /// <summary>
            /// Returns an empty statement object
            /// </summary>
            public static Statement Empty
            {
                get
                {
                    return new Statement { Parameters = null, Sql = null, Type = CommandType.Text };
                }
            }
            
        }

        /// <summary>
        /// Creates a log item for a given Statement
        /// </summary>
        /// <param name="aStatement">The statement to create a log item for</param>
        private void fLogStatement(Statement aStatement)
        {

            // Let's build a logging message
            StringBuilder oStringBuilder = new StringBuilder();

            // Start building!
            oStringBuilder.AppendFormat("Statement: {0} ", aStatement.Sql);

            // Now parameter checking
            if (aStatement.Parameters != null && aStatement.Parameters.Any())
                oStringBuilder.AppendFormat("Parameters: {0} ", aStatement.Parameters.ToFormattedString());

            // Log.
            fAddLogEntry(new SqlLogItem(oStringBuilder.ToString(), aStatement, Environment.StackTrace));

        }
        
        #endregion

    }

}
