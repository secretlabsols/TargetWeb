<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ManageLogins.aspx.vb" Inherits="Target.Web.Apps.Security.Admin.ManageLogins" %>

    <asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		The list below displays all of the currently logged in users. To forcibly log a user out, click the "Logout" link next to the relevant user.
		To forcibly log all users out (except yourself), click on the "Logout All" button.
	</asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <asp:Panel id="pnlNotActive" runat="server" visible="false">
	        <span class="warningText">
	            The "Prevent the same login being used simultaneously from more than one computer" option on the 
	            <a href="AccountPolicy.aspx?mode=1">account policy screen</a> is not active. Therefore, no users will be
	            listed below.
	        </span>
	        <br /><br />
	    </asp:Panel>
	    <asp:Button id="btnLogoutAll" runat="server" Text="Logout All"></asp:Button>
	    <input type="button" style="float:left;" value="Refresh" onclick="document.location.href='ManageLogins.aspx'" />
	    <br /><br />
	    <asp:Repeater id="rptUsers" runat="server">
		    <HeaderTemplate>
			    <table class="listTable sortable" cellpadding="4" cellspacing="0" width="100%" summary="List of currently logged in users.">
				    <caption>List of currently logged in users.</caption>
				    <tr>
					    <th valign="top">User</th>
					    <th valign="top">Username</th>
					    <th valign="top">Logged In Date/Time</th>
					    <th valign="top">Logged In Location</th>
					    <th valign="top">Idle For</th>
					    <th valign="top"></th>
				    </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td valign="top"><%#Container.DataItem.FirstName%> <%#Container.DataItem.Surname%>&nbsp;</td>
				    <td valign="top"><%#Container.DataItem.Email%>&nbsp;</td>
				    <td valign="top"><%#Container.DataItem.LastLoginDate%>&nbsp;</td>
				    <td valign="top"><%#GetLocation(Container.DataItem.LastLoginIPAddress)%>&nbsp;</td>
				    <td valign="top"><%#GetIdleTime(Container.DataItem.LastRequestDate)%>&nbsp;</td>
				    <td valign="top" style="text-align:right;">
				        <asp:LinkButton id="lnkLogout" runat="server"
				            CommandName="logout" 
					        CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>'
					        Visible='<%# (DataBinder.Eval(Container.DataItem, "ID") <> CurrentUserID) %>'
					        OnClientClick="return window.confirm('Are you sure you wish to log this user out?');">[Logout]</asp:LinkButton>&nbsp;
                    </td>
			    </tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
	    </asp:Repeater>
    	
	    <div class="clearer"></div>
    </asp:Content>