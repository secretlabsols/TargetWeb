<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="NonResidentialParameters.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.AnnualParameters.NonResidentialParameters" EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="conPageError" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
</asp:Content>

<asp:Content ID="conContent" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" />
    <fieldset id="fsAllDomciciliaryParams" style="padding : 0.5em; width : 72em;" runat="server">
        <legend>Domiciliary Parameters</legend> 
        <div style="float : left; width : 22em; margin : 0em 0.5em 0em 0em;">
            <fieldset id="fsDomParamControls" style="padding:0.5em;" runat="server">  
                <legend>Details</legend>              
		        <cc1:TextBoxEx ID="txtDetailsFromDate" runat="server" LabelText="From Date" LabelWidth="11em"  
                Width="7em" Required="true" RequiredValidatorErrMsg="Please enter a From Date" SetFocus="true"
                ValidationGroup="Save" Format="DateFormatJquery" AllowClear="true" />       
                <br /> 
                <cc1:TextBoxEx ID="txtDetailsToDate" runat="server" LabelText="To Date" LabelWidth="11em" 
                Width="7em" Required="true" RequiredValidatorErrMsg="Please enter a To Date" SetFocus="false"
                ValidationGroup="Save" Format="DateFormatJquery" AllowClear="true" />
                <br />
                <cc1:TextBoxEx ID="txtDetailsMinCharge" runat="server" LabelText="Minimum Charge" LabelWidth="12em" MaxLength="255" 
                Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter a Minimum Charge" SetFocus="false"
                ValidationGroup="Save" Format="CurrencyFormat" />       
                <br /> 
                <cc1:TextBoxEx ID="txtDetailsMaxCharge" runat="server" LabelText="Maximum Charge" LabelWidth="12em" MaxLength="255" 
                Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter a Maximum Charge" SetFocus="false"
                ValidationGroup="Save" Format="CurrencyFormat" />       
                <br /> 
            </fieldset>
        </div>
        <div style="float : left; width : 49em;">
            <fieldset id="fsWhenAssessingClientControls" style="padding:0.5em;" runat="server">        
                <legend>When assessing the client</legend>    
                <asp:CheckBox ID="cbAssessingClientAutoCalculateIsAllowance" runat="server" Text="Auto-calculate I/S Allowance?" />
                <br />
            </fieldset>
            <fieldset id="fsWhenCalculatingWeeklyChargeControls" style="padding:0.5em;" runat="server">        
                <legend>When calculating the weekly charge</legend>    
                <asp:CheckBox ID="cbCalculatingWcDoNotApplyMinCharge" runat="server" Text="Do not apply Minimum Charge <br />(and cap Weekly Charge at the lower of the Assessed Charge and the Maximum Charge)" />
                <br />
                <br />
                <asp:CheckBox ID="cbCalculatingWcCapUnitCostOfServiceAtProvCharge" runat="server" Text="Cap the unit cost of service at the provider charge?" />
            </fieldset>
        </div> 
        <br />    
    </fieldset>  
    <fieldset id="fsAssessableIncome" style="padding : 0.5em; width : 72em;" runat="server">
        <legend>Assessable Income</legend>
        <div style="float : left; width : 22em; margin : 0em 0.5em 0em 0em;">
            <cc1:TextBoxEx ID="txtAssessableMig18to24" runat="server" LabelText="MIG 18-24 years" LabelWidth="12em" MaxLength="255" 
            Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter MIG 18-24 years" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />       
            <br />
            <cc1:TextBoxEx ID="txtAssessablePercentAssessableIncome" runat="server" LabelText="% Assessable Income" LabelWidth="12em" MaxLength="255" 
            Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter % Assessable Income" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />       
            <br />
            <cc1:TextBoxEx ID="txtAssessablePensionerCouple" runat="server" LabelText="Pensioner Couple" LabelWidth="12em" MaxLength="255" 
            Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter Pensioner Couple" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />       
            <br />
            <cc1:TextBoxEx ID="txtAssessableDreAllowance" runat="server" LabelText="DRE Allowance" LabelWidth="12em" MaxLength="255" 
            Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter DRE Allowance" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />       
            <br />
        </div>
        <div style="float : left; width : 22em; margin : 0em 0.5em 0em 0em;">
            <cc1:TextBoxEx ID="txtAssessableMig25to59" runat="server" LabelText="MIG 25-59 years" LabelWidth="12em" MaxLength="255" 
            Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter MIG 25-59 years" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />       
            <div style="height : 4.25em; width : 100%; clear : both;"></div>            
            <cc1:TextBoxEx ID="txtAssessableDisabledCouple" runat="server" LabelText="Disabled Couple" LabelWidth="12em" MaxLength="255" 
            Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter Disabled Couple" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />       
            <br />
            <cc1:TextBoxEx ID="txtAssessableDreAllowanceCouple" runat="server" LabelText="DRE Al'nce Couple" LabelWidth="12em" MaxLength="255" 
            Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter DRE Al'nce Couple" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />       
            <br />
        </div>
        <div style="float : left; width : 22em; margin : 0em 0.5em 0em 0em;">
            <cc1:TextBoxEx ID="txtAssessableMig60Up" runat="server" LabelText="MIG 60+ years" LabelWidth="12em" MaxLength="255" 
            Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter MIG 60+ years" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />       
            <div style="height : 7em; width : 100%; clear : both;"></div>
            <cc1:TextBoxEx ID="txtAssessableDreUpperLimit" runat="server" LabelText="DRE Upper Limit" LabelWidth="12em" MaxLength="255" 
            Width="6.5em" Required="false" RequiredValidatorErrMsg="Please enter DRE Upper Limit" SetFocus="false"
            ValidationGroup="Save" Format="CurrencyFormat" />       
            <br />
        </div>
    </fieldset>  
    <fieldset id="fsCapitalAllowanceControls" style="padding : 0.5em; width : 72em;" runat="server">
        <legend>Capital Allowances</legend>
        <div style="float : left; width : 30em; margin : 0em 0.5em 1em 0em;">
            <fieldset id="fsCapitalAllowanceDetailControls" style="padding:0.5em;" runat="server">  
                <legend>Details</legend>  
                <div style="float: left; clear : both; width : 100%">
                    <div style="float: left; width : 18.5em;">
                        <span style="padding-left : 11.25em">Singles</span>   
                        <br />                                                       
                        <cc1:TextBoxEx ID="txtCapAllowancesDisgregardSingle" runat="server" LabelText="Disregard" LabelWidth="11.25em" MaxLength="255" 
                        Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter Disregard (Singles)" SetFocus="false"
                        ValidationGroup="Save" Format="CurrencyFormat" />       
                        <br />  
                    </div> 
                    <div style="float: left; width : 8.5em;"> 
                        Couples
                        <br />                          
                        <cc1:TextBoxEx ID="txtCapAllowancesDisgregardCouple" runat="server" LabelText="" LabelWidth="0em" MaxLength="255" 
                        Width="8em" Required="true" RequiredValidatorErrMsg="Please enter Disregard (Couples)" SetFocus="false"
                        ValidationGroup="Save" Format="CurrencyFormat" />       
                        <br /> 
                    </div>  
                </div>
                <div style="float: left; clear : both; width : 100%">
                    <div style="float: left; width : 18.5em;">                                                    
                        <cc1:TextBoxEx ID="txtCapAllowancesUpperLimitSingle" runat="server" LabelText="Upper Limit" LabelWidth="11.25em" MaxLength="255" 
                        Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter Upper Limit (Singles)" SetFocus="false"
                        ValidationGroup="Save" Format="CurrencyFormat" />       
                        <br />  
                    </div> 
                    <div style="float: left; width : 8.5em;">                        
                        <cc1:TextBoxEx ID="txtCapAllowancesUpperLimitCouple" runat="server" LabelText="" LabelWidth="0em" MaxLength="255" 
                        Width="8em" Required="true" RequiredValidatorErrMsg="Please enter Upper Limit (Couples)" SetFocus="false"
                        ValidationGroup="Save" Format="CurrencyFormat" />       
                        <br /> 
                    </div>  
                </div>
            </fieldset>        
        </div>
        <div style="float : left; width : 41em;">
            <fieldset id="fsCapitalAllowanceCalculationControls" style="padding:0.5em;" runat="server">  
                <legend>Calculation</legend>  
                <div style="float: left; width : 50%">
                    <cc1:TextBoxEx ID="txtCapAllowancesCalculationWeeklyIncome" runat="server" LabelText="Weekly Income" LabelWidth="12em" MaxLength="255" 
                    Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter Weekly Income" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />       
                    <br /> 
                    <cc1:TextBoxEx ID="txtCapAllowancesCalculationPerPart" runat="server" LabelText="Per Part" LabelWidth="12em" MaxLength="255" 
                    Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter Per Part" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />       
                    <br /> 
                </div>
                <div style="float: left; width : 50%; clear : right; padding : 1.25em 0em 0em 0em;">
                    <cc1:TextBoxEx ID="txtCapAllowancesCalculationInterestRate" runat="server" LabelText="... or ... Interest Rate" LabelWidth="12em" MaxLength="255" 
                    Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter Interest Rate" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />       
                    <br />
                </div>
            </fieldset>       
        </div>
    </fieldset>    
</asp:Content>
