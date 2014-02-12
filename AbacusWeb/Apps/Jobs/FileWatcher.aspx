<%@ Page Language="vb" AutoEventWireup="false" Codebehind="FileWatcher.aspx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.FileWatcher" EnableViewState="true" 
   ValidateRequest="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

    <asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
        To edit the folders and filenames that Job Service monitors, edit the details below and click the "Save" button.
    </asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    	    
	    <fieldset>
	        <legend>Important Notes</legend>
	        The Job Service monitors the folders below for the <strong>creation</strong> of files with the specified filename pattern. 
	        <br /><br />
	        The folder paths must be relative to the server where the Job Service is installed.
	        <br /><br />
	        The user account under which the Job Service runs must have access rights to the folder paths.
	        <br /><br />
	        Files that are already present in the folders when the Job Service starts will not be processed. The files must then be moved out of and back into the folders.
	        <br /><br />
	        Any changes made will not take effect until the Job Service is restarted.
	        <br /><br />
	        The available placeholders for "Folder" are: 
	        <ul>
	            <li><strong>&lt;datapath&gt;</strong> means the Abacus data path</li>
	        </ul>
	        The available placeholders for "Filename Pattern" are: 
	        <ul>
	            <li><strong>*</strong> means wildcard</li>
	        </ul>
	    </fieldset>
	    <br />
	    
	    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
	    
	    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
            <table class="listTable" cellpadding="4" cellspacing="0" width="100%" summary="Lists the folders monitored by the Job Service.">
            <caption>Lists the folders monitored by the Job Service.</caption>
            <thead>
                <tr>
	                <th>Folder</th>
	                <th>Filename Pattern</th>
                </tr>
            </thead>
            <tbody>
                <asp:Placeholder id="phWatchers" runat="server"></asp:Placeholder>
            </tbody>
	        </table>
        </fieldset>
            
    </asp:Content>