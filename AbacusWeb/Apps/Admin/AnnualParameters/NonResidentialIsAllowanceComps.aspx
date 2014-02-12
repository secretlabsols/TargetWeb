<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="NonResidentialIsAllowanceComps.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.AnnualParameters.NonResidentialIsAllowanceComps" EnableViewState="false" %>
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
        <div style="float : left; height : 150px;">
            <div style="float : left; width : 38em; height : 100%; margin : 0em 0.5em 0em 0em;">
                <fieldset id="fsDetailControls" style="padding:0.5em; height : 100%" runat="server">  
                    <legend>Details</legend>  
                    <div style="width : 100%; clear : both; float : left;">
		                <div style="float: left; width : 19.5em;">
		                    <cc1:TextBoxEx ID="txtFromDate" runat="server" LabelText="From Date" LabelWidth="12em" MaxLength="255" 
                            Width="10.4em" Required="true" RequiredValidatorErrMsg="Please enter a From Date" SetFocus="true"
                            ValidationGroup="Save" Format="DateFormatJquery"  AllowClear="true" />       
                            <br />  
                        </div> 
                        <div style="float: left; clear : right; margin-left : 1em; width : 14em;">         
                            <cc1:TextBoxEx ID="txtToDate" runat="server" LabelText="To Date" LabelWidth="6em" MaxLength="255" 
                            Width="10.4em" Required="false" RequiredValidatorErrMsg="Please enter a To Date" SetFocus="false"
                            ValidationGroup="Save" Format="DateFormatJquery" AllowClear="true" />
                            <br />
                        </div> 
                    </div>                                   
                    <cc1:DropDownListEx ID="cboPremium" runat="server" LabelText="Premium" LabelWidth="12em" 
		            Width="21.5em" Required="false" RequiredValidatorErrMsg="Please select a Premium" ValidationGroup="Save"
                    />
		            <br />
		            <cc1:DropDownListEx ID="cboPremiumRecipient" runat="server" LabelText="Recipient of Premium" LabelWidth="12em" 
		            Width="21.5em" Required="false" RequiredValidatorErrMsg="Please select a Recipient of Premium" ValidationGroup="Save" />
		            <br />          
                </fieldset>
            </div>
            <div style="float : left; width : 30em; height : 100%;">
                <fieldset id="fsAssessedAsControls" style="padding:0.5em; height : 100%" runat="server">        
                    <legend>Assessed As</legend>  
                    <asp:RadioButtonList ID="rblAssessedAs" runat="server" RepeatDirection="Vertical" RepeatLayout="Table" EnableViewState="true">
                        <asp:ListItem Text="an Individual" Value="INDIVIDUAL" Selected="True" />
                        <asp:ListItem Text="a Couple" Value="COUPLE" />
                        <asp:ListItem Text="a Half Couple" Value="HALF COUPLE" />
                    </asp:RadioButtonList>
                    <br /> 
                    <div style="width : 100%; clear : both; float : left;">
                        <div style="float: left; width : 13em;">
                            <cc1:TextBoxEx ID="txtFromAge" runat="server" LabelText="From Age" LabelWidth="6em" MaxLength="255" 
                            Width="6em" Required="true" RequiredValidatorErrMsg="Please enter a From Age" SetFocus="false"
                            ValidationGroup="Save" Format="IntegerFormat" />       
                            <br />  
                        </div>    
                        <div style="float: left; clear : right; margin-left : 1.75em; width : 13em;">     
                            <cc1:TextBoxEx ID="txtToAge" runat="server" LabelText="To Age" LabelWidth="6em" MaxLength="255" 
                            Width="6em" Required="false" RequiredValidatorErrMsg="Please enter a To Age" SetFocus="false"
                            ValidationGroup="Save" Format="IntegerFormat" />
                            <br />
                        </div> 
                    </div> 
                    <br /> 
                </fieldset>
            </div> 
        </div>
        <div style="clear: both; margin : 2em 0em 0em 0em; float : left;">
            <div style="float : left; width : 38em; height : 100%; margin : 0em 0.5em 0em 0em;">
                <fieldset id="fsAllowanceControls" style="padding:0.5em;" runat="server">
                        
                    <legend>Allowance</legend>  
                    <cc1:TextBoxEx ID="txtAllowance" runat="server" LabelText="Allowance" LabelWidth="12em" MaxLength="255" 
                    Width="21.5em" Required="true" RequiredValidatorErrMsg="Please enter Allowance" SetFocus="false"
                    ValidationGroup="Save" Format="CurrencyFormat" />
                    <br /> 
                </fieldset>          
            </div>
        </div>
        <br />    
    </fieldset>
    <br />
</asp:Content>
