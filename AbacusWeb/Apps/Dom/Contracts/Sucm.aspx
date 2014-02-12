<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Sucm.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.Sucm" 
	EnableEventValidation="false" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="cc2" Namespace="Target.Web.Apps.ApplicationSystemSettings.UserControls" Assembly="Target.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ucServiceUserMinutesCalc" Src="~/AbacusWeb/Apps/UserControls/ucServiceUserMinutesCalc.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
    <div style="width : 100%;">
        <asp:radiobutton id="optUseSystemSettings" groupname="grpUseSettings" TextAlign="right" height="2em" width="100%" 
            runat="server" text="Use the Service User Minute Calculation Method from System Settings" checked="True" onclick="javascript:optUseSettings_Click();" ToolTip="Use default settings" />
            
        <asp:radiobutton id="optUseLocalSettings" groupname="grpUseSettings" TextAlign="right" height="2em" width="100%" 
            runat="server" text="Use the Service User Minute Calculation Method below:" onclick="javascript:optUseSettings_Click();" ToolTip="Use settings specific to this contract period" />
            
        <div style="float:left;" runat="server" id="divUseLocalSettings">
            <div style="float:left;">
                <uc2:ucServiceUserMinutesCalc ID="ucCalcMethod" runat="server" ValidationGroup="Save" />        
            </div>
            <div class="clearer" />  
        </div>
    </div>
    <div class="clearer"></div>
    </fieldset>
    <br />
</asp:Content>