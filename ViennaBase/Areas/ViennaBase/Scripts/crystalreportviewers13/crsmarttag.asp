<% Option Explicit
Response.ExpiresAbsolute = Now() - 1
Session.CodePage  = 65001 ' UTF-8
Const ACTION_VIEWREPORT = "viewReport"
Const ACTION_REFRESH = "refresh"
Const SMARTTAGACTIONATTR = "smarttagaction"

Function OutputReport()
	Dim smarttagAction, reportSource
    Dim dataContext, objectName
    Dim viewer
	Dim smartTagInfoParser

    Dim requestMethod
    requestMethod = UCase(Request.ServerVariables("REQUEST_METHOD"))

	Select Case requestMethod
        Case "POST"
            smarttagAction = Request.Form(SMARTTAGACTIONATTR)
        Case "GET"
            smarttagAction = Request.QueryString(SMARTTAGACTIONATTR)
        Case Else
            smarttagAction = Empty
    End Select

	' Initialize the Smart Tag info parser
	Set smartTagInfoParser = CreateObject("CrystalReports.CrystalReportSmartTagInfoParser")
    call smartTagInfoParser.Init(request)

	reportSource = smartTagInfoParser.ReportSource

    If (smarttagAction = ACTION_REFRESH) Then
        ' Refresh
        If (Not IsEmpty(reportSource)) Then
            Set viewer = CreateObject("CrystalReports.CrystalReportPartsViewer")
	        With viewer
                .ReportParts = smartTagInfoParser.ReportParts
                .RecordNumber = 1
    		    .IsOwnPage = true
                .IsDisplayTitle = false
                .IsDisplayHeadings = false
                .EnableLogonPrompt = false
                .EnableParameterPrompt = false
                .DatabaseLogOnInfos = smartTagInfoParser.ConnectionInfos
                .ParameterFields = smartTagInfoParser.ParameterFields
'                .EnterpriseLogon = smartTagInfoParser.EnterpriseLogon
		        .ReportSource = reportSource
		        .Name = "SmartTagViewer"
	        End With

            Call viewer.Refresh()
        End If
    Else
        ' View report
        If (IsEmpty(reportSource) OR (Len(reportSource) = 0)) Then
        ' This is not from the SmartTag.  Use the HTML page viewer to view the report
            reportSource = Session("CrystalReportSmartTagReportSource")
        End If

        If (IsEmpty(reportSource)) Then
            Call Err. Raise (vbObjectError + 1, "Crystal Report Smart Tag Viewer", "Report source is not defined")
        Else
			dataContext = smartTagInfoParser.DataContext
			objectName = smartTagInfoParser.ObjectName

            Session("CrystalReportSmartTagReportSource") = reportSource

            Set viewer = CreateObject("CrystalReports.CrystalReportViewer")

	        With viewer
		        .ReportSource = reportSource
    		    .IsOwnPage = true
		        .Name = "HTML Page Viewer"
	        End With

            If (Not (IsEmpty(objectName) OR (Len(objectName) = 0))) Then
                Call viewer.NavigateTo(dataContext, objectName)
            End If
        End If

    End If

    Call viewer.ProcessHttpRequest(Request, Response, Session)

	if Err.number <> 0 then
		Response.Write Err.Description
		Err.Clear
	end if

    Set smartTagInfoParser = nothing
    Set viewer = nothing
End Function

%>

<% OutputReport%>
