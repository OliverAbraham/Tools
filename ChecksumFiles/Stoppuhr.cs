//-------------------------------------------------------------------------------------------------
//
//                                  PRORIS 3
//                      Stoppuhr zur Ermittlung von Laufzeiten
//
//                      Oliver Abraham, Inveos GmbH, 5/2012
//                      oliver.abraham@inveos.com
//
//-------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inveos.DateTimeExtensions
{
    /// <summary>
    /// Stellt Funktionen für einfache Zeitmessung zur Verfügung.
    /// Nur "new" aufrufen und später die Eigenschaft "Millisekunden" abfragen.
    /// </summary>
    /// 
    /// <example>
    /// Stoppuhr Uhr = new Stoppuhr();
    /// ...... irgendwas machen ......
    /// Writeln("Benötigte zeit: " + Uhr.Millisekunden);
    /// </example>
    public class Stoppuhr
    {
        /// <summary>
        ///  Zeitpunkt, zu dem der Konstruktor lief bzw. der letzte Aufruf von "Reset" erfolgte.
        /// </summary>
        public System.DateTime Startzeit;


        /// <summary>
        /// Erzeugt ein neues Objekt und setzt die Startzeit.
        /// </summary>
        public Stoppuhr()
        {
            Reset();
        }

        /// <summary>
        /// Setzt die Startzeit auf Jetzt.
        /// </summary>
        public void Reset()
        {
            Startzeit = System.DateTime.Now;
        }

        /// <summary>
        /// Verstrichene Timer-Ticks (1 Tick = 100 Nanosekunden)
        /// </summary>
        public long Ticks
        {
            get { return System.DateTime.Now.Ticks - Startzeit.Ticks; }
        }

        /// <summary>
        /// Verstrichene Millisekunden
        /// </summary>
        public long Millisekunden
        {
            get { return Ticks / 10000; }
        }

        /// <summary>
        /// Verstrichene Sekunden
        /// </summary>
        public long Sekunden
        {
            get { return Millisekunden / 1000; }
        }

        public string MinutenSekunden
        {
            get 
            { 
                long Sek = Sekunden % 60;
                long Min = Sekunden / 60;
                return string.Format("{0:0.00}:{1:0.00}", Min, Sek);
            }
        }

        /// <summary>
        /// Verstrichene Zeitspanne (Jahre/Tage/Stunden/Minuten/Sekunden/Milisekunden/Ticks)
        /// </summary>
        public TimeSpan Zeit
        {
            get { return new TimeSpan(Ticks); }
        }
    }
}
