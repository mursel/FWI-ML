/*
    Java to C# conversion is based on source code in Table 5. from article:

    Updated source code for calculating fire danger indices in the Canadian Forest Fire Weather Index System. 2015. 
    Wang, Y.; Anderson, K.R.; Suddaby, R.M. Natural Resources Canada, Canadian Forest Service, Northern Forestry Centre, 
    Edmonton, Alberta. Information Report NOR-X-424.
    (https://d1ied5g1xfgpx8.cloudfront.net/pdfs/36461.pdf)

    and as described in article

    Development and structure of the Canadian Forest Fire Weather Index System. 1987. Van Wagner, C.E. 
    Canadian Forestry Service, Headquarters, Ottawa. Forestry Technical Report.
    (https://d1ied5g1xfgpx8.cloudfront.net/pdfs/19927.pdf)

    Direct conversion to C# created by Mursel Musabasic, Gorazde, September 2020.
*/

using MainApp.Service.Interfaces;
using System;

namespace MainApp
{
    public class FWIService : ICFWIService
    {

        public double FFMCcalc(double T, double H, double W, double Ro, double Fo)
        {
            double Mo, Rf, Ed, Ew, M, Kl, Kw, Mr, Ko, Kd,F;
            Mo = 147.2 * (101.0- Fo) / (59.5 + Fo); /*Eq. 1 in */
            if (Ro > 0.5)
            { /*van Wagner and Pickett (1985)*/
                Rf = Ro - 0.5; /*Eq.2*/
                if (Mo <= 150.0)
                    Mr = Mo + 42.5 * Rf * (Math.Exp(-100.0/ (251.0- Mo))) * (1 - Math.Exp(-6.93 / Rf)); /*Eq. 3a*/
                else
                    Mr = Mo + 42.5 * Rf * (Math.Exp(-100.0/ (251.0- Mo))) * (1 - Math.Exp(-6.93 / Rf)) + 
                        .0015 * Math.Pow(Mo - 150.0, 2.0) * Math.Pow(Rf, 0.5); /*Eq. 3b*/
                if (Mr > 250.0) Mr = 250.0;
                Mo = Mr;
            }
            Ed = 0.942 * Math.Pow(H, .679) + 11.0* Math.Exp((H - 100.0) / 10.0) + .18 * (21.1 - T) * (1.0- Math.Exp(-.115 * H)); /*Eq. 4*/
            if (Mo > Ed)
            {
                Ko = 0.424 * (1.0- Math.Pow(H / 100.0, 1.7))+ 0.0694 * Math.Pow(W, .5) * (1.0- Math.Pow(H / 100.0, 8.0)); /*Eq. 6a*/
                Kd = Ko * .581 * Math.Exp(0.0365 * T); /*Eq. 6b*/
                M = Ed + (Mo - Ed) * Math.Pow(10.0, -Kd); /*Eq. 8*/
            }
            else
            {
                Ew = 0.618 * Math.Pow(H, .753) + 10.0* Math.Exp((H - 100.0) / 10.0) + .18 * (21.1 - T) * (1.0- Math.Exp(-.115 * H)); /*Eq. 5*/
                if (Mo < Ew)
                {
                    Kl = 0.424 * (1.0- Math.Pow((100.0- H) / 100.0, 1.7)) + 0.0694 * Math.Pow(W, .5) * (1 - Math.Pow((100.0- H) / 100.0, 8.0)); /*Eq. 7a*/
                    Kw = Kl * .581 * Math.Exp(0.0365 * T); /*Eq. 7b*/
                    M = Ew - (Ew - Mo) * Math.Pow(10.0, -Kw); /*Eq. 9*/
                }
                else M = Mo;
            }
            /*Finally calculate FFMC */
            F = (59.5 * (250.0 - M)) / (147.2 + M);
            /*..............................*/
            /*Make sure 0. <= FFMC <= 101.0 */
            /*..............................*/
            if (F > 101.0) F = 101.0;
            //if (F <= 0.0) F = 0.0;
            return F;
        }

        /* DMC calculation */
        public double DMCcalc(double T, double H, double Ro, double Po, int I)
        {
            double Re, Mo, Mr, K, B, P, Pr;
            double[] Le = { 6.5, 7.5, 9.0, 12.8, 13.9, 13.9, 12.4, 10.9, 9.4, 8.0, 7.0, 6.0};
            if (T >= -1.1) 
                K = 1.894 * (T + 1.1) * (100.0- H) * Le[I - 1] * 0.0001;
            else 
                K = 0.0;
            if (Ro <= 1.5) { Pr = Po; }
            else
            {
                Re = 0.92 * Ro - 1.27;
                Mo = 20.0 + 280.0 / Math.Exp(0.023 * Po);
                if (Po <= 33.0) { B = 100.0 / (0.5 + 0.3 * Po); }
                else
                {
                    if (Po <= 65.0) { B = 14.0 - 1.3 * Math.Log(Po); }
                    else { B = 6.2 * Math.Log(Po) - 17.2; }
                }
                Mr = Mo + 1000.0 * Re / (48.77 + B * Re);
                Pr = 43.43 * (5.6348 - Math.Log(Mr - 20.0));
            }
            if (Pr < 0.0) { Pr = 0.0; }
            P = Pr + K;
            if (P <= 0.0) { P = 0.0; }
            return P;
        }

        /* DC calculation */
        public double DCcalc(double T, double Ro, double Do, int I)
        {
            double Rd, Qo, Qr, V, D, Dr;
            double[] Lf = { -1.6, -1.6, -1.6, 0.9, 3.8, 5.8, 6.4, 5.0, 2.4, 0.4, -1.6, -1.6 };
            if (Ro > 2.8)
            {
                Rd = 0.83 * (Ro) - 1.27;
                Qo = 800.0* Math.Exp(-Do / 400.0);
                Qr = Qo + 3.937 * Rd;
                Dr = 400.0* Math.Log(800.0/ Qr);
                if(Dr > 0.0) Do = Dr;
                    else Do = 0.0;
            }
            if(T > -2.8) V = 0.36 * (T + 2.8) + Lf[I - 1];
            else V = Lf[I - 1];
            if (V < 0.0) V = 0.0;
            D = Do + 0.5 * V;
            return D;
        }

    /* ISI calculation */
        public double ISIcalc(double F, double W)
        {
            double Fw, M, Ff, R;
            M = 147.2 * (101 - F) / (59.5 + F);
            Fw = Math.Exp(0.05039 * W);
            Ff = 91.9 * Math.Exp(-0.1386 * M) * (1.0+ Math.Pow(M, 5.31) / 4.93E7);
            R = 0.208 * Fw * Ff; /*Eq. 26*/
            return R;
        }

        /* BUI calculation */
        public double BUIcalc(double P, double D)
        {
            double U;
            if (P <= .4 * D) U = 0.8 * P * D / (P + .4 * D);
            else U = P - (1.0- .8 * D / (P + .4 * D)) * (.92 + Math.Pow(.0114 * P, 1.7));
            if (U <= 0.0 || double.IsNaN(U)) U = 0.0;
            return U;
        }

        /* FWI calculation */
        public double FWIcalc(double R, double U)
        {
            double Fd, B, S;
            if (U <= 80.0) Fd = .626 * Math.Pow(U, .809) + 2.0;
            else Fd = 1000.0/ (25.0+ 108.64 * Math.Exp(-.023 * U));
            B = .1 * R * Fd;
            if (B > 1.0) S = Math.Exp(2.72 * Math.Pow(.434 * Math.Log(B), .647));
            else S = B;
            return S;
        }
  

    /* Daily Severity Rating calculation for current day*/
        public double DSRCalc(double FWI)
        {
            double dsr = 0.0272 * Math.Pow(FWI, 1.77);
            return dsr;
        }
    }
}