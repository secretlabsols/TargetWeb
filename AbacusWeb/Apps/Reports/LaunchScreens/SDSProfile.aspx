<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SDSProfile.aspx.vb" Inherits="Target.Abacus.Web.Apps.Reports.LaunchScreens.SDSProfile"   masterpagefile="~/Popup.Master"%>

<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="ReportHeader" Src="~/AbacusWeb/Apps/Reports/LaunchScreens/ReportHeader.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <div style="margin-left:0.5em; margin-top:0.5em;">
        <h2>SDS Profile Report</h2>
        <cc1:CollapsiblePanel id="cpDetail" runat="server" HeaderLinkText="Report Parameters" MaintainClientState="true"> 
        <ContentTemplate>
           <%-- <fieldset id="fsControls" runat="server">--%>
	            <input id="optShowAll" runat="server" type="radio" name="type" value="1" style="float:left; margin-left:1em;" onclick="javascript:optType_Click();" />
                <label class="label" style="float:left" for="optShowAll" >Show All Service Users</label>
                <div class="clearer" />
                <input id="optPermResOnly" runat="server" type="radio" name="type" value="2" style="float:left; margin-left:1em;" onclick="javascript:optType_Click();" />
                <label class="label" style="float:left" for="optPermResOnly" >Only Show Permanent Residential Admissions</label>
                <div class="clearer" />
                <input id="optExcludePermRes" runat="server" type="radio" name="type" value="3" style="float:left; margin-left:1em;" onclick="javascript:optType_Click();" />
                <label class="label" style="float:left" for="optExcludePermRes" >Do Not Show Permanent Residential Admissions</label>
                
                <div class="clearer" />
                <div style="float:left; margin-left:1em;">
                <cc1:DropDownListEx ID="cboOutput"  OutputBrAfter="false" runat="server" LabelText="Ouput To"  LabelWidth="6em" Required="true" >
		        </cc1:DropDownListEx>
		        </div>
		        <input type="button" id="btnReport" runat="server" style="float:right;width:10em;" value="Generate Report" />
		        <div class="clearer" />
            <%--</fieldset>--%>
        </ContentTemplate>
        </cc1:CollapsiblePanel>
    </div>
    <asp:Chart ID="Chart1" runat="server">

    </asp:Chart>
    <uc2:ReportsButton id="btnView" disabled="False" runat="server" ButtonText="View"></uc2:ReportsButton>
    <div id="divDownloadContainer" style="display:none;" runat="server"></div>
</asp:Content>
