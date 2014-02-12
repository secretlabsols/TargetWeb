<%@ Page Language="vb" AutoEventWireup="false" Codebehind="JobStepInputDefaults.aspx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.JobStepInputDefaults" EnableViewState="true" 
   ValidateRequest="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

    <asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
        To edit the job step input default values, edit the details below and click the "Save" button.
    </asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    
	    <fieldset>
	        <legend>Important Notes</legend>
	        The job step input default values displayed below are used when jobs are automatically created by the Job Service file watcher.
	        These default values are also displayed (and can be overridden) on the <a href="CreateNew.aspx">create new job</a> screen.
	        <br /><br />
	        When the job is created, these values are saved against the relevant job step, ready for when the job is executed.
	        <br /><br />
	        The available placeholders for "Default Value" are: 
	        <ul>
	            <li><strong>%DataPath%</strong> means the Abacus data path</li>
	            <li><strong>%FileWatcherFile%</strong> means the full path to the file watcher file that triggered the creation of the job</li>
	            <li><strong>%FileWatcherFolder%</strong> means the folder which contains the file watcher file that triggered the creation of the job</li>
	        </ul>
	    </fieldset>
	    <br />
	    
	    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
	    
	    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
            <table class="listTable" cellpadding="4" cellspacing="0" width="100%" summary="Lists the job step input default values.">
            <caption>Lists the job step input default values.</caption>
            <thead>
                <tr>
	                <th>Job</th>
	                <th>Job Step</th>
	                <th>Input</th>
	                <th>Default Value</th>
                </tr>
            </thead>
            <tbody>
                <asp:Placeholder id="phDefaults" runat="server"></asp:Placeholder>
            </tbody>
	        </table>
        </fieldset>
            
    </asp:Content>