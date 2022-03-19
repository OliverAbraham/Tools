				sSelect = "\r\n" +
				"SELECT 	D.TANNR,\r\n" +
				"	D.GARTKNAME,\r\n" +
				"	D.GSPNr,\r\n" +
				"	D.GSPNRPARTNER,	\r\n" +
				"	D.DSPNR,\r\n" +
				"	D.BSPNR, \r\n" +
				"	D.ANLAGEAUFNAHME, \r\n" +
				"	DK.ISONominal,\r\n" +
				"	DK.LaufzeitBis, 	\r\n" +
				"	DK.NOMINALBTR\r\n	" + sSelectGesellschaft + "\r\n	" + i_OK_select + "\r\n" +
				"INTO 	:dlgDAR_Grundschuldenuebersicht.i_nTANNr, 	\r\n" +
				"	:dlgDAR_Grundschuldenuebersicht.w_sGARTKNAME,\r\n" +
				"	:dlgDAR_Grundschuldenuebersicht.w_nGSPNR,\r\n" +
				"	:dlgDAR_Grundschuldenuebersicht.w_nGSPNrPartner, \r\n" +
				"	:dlgDAR_Grundschuldenuebersicht.w_nDSPNR,	\r\n" +
				"	:dlgDAR_Grundschuldenuebersicht.w_nBSPNR, \r\n" +
				"	:dlgDAR_Grundschuldenuebersicht.w_bAufnahme, \r\n" +
				"	:dlgDAR_Grundschuldenuebersicht.w_sISONominal,\r\n" +
				"	:dlgDAR_Grundschuldenuebersicht.w_dtLZEnde_and,		\r\n" +
				"	:dlgDAR_Grundschuldenuebersicht.w_nNominalBtr \r\n	" + sIntoGesellschaft + "\r\n	" + i_OK_Into + "\r\n" +
				"FROM 	DARLEHEN D, DARKONTRAKT DK " + sFromGesellschaft + i_OK_From + "\r\n" +
				"WHERE 	D.AUFLOESEN = 0\r\n" + sWhereExtendDarlehen + "\r\n" + sWhereExtendDarObjZuo + "\r\n" +
				"and DK.TANNR = D.TANNR\r\n" +
				"AND 	DK.KONTRAKTNR = ( SELECT MAX( KontraktNr ) FROM DARKONTRAKT\r\n" +
				"			 	WHERE TANNR = DK.TANNR\r\n" +
				"				AND LaufzeitVon <= " + HanseOrga.WebFinanceSolutions.FW.Global.Konst.Common.Int.DB_DateToString(dfdDatumBis.DateTime) + "\r\n" +
				"				AND FREIGABE = 1\r\n" +
				"				AND ZTPLANNEUAB IS NOT NULL\r\n			)\r\n" + sWhereExtendDarKontrakt + "\r\n" + sWhereExtendDarOrdnung + "\r\n" + sWhereGesellschaft + "\r\n" + Int.fWhereDarAnwNr("D.") + "\r\n" +
				"ORDER BY  " + i_OK_OrderBy + " D.GARTKNAME " + sOrderByGSPNr + ", DK.ISONominal,\r\n" +
				"D.GSPNRPARTNER, D.DSPNR, D.BSPNR\r\n";
