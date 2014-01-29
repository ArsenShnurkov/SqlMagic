using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlMagic.Monitor.Items.Logging;

namespace SqlMagic.Monitor.EventArgs
{

    /// <summary>
    /// The SqlLogEventArgs are passed along whenever an item is logged into the internal logging mechanism
    /// </summary>
    public class SqlLogEventArgs : System.EventArgs
    {

        /// <summary>
        /// The item that was logged by the SQL class
        /// </summary>
        public SqlLogItem Item { get; private set; }

        /// <summary>
        /// Creates a new instance of the SqlLogEventArgs object.
        /// </summary>
        /// <param name="aItem">The SqlLogItem that was added to the internal logging routine</param>
        public SqlLogEventArgs(SqlLogItem aItem)
        {
            Item = aItem;
        }

    }

}
