/*
    Direct C++ to C# conversion is based on source code in Table 3. from article 

    Updated source code for calculating fire danger indices in the Canadian Forest Fire Weather Index System. 2015. 
    Wang, Y.; Anderson, K.R.; Suddaby, R.M. Natural Resources Canada, Canadian Forest Service, Northern Forestry Centre, 
    Edmonton, Alberta. Information Report NOR-X-424. 26 p.
    (https://d1ied5g1xfgpx8.cloudfront.net/pdfs/36461.pdf)

    and as described in article

    Development and structure of the Canadian Forest Fire Weather Index System. 1987. Van Wagner, C.E. 
    Canadian Forestry Service, Headquarters, Ottawa. Forestry Technical Report 35. 35 p.
    (https://d1ied5g1xfgpx8.cloudfront.net/pdfs/19927.pdf)

    Direct conversion to C# created by Mursel Musabasic, Gorazde, September 2020.
*/

using MainApp.Service.Interfaces;
using System;

namespace MainApp
{
    public class FWIService : ICFWIService
    {

        public void FFMCcalc(float T, float H, float W, float Ro, float Fo, ref float ffmc)
        {
            float Mo, Rf, Ed, Ew, M, Kl, Kw, Mr, Ko, Kd;
            Mo = 147.2f * (101.0f - Fo) / (59.5f + Fo); /* Eq. 1 in */
            if (Ro > 0.5f)
            { /* van Wagner and Pickett (1985) */
                Rf = Ro - 0.5f; /* Eq.2 */
                if (Mo <= 150f)
                    Mr = Mo + (float)(42.5 * Rf * (Math.Exp(-100.0 / (251 - Mo))) * (1 - Math.Exp(-6.93 / Rf))); /*Eq. 3a*/
                else
                    Mr = Mo + (float)(42.5 * Rf * (Math.Exp(-100 / (251 - Mo))) * (1 - Math.Exp(-6.93 / Rf)) + 0.0015 * Math.Pow(Mo - 150f, 2) * Math.Pow(Rf, 0.5)); /*Eq. 3b*/
                if (Mr > 250f) Mr = 250f;
                Mo = Mr;
            }
            Ed = (float)(0.942 * Math.Pow(H, 0.679) + 11.0 * Math.Exp((H - 100.0) / 10.0) + 0.18 * (21.1 - T) * (1.0 - Math.Exp(-0.115 * H))); /*Eq. 4*/
            if (Mo > Ed)
            {
                Ko = (float)(0.424 * (1.0 - Math.Pow(H / 100.0, 1.7)) + 0.0694 * Math.Pow(W, .5) * (1.0 - Math.Pow(H / 100.0, 8.0))); /*Eq. 6a*/
                Kd = (float)(Ko * .581 * Math.Exp(0.0365 * T)); /*Eq. 6b*/
                M = Ed + (Mo - Ed) * (float)Math.Pow(10.0, -Kd); /*Eq. 8*/
            }
            else
            {
                Ew = (float)(0.618 * Math.Pow(H, .753) + 10.0 * Math.Exp((H - 100.0) / 10.0) + .18 * (21.1 - T) * (1.0 - Math.Exp(-.115 * H))); /*Eq. 5*/
                if (Mo < Ew)
                {
                    Kl = (float)(0.424 * (1.0 - Math.Pow((100.0 - H) / 100.0, 1.7)) + 0.0694 * Math.Pow(W, .5) * (1 - Math.Pow((100.0 - H) / 100.0, 8.0))); /*Eq. 7a*/
                    Kw = (float)(Kl * .581 * Math.Exp(0.0365 * T)); /*Eq. 7b*/
                    M = (float)(Ew - (Ew - Mo) * Math.Pow(10.0, -Kw)); /*Eq. 9*/
                }
                else M = Mo;
            }
            /*Finally calculate FFMC */
            ffmc = (59.5f * (250.0f - M)) / (147.2f + M);
            /*..............................*/
            /*Make sure 0. <= FFMC <= 101.0 */
            /*..............................*/
            if (ffmc > 101.0) ffmc = 101.0f;
            if (ffmc <= 0.0) ffmc = 0.0f;
        }

        /* DMC calculation */
        public void DMCcalc(float T, float H, float Ro, float Po, int I, ref float dmc)
        {
            float Re, Mo, Mr, K, B, P, Pr;
            float[] Le = { 6.5f, 7.5f, 9f, 12.8f, 13.9f, 13.9f, 12.4f, 10.9f, 9.4f, 8f, 7f, 6f };
            if (T >= -1.1f) K = 1.894f * (T + 1.1f) * (100f - H) * Le[I - 1] * 0.0001f;   /*Eq. 16*/
            else K = 0;                                                     /*Eq. 17*/


            if (Ro <= 1.5f) Pr = Po;
            else
            {
                Re = 0.92f * Ro - 1.27f;                                        /*Eq. 11*/
                Mo = 20f + 280.0f / (float)Math.Exp(0.023 * Po);                /*Eq. 12*/
                if (Po <= 33f) B = 100f / (.5f + .3f * Po);                        /*Eq. 13a*/
                else
                {
                    if (Po <= 65f) B = 14f - 1.3f * (float)Math.Log(Po);       /*Eq. 13b*/
                    else B = 6.2f * (float)Math.Log(Po) - 17.2f;
                }                                                           /*Eq. 13c*/
                Mr = Mo + 1000f * Re / (48.77f + B * Re);                            /*Eq. 14*/
                Pr = 43.43f * (5.6348f - (float)Math.Log(Mr - 20));           /*Eq. 15*/
            }
            if (Pr < 0) Pr = 0.0f;
            P = Pr + K;
            if (P <= 0.0f) P = 0.0f;
            dmc = P;
        }

        /* DC calculation */
        public void DCcalc(float T, float Ro, float Do, int I, ref float dc)
        {
#pragma warning disable CS0168 // The variable 'D' is declared but never used
            float Rd, Qo, Qr, V, D, Dr;
#pragma warning restore CS0168 // The variable 'D' is declared but never used
            float[] Lf = { -1.6f, -1.6f, -1.6f, .9f, 3.8f, 5.8f, 6.4f, 5f, 2.4f, .4f, -1.6f, -1.6f };
            if (Ro > 2.8)
            {
                Rd = 0.83f * (Ro) - 1.27f;                                    /*Eq. 18*/
                Qo = 800f * (float)Math.Exp(-Do / 400);                         /*Eq. 19*/
                Qr = Qo + 3.937f * Rd;                                         /*Eq. 20*/
                Dr = 400f * (float)Math.Log(800 / Qr);                          /*Eq. 21*/
                if (Dr > 0) Do = Dr;
                else Do = 0.0f;
            }
            if (T > -2.8f) V = 0.36f * (T + 2.8f) + Lf[I - 1];                      /*Eq. 22*/
            else V = Lf[I - 1];
            if (V < 0) V = .0f;                                               /*Eq. 23*/
            dc = Do + 0.5f * V;
        }

        /* ISI calculation */
        public void ISIcalc(float F, float W, ref float isi)
        {
            float Fw, M, Ff;
            M = 147.2f * (101f - F) / (59.5f + F);                                  /*Eq. 1*/
            Fw = (float)Math.Exp(0.05039 * W);                                /*Eq. 24*/
            Ff = 91.9f * (float)(Math.Exp(-.1386 * M) * (1f + Math.Pow(M, 5.31) / 4.93E7));    /*Eq. 25*/
            isi = 0.208f * Fw * Ff;                                             /*Eq. 26*/
        }

        /* BUI calculation */
        public void BUIcalc(float P, float D, ref float bui)
        {
            if (P <= .4 * D) bui = 0.8f * P * D / (P + .4f * D);                           /*Eq. 27a*/
            else bui = P - (1 - .8f * D / (P + .4f * D)) * (.92f + (float)Math.Pow(.0114 * P, 1.7));       /*Eq. 27b*/
            if (bui <= 0.0f) bui = 0.0f;
            if (float.IsNaN(bui)) bui = 0.0f;   // check for NaN value - Mursel Musabasic (08.10.2020)
        }

        /* FWI calculation */
        public void FWIcalc(float R, float U, ref float fwi)
        {
#pragma warning disable CS0168 // The variable 'S' is declared but never used
            float Fd, B, S;
#pragma warning restore CS0168 // The variable 'S' is declared but never used
            if (U <= 80f) Fd = .626f * (float)Math.Pow(U, .809) + 2f;                 /*Eq. 28*/
            else Fd = 1000 / (float)(25f + 108.64f * Math.Exp(-.023 * U));              /*Eq. 28b*/
            B = .1f * R * Fd;                                                       /*Eq. 29*/
            if (B > 1f) fwi = (float)Math.Exp(2.72 * Math.Pow(.434 * Math.Log(B), .647)); /*Eq. 30a*/
            else fwi = B;                                                       /*Eq. 30b*/
        }

        /* Daily Severity Rating calculation for current day*/
        public void DSRCalc(float FWI, ref float dsr)
        {
            dsr = 0.0272f * (float)Math.Pow(FWI, 1.77);
        }
    }
}