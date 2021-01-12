using BspCore;
using MainApp.Models;
using MainApp.Service.Interfaces;
using System;
using System.Linq;
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

        private List<DataModel> data;

        public List<DataModel> Data
        {
            get { return data; }
            set { data = value; }
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



        public async Task<List<DataModel>> GetAllAsync(string _fileName)
        {
            data = new List<DataModel>();
            try
            {
                using (StreamReader streamReader2 = new StreamReader(_fileName))
                {
                    // just checking
                    if (streamReader2 == null) this.Load(_fileName);

                    // Mjeseci;RelVlaznost;TempZraka;Padavine24;BrzinaVjetraKMh

                    FWIService fWIService = new FWIService();
                                        
                    string line = await streamReader2.ReadLineAsync();  // header! not needed right now...
                    line = await streamReader2.ReadLineAsync();

                    double ffmc0 = 85, dmc0 = 6, dc0 = 15, isi = 0.0, bui = 0.0, fwi = 0.0;    // init
                    double ffmc = 0.0, dmc = 0.0, dc = 0.0, dsr = 0.0;                           //

                    DateTime dateTime = new DateTime(2010, 7, 31);

                    while (line != null)
                    {
                        string[] fields = line.Split(';');

                        CultureInfo cultureInfo = CultureInfo.GetCultureInfo("bs-Latn-BA");
                        //cultureInfo.NumberFormat.NumberDecimalSeparator = ",";

                        int mjesec = short.Parse(fields[0], cultureInfo);
                        double RelVlaznost = double.Parse(fields[1], cultureInfo); //, System.Globalization.NumberStyles.double);
                        double TempZraka = double.Parse(fields[2], cultureInfo);
                        double Padavine24 = double.Parse(fields[3], cultureInfo);
                        double BrzinaVjetraKMh = double.Parse(fields[4], cultureInfo);

                        //_temps.Add(TempZraka);

                        ffmc = fWIService.FFMCcalc(TempZraka, RelVlaznost, BrzinaVjetraKMh, Padavine24, ffmc0);
                        dmc = fWIService.DMCcalc(TempZraka, RelVlaznost, Padavine24, dmc0, mjesec);   
                        dc = fWIService.DCcalc(TempZraka, Padavine24, dc0, mjesec);
                        isi = fWIService.ISIcalc(ffmc, BrzinaVjetraKMh);
                        bui = fWIService.BUIcalc(dmc, dc);
                        fwi = fWIService.FWIcalc(isi, bui);
                        dsr = fWIService.DSRCalc(fwi);

                        DataModel dataModel = new DataModel()
                        {
                            Datum = dateTime,
                            Mjesec = mjesec,
                            RelativeHumidity = Math.Round(RelVlaznost, 4),
                            Temperature = Math.Round(TempZraka, 4),
                            Precipitation = Math.Round(Padavine24, 4),
                            WindSpeed = Math.Round(BrzinaVjetraKMh, 4),
                            FFMC = Math.Round(ffmc, 4),
                            DMC = Math.Round(dmc, 4),
                            DC = Math.Round(dc, 4),
                            ISI = Math.Round(isi, 4),
                            BUI = Math.Round(bui, 4),
                            FWI = Math.Round(fwi, 4),
                            DSR = Math.Round(dsr, 4),
                            Fire = (short)Math.Round((fwi >= 11.2) ? 1.0 : 0.0, 0)
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

            Data = data;

            return data;
        }

        /// <summary>
        /// Ostaviti mogucnost proslijedjivanja indexa kolona koje nas interesuju
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public double[][] ToArray(int[] columns)
        {
            double[][] newData = new double[data.Count][];

            if (columns.Length > 0)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    newData[i] = new double[columns.Length + 1];

                    for (int j = 0; j < columns.Length; j++)
                    {
                        /* 1 - temp         5 - ffmc
                         * 2 - wind         6 - dmc
                         * 3 - humidity     7 - dc
                         * 4 - rain         8 - isi
                         *                  9 - bui
                         *                  10 - fwi
                         */
                        var index = columns[j];
                        switch (index)
                        {
                            case 1:
                                newData[i][j] = data[i].Temperature;
                                break;
                            case 2:
                                newData[i][j] = data[i].WindSpeed;
                                break;
                            case 3:
                                newData[i][j] = data[i].RelativeHumidity;
                                break;
                            case 4:
                                newData[i][j] = data[i].Precipitation;
                                break;
                            case 5:
                                newData[i][j] = data[i].FFMC;
                                break;
                            case 6:
                                newData[i][j] = data[i].DMC;
                                break;
                            case 7:
                                newData[i][j] = data[i].DC;
                                break;
                            case 8:
                                newData[i][j] = data[i].ISI;
                                break;
                            case 9:
                                newData[i][j] = data[i].BUI;
                                break;
                            case 10:
                                newData[i][j] = data[i].FWI;
                                break;
                            default:
                                break;
                        }
                        
                    }
                    
                    newData[i][columns.Length] = data[i].Fire;
                }
            }
            else {

                for (int i = 0; i < data.Count; i++)
                {
                    newData[i] = new double[14];
                    newData[i][0] = data[i].Datum.ToOADate();
                    newData[i][1] = data[i].Mjesec;
                    newData[i][2] = data[i].RelativeHumidity;
                    newData[i][3] = data[i].Temperature;
                    newData[i][4] = data[i].Precipitation;
                    newData[i][5] = data[i].WindSpeed;
                    newData[i][6] = data[i].FFMC;
                    newData[i][7] = data[i].DMC;
                    newData[i][8] = data[i].DC;
                    newData[i][9] = data[i].ISI;
                    newData[i][10] = data[i].BUI;
                    newData[i][11] = data[i].FWI;
                    newData[i][12] = data[i].DSR;
                    newData[i][13] = data[i].Fire;
                }
            }

            return newData;
        }


        /// <summary>
        /// Ostaviti mogucnost indeksiranja svih kolona 
        /// </summary>
        /// <returns></returns>
        public double[][] ToArray()
        {
            return ToArray(new int[] { });
        }

        public int Save()
        {
            throw new NotImplementedException();
        }

        public double[] GetAllByColumnIndex(int index)
        {
           /* 1 - temp         5 - ffmc
            * 2 - wind         6 - dmc
            * 3 - humidity     7 - dc
            * 4 - rain         8 - isi
            *                  9 - bui
            *                  10 - fwi
            */
           double[] tempData = new double[D]
            switch (index)
            {
                case 1:
                    newData[i][j] = data[i].Temperature;
                    break;
                case 2:
                    newData[i][j] = data[i].WindSpeed;
                    break;
                case 3:
                    newData[i][j] = data[i].RelativeHumidity;
                    break;
                case 4:
                    newData[i][j] = data[i].Precipitation;
                    break;
                case 5:
                    newData[i][j] = data[i].FFMC;
                    break;
                case 6:
                    newData[i][j] = data[i].DMC;
                    break;
                case 7:
                    newData[i][j] = data[i].DC;
                    break;
                case 8:
                    newData[i][j] = data[i].ISI;
                    break;
                case 9:
                    newData[i][j] = data[i].BUI;
                    break;
                case 10:
                    newData[i][j] = data[i].FWI;
                    break;
                default:
                    break;
            }
        }
    }
}