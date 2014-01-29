using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlMagic.Monitor.EventArgs;

namespace SqlMagic.Monitor.Sql
{
    /// <summary>
    /// A fully managed SQL Server class that takes care of handling connections.
    /// </summary>
    public sealed partial class Sql : IDisposable
    {

        /// <summary>
        /// The ItemLogged event is fired whenever there is an item that has been appended to the log.
        /// </summary>
        public event EventHandler<SqlLogEventArgs> ItemLogged;

    }
}
