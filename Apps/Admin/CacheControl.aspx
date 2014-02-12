<%@ Page Language="vb" AutoEventWireup="false" Codebehind="CacheControl.aspx.vb" Inherits="Target.Web.Apps.Admin.CacheControl" EnableViewState="False" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	    Below is a list of all of the objects in the system cache. Click on the name of an item to remove it from the 
	    cache or click 'Remove All' to clear the cache completely.
    </asp:Content>
    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <asp:Button id="btnRemoveAll" runat="server" Text="Remove All"></asp:Button>
	    <input type="button" style="float:left;" value="Refresh" onclick="document.location.href='CacheControl.aspx'" />
	    <br /><br />
	    <asp:Repeater id="rptCacheItems" runat="server">
		    <HeaderTemplate>
			    <table class="listTable sortable" style="table-layout:fixed;" cellpadding="4" cellspacing="0" width="100%" summary="List of currently cached items.">
				    <caption>List of currently cached items.</caption>
				    <tr>
					    <th>Name</th>
					    <th valign="top">Data Type</th>
				    </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td valign="top"><a href='javascript:if(window.confirm("Are you sure you wish to remove this item from the cache?")) document.location.href="CacheControl.aspx?remove=<%# Container.DataItem.Key %>";'><%# Container.DataItem.Key %></a>&nbsp;</td>
				    <td valign="top"><%# Container.DataItem.Value %>&nbsp;</td>
			    </tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
	    </asp:Repeater>
    	
	    <div class="clearer"></div>
    </asp:Content>