using System.Data;

namespace CsvUtilities.Interfaces
{
    public interface IDataTableLoader
    {
        ICsvReader Reader { get; set; }
        DataTable Load();
    }
}