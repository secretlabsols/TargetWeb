<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="AssessmentBandRates.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.AssessmentBandRates" EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different Assessment Band Rates.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" ></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:DropDownListEx ID="cboAssessmentBand" runat="server" LabelText="Assessment Band" LabelWidth="10em"  
			Width="28em" Required="true" RequiredValidatorErrMsg="Please select an Assessment Band" ValidationGroup="Save" />
		<br />
		<div style="width : 100%; clear : both; float : left;">
            <div style="float : left;">
	            <cc1:TextBoxEx ID="txtFromDate" runat="server" LabelText="Date From" LabelWidth="10em"  
                Width="10.3em" Required="true" RequiredValidatorErrMsg="Please enter a Date From" 
                ValidationGroup="Save" Format="DateFormatJquery" AllowClear="true" /> 
                <br />
            </div> 
            <div style="padding-left : 1.5em; float : left;">         
                <cc1:TextBoxEx ID="txtToDate" runat="server" LabelText="to" LabelWidth="2em" 
                Width="10.3em" ValidationGroup="Save" Format="DateFormatJquery" AllowClear="true" />
                <br /> 
            </div>  
        </div>
        <cc1:TextBoxEx ID="txtAssessedCharge"  runat="server"  LabelText="Assessed Charge" LabelWidth="10em" MaxLength="50" 
            Width="28em"  ValidationGroup="Save" Format="CurrencyFormat"></cc1:TextBoxEx>

    </fieldset>
    <br />
</asp:Content>