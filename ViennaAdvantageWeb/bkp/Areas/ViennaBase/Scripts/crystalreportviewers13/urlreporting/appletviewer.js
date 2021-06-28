function getArchiveParameters(lang)
{
    var resultParameters = "ReportViewer.jar";
    if (!(lang in ["de", "en", "es", "fr", "it", "ja", "ko", "nl", "pt", "sv", "zh_CN", "zh_TW"]))
    {
    	resultParameters += ",ReportViewer_" + lang + ".jar";
    }
    return resultParameters;
}

function writeJavaViewer_part1(browser, jdkVer, type, lang, pvLang, rptName, sf, promptOnRefresh, param)
{
	document.write("<P align=\"center\">");
	if (browser == "msie")
	{
		document.write("<OBJECT");
		document.write("\tclassid=\"clsid:8AD9C840-044E-11D1-B3E9-00805F499D93\"");
		document.write("\twidth=\"100%\" height=\"100%\"");
		document.write("\tcodebase=\"" + gPath + pluginPath + "#Version=" + jdkVer + "\">");
		document.write("<param name=type value=\"" + type + "\">");
		document.write("<param name=code value=\"com.crystaldecisions.ReportViewer.ReportViewer\">");
		document.write("<param name=codebase value=\"" + gPath + viewerPath + "JavaViewer/\">");
		document.write("<param name=archive value=\"" + getArchiveParameters(lang) + "\">");

		document.write("<param name=Language value=\"" + lang + "\">");
		document.write("<param name=PreferredViewingLanguage value=\"" + pvLang + "\">");
		document.write("<param name=ReportName value=\"" + url + rptName + "\">");
		if (sf != "")
			document.write("<param name=SelectionFormula value=\"" + sf 	 + "\">");
	 	document.write("<param name=ServerParameters value=\"" + url + rptName + "\">");	 	
		document.write("<param name=PromptOnRefresh value=\"" + promptOnRefresh + "\">");
		document.write("<param name=ReportParameter value=\"" + param + "\">");				 
	} else if (browser == "safari") {
		document.write("<APPLET");
		document.write("	WIDTH=\"100%\" HEIGHT=\"100%\" ");
		document.write("	codebase=\"" + gPath + viewerPath + "JavaViewer/\"");
		document.write("	archive=\"" + getArchiveParameters(lang) + "\" ");
		document.write("	code=\"com.crystaldecisions.ReportViewer.ReportViewer\">");
		document.write("<param name=Language value=\"" + lang + "\">");
		document.write("<param name=PreferredViewingLanguage value=\"" + pvLang + "\">");
		document.write("<param name=ReportName value=\"" + url + rptName + "\">");
		if (sf != "")
			document.write("<param name=SelectionFormula value=\"" + sf      + "\">");
		document.write("<param name=ServerParameters value=\"" + url + rptName + "\">");
		document.write("<param name=PromptOnRefresh value=\"" + promptOnRefresh + "\">");
		document.write("<param name=ReportParameter value=\"" + param + "\">");	
	} else {	 
		document.write("<EMBED MAYSCRIPT name=\"AppletViewer\" scriptable=\"true\" ");
		document.write("style=\"width: 100%; height: 100%;\"");
		document.write("pluginspage=\"http://java.sun.com/j2se/1.4/download.html\"");
		document.write("type=\"application/x-java-applet;version=1.4\"");
		document.write("java_codebase=\"" + gPath + viewerPath + "JavaViewer/\"");
		document.write("code=\"com.crystaldecisions.ReportViewer.ReportViewer\"");
		document.write("archive=\"" + getArchiveParameters(lang) + "\"");		
		document.write("Language=\"" + lang + "\"");
		document.write("PreferredViewingLanguage=\"" + pvLang + "\"");
		document.write("ReportName=\"" + url + rptName + "\"");
		document.write("SelectionFormula=\"" + sf + "\"");
		document.write("ServerParameters=\"" + url + rptName + "\"");
		document.write("PromptOnRefresh=\"" + promptOnRefresh + "\"");
		document.write("ReportParameter=\"" + param + "\"");		
	}
}

function writeJavaViewer_part2(browser, bDrillDown, bExport, bGroupTree,
							bShowGroupTree, bPrint, bRefresh, bSearch, 
							bZoom, bSearchExpert, bSelectExpert, bLogo)
{	 
	if (browser == "msie" || browser == "safari")
	{		
		document.write("<param name=CanDrillDown value=\"" + bDrillDown + "\">");
		document.write("<param name=HasExportButton value=\"" + bExport + "\">");	
		document.write("<param name=HasGroupTree value=\"" + bGroupTree + "\">");
		document.write("<param name=ShowGroupTree value=\"" + bShowGroupTree + "\">");
		document.write("<param name=HasPrintButton value=\"" + 	bPrint + "\">");
		document.write("<param name=HasRefreshButton value=\"" + bRefresh + "\">");
		
		document.write("<param name=HasTextSearchControls value=\"" + bSearch + "\">");
		document.write("<param name=HasZoomControl value=\"" + bZoom + "\">");
		document.write("<param name=HasSearchExpert value=\"" + bSearchExpert + "\">");
		document.write("<param name=HasSelectExpert value=\"" + bSelectExpert +"\">");
		document.write("<param name=ShowLogo value=\"" + bLogo + "\">");	 
		if (browser == "msie")
			document.write("</OBJECT>");	 
		else if (browser == "safari")
			document.write("</APPLET>");
	} else {	 		
		document.write("CanDrillDown=\"" + bDrillDown + "\"");
		document.write("HasExportButton=\"" + bExport + "\"");
		document.write("HasGroupTree=\"" + bGroupTree + "\"");
		document.write("ShowGroupTree=\"" + bShowGroupTree +"\"");
		document.write("HasPrintButton=\"" + bPrint + "\"");
		document.write("HasRefreshButton=\"" + bRefresh + "\"");
		document.write("HasTextSearchControls=\"" + bSearch + "\"");
		document.write("HasZoomControl=\"" + bZoom + "\"");
		document.write("HasSearchExpert=\"" + bSearchExpert + "\"");
		document.write("HasSelectExpert=\"" + bSelectExpert + "\"");
		document.write("ShowLogo=\"" + bLogo + "\"");
		document.write(">");
		document.write("</EMBED>");
	}
	document.write("</p>");
}
