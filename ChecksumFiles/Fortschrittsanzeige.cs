using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChecksumFiles
{
    class Fortschrittsanzeige
    {

        int Fortschritt_Gesamt = 0;
        int Fortschritt_Jetzt  = 0;
        int Fortschritt_Last   = 0;

        public void Init(int gesamtmenge)
        {
            Fortschritt_Gesamt = gesamtmenge;
            Fortschritt_Jetzt  = 0;
            Fortschritt_Last   = 0;
        }

        public void Print()
        {
            if (Fortschritt_Gesamt == 0) 
                return;
            Fortschritt_Jetzt++;
            int Prozent = (Fortschritt_Jetzt  * 100) / Fortschritt_Gesamt;
            if (Prozent > Fortschritt_Last)
            {
                System.Console.WriteLine("  {0} %", Prozent);
                Fortschritt_Last = Prozent; 
            }
        }
    }
}
