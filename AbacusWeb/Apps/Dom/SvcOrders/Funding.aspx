<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Funding.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.SvcOrders.Funding" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to view and edit the Funding .
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<input type="button" id="btnBack" value="Back" onclick="btnBack_Click();" />
		<br /><br />
		<iframe id="frmDSODetails" runat="server" style="float:left;width:100%;height:7em;border:solid 1px silver;" frameborder="0"></iframe>
	    <iframe id="frmTree" runat="server" style="float:left;width:20%;border:solid 1px silver;" frameborder="0"></iframe>
	    <div style="width:79%;padding:0.5em;float:left;border:solid 1px silver;border-left-width:0px;">
			<iframe id="frmContent" runat="server" style="width:100%;" frameborder="0"></iframe>
		</div>
		<div class="clearer"></div>
	</asp:Content>
