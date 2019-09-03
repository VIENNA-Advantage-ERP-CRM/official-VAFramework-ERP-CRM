<%@ Language=VBScript%>

<% 
Function handleImage()
	Dim handler
	On Error Resume Next
	Set handler = CreateObject("CrystalReports.CrystalImageHandler")
	Call handler.HandleImage(Request, Response)
 	if Err.number <> 0 then
		Response.Write Err.Description
		Err.Clear
	end if
End Function
%>

<% handleImage %>
