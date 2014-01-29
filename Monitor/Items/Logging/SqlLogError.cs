using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlMagic.Monitor.Items.Logging
{

    /// <summary>
    /// A logging object for the Sql class that allows
    /// it to maintain an internal list of actions and
    /// provide diagnostic information.
    /// 
    /// This is an inherited class that contains an exception object
    /// </summary>
    public class SqlLogError : SqlLogItem
    {

        /// <summary>
        /// Creates a new SqlLogItemWithError object
        /// </summary>
        /// <param name="aBaseItem">The base SqlLogItem object</param>
        /// <param name="aException">The exception object</param>
        public SqlLogError(SqlLogItem aBaseItem,
            Exception aException)
            : base(aBaseItem.Message,
            aBaseItem.Statement,
            aBaseItem.StackTrace,
            aBaseItem.EntryTime)
        {
            Exception = aException;
        }

        /// <summary>
        /// An exception object that, if not null, contains error information about this entry.
        /// </summary>
        public Exception Exception { get; set; }

    }

}
