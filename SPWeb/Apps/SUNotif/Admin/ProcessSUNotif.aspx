<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ProcessSUNotif.aspx.vb" Inherits="Target.SP.Web.Apps.SUNotif.Admin.ProcessSUNotif" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details of the submitted service user notification.  
		<p>
		You should use these details to enable you to correctly carry out the necessary steps to process the notification 
		manually in the Supporting People EXE application.  These steps are <strong>NOT</strong> automatically carried out 
		by this screen.
		</p>
		Once you have performed the necessary steps to accept or decline the notification, you should choose the appropriate 
		outcome by clicking	on the "Accept" or "Decline" buttons below.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <fieldset class="group">
		    <legend>Notification Details</legend>
		    <cc1:TextBoxEx id="txtReference" runat="server" LabelText="Reference" LabelWidth="10em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtType" runat="server" LabelText="Type" LabelWidth="10em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtCreated" runat="server" LabelText="Created" LabelWidth="10em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtSubmitted" runat="server" LabelText="Submitted" LabelWidth="10em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtRequestedBy" runat="server" LabelText="Requested By" LabelWidth="10em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
	    </fieldset>
    	
	    <fieldset class="group" id="grpNew" runat="server">
		    <legend>New Service User Notification</legend>
		    <cc1:TextBoxEx id="txtNewPrimaryName" runat="server" LabelText="Primary Service User" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtNewPrimaryNINO" runat="server" LabelText="National Insurance No" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtNewPrimaryBirthDate" runat="server" LabelText="Date of Birth" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br /><br />
		    <cc1:TextBoxEx id="txtNewSecondaryName" runat="server" LabelText="Secondary Service User" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtNewSecondaryNINO" runat="server" LabelText="National Insurance No" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtNewSecondaryBirthDate" runat="server" LabelText="Date of Birth" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br /><br />
		    <cc1:TextBoxEx id="txtNewAddress" runat="server" LabelText="Address" LabelWidth="14em" IsReadOnly="True" LabelBold="True" ReadOnlyContentCssClass="content"></cc1:TextBoxEx>
		    <div class="clearer"></div><br />
		    <cc1:TextBoxEx id="txtNewProvider" runat="server" LabelText="Provider" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtNewService" runat="server" LabelText="Service" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtExpectedStartDate" runat="server" LabelText="Expected Start Date" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtAgreement" runat="server" LabelText="Tenancy/Support Agreement?" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br /><br />
		    <cc1:TextBoxEx id="txtYourReference" runat="server" LabelText="Your Reference" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtServiceLevel" runat="server" LabelText="Service Level" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtUnitCost" runat="server" LabelText="Unit Cost" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <input type="button" id="btnViewNotif" title="Click here to view the submitted notification document." style="float:right;" value="View Notification" onclick="window.open('../ViewSUNotif.aspx?suNotifID=' + GetQSParam(document.location.search, 'suNotifID'));" />
	    </fieldset>
    	
	    <fieldset class="group" id="grpEnd" runat="server">
		    <legend>End Subsidy Notification</legend>
		    <cc1:TextBoxEx id="txtEndProvider" runat="server" LabelText="Provider" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtEndService" runat="server" LabelText="Service" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtEndPrimaryName" runat="server" LabelText="Primary Service User" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtEndSecondaryName" runat="server" LabelText="Secondary Service User" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtEndDateFrom" runat="server" Format="DateFormat" LabelText="Date From" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtEndSubsidy" runat="server" Format="CurrencyFormat" LabelText="Subsidy" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtEndDateTo" runat="server" Format="DateFormat" LabelText="End Date" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <cc1:TextBoxEx id="txtEndReason" runat="server" LabelText="End Reason" LabelWidth="14em" IsReadOnly="True" LabelBold="True"></cc1:TextBoxEx>
		    <br />
	    </fieldset>
    	
	    <fieldset class="group">
		    <legend>Outcome</legend>
		    <cc1:TextBoxEx id="txtComment" runat="server" LabelText="Comment" LabelWidth="8em" Width="30em" LabelBold="True"></cc1:TextBoxEx>
		    <br />
		    <div style="float:left;">
			    <input type="hidden" id="hidDecision" runat="server" />
			    <input type="button" id="btnAccept" title="Click here to accept the notification." value="Accept" onclick="btnAccept_OnClick('True');" />
			    <input type="button" id="btnDecline" title="Click here to decline the notification." value="Decline" onclick="btnDecline_OnClick();" />
			    <input type="button" id="btnCancel" title="Click here to return to the previous screen without making any changes." 
				    value="Cancel" onclick="history.go(-1);" />
		    </div>
	    </fieldset>
    </asp:Content>