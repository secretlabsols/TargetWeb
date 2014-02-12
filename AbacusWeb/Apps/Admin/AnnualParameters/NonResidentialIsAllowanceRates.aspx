<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="NonResidentialIsAllowanceRates.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.AnnualParameters.NonResidentialIsAllowanceRates" EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="conPageOverview" ContentPlaceHolderID="MPPageOverview" runat="server">

</asp:Content>

<asp:Content ID="conPageError" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
</asp:Content>

<asp:Content ID="conContent" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" />
    <fieldset id="fsAll" style="padding:0.5em;" runat="server">
        <div style="float : left; width : 38em; margin : 0px 0.5em 10px 0px;">
            <fieldset id="fsDetailControls" style="padding:0.5em;" runat="server">  
                <legend>Details</legend>      
                <cc1:DropDownListEx ID="cboType" runat="server" LabelText="Type" LabelWidth="12em" 
		        Width="21.5em" Required="true" RequiredValidatorErrMsg="Please select a Type" ValidationGroup="Save" />
		        <br />
		        <div style="width : 100%; clear : both; float : left;">
		            <div style="float: left; width : 19.5em;">
		                <cc1:TextBoxEx ID="txtFromDate" runat="server" LabelText="From Date" LabelWidth="12em" MaxLength="255" 
                        Width="20em" Required="true" RequiredValidatorErrMsg="Please enter a From Date" SetFocus="true"
                        ValidationGroup="Save" Format="DateFormat" />       
                        <br />  
                    </div> 
                    <div style="float: left; clear : right; margin-left : 1em; width : 15em;">         
                        <cc1:TextBoxEx ID="txtToDate" runat="server" LabelText="To Date" LabelWidth="6em" MaxLength="255" 
                        Width="20em" Required="true" RequiredValidatorErrMsg="Please enter a To Date" SetFocus="false"
                        ValidationGroup="Save" Format="DateFormat" />
                        <br />
                    </div>
                </div> 
                <div style="width : 100%; clear : both; float : left;">               
                    <div style="float: left; width : 19.5em;">
                        <cc1:TextBoxEx ID="txtFromAge" runat="server" LabelText="From Age" LabelWidth="12em" MaxLength="255" 
                        Width="6em" Required="true" RequiredValidatorErrMsg="Please enter a From Age" SetFocus="false"
                        ValidationGroup="Save" Format="IntegerFormat" />       
                        <br />  
                    </div>    
                    <div style="float: left; clear : right; margin-left : 1em; width : 15em">     
                        <cc1:TextBoxEx ID="txtToAge" runat="server" LabelText="To Age" LabelWidth="6em" MaxLength="255" 
                        Width="6em" Required="true" RequiredValidatorErrMsg="Please enter a To Age" SetFocus="false"
                        ValidationGroup="Save" Format="IntegerFormat" />
                        <br />
                    </div>
                </div>
            </fieldset>
        </div>
        <div style="float : left; width : 20em; margin : 0px 0.5em 20px 0px;">
            <fieldset id="fsAllowanceControls" style="padding:0.5em;" runat="server">        
                <legend>Allowances</legend>    
                <cc1:TextBoxEx ID="txtAllowanceSingle" runat="server" LabelText="Single" LabelWidth="8em" MaxLength="255" 
                Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Single Allowance" SetFocus="false"
                ValidationGroup="Save" Format="CurrencyFormat" />
                <br />  
                <cc1:TextBoxEx ID="txtAllowanceCouple" runat="server" LabelText="Couple" LabelWidth="8em" MaxLength="255" 
                Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Couple Allowance" SetFocus="false"
                ValidationGroup="Save" Format="CurrencyFormat" />
                <br /> 
                <cc1:TextBoxEx ID="txtAllowanceHalfCouple" runat="server" LabelText="Half Couple" LabelWidth="8em" MaxLength="255" 
                Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Half Couple Allowance" SetFocus="false"
                ValidationGroup="Save" Format="CurrencyFormat" />
                <br /> 
            </fieldset>
        </div> 
        <div style="float : left; width : 20em; margin : 0px 0px 20px 0px;">
            <fieldset id="fsDWPMIGRates" style="padding:0.5em;" runat="server">        
                <legend>DWP MIG Rates</legend>    
                <cc1:TextBoxEx ID="txtDWPMIGAllowanceSingle" runat="server" LabelText="Single" LabelWidth="8em" MaxLength="255" 
                Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Single DWP MIG Rate" SetFocus="false"
                ValidationGroup="Save" Format="CurrencyFormat" />
                <br />  
                <cc1:TextBoxEx ID="txtDWPMIGAllowanceCouple" runat="server" LabelText="Couple" LabelWidth="8em" MaxLength="255" 
                Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Couple Allowance DWP MIG Rate" SetFocus="false"
                ValidationGroup="Save" Format="CurrencyFormat" />
                <br /> 
                <cc1:TextBoxEx ID="txtDWPMIGAllowanceHalfCouple" runat="server" LabelText="Half Couple" LabelWidth="8em" MaxLength="255" 
                Width="7em" Required="true" RequiredValidatorErrMsg="Please enter Half Couple Allowance DWP MIG Rate" SetFocus="false"
                ValidationGroup="Save" Format="CurrencyFormat" />
                <br /> 
            </fieldset>
        </div> 
        <br />    
    </fieldset>       
</asp:Content>
