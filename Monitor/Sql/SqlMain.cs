using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using SqlMagic.Monitor.Items.Results;
using SqlMagic.Monitor.Items.Logging;
using SqlMagic.Monitor.Extensions;


namespace SqlMagic.Monitor.Sql
{
    
    /// <summary>
    /// A fully managed SQL Server class that takes care of handling connections.
    /// </summary>
    public sealed partial class Sql : IDisposable
    {

        #region IDispose Implementation

        /// <summary>
        /// Disposes the Sql library and closes any active connections.
        /// </summary>
        public void Dispose()
        {

            // Call the dispose method with true
            Dispose(true);

        }
        
        /// <summary>
        /// Disposes the Sql library and closes any active connections.
        /// 
        /// When aDispose is set to true, all managed resources are explicitly cleaned up
        /// if they are applicable to be cleaned up. This includes any open connections
        /// as well as any open transactions, with transactions being cleaned up first.
        /// 
        /// By default, all transactions are rolled back. This is to prevent a client-side crash
        /// from being able to mess up any transactions and gives the programmer the ability
        /// to clean up properly in the event of a crash/exception arising from another operation.
        /// </summary>
        public void Dispose(Boolean aDispose)
        {

            // Check
            if (aDispose && iOpenConnections != null)
            {

                // This is no longer needed; connections for active transactions
                // that are closed automatically roll back the transaction.
                // Loop through the open transactions and roll them back
                // foreach (SqlTransaction oTransaction in iTransactions.Keys) if (oTransaction.Connection != null) oTransaction.Rollback();

                // loop through the connections
                foreach (SqlConnection oConnection in iOpenConnections.Keys)
                {

                    // Remove from the changing routine
                    oConnection.StateChange -= fStateChange;

                    // Check to see if it's open
                    if ((oConnection.State != ConnectionState.Broken || oConnection.State != ConnectionState.Closed)) oConnection.Dispose();

                }

                // Now clear
                iOpenConnections.Clear();
                iOpenConnections = null;

                iTransactions.Clear();
                iTransactions = null;

            }

            // Reset and clear the variables
            iLogging = false;
            iTimeout = 0;

            // Tell the garbage collector that it doesn't need to finalize this object
            // as it has already been disposed.
            GC.SuppressFinalize(this);

        }

        #endregion

        #region Destructor

        /// <summary>
        /// Calls the dispose routine to clean up any open connections+transactions
        /// </summary>
        ~Sql()
        {

            // Call the dispose routine again
            Dispose(false);

        }

        #endregion

    }

}