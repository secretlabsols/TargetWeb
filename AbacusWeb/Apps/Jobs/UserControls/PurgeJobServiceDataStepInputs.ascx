<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PurgeJobServiceDataStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.PurgeJobServiceDataStepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

To submit a job that will delete old job service data, set the required filters below and click the "Create New Job" button.
<br /><br />
Data for jobs that completed before the specified "Completion Date" with the specified status value(s) will be deleted. However, 
note that some types of job are explicitly excluded from the purge process, e.g. financial export interface jobs. As such these 
jobs and their data will be unaffected by this process.
<br /><br />
The filters you set will be applied at the time that the job runs to gather the job service data for processing.
<br />
<br />
<span class="warningText">IMPORTANT: Once the job data has been deleted it cannot be recovered.</span>
<br />
<br />
<span class="warningText">IMPORTANT: This job can affect large amounts of data, take a long time to complete
and can adversely affect database server performance during processing. Therefore it is recommended that this job is scheduled to run at a time 
of low user activity.</span>
<br />
<br />
<span class="warningText">IMPORTANT: This job can free up large amounts of space from the database. Therefore it is recommended 
that the database space requirements are reviewed after this job has completed.</span>
<br /><br />

<asp:PlaceHolder ID="phJobstatus" runat="server"></asp:PlaceHolder>
<br />
<cc1:TextBoxEx ID="dteCompletionDate" runat="server" LabelText="Completion Date" LabelWidth="10em" Format="DateFormat"
    Required="true" RequiredValidatorErrMsg="Please enter a completion date"></cc1:TextBoxEx>
<input type="hidden" id="hidCreatingJob" runat="server" value="1" />
