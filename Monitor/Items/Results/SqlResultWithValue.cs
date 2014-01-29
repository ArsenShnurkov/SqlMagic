using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlMagic.Monitor.Items.Results
{

    /// <summary>
    /// The SqlResultWithValue object is the same
    /// as the SqlResult object, except it includes
    /// a single scalar value as a result from the query
    /// </summary>
    /// <typeparam name="T">The type of return value contained within this object</typeparam>
    public sealed class SqlResultWithValue<T> : SqlResultBase
    {

        /// <summary>
        /// The return value from the query execution
        /// </summary>
        public T Value { get; set; }

    }

}
