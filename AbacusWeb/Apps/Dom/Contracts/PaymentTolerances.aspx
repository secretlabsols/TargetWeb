<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PaymentTolerances.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.PaymentTolerances" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsSettings" style="padding:0.5em;" runat="server">
    <legend>Payment Tolerance Settings</legend>
        <br />
       	<asp:radiobutton id="optSuspendAsIndicatedInSystemSetting" groupname="grpTolerances" TextAlign="right" height="2em" width="100%" 
        runat="server" text="" 
        onclick="javascript:optPaymentToleranceSetting_Click();" />
        <br />
        <asp:radiobutton id="optTolerateRules" groupname="grpTolerances" TextAlign="right" height="2em" width="100%" 
            runat="server" text=""
            onclick="javascript:optPaymentToleranceSetting_Click();" />
        <br />
    </fieldset>
    <div id="divPlaceHolderPaymentTolerances" runat="server">
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
    <legend id="fsControlsLegend" runat="server">Payment Tolerances</legend>
    <br />
        <asp:PlaceHolder ID="phPaymentTolerances" runat="server"></asp:PlaceHolder>
	<br />
    </fieldset>
    </div>
    <br />
</asp:Content>
