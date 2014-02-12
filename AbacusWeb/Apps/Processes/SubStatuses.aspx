<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SubStatuses.aspx.vb" Inherits="Target.Abacus.Web.Processes.SubStatuses" EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="cpOverview" ContentPlaceHolderID="MPPageOverview" runat="server">

    This screen allows you to maintain sub statuses for processes.

</asp:Content>

<asp:Content ID="cpError" ContentPlaceHolderID="MPPageError" runat="server">

    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:Content>

<asp:Content ID="cpContent" ContentPlaceHolderID="MPContent" runat="server">

    <uc1:StdButtons id="stdButtons1" runat="server" />

    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        
        <cc1:TextBoxEx ID="txtProcess" runat="server" LabelText="Process" LabelWidth="11.75em" MaxLength="30" Width="20em" IsReadOnly="true" />
        <br /><br />
        <cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description" LabelWidth="11.75em" MaxLength="30" Width="20em" ValidationGroup="Save" Required="true" RequiredValidatorErrMsg="Please enter a description" />
        <br />
        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="11.5em" />

    </fieldset>

</asp:Content>
