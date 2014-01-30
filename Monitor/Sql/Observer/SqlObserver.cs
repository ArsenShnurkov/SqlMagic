using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Immutable;

namespace SqlMagic.Monitor.Sql
{

    /// <summary>
    /// A fully managed SQL Server class that takes care of handling connections.
    /// </summary>
    public sealed partial class Sql : IDisposable
    {

        /// <summary>
        /// The Observer will monitor all connections and commands and other associated content.
        /// </summary>
        public static class Observer
        {

            /// <summary>
            /// A Dictionary object that is thread-safe and contains a
            /// collection of active commands that are being monitored
            /// </summary>
            private static ConcurrentDictionary<SqlCommand, Boolean> iObserveList;


            /// <summary>
            /// This method is called whenever a command has completed executing a function.
            /// </summary>
            /// <param name="aSender">The SqlCommand object</param>
            /// <param name="aArgs">The event arguments</param>
            private static void aCommandCompleted(Object aSender, StatementCompletedEventArgs aArgs)
            {

            }

            /// <summary>
            /// Returns an ImmutableDictionary that represents the current in-memory collection of command objects that were assigned to be observed.
            /// </summary>
            public static ImmutableDictionary<SqlCommand, Boolean> Commands
            {
                get { return iObserveList.ToImmutableDictionary(); }
            }

            /// <summary>
            /// Adds a SqlCommand object to be observed.
            /// </summary>
            /// <param name="aCommandObject">The SqlCommand object to observe</param>
            internal static void Observe(SqlCommand aCommandObject)
            {
                aCommandObject.StatementCompleted += aCommandCompleted;
            }

            /// <summary>
            /// Destroys any managed objects and cleans up the Observer class
            /// </summary>
            internal static void Dispose()
            {

            }

        }

    }


}
