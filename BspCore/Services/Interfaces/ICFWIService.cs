using System;
namespace BspCore
{
    public interface ICFWIService
    {
        void FFMCcalc(float T, float H, float W, float Ro, float Fo, ref float ffmc);
        void DMCcalc(float T,float H,float Ro,float Po,int I, ref float dmc);
        void DCcalc(float T,float Ro,float Do,int I, ref float dc);
        void ISIcalc(float F,float W,ref float isi);
        void BUIcalc(float P,float D, ref float bui);
        void FWIcalc(float R,float U, ref float fwi);

    }
}