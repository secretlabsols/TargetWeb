<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Popup.Master" CodeBehind="ServiceUserMinutesCalc.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.ServiceUserMinutesCalc" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="cc1" Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPContent" runat="server">
    <asp:Label ID="lblPageError" runat="server" CssClass="errorText"></asp:Label>

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
                <uc1:StdButtons id="stdButtons1" runat="server" />    
                <asp:PlaceHolder ID="phSettingControls" runat="server" /> 
            </ContentTemplate>
        </asp:UpdatePanel>      
    </asp:Panel>     
        
</asp:Content>