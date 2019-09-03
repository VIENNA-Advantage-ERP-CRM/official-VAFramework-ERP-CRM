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

_default="Standard"
_black="Sort"
_brown="Brun"
_oliveGreen="Olivengrøn"
_darkGreen="Mørkegrøn"
_darkTeal="Dybblå"
_navyBlue="Marineblå"
_indigo="Indigo"
_darkGray="Mørkegrå"
_darkRed="Mørkerød"
_orange="Orange"
_darkYellow="Mørkegul"
_green="Grøn"
_teal="Blågrøn"
_blue="Blå"
_blueGray="Blågrå"
_mediumGray="Mellemgrå"
_red="Rød"
_lightOrange="Lys orange"
_lime="Lime"
_seaGreen="Havgrøn"
_aqua="Akvamarin"
_lightBlue="Lyseblå"
_violet="Violet"
_gray="Grå"
_magenta="Magenta"
_gold="Guld"
_yellow="Gul"
_brightGreen="Knaldgrøn"
_cyan="Cyan"
_skyBlue="Himmelblå"
_plum="Blomme"
_lightGray="Lysegrå"
_pink="Pink"
_tan="Tan"
_lightYellow="Lysegul"
_lightGreen="Lysegrøn"
_lightTurquoise="Blegturkis"
_paleBlue="Blegblå"
_lavender="Lavendel"
_white="Hvid"
_lastUsed="Sidst anvendt:"
_moreColors="Flere farver..."

_month=new Array

_month[0]="JANUAR"
_month[1]="FEBRUAR"
_month[2]="MARTS"
_month[3]="APRIL"
_month[4]="MAJ"
_month[5]="JUNI"
_month[6]="JULI"
_month[7]="AUGUST"
_month[8]="SEPTEMBER"
_month[9]="OKTOBER"
_month[10]="NOVEMBER"
_month[11]="DECEMBER"

_day=new Array
_day[0]="S"
_day[1]="M"
_day[2]="T"
_day[3]="O"
_day[4]="T"
_day[5]="F"
_day[6]="L"

_today="I dag"

_AM="AM"
_PM="PM"

_closeDialog="Luk vindue"

_lstMoveUpLab="Flyt op"
_lstMoveDownLab="Flyt ned"
_lstMoveLeftLab="Flyt til venstre" 
_lstMoveRightLab="Flyt til højre"
_lstNewNodeLab="Tilføj indlejret filter"
_lstAndLabel="OG"
_lstOrLabel="ELLER"
_lstSelectedLabel="Valgt"
_lstQuickFilterLab="Tilføj hurtigfilter"

_openMenu="Klik her for at åbne {0}-indstillinger"
_openCalendarLab="Åbn kalender"

_scroll_first_tab="Rul til første fane"
_scroll_previous_tab="Rul til forrige fane"
_scroll_next_tab="Rul til næste fane"
_scroll_last_tab="Rul til sidste fane"

_expandedLab="Udvidet"
_collapsedLab="Skjult"
_selectedLab="Valgt"

_expandNode="Udvid node %1"
_collapseNode="Skjul node %1"

_checkedPromptLab="Angivet"
_nocheckedPromptLab="Ikke angivet"
_selectionPromptLab="værdier er lig med"
_noselectionPromptLab="ingen værdier"

_lovTextFieldLab="Indtast værdier her"
_lovCalendarLab="Indtast dato her"
_lovPrevChunkLab="Gå til forrige segment"
_lovNextChunkLab="Gå til næste segment"
_lovComboChunkLab="Segment"
_lovRefreshLab="Opdater"
_lovSearchFieldLab="Indtast tekst, der søges efter, her"
_lovSearchLab="Søg"
_lovNormalLab="Normal"
_lovMatchCase="Forskel på store og små bogstaver"
_lovRefreshValuesLab="Opdater værdier"

_calendarNextMonthLab="Gå til næste måned"
_calendarPrevMonthLab="Gå til forrige måned"
_calendarNextYearLab="Gå til næste år"
_calendarPrevYearLab="Gå til forrige år"
_calendarSelectionLab="Valgt dag "

_menuCheckLab="Markeret"
_menuDisableLab="Deaktiveret"
	
_level="Niveau"
_closeTab="Luk fane"
_of="af"

_RGBTxtBegin= "RGB("
_RGBTxtEnd= ")"

_helpLab="Hjælp"

_waitTitleLab="Vent venligst"
_cancelButtonLab="Annuller"

_modifiers= new Array
_modifiers[0]="Ctrl+"
_modifiers[1]="Shift+"
_modifiers[2]="Alt+"

_bordersMoreColorsLabel="Flere rammer..."
_bordersTooltip=new Array
_bordersTooltip[0]="Ingen ramme"
_bordersTooltip[1]="Venstre ramme"
_bordersTooltip[2]="Højre ramme"
_bordersTooltip[3]="Nederste ramme"
_bordersTooltip[4]="Medium nederste ramme"
_bordersTooltip[5]="Tyk nederste ramme"
_bordersTooltip[6]="Øverste og nederste ramme"
_bordersTooltip[7]="Øverste og medium nederste ramme"
_bordersTooltip[8]="Øverste og tyk nederste ramme"
_bordersTooltip[9]="Alle rammer"
_bordersTooltip[10]="Alle rammer medium"
_bordersTooltip[11]="Alle rammer tykke"