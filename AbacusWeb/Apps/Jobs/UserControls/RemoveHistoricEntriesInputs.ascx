<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RemoveHistoricEntriesInputs.ascx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.RemoveHistoricEntriesInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

To submit a job that will delete entries from the external client, external client address tables, and external client exception work tray.
<br />
The 'deletion before date' you set will be applied at the time that the job runs to gather the required data for deletion.

<br /><br />
<cc1:TextBoxEx ID="dteDeleteBeforeDate" runat="server" LabelText="Delete external clients imported on or before" LabelWidth="24em" Format="DateFormatJquery"  
Width="6em" AllowClear="true" required="true" RequiredValidatorErrMsg="Please enter an Effective From date" ValidationGroup="Save"/>
<br />
<br />
