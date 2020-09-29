using BspCore;
using MainApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Services
{
    public class DataLoaderService : IDataLoader
    {
        private FileStream fileStream;
        private StreamReader streamReader;

        #region Properties

        public int NumberOfRecords { get; set; }
        public int CurrentPosition { get; set; }

        #endregion

        public int Load(string _fileName)
        {
            try
            {
                fileStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                streamReader = new StreamReader(fileStream);
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
                fileStream.Close();
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
                // just checking
                if (streamReader == null) return new List<DataModel>();
                
                // RelVlaznost;TempZraka;Padavine24;BrzinaVjetraKMh

                FWIService fWIService = new FWIService();

                string line = await streamReader.ReadLineAsync();

                while (line != null)
                {
                    string[] fields = line.Split(';');

                    CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                    cultureInfo.NumberFormat.NumberDecimalSeparator = ",";

                    float RelVlaznost = float.Parse(fields[0], cultureInfo); //, System.Globalization.NumberStyles.Float);
                    float TempZraka = float.Parse(fields[1], cultureInfo);
                    float Padavine24 = float.Parse(fields[2], cultureInfo);
                    float BrzinaVjetraKMh = float.Parse(fields[3], cultureInfo);

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

                    // get next one
                    line = await streamReader.ReadLineAsync();
                }
            }
            catch (System.Exception)
            {
                return new List<DataModel>();
            }

            //streamReader.Close();

            return data;
        }

        public int Save()
        {
            throw new NotImplementedException();
        }
    }
}