<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Viewer.aspx.vb" Inherits="Target.Web.Apps.Reports.Viewer" MasterPageFile="~/Popup.master" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

	<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
    
        <table width="100%">
        <tr><td>
        <div>
        <rsweb:ReportViewer ID="rvReportViewer" runat="server"
            ShowRefreshButton="false"
            ShowPrintButton="false"
            Height="100%" Width="800px" AsyncRendering="false" SizeToReportContent="true">
        </rsweb:ReportViewer>
        </div>
        </td>
        </tr>
        </table>

    </asp:Content>