<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ucCreateJobPointInTime.ascx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.ucCreateJobPointInTime" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>

<div style="width : 100%;">
    <asp:radiobutton id="optCreateJobNow" groupname="grpCreateJobWhen" TextAlign="left" height="2em" width="100%" 
        runat="server" text="Create job now" checked="True" onclick="javascript:optCreate_Click();" ToolTip="Create the job immediately" />
        
    <asp:radiobutton id="optCreateJobLater" groupname="grpCreateJobWhen" TextAlign="left" height="2em" width="100%" 
        runat="server" text="Defer creation of job" onclick="javascript:optCreate_Click();" ToolTip="Defer creation of the job until later" />
        
    <div style="float:left;" runat="server" id="divCreateJobLater">
        <div style="float:left;">
            <cc1:TextBoxEx ID="dteStartDate" runat="server" LabelText="Start Date/Time" LabelWidth="17em"
            Required="true" RequiredValidatorErrMsg="Please enter a valid start date" Format="DateFormat"
            ValidationGroup="Save" />        
        </div>
        <div style="float:left;">
            <cc1:TimePicker ID="tmeStartDate" runat="server" ShowSeconds="False" LabelText="&nbsp;" />        
        </div>
        <div class="clearer" />  
        <asp:RangeValidator ID="valDates" ControlToValidate="dteStartDate$txtTextBox" runat="server" ValidationGroup="Save" />      
    </div>
</div>
<div class="clearer"></div>
<asp:HiddenField ID="hidDisplayMode" runat="server" Value="2" />


 