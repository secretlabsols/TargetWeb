<%@ Control Language="vb" AutoEventWireup="false" Codebehind="ServiceOrderReports.ascx.vb" Inherits="Target.Abacus.Extranet.Apps.UserControls.ServiceOrderReports" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<br /><br />

<fieldset class="availableReports">
    <legend>Available Reports</legend>
    <asp:ListBox ID="lstReports" runat="server"></asp:ListBox>
</fieldset>

<fieldset id="fsSelectedReport" class="selectedReport">
    <legend>Selected Report</legend>
    <div id="divDefault">Please select a report from the list</div>
        
    <!-- DSO list -->
    <div id="divDsoList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlDsoList" runat="server"></uc1:ReportsButton>
    </div>
    
    <!-- DSO detail list -->
    <div id="divDsoDetailList" runat="server" class="availableReport">
   
        <fieldset>
         <legend>Additional Filters</legend>
            <cc1:TextBoxEx ID="detailWeekEndingDate" runat="server" Format="DateFormatJquery" 
            LabelWidth="7em"  LabelText="Week Ending"  Width="6em" AllowClear="false" ></cc1:TextBoxEx>

            <asp:CheckBox ID="chkDonotfilter" Text="Do not filter by week ending date" TextAlign="Left" runat="server"/>
        </fieldset>
   
        <br />
        <uc1:ReportsButton id="ctlDsoDetailList" runat="server"></uc1:ReportsButton>
    </div>
    
      

    <!-- DSO suspension list -->
    <div id="divDsoSuspensionList" runat="server" class="availableReport">
        <uc1:ReportsButton id="ctlDsoSuspensionList" runat="server"></uc1:ReportsButton>
    </div>
            
</fieldset>
<div class="clearer"></div>
