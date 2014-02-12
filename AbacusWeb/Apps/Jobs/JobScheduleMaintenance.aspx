<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" Codebehind="JobScheduleMaintenance.aspx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.JobScheduleMaintenance" EnableViewState="true" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="RecPattern" Src="~/AbacusWeb/Apps/UserControls/RecurrencePattern.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

    <asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
        Maintain details of job schedules.
    </asp:Content>
    <asp:Content ID="Content1" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
	    
	    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
	        
            <cc1:DropDownListEx id="cboJobType" runat="server"  LabelText="Job Type" LabelWidth="12em"
                    Required="true"  RequiredValidatorErrMsg="Please select a job type" ValidationGroup="Save"></cc1:DropDownListEx>
            <br />
            <cc1:TextBoxEx id="txtDescription" runat="server" LabelText="Description" 
                    Required="true" RequiredValidatorErrMsg="Please select a Description"
                    LabelWidth="12em" Width="35em" MaxLength="255" ValidationGroup="Save"></cc1:TextBoxEx>
            <br />
            <cc1:CheckBoxEx ID="chkEnabled" runat="server" Text="Enabled" LabelWidth="11.75em"></cc1:CheckBoxEx>
            <br /><br />
            
            <uc2:RecPattern id="recPattern" runat="Server"></uc2:RecPattern>
       </fieldset>
       
    </asp:Content>
