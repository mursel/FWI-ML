using BspCore;
using MainApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MainApp.Services
{
    public class DataLoaderService : IDataLoader
    {
        private StreamReader streamReader;

        #region Properties

        public int NumberOfRecords { get; set; }
        public int CurrentPosition { get; set; }

        #endregion

        public int Load(string _fileName)
        {
            try
            {
                streamReader = File.OpenText(_fileName); //new StreamReader(_fileName);    

            }
            catch (System.Exception ex)
            {
                return ex.HResult;
            }
            finally
            {
                NumberOfRecords = Task.Run(() => File.ReadAllLinesAsync(_fileName)).Result.Length;
                CurrentPosition = 0;
            }
            return 1;
        }
        public int Close()
        {
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
        public IEnumerable<DataModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<List<DataModel>> GetAllAsync()
        {
            List<DataModel> data = new List<DataModel>();

            try
            {
                if (streamReader == null) return new List<DataModel>();

                string line = string.Empty;

                // RelVlaznost;TempZraka;Padavine24;BrzinaVjetraKMh

                FWIService fWIService = new FWIService();

                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    string[] fields = line.Split(';');

                    float RelVlaznost = float.Parse(fields[0]); //, System.Globalization.NumberStyles.Float);
                    float TempZraka = float.Parse(fields[1]);
                    float Padavine24 = float.Parse(fields[2]);
                    float BrzinaVjetraKMh = float.Parse(fields[3]);

                    float ffmc = 85f, dmc = 6f, dc = 15f, isi = 0.0f, bui = 0.0f, fwi = 0.0f;

                    fWIService.FFMCcalc(TempZraka, RelVlaznost, BrzinaVjetraKMh, Padavine24, ffmc, ref ffmc);
                    fWIService.DMCcalc(TempZraka, RelVlaznost, Padavine24, dmc, 9, ref dmc);   // fale dani i mjeseci. Dodati u dataset
                    fWIService.DCcalc(TempZraka, Padavine24, dc, 9, ref dc);
                    fWIService.ISIcalc(ffmc, BrzinaVjetraKMh, ref isi);
                    fWIService.BUIcalc(dmc, dc, ref bui);
                    fWIService.FWIcalc(isi, bui, ref fwi);

                    DataModel dataModel = new DataModel()
                    {
                        RelativeHumidity = RelVlaznost,
                        Temperature = TempZraka,
                        Precipitation = Padavine24,
                        WindSpeed = BrzinaVjetraKMh,
                        FFMC = ffmc,
                        DMC = dmc,
                        DC = dc,
                        ISI = isi,
                        BUI = bui,
                        FWI = fwi
                    };

                    data.Add(dataModel);
                    CurrentPosition++;
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (System.Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return new List<DataModel>();
            }

            return data;
        }

        public int Save()
        {
            throw new NotImplementedException();
        }
    }
}