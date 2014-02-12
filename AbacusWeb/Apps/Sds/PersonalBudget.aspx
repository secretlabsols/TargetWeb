<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PersonalBudget.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.PersonalBudget" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to view and edit the details of a personal budget.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    
    <fieldset id="fsControls" runat="server" EnableViewState="false">
        <div style="float:left;width:50%;">
            <br />
            <cc1:TextBoxEx ID="txtClient" runat="server" LabelText="Service User" LabelWidth="10em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
            <br /><br />
            <cc1:DropDownListEx ID="cboRasType" runat="server" LabelText="RAS Type" LabelWidth="10em"
	            Required="true" RequiredValidatorErrMsg="Please select a RAS type" ValidationGroup="Save"></cc1:DropDownListEx>
            <br />
            <cc1:TextBoxEx ID="dteRasDate" runat="server" LabelText="RAS Date" LabelWidth="10em"
	            Required="true" RequiredValidatorErrMsg="Please enter a RAS date" Format="DateFormat"
	            ValidationGroup="Save"></cc1:TextBoxEx>
            <br />
            <cc1:TextBoxEx ID="txtRasRevision" runat="server" LabelText="RAS Revision" LabelWidth="10em" MaxLength="10"></cc1:TextBoxEx>
            <br />
            <cc1:TextBoxEx ID="dteEffectiveDate" runat="server" LabelText="Effective Date" LabelWidth="10em"
	            Required="true" RequiredValidatorErrMsg="Please enter an effective date" Format="DateFormat"
	            ValidationGroup="Save"></cc1:TextBoxEx>
            <br />
            <cc1:TextBoxEx ID="dteEndDate" runat="server" LabelText="End Date" LabelWidth="10em" Format="DateFormat"></cc1:TextBoxEx>
            <br />
            <cc1:TextBoxEx ID="txtPointScore" runat="server" LabelText="Point Score" LabelWidth="10em"
	            Required="true" RequiredValidatorErrMsg="Please enter a point score" Format="IntegerFormat"
	            ValidationGroup="Save" Width="5em"></cc1:TextBoxEx>
            <br />
        </div>
        <div style="float:left;width:50%;">
            <br />
            <fieldset>
                <legend>
                    <asp:CheckBox id="chkOverridePointScore" runat="server"></asp:CheckBox>
                    Override Point Score
                </legend>
                <cc1:TextBoxEx ID="txtOverriddenPointScore" runat="server" LabelText="Overridden Point Score" LabelWidth="13em"
	                Required="true" RequiredValidatorErrMsg="Please enter an overridden point score" Format="IntegerFormat"
	                ValidationGroup="Save" Width="5em"></cc1:TextBoxEx>
	            <br />
	            <cc1:DropDownListEx ID="cboOverriddenPointScoreReason" runat="server" LabelText="Override Reason" LabelWidth="13em"
	                Required="true" RequiredValidatorErrMsg="Please select an override reason" ValidationGroup="Save"></cc1:DropDownListEx>
            </fieldset>
            <br />
            
            <cc1:TextBoxEx ID="txtBudget" runat="server" LabelText="Calculated Budget" LabelWidth="10em" Width="5em"></cc1:TextBoxEx>
            <span id="spnBudgetRecalcWarning" class="warningText"></span>
            <br />   
                 
            <fieldset>
                <legend>
                    <asp:CheckBox id="chkOverrideBudget" runat="server"></asp:CheckBox>
                    Override Budget
                </legend>
                <cc1:TextBoxEx ID="txtOverriddenBudget" runat="server" LabelText="Overridden Budget" LabelWidth="13em"
	                Required="true" RequiredValidatorErrMsg="Please enter an overridden budget" Format="CurrencyFormat"
	                ValidationGroup="Save" Width="5em"></cc1:TextBoxEx>
	            <br />
	            <cc1:DropDownListEx ID="cboOverriddenBudgetReason" runat="server" LabelText="Override Reason" LabelWidth="13em"
	                Required="true" RequiredValidatorErrMsg="Please select an override reason" ValidationGroup="Save"></cc1:DropDownListEx>
            </fieldset>    
        </div>        
        
    </fieldset>
            
</asp:Content>