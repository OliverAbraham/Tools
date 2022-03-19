using System;
using System.Text;
using Abraham.String;

namespace Abraham.Text
{
    /// <summary>
    /// 
    /// </summary>
    public class CodeFormatter
    {
        #region ------------- Eigenschaften -------------------------------------------------------

        public bool Linefeeds_before_keywords { get; set; } = true;
        public bool Linefeeds_after_keywords  { get; set; } = true;

        public int Indentation1 { get; set; } = 0;
        public int Indentation2 { get; set; } = 4;

        #endregion



        #region ------------- Felder --------------------------------------------------------------

        private string[] _Keywords =
        {
            "SELECT",
            "INTO",
            "FROM",
            "WHERE",
            "AND",
            "OR",
            "NOT",
            "EXISTS",
            "GROUP BY",
            "ORDER BY",
            "JOIN",
            "HAVING",
        };

        private const string CHARSET = "abcdefghijklmnopqrstuvwxyzüöäßABCDEFGHIJKLMNOPQRSTUVWXYZÜÖÄ0123456789_";

        #endregion



        #region ------------- Init ----------------------------------------------------------------
        #endregion



        #region ------------- Methoden ------------------------------------------------------------

        public string Format(string sql_command)
        {
            string s = sql_command;
            s = Remove_all_unnecessary_content_from_SQL(s);
            s = Convert_all_sql_keywords_to_UPPERCASE(s);

            if (Linefeeds_before_keywords || Linefeeds_after_keywords)
                s = Insert_Linefeeds_at_specific_points(s);

            s = Insert_linefeeds_in_select_clause(s);
            s = Insert_linefeeds_in_into_clause(s);
            s = Insert_linefeeds_in_from_clause(s);
            s = Insert_linefeeds_in_where_clause(s);
            s = s.Replace("=\"", "=$@\"");
            return s;
        }

        public string Remove_all_unnecessary_content_from_SQL(string sql_command)
        {
            string s = sql_command;
            s = Remove_all_unnecessary_characters_outside(s);
            s = Remove_all_unnecessary_characters_inside(s);
            s = Remove_all_unnecessary_characters_inside(s);
            s = Remove_all_unnecessary_characters_inside2(s);
            s = Remove_all_unnecessary_concatenations(s);
            s = Replace_addition_by_string_interpolation(s);
            s = Remove_all_unnecessary_characters_inside(s);
            s = Remove_all_unnecessary_characters_inside3(s);
            s = Remove_all_unnecessary_characters_inside(s);
            return s;
        }

        public string Convert_all_sql_keywords_to_UPPERCASE(string sql_command)
        {
            string s = sql_command;

            foreach (string keyword in _Keywords)
                s = Convert_to_UPPERCASE(s, keyword);

            return s;
        }

        public string Insert_Linefeeds_at_specific_points(string sql_command)
        {
            string s = sql_command;
            s = Insert_linefeed_at_keyword(s, "SELECT");
            s = Insert_linefeed_at_keyword(s, "INTO");
            s = Insert_linefeed_at_keyword(s, "FROM");
            s = Insert_linefeed_at_keyword(s, "WHERE");
            s = Insert_linefeed_at_keyword(s, "ORDER BY");
            return s;
        }

        #endregion



        #region ------------- Implementation ------------------------------------------------------

        private string Remove_all_unnecessary_characters_outside(string s)
        {
            // Zustandsautomat
            bool InString = false;
            for (int pos = 0; pos < s.Length; pos++)
            {
                Recognize_if_inside_outside_csharp_string(s, pos, ref InString);

                if (!InString)
                {
                    if (s[pos] == '\n' || s[pos] == '\r' || s[pos] == '\t' || s[pos] == ' ')
                    {
                        s = s.Remove(pos, 1);
                        pos--;
                    }
                }
            }

            return s;
        }

        private string Remove_all_unnecessary_characters_inside(string s)
        {
            // Zustandsautomat
            bool InString = false;
            for (int pos = 0; pos < s.Length; pos++)
            {
                Recognize_if_inside_outside_csharp_string(s, pos, ref InString);

                if (InString)
                {
                    if (s[pos] == '\t')
                    {
                        s = s.Remove(pos, 1);
                        s = s.Insert(pos, " ");
                    }
                    if (pos+1 < s.Length)
                    {
                        if (s[pos] == ' ' && s[pos+1] == ' ')
                        {
                            s = s.Remove(pos, 1);
                            pos--;
                            pos--;
                        }
                    }
                }
            }

            return s;
        }

        private string Remove_all_unnecessary_characters_inside2(string s)
        {
            bool InString = false;
            for (int pos = 0; pos < s.Length; pos++)
            {
                Recognize_if_inside_outside_csharp_string(s, pos, ref InString);

                if (InString)
                {
                    if (pos + 4 < s.Length)
                    {
                        string sub = s.Substring(pos, 4);
                        if (sub == "\\r\\n")
                        {
                            s = s.Remove(pos, 4);
                            s = s.Insert(pos, " ");
                            pos--;
                            pos--;
                            pos--;
                        }
                    }
                }
            }

            return s;
        }

        private string Remove_all_unnecessary_characters_inside3(string s)
        {
            bool InString = false;
            for (int pos = 0; pos < s.Length; pos++)
            {
                Recognize_if_inside_outside_csharp_string(s, pos, ref InString);

                if (InString)
                {
                    if (pos + 4 < s.Length)
                    {
                        string sub = s.Substring(pos, 4);
                        if (sub == "\\r\\n")
                        {
                            s = s.Remove(pos, 4);
                            s = s.Insert(pos, " ");
                        }
                    }
                }
            }

            return s;
        }

        private string Remove_all_unnecessary_concatenations(string s)
        {
            bool InString = false;
            for (int pos = 0; pos < s.Length; pos++)
            {
                if (InString)
                {
                    if (pos + 3 < s.Length)
                    {
                        string sub = s.Substring(pos, 3);
                        if (sub == "\"+\"")
                        {
                            s = s.Remove(pos, 3);
                        }
                    }
                }

                Recognize_if_inside_outside_csharp_string(s, pos, ref InString);
            }

            return s;
        }

        private void Recognize_if_inside_outside_csharp_string(string s, int pos, ref bool InString)
        {
            if (!InString)
            {
                if (s[pos] == '\"')
                {
                    InString = true;
                }
            }
            else
            {
                if (s[pos] == '\"')
                {
                    InString = false;
                }
            }
        }

        private string Replace_addition_by_string_interpolation(string s)
        {
            bool InString = false;
            bool We_have_started_string_interpolation = false;
            for (int pos = 0; pos < s.Length; pos++)
            {
                if (InString)
                {
                    // We_are_leaving_a_string:
                    if (pos + 2 < s.Length)
                    {
                        string sub = s.Substring(pos, 2);
                        if (sub == "\"+")
                        {
                            s = s.Remove(pos, 2);
                            s = s.Insert(pos, "{");
                            pos--;
                            We_have_started_string_interpolation = true;
                            InString = false;
                        }
                    }
                }

                if (!InString)
                {
                    if (We_have_started_string_interpolation)
                    {
                        if (pos + 2 < s.Length)
                        {
                            string sub = s.Substring(pos, 2);
                            if (sub == "+\"")
                            {
                                s = s.Remove(pos, 2);
                                s = s.Insert(pos, "}");
                                pos--;
                                InString = true;
                            }
                        }
                    }
                }

                Recognize_if_inside_outside_csharp_string(s, pos, ref InString);
            }

            return s;
        }

        private static void Insert_Sequence_in_front_of_first_quotation_mark(ref string s, ref bool FirstTime, ref int pos)
        {
            if (FirstTime)
            {
                FirstTime = false;
                s = s.Insert(pos, "$@");
                pos += 2;
            }
        }

        private string Convert_to_UPPERCASE(string s, string keyword)
        {
            string keywordUpper = keyword.ToUpper();
            string Result = s;
            int position = 0;
            do
            {
                Result = Result.ReplaceOneTimeWholeWordOnly(keyword, keywordUpper, CHARSET, ref position);
            }
            while (position < Result.Length);
            return Result;

            //int KeywordLength = keyword.Length;
            //string KeywordAllUpper = keyword.ToUpper();
            //int i = 0;
            //bool InString = false;
            //for (int pos = 0; pos < s.Length; pos++)
            //{
            //    if (InString)
            //    {
            //        if (pos + KeywordLength < s.Length)
            //        {
            //            string sub = s.Substring(pos, KeywordLength);
            //            if (sub.ToUpper() == KeywordAllUpper)
            //            {
            //                if (sub != KeywordAllUpper)
            //                {
            //                    s = s.Remove(pos, KeywordLength);
            //                    s = s.Insert(pos, KeywordAllUpper);
            //                }
            //            }
            //        }
            //    }

            //    Recognize_if_inside_outside_csharp_string(s, pos, ref InString);
            //}

            //return s;
        }

        private string Insert_linefeed_at_keyword(string s, string keyword)
        {
            string Indentation1_String = new string(' ', Indentation1);
            string Indentation2_String = new string(' ', Indentation2);

            int KeywordLength = keyword.Length;
            string KeywordAllUpper = keyword.ToUpper();
            bool InString = false;
            for (int pos = 0; pos < s.Length; pos++)
            {
                if (InString)
                {
                    if (pos + KeywordLength < s.Length)
                    {
                        string sub = s.Substring(pos, KeywordLength);
                        if (sub.ToUpper() == KeywordAllUpper)
                        {
                            if (Linefeeds_before_keywords)
                            {
                                s = s.Insert(pos, "\r\n");
                                pos += "\r\n".Length;
                            }

                            if (Indentation1 > 0)
                            {
                                s = s.Insert(pos, Indentation1_String);
                                pos += Indentation1;
                            }

                            pos += KeywordLength;
                            if (Linefeeds_after_keywords)
                            {
                                if (s[pos] == ' ')
                                    s = s.Remove(pos, 1);
                                s = s.Insert(pos, "\r\n" + Indentation2_String);
                                pos += "\r\n".Length + Indentation2;
                            }
                            break;
                        }
                    }
                }

                Recognize_if_inside_outside_csharp_string(s, pos, ref InString);
            }

            return s;
        }

        private string Insert_linefeeds_in_select_clause(string s)
        {
            string Starting_keyword = "SELECT";
            string Ending_keyword   = "INTO";
            string Token            = ",";
            return Insert_linefeeds_in_range(s, Starting_keyword, Ending_keyword, Token);
        }

        private string Insert_linefeeds_in_into_clause(string s)
        {
            string Starting_keyword = "INTO";
            string Ending_keyword   = "FROM";
            string Token            = ":";
            return Insert_linefeeds_in_range(s, Starting_keyword, Ending_keyword, Token, insert_before_token:true, skip_first_occurrence:true);
        }

        private string Insert_linefeeds_in_from_clause(string s)
        {
            string Starting_keyword = "FROM";
            string Ending_keyword   = "WHERE";
            string Token            = ",";
            return Insert_linefeeds_in_range(s, Starting_keyword, Ending_keyword, Token);
        }

        private string Insert_linefeeds_in_where_clause(string s)
        {
            string Starting_keyword = "WHERE";
            string Ending_keyword   = "ORDER BY";
            string Token            = "AND";
            return Insert_linefeeds_in_range(s, Starting_keyword, Ending_keyword, Token, insert_before_token:true);
        }

        private string Insert_linefeeds_in_range(string s, 
                                                 string Starting_keyword,
                                                 string Ending_keyword,
                                                 string Token,
                                                 bool   insert_before_token = false,
                                                 bool   skip_first_occurrence = false)
        {
            string InsertedText = "\r\n";
            if (Indentation2 > 0)
            {
                InsertedText += new string(' ', Indentation2);
            }

            int Tokenlength = Token.Length;
            int KeywordLength = Starting_keyword.Length;
            string KeywordAllUpper = Starting_keyword.ToUpper();
            bool InString = false;
            bool Started = false;
            bool First_Occurrence = true;

            for (int pos = 0; pos < s.Length; pos++)
            {
                if (InString)
                {
                    if (pos + KeywordLength < s.Length)
                    {
                        string sub = s.Substring(pos, KeywordLength);
                        if (sub.ToUpper() == KeywordAllUpper)
                        {
                            Started = true;
                        }
                    }

                    if (pos + Tokenlength < s.Length)
                    {
                        string Found = s.Substring(pos, Tokenlength);
                        if (Started && Found == Token)
                        {
                            if (!(skip_first_occurrence && First_Occurrence))
                            {
                                int Position = (insert_before_token) ? pos : pos+1;
                                s = s.Insert(Position, InsertedText);
                                pos += InsertedText.Length;
                            }
                            pos += 2;
                            First_Occurrence = false;
                        }
                    }

                    if (pos + Ending_keyword.Length < s.Length)
                    {
                        string sub2 = s.Substring(pos, Ending_keyword.Length);
                        if (sub2.ToUpper() == Ending_keyword)
                            break;
                    }
                }

                Recognize_if_inside_outside_csharp_string(s, pos, ref InString);
            }

            return s;
        }
        #endregion
    }
}
