using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SqlMagic.Monitor.Items.Logging;

namespace SqlMagic.Monitor.Sql
{

    /// <summary>
    /// A fully managed SQL Server class that takes care of handling connections.
    /// </summary>
    public sealed partial class Sql : IDisposable
    {

        /*

        #region RunTransaction

        /// <summary>
        /// Executes a list of transaction items in sequential order as one Sql Transaction.
        /// </summary>
        /// <param name="aTransactionItems">The set of transaction items</param>
        /// <param name="aSettings">The settings that are to be used for the current set of transaction items</param>
        /// <returns>A SqlTransactionResult object that contains post execution information about a given transaction</returns>
        /// <seealso cref="Items.Transaction.SqlTransactionSettings"/>
        /// <seealso cref="Items.Transaction.SqlTransactionResults"/>
        /// <seealso cref="Items.Transaction.SqlTransactionItemBase"/>
        public Items.Transaction.SqlTransactionResults RunTransaction(IEnumerable<Items.Transaction.SqlTransactionItemBase> aTransactionItems,
            Items.Transaction.SqlTransactionSettings aSettings)
        {

            // Convert the transaction collection to a List object
            List<Items.Transaction.SqlTransactionItemWithError> oErrorList = new List<Items.Transaction.SqlTransactionItemWithError>();
            List<Items.Transaction.SqlTransactionItemBase> oCompletedList = new List<Items.Transaction.SqlTransactionItemBase>();
            Items.Transaction.SqlTransactionResults oTransactionResult = new Items.Transaction.SqlTransactionResults();
            List<Items.Transaction.SqlTransactionItemBase> oItems = aTransactionItems.ToList();
            SqlConnection oConnection = CreateConnection();
            SqlTransaction oTransaction = null;
            bool bContinueTransaction = true;
            bool bCommit = true;

            // Let's see if we have any items to process
            if (oItems.Any())
            {

                // Begin the transaction
                oTransaction = oConnection.BeginTransaction();

                // Loop for all transaction items
                while (oItems.Any() && bContinueTransaction)
                {

                    // Grab the first item
                    Items.Transaction.SqlTransactionItemBase oItem = oItems[0];

                    // Remove the first item
                    oItems.RemoveAt(0);

                    // Store the resulting data here
                    Items.Results.SqlResultWithDataSet oResults = null;


                    // Attempt to execute the item
                    bool bResult = TryOpen(oItem.Statement, oConnection, false, oTransaction, out oResults);

                    // Create a new event argument object to be used for invoking purposes
                    EventArgs.SqlTransactionStatusEventArgs oArguments =
                        new EventArgs.SqlTransactionStatusEventArgs(oItem.Statement, oResults.Results, bResult);

                    // See if we can invoke members
                    if (aSettings.CanInvokeMembers)
                    {

                        // Invoke the member
                        oItem.Method.Invoke(oArguments);

                    }

                    // Now that we are done, check the result of the item
                    if (bResult)
                    {

                        // Check to see if we have new items to insert into the transaction
                        if ((oArguments.NewItems ?? new List<Items.Transaction.SqlTransactionItem>()).Any()) oItems.InsertRange(0, oArguments.NewItems);

                        // We have completed. Let's analyze the arguments to see if anything needs to change
                        bContinueTransaction = oArguments.Continue;

                        // Add it to the completed list
                        oCompletedList.Add(oItem);

                    }
                    else
                    {

                        // Add the item to the failed list
                        oErrorList.Add(new Items.Transaction.SqlTransactionItemWithError(oItem, oResults.Exception));

                        // The current item failed. Check the settings object to determine what happens
                        switch (aSettings.OnTransactionItemFailure)
                        {
                            case Items.Transaction.SqlTransactionItemFailureOptions.Rollback:
                                bContinueTransaction = false;
                                bCommit = false;
                                break;
                            case Items.Transaction.SqlTransactionItemFailureOptions.CommitAndStop:
                                bContinueTransaction = false;
                                break;
                        }

                    }

                }

                // All done
                if (bCommit)
                {
                    switch (aSettings.OnTransactionCompletion)
                    {
                        case Items.Transaction.SqlTransactionCompletionOptions.Commit:
                            oTransaction.Commit();
                            break;
                        case Items.Transaction.SqlTransactionCompletionOptions.Rollback:
                            oTransaction.Rollback();
                            break;
                    }
                }
                else
                {
                    oTransaction.Rollback();
                }

                // Dispose of the transaction
                oTransaction.Dispose();

            }

            // Set some properties
            oTransactionResult.Completed = oCompletedList;
            oTransactionResult.Errors = oErrorList;

            if (!oErrorList.Any())
            {

                // This finished without errors, but did it actually finish or was it ordered to stop prematurely
                oTransactionResult.TransactionResult = oItems.Any() ?
                    (aSettings.OnTransactionCompletion == Items.Transaction.SqlTransactionCompletionOptions.Commit ?
                        Items.Transaction.SqlTransactionCompletionResult.CommittedAndStopped :
                        Items.Transaction.SqlTransactionCompletionResult.RolledBack) :
                    (aSettings.OnTransactionCompletion == Items.Transaction.SqlTransactionCompletionOptions.Commit ?
                        Items.Transaction.SqlTransactionCompletionResult.Completed :
                        Items.Transaction.SqlTransactionCompletionResult.RolledBack);

            }
            else
            {

                // There were errors. We need to check to see what the settings told us to do
                switch (aSettings.OnTransactionItemFailure)
                {
                    case Items.Transaction.SqlTransactionItemFailureOptions.CommitAndStop:
                        oTransactionResult.TransactionResult =
                            aSettings.OnTransactionCompletion == Items.Transaction.SqlTransactionCompletionOptions.Commit ?
                            Items.Transaction.SqlTransactionCompletionResult.CommittedAndStopped :
                            Items.Transaction.SqlTransactionCompletionResult.RolledBack;
                        break;
                    case Items.Transaction.SqlTransactionItemFailureOptions.Continue:
                        oTransactionResult.TransactionResult =
                            aSettings.OnTransactionCompletion == Items.Transaction.SqlTransactionCompletionOptions.Commit ?
                            Items.Transaction.SqlTransactionCompletionResult.CommittedWithErrors :
                            Items.Transaction.SqlTransactionCompletionResult.RolledBack;
                        break;
                    case Items.Transaction.SqlTransactionItemFailureOptions.Rollback:
                        oTransactionResult.TransactionResult = Items.Transaction.SqlTransactionCompletionResult.RolledBack;
                        break;
                }

            }

            // Return the transaction result
            return oTransactionResult;

        }

        /// <summary>
        /// Executes a list of queries in sequential order as one Sql Transaction.
        /// </summary>
        /// <param name="aQueries">The set of queries to run</param>
        /// <param name="aSettings">The settings that are to be used for the current set of transaction items</param>
        /// <returns>A SqlTransactionResult object that contains post execution information about a given transaction</returns>
        /// <seealso cref="Items.Transaction.SqlTransactionSettings"/>
        /// <seealso cref="Items.Transaction.SqlTransactionResults"/>
        public Items.Transaction.SqlTransactionResults RunTransaction(Items.Transaction.SqlTransactionSettings aSettings,
            params string[] aQueries)
        {

            // Run time
            return RunTransaction(aSettings, null, aQueries);

        }

        /// <summary>
        /// Executes a list of queries in sequential order as one Sql Transaction.
        /// </summary>
        /// <param name="aQueries">The set of queries to run</param>
        /// <param name="aAction">The action to invoke on every query</param>
        /// <param name="aSettings">The settings that are to be used for the current set of transaction items</param>
        /// <returns>A SqlTransactionResult object that contains post execution information about a given transaction</returns>
        /// <seealso cref="Items.Transaction.SqlTransactionSettings"/>
        /// <seealso cref="Items.Transaction.SqlTransactionResults"/>
        public Items.Transaction.SqlTransactionResults RunTransaction(Items.Transaction.SqlTransactionSettings aSettings,
            Action<EventArgs.SqlTransactionStatusEventArgs> aAction,
            params string[] aQueries)
        {

            // Create our transaction items to run
            List<Items.Transaction.SqlTransactionItemBase> oItems = new List<Items.Transaction.SqlTransactionItemBase>(aQueries.Count());

            // Convert each query
            foreach (var sQuery in aQueries)
            {

                // New transaction item
                Statement oStatement = new Statement();
                oStatement.Sql = sQuery;
                oStatement.Parameters = null;
                oStatement.Type = CommandType.Text;

                // Add it to the list
                oItems.Add(new Items.Transaction.SqlTransactionItem(oStatement, aAction));

            }

            // Run the list
            return RunTransaction(oItems.AsEnumerable(), aSettings);

        }

        #endregion

        */

        #region Begin/End Transaction

        /// <summary>
        /// Begins a new transaction, with the isolation level set to serializable, on a new connection. The returned transaction object
        /// contains a connection that should not be closed for the lifetime of the transaction.
        /// </summary>
        /// <returns>A SqlTransaction object</returns>
        /// <remarks>
        /// When using the transaction systems, make sure that the method overloads being used, whether they are
        /// asynchronous or not, include overloads to pass a connection and to leave the connection open after
        /// execution. Failure to follow these rules can and will lead to unexpected or undesired side effects
        /// </remarks>
        public SqlTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.Serializable);
        }

        /// <summary>
        /// Begins a new transaction on a new connection. The returned transaction object
        /// contains a connection that should not be closed for the lifetime of the transaction.
        /// </summary>
        /// <returns>A SqlTransaction object</returns>
        /// <remarks>
        /// When using the transaction systems, make sure that the method overloads being used, whether they are
        /// asynchronous or not, include overloads to pass a connection and to leave the connection open after
        /// execution. Failure to follow these rules can and will lead to unexpected or undesired side effects
        /// </remarks>
        public SqlTransaction BeginTransaction(IsolationLevel aIsolationLevel)
        {

            // Create the connection
            SqlConnection oConnection = CreateConnection();

            // Begin the transaction
            SqlTransaction oTransaction = oConnection.BeginTransaction(aIsolationLevel);
                        
            // Log time.
            if (iLogging) fAddLogEntry(new SqlLogItem(String.Format("Transaction created with Isolation Level: {0}", aIsolationLevel), Statement.Empty, Environment.StackTrace));

            // Add it to the transaction list
            iTransactions.TryAdd(oTransaction, null);

            // Return it
            return oTransaction;

        }

        /// <summary>
        /// Ends the transaction, commits all changes, and closes the connection
        /// </summary>
        /// <param name="aTransaction">The transaction to end</param>
        public void EndTransaction(SqlTransaction aTransaction)
        {
            EndTransaction(aTransaction, true);
        }

        /// <summary>
        /// Ends the transaction and closes the connection.
        /// </summary>
        /// <param name="aCommit">If true, the transaction will be committed. Otherwise, the transaction will be rolled back</param>
        /// <param name="aTransaction">The transaction to end</param>
        public void EndTransaction(SqlTransaction aTransaction, Boolean aCommit)
        {

            // Log
            if (iLogging) fAddLogEntry(new SqlLogItem(String.Format("Ending Transaction (Commit: {0})", aCommit), Statement.Empty, Environment.StackTrace));

            // Figure out what to do
            if (aCommit)
                aTransaction.Commit();
            else
                aTransaction.Rollback();
            
            // Storage
            Object oBlankObject = null;

            // Try and Remove
            iTransactions.TryRemove(aTransaction, out oBlankObject);

        }

        #endregion
        
    }

}
