using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlMagic.Monitor.Items.Results
{

    /// <summary>
    /// The SqlResultBase object contains the output from
    /// an attempted Sql execution
    /// </summary>
    public abstract class SqlResultBase
    {

        /// <summary>
        /// The exception object contains any information
        /// pertaining to a failed query execution
        /// </summary>
        public virtual Exception Exception { get; set; }

        /// <summary>
        /// Returns a Boolean variable that determines whether or not the result was a success.
        /// </summary>
        public virtual Boolean Success
        {
            get
            {
                return Exception == null;
            }
        }

        /// <summary>
        /// A collection of statistics that came from the sql query.
        /// </summary>
        public virtual System.Collections.IDictionary Statistics { get; set; }


        /// <summary>
        /// Creates a new instance of the SqlResultBase class
        /// </summary>
        public SqlResultBase()
        {

        }

    }

}
