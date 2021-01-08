namespace MainApp.Service.Interfaces
{
    public interface ICFWIService
    {
        double FFMCcalc(double T, double H, double W, double Ro, double Fo);
        double DMCcalc(double T, double H, double Ro, double Po, int I);
        double DCcalc(double T, double Ro, double Do, int I);
        double ISIcalc(double F, double W);
        double BUIcalc(double P, double D);
        double FWIcalc(double R, double U);

    }
}