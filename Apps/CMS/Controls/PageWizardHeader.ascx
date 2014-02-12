<%@ Control Language="vb" AutoEventWireup="false" Codebehind="PageWizardHeader.ascx.vb" Inherits="Target.Web.Apps.CMS.Controls.PageWizardHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<hr />
<asp:HyperLink id="lnkSelectPage" runat="server" NavigateUrl="../Admin/Default.aspx">Select Page</asp:HyperLink>
&nbsp;<img src="../Images/arrow_blue1.gif" alt="Arrow" />&nbsp;
<asp:HyperLink id="lnkEditPage" runat="server" NavigateUrl="../Admin/EditPage.aspx" Enabled="False"
	CssClass="disabled">Edit Page</asp:HyperLink>
<!--
&nbsp;<img src="../Images/arrow_blue1.gif" alt="Arrow" />&nbsp;
<asp:HyperLink id="lnkPageLocation" runat="server" NavigateUrl="../Admin/PageLocation.aspx" Enabled="False"
	CssClass="disabled">Page Location</asp:HyperLink>
&nbsp;<img src="../Images/arrow_blue1.gif" alt="Arrow" />&nbsp;
<asp:HyperLink id="lnkPageMenu" runat="server" NavigateUrl="../Admin/PageMenu.aspx" Enabled="False"
	CssClass="disabled">Page Menu</asp:HyperLink>
&nbsp;<img src="../Images/arrow_blue1.gif" alt="Arrow" />&nbsp;
<asp:HyperLink id="lnkViewPage" runat="server" NavigateUrl="../Admin/ViewPage.aspx" Enabled="False"
	CssClass="disabled">View/Email Page</asp:HyperLink>
-->
<hr />
