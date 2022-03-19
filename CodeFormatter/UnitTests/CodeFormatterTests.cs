using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Abraham.Text;
using Abraham.String;
using FluentAssertions;

namespace UnitTests
{
    [TestClass]
    public class CodeFormatter_should
    {
        [TestMethod]
        public void Find_a_substring_case_insensitive()
        {
            string Input = "ABCDEFG";
            int Result = Input.IndexOf_CaseInsensitive("abc");
            Result.Should().Be(0);

            Result = Input.IndexOf_CaseInsensitive("bcd");
            Result.Should().Be(1);

            Result = Input.IndexOf_CaseInsensitive("efg");
            Result.Should().Be(4);

            Result = Input.IndexOf_CaseInsensitive("fgh");
            Result.Should().Be(-1);

            Result = Input.IndexOf_CaseInsensitive("");
            Result.Should().Be(-1);

            Result = Input.IndexOf_CaseInsensitive("ABCDEFG");
            Result.Should().Be(0);

            Result = Input.IndexOf_CaseInsensitive("abcdefg");
            Result.Should().Be(0);
        }

        [TestMethod]
        public void Remove_unneccesary_content_from_input1()
        {
        	CodeFormatter Formatter = new CodeFormatter();
            string Input = File.ReadAllText(@"..\..\..\Testdaten1.cs");
            string Output = Formatter.Remove_all_unnecessary_content_from_SQL(Input);
            Output.Should().Be("sSelect=\"SELECT NOMINALBTR {sSelectGesellschaft} WHERE\";");
        }

        [TestMethod]
        public void Remove_unneccesary_content_from_input2()
        {
        	CodeFormatter Formatter = new CodeFormatter();
            string Input = File.ReadAllText(@"..\..\..\Testdaten2.cs");
            string Output = Formatter.Remove_all_unnecessary_content_from_SQL(Input);
            Output.Should().Be("sSelect=\" SELECT D.TANNR, DK.NOMINALBTR {sSelectGesellschaft} {i_OK_select} INTO :dlgDAR_Grundschuldenuebersicht.i_nTANNr, FROM DARLEHEN D, DARKONTRAKT DK {sFromGesellschaft+i_OK_From} WHERE D.AUFLOESEN = 0 {sWhereExtendDarlehen} {sWhereExtendDarObjZuo} D.GSPNRPARTNER\";");
        }

        [TestMethod]
        public void Convert_all_keywords_to_uppercase()
        {
        	CodeFormatter Formatter = new CodeFormatter();
            string Input = "sSelect=\" select bla into intoxxx from xxfrom whERe bla existS ordER by bla\";";
            string Output = Formatter.Convert_all_sql_keywords_to_UPPERCASE(Input);
            Output.Should().Be("sSelect=\" SELECT bla INTO intoxxx FROM xxfrom WHERE bla EXISTS ORDER BY bla\";");
        }

        [TestMethod]
        public void Format_correctly_1()
        {
        	CodeFormatter Formatter = new CodeFormatter();
            string Input = "sSelect=\" select bla INTO :bla from BLA whERe bla existS ordER by bla\";";
            string Output = Formatter.Format(Input);
            Output.Should().Be("sSelect=$@\" \r\nSELECT\r\n    bla \r\nINTO\r\n    :bla \r\nFROM\r\n    BLA \r\nWHERE\r\n    bla EXISTS \r\nORDER BY\r\n    bla\";");
        }
 
        [TestMethod]
        public void Format_correctly_2()
        {
        	CodeFormatter Formatter = new CodeFormatter();
            string Input = File.ReadAllText(@"..\..\..\Testdaten3.cs");
            //Formatter.Indentation1 = 20;
            //Formatter.Indentation2 = 20 + 8;
            string Output = Formatter.Format(Input);
            Output.Should().Be("sSelect=$@\" \r\nSELECT\r\n    D.TANNR,\r\n     D.GARTKNAME,\r\n     D.GSPNr,\r\n     D.GSPNRPARTNER,\r\n     D.DSPNR,\r\n     D.BSPNR,\r\n     D.ANLAGEAUFNAHME,\r\n     DK.ISONominal,\r\n     DK.LaufzeitBis,\r\n     DK.NOMINALBTR {sSelectGesellschaft} {i_OK_select} \r\nINTO\r\n    :dlgDAR_Grundschuldenuebersicht.i_nTANNr, \r\n    :dlgDAR_Grundschuldenuebersicht.w_sGARTKNAME, \r\n    :dlgDAR_Grundschuldenuebersicht.w_nGSPNR, \r\n    :dlgDAR_Grundschuldenuebersicht.w_nGSPNrPartner, \r\n    :dlgDAR_Grundschuldenuebersicht.w_nDSPNR, \r\n    :dlgDAR_Grundschuldenuebersicht.w_nBSPNR, \r\n    :dlgDAR_Grundschuldenuebersicht.w_bAufnahme, \r\n    :dlgDAR_Grundschuldenuebersicht.w_sISONominal, \r\n    :dlgDAR_Grundschuldenuebersicht.w_dtLZEnde, \r\n    :dlgDAR_Grundschuldenuebersicht.w_nNominalBtr {sIntoGesellschaft} {i_OK_Into} \r\nFROM\r\n    DARLEHEN D,\r\n     DARKONTRAKT DK {sFromGesellschaft+i_OK_From} \r\nWHERE\r\n    D.AUFLOESEN = 0 {sWhereExtendDarlehen} {sWhereExtendDarObjZuo} \r\n    AND DK.TANNR = D.TANNR \r\n    AND DK.KONTRAKTNR = ( SELECT MAX( KontraktNr ) FROM DARKONTRAKT WHERE TANNR = DK.TANNR \r\n    AND LaufzeitVon <= {HanseOrga.WebFinanceSolutions.FW.Global.Konst.Common.Int.DB_DateToString(dfdDatumBis.DateTime)} \r\n    AND FREIGABE = 1 \r\n    AND ZTPLANNEUAB IS NOT NULL ) {sWhereExtendDarKontrakt} {sWhereExtendDarOrdnung} {sWhereGesellschaft} {Int.fWhereDarAnwNr(\"D.\")} \r\nORDER BY\r\n    {i_OK_OrderBy} D.GARTKNAME {sOrderByGSPNr}, DK.ISONominal, D.GSPNRPARTNER, D.DSPNR, D.BSPNR \";");
        }
    }
}
