<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommitmentProfile.aspx.vb" Inherits="Target.Abacus.Web.Apps.Reports.LaunchScreens.CommitmentProfile"   masterpagefile="~/Popup.Master"%>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<%@ Register TagPrefix="uc3" TagName="DateRange" Src="~/AbacusWeb/Apps/Reports/LaunchScreens/DateRange.ascx" %>

<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <div style="margin-left:0.5em; margin-top:0.5em;">
        <h2>Commitment Profile Report</h2>
        <cc1:CollapsiblePanel id="cpDetail" runat="server" HeaderLinkText="Report Parameters" MaintainClientState="true"> 
        <ContentTemplate>
            <uc3:DateRange id="ctlDateRange" runat="server"></uc3:DateRange>
            <fieldset id="fsSelectedReport" >
                <legend>Report Type</legend>
                <input id="optCommType" runat="server" type="radio" name="type" value="1" style="float:left;" onclick="javascript:optType_Click();" />
                <label class="label" style="float:left" for="optShowAll" >Show Commitment by Commitment Type / Transaction Type</label>
                <div class="clearer" />
                <input id="optServClass" runat="server" type="radio" name="type" value="2" style="float:left;" onclick="javascript:optType_Click();" />
                <label class="label" style="float:left" for="optPermResOnly" >Show Commitment by Service Classification / Service Type</label>
                <div class="clearer" />
                <input id="optServType" runat="server" type="radio" name="type" value="3" style="float:left;" onclick="javascript:optType_Click();" />
                <label class="label" style="float:left" for="optExcludePermRes" >Show Commitment by Service Type / Budget Category</label>
                <div class="clearer" />
                <div style="padding-left:2em;">
                    <cc1:DropDownListEx ID="cboSvcClassification"  OutputBrAfter="false" runat="server" LabelText="Service Classification"  LabelWidth="12em" Required="true" >
	                </cc1:DropDownListEx>
	            </div>
	        </fieldset>
            <br />
            <div class="clearer" />
            <div style="float:left;">
            <cc1:DropDownListEx ID="cboOutput"  OutputBrAfter="false" runat="server" LabelText="Ouput To"  LabelWidth="10.5em" Required="true" >
	        </cc1:DropDownListEx>
	        </div>
	        <input type="button" id="btnReport" runat="server" style="float:right;width:10em;" value="Generate Report" />
	        <div class="clearer" />
        </ContentTemplate>
        </cc1:CollapsiblePanel>
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </div>
    
    <asp:Chart ID="Chart1" runat="server">

    </asp:Chart>
    <uc2:ReportsButton id="btnView" disabled="False" runat="server" ButtonText="View"></uc2:ReportsButton>
    <div id="divDownloadContainer" style="display:none;" runat="server"></div>
</asp:Content>

