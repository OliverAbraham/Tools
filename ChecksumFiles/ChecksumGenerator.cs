using System;
using System.IO;
using System.Security.Cryptography;

namespace ChecksumFiles
{
    class ChecksumGenerator
    {
        /*  The checksum method below comes almost verbatim from Jeff Barnes, MS MVP, and his blog article posted here:
        http://jeffbarnes.net/blog/post/2007/01/12/File-Checksum-using-NET.aspx
        */

        public static string GetChecksum(string m_fileinput)
        {
            string m_checksum;
            using (FileStream stream = File.OpenRead(m_fileinput))
            {
                SHA256Managed sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(stream);
                m_checksum = BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
            return m_checksum;
        }

    }
}
