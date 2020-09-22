namespace BspCore
{
    public class DataModel
    {
        float Temperature { get; set; }
        float WindSpeed { get; set; }
        float RelativeHumidity { get; set; }
        float Precipitation { get; set; }
        float FFMC { get; set; }
        float DMC { get; set; }
        float DC { get; set; }
        float ISI { get; set; }
        float BUI { get; set; }
        float FWI { get; set; }

        public override string ToString()
        {
            return $"T: {this.Temperature}, RH: {this.RelativeHumidity}, PR: {this.Precipitation}, WS: {this.WindSpeed}, " +
            "FFMC: {this.FFMC}, DMC: {this.DMC}, DC: {this.DC}, ISI: {this.ISI}, BUI: {this.BUI}, FWI: {this.FWI} ";
        }
    }
}