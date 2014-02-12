<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReportConfig.aspx.vb" Inherits="Target.Web.Apps.Reports.Admin.ReportConfig"
    EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="BasicAuditDetails" Src="~/Library/UserControls/BasicAuditDetails.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the configuration of different SQL Server Reporting Services (SSRS) reports.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblConfigWarning" runat="server" CssClass="warningText" Visible="false">
        WARNING: The SSRS server configuration does not appear to be valid. You may not be able to access the SSRS reports. Please contact your system administrator.
    </asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <uc2:BasicAuditDetails id="auditDetails" runat="server"></uc2:BasicAuditDetails>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="12em" MaxLength="255" 
            Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a description" SetFocus="true"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtServerConfig" runat="server" LabelText="SSRS Base Path" LabelWidth="12em" Width="20em"
            IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
        <asp:HyperLink id="lnkBrowseServer" runat="server" Text="[Browse]" Target="_blank" Title="Browse the SSRS server reports"></asp:HyperLink>
        <br />
        <br />
        <cc1:TextBoxEx ID="txtReportPath"  runat="server"  LabelText=" Report Path" LabelWidth="12em" MaxLength="255" 
            Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter the report path"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <span class="disabled">(Report Path is relative to SSRS Base Path)</span>                    
        <br />
        <br />
        <cc1:DualList ID="dlCategories" runat="server" SrcListCaption="Available Categories" SrcListWidth="20em" DestListCaption="Selected Categories" DestListWidth="20em"></cc1:DualList>
        <div class="clearer"></div>
                
    </fieldset>
    <br />
</asp:Content>