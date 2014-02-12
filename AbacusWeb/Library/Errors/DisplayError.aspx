<%@ Page Language="vb" AutoEventWireup="false" Codebehind="DisplayError.aspx.vb" Inherits="Target.Abacus.Web.Library.Errors.DisplayError" EnableViewState="False" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview">
		An unexpected error has occurred during the operation of this application. 
		The details are shown below.
		<br /><br />
	</asp:Content>
    <asp:Content ContentPlaceHolderID="MPContent">
	    <table cellspacing="10" cellpadding="0">
	    <tr>
		    <td valign="top"><strong>Error</strong></td>
		    <td valign="top">
			    <asp:Literal id="litError" runat="server"></asp:Literal>
		    </td>
	    </tr>
	    <tr>
		    <td valign="top"><strong>Date/Time</strong></td>
		    <td valign="top">
			    <asp:Literal id="litDateTime" runat="server"></asp:Literal>
		    </td>
	    </tr>
	    <tr>
		    <td valign="top"><strong>Url</strong></td>
		    <td valign="top">
			    <asp:Literal id="litUrl" runat="server"></asp:Literal>
		    </td>
	    </tr>
	    </table>

	    <p>
		    <asp:Label id="lblReportSent" runat="server" Visible="False" CssClass="warningText">Thank you. Your error report has been sent.</asp:Label>
	    </p>
	    <asp:panel id="pnlSubmitReport" runat="Server" Visible="False">
		    <h3>Submit Error Report</h3>
		    <strong>Please describe the problem and what you were doing at the time</strong>
		    <br />
		    <asp:TextBox id="txtUserComments" runat="server" TextMode="MultiLine" Rows="5" Width="80%"></asp:TextBox>
		    <br />
		    <asp:Button id="btnSubmitReport" runat="server" Text="Submit Report"></asp:Button>
	    </asp:panel>
	    <asp:panel id="pnlDetails" runat="Server" Visible="False">
		    <h3>Error Details</h3>
		    <table cellspacing="10" cellpadding="0">
			    <tr>
				    <td valign="top" nowrap><strong>System Exception</strong></td>
				    <td valign="top">
					    <asp:literal id="litsystemexception" runat="server"></asp:literal></td>
			    </tr>
			    <tr>
				    <td valign="top" nowrap><strong>System Message</strong></td>
				    <td valign="top">
					    <asp:literal id="litsystemmessage" runat="server"></asp:literal></td>
			    </tr>
			    <tr>
				    <td valign="top" nowrap><strong>Source</strong></td>
				    <td valign="top">
					    <asp:literal id="litsource" runat="server"></asp:literal></td>
			    </tr>
			    <tr>
				    <td valign="top" nowrap><strong>Target</strong></td>
				    <td valign="top">
					    <asp:literal id="littarget" runat="server"></asp:literal></td>
			    </tr>
			    <tr>
				    <td valign="top" nowrap><strong>Inner Exceptions</strong></td>
				    <td valign="top">
					    <asp:literal id="litinnerexcpetions" runat="server"></asp:literal></td>
			    </tr>
			    <tr>
				    <td valign="top" nowrap><strong>Stack Trace</strong></td>
				    <td valign="top">
					    <asp:literal id="litstacktrace" runat="server"></asp:literal></td>
			    </tr>
			    <tr>
				    <td valign="top" nowrap><strong>Extra Information</strong></td>
				    <td valign="top">
					    <asp:literal id="litextrainfo" runat="server"></asp:literal></td>
			    </tr>
		    </table>
	    </asp:panel>
	</asp:Content>