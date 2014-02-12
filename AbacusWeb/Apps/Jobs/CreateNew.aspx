<%@ Page Language="vb" AutoEventWireup="false" Codebehind="CreateNew.aspx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.CreateNew" EnableViewState="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

    <asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
        To create a new job and submit it for processing, enter the details below and click on the "Create New Job" button.
    </asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    	    
	    <fieldset id="fsJobType" runat="server" style="margin-bottom:1em;">
	        <legend>Create a New Job</legend>
	        <cc1:DropDownListEx id="cboJobType" runat="server" LabelText="Job Type" LabelWidth="12em"
	            Required="true" RequiredValidatorErrMsg="Please select a job type"></cc1:DropDownListEx>
            <br />
	    </fieldset>
	    <fieldset id="fsInputs" visible="false" runat="server" enableviewstate="false">
	        <legend>Job Inputs</legend>
	        <cc1:TextBoxEx id="dteStartDate"  runat="server" Format="DateFormatJquery" AllowClear="true" LabelText="Scheduled Start Date" LabelWidth="10.5em" 
                Required="True" RequiredValidatorErrMsg="Please enter the date to start the job"></cc1:TextBoxEx>
            <br />
            <cc1:TimePicker id="tmeStartTime" runat="server" LabelText="Scheduled Start Time" LabelWidth="12em"></cc1:TimePicker>
            <br /><br />
            <cc1:TextBoxEx id="txtComment" runat="server" LabelText="Comment" LabelWidth="12em" Width="35em" MaxLength="255"></cc1:TextBoxEx>
            <br />
            <hr />
            <br />
	    </fieldset>
        <br />
	    <asp:Button id="btnCreate" runat="server" Text="Create New Job" Visible="false" />
	    
    </asp:Content>