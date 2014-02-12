<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="OutgoingMaxAmounts.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.AnnualParameters.OutgoingMaxAmounts" EnableViewState="false" %>
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
        <fieldset id="fsOutgoingControls" style="padding:0.5em;" runat="server">  
            <legend>Outgoing</legend>  
            <cc1:DropDownListEx ID="cboOutgoing" runat="server" LabelText="Outgoing" LabelWidth="12em" 
            Width="23em" Required="true" RequiredValidatorErrMsg="Please select Outgoing" ValidationGroup="Save" />
            <br />
            <div style="float: left; width : 21em; ">
                <cc1:TextBoxEx ID="txtFromDate" runat="server" LabelText="From Date" LabelWidth="12em"  
                Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter a From Date" SetFocus="true"
                ValidationGroup="Save" Format="DateFormatJquery" AllowClear="true"/>       
                <br />  
            </div> 
            <div style="float: left; clear : right; margin-left : 0.5em; width : 20em;">         
                <cc1:TextBoxEx ID="txtToDate" runat="server" LabelText="To Date" LabelWidth="5em"  
                Width="6.5em" Required="true" RequiredValidatorErrMsg="Please enter a To Date" SetFocus="false"
                ValidationGroup="Save" Format="DateFormatJquery" AllowClear="true" />
                <br />
            </div>                                            
        </fieldset>        
        <br /> 
        <fieldset id="fsMaximumAmountControls" style="padding:0.5em;" runat="server"> 
            <legend>Maximum amounts for assessments</legend>        
            <div style="float: left; clear : both;">
                <div style="float: left; width : 25em;">
                    <span style="padding-left : 16em">Non-Residential</span>   
                    <br />                                                       
                    <cc1:TextBoxEx ID="txtMaximumTakenIntoAccountNonRes" runat="server" LabelText="Maximum taken into account" LabelWidth="16em" MaxLength="255" 
                    Width="8em" Required="true" RequiredValidatorErrMsg="Please enter Maximum taken into account" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />       
                    <br />  
                </div> 
                <div style="float: left; padding-left : 0.5em; width : 22em;"> 
                    <span style="padding-left : 1em">Residential</span>
                    <br />                          
                    <cc1:TextBoxEx ID="txtMaximumTakenIntoAccountRes" runat="server" LabelText="" LabelWidth="1em" MaxLength="255" 
                    Width="8em" Required="true" RequiredValidatorErrMsg="Please enter Maximum taken into account" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />       
                    <br /> 
                </div>  
            </div>
            <br /> 
            <div style="float: left; clear : both;">
                <div style="float: left; width : 25em;">
                    <cc1:TextBoxEx ID="txtMaximumReasonableAmountNonRes" runat="server" LabelText="Maximum reasonable amount" LabelWidth="16em" MaxLength="255" 
                    Width="8em" Required="true" RequiredValidatorErrMsg="Please enter Maximum reasonable ammount" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />       
                    <br />  
                </div> 
                <div style="float: left; padding-left : 0.5em; width : 22em;">         
                    <cc1:TextBoxEx ID="txtMaximumReasonableAmountRes" runat="server" LabelText="" LabelWidth="1em" MaxLength="255" 
                    Width="8em" Required="true" RequiredValidatorErrMsg="Please enter Maximum reasonable ammount" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />       
                    <br /> 
                </div>            
            </div>                                                      
        </fieldset>    
    </fieldset>
    <br />
</asp:Content>
