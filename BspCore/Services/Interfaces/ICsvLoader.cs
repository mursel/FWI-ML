using System;
using System.Collections.Generic;

namespace BspCore
{
    public interface ICsvLoader : IFileLoader<DataModel>
    {
        Task<IEnumerable<T>> GetAllAsync();
        
        // any other custom methods goes here
    }
}