using BspCore;
using MainApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MainApp.Service.Interfaces
{
    public interface IDataLoader : IFileLoader<DataModel>
    {
        Task<List<DataModel>> GetAllAsync(string _fileName);
        double[][] ToArray(int[] columns);
        double[][] ToArray();

        // any other custom methods goes here
    }
}