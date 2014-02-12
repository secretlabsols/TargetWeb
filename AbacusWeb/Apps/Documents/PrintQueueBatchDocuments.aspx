<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PrintQueueBatchDocuments.aspx.vb"
	Inherits="Target.Abacus.Web.Apps.Documents.PrintQueueBatchDocuments" %>

<%@ Register TagPrefix="DS" TagName="DocumentSelector" Src="~/AbacusWeb/Apps/Documents/UserControls/DocumentSelector.ascx" %>
	
<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">

</asp:Content>

<asp:Content ContentPlaceHolderID="MPContent" runat="server">

	<DS:DocumentSelector id="docSelector" runat="server" />

</asp:Content>
