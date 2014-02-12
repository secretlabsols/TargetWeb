<%@ Page Language="vb" AutoEventWireup="false" AspCompat="true" CodeBehind="Administration.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.Enquiry.Administration" MasterPageFile="~/popup.master" EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MPPageOverview" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
<style type="text/css">
    .pnl{
    font-weight: bold;
    background-color: #eeeeee;
    padding: 5px;
    cursor: pointer; 
    border: solid 1px #c0c0c0
    }
</style>
    <cc1:CollapsiblePanel id="cpDetail" runat="server" 
            HeaderLinkText="Details" 
            MaintainClientState="true"
            ExpandedJS="PanelClick('Details',true);" 
            CollapsedJS="PanelClick('Details',false);">
        <ContentTemplate>
        
            <fieldset id="fsDetailsPersonal" style="float:left;width:46.5%;">
                <legend>Personal</legend>
                <cc1:TextBoxEx ID="txtReference" runat="server" LabelText="Reference" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtNHSNumber" runat="server" LabelText="NHS Number" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
		        <br />
		        <cc1:TextBoxEx ID="txtForenames" runat="server" LabelText="Forenames" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
		        <br />
		        <cc1:TextBoxEx ID="txtSurname" runat="server" LabelText="Surname" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
		        <br />
		        <cc1:TextBoxEx ID="txtTitle" runat="server" LabelText="Title" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
		        <br />
		        <cc1:TextBoxEx ID="txtNiNo" runat="server" LabelText="NI No" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
		        <br />
		        <cc1:TextBoxEx ID="txtDOB" runat="server" LabelText="Date of Birth" LabelWidth="10.5em" Format="DateFormat"></cc1:TextBoxEx>
		        <br />
		        <cc1:TextBoxEx ID="txtDOD" runat="server" LabelText="Date of Death" LabelWidth="10.5em" Format="DateFormat"></cc1:TextBoxEx>
		        <br />
		        <cc1:TextBoxEx ID="txtGender" runat="server" LabelText="Gender" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
                <br />    
                <cc1:TextBoxEx ID="txtEthnicity" runat="server" LabelText="Ethnicity" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtEmail" runat="server" LabelText="Email" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
		        <br />
            </fieldset>
            <fieldset id="fsDetailsGeneral" style="float:left;width:46.5%;margin-left:10px;">
                <legend>General</legend>
                <cc1:TextBoxEx ID="txtAlternativeRef" runat="server" LabelText="Alternative Ref" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
		        <br />
		        <cc1:TextBoxEx ID="txtDebtorReference" runat="server" LabelText="Debtor Reference" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
	            <br />
	            <cc1:TextBoxEx ID="txtCreditorReference" runat="server" LabelText="Creditor Reference" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
	            <br />
		        <cc1:TextBoxEx ID="txtCurrentTeam" runat="server" LabelText="Current Team" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
                <br /> 
                <cc1:TextBoxEx ID="txtCurrentCareManager" runat="server" LabelText="Current Care Manager" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtDateofLastAssessment" runat="server" LabelText="Date of Last Assessment" LabelWidth="10.5em" Format="DateFormat"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtDateofNextAssessment" runat="server" LabelText="Date of Next Assessment" LabelWidth="10.5em" Format="DateFormat"></cc1:TextBoxEx>
                <br />
		        <cc1:TextBoxEx ID="txtUserInitials" runat="server" LabelText="User Initials" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtFinanceCode" runat="server" LabelText="Finance Code" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
	            <br />
            </fieldset>
            <div class="clearer"></div>
        </ContentTemplate>
    </cc1:CollapsiblePanel>  
    <cc1:CollapsiblePanel id="cpAdministrativeDetail" runat="server" 
            HeaderLinkText="Administrative Details" 
            MaintainClientState="true"
            ExpandedJS="PanelClick('Administrative Details',true);" 
            CollapsedJS="PanelClick('Administrative Details',false);">
        <ContentTemplate>
            <fieldset id="fsAdminDetailPersonalBudgetStatements" style="float:left;width:46.5%;">
                <legend>Personal Budget Statements</legend>
                <label for="txtLastStatementProducedOn_txtTextBox">Last statement produced on</label>
                <br />
                <cc1:TextBoxEx ID="txtLastStatementProducedOn" runat="server" Format="DateFormat" OutputBrAfter="false" />
                <span id="spnNextStatementDue" style="margin-left:1em;font-size:85%;"></span>
                <br /><br />
		        <label for="cboStatementFrequency_cboDropDownList">Statement frequency</label>
                <br />
                <cc1:DropDownListEx ID="cboStatementFrequency" runat="server" EnableViewState="false" />
                <br />
		        <label for="cboStatementLayout_cboDropDownList">Statement Layout</label>
                <br />
                <cc1:DropDownListEx ID="cboStatementLayout" runat="server" EnableViewState="false" />
                <br />
                <div style="float:right;">
                    <asp:Button ID="btnUpdatePersonalBudgetStatements" runat="server" Text="Update" OnClientClick="return confirm('Are you sure you wish to change the frequency and/or layout of Personal Budget Statements for this Service User?');" ToolTip="Update Personal Budget Statements" />
			    </div>
			    <div class="clearer"></div>
            </fieldset>
            <fieldset id="fsBankAccountDetails" style="float:left;width:46.5%;margin-left:10px;">
                <legend>Bank Account Details</legend>
                <cc1:TextBoxEx ID="txtSortCode" runat="server" LabelText="Sort Code" LabelWidth="10.5em" Width="10em"></cc1:TextBoxEx>
	            <br />
	            <cc1:TextBoxEx ID="txtAccountNumber" runat="server" LabelText="Account Number" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
	            <br />
	            <cc1:TextBoxEx ID="txtPaymentRef" runat="server" LabelText="Payment Ref" LabelWidth="10.5em" Width="17em"></cc1:TextBoxEx>
	            <br />
	            <asp:Label ID="txtBankAccountWarning" runat="server" CssClass="warningText transBg"></asp:Label>
            </fieldset>
            <div class="clearer"></div>
        </ContentTemplate>
    </cc1:CollapsiblePanel>
    <cc1:CollapsiblePanel id="cpOtherInfo" runat="server" 
            HeaderLinkText="Other Information" 
            MaintainClientState="true"
            ExpandedJS="PanelClick('Other Information',true);" 
            CollapsedJS="PanelClick('Other Information',false);">
        <ContentTemplate>
            <div style="float:left;width:45%;">
                <cc1:TextBoxEx ID="txtDSSOffice" runat="server" LabelText="DSS Office" LabelWidth="10.5em"></cc1:TextBoxEx>
		        <br />
		        <cc1:TextBoxEx ID="txtCurrentCaseID" runat="server" LabelText="Current Case ID" LabelWidth="10.5em"></cc1:TextBoxEx>
		        <br />
                <cc1:TextBoxEx ID="txtMPSNumber" runat="server" LabelText="MPS Number" LabelWidth="10.5em"></cc1:TextBoxEx>
                <br />
                <cc1:CheckBoxEx ID="chkParentalContributionRequired" Text="Parental Contribution Required" runat="server"></cc1:CheckBoxEx>
                <br /><br />
            </div>
            <div style="float:left;width:45%;">
                <cc1:TextBoxEx ID="txtBillingType" runat="server" LabelText="Billing Type" LabelWidth="10.5em"></cc1:TextBoxEx>
	            <br />
	            <cc1:TextBoxEx ID="txtRiskLevel" runat="server" LabelText="Risk Level" LabelWidth="10.5em"></cc1:TextBoxEx>
                <br />
            </div>
            <div class="clearer"></div>
            <fieldset id="fsMemoIndicators" style="padding:0.5em;" runat="server" EnableViewstate="false">
               <legend>Memo Indicators</legend>
               <div style="float:left;width:33%;">
                    <asp:PlaceHolder ID="phMemoIndicators1" runat="server"></asp:PlaceHolder> 
               </div>
               <div style="float:left;width:33%;">
                    <asp:PlaceHolder ID="phMemoIndicators2" runat="server"></asp:PlaceHolder> 
               </div>
               <div style="float:left;width:33%;">
                    <asp:PlaceHolder ID="phMemoIndicators3" runat="server"></asp:PlaceHolder> 
               </div>
               <div class="clearer"></div>
            </fieldset>
        </ContentTemplate>
    </cc1:CollapsiblePanel>
    <cc1:CollapsiblePanel id="cpLegacy" runat="server" 
            HeaderLinkText="Legacy" 
            MaintainClientState="true"
            ExpandedJS="PanelClick('Legacy',true);" 
            CollapsedJS="PanelClick('Legacy',false);">
        <ContentTemplate>
            <fieldset id="fsAdminResBilling" style="float:left;width:28%;">
                <legend>Residential Billing</legend>
                <cc1:TextBoxEx ID="txtResInvoicedUpTo" runat="server" LabelText="Invoiced up to" LabelWidth="10.5em" Format="DateFormat"></cc1:TextBoxEx>
		        <br />
		        <cc1:CheckBoxEx ID="chkResSuppressInvoicing" Text="Suppress invoicing" runat="server"></cc1:CheckBoxEx>
		        <br /><br />
		        <cc1:CheckBoxEx ID="chkResSuppressInvoicePrinting" Text="Suppress invoice printing" runat="server"></cc1:CheckBoxEx>
		        <br /><br />
		        <cc1:CheckBoxEx ID="chkResPayByDD" Text="Pay by DD" runat="server"></cc1:CheckBoxEx>
		        <br /><br />
		        <cc1:CheckBoxEx ID="chkSuppressFromDebtorsInterface" Text="Suppress from Debtors Interface" runat="server"></cc1:CheckBoxEx>
		        <br /><br />
            </fieldset>
            <fieldset id="fsAdminNonResBilling" style="float:left;width:28%;margin-left:5px;">
                <legend>Non-Residential Billing</legend>
                <cc1:TextBoxEx ID="txtDomInvoicedUpTo" runat="server" LabelText="Invoiced up to" LabelWidth="10.5em" Format="DateFormat"></cc1:TextBoxEx>
		        <br />
		        <cc1:CheckBoxEx ID="chkDomSuppressInvoicing" Text="Suppress invoicing" runat="server"></cc1:CheckBoxEx>
		        <br /><br />
		        <cc1:CheckBoxEx ID="chkDomSuppressInvoicePrinting" Text="Suppress invoice printing" runat="server"></cc1:CheckBoxEx>
		        <br /><br />
		        <cc1:CheckBoxEx ID="chkDomPayByDD" Text="Pay by DD" runat="server"></cc1:CheckBoxEx>
		        <br /><br />
		        <cc1:TextBoxEx ID="txtSwipeCardNo" runat="server" LabelText="Swipe Card No" LabelWidth="10.5em" Width="10em"></cc1:TextBoxEx>
		        <br />
		        <cc1:TextBoxEx ID="txtCardProducedDate" runat="server" LabelText="Card Produced Date" LabelWidth="10.5em" Format="DateFormat"></cc1:TextBoxEx>
		        <br />
            </fieldset>
            <fieldset id="fsNonResPayments" style="float:left;width:34%;margin-left:5px;">
                <legend>Non-Residential Payments</legend>
                <cc1:CheckBoxEx ID="chkSuspensionSetting" Text="Suspend invoices where actual service exceeds planned" runat="server"></cc1:CheckBoxEx>
		        <br /><br />
            </fieldset>
            <div class="clearer"></div>
            <fieldset id="fsResNonResStatements" style="float:left;">
                <legend>Residential and Non-Residential Statements</legend>
                <cc1:CheckBoxEx ID="chkSuppressStatements" Text="Suppress statements" runat="server"></cc1:CheckBoxEx>
		        <br /><br />
            </fieldset>  
            <div class="clearer"></div>  
        </ContentTemplate>
    </cc1:CollapsiblePanel>    
</asp:Content>

