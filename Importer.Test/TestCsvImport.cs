using System;
using System.Collections.Generic;
using System.IO;
using Importer.CustomImporters;
using Importer.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Importer.Test
{
    [TestClass]
    public class TestCsvImport
    {
        private readonly string[] _csvFileData =
        {
            "FirstName,LastName,Address,PhoneNumber",
            "Jimmy,Smith,102 Long Lane,29384857",
            "Clive,Owen,65 Ambling Way,31214788",
            "James,Brown,82 Stewart St,32114566",
            "Graham,Howe,12 Howard St,8766556",
            "John,Howe,78 Short Lane,29384857",
            "Clive,Smith,49 Sutherland St,31214788",
            "James,Owen,8 Crimson Rd,32114566",
            "Graham,Brown,94 Roland St,8766556"
        };

        private readonly string[] _expectedResultFile1 =
        {
            "Clive, 2",
            "Graham, 2",
            "James, 2",
            "Jimmy, 1",
            "John, 1",
            " ",
            "Brown, 2",
            "Howe, 2",
            "Owen, 2",
            "Smith, 2"
        };

        private readonly string[] _expectedResultFile2 =
        {
            "65 Ambling Way",
            "8 Crimson Rd",
            "12 Howard St",
            "102 Long Lane",
            "94 Roland St",
            "78 Short Lane",
            "82 Stewart St",
            "49 Sutherland St"
        };

        private readonly string[] _csvFileDataColumnMissMatchException =
        {
            "FirstName,LastName,Address,PhoneNumber",
            "Jimmy,Smith,102 Long Lane,29384857",
            "Clive,Owen,65 Ambling Way,31214788",
            "James,Brown,82 Stewart St,32114566",
            "Graham,Howe,12 Howard St,8766556",
            "John,Howe,78 Short Lane,29384857,blahblahblahblahblahblah",
            "Clive,Smith,49 Sutherland St,31214788",
            "James,Owen,8 Crimson Rd,32114566",
            "Graham,Brown,94 Roland St,8766556"
        };

        [TestMethod]
        public void ImportCsvNoFiles()
        {
            ICustomImporter customImport = new CustomImport(_csvFileData);
            var reportOutput = GetReportOutput(customImport);
            CollectionAssert.AreEqual(customImport.ReportOutput, reportOutput);
        }

        [TestMethod]
        public void ImportCsvColumnMissmatch()
        {
            try
            {
                ICustomImporter customImport = new CustomImport(_csvFileDataColumnMissMatchException);
                GetReportOutput(customImport);
            }
            catch (CsvUtilities.Exceptions.ColumnsMismatchException exc)
            {
                StringAssert.Contains(exc.Message, "column count mismatch");
                return;
            }
            Assert.Fail("No ColumnsMismatchException thrown");
        }

        [TestMethod]
        public void ImportCsvFiles()
        {
            string pathDataFile = Path.Combine(Environment.CurrentDirectory, "Sample", "Data.csv");
            string pathOutputFolder = Path.Combine(Environment.CurrentDirectory, "Sample", "Output");
            ICustomImporter customImport = new CustomImport(
                pathDataFile,
                pathOutputFolder);
            customImport.ImportData();
            customImport.ProcessOutput();

            //Read output file
            string pathFile1 = Path.Combine(pathOutputFolder, "FirstName And LastName Frequency.txt");
            string pathFile2 = Path.Combine(pathOutputFolder, "Address List.txt");

            CollectionAssert.AreEqual(File.ReadAllLines(pathFile1), _expectedResultFile1);
            CollectionAssert.AreEqual(File.ReadAllLines(pathFile2), _expectedResultFile2);
        }

        private List<string> GetReportOutput(ICustomImporter customImport)
        {
            customImport.ImportData();
            customImport.ProcessOutput();
            List<string> results = new List<string>(_expectedResultFile1);
            results.Add(" ");
            results.AddRange(_expectedResultFile2);
            return results;
        }

    }
}
