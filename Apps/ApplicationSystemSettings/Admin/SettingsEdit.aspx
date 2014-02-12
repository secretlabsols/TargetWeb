<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Popup.Master" CodeBehind="SettingsEdit.aspx.vb" Inherits="Target.Web.Apps.ApplicationSystemSettings.Admin.SettingsEdit" %>

<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
	<asp:Literal id="litPageError" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MPContent" runat="server">
    <asp:Panel ID="pnlForm" runat="server">
        <asp:UpdateProgress ID="updProgress" AssociatedUpdatePanelID="upUpdater" runat="server">
            <ProgressTemplate> 
            <div class="SplashScreen">
                <div class="SplashScreenMessage">
                    Saving Settings...
                    <br />
                    <img alt="progress" src="../../../Images/busy.gif" />
                </div>                
            </div>                 
            </ProgressTemplate>
        </asp:UpdateProgress>
            
        <asp:UpdatePanel ID="upUpdater" runat="server">
            <ContentTemplate>    
                <asp:ValidationSummary ID="vldSummary" runat="server" HeaderText="Validation errors occurred whilst attempting to save, please review the errors below and try again.<br />" Font-Bold="true" EnableClientScript="true" ValidationGroup="Save" DisplayMode="SingleParagraph" />    
                <uc1:StdButtons id="stdButtons1" runat="server" />    
                <asp:PlaceHolder ID="phSettingControls" runat="server"/> 
            </ContentTemplate>        
        </asp:UpdatePanel>      
    </asp:Panel>     
        
</asp:Content>