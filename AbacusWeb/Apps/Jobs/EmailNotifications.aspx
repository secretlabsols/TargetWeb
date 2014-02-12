<%@ Page Language="vb" AutoEventWireup="false" Codebehind="EmailNotifications.aspx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.EmailNotifications" 
   ValidateRequest="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

    <asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
        This screen allows you to manage email notification settings for the different available Abacus Job Service jobs.
    </asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
	    <fieldset id="fsControls" runat="server" style="padding:0.5em;">
            
            <cc1:TextBoxEx ID="txtName" runat="server"
                LabelText="Name"
                LabelWidth="8em"
                LabelBold="true"
                IsReadOnly="true"
                ReadOnlyContentCssClass="disabled"
                EnableViewState="false">
            </cc1:TextBoxEx>
            <br />
            <cc1:TextBoxEx ID="txtDesc" runat="server"
                LabelText="Description"
                LabelWidth="8em"
                LabelBold="true"
                IsReadOnly="true"
                ReadOnlyContentCssClass="disabled"
                EnableViewState="false">
            </cc1:TextBoxEx>
            <br /><br />
            
			<table class="listTable" cellspacing="0" cellpadding="2" summary="List of email addresses that will receive notifications" width="100%">
            <caption>List of email addresses that will receive notifications</caption>
            <thead>
                <tr>
                    <td class="headerGroup" style="border-width:0px;">&nbsp;</td>
                    <td colspan="4" class="headerGroup headerStatus" style="border-width:0px;">Job Status</td>
                    <td class="headerGroup" style="border-width:0px;">&nbsp;</td>
                </tr>
	            <tr>
		            <th style="width:50%;">Email Address</th>
		            <th>Success</th>
		            <th>Warnings</th>
		            <th>Exceptions</th>
		            <th>Failed</th>
		            <th>&nbsp;</th>
	            </tr>
            </thead>
            <tbody>
				<asp:PlaceHolder ID="phNotifs" runat="server"></asp:PlaceHolder>
            </tbody>
			</table>
			<div><asp:Button id="btnAdd" runat="server" Text="Add" /></div>
			<br />
			<span style="font-style:italic;font-size:0.85em;">
			    To send notifications to the user who created the job, enter 
			    <strong><%= Target.Abacus.Library.Core.JobServiceBL.EMAIL_NOTIFICATION_JOB_CREATOR_ADDRESS %></strong> as the Email Address.
			    <br /><br />
			    NOTE: An email can only be sent to the user who created the job when all of the following conditions are met:
			    <li>the job was not created via the "FileWatcher" functionality</li>
			    <li>the job was not created via a recurring schedule</li>
			    <li>for jobs created through Abacus Intranet, the Abacus user account must have an email address recorded against it</li>
			    
            </span>
        </fieldset>
            
    </asp:Content>