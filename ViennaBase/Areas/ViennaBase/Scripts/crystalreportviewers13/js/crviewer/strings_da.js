/* Copyright (c) Business Objects 2006. All rights reserved. */

var L_bobj_crv_MainReport = "Hovedrapport";
// Viewer Toolbar tooltips
var L_bobj_crv_FirstPage = "G\u00E5 til f\u00F8rste side";
var L_bobj_crv_PrevPage = "G\u00E5 til forrige side";
var L_bobj_crv_NextPage = "G\u00E5 til n\u00E6ste side";
var L_bobj_crv_LastPage = "G\u00E5 til sidste side";
var L_bobj_crv_ParamPanel = "Parameterpanel";
var L_bobj_crv_Parameters = "Parametre";
var L_bobj_crv_GroupTree = "Gruppetr\u00E6";
var L_bobj_crv_DrillUp = "Analyser stigende";
var L_bobj_crv_Refresh = "Opdater rapport";
var L_bobj_crv_Zoom = "Zoom";
var L_bobj_crv_PageNav = "Sidenavigation";
var L_bobj_crv_SelectPage = "G\u00E5 til side";
var L_bobj_crv_SearchText = "S\u00F8g efter tekst";
var L_bobj_crv_Export = "Eksporter denne rapport";
var L_bobj_crv_Print = "Udskriv denne rapport";
var L_bobj_crv_TabList = "Faneliste";
var L_bobj_crv_Close = "Luk";
var L_bobj_crv_Logo=  "Business Objects-logo";
var L_bobj_crv_FileMenu = "Menuen Filer";

var L_bobj_crv_File = "Filer";

var L_bobj_crv_Show = "Vis";
var L_bobj_crv_Hide = "Skjul";

var L_bobj_crv_Find = "S\u00F8g...";
var L_bobj_crv_of = "%1 af %2"; // Example: Page "1 of 3"

var L_bobj_crv_submitBtnLbl = "Eksporter";
var L_bobj_crv_ActiveXPrintDialogTitle = "Udskriv";
var L_bobj_crv_PDFPrintDialogTitle = "Udskriv som PDF";
var L_bobj_crv_PrintRangeLbl = "Sideomr\u00E5de:";
var L_bobj_crv_PrintAllLbl = "Alle sider";
var L_bobj_crv_PrintPagesLbl = "V\u00E6lg sider";
var L_bobj_crv_PrintFromLbl = "Fra:";
var L_bobj_crv_PrintToLbl = "Til:";
var L_bobj_crv_PrintInfoTitle = "Udskriv som PDF:";
var L_bobj_crv_PrintInfo1 = 'Fremviseren skal eksportere til PDF for at udskrive. V\u00E6lg indstillingen Udskriv i PDF-l\u00E6serprogrammet, n\u00E5r dokumentet er \u00E5bnet.';
var L_bobj_crv_PrintInfo2 = 'Bem\u00E6rk: Du skal have en PDF-l\u00E6ser installeret. (f.eks. Adobe Reader)';
var L_bobj_crv_PrintPageRangeError = "Indtast et gyldigt sideomr\u00E5de.";

var L_bobj_crv_ExportBtnLbl = "Eksporter";
var L_bobj_crv_ExportDialogTitle = "Eksporter";
var L_bobj_crv_ExportFormatLbl = "Filformat:";
var L_bobj_crv_ExportInfoTitle = "Til eksport:";

var L_bobj_crv_ParamsApply = "Anvend";
var L_bobj_crv_ParamsAdvDlg = "Rediger parameterv\u00E6rdi";
var L_bobj_crv_ParamsDeleteTooltip = "Slet parameterv\u00E6rdi";
var L_bobj_crv_ParamsAddValue = "Klik for at tilf\u00F8je...";
var L_bobj_crv_ParamsApplyTip = "Knappen Anvend (aktiveret)";
var L_bobj_crv_ParamsApplyDisabledTip = "Knappen Anvend (deaktiveret)";
var L_bobj_crv_ParamsDlgTitle = "Indtast v\u00E6rdier";
var L_bobj_crv_ParamsCalBtn = "Knappen Kalender";
var L_bobj_crv_Reset= "Nulstil";
var L_bobj_crv_ResetTip = "Knappen Nulstil (aktiveret)";
var L_bobj_crv_ResetDisabledTip = "Knappen Nulstil (deaktiveret)";
var L_bobj_crv_ParamsDirtyTip = "Parameterv\u00E6rdi er \u00E6ndret. Klik p\u00E5 knappen Anvend for at anvende \u00E6ndringer.";
var L_bobj_crv_ParamsDataTip = "Dette er en parameter til datahentning";
var L_bobj_crv_ParamsMaxNumDefaultValues = "Klik her for at f\u00E5 flere elementer...";
var L_bobj_crv_paramsOpenAdvance = "Avanceret foresp\u00F8rgselsknap til \'%1\'";

var L_bobj_crv_ParamsInvalidTitle = "Parameterv\u00E6rdien er ikke gyldig";
var L_bobj_crv_ParamsTooLong = "Parameterv\u00E6rdien m\u00E5 ikke v\u00E6re mere end %1 tegn lang";
var L_bobj_crv_ParamsTooShort = "Parameterv\u00E6rdien skal v\u00E6re mindst %1 tegn lang";
var L_bobj_crv_ParamsBadNumber = "Denne parameter er af typen \"Tal\" and can only contain a negative sign symbol, digits (\"0-9\"), digit grouping symbols or a decimal symbol.";
var L_bobj_crv_ParamsBadCurrency = "Denne parameter er af typen \"Valuta\" og m\u00E5 kun indeholde symbolet for negative tal, cifrene (\"0-9\"), ciffergruppeseparatorer eller et decimaltegn. Ret den indtastede parameterv\u00E6rdi.";
var L_bobj_crv_ParamsBadDate = "Denne parameter er af typen \"Dato\", og det korrekte format er \"%1\", hvor \"yyyy\" er det fircifrede \u00E5rstal, \"mm\" er m\u00E5neden (f.eks. januar = 1), og \"dd\" er dagen i m\u00E5neden.";
var L_bobj_crv_ParamsBadTime = "Denne parameter er af typen \"Klokkesl\u00E6t\" og skal v\u00E6re i formatet \"hh:mm:ss\", hvor \"hh\" er timer i et 24-timers ur, \"mm\" er minutter, og \"ss\" er sekunder.";
var L_bobj_crv_ParamsBadDateTime = "Denne parameter er af typen \"Dato/klokkesl\u00E6t\", og det aktuelle format er \"%1 hh:mm:ss\". \"yyyy\" er det fircifrede \u00E5rstal, \"mm\" er m\u00E5neden (f.eks. januar = 1), \"dd\" er dagen i m\u00E5neden, \"hh\" er timer i et 24-timers ur, \"mm\" er minutter, og \"ss\" er sekunder.";
var L_bobj_crv_ParamsMinTooltip = "Angiv en %1-v\u00E6rdi, der er st\u00F8rre end eller lig med %2.";
var L_bobj_crv_ParamsMaxTooltip = "Angiv en %1-v\u00E6rdi, der er mindre end eller lig med %2.";
var L_bobj_crv_ParamsMinAndMaxTooltip = "Angiv en %1-v\u00E6rdi mellem %2 og %3.";
var L_bobj_crv_ParamsStringMinOrMaxTooltip = "%1-l\u00E6ngden for dette felt er %2.";
var L_bobj_crv_ParamsStringMinAndMaxTooltip = "V\u00E6rdien skal v\u00E6re mellem %1 og %2 tegn lang.";
var L_bobj_crv_ParamsYearToken = "\u00E5\u00E5\u00E5\u00E5";
var L_bobj_crv_ParamsMonthToken = "mm";
var L_bobj_crv_ParamsDayToken = "dd";
var L_bobj_crv_ParamsReadOnly = "Denne parameter er af typen \"Skrivebeskyttet\".";
var L_bobj_crv_ParamsNoValue = "Ingen v\u00E6rdi";
var L_bobj_crv_ParamsDuplicateValue = "Duplikerede v\u00E6rdier er ikke tilladt.";
var L_bobj_crv_ParamsEnterOptional = "Indtast %1 (valgfrit)";
var L_bobj_crv_ParamsNoneSelected= "(Ingen valgte)";
var L_bobj_crv_ParamsClearValues= "Slet v\u00E6rdier";
var L_bobj_crv_ParamsMoreValues= "%1 flere v\u00E6rdier...";
var L_bobj_crv_ParamsMoreValue= "%1 mere v\u00E6rdi...";
var L_bobj_crv_Error = "Fejl";
var L_bobj_crv_OK = "OK";
var L_bobj_crv_Cancel = "Annuller";
var L_bobj_crv_showDetails = "Vis detaljer";
var L_bobj_crv_hideDetails = "Skjul detaljer";
var L_bobj_crv_RequestError = "Det var ikke muligt at behandle din anmodning";
var L_bobj_crv_ServletMissing = "Fremviseren kunne ikke oprette forbindelse til CrystalReportViewerServlet, der h\u00E5ndterer asynkrone anmodninger.\nKontroller, at Servlet\'en og Servlet-Mapping er defineret i programmets web.xml-fil.";
var L_bobj_crv_FlashRequired = "Dette indhold kr\u00E6ver Adobe Flash Player 9 eller h\u00F8jere. {0}Klik her for at installere";
var L_bobj_crv_ReadOnlyInPanel= "Denne parameter kan ikke redigeres i dette panel. \u00C5bn den avancerede foresp\u00F8rgselsdialog for at \u00E6ndre dens v\u00E6rdi";

var L_bobj_crv_Tree_Drilldown_Node = "Detaljeudledningsnode %1";

var L_bobj_crv_ReportProcessingMessage = "Vent, mens dokumentet behandles.";
var L_bobj_crv_PrintControlProcessingMessage = "Vent mens Crystal Reports-udskriftskontrol indl\u00E6ses.";

var L_bobj_crv_SundayShort = "S";
var L_bobj_crv_MondayShort = "M";
var L_bobj_crv_TuesdayShort = "T";
var L_bobj_crv_WednesdayShort = "O";
var L_bobj_crv_ThursdayShort = "T";
var L_bobj_crv_FridayShort = "F";
var L_bobj_crv_SaturdayShort = "S";

var L_bobj_crv_Minimum = "minimum";
var L_bobj_crv_Maximum = "maksimum";

var L_bobj_crv_Date = "Dato";
var L_bobj_crv_Time = "Klokkesl\u00E6t";
var L_bobj_crv_DateTime = "Dato/klokkesl\u00E6t";
var L_bobj_crv_Boolean = "Boolesk";
var L_bobj_crv_Number = "Tal";
var L_bobj_crv_Text = "Tekst";

var L_bobj_crv_InteractiveParam_NoAjax = "Webbrowseren, du anvender, er ikke konfigureret til at vise parameterpanelet.";
var L_bobj_crv_AdvancedDialog_NoAjax= "Fremviseren kan ikke \u00E5bne en avanceret foresp\u00F8rgselsdialog.";

var L_bobj_crv_EnableAjax= "Kontakt din administrator for at aktivere asynkrone anmodninger.";

var L_bobj_crv_LastRefreshed = "Senest opdateret";

var L_bobj_crv_Collapse = "Skjul";

var L_bobj_crv_CatalystTip = "Online-ressourcer";
