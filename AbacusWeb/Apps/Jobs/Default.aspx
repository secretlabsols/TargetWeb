<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Default.aspx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.DefaultPage" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <!-- JobList view containers -->
	    <div id="JobList_PageTitle" class="hidden">Job Service - View Jobs</div>
	    <div id="JobList_PageOverview" class="hidden">The list below displays the jobs submitted to the Abacus Job Service. Use the "Filters" to help find the job you are interested in.</div>
	    <div id="JobList_Content" class="hidden">
    	
    	    <fieldset style="float:left;width:50%;">
    	        <legend>Filters</legend>
    	        
    	        <cc1:DropDownListEx id="cboUser" runat="server" LabelText="Created By" LabelWidth="8em" Width="20em"></cc1:DropDownListEx>
    	        <br />
    	        <cc1:DropDownListEx id="cboJobStatus" runat="server" LabelText="Job Status" LabelWidth="8em" Width="20em"></cc1:DropDownListEx>
    	        <br />
    	        <cc1:DropDownListEx id="cboJobName" runat="server" LabelText="Job Name" LabelWidth="8em" Width="20em"></cc1:DropDownListEx>
		        <!--
		        <div style="position:absolute; right:10px;">
			        <input type="button" value="About" title="View technical information about your browser and the Abacus Web software." onclick="FetchAbout();" />
			        <input type="button" value="Log out" title="Log out of Abacus Web" onclick="if(window.confirm('Are you sure you wish to log out?')) document.location.href='../Security/Logout.aspx';" />
		        </div>
		        -->    
   	        
    	    </fieldset>
    	    <img src="../../Images/Jobs/refresh.png" style="float:left;margin-left:1em;cursor:pointer;" alt="Refresh the list of jobs" onclick="RefreshJobList()" />
    	    <div class="clearer"></div>
	        <table class="listTable sortable" id="JobList_Table" cellpadding="4" cellspacing="0" width="100%" summary="Lists the available jobs.">
	        <caption>Lists the jobs in the queue.</caption>
	        <thead>
		        <tr>
			        <th>Name</th>
			        <th>Status</th>
			        <th>Created By</th>
			        <th>Scheduled Start</th>
			        <th>Start</th>
			        <th>End</th>
			        <th>&nbsp;</th>
		        </tr>
	        </thead>
	        <tbody><tr><td></td></tr></tbody>
	        </table>
	        <div id="JobList_PagingLinks" style="float:left;"></div>
	        <div class="clearer"></div>
	    </div>
    	
	    <!-- JobStepList view containers -->
	    <div id="JobStepList_PageTitle" class="hidden">Job Steps</div>
	    <div id="JobStepList_PageOverview" class="hidden">Displayed below are the details for the different steps that make up the selected job.</div>
	    <div id="JobStepList_Content" class="hidden">
		    <img src="../../Images/Jobs/back.png" style="cursor:pointer;" alt="Jump back to view the job list" onclick="ChangePageView('JobList');RefreshJobList();" />
		    <img src="../../Images/Jobs/refresh.png" style="margin-left:1em;cursor:pointer;" alt="Refresh the list of steps" onclick="RefreshJobStepList()" />
		    <br /><br />
		    <div id="JobStepList_JobDetails" class="bold"></div>
		    <br />
		    <table class="listTable sortable" id="JobStepList_Table" cellpadding="2" cellspacing="0" width="100%" summary="Lists the available job steps.">
		    <caption>Lists the steps in the selected job.</caption>
		    <thead>
			    <tr>
				    <th>Step</th>
				    <th>Type</th>
				    <th>Status</th>
				    <th>Start</th>
				    <th>End</th>
				    <th>&nbsp;</th>
			    </tr>
		    </thead>
		    <tbody><tr><td></td></tr></tbody>
		    </table>
	    </div>
    	
	    <!-- JobStepXml view containers -->
	    <div id="JobStepXml_PageTitle" class="hidden"></div>
	    <div id="JobStepXml_PageOverview" class="hidden"></div>
	    <div id="JobStepXml_Content" class="hidden">
		    <img src="../../Images/Jobs/back.png" style="cursor:pointer;" alt="Jump back to view the job step list" onclick="ChangePageView('JobStepList');RefreshJobStepList();" />
		    <img src="../../Images/Jobs/refresh.png" style="margin-left:1em;cursor:pointer;" alt="Refresh the information" onclick="RefreshJobStepXml()" />
		    <img src="../../Images/Jobs/copy.png" style="margin-left:1em;cursor:pointer;" alt="Copy the job step details displayed below to the clipboard" onclick="CopyJobStepDetails()" />
		    <img src="../../Images/Jobs/prevstep.png" style="margin-left:1em;cursor:pointer;" alt="View the same details of the previous step" onclick="PrevJobStepXml()" />
		    <img src="../../Images/Jobs/nextstep.png" style="cursor:pointer;" alt="View the same details of the next step" onclick="NextJobStepXml()" />
		    <br /><br />
		    <div id="JobStepXml_JobStepDetails" class="bold"></div>
		    <br />
    		
            <ajaxToolkit:TabContainer runat="server" ID="TabStrip" OnClientActiveTabChanged="ActiveTabChanged">
                <ajaxToolkit:TabPanel runat="server" ID="TabPanel1" HeaderText="Inputs">
                    <ContentTemplate></ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="TabPanel2" HeaderText="Progress">
                    <ContentTemplate></ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="TabPanel3" HeaderText="Results">
                    <ContentTemplate></ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
	    </div>
    	
	    <!-- About containers -->
	    <div id="About_PageTitle" class="hidden">About</div>
	    <div id="About_PageOverview" class="hidden">Displayed below is technical information about your browser and the Abacus Web software.</div>
	    <div id="About_Content" class="hidden">
		    <img src="../../Images/Jobs/back.png" style="cursor:pointer;" alt="Jump back to view the job list" onclick="ChangePageView('JobList');RefreshJobList();" />
		    <img src="../../Images/Jobs/refresh.png" style="margin-left:1em;cursor:pointer;" alt="Refresh the information" onclick="FetchAbout()" />
            <img src="../../Images/Jobs/copy.png" style="margin-left:1em;cursor:pointer;" alt="Copy the information displayed below to the clipboard" onclick="CopyAboutDetails()" />
		    <br /><br />
		    <div id="About_Details">
    		
		    </div>
	    </div>
	    
    </asp:Content>