using System;

namespace CsvUtilities.Exceptions
{
    /// <summary>
    /// Exception that is thrown when not all rows in the csv file have the same amount of columns
    /// </summary>
    public class ColumnsMismatchException : Exception
    {
        /// <summary>
        /// Exception that is thrown when not all rows in the csv file have the same amount of columns
        /// </summary>
        /// <param name="expectedCount">The expected amount of columns (ususally the first row)</param>
        /// <param name="foundCount">The amount of columns found that did not correspond to the expected count</param>
        /// <param name="lineNumber">The first line number that caused the mismatch</param>
        /// <param name="line">The actual line data</param>
        public ColumnsMismatchException(int expectedCount, int foundCount, int lineNumber, string line) 
            : base($"Csv column count mismatch. Expecting {expectedCount} columns but " +
                   $"found a row with {foundCount} columns at line number {lineNumber} - {line}")
        {}
    }
}
