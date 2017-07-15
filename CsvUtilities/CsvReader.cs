using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CsvUtilities.Interfaces;

namespace CsvUtilities
{
    /// <summary>
    /// Class to read csv files
    /// </summary>
    public class CsvReader : ICsvReader
    {
        private readonly string _csvPath;
        private string[] _csvData;
        private readonly bool _containsHeader;
        private bool _isPopulated;

        /// <summary>
        /// The header values if containsHeader was true in the constructor, else null
        /// </summary>
        public List<string> Header { get; private set; }

        /// <summary>
        /// The data as read from the csv file (excluding the header if containsHeader was true in the constructor)
        /// </summary>
        public List<List<string>> Data { get; private set; }

        /// <summary>
        /// After a run of Read, IsPopulated will become true, else false
        /// </summary>
        public bool IsPopulated => _isPopulated;

        /// <summary>
        /// Create a CsvReader object that can be used to read a csv file and populate the header and data properties.
        /// Throws FileNotFoundException if the csv path and filename does not exist.
        /// </summary>
        /// <param name="csvPath">The full path and filename of the csv file</param>
        /// <param name="containsHeader">True of the csv file contains a header as the first row, false otherwise</param>
        public CsvReader(string csvPath, bool containsHeader = true)
        {
            if (!File.Exists(csvPath))
                throw new FileNotFoundException("Csv file not found", csvPath);

            _csvPath = csvPath;
            _containsHeader = containsHeader;
        }

        /// <summary>
        /// Create a CsvReader object that can be used to read a csv file and populate the header and data properties
        /// </summary>
        /// <param name="csvData">Csv file data lines</param>
        /// <param name="containsHeader">True of the csv file contains a header as the first row, false otherwise</param>
        public CsvReader(string[] csvData, bool containsHeader = true)
        {
            _csvData = csvData;
            _containsHeader = containsHeader;
        }

        /// <summary>
        /// Reads and processes the csv file.
        /// Throws ColumnsMismatchException if all row column counts do not match.
        /// </summary>
        public void Read()
        {
            List<List<string>> results = new List<List<string>>();
            if (!string.IsNullOrEmpty(_csvPath))
                _csvData = File.ReadAllLines(_csvPath);
            ProcessLines(_csvData, results);
            Data = results;
            _isPopulated = true;
        }

        /// <summary>
        /// Processes an array of lines from the csv file.
        /// Throws ColumnsMismatchException if all row column counts do not match.
        /// </summary>
        /// <param name="fileLines">The csv lines as read from the file</param>
        /// <param name="results">The results that would be appended</param>
        private void ProcessLines(string[] fileLines, List<List<string>> results)
        {
            int expectedColumnCount = -1;
            for (int i = 0; i < fileLines.Length; i++)
            {
                string line = fileLines[i];
                int columnCount = ReadLine(line, i, results);
                if (expectedColumnCount == -1)
                    expectedColumnCount = columnCount;
                else
                {
                    if (columnCount != expectedColumnCount)
                        throw new Exceptions.ColumnsMismatchException(expectedColumnCount, columnCount, i + 1, line);
                }
            }
        }

        /// <summary>
        /// Process a line from the csv file
        /// </summary>
        /// <param name="line">The line before delimited</param>
        /// <param name="lineNumber">The line number as a zero index</param>
        /// <param name="results">The results that would be appended</param>
        /// <returns>The amount of columns found in the line</returns>
        private int ReadLine(string line, int lineNumber, List<List<string>> results)
        {
            List<string> lineColumnValues = line.Split(',').ToList();
            if (lineNumber == 0 && _containsHeader)
                Header = lineColumnValues;
            else
                results.Add(lineColumnValues);
            return lineColumnValues.Count;
        }

        /// <summary>
        /// Returns a DataTable of the csv data and headet
        /// </summary>
        public DataTable AsDataTable()
        {
            DataTableLoader loader = new DataTableLoader(this);
            return loader.Load();
        }
    }
}
