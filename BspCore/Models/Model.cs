namespace BspCore
{
    public class DataModel
    {
        public int Mjesec { get; set; }
        public double Temperature { get; set; }
        public float WindSpeed { get; set; }
        public float RelativeHumidity { get; set; }
        public float Precipitation { get; set; }
        public float FFMC { get; set; }
        public float DMC { get; set; }
        public float DC { get; set; }
        public float ISI { get; set; }
        public float BUI { get; set; }
        public float FWI { get; set; }
        public float DSR { get; set; }

        //public override string ToString()
        //{
        //    return $"T: {this.Temperature}, RH: {this.RelativeHumidity}, PR: {this.Precipitation}, WS: {this.WindSpeed}, " +
        //    "FFMC: {this.FFMC}, DMC: {this.DMC}, DC: {this.DC}, ISI: {this.ISI}, BUI: {this.BUI}, FWI: {this.FWI} ";
        //}
    }
}