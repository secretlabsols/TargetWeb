<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Vst.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.Vst" 
	EnableEventValidation="false" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:DropDownListEx ID="cboServiceType" runat="server" LabelText="Service Type" LabelWidth="16em"
			Required="True" RequiredValidatorErrMsg="Please select a service type" ValidationGroup="Save"></cc1:DropDownListEx>
		<br />
		<cc1:CheckBoxEx ID="chkReEvalAtMidnight" runat="server" Text="Re-evaluate Rates at Midnight" LabelWidth="16em"></cc1:CheckBoxEx>
		<br /><br />
		<cc1:CheckBoxEx ID="chkReEvalAtTimeBandBoundary" runat="server" Text="Re-evaluate Rates at Time Band Boundary" LabelWidth="16em"></cc1:CheckBoxEx>
		<br /><br /><br />
        <cc1:TextBoxEx ID="txtMinDuration" runat="server" LabelText="Minimum Duration" Format="IntegerFormat" LabelWidth="16em" Width="4em" 
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TimePicker ID="tmeMinStartTime" runat="server" LabelText="Expected Minimum Start Time" ShowSeconds="false" LabelWidth="16em"></cc1:TimePicker>
        <br />
        <cc1:TimePicker ID="tmeMaxEndTime" runat="server" LabelText="Expected Maximum End Time" ShowSeconds="false" LabelWidth="16em"></cc1:TimePicker>
        <br />
        <span id="spnExpectedVisitTimes" class="warningText"></span>
    </fieldset>
    <br />
</asp:Content>