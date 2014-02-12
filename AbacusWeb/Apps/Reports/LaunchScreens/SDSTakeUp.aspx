<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SDSTakeUp.aspx.vb" Inherits="Target.Abacus.Web.Apps.Reports.LaunchScreens.SDSTakeUp" masterpagefile="~/Popup.Master"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="ReportHeader" Src="~/AbacusWeb/Apps/Reports/LaunchScreens/ReportHeader.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<%@ Register TagPrefix="uc3" TagName="DateRange" Src="~/AbacusWeb/Apps/Reports/LaunchScreens/DateRange.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <div style="margin:1em;">
        <h2>Launch Report</h2>
        <fieldset id="fsControls" style="padding:0.5em;" runat="server">
            <uc1:ReportHeader id="ctlRptHeader" runat="server"></uc1:ReportHeader>
            <br /><br />
            <uc3:DateRange id="ctlDateRange" runat="server"></uc3:DateRange>
            <input id="optShowAll" runat="server" type="radio" name="type" value="1" style="float:left; margin-left:10.5em;" onclick="javascript:optType_Click();" />
            <label class="label" style="float:left" for="optShowAll" >Show All Service Users</label>
            <div class="clearer" />
            <input id="optPermResOnly" runat="server" type="radio" name="type" value="2" style="float:left; margin-left:10.5em;" onclick="javascript:optType_Click();" />
            <label class="label" style="float:left" for="optPermResOnly" >Only Show Permanent Residential Admissions</label>
            <div class="clearer" />
            <input id="optExcludePermRes" runat="server" type="radio" name="type" value="3" style="float:left; margin-left:10.5em;" onclick="javascript:optType_Click();" />
            <label class="label" style="float:left" for="optExcludePermRes" >Do Not Show Permanent Residential Admissions</label>
            <div class="clearer" />
            <br />
	    </fieldset>
	    <br />
	    <div style="float:right;"><uc2:ReportsButton id="btnView" disabled="False" runat="server" ButtonText="View"></uc2:ReportsButton></div>
	    <div class="clearer"></div>
	</div>
	
</asp:Content>