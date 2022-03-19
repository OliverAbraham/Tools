using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace ChecksumFiles
{
    public class EmailAnhang
    {
        public string DateinameEcht;
        public string DateinameAngezeigt;

        public EmailAnhang(string dateinameEcht, string dateinameAngezeigt)
        {
            DateinameEcht      = dateinameEcht;
            DateinameAngezeigt = dateinameAngezeigt;
        }
    }

    public class Email
    {
        public string            SmtpServer    { get; set; }
        public string            SmtpUser      { get; set; }
        public string            SmtpPassword  { get; set; }
        public int               SmtpPort      { get; set; }
        public bool              SmtpEnableSSL { get; set; }
        public int               SmtpTimeout   { get; set; }
        public List<EmailAnhang> Anhänge       { get; set; }


        public Email(string server   ,
                     string user     ,
                     string password ,
                     int    port     ,
                     bool   enableSSL,
                     int    timeout  )
        {
            SmtpServer    = server    ;
            SmtpUser      = user      ;
            SmtpPassword  = password  ;
            SmtpPort      = port      ;
            SmtpEnableSSL = enableSSL ;
            SmtpTimeout   = timeout   ;
            Anhänge = new List<EmailAnhang>();
        }


        public void Sende(string from, string to, string title, string body)
        {
            // Email configuration and send
            var Client = new SmtpClient(SmtpServer, SmtpPort)
            {
                Credentials = new NetworkCredential(SmtpUser, SmtpPassword),
                EnableSsl   = SmtpEnableSSL,
                Timeout     = SmtpTimeout * 1000
            };

            MailMessage Message = new MailMessage(to, from, title, body);

            if (Anhänge.Count > 0)
                foreach(var Anhang in Anhänge)
                    Message.Attachments.Add(Konvertiere(Anhang));

            Client.Send(Message);

            Anhänge = new List<EmailAnhang>();
        }

        public void ErzeugeAnhang(string dateiname, string dateiname_angezeigt)
        {
            if (!File.Exists(dateiname))
                throw new Exception("Die Datei existiert nicht");
            Anhänge.Add(new EmailAnhang(dateiname, dateiname_angezeigt));
        }

        public Attachment Konvertiere(EmailAnhang anhang)
        {
            Attachment a = new Attachment(anhang.DateinameEcht, MediaTypeNames.Application.Octet);
            a.Name = anhang.DateinameAngezeigt;
            //ContentDisposition Timestamp = a.ContentDisposition;
            //Timestamp.CreationDate     = System.IO.File.GetCreationTime  (anhang.DateinameEcht);
            //Timestamp.ModificationDate = System.IO.File.GetLastWriteTime (anhang.DateinameEcht);
            //Timestamp.ReadDate         = System.IO.File.GetLastAccessTime(anhang.DateinameEcht);
            return a;
        }
    }
}
