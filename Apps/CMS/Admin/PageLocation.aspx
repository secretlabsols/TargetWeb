<%@ Register TagPrefix="uc1" TagName="CMSTree" Src="../Controls/CMSTree.ascx" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PageLocation.aspx.vb" Inherits="Target.Web.Apps.CMS.Admin.PageLocation" EnableViewState="True" %>
<%@ Register TagPrefix="uc1" TagName="PageWizardHeader" Src="../Controls/PageWizardHeader.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server"><asp:Literal id=litPageTitle runat="server"></asp:Literal></asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:PageWizardHeader id=wizHeader runat="server"></uc1:PageWizardHeader>
    	
	    <uc1:CMSTree id=tree runat="server"></uc1:CMSTree>	
	    <input type="button" value=" Next " onclick="document.location.href=nextUrl;" />
    </asp:Content>