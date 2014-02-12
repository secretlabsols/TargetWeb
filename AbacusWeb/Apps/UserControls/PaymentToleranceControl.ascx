<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PaymentToleranceControl.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.PaymentToleranceControl" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<asp:Label ID="lblRateCategoryDescriptions" runat="server" Font-Italic="true" Font-Size="Small" ></asp:Label>
<br />
<fieldset id="fsUnitsOfServiceControls" style="padding:0.5em;" runat="server">
<legend>Units of Service</legend>
    <asp:Panel id="pnlVisitBasedUnitsofService" runat="Server">
        <div>
            <cc1:TextBoxEx ID="txtAcceptableAdditionalUnits"  runat="server" LabelText="Acceptable additional units" LabelWidth="27em" ></cc1:TextBoxEx>
        </div>
        <div>
            <cc1:TextBoxEx ID="txtAcceptableAdditionalUnitsAsPercentage"  runat="server" LabelText="Acceptable additional units as a percentage" LabelWidth="27em" Format="IntegerFormat" ></cc1:TextBoxEx>
        </div>
        <div>
            <cc1:TextBoxEx ID="txtUnitsOfServiceCappedAt"  runat="server" LabelText="% capped at" LabelWidth="27em" ></cc1:TextBoxEx>
        </div>
    </asp:Panel>
    <asp:Panel id="pnlUserEnteredUnitsofService" runat="Server">
        <div>
         <cc1:CheckBoxEx ID="chkSuspendInvoiceWhenPlannedUnitsExceeded" runat="server" Text="Suspend Invoice when planned units are exceeded" LabelWidth="27em" ></cc1:CheckBoxEx>
        </div>
    </asp:Panel>
 </fieldset>	
<br />
    <cc1:DropDownListEx ID="cboPaymentToleranceCombinationMethod" runat="server" LabelText="" LabelWidth="0em" Width="20em"></cc1:DropDownListEx>
<br />
 <fieldset id="fsCostOfServiceControls" style="padding:0.5em;" runat="server">
<legend>Cost Of Service</legend>
    <asp:Panel id="pnlCostOfService" runat="Server">
        <div><div>
            <cc1:TextBoxEx ID="txtAcceptableAdditionalPayment"  runat="server" LabelText="Acceptable additional payment" LabelWidth="27em"></cc1:TextBoxEx>
            </div>
            <div>
            <cc1:TextBoxEx ID="txtAcceptableAdditionalPaymentAsPercentage"  runat="server" LabelText="Acceptable additional payment as a percentage" LabelWidth="27em" Format="IntegerFormat"></cc1:TextBoxEx>
            </div>
            <div>
            <cc1:TextBoxEx ID="txtCostOfServiceCappedAt"  runat="server" LabelText="% capped at" LabelWidth="27em"></cc1:TextBoxEx>
            </div>
        </div>
    </asp:Panel>
 </fieldset>	
 <br />
 <div id="divWarning" runat="server">
   <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</div>
 <asp:HiddenField ID="hidPaymentToleranceID" runat="server" Value="0" />
 <asp:HiddenField ID="hidPaymentToleraceGroupSystemType" runat="server" Value="0" />
 <asp:HiddenField ID="hidPaymentToleranceDisplayMode" runat="server" Value="0" />
 <asp:HiddenField ID="hidIsPaymentTolerancesSet" runat="server" Value="0" />


