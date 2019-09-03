
function writeActxViewer(sViewerVer, sProductLang, sPreferredViewingLang, bDrillDown, bExport, bDisplayGroupTree, 
						bGroupTree, bAnimation, bPrint, bRefresh, bSearch, 
						bZoom, bSearchExpert, bSelectExpert, sParamVer) {
	document.write("<OBJECT ID=\"CRViewer\"");
	document.write("CLASSID=\"CLSID:C0A870C3-66BB-4106-9A25-60A26F3C1DA8\"");
	document.write("WIDTH=\"100%\" HEIGHT=\"99%\"");
	document.write("CODEBASE=\"" + gPath + viewerPath + "ActiveXControls/ActiveXViewer.cab#Version=" + sViewerVer + "\">");
	document.write("<PARAM NAME=\"LocaleID\" VALUE=\"" + sProductLang + "\">");
	document.write("<PARAM NAME=\"PreferredViewingLocaleID\" VALUE=\"" + sPreferredViewingLang + "\">");
	document.write("<PARAM NAME=\"EnableDrillDown\" VALUE=" + bDrillDown + ">");
	document.write("<PARAM NAME=\"EnableExportButton\" VALUE=" + bExport + ">");
	document.write("<PARAM NAME=\"DisplayGroupTree\" VALUE=" + bDisplayGroupTree + ">");
	
	document.write("<PARAM NAME=\"EnableGroupTree\" VALUE=" + bGroupTree +">");
	document.write("<PARAM NAME=\"EnableAnimationControl\" VALUE=" + bAnimation + ">");
	document.write("<PARAM NAME=\"EnablePrintButton\" VALUE=" + bPrint + ">");
	document.write("<PARAM NAME=\"EnableRefreshButton\" VALUE=" + bRefresh + ">");
	document.write("<PARAM NAME=\"EnableSearchControl\" VALUE=" + bSearch + ">");
	
	document.write("<PARAM NAME=\"EnableZoomControl\" VALUE=" + bZoom + ">");
	document.write("<PARAM NAME=\"EnableSearchExpertButton\" VALUE=" + bSearchExpert + ">");
	document.write("<PARAM NAME=\"EnableSelectExpertButton\" VALUE=" + bSelectExpert + ">");
	document.write("</OBJECT>");

	document.write("<OBJECT ID=\"ReportSource\"");
	document.write("CLASSID=\"CLSID:C05C1BE9-3285-4ED8-B47E-8F408534E89D\"");
	document.write("HEIGHT=\"1%\" WIDTH=\"1%\"");
	document.write("CODEBASE=\"" + gPath + viewerPath + "ActiveXControls/ActiveXViewer.cab#Version=" + sViewerVer + "\">");
	document.write("</OBJECT>");

	document.write("<OBJECT ID=\"ViewHelp\"");
	document.write("CLASSID=\"CLSID:C02176CF-8629-4AF6-8F96-00D2DAA4EFB2\"");
	document.write("HEIGHT=\"1%\" WIDTH=\"1%\"");
	document.write("CODEBASE=\"" + gPath + viewerPath + "ActiveXControls/ActiveXViewer.cab#Version=" + sViewerVer + "\">");
	document.write("</OBJECT>");	
}

