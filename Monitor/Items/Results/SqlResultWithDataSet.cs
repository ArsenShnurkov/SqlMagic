using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlMagic.Monitor.Items.Results
{

    /// <summary>
    /// The SqlResultWithDataSet object is the same
    /// as the SqlResult object, except it includes
    /// a DataSet object that may contain the resulting
    /// DataSet from the given execution.
    /// </summary>
    /// <seealso cref="SqlResultBase"/>
    /// <seealso cref="System.Data.DataSet"/>
    public sealed class SqlResultWithDataSet : SqlResultBase
    {

        /// <summary>
        /// The result set from the SQL execution
        /// </summary>
        public DataSet Results { get; set; }

        /// <summary>
        /// Returns whether or not the query threw
        /// an exception while executing and whether
        /// or not there was at least one recordset returned
        /// </summary>
        public override bool Success
        {
            get
            {
                return base.Success && Results.Tables.Count > 0;
            }
        }

    }

}
