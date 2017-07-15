using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvUtilities;
using CsvUtilities.Interfaces;
using Importer.Interfaces;

namespace Importer.CustomImporters
{
    /// <summary>
    /// Custom csv import operation on file with headers FirstName,LastName,Address,PhoneNumber.
    /// Outputs report to target folder specified 
    /// </summary>
    public class CustomImport : ICustomImporter
    {
        private readonly ICsvReader _csvReader;
        private readonly string _targetFolderOutput;
        private const string LineDelimter = " ";

        /// <summary>
        /// If no output path is provided, all reports will appear within this property
        /// </summary>
        public List<string> ReportOutput { get; private set; }

        public CustomImport(string sourceCsv, string targetFolderOutput)
        {
            _csvReader = new CsvReader(sourceCsv);
            _targetFolderOutput = targetFolderOutput;
        }

        public CustomImport(string[] csvData, string targetFolderOutput)
        {
            _csvReader = new CsvReader(csvData);
            _targetFolderOutput = targetFolderOutput;
        }

        public CustomImport(string[] csvData)
        {
            _csvReader = new CsvReader(csvData);
        }

        /// <summary>
        /// Runs the custom importer
        /// </summary>
        public void ImportData()
        {
            _csvReader.Read();
        }

        /// <summary>
        /// Processes any report or output requirements
        /// </summary>
        public void ProcessOutput()
        {
            DataTable dt = _csvReader.AsDataTable();
            var rows = dt.AsEnumerable();
            var firstNameLastNameReport = GetFirstNameLastNameReport(rows);
            List<string> addressListSortedOnStreetName = GetListSortedOnWordX(rows, "Address", 2);

            if (!string.IsNullOrWhiteSpace(_targetFolderOutput))
            {
                WriteReport(firstNameLastNameReport, _targetFolderOutput, "FirstName And LastName Frequency.txt");
                WriteReport(addressListSortedOnStreetName, _targetFolderOutput, "Address List.txt");
            }
            else
            {
                SetReportOutput(new List<List<string>> { firstNameLastNameReport, addressListSortedOnStreetName });
            }
        }

        /// <summary>
        /// Sets the ReportOutput Property
        /// </summary>
        /// <param name="lists">List of reports (list of lists)</param>
        private void SetReportOutput(List<List<string>> lists)
        {
            ReportOutput = new List<string>();

            for (int i = 0; i < lists.Count; i++)
            {
                List<string> list = lists[i];
                ReportOutput.AddRange(list);
                if (i < lists.Count - 1)
                    ReportOutput.Add(LineDelimter); //Report line delimiter
            }
        }

        private List<string> GetFirstNameLastNameReport(EnumerableRowCollection<DataRow> rows)
        {
            List<string> reportFirstNamesFrequency = GetDataFrequency(rows, "FirstName");
            List<string> reportLastNamesFrequency = GetDataFrequency(rows, "LastName");

            List<string> firstNameLastNameReport = new List<string>(reportFirstNamesFrequency);
            firstNameLastNameReport.Add(LineDelimter); //empty line to split reports
            firstNameLastNameReport.AddRange(reportLastNamesFrequency);
            return firstNameLastNameReport;
        }

        /// <summary>
        /// Writes a report file to the specified location
        /// </summary>
        /// <param name="report">The lines of text appearring in the report</param>
        /// <param name="targetFolderOutput">The output foldr name to save to</param>
        /// <param name="filename">The filename to be used when saving the report</param>
        private void WriteReport(List<string> report, string targetFolderOutput, string filename)
        {
            string fileAndPath = Path.Combine(targetFolderOutput, filename);
            File.WriteAllLines(fileAndPath, report);
        }

        /// <summary>
        /// Gets the report that lists the frequency of the occurring fieldName, 
        /// first ordered by frequency, then alphabetically on the same field
        /// </summary>
        /// <param name="rows">The DataTable.AsEnumerable() object</param>
        /// <param name="fieldName">Name of field to get frequency on its data</param>
        /// <returns>List of rows for the report</returns>
        private List<string> GetDataFrequency(EnumerableRowCollection<DataRow> rows, string fieldName)
        {
            IEnumerable<string> allDataForColumn = rows
                .Select(x => x.Field<string>(fieldName));

            Dictionary<string, int> dataWithFrequency = new Dictionary<string, int>();

            foreach (string value in allDataForColumn)
            {
                if (dataWithFrequency.ContainsKey(value))
                    dataWithFrequency[value]++;
                else
                    dataWithFrequency.Add(value, 1);
            }
            return dataWithFrequency
                .OrderByDescending(x => x.Value)
                .ThenBy(x => x.Key)
                .Select(x => $"{x.Key}, {x.Value}")
                .ToList();
        }

        /// <summary>
        /// Gets the report that lists the items sorted by from a specific word number only
        /// </summary>
        /// <param name="rows">The DataTable.AsEnumerable() object</param>
        /// <param name="fieldName">Name of field to get data for</param>
        /// <param name="wordNumber">From which word number to start sorting</param>
        private List<string> GetListSortedOnWordX(EnumerableRowCollection<DataRow> rows, string fieldName, int wordNumber)
        {
            IEnumerable<string> allDataForColumn = rows
                .Select(x => x.Field<string>(fieldName));

            SortedDictionary<string, string> sortedDict = new SortedDictionary<string, string>();

            foreach (string value in allDataForColumn)
            {
                List<string> words = value.Split(' ').ToList();
                List<string> wordsToSort = words.Skip(wordNumber - 1).ToList();
                string phraseToSort = wordsToSort.Aggregate("", (current, w) => current + " " + w);
                sortedDict.Add(phraseToSort, value);
            }

            return sortedDict
                .Select(x => x.Value)
                .ToList(); //already sorted, no need to double work
        }
    }
}
