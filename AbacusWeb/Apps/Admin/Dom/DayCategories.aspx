<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DayCategories.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.DayCategories"
	EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different day categories.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server" EnableViewState="false">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description" LabelWidth="12em" MaxLength="255" 
            Width="20em" Required="true" RequiredValidatorErrMsg="Please enter a description" SetFocus="true"
            ValidationGroup="Save"></cc1:TextBoxEx>
         <br />
         <cc1:TextBoxEx ID="txtAbbreviation" runat="server" LabelText="Abbreviation" LabelWidth="12em" MaxLength="50" 
            Width="20em"></cc1:TextBoxEx>
         <br />
         <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="11.7em"></cc1:CheckBoxEx>
         <br /><br />
         <cc1:DropDownListEx ID="cboTimeBandGroupID" runat="server" LabelText="Time Band Group" LabelWidth="12em" 
			Width="20em" Required="true" RequiredValidatorErrMsg="Please select a time band group" ValidationGroup="Save"></cc1:DropDownListEx>
         <br />
         <cc1:DropDownListEx ID="cboStandardEnhanced" runat="server" LabelText="Standard/Enhanced" LabelWidth="12em" 
			Width="20em" Required="true" RequiredValidatorErrMsg="Please select a standard/enhanced value" ValidationGroup="Save"></cc1:DropDownListEx>
         <br />
         <cc1:CheckBoxEx ID="chkSunday" runat="server" Text="Sunday" LabelWidth="11.7em"></cc1:CheckBoxEx>
         <br /><br />
         <cc1:CheckBoxEx ID="chkMonday" runat="server" Text="Monday" LabelWidth="11.7em"></cc1:CheckBoxEx>
         <br /><br />
         <cc1:CheckBoxEx ID="chkTuesday" runat="server" Text="Tuesday" LabelWidth="11.7em"></cc1:CheckBoxEx>
         <br /><br />
         <cc1:CheckBoxEx ID="chkWednesday" runat="server" Text="Wednesday" LabelWidth="11.7em"></cc1:CheckBoxEx>
         <br /><br />
         <cc1:CheckBoxEx ID="chkThursday" runat="server" Text="Thursday" LabelWidth="11.7em"></cc1:CheckBoxEx>
         <br /><br />
         <cc1:CheckBoxEx ID="chkFriday" runat="server" Text="Friday" LabelWidth="11.7em"></cc1:CheckBoxEx>
         <br /><br />
         <cc1:CheckBoxEx ID="chkSaturday" runat="server" Text="Saturday" LabelWidth="11.7em"></cc1:CheckBoxEx>
         <br /><br />
    </fieldset>
    <br />
</asp:Content>