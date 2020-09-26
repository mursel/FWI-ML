using System.Collections.Generic;

namespace MainApp.Service.Interfaces
{
    public interface IFileLoader<T>
    {
        int Load(string _fileName);
        int Save();
        int Close();
        IEnumerable<T> GetAll();

    }
}