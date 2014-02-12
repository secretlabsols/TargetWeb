<%@ Page Language="vb" AutoEventWireup="false" Codebehind="JobSchedules.aspx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.JobSchedules" EnableViewState="true" %>

    <asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
        This screen allows you to view and manage Abacus Job Service jobs that reoccur on a scheduled basis.
    </asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <table class="listTable" id="tblJobs" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of Recurring Jobs.">
        <caption>List of Recurring Jobs.</caption>
        <thead>
	        <tr>
		        <th style="width:1.5em;"></th>
		        <th id="thDescription" style="width:40%">Description</th>
		        <th id="thJobType" style="width:35%">Job Type</th>
		        <th id="thEnabled" style="width:10%">Enabled</th>
		        <th id="thNextRun" style="width:15%">Next Run</th>
	        </tr>
        </thead>
        <tbody><tr><td></td><td></td><td></td><td></td></tr></tbody>
        </table>
        <div id="JobSchedule_PagingLinks" style="float:left;"></div>
        <input type="button" id="btnView" runat="server" style="float:right;width:7em;" value="View" onclick="btnView_Click();" />
        <input type="button" id="btnNew" runat="server" style="float:right;width:7em;" value="New" onclick="btnNew_Click();" /> 
        <div class="clearer"></div>	    
    </asp:Content>
