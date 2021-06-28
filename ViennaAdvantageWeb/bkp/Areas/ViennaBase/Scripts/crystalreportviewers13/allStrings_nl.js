/* Copyright (c) Business Objects 2006. All rights reserved. */

var L_bobj_crv_MainReport = "Hoofdrapport";
// Viewer Toolbar tooltips
var L_bobj_crv_FirstPage = "Ga naar eerste pagina";
var L_bobj_crv_PrevPage = "Ga naar vorige pagina";
var L_bobj_crv_NextPage = "Ga naar volgende pagina";
var L_bobj_crv_LastPage = "Ga naar laatste pagina";
var L_bobj_crv_ParamPanel = "Parametervenster";
var L_bobj_crv_Parameters = "Parameters";
var L_bobj_crv_GroupTree = "Groepsstructuur";
var L_bobj_crv_DrillUp = "Diepteanalyse";
var L_bobj_crv_Refresh = "Rapport vernieuwenRefresh Report";
var L_bobj_crv_Zoom = "In-/uitzoomen";
var L_bobj_crv_PageNav = "Paginanavigatie";
var L_bobj_crv_SelectPage = "Ga naar pagina";
var L_bobj_crv_SearchText = "Zoeken naar tekst";
var L_bobj_crv_Export = "Dit rapport exporteren";
var L_bobj_crv_Print = "Dit rapport afdrukken";
var L_bobj_crv_TabList = "Tablijst";
var L_bobj_crv_Close = "Sluiten";
var L_bobj_crv_Logo=  "Business Objects-logo";
var L_bobj_crv_FileMenu = "het menu Bestand";

var L_bobj_crv_File = "Bestand";

var L_bobj_crv_Show = "Weergeven";
var L_bobj_crv_Hide = "Verbergen";

var L_bobj_crv_Find = "Zoeken...";
var L_bobj_crv_of = "%1 van %2"; // Example: Page "1 of 3"

var L_bobj_crv_submitBtnLbl = "Exporteren";
var L_bobj_crv_ActiveXPrintDialogTitle = "Afdrukken";
var L_bobj_crv_PDFPrintDialogTitle = "Afdrukken naar PDF";
var L_bobj_crv_PrintRangeLbl = "Paginabereik:";
var L_bobj_crv_PrintAllLbl = "Alle pagina\'s";
var L_bobj_crv_PrintPagesLbl = "Pagina\'s selecteren";
var L_bobj_crv_PrintFromLbl = "Van:";
var L_bobj_crv_PrintToLbl = "Tot:";
var L_bobj_crv_PrintInfoTitle = "Afdrukken naar PDF:";
var L_bobj_crv_PrintInfo1 = 'De viewer moet naar PDF exporteren om te kunnen afdrukken. Kies de optie Afdrukken in de PDF reader wanneer het document is geopend.';
var L_bobj_crv_PrintInfo2 = 'Opmerking: u moet een PDF reader (bijvoorbeeld Adobe Reader) hebben ge\u00EFnstalleerd om te kunnen afdrukken.';
var L_bobj_crv_PrintPageRangeError = "Voer een geldig paginabereik in.";

var L_bobj_crv_ExportBtnLbl = "Exporteren";
var L_bobj_crv_ExportDialogTitle = "Exporteren";
var L_bobj_crv_ExportFormatLbl = "Bestandsindeling:";
var L_bobj_crv_ExportInfoTitle = "U kunt als volgt exporteren:";

var L_bobj_crv_ParamsApply = "Toepassen";
var L_bobj_crv_ParamsAdvDlg = "Parameterwaarde bewerken";
var L_bobj_crv_ParamsDeleteTooltip = "Parameterwaarde verwijderen";
var L_bobj_crv_ParamsAddValue = "Klik om toe te voegen...";
var L_bobj_crv_ParamsApplyTip = "De knop Toepassen (ingeschakeld)";
var L_bobj_crv_ParamsApplyDisabledTip = "Knop Toepassen (uitgeschakeld)";
var L_bobj_crv_ParamsDlgTitle = "Waarden invoeren";
var L_bobj_crv_ParamsCalBtn = "Kalenderknop";
var L_bobj_crv_Reset= "Opnieuw instellen";
var L_bobj_crv_ResetTip = "de knop Opnieuw instellen (ingeschakeld)";
var L_bobj_crv_ResetDisabledTip = "Knop Opnieuw instellen (uitgeschakeld)";
var L_bobj_crv_ParamsDirtyTip = "De parameterwaarde is gewijzigd. Klik op de knop Toepassen om wijzigingen toe te passen.";
var L_bobj_crv_ParamsDataTip = "Dit is een parameter voor het  ophalen van gegevens";
var L_bobj_crv_ParamsMaxNumDefaultValues = "Klik hier voor meer items...";
var L_bobj_crv_paramsOpenAdvance = "Geavanceerde aanwijzingsknop voor \'%1\'";

var L_bobj_crv_ParamsInvalidTitle = "De parameterwaarde is niet geldig";
var L_bobj_crv_ParamsTooLong = "Parameterwaarde mag niet langer zijn dan %1 tekens";
var L_bobj_crv_ParamsTooShort = "Parameterwaarde moet minstens %1 tekens lang zijn";
var L_bobj_crv_ParamsBadNumber = "Deze parameter is van het type \"Getal\" en mag alleen een minteken, cijfers (\"0-9\"), cijfergroepeersymbolen of een decimaalteken bevatten.";
var L_bobj_crv_ParamsBadCurrency = "Deze parameter is van het type \"Valuta\" en mag alleen een minteken, cijfers (\"0-9\"), cijfergroepeersymbolen of een decimaalteken bevatten.";
var L_bobj_crv_ParamsBadDate = "Deze parameter is van het type \"Datum\" en de juiste notatie is \"%1\" waarbij \"jjjj\" is het viercijferige jaartal, \"mm\" de maand (bijvoorbeeld januari = 1) en \"dd\" de dag van de maand zijn.";
var L_bobj_crv_ParamsBadTime = "Deze parameter is van het type \"Tijd\" en de juiste notatie is \"uuh:mm:ss\" waarbij \"uu\" het uur in een 24-uurs klok, \"mm\" de minuten en \"ss\" de seconden zijn.";
var L_bobj_crv_ParamsBadDateTime = "Deze parameter is van het type \"DatumTijd\" en de juiste notatie is \"%1 uu:mm:ss\". \"jjjj\" is het viercijferige jaartal, \"mm\" is de maand (bijvoorbeeld januari = 1), \"dd\" is de dag van de maand, \"uu\" is het uur in een 24-uurs klok, \"mm\" zijn de minuten en \"ss\" de seconden.";
var L_bobj_crv_ParamsMinTooltip = "Geef een %1-waarde op die groter dan of gelijk aan %2 is.";
var L_bobj_crv_ParamsMaxTooltip = "Geef een %1-waarde op die kleiner dan of gelijk aan %2 is.";
var L_bobj_crv_ParamsMinAndMaxTooltip = "Geef een %1-waarde tussen %2 en %3 op.";
var L_bobj_crv_ParamsStringMinOrMaxTooltip = "De %1 lengte voor dit veld is %2.";
var L_bobj_crv_ParamsStringMinAndMaxTooltip = "De waarde moet tussen %1 en %2 tekens lang zijn.";
var L_bobj_crv_ParamsYearToken = "jjjj";
var L_bobj_crv_ParamsMonthToken = "mm";
var L_bobj_crv_ParamsDayToken = "dd";
var L_bobj_crv_ParamsReadOnly = "Deze parameter is van het type \"Alleen-lezen\".";
var L_bobj_crv_ParamsNoValue = "Geen waarde";
var L_bobj_crv_ParamsDuplicateValue = "Dubbele waarden zijn niet toegestaan.";
var L_bobj_crv_ParamsEnterOptional = "Voer %1 in (optioneel)";
var L_bobj_crv_ParamsNoneSelected= "(Geen geselecteerd)";
var L_bobj_crv_ParamsClearValues= "Waarden verwijderen";
var L_bobj_crv_ParamsMoreValues= "nog %1 waarden...";
var L_bobj_crv_ParamsMoreValue= "nog %1 waarde...";
var L_bobj_crv_Error = "FOUT";
var L_bobj_crv_OK = "OK";
var L_bobj_crv_Cancel = "Annuleren";
var L_bobj_crv_showDetails = "Details weergeven";
var L_bobj_crv_hideDetails = "Details verbergen";
var L_bobj_crv_RequestError = "Uw verzoek kan niet worden verwerkt";
var L_bobj_crv_ServletMissing = "De viewer kan geen verbinding maken met de CrystalReportViewerServlet die asynchrone verzoeken afhandelt.\nControleer of de Servlet en Servlet-Mapping correct zijn gedeclareerd in het bestand web.xml van de toepassing.";
var L_bobj_crv_FlashRequired = "Voor  deze inhoud hebt u Adobe Flash Player 9 of later nodig. {0}Klik hier om het programma te installeren.";
var L_bobj_crv_ReadOnlyInPanel= "Deze parameter kan niet worden bewerkt in het paneel. Open het dialoogvenster voor geavanceerde aanwijzingen om de waarde te wijzigen.";

var L_bobj_crv_Tree_Drilldown_Node = "Knooppunt %1 analyseren";

var L_bobj_crv_ReportProcessingMessage = "Wacht totdat het document is verwerkt.";
var L_bobj_crv_PrintControlProcessingMessage = "Een ogenblik geduld terwijl Crystal Reports-afdrukbeheer wordt geladen.";

var L_bobj_crv_SundayShort = "Z";
var L_bobj_crv_MondayShort = "M";
var L_bobj_crv_TuesdayShort = "D";
var L_bobj_crv_WednesdayShort = "W";
var L_bobj_crv_ThursdayShort = "D";
var L_bobj_crv_FridayShort = "V";
var L_bobj_crv_SaturdayShort = "Z";

var L_bobj_crv_Minimum = "minimum";
var L_bobj_crv_Maximum = "maximum";

var L_bobj_crv_Date = "Datum";
var L_bobj_crv_Time = "Tijd";
var L_bobj_crv_DateTime = "Datum en tijd";
var L_bobj_crv_Boolean = "Boolean-waarde";
var L_bobj_crv_Number = "Getal";
var L_bobj_crv_Text = "Tekst";

var L_bobj_crv_InteractiveParam_NoAjax = "De webbrowser die u gebruikt, is niet geconfigureerd om het parametervenster weer te geven.";
var L_bobj_crv_AdvancedDialog_NoAjax= "De viewer kan geen dialoogvenster met een geavanceerde aanwijzing openen.";

var L_bobj_crv_EnableAjax= "Neem contact op met de beheerder om asynchrone verzoeken in te schakelen.";

var L_bobj_crv_LastRefreshed = "Laatst vernieuwd";

var L_bobj_crv_Collapse = "Samenvouwen";

var L_bobj_crv_CatalystTip = "Onlinebronnen";
// <script>
/*
=============================================================
WebIntelligence(r) Report Panel
Copyright(c) 2001-2003 Business Objects S.A.
All rights reserved

Use and support of this software is governed by the terms
and conditions of the software license agreement and support
policy of Business Objects S.A. and/or its subsidiaries. 
The Business Objects products and technology are protected
by the US patent number 5,555,403 and 6,247,008

File: labels.js


=============================================================
*/

_default="Standaard"
_black="Zwart"
_brown="Bruin"
_oliveGreen="Olijfgroen"
_darkGreen="Donkergroen"
_darkTeal="Donker groenblauw"
_navyBlue="Marineblauw"
_indigo="Indigo"
_darkGray="Donkergrijs"
_darkRed="Donkerrood"
_orange="Oranje"
_darkYellow="Donkergeel"
_green="Groen"
_teal="Groenblauw"
_blue="Blauw"
_blueGray="Blauwgrijs"
_mediumGray="Grijs"
_red="Rood"
_lightOrange="Lichtoranje"
_lime="Lichtgroen"
_seaGreen="Zeegroen"
_aqua="Zeeblauw"
_lightBlue="Lichtblauw"
_violet="Violet"
_gray="Grijs"
_magenta="Magenta"
_gold="Goudkleurig"
_yellow="Geel"
_brightGreen="Heldergroen"
_cyan="Cyaan"
_skyBlue="Hemelsblauw"
_plum="Pruimenpaars"
_lightGray="Lichtgrijs"
_pink="Roze"
_tan="Gebruind roze"
_lightYellow="Lichtgeel"
_lightGreen="Lichtgroen"
_lightTurquoise="Lichtturkoois"
_paleBlue="Pastelblauw"
_lavender="Lavendel"
_white="Wit"
_lastUsed="Laatst gebruikt:"
_moreColors="Meer kleuren..."

_month=new Array

_month[0]="Januari"
_month[1]="Februari"
_month[2]="Maart"
_month[3]="April"
_month[4]="Mei"
_month[5]="Juni"
_month[6]="Juli"
_month[7]="Augustus"
_month[8]="September"
_month[9]="Oktober"
_month[10]="November"
_month[11]="December"

_day=new Array
_day[0]="Z"
_day[1]="M"
_day[2]="D"
_day[3]="W"
_day[4]="D"
_day[5]="V"
_day[6]="Z"

_today="Vandaag"

_AM="AM"
_PM="PM"

_closeDialog="Venster sluiten"

_lstMoveUpLab="Omhoog"
_lstMoveDownLab="Omlaag"
_lstMoveLeftLab="Naar links" 
_lstMoveRightLab="Naar rechts"
_lstNewNodeLab="Genest filter toevoegen"
_lstAndLabel="AND"
_lstOrLabel="OR"
_lstSelectedLabel="Geselecteerd"
_lstQuickFilterLab="Snelfilter toevoegen"

_openMenu="Klik hier om opties voor {0} op te roepen"
_openCalendarLab="Kalender openen"

_scroll_first_tab="Naar eerste tab"
_scroll_previous_tab="Naar vorige tab"
_scroll_next_tab="Naar volgende tab"
_scroll_last_tab="Naar laatste tab"

_expandedLab="Uitgevouwen"
_collapsedLab="Samengevouwen"
_selectedLab="Geselecteerd"

_expandNode="Knooppunt %1 uitvouwen"
_collapseNode="Knooppunt %1 samenvouwen"

_checkedPromptLab="Ingesteld"
_nocheckedPromptLab="Niet ingesteld"
_selectionPromptLab="waarden gelijk aan"
_noselectionPromptLab="geen waarden"

_lovTextFieldLab="Typ hier de waarden"
_lovCalendarLab="Typ hier de datum"
_lovPrevChunkLab="Ga naar vorig segment"
_lovNextChunkLab="Ga naar volgend segment"
_lovComboChunkLab="Segment"
_lovRefreshLab="Vernieuwen"
_lovSearchFieldLab="Typ hier de tekst die u zoekt"
_lovSearchLab="Zoeken"
_lovNormalLab="Normaal"
_lovMatchCase="Identieke hoofdletters/kleine letters"
_lovRefreshValuesLab="Waarden vernieuwen"

_calendarNextMonthLab="Ga naar volgende maand"
_calendarPrevMonthLab="Ga naar vorige maand"
_calendarNextYearLab="Ga naar volgend jaar"
_calendarPrevYearLab="Ga naar vorig jaar"
_calendarSelectionLab="Geselecteerde dag"

_menuCheckLab="Ingeschakeld"
_menuDisableLab="Uitgeschakeld"
	
_level="Niveau"
_closeTab="Tabblad sluiten"
_of=" van "

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="Help"

_waitTitleLab="Een ogenblik geduld"
_cancelButtonLab="Annuleren"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="Meer randen..."
_bordersTooltip=new Array
_bordersTooltip[0]="Geen rand"
_bordersTooltip[1]="Linkerrand"
_bordersTooltip[2]="Rechterrand"
_bordersTooltip[3]="Onderrand"
_bordersTooltip[4]="Normale onderrand"
_bordersTooltip[5]="Dikke onderrand"
_bordersTooltip[6]="Boven- en onderrand"
_bordersTooltip[7]="Bovenrand en normale onderrand"
_bordersTooltip[8]="Bovenrand en dikke onderrand"
_bordersTooltip[9]="Alle randen"
_bordersTooltip[10]="Allemaal normale randen"
_bordersTooltip[11]="Allemaal dikke randen"/* Copyright (c) Business Objects 2006. All rights reserved. */

// LOCALIZATION STRING

// Strings for calendar.js and calendar_param.js
var L_Today     = "Vandaag";
var L_January   = "januari";
var L_February  = "februari";
var L_March     = "maart";
var L_April     = "april";
var L_May       = "mei";
var L_June      = "juni";
var L_July      = "juli";
var L_August    = "augustus";
var L_September = "september";
var L_October   = "oktober";
var L_November  = "november";
var L_December  = "december";
var L_Su        = "zo";
var L_Mo        = "ma";
var L_Tu        = "di";
var L_We        = "wo";
var L_Th        = "do";
var L_Fr        = "vr";
var L_Sa        = "za";

// Strings for prompts.js and prompts_param.js
var L_YYYY          = "jjjj";
var L_MM            = "mm";
var L_DD            = "dd";
var L_BadNumber     = "Deze parameter is van het type \"Getal\" en mag alleen een minteken, cijfers (\"0-9\"), cijfergroepeersymbolen of een decimaalteken bevatten. Corrigeer de ingevoerde parameterwaarde.";
var L_BadCurrency   = "Deze parameter is van het type \"Valuta\" en kan alleen bestaan uit een minteken, cijfers (\"0-9\"), cijfergroepeersymbolen of een decimaalteken. Corrigeer de ingevoerde parameterwaarde.";
var L_BadDate       = "Deze parameter is van het type \"Datum\" en moet de notatie \"%1\" hebben, waarbij \"jjjj\" het viercijferige jaar, \"mm\" de maand (bijvoorbeeld januari = 1) en \"dd\" de dag van de maand zijn.";
var L_BadDateTime   = "Deze parameter is van het type \"DatumTijd\" en de juiste notatie is \"%1 uu:mm:ss\". \"jjjj\" is het viercijferige jaartal, \"mm\" is de maand (bijvoorbeeld januari = 1), \"dd\" is de dag van de maand, \"uu\" is het uur in een 24-uurs klok, \"mm\" zijn de minuten en \"ss\" de seconden.";
var L_BadTime       = "Deze parameter is van het type \"Tijd\" en moet de notatie \"uu:mm:ss\" hebben, waarbij \"uu\" het uur in een 24-uurs klok, \"mm\" de minuten en \"ss\" de seconden zijn.";
var L_NoValue       = "Geen waarde";
var L_BadValue      = "Als u \"Geen waarde\" wilt instellen, moet u \"Geen waarde\" instellen in zowel het vak Van als het vak Tot.";
var L_BadBound      = "U kunt niet tegelijk \"Geen ondergrens\" en \"Geen bovengrens\" instellen.";
var L_NoValueAlready = "Deze parameter is al ingesteld op \"Geen waarde\". Verwijder \"Geen waarde\" voordat u andere waarden toevoegt.";
var L_RangeError    = "De beginwaarde van het bereik mag niet groter zijn dan de eindwaarde.";
var L_NoDateEntered = "U moet een datum invoeren.";
var L_Empty         = "Voer een waarde in.";

// Strings for filter dialog
var L_closeDialog="Venster sluiten";

var L_SetFilter = "Filter instellen";
var L_OK        = "OK";
var L_Cancel    = "Annuleren";

 /* Crystal Decisions Confidential Proprietary Information */
