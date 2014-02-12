<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PasswordExceptions.aspx.vb" Inherits="Target.Web.Apps.Security.Admin.PasswordExceptions" 
	EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="PasswordExceptionSelector" Src="../UserControls/PasswordExceptionSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to remove individual words from password exception lists (regardless of if the list is being used)
		and add new words to the "Custom" list.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litPageError" runat="server"></asp:Literal>
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" id="btnBack" value="Back" onclick="document.location.href=unescape(GetQSParam(document.location.search,'backUrl'));" />
	    <br />
        <uc1:PasswordExceptionSelector id="selector" runat="server"></uc1:PasswordExceptionSelector>
        <p style="font-style:italic;">
            To load new or pre-defined password exception lists, please <a href="mailto:a4wsupport@targetsys.co.uk">contact A4W Support</a>.
        </p>
    </asp:Content>