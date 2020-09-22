using System;
using System.Collections.Generic;
using System.IO;

namespace BspCore
{
    public class CsvLoaderService : ICsvLoader
    {
        private StreamReader streamReader;
        public int Load(string _fileName) {
            try
            {
                streamReader = File.OpenText(_fileName); //new StreamReader(_fileName);    
            }
            catch (System.Exception ex)
            {
                return ex.HResult;
            }            
            return 1;
        }
        public int Close() {
            try
            {
                streamReader.Close();                
            }
            catch (System.Exception ex)
            {
                return ex.HResult;
            }
            return 1;
        }
        public IEnumerable<DataModel> GetAll() {
            List<DataModel> data = new List<DataModel>();

            try
            {
                if(streamReader == null)    return new List<DataModel>();

                string line = string.Empty;

                while ((line= await streamReader.ReadLineAsync())!=null)
                {
                    DataModel dataModel = new DataModel() {
                        
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new List<DataModel>();
            }

            return data;
        }
    }
}