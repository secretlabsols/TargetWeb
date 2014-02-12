<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BenefitRates.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.AnnualParameters.BenefitRates" EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="conPageOverview" ContentPlaceHolderID="MPPageOverview" runat="server">

</asp:Content>

<asp:Content ID="conPageError" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
</asp:Content>

<asp:Content ID="conContent" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" />
    <fieldset id="fsBenefitControls" style="padding:0.5em;" runat="server"> 
        <legend>Benefit</legend>       
            <cc1:DropDownListEx ID="cboBenefit" runat="server" LabelText="Benefit" LabelWidth="12em" 
			Width="28em" Required="true" RequiredValidatorErrMsg="Please select a Benefit" ValidationGroup="Save" />
			<br />
			<div style="width : 100%; clear : both; float : left;">
                <div style="float : left;">
		            <cc1:TextBoxEx ID="txtFromDate" runat="server" LabelText="From Date" LabelWidth="12em"
                    Width="8.2em" Required="true" RequiredValidatorErrMsg="Please enter a From Date" SetFocus="true"
                    ValidationGroup="Save" Format="DateFormatJquery" AllowClear="true" /> 
                    <br />
                </div> 
                <div style="padding-left : 1.75em; float : left;">         
                    <cc1:TextBoxEx ID="txtToDate" runat="server" LabelText="To Date" LabelWidth="6em" 
                    Width="8.2em" Required="true" RequiredValidatorErrMsg="Please enter a To Date" SetFocus="false"
                    ValidationGroup="Save" Format="DateFormatJquery"  AllowClear="true" />
                    <br /> 
                </div>  
            </div>
            <cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description" LabelWidth="12em" MaxLength="30" 
            Width="27.5em" Required="true" RequiredValidatorErrMsg="Please enter a Description" SetFocus="false"
            ValidationGroup="Save" Format="TextFormat" />
            <br />  
            <cc1:TextBoxEx ID="txtRate" runat="server" LabelText="Rate" LabelWidth="12em" MaxLength="255" 
            Width="7em" Required="true" RequiredValidatorErrMsg="Please enter a Rate" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />
            <br />
            <cc1:TextBoxEx ID="txtStatutoryDisregard" runat="server" LabelText="Statutory Disgregard" LabelWidth="12em" MaxLength="255" 
            Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Statutory Disgregard" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />
            <br />
            <div style="float: left; width : 100%;">
                <div style="float: left; width : 20em;">
                    <cc1:TextBoxEx ID="txtMaxTakenIntoAccountNonRes" runat="server" LabelText="Max taken into account (Non Residential)" LabelWidth="12em" MaxLength="255" 
                    Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Max taken into account (Non Residential)" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />
                    <br />
                </div>
                <div style="float: left; padding-left : 0.5em;"> 
                    <cc1:TextBoxEx ID="txtMaxReasonableAmount" runat="server" LabelText="Maximum reasonable amount" LabelWidth="12em" MaxLength="255" 
                    Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Maximum reasonable amount" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />
                    <br />
                </div>
            </div>
            <div style="float: left; width : 100%;">
                <div style="float: left; width : 20em;">
                    <cc1:TextBoxEx ID="txtPercentageDisregardNonRes" runat="server" LabelText="Percentage disregarded (Non Residential)" LabelWidth="12em" MaxLength="255" 
                    Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Percentage disregarded (Non Residential)" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat"  />
                    <br />
                </div>
                <div style="float: left; padding-left : 0.5em;"> 
                    <cc1:TextBoxEx ID="txtPercentageDisregardRes" runat="server" LabelText="Percentage disregarded (Residential)" LabelWidth="12em" MaxLength="255" 
                    Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Percentage disregarded (Residential)" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />
                    <br />
                </div>
            </div>
            <br />
    </fieldset>
    <br />
</asp:Content>
