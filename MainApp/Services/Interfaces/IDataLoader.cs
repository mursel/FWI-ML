using BspCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainApp
{
    public interface IDataLoader : IFileLoader<DataModel>
    {
        Task<List<DataModel>> GetAllAsync();
        
        // any other custom methods goes here
    }
}