using System;
using System.Collections.Generic;
using System.IO;
using BspCore;

namespace BspCore
{
    public class DataLoaderService : IDataLoader
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
                
                // RelVlaznost;TempZraka;Padavine24;BrzinaVjetraKMh

                while ((line= await streamReader.ReadLineAsync())!=null)
                {
                    string[] fields = line.Split(';');

                    float RelVlaznost = float.Parse(fields[0]); //, System.Globalization.NumberStyles.Float);
                    float TempZraka = float.Parse(fields[1]);
                    float Padavine24 = float.Parse(fields[2]);
                    float BrzinaVjetraKMh = float.Parse(fields[3]);

                    

                    // DataModel dataModel = new DataModel() {
                    //     Temperature = fields[0];

                    // }
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