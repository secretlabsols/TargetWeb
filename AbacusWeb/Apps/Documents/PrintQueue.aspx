<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PrintQueue.aspx.vb"
	Inherits="Target.Abacus.Web.Apps.Documents.PrintQueue" %>

<%@ Register TagPrefix="DS" TagName="DocumentSelector" Src="~/AbacusWeb/Apps/Documents/UserControls/DocumentSelector.ascx" %>
	
<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">

</asp:Content>

<asp:Content ContentPlaceHolderID="MPContent" runat="server">

	<DS:DocumentSelector id="docSelector" runat="server" />

   <script type="text/javascript">

       isPrintQueueScreen = true;

   </script>

</asp:Content>
