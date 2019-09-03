<%@ page session="true"%>
<%@ page contentType="text/html; charset=utf-8" %>
<%@ page import="com.crystaldecisions.report.web.viewer.CrystalReportSmartTagInfoParser" %>
<%@ page import="com.crystaldecisions.report.web.viewer.CrystalReportPartsViewer" %>
<%@ page import="com.crystaldecisions.report.web.viewer.CrystalReportViewer" %>

<%
    String smartTagAction = request.getParameter("smarttagaction");
    CrystalReportSmartTagInfoParser parser = new CrystalReportSmartTagInfoParser();
    parser.init(request, getServletConfig().getServletContext());
    String reportSource = parser.getReportSource();

    if ((smartTagAction != null) && (smartTagAction.compareTo("refresh") == 0))
    {
        // Refresh

        if (reportSource != null)
        {
            CrystalReportPartsViewer reportPartsViewer = new CrystalReportPartsViewer();
            reportPartsViewer.setName("SmartTagViewer");
            reportPartsViewer.setRecordNumber(1);
            reportPartsViewer.setOwnPage(true);
            reportPartsViewer.setDisplayTitle(false);
            reportPartsViewer.setDisplayHeadings(false);
            reportPartsViewer.setEnableLogonPrompt(false);
            reportPartsViewer.setEnableParameterPrompt(false);

            reportPartsViewer.setReportSource(reportSource);
            reportPartsViewer.setDatabaseLogonInfos (parser.getDatabaseLogonInfos());
            reportPartsViewer.setParameterFields (parser.getParameterFields());
            //reportPartsViewer.setEnterpriseLogon (parser.getEnterpriseLogonInfo());
            reportPartsViewer.setReportParts (parser.getReportParts());
            reportPartsViewer.refresh();
            reportPartsViewer.processHttpRequest(request, response, getServletConfig().getServletContext(), null);
            reportPartsViewer.dispose();
        }
    }
    else
    {
        // View report
        if (reportSource == null)
            reportSource = (String)session.getAttribute("crystalreportsmarttagreportsource");

        if (reportSource != null)
        {
            String dataContext = parser.getDataContext();
            String objectName = parser.getObjectName();

            session.setAttribute("crystalreportsmarttagreportsource", reportSource);

            CrystalReportViewer reportPageViewer = new CrystalReportViewer();

            reportPageViewer.setReportSource(reportSource);
            reportPageViewer.setName("HTML Page Viewer");
            reportPageViewer.setOwnPage(true);

            if ((objectName != null) && (objectName.length() > 0))
            {
                if (dataContext == null)
                    dataContext = new String();

                reportPageViewer.navigateTo (dataContext, objectName);
            }

            reportPageViewer.processHttpRequest(request, response, getServletConfig().getServletContext(), null);
            reportPageViewer.dispose();
        }
    }

%>

