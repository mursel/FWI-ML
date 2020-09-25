using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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