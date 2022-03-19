				    sSelect = "\r\n" +
				    "SELECT 	D.TANNR,\r\n" +
				    "	DK.NOMINALBTR\r\n	" + sSelectGesellschaft + "\r\n	" + i_OK_select + "\r\n" +
				    "INTO 	:dlgDAR_Grundschuldenuebersicht.i_nTANNr, 	\r\n" +
				    "FROM 	DARLEHEN D, DARKONTRAKT DK " + sFromGesellschaft + i_OK_From + "\r\n" +
				    "WHERE 	D.AUFLOESEN = 0\r\n" + sWhereExtendDarlehen + "\r\n" + sWhereExtendDarObjZuo + "\r\n" +
				    "D.GSPNRPARTNER";
