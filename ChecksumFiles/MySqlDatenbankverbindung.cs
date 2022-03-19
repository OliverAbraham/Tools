using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChecksumFiles
{
    public class MySqlDatenbankverbindung
    {
        #region ------------- Eigenschaften -------------------------------------------------------
        
        public MySqlConnection Connection         { get; set; }
        public string          LetzteSqlAnweisung { get; set; }

        #endregion



        #region ------------- Init ----------------------------------------------------------------

        string ConnectionString;
        readonly object Locker = new object();

        public MySqlDatenbankverbindung(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #endregion

        
        
        #region ------------- Methoden ------------------------------------------------------------

        public void Connect()
        {
            Connection = new MySqlConnection(ConnectionString);
            Connection.Open();
        }

        public void Disconnect()
        {
            Connection.Close();
        }

        public ChecksumRow SingleSelect(string sql)
        {
            List<ChecksumRow> Results = MultiSelect(sql);
            if (Results.Count() == 0)
                return null;
            else
                return Results[0];
        }

        public List<ChecksumRow> MultiSelect(string sql)
        {
            LetzteSqlAnweisung = sql;
            List<ChecksumRow> Results = new List<ChecksumRow>();

            lock (Locker)
            {
                var Reader = ExecuteReader(sql);
                if (Reader.HasRows)
                {
                    if (Reader.Read())
                    {
                        ChecksumRow Row = new ChecksumRow()
                        {
                            ID          = Reader.GetInt32(0),
                            FILEPATH    = Reader.GetString(1),
                            FILENAME    = Reader.GetString(2),
                            SHA256      = Reader.GetString(3),
                            MOD_DATE    = Reader.GetDateTime(4),
                            DELETED     = Reader.GetChar(5),
                            REPORTED    = Reader.GetChar(6),
                            FILESIZE    = Reader.GetInt64(7),
                            ATTRIBUTES  = Reader.GetString(8)
                        };
                        Reader.Close();
                        Results.Add(Row);
                    }
                }
                Reader.Close();
            }
            return Results;
        }

        public MySqlDataReader ExecuteReader(string sql)
        {
            MySqlCommand Cmd = new MySqlCommand(sql, Connection);
            try
            {
                return Cmd.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                System.Console.WriteLine("Exception bei ExecuteReader: {0}", ex.ToString());
                Connect();
                return Cmd.ExecuteReader();
            }
        }

        public int SkalarSelect(string sql)
        {
            LetzteSqlAnweisung = sql;
            MySqlCommand Cmd = new MySqlCommand(sql, Connection);
            object Result;

            lock (Locker)
            {
                try
                {
                    Result = Cmd.ExecuteScalar();
                }
                catch (MySqlException)
                {
                    Connect();
                    Result = Cmd.ExecuteScalar();
                }
            }
            if (Result == null)
                throw new Exception("Datensatz nicht gefunden bei SkalarSelect, sql=" + sql);
            return Convert.ToInt32(Result);
        }

        public int NonQuery(string sql)
        {
            LetzteSqlAnweisung = sql;
            MySqlCommand Cmd = new MySqlCommand(sql, Connection);

            lock (Locker)
            {
                try
                {
                    return Cmd.ExecuteNonQuery();
                }
                catch (MySqlException)
                {
                    Connect();
                    return Cmd.ExecuteNonQuery();
                }
            }
        }

        public string CodeValue(string eingabe)
        {
            string Ausgabe = "";
            for (int i = 0; i < eingabe.Length; i++)
            {
                if (eingabe[i] == '\\')
                    Ausgabe += eingabe[i];
                else if (eingabe[i] == '\'')
                    Ausgabe += eingabe[i];
                Ausgabe += eingabe[i];
            }
            return Ausgabe;
        }

        public string DecodeValue(string eingabe)
        {
            // Kleine Optimierung:
            if (!eingabe.Contains('\\'))
                return eingabe;

            // Das Character-Destuffing können wir nicht mit "Replace" machen,
            // weil der z.B. aus "\\\\servername\\xxx" dies hier macht: "\servername\xxx".
            // Das ist falsch, das Ergebnis muss "\\servername\xxx" sein.
            // Replace entfernt bei mehrfach hintereinander vorkommenden Stopfzeichen
            // also zuviele.

            string Ausgabe = "";
            for (int i = 0; i < eingabe.Length; i++)
            {
                if (eingabe[i] == '\\' && i < eingabe.Length-1)
                    i++;
                else if (eingabe[i] == '\'' && i < eingabe.Length-1 && eingabe[i+1] == '\'')
                    i++;
                Ausgabe += eingabe[i];
            }
            return Ausgabe;
        }

        #endregion
    }
}
