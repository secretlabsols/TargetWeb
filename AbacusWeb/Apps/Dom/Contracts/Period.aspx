<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Period.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.Period" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <%--<asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>--%>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <style type="text/css">
	      .rbInv {
	            float: left;
	            width: 100%;
	            margin-bottom: 1em;
	      }  
	      .rbInv input {
	            float:left;
	            display: table-cell;
	      } 
	      .rbInv label {
	            float:left;
	            display: table-cell;
	            padding: 0px 0px 0px 10px;
	      }  
	      .email {
	      } 
	      .email label {
	            padding-top: 5px;	        
	      }                       
    </style>
    <uc1:StdButtons id="stdButtons1" runat="server" OnEditClientClick="stdbutEdit_Click();"  ></uc1:StdButtons>
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <span id="spnDayOfWeekWarning" class="warningText" style="display:none;"></span>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="dteDateFrom"  runat="server" LabelText="Date From" Format="DateFormat" LabelWidth="10em"
            Required="true" RequiredValidatorErrMsg="Please enter a start date" 
            ValidationGroup="Save" SetFocus="true"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="dteDateTo"  runat="server" LabelText="Date To" Format="DateFormat" LabelWidth="10em"></cc1:TextBoxEx>
        <cc1:CompositeCompareValidator id="compDates" runat="server" ControlToValidate="dteDateFrom" ControlToCompare="dteDateTo"
			    ControlToValidateSuffix="_txtTextBox" ControlToCompareSuffix="_txtTextBox" Display="Dynamic" 
			    ErrorMessage="The dates entered are invalid." Type="Date" Operator="LessThan" ValidationGroup="Save"></cc1:CompositeCompareValidator>
        <br />
        <asp:Panel ID="pnlVisitCodeGroup" runat="server">
            <cc1:DropDownListEx ID="cboVisitCodeGroup" runat="server" LabelText="Visit Code Group" LabelWidth="10em"
			    Required="True" RequiredValidatorErrMsg="Please select a visit code group" ValidationGroup="Save"></cc1:DropDownListEx>
		    <br />
		</asp:Panel>
		<asp:Panel ID="pnlServiceOutcomeGroup" runat="server">
            <cc1:DropDownListEx ID="cboServiceOutcomeGroup" runat="server" LabelText="Service Outcome Group" LabelWidth="10em"
			    Required="True" RequiredValidatorErrMsg="Please select a service outcome group" ValidationGroup="Save"></cc1:DropDownListEx>
		    <br />
		</asp:Panel>
		<asp:Panel ID="pnlManAmendGroup" runat="server">
		    <cc1:DropDownListEx ID="cboManAmendGroup" runat="server" LabelText="Manually Amended Indicator Group" LabelWidth="10em"
			    Required="True" RequiredValidatorErrMsg="Please select a manually amended indicator group" ValidationGroup="Save"></cc1:DropDownListEx>
		    <br /><br />
		</asp:Panel>
        <cc1:TextBoxEx ID="txtContractedUnits" runat="server" LabelText="Contracted Units" LabelWidth="10em" Format="CurrencyFormat" Width="8em"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description" LabelWidth="10em" MaxLength="255" Width="20em"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtReference"  runat="server" LabelText="Reference" LabelWidth="10em" MaxLength="50"></cc1:TextBoxEx>
        <br />
        <fieldset>
            <legend>Non-Residential Service Orders</legend>
            <asp:Panel ID="pnlVisitBasedReturns" runat="server">
            <cc1:CheckBoxEx ID="chkVisitBasedPlans" runat="server" Text="Record Care Plans At Visit level" LabelWidth="17.0em"></cc1:CheckBoxEx>
            <br /><br />
            </asp:Panel>
            <cc1:CheckBoxEx ID="chkSpecifyPlannedDoW" runat="server" Text="Must Record Day Of Week Against Care Plan Summary" LabelWidth="17.0em"></cc1:CheckBoxEx>
            <br /><br />
        </fieldset>
        <br />
        <fieldset>
            <legend>Provider Invoice Input Method</legend>
            <asp:RadioButton ID="rbInvManuallyEntered" runat="server" GroupName="rbInv" CssClass="rbInv" TextAlign="Right" Text="Provider Invoices are manually input by the council into Abacus Intranet" />
            <asp:RadioButton ID="rbInvCreatedFromServiceRegisters" runat="server" GroupName="rbInv" CssClass="rbInv" TextAlign="Right" Text="Provider Invoices are created during processing of Service Registers" />
            <asp:RadioButton ID="rbInvCreatedVisitProForma" runat="server" GroupName="rbInv" CssClass="rbInv" TextAlign="Right" Text="Provider Invoices are created during processing of visit-based Pro forma Invoices (which were constructed by Abacus using sets of visits submitted via Abacus Extranet)" />
            <blockquote>
                    <asp:CheckBox ID="chkAutoSetSecondryVisit" runat="server" Text="Automatically set the Secondary visit indicator in Service delivery files" />
                </blockquote>
            <asp:RadioButton ID="rbInvCreatedSummaryProForma" runat="server" GroupName="rbInv" CssClass="rbInv" TextAlign="Right" Text="Provider Invoices are created during processing of summary-level Pro forma Invoices (created during processing of a Payment Request made via Abacus Extranet)" />
            <fieldset id="fldPaymentRequests" style="margin: 0 0 0 2em">
                <legend>Payment Requests</legend>
                <asp:RadioButton ID="rbPrCareProvRequestPayments" GroupName="rbPr" runat="server" CssClass="rbInv" TextAlign="Right" Text="Care Provider may request payments" />
                <asp:RadioButton ID="rbPrCouncilStaffRequestPayments" GroupName="rbPr"  runat="server" CssClass="rbInv" TextAlign="Right" Text="Council staff (on behalf of the Care Provider) may request payments" />
                <div style="margin-left: 4em">
                    <cc1:DropDownListEx ID="ddPrMinPayPeriod" runat="server" LabelText="Minimum payment period" LabelWidth="16em" />
                </div>
                <br />
                <p>Send an email notification when payments are available to:</p>

                <div class="email">
                    <cc1:CheckBoxEx ID="cbPrProviderEmail" runat="server"  Text="Provider email address" LabelWidth="12em"  />
                    <cc1:TextBoxEx ID="tbPrProviderEmail" runat="server" Format="TextFormat" Width="55%" LabelText="&nbsp;&nbsp;OR&nbsp;" LabelWidth="4em" />
                </div>
            </fieldset>
            <br />
            <asp:RadioButton ID="rbInvNotEntered" runat="server" GroupName="rbInv" CssClass="rbInv" TextAlign="Right" Text="Provider Invoices are not entered for this Contract" />
            <br />
        </fieldset>
    </fieldset>
    <input type="text" style="display:none;" id="hidProviderEmail" runat="server" />
    <br />
</asp:Content>