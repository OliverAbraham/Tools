using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChecksumFiles
{
    public class ChecksumRow
    {
        public int      ID;
        public string   FILEPATH;
        public string   FILENAME;
        public string   SHA256;
        public DateTime MOD_DATE;
        public char     DELETED;
        public char     REPORTED;
        public Int64    FILESIZE;
        public string   ATTRIBUTES;
        public string   Action;
        private string Pfad;
        private System.IO.FileInfo Info;

        public ChecksumRow()
        {
        }

        public ChecksumRow(string Pfad, System.IO.FileInfo Info)
        {
            // TODO: Complete member initialization
            this.Pfad = Pfad;
            this.Info = Info;

            FILEPATH    = Pfad;
            FILENAME    = Path.GetFileName(Pfad);
            MOD_DATE    = Info.LastWriteTime;
            REPORTED    = 'N';
            DELETED     = 'N';
            FILESIZE    = Info.Length;
            ATTRIBUTES  = ConvertFileAttributes(Info.Attributes);
        }

        private string ConvertFileAttributes(FileAttributes fileAttributes)
        {
            string s = "";
            if ((fileAttributes & FileAttributes.Archive             ) != 0) s += "A";
            if ((fileAttributes & FileAttributes.Compressed          ) != 0) s += "C";
            if ((fileAttributes & FileAttributes.Directory           ) != 0) s += "D";
            if ((fileAttributes & FileAttributes.Encrypted           ) != 0) s += "E";
            if ((fileAttributes & FileAttributes.Hidden              ) != 0) s += "H";
            if ((fileAttributes & FileAttributes.NotContentIndexed   ) != 0) s += "N";
            if ((fileAttributes & FileAttributes.Offline             ) != 0) s += "O";
            if ((fileAttributes & FileAttributes.ReadOnly            ) != 0) s += "R";
            if ((fileAttributes & FileAttributes.System              ) != 0) s += "S";
            if ((fileAttributes & FileAttributes.Temporary           ) != 0) s += "T";
            return s;
        }
    }
}
