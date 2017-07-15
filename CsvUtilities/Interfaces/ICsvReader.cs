using System.Collections.Generic;
using System.Data;

namespace CsvUtilities.Interfaces
{
    /// <summary>
    /// Interface to read csv files
    /// </summary>
    public interface ICsvReader
    {
        /// <summary>
        /// The header values if containsHeader was true in the constructor, else null
        /// </summary>
        List<string> Header { get; }

        /// <summary>
        /// The data as read from the csv file (excluding the header if containsHeader was true in the constructor)
        /// </summary>
        List<List<string>> Data { get; }

        /// <summary>
        /// After a run of Read, IsPopulated will become true, else false
        /// </summary>
        bool IsPopulated { get; }

        /// <summary>
        /// Reads and processes the csv file.
        /// Throws ColumnsMismatchException if all row column counts do not match.
        /// </summary>
        void Read();

        /// <summary>
        /// Returns a DataTable of the csv data and headet
        /// </summary>
        DataTable AsDataTable();
    }
}