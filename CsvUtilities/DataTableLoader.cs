using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CsvUtilities.Interfaces;

namespace CsvUtilities
{
    internal class DataTableLoader : IDataTableLoader
    {
        private bool _containsHeader;
        private bool _containsData;
        private ICsvReader _reader;

        public ICsvReader Reader
        {
            get { return _reader; }
            set
            {
                _reader = value;
                InitReader();
            }
        }

        public DataTableLoader()
        {
        }

        public DataTableLoader(ICsvReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Reader = reader;
        }

        private void InitReader()
        {
            if (!Reader.IsPopulated)
                Reader.Read();

            _containsHeader = Reader.Header != null && Reader.Header.Any();
            _containsData = Reader.Data != null && Reader.Data.Any();
        }

        public DataTable Load()
        {
            DataTable dt = new DataTable();
            PopulateColumns(dt);
            PopulateData(dt);
            return dt;
        }

        private void PopulateData(DataTable dt)
        {
            if (dt.Columns.Count == 0 || !_containsData) return;
            foreach (List<string> lineData in _reader.Data)
            {
                if (lineData == null || lineData.Count == 0) continue;
                dt.LoadDataRow(lineData
                    .OfType<object>()
                    .ToArray(), false);
            }
        }

        private void PopulateColumns(DataTable dt)
        {
            if (_containsHeader)
            {
                PopulateColumnsFromList(dt, Reader.Header);
            }
            else
            {
                if (!_containsData) return;
                int counter = 0;
                List<string> dummyColumns = Reader.Data[0]
                    .Select(x => (++counter).ToString())
                    .ToList();
                PopulateColumnsFromList(dt, dummyColumns);
            }
        }

        private void PopulateColumnsFromList(DataTable dt, List<string> columns)
        {
            foreach (string headerColumn in columns)
            {
                dt.Columns.Add(headerColumn);
            }
        }
    }
}
