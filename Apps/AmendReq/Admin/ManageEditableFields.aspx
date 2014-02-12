<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ManageEditableFields.aspx.vb" Inherits="Target.Web.Apps.AmendReq.Admin.ManageEditableFields"
    EnableViewstate="false" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		The list below displays the editable field settings for each external user account that has access to this site.
		To change the default settings for all users who do not have any custom settings defined, click on the "Edit Default Settings" button.
		To change the settings for an individual external user account, click on the relevant option in the list below.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server"></asp:Content>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" value="Edit Default Settings" style="float:left;" title="Click here to edit the default settings." onclick="btnEdit_OnClick(0, '');" />
    		
	    <asp:Repeater id="rptExternalUsers" runat="server">
		    <HeaderTemplate>
			    <table class="listTable" id="tblExternalUsers" cellpadding="4" cellspacing="0" width="100%" summary="List of external user accounts.">
				    <caption style="margin-top:1em;">List of external user accounts.</caption>
				    <tr>
					    <th style="width:50%;">Full Name</th>
					    <th style="width:25%;">Username</th>
					    <th style="width:25%;">Settings</th>
				    </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td><%# GetName(Container.DataItem.ExternalFullname) %></td>
				    <td><%# GetName(Container.DataItem.ExternalUsername) %></td>
				    <td>
					    <input type="hidden" id="hidDefaultSettings<%# Container.DataItem.ExternalUserID %>" value="<%# Container.DataItem.DefaultSettings %>" />
					    <input type="radio" id="rdoDefault<%# Container.DataItem.ExternalUserID %>" name="setting<%# Container.DataItem.ExternalUserID %>" 
						    style="float:left;" title="Select to use the default settings for this user." 
						    value="1" onclick="rdoSetting_OnClick(<%# Container.DataItem.ExternalUserID %>, '<%# Container.DataItem.ExternalFullname %>', true);" />
					    <label for="rdoDefault<%# Container.DataItem.ExternalUserID %>" title="Select to use the default settings for this user.">Default</label>
					    <span style="float:left;">&nbsp;</span>
					    <input type="radio" id="rdoCustom<%# Container.DataItem.ExternalUserID %>" name="setting<%# Container.DataItem.ExternalUserID %>" 
						    style="float:left;" title="Select to use custom settings for this user." 
						    value="0" onclick="rdoSetting_OnClick(<%# Container.DataItem.ExternalUserID %>, '<%# Container.DataItem.ExternalFullname %>', false);" />
					    <label for="rdoCustom<%# Container.DataItem.ExternalUserID %>" title="Select to use custom settings for this user.">Custom</label>
					    <span style="float:left;">&nbsp;</span>
					    <input type="button" id="btnEdit<%# Container.DataItem.ExternalUserID %>" disabled="disabled" value="Edit" 
						    title="Click here to edit the custom settings for this user." onclick="btnEdit_OnClick(<%# Container.DataItem.ExternalUserID %>, '<%# Container.DataItem.ExternalFullname %>');" />
				    </td>
			    </tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
	    </asp:Repeater>
    	
	    <div class="clearer"></div>
    </asp:Content>