<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SettingEditor.ascx.vb" Inherits="Target.Web.Apps.ApplicationSystemSettings.UserControls.SettingEditor" %>

<asp:HiddenField ID="hdnSettingID" runat="server" />
<asp:HiddenField ID="hdnSettingEditable" runat="server" />
<asp:HiddenField ID="hdnSettingLastValue" runat="server" />
<asp:HiddenField ID="hdnSettingType" runat="server" />

<fieldset id="fsSettings" style="padding:0.5em;" runat="server">  
    <legend><asp:Literal ID="litSettingName" runat="server" /></legend>  
    <asp:Literal ID="litSettingDetails" runat="server" />
    <br />
    <br />
    <asp:PlaceHolder ID="phSettingControls" runat="server" />
    <br />
    <br />          
</fieldset> 

<br />



