using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChecksumFiles
{
    enum Änderungsart
    {
        Neu,
        Geändert,
        Verschoben,
        Umbenannt,
        Gelöscht,
        AttributeGeändert,
        Datumsänderung,
        Größenänderung
    }

    class Änderung
    {
        public Änderungsart Art;
        public string       Info;
        public string       Path;

        public Änderung()
        {
        }

        public Änderung(string path, string info, Änderungsart art)
        {
            this.Art  = art;
            this.Path = path;
            this.Info = info;
        }

        public static string ListToString(List<Änderung> änderungen)
        {
            StringBuilder Sb = new StringBuilder();
            foreach(var ä in änderungen)
                Sb.AppendLine(String.Format("{0,-20}  {1,-300}  {2}", ä.Art.ToString("G"), ä.Path, ä.Info));

            return Sb.ToString().Replace("\r\n", "\n");
        }
    }
}
