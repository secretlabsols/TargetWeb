<%@ Page Language="vb" AutoEventWireup="false" Codebehind="AccountPolicy.aspx.vb" Inherits="Target.Web.Apps.Security.Admin.AccountPolicy" 
	EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="PasswordExceptionSelector" Src="../UserControls/PasswordExceptionSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to change the login account policies for the application.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litPageError" runat="server"></asp:Literal>
	</asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
        
        <fieldset id="fsAccountPolicy" style="padding:0.5em;" runat="server">
            <legend>Account Policy</legend>
            <cc1:CheckBoxEx ID="chkPreventMultipleLogins" runat="server" LabelWidth="25em" 
                Text="Prevent the same login being used simultaneously from more than one computer"></cc1:CheckBoxEx>
            <br /><br />
        </fieldset>
        <br />
        
        <fieldset id="fsPasswordPolicy" style="padding:0.5em;" runat="server">
            <legend>Password Policy</legend>
            <cc1:TextBoxEx ID="txtMinLength" runat="server" LabelText="MinLength" LabelWidth="25em" Format="IntegerFormat"
                Required="true" RequiredValidatorErrMsg="Please enter a minimum password length" 
                MaxLength="5" Width="3em" ValidationGroup="Save"></cc1:TextBoxEx>
    	    <br />
            <cc1:CheckBoxEx ID="chkLowerCase" runat="server" LabelWidth="25em"></cc1:CheckBoxEx>
            <br /><br /><br />
            <cc1:CheckBoxEx ID="chkUpperCase" runat="server" LabelWidth="25em"></cc1:CheckBoxEx>
            <br /><br /><br />
            <cc1:CheckBoxEx ID="chkNumber" runat="server" LabelWidth="25em"></cc1:CheckBoxEx>
            <br /><br /><br />
            <cc1:CheckBoxEx ID="chkSpecialChar" runat="server" LabelWidth="25em"></cc1:CheckBoxEx>
            <asp:Label ID="lblSpecialChars" runat="server" CssClass="validValues"></asp:Label>
            <br /><br /><br />
	        <cc1:CheckBoxEx ID="chkUsernameBased" runat="server" LabelWidth="25em"></cc1:CheckBoxEx>
            <br /><br /><br />
            <cc1:CheckBoxEx ID="chkSameChar" runat="server" LabelWidth="25em"></cc1:CheckBoxEx>
            <br /><br /><br />
            <cc1:CheckBoxEx ID="chkUseRecaptcha" runat="server" LabelWidth="25em"></cc1:CheckBoxEx>
            <br /><br /><br />
            <fieldset>
                <legend>Password Exceptions</legend>
                <cc1:CheckBoxEx ID="chkDictionaryWord" runat="server" LabelWidth="25em"></cc1:CheckBoxEx>
                <br /><br /><br />
                <cc1:DualList ID="dlLists" runat="server" 
                    SrcListCaption="Available Lists" SrcListWidth="20em" SrcListRows="5"
                    DestListCaption="Selected Lists" DestListWidth="20em" DestListRows="5">
                </cc1:DualList>
                <div class="clearer"></div>
                <br />
                <div>
                    <input type="button" id="btnManagePasswordException" runat="server" style="width:16em;" value="Manage Password Exceptions" onclick="document.location.href='PasswordExceptions.aspx?mode=1&backUrl=' + escape(document.location.href);" />
                </div>
            </fieldset>
            <br /><br />
            <cc1:TextBoxEx ID="txtBlockHistoricDays" runat="server" LabelText="BlockHistoricDays" LabelWidth="25em" Format="IntegerFormat"
                Required="true" RequiredValidatorErrMsg="Please enter a block historic days value" 
                MaxLength="5" Width="3em" ValidationGroup="Save"></cc1:TextBoxEx>
    	    <br />
    	    <cc1:TextBoxEx ID="txtExpiryDays" runat="server" LabelText="ExpiryDays" LabelWidth="25em" Format="IntegerFormat"
                Required="true" RequiredValidatorErrMsg="Please enter an expiry days value" 
                MaxLength="5" Width="3em" ValidationGroup="Save"></cc1:TextBoxEx>
    	    <br />
    	    <cc1:TextBoxEx ID="txtRejectedLoginLimit" runat="server" LabelText="RejectedLoginLimit" LabelWidth="25em" Format="IntegerFormat"
                Required="true" RequiredValidatorErrMsg="Please enter a rejected login limit value" 
                MaxLength="5" Width="3em" ValidationGroup="Save"></cc1:TextBoxEx>
    	    <br />
	    </fieldset>
		    
    </asp:Content>