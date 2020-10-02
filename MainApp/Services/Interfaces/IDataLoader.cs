using BspCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MainApp.Service.Interfaces
{
    public interface IDataLoader : IFileLoader<DataModel>
    {
        Task<List<DataModel>> GetAllAsync();

        // any other custom methods goes here
    }
}