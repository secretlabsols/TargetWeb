<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RateCategories.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.RateCategories" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceFinanceCode" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.ascx" %>
<%@ Register TagPrefix="uc9" TagName="InPlaceService" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceServiceSelector.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different rate categories.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewstate="false">

        <asp:Panel ID="pnlDescription" runat="server">
            <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="14em" MaxLength="255" 
                Width="20em" Required="true" RequiredValidatorErrMsg="Please enter a description" SetFocus="true"
                ValidationGroup="Save"></cc1:TextBoxEx>
            <br />
        </asp:Panel>
		<asp:Panel id="pnlRegisterGroup" runat="Server">
		    <cc1:TextBoxEx ID="txtRegisterGroup"  runat="server" LabelText="Register Group" AllowableContent="2" LabelWidth="14em" MaxLength="255" 
                Width="20em" ></cc1:TextBoxEx>
            <br />
		</asp:Panel>		
		<asp:Panel ID="pnlUom" runat="server">
            <cc1:DropDownListEx ID="cboUom" runat="server" LabelText="Measured In" LabelWidth="14em"
			    Width="20em" Required="true" RequiredValidatorErrMsg="Please select a unit of measure"
			    ValidationGroup="Save"></cc1:DropDownListEx>
		    <br />
		    <cc1:DropDownListEx ID="cboPaymentToleranceGroup" runat="server" LabelText="Payment Tolerance Group" LabelWidth="14em"
			    Width="20em" ValidationGroup="Save"></cc1:DropDownListEx>
		    <br />
		</asp:Panel>
		<asp:Panel ID="pnlOneUnitPerOrder" runat="server">
            <cc1:CheckBoxEx ID="chkOneUnitPerOrder" runat="server" Text="One Unit Per Order" LabelWidth="13.7em"></cc1:CheckBoxEx>
            <br /><br />
        </asp:Panel>
		<asp:Panel ID="pnlManualPayments" runat="server">
            <cc1:CheckBoxEx ID="chkManualPayments" runat="server" Text="Allow use with Manual Payments" LabelWidth="13.7em"></cc1:CheckBoxEx>
            <br /><br /><br />
        </asp:Panel>
        <asp:Panel ID="pnlCareProviderNotPaid" runat="server">
            <cc1:CheckBoxEx ID="chkCareProviderNotPaid" runat="server" Text="Care Provider Not Paid" LabelWidth="13.7em"></cc1:CheckBoxEx>
            <br /><br />
        </asp:Panel>
        <asp:Panel ID="pnlServiceType" runat="server">
            <cc1:DropDownListEx ID="cboServiceType" runat="server" LabelText="Service Type" LabelWidth="14em"
			    Width="20em" Required="true"  RequiredValidatorErrMsg="Please select a service type"
			    ValidationGroup="Save"></cc1:DropDownListEx>			    
		    <br />
		</asp:Panel>
		<asp:Panel ID="pnlDayCategory" runat="server">
		    <cc1:DropDownListEx ID="cboDayCategory" runat="server" LabelText="Day Category" LabelWidth="14em"
			    Width="20em" Required="true" RequiredValidatorErrMsg="Please select a day category"
			    ValidationGroup="Save"></cc1:DropDownListEx>
		    <br />
		</asp:Panel>
		<asp:Panel ID="pnlTimeBand" runat="server">
            <cc1:DropDownListEx ID="cboTimeBand" runat="server" LabelText="Time Band" LabelWidth="14em"
			    Width="20em" Required="true" RequiredValidatorErrMsg="Please select a time band" ValidationGroup="Save"></cc1:DropDownListEx>
		    <br />
		</asp:Panel>
		<asp:Panel ID="pnlConstructedAbbreviation" runat="server">
		    <cc1:TextBoxEx ID="txtConstructedAbbreviation" runat="server" LabelText="Constructed Abbreviation" LabelWidth="14em" MaxLength="50" 
                Width="20em"></cc1:TextBoxEx>
            <br />
        </asp:Panel>  
        <asp:Panel ID="pnlAbbreviationSuffix" runat="server">      
            <cc1:TextBoxEx ID="txtAbbreviationSuffix" runat="server" LabelText="Abbreviation Suffix" LabelWidth="14em" MaxLength="50" 
                Width="20em" ValidationGroup="Save"></cc1:TextBoxEx>
            <br />
        </asp:Panel> 
        <asp:Panel ID="pnlDomService" runat="server">
		    <asp:Label id="lblService" runat="server" Text="Service" Width="13.5em" />
            <uc9:InPlaceService id="txtService" runat="server" Required="true" RequiredValidatorErrorMessage="Please select a service" RequiredValidatorValidationGroup="Save" />	
		    <br />
		</asp:Panel> 
		<asp:Label AssociatedControlID="txtFinanceCode" runat="server" Text="Finance Code" Width="13.5em"></asp:Label>
		<uc2:InPlaceFinanceCode id="txtFinanceCode" runat="server"></uc2:InPlaceFinanceCode>
		<br />
	    <asp:Label AssociatedControlID="txtFinanceCode2" runat="server" Text="Finance Code 2" Width="13.5em"></asp:Label>
	    <uc2:InPlaceFinanceCode id="txtFinanceCode2" runat="server"></uc2:InPlaceFinanceCode>
	    <br />
    </fieldset>
    <br />
</asp:Content>