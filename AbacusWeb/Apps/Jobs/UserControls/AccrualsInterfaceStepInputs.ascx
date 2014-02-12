<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="AccrualsInterfaceStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.AccrualsInterfaceStepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

To submit a job that will calculate and report accruals information, set the required fields below and click the "Create New Job" button.
<br />
<cc1:TextBoxEx ID="dteAccrualDate" runat="server" LabelText="Accrual Date" LabelWidth="11em" Format="DateFormat"
    Required="true" RequiredValidatorErrMsg="A valid date must be provided" ValidationGroup="Save"></cc1:TextBoxEx>
<br />
<cc1:DropDownListEx ID="cboFinancialYear" runat="server" LabelText="Financial Year" LabelWidth="11em"
    Required="true" RequiredValidatorErrMsg="Please select a financial year" ValidationGroup="Save"></cc1:DropDownListEx>
<br />
<cc1:DropDownListEx ID="cboPeriodNum" runat="server" LabelText="Period Number" LabelWidth="11em"></cc1:DropDownListEx>
<br />
<cc1:TextBoxEx id="txtFilePath" runat="server" LabelText="File Path" LabelWidth="11em" Width="40em"></cc1:TextBoxEx>
<input type="hidden" id="hidCreatingJob" runat="server" value="1" />
