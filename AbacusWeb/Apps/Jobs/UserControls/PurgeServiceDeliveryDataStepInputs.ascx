<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PurgeServiceDeliveryDataStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.PurgeServiceDeliveryDataStepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

To submit a job that will delete spent service delivery data amendments, click the "Create New Job" button.
<br /><br />
Data for service delivery amendments that were processed before the "cut-off" date specified below will be deleted. A date 
must be specified before any action will be taken.
<br /><br />
<span class="warningText">IMPORTANT:</span>
<br /><br />
<span class="warningText">Once the job data has been deleted it cannot be recovered.</span>
<br /><br />
<span class="warningText">This job can affect large amounts of data, take a long time to complete
and can adversely affect database server performance during processing. Therefore it is recommended that this job is scheduled to run at a time 
of low user activity.</span>
<br /><br />
<span class="warningText">This job can free up large amounts of space from the database. Therefore it is recommended 
that the database space requirements are reviewed after this job has completed.</span>
<br /><br />
<cc1:TextBoxEx ID="dteCutOffDate" runat="server" LabelText="Cut-off Date" LabelWidth="10em" Format="DateFormat"
    Required="true" RequiredValidatorErrMsg="A valid date must be provided"></cc1:TextBoxEx>
<asp:RangeValidator ID="valFutureDates" ControlToValidate="dteCutOffDate$txtTextBox" runat="server"></asp:RangeValidator>
<input type="hidden" id="hidCreatingJob" runat="server" value="1" />

