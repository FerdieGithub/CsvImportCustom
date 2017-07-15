using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer.Interfaces
{
    public interface ICustomImporter
    {
        /// <summary>
        /// If no output path is provided, all reports will appear within this property
        /// </summary>
        List<string> ReportOutput { get; }

        /// <summary>
        /// Runs the custom importer
        /// </summary>
        void ImportData();

        /// <summary>
        /// Processes any report or output requirements
        /// </summary>
        void ProcessOutput();
    }
}
