<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Edit.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.Edit" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to view and edit the details of a non-residential contract.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<input type="button" id="btnBack" value="Back" onclick="btnBack_Click();" />
		<br /><br />
	    <iframe id="frmTree" runat="server" style="float:left;width:30%;border:solid 1px silver;" frameborder="0"></iframe>
	    <div style="width:68%;padding:0.5em;float:left;border:solid 1px silver;border-left-width:0px;">
			<iframe id="frmContent" runat="server" style="width:100%;" frameborder="0"></iframe>
		</div>
		<div class="clearer"></div>
	</asp:Content>