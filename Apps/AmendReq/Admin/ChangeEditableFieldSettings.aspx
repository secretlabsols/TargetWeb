<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ChangeEditableFieldSettings.aspx.vb" Inherits="Target.Web.Apps.AmendReq.Admin.ChangeEditableFieldSettings"
    EnableViewstate="false" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		<asp:Literal id="litPageOverview" runat="server"></asp:Literal>
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server"></asp:Content>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" value="Back" style="float:left;" title="Click here to return to the previous screen." onclick="document.location.href='ManageEditableFields.aspx';" />
    		
	    <asp:Repeater id="rptSettings" runat="server">
		    <HeaderTemplate>
			    <table class="listTable" id="tblSettings" cellpadding="4" cellspacing="0" width="100%" summary="List of settings.">
				    <caption style="margin-top:1em;">List of settings.</caption>
				    <tr>
					    <th>Area</th>
					    <th>Name</th>
					    <th>Setting</th>
				    </tr>
		    </HeaderTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
	    </asp:Repeater>
    	
	    <div class="clearer"></div>
    	
	    <asp:Button id="btnSave" title="Click here to save the settings." runat="server" text="Save"></asp:Button>
	    <input type="button" value="Cancel" title="Click here to return to the previous screen without making any changes." onclick="document.location.href='ManageEditableFields.aspx'" />
    </asp:Content>