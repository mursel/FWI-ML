using BspCore;
using MainApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private List<float> _temps;

        public List<float> Temperatures
        {
            get { return _temps; }
            set { _temps = value; }
        }

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
                using (StreamReader streamReader2 = new StreamReader("dataset_final.csv"))
                {
                    // just checking
                    if (streamReader2 == null) this.Load("dataset_final.csv");

                    // Mjeseci;RelVlaznost;TempZraka;Padavine24;BrzinaVjetraKMh

                    FWIService fWIService = new FWIService();
                                        
                    string line = await streamReader2.ReadLineAsync();  // header! not needed right now...
                    line = await streamReader2.ReadLineAsync();

                    float ffmc0 = 85f, dmc0 = 6f, dc0 = 15f, isi = 0.0f, bui = 0.0f, fwi = 0.0f;    // init
                    float ffmc = 0.0f, dmc = 0.0f, dc = 0.0f, dsr = 0.0f;                           //

                    DateTime dateTime = new DateTime(2010, 7, 31);

                    while (line != null)
                    {
                        string[] fields = line.Split(';');

                        CultureInfo cultureInfo = CultureInfo.GetCultureInfo("bs-Latn-BA");
                        //cultureInfo.NumberFormat.NumberDecimalSeparator = ",";

                        int mjesec = short.Parse(fields[0], cultureInfo);
                        float RelVlaznost = float.Parse(fields[1], cultureInfo); //, System.Globalization.NumberStyles.Float);
                        float TempZraka = float.Parse(fields[2], cultureInfo);
                        float Padavine24 = float.Parse(fields[3], cultureInfo);
                        float BrzinaVjetraKMh = float.Parse(fields[4], cultureInfo);

                        //_temps.Add(TempZraka);

                        fWIService.FFMCcalc(TempZraka, RelVlaznost, BrzinaVjetraKMh, Padavine24, ffmc0, ref ffmc);
                        fWIService.DMCcalc(TempZraka, RelVlaznost, Padavine24, dmc0, mjesec, ref dmc);   
                        fWIService.DCcalc(TempZraka, Padavine24, dc0, mjesec, ref dc);
                        fWIService.ISIcalc(ffmc, BrzinaVjetraKMh, ref isi);
                        fWIService.BUIcalc(dmc, dc, ref bui);
                        fWIService.FWIcalc(isi, bui, ref fwi);
                        fWIService.DSRCalc(fwi, ref dsr);

                        DataModel dataModel = new DataModel()
                        {
                            Datum = dateTime,
                            Mjesec = mjesec,
                            RelativeHumidity = (float)Math.Round(RelVlaznost, 2),
                            Temperature = (float)Math.Round(TempZraka, 2),
                            Precipitation = (float)Math.Round(Padavine24, 2),
                            WindSpeed = (float)Math.Round(BrzinaVjetraKMh, 2),
                            FFMC = (float)Math.Round(ffmc, 2),
                            DMC = (float)Math.Round(dmc, 2),
                            DC = (float)Math.Round(dc, 2),
                            ISI = (float)Math.Round(isi, 2),
                            BUI = (float)Math.Round(bui, 2),
                            FWI = (float)Math.Round(fwi, 2),
                            DSR = (float)Math.Round(dsr, 2),
                            Fire = (short)Math.Round((fwi > 20) ? 1.0 : 0.0, 0)
                        };

                        data.Add(dataModel);
                        CurrentPosition++;

                        dateTime = dateTime.AddDays(1);

                        // get next one
                        line = await streamReader2.ReadLineAsync();

                        // set previous values as input on next iteration
                        ffmc0 = ffmc;
                        dmc0 = dmc;
                        dc0 = dc;
                    }

                    //streamReader2.Close();
                }
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Greška!", ex.InnerException);
                //return new List<DataModel>();
            }

            return data;
        }

        public int Save()
        {
            throw new NotImplementedException();
        }
    }
}