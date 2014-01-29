using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlMagic.Monitor;

namespace SqlMagic.Monitor.Items.Logging
{

    /// <summary>
    /// A logging object for the Sql class that allows
    /// it to maintain an internal list of actions and
    /// provide diagnostic information.
    /// </summary>
    public class SqlLogItem
    {

        /// <summary>
        /// The date and time (locally) that this item was entered into the log.
        /// </summary>
        public DateTime EntryTime { get; private set; }

        /// <summary>
        /// A message that is attached to this log.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// If this is not equal to Sql.Statement.Empty then this object
        /// contains Statement information regarding a Sql execution
        /// </summary>
        public SqlMagic.Monitor.Sql.Sql.Statement Statement { get; private set; }

        /// <summary>
        /// The stacktrace of the environment at the time of this logging call.
        /// </summary>
        public string StackTrace { get; private set; }

        /// <summary>
        /// Creates a new SqlLogItem with the given parameters and assumes the entry time of DateTime.Now
        /// </summary>
        /// <param name="aMessage">The message for this entry item</param>
        /// <param name="aStatement">The Statement Object for this entry</param>
        /// <param name="aStacktrace">The stacktrace for this entry</param>
        public SqlLogItem(string aMessage,
            SqlMagic.Monitor.Sql.Sql.Statement aStatement,
            string aStacktrace)
            : this(aMessage,
            aStatement,
            aStacktrace,
            DateTime.Now)
        {

        }

        /// <summary>
        /// Creates a new SqlLogItem with the given parameters
        /// </summary>
        /// <param name="aMessage">The message for this entry item</param>
        /// <param name="aStatement">The Statement Object for this entry</param>
        /// <param name="aStacktrace">The stacktrace for this entry</param>
        /// <param name="aEntryTime">The entry time for this item</param>
        public SqlLogItem(string aMessage,
            SqlMagic.Monitor.Sql.Sql.Statement aStatement,
            string aStacktrace,
            DateTime aEntryTime)
        {
            EntryTime = aEntryTime;
            Statement = aStatement;
            StackTrace = aStacktrace;
            Message = aMessage;
        }

    }

}
