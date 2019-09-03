<%@ Page Language="vb" validateRequest="false" %>

<%@ Register TagPrefix="cr" Namespace="CrystalDecisions.Web" Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" %>
<%@ Register TagPrefix="ctrls" Namespace="CrystalDecisions.ReportAppServer.Controllers" Assembly="CrystalDecisions.ReportAppServer.Controllers, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" %>
<%@ Register TagPrefix="engine" Namespace="CrystalDecisions.CrystalReports.Engine" Assembly="CrystalDecisions.CrystalReports.Engine, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" %>
<%@ Register TagPrefix="rptsrc" Namespace="CrystalDecisions.ReportSource" Assembly="CrystalDecisions.ReportSource, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" %>
<%@ Register TagPrefix="ser" Namespace="CrystalDecisions.ReportAppServer.XmlSerialize" Assembly="CrystalDecisions.ReportAppServer.XmlSerialize, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" %>
<%@ Register TagPrefix="objfact" Namespace="CrystalDecisions.ReportAppServer.ObjectFactory" Assembly="CrystalDecisions.ReportAppServer.ObjectFactory, Version=12.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" %>

<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="CrystalDecisions.ReportAppServer.Controllers" %>
<%@ Import Namespace="CrystalDecisions.CrystalReports.Engine"  %>
<%@ Import Namespace="CrystalDecisions.ReportSource" %>
<%@ Import Namespace="CrystalDecisions.Web" %>

<script runat="server" language="VB">
    Protected overrides Sub OnInit(ByVal e As System.EventArgs)
        MyBase.OnInit( e )
        Dim rptSrc As CrystalDecisions.ReportAppServer.Controllers.ReportSource


        'Response.ExpiresAbsolute = Now()
        Dim viewer As CrystalDecisions.Web.CrystalReportViewer
        viewer = CrystalReportViewer1
        'viewer = new CrystalDecisions.Web.CrystalReportViewer

        Const CLOSED_RPT_ID = "closedreportid"
        Const RPT_ID = "reportid"
        Const RPT_SOURCE = "reportsource"
        Const PRODUCT_LOCALE = "language"

        Dim requestMethod
        requestMethod = UCase(Request.ServerVariables("REQUEST_METHOD"))

        Dim serializedRptSrc As String
        Dim reportid
        Dim closedreportid As String
        Dim productLocale As String
        
        If (requestMethod = "POST") Then
            serializedRptSrc = Request.Form(RPT_SOURCE)
            closedreportid = Request.Form(CLOSED_RPT_ID)
        Else
            serializedRptSrc = Request.QueryString(RPT_SOURCE)
            closedreportid = Request.QueryString(CLOSED_RPT_ID)
        End If

        ' reportid and roduct locale are always passed in the URL
        reportid = Request.QueryString(RPT_ID)
        productLocale = Request.QueryString(PRODUCT_LOCALE)

        If (Not (productLocale = Nothing Or (Len(productLocale) = 0))) Then
            Dim culture As CultureInfo
            culture = New CultureInfo(productLocale.Replace("_", "-"))
            viewer.ProductLocale = culture
        End If

        If (Not (closedreportid = Nothing Or (Len(closedreportid) = 0))) Then
            ' Clear the report source in session with the given closedreportid
            dim rptSrcI as ISCRReportSource
            rptSrcI = Session(closedreportid)
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rptSrcI)
            Session.Remove(closedreportid)
            rptSrcI = Nothing
        End If

        If (Not (reportid = Nothing Or (Len(reportid) = 0))) Then
            On Error Resume Next

            ' Try to load report source from session if it's there
            If (Not (serializedRptSrc = Nothing Or (Len(serializedRptSrc) = 0))) Then
                ' session doesn't contain report source
                ' need to deserialize the report source string and pass to viewer
                Dim XMLSer As New CrystalDecisions.ReportAppServer.XmlSerialize.XmlSerializer

                XMLSer.ObjectCreater = New CrystalDecisions.ReportAppServer.ObjectFactory.ObjectFactory

                Dim objXmlSer As Object = XMLSer.CreateObjectFromString(serializedRptSrc)
                Session(reportid) = objXmlSer


                viewer.ReportSource = CType(objXmlSer, ISCRReportSource)

            Else
                Dim rptSrcI as ISCRReportSource
                rptSrcI = Session(reportid)
                If (rptSrcI Is Nothing) Then
                    ' If the product locale was not specified, use the browser locale
                    Dim errLoc As String = productLocale
                    If ((errLoc Is Nothing Or (Len(errLoc) = 0)) And Not (Request.UserLanguages Is Nothing)) Then
                        errLoc = Request.UserLanguages(0)
                    End If
                    
                    Response.Write("<script src=""js/previewerror.js""></" & "script>")
                    Response.Write("<script> loadResources(""")
                    
                    ' If we still dont have a locale, let the script use its default
                    If (Not (errLoc Is Nothing Or (Len(errLoc) = 0))) Then
                        Response.Write(errLoc)
                    End If
                    
                    Response.Write(""");</" & "script>")
                    Response.Write("<script>writeError(L_SessionExpired, L_PleaseRefresh);</" & "script>")
                Else
                    viewer.ReportSource = rptSrcI
                End If
            End If

            If Err.Number <> 0 Then
                Response.Write(Err.Description)
                Err.Clear()
            End If

            On Error goto 0
        End If

    End Sub

</script>



<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
  <HEAD>
        <title>WebForm1</title>
</HEAD>
<body>
    <form id=Form1 method="post" runat="server">
                <CR:CrystalReportViewer 
                    id="CrystalReportViewer1" 
                    runat="server"
                    HasRefreshButton = false
                    HasExportButton = false
                    HasPrintButton = false
>
                </CR:CrystalReportViewer>
    </form>
</body>
</HTML>
