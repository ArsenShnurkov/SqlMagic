using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SqlMagic.Monitor.Extensions
{

    /// <summary>
    /// This class contains a series of extension methods that 
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// Converts a SqlDataReader object into a DataSet
        /// </summary>
        /// <param name="aReader">The SqlDataReader to convert</param>
        /// <returns>A DataSet object that contains the results from the SqlDataReader</returns>
        /// <seealso cref="System.Data.DataSet"/>
        /// <seealso cref="System.Data.SqlClient.SqlDataReader"/>
        public static DataSet ToDataSet(this SqlDataReader aReader)
        {
            return aReader.ToDataSet(true);
        }

        /// <summary>
        /// Converts a SqlDataReader object into a DataSet
        /// </summary>
        /// <param name="aReader">The SqlDataReader to convert</param>
        /// <param name="aClose">If true, after the SqlDataReader is converted to a Dataset, it will be closed</param>
        /// <returns>A DataSet object that contains the results from the SqlDataReader</returns>
        /// <seealso cref="System.Data.DataSet"/>
        public static DataSet ToDataSet(this SqlDataReader aReader, Boolean aClose)
        {

            // Grab the collection of tables
            IEnumerable<DataTable> oTables = aReader.ToTables(aClose);

            // Create a new DataSet object
            DataSet oDataSet = new DataSet();

            // Add the tables to the dataset
            foreach (DataTable oTable in oTables)
                oDataSet.Tables.Add(oTable);

            // Return the completed DataSet
            return oDataSet;

        }

        /// <summary>
        /// Converts a SqlDataReader to a DataTable and closes the SqlDataReader
        /// </summary>
        /// <param name="aReader">The SqlDataReader to convert</param>
        /// <returns>A DataTable that contains the columns and rows of the SqlDataReader</returns>
        /// <seealso cref="System.Data.DataTable"/>
        /// <seealso cref="System.Data.SqlClient.SqlDataReader"/>
        public static DataTable ToTable(this SqlDataReader aReader)
        {

            // Convert and return
            return aReader.ToTable(true);

        }

        /// <summary>
        /// Converts a SqlDataReader to a DataTable and closes the SqlDataReader
        /// </summary>
        /// <param name="aReader">The SqlDataReader to convert</param>
        /// <param name="aClose">If true, after the SqlDataReader is converted to a table, it will be closed</param>
        /// <returns>A DataTable that contains the columns and rows of the SqlDataReader</returns>
        /// <seealso cref="System.Data.DataTable"/>
        /// <seealso cref="System.Data.SqlClient.SqlDataReader"/>
        public static DataTable ToTable(this SqlDataReader aReader, Boolean aClose)
        {

            // Create a new table
            DataTable oTable = new DataTable();

            // Check to see if the reader is null
            if (aReader != null)
            {

                // Add the columns in the DataReader
                for (int nColumnCount = 0; nColumnCount < aReader.FieldCount; nColumnCount++)
                    oTable.Columns.Add(aReader.GetName(nColumnCount), aReader.GetFieldType(nColumnCount));

                // Check if it has rows
                if (aReader.HasRows)
                {

                    // Read the data in a loop
                    while (aReader.Read())
                    {

                        // Create the new data row
                        DataRow oRow = oTable.NewRow();

                        // Add the row based on the number of columns
                        for (int nColumn = 0; nColumn < oTable.Columns.Count; nColumn++)
                        {
                            // Set field values
                            oRow.SetField(nColumn, aReader[nColumn]);
                        }

                        // Add the row to the table
                        oTable.Rows.Add(oRow);

                    }

                }

                // All done.
                if (aClose) aReader.Close();

            }

            // Return
            return oTable;

        }

        /// <summary>
        /// Converts a SqlDataReader to a list of DataTables and closes the SqlDataReader.
        /// This is only useful when applied to a SqlDataReader that executed a series of SQL statements at once
        /// </summary>
        /// <param name="aReader">The SqlDataReader to convert</param>
        /// <returns>A list of DataTables with each DataTable being its own result set</returns>
        /// <seealso cref="System.Data.DataTable"/>
        /// <seealso cref="System.Data.SqlClient.SqlDataReader"/>
        public static IEnumerable<DataTable> ToTables(this SqlDataReader aReader)
        {

            // Return and close the SqlDataReader
            return aReader.ToTables(true);

        }

        /// <summary>
        /// Converts a SqlDataReader to a list of DataTables and closes the SqlDataReader.
        /// This is only useful when applied to a SqlDataReader that executed a series of SQL statements at once
        /// </summary>
        /// <param name="aReader">The SqlDataReader to convert</param>
        /// <param name="aClose">If true, after the SqlDataReader is converted to a table, it will be closed</param>
        /// <returns>A list of DataTables with each DataTable being its own result set</returns>
        /// <seealso cref="System.Data.DataTable"/>
        /// <seealso cref="System.Data.SqlClient.SqlDataReader"/>
        public static IEnumerable<DataTable> ToTables(this SqlDataReader aReader, Boolean aClose)
        {

            // Build a list of DataTables
            List<DataTable> oTables = new List<DataTable>();
            
            // Check to make sure the reader isn't null
            if (aReader != null)
            {

                // Create the various tables
                do
                {
                    oTables.Add(aReader.ToTable(false));
                } while (aReader.NextResult());

            }

            // All done!
            if (aClose) aReader.Close();

            // Return
            return oTables.AsEnumerable<DataTable>();

        }

        

        /// <summary>
        /// Performs a deep copy of the given parameter list, returning a new instance of an IEnumerable object.
        /// </summary>
        /// <param name="aItems">The list of parameters to copy</param>
        /// <returns>A new set of SqlParameters</returns>
        public static IEnumerable<T> Copy<T>(this IEnumerable<T> aItems)
        {

            // Create a list<Sqlparameter>
            List<T> oItems = new List<T>();

            // AddRange
            oItems.AddRange(aItems);

            // Done
            return oItems.AsEnumerable<T>();

        }

        /// <summary>
        /// Converts a collection of items to a Queue object
        /// </summary>
        /// <param name="aItems">The collection of items to convert</param>
        /// <returns>A queue of items to convert</returns>
        public static Queue<T> ToQueue<T>(this IEnumerable<T> aItems)
        {
            return new Queue<T>(aItems != null ? aItems : new List<T>());
        }

        /// <summary>
        /// Converts an enumerable collection into a readonly collection
        /// </summary>
        /// <param name="aItems">The collection to convert</param>
        /// <returns>A ReadOnly Collection of SqlParameters that cannot be modified</returns>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> aItems)
        {
            return new ReadOnlyCollection<T>(aItems != null ? aItems.ToList() : new List<T>());
        }

        /// <summary>
        /// Converts a SqlParameterCollection into an IEnumerable{SqlParameter} object
        /// </summary>
        /// <param name="aCollection">The SqlParameterCollection object to convert</param>
        /// <returns>IEnumerable{SqlParameter}</returns>
        public static IEnumerable<SqlParameter> ToEnumerable(this SqlParameterCollection aCollection)
        {

            // Declare our return list
            List<SqlParameter> oParameters = new List<SqlParameter>(aCollection.Count);

            // Loop
            foreach (SqlParameter oParameter in aCollection)
                oParameters.Add(oParameter);

            // Return
            return oParameters.AsEnumerable();

        }

        /// <summary>
        /// Converts an enumerable list of parameters into a specially formatted string
        /// </summary>
        /// <param name="aParameters">The list of parameters to format</param>
        /// <returns>A formatted string that contains the name, value, and type of the given list of parameters</returns>
        /// <seealso cref="System.Data.SqlClient.SqlParameter"/>
        public static String ToFormattedString(this IEnumerable<SqlParameter> aParameters)
        {
            StringBuilder oBuilder = new StringBuilder();
            foreach (SqlParameter oParameter in aParameters)
                oBuilder.AppendFormat("{{{0}, {1}, {2}}}, ",
                    oParameter.ParameterName,
                    oParameter.Value ?? String.Empty,
                    (oParameter.Value ?? String.Empty).GetType());
            return oBuilder.ToString().TrimEnd(new[] { ',', ' ' });
        }

        /// <summary>
        /// Converts a DataSet to an XML string
        /// </summary>
        /// <param name="aDataSet">The DataSet to convert to an XML string</param>
        /// <returns>String</returns>
        /// <seealso cref="System.Data.DataSet"/>
        public static String ToXml(this DataSet aDataSet)
        {
            string sXml = string.Empty;
            MemoryStream oMemoryStream = new MemoryStream();
            using (TextWriter streamWriter = new StreamWriter(oMemoryStream, Encoding.UTF8))
            {
                var xmlSerializer = new XmlSerializer(typeof(DataSet));
                xmlSerializer.Serialize(streamWriter, aDataSet);
                sXml = Encoding.UTF8.GetString(oMemoryStream.ToArray());
            }
            return sXml;
        }

    }

}
