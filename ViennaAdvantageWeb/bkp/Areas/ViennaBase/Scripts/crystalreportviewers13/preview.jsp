<%@ page session="true"%>

<%@ page import="java.io.*,
                 java.util.Locale,
                 com.crystaldecisions.client.helper.ResourceManager,
                 com.crystaldecisions.report.web.viewer.CrystalReportViewer,
                 com.crystaldecisions.sdk.occa.report.reportsource.IReportSource,
                 com.crystaldecisions.xml.serialization.*" %>
<%
String CLOSED_RPT_ID = "closedreportid";
String RPT_ID = "reportid";
String RPT_SOURCE = "reportsource";
String PRODUCT_LOCALE = "language"; 

String closeRptID = request.getParameter(CLOSED_RPT_ID);
String reportID = request.getParameter(RPT_ID);
String serializedRptSrc = request.getParameter(RPT_SOURCE);
String productLocale = request.getParameter(PRODUCT_LOCALE);

if (closeRptID != null && closeRptID.length() > 0)
{
    // Clear the report source in session with the given closedreportid
    session.removeAttribute(closeRptID);
}

if (reportID != null && reportID.length() > 0)
{
    CrystalReportViewer viewer = new CrystalReportViewer();
    viewer.setName("htmlpreview");
    viewer.setHasRefreshButton(false);
    viewer.setHasExportButton(false);
    viewer.setHasPrintButton(false);
    viewer.setOwnForm(true);
    viewer.setOwnPage(true);
    
    if (productLocale != null && productLocale.length() > 0)
    {
        String[] localeParts = productLocale.split("[_\\-]");
        if (localeParts.length > 0)
        {
            Locale locale = null;
            if (1 == localeParts.length)  
            {
                locale = new Locale(localeParts[0] /*language*/);
            }
            else if (2 == localeParts.length)
            {
                locale = new Locale(localeParts[0],  // language
                                    localeParts[1]); // country
            }
            else
            {
                locale = new Locale(localeParts[0],  // language
                                    localeParts[1],  // country
                                    localeParts[2]); // variant
            }
            viewer.setProductLocale(locale);  
        }
    }
    

    IReportSource reportSource = null;

    if (serializedRptSrc != null && serializedRptSrc.length() > 0)
    {
        // Got a seralizedRprtSrc string
        // need to deserialize the report source string and pass to viewer
        byte[] byteRptSrc = serializedRptSrc.getBytes();
        ByteArrayInputStream byteInStream = new ByteArrayInputStream(byteRptSrc);

        try
        {
            XMLObjectSerializer serializer = new XMLObjectSerializer();
            SaveOption saveOpt = serializer.getSaveOption();
            saveOpt.setExcludeNullObjects(true);
            // Enabled since server is ready.
            saveOpt.setSkipWritingIdenticalObject(true);

            reportSource = (IReportSource)serializer.load(byteInStream);
        }
        catch (Exception e)
        {            
            out.write("Error deserializing report source.");
            return;
        }
        finally
        {
            byteInStream.close();
        }
        viewer.setReportSource(reportSource);
        session.setAttribute(reportID, viewer.getReportSource());
    }
    else
    {
        // Try to load report source from session
        reportSource = (IReportSource) session.getAttribute(reportID);
        if (reportSource == null)
        {
        	Locale errLoc = viewer.getProductLocale();
        	if (errLoc == null) 
        	{
        		errLoc = request.getLocale();	
        	}
        	
        	String componentName = "crystalreportviewer";
        	String sessionExpired = ResourceManager.getString(componentName, "Error_HtmlPreviewSessionTimeout", errLoc);
        	String needRefresh = ResourceManager.getString(componentName, "Error_HtmlPreviewNeedsRefresh", errLoc);
        	
        	out.write("<script src=\"js/previewerror.js\"></script>"); 
            out.write("<script>writeError(\"");
            out.write(sessionExpired);
            out.write("\", \"");
            out.write(needRefresh);
            out.write("\");</script>");
            return;
        }
        viewer.setReportSource(reportSource);
    }
    viewer.setURI(request.getRequestURI() + "?" + RPT_ID + "=" + reportID + "&" + PRODUCT_LOCALE  + "=" + productLocale);
    viewer.processHttpRequest(request, response, application, null);
}
%>


