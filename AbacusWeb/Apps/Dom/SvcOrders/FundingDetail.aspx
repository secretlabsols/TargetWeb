<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FundingDetail.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.SvcOrder.FundingDetail" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceExp" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceExpenditureAccountSelector.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewState="false">
        <cc1:DropDownListEx ID="cboExpAccountType" runat="server" LabelText="Expenditure Account Type" LabelWidth="18em"
			    Width="11em" Required="true" RequiredValidatorErrMsg="Please select an Expenditure Account Type"
			    ValidationGroup="Save" ></cc1:DropDownListEx>
	    <br />
	        <asp:Label ID="Label1" AssociatedControlID="expenditureAccount" runat="server" Text="Expenditure Account" Width="18em" ></asp:Label>
	        <uc2:InPlaceExp id="expenditureAccount" ThirdPartyLabelWidth="11em" runat="server" enableAccountTypeCombo="false" ></uc2:InPlaceExp>
	        <br />
	    <cc1:TextBoxEx ID="txtExpenditure"  runat="server"  LabelText="Expenditure Code" LabelWidth="18em" MaxLength="50" 
            Width="20em" IsReadOnly="true" ></cc1:TextBoxEx>
        <br /><br />
        <cc1:TextBoxEx ID="txtIncomeDue"  runat="server"  LabelText="Income Due Code" LabelWidth="18em" MaxLength="50" 
            Width="20em" IsReadOnly="true" ></cc1:TextBoxEx>
        <br /><br />
        <cc1:TextBoxEx ID="txtProportion"  runat="server"  LabelText="Proportion" LabelWidth="18em" MaxLength="50" 
            Width="20em" IsReadOnly="true" ></cc1:TextBoxEx>
        <br /><br />
        <cc1:CheckBoxEx ID="chkBalancingAmount" runat="server" Text="Receive Balancing Amount" LabelWidth="18em"></cc1:CheckBoxEx>
        <br /><br />
        <cc1:CheckBoxEx ID="chkUnitsFunded" runat="server" Text="Units Funded by the Service User" LabelWidth="18em"></cc1:CheckBoxEx>
	</fieldset>
</asp:Content>
