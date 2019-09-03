<%@ Language=VBScript codepage=65001 aspcompat=true%>

<% 
	Dim handler
	On Error Resume Next
	handler = CreateObject("CrystalReports.CrystalImageHandler")
	Call handler.HandleDotNetBridgeImage()
 	if Err.number <> 0 then
		Response.Write (Err.Description)
		Err.Clear
	end if
%>


