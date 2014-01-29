using SqlMagic.Monitor.Items.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlMagic.Monitor.Sql
{

    /// <summary>
    /// A fully managed SQL Server class that takes care of handling connections.
    /// </summary>
    public sealed partial class Sql : IDisposable
    {

        #region Constructors

        /// <summary>
        /// Creates a new instance of the Sql class with the given connection string.
        /// </summary>
        /// <param name="aConnectionString">The connection string used to connect to the server</param>
        /// <remarks>
        /// This will use the default Timeout value and set logging to false
        /// </remarks>
        public Sql(String aConnectionString)
            : this(aConnectionString, ExecutionTimeout){ }

        /// <summary>
        /// Creates a new instance of the Sql class with the given connection string and timeout value.
        /// </summary>
        /// <param name="aConnectionString">The connection string used to connect to the server</param>
        /// <param name="aTimeout">The timeout, in milliseconds, for a command to execute</param>
        /// <remarks>
        /// This will use the supplied timeout value and set logging to false
        /// </remarks>
        public Sql(String aConnectionString, Int32 aTimeout)
            : this(aConnectionString, aTimeout, false) { }

        /// <summary>
        /// Creates a new instance of the Sql class with the given connection string and timeout value and whether or not logging is enabled.
        /// </summary>
        /// <param name="aConnectionString">The connection string used to connect to the server</param>
        /// <param name="aLogging">A boolean value that represents whether or not logging is enabled</param>
        /// <remarks>
        /// This will use the default Timeout value and set logging to the supplied boolean value
        /// </remarks>
        public Sql(String aConnectionString, Boolean aLogging)
            : this(aConnectionString, ExecutionTimeout, aLogging) { }

        /// <summary>
        /// Creates a new instance of the Sql class with the given connection string, timeout value, and whether or not logging is enabled.
        /// </summary>
        /// <param name="aConnectionString">The connection string used to connect to the server</param>
        /// <param name="aTimeout">The timeout, in milliseconds, for a command to execute</param>
        /// <param name="aLogging">A boolean value that represents whether or not logging is enabled</param>
        public Sql(String aConnectionString, Int32 aTimeout, Boolean aLogging)
        {
            iConnectionString = aConnectionString;
            iTimeout = aTimeout;
            iLogging = aLogging;
            iOpenConnections = new ConcurrentDictionary<SqlConnection, Object>();
            iTransactions = new ConcurrentDictionary<SqlTransaction, Object>();
            iLogItems = new ConcurrentQueue<SqlLogItem>();
        }

        #endregion Constructors

    }

}
