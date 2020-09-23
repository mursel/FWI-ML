using System;
using System.Collections.Generic;

namespace BspCore
{
    public interface IDataLoader : IFileLoader<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        
        // any other custom methods goes here
    }
}