<%@ Register TagPrefix="uc1" TagName="NavTree" Src="../../Navigation/Controls/NavTree.ascx" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PageMenu.aspx.vb" Inherits="Target.Web.Apps.CMS.Admin.PageMenu" EnableViewState="True" %>
<%@ Register TagPrefix="uc1" TagName="PageWizardHeader" Src="../Controls/PageWizardHeader.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server"><asp:Literal id=litTitle runat="server"></asp:Literal></asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:PageWizardHeader id="wizHeader" runat="server"></uc1:PageWizardHeader>
    	
	    <uc1:NavTree id=tree runat="server"></uc1:NavTree>	
	    <br /><br />
	    <input type="button" value=" Next " onclick="document.location.href=nextUrl;" />
    </asp:Content>