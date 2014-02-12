<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EmailTemplates.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Misc.EmailTemplates" ValidateRequest="False"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ftb" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="9em" MaxLength="255" 
                Width="20em" IsReadOnly="true" ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtSubject"  runat="server"  LabelText="Subject" LabelWidth="9em" MaxLength="255" 
                Width="82%" Required="true" RequiredValidatorErrMsg="Please enter a subject" SetFocus="true"
                ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <asp:Label runat="server" Text="Message" style="float:left;" Width="9em"></asp:Label>
        <div id="divMessage" style="float:left">
            <div id="divContent" runat="server" style="width:99%;"></div>
            <div id="divFreebox" runat="server" >
                <FTB:FreeTextBox id="ftbContent" runat="server" 
                        AutoHideToolbar="False" 
                        AutoGenerateToolbarsFromString="True" 
                        SupportFolder="~/Apps/CMS/" 
                        StartMode="DesignMode" 
                        StripAllScripting="False" 
                        Focus="False" 
                        JavaScriptLocation="ExternalFile"
                        Width="99%">
                       
                </FTB:FreeTextBox>
            </divFreebox>
            <br />
            <asp:Label runat="server" Text="Note: placeholders can be used in both the Subject and Message fields." Width="90%"></asp:Label>
        </div>
    </fieldset>
    <br />
</asp:Content>
