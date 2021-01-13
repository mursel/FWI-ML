using System;

namespace MainApp.Models
{
    public class DataModel
    {
        public DateTime Datum { get; set; }
        public int Mjesec { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public double RelativeHumidity { get; set; }
        public double Precipitation { get; set; }
        public double FFMC { get; set; }
        public double DMC { get; set; }
        public double DC { get; set; }
        public double ISI { get; set; }
        public double BUI { get; set; }
        public double FWI { get; set; }
        public double DSR { get; set; }
        public short Fire { get; set; }


        //public override string ToString()
        //{
        //    return $"T: {this.Temperature}, RH: {this.RelativeHumidity}, PR: {this.Precipitation}, WS: {this.WindSpeed}, " +
        //    "FFMC: {this.FFMC}, DMC: {this.DMC}, DC: {this.DC}, ISI: {this.ISI}, BUI: {this.BUI}, FWI: {this.FWI} ";
        //}
    }

}