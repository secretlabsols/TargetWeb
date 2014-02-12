<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Popup.aspx.vb" Inherits="Target.Web.Apps.AuditLog.Popup" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="AuditLogSelector" Src="Controls/AuditLogSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		
		<script type="text/javascript">
			function btnClose_Click() {
				GetParentWindow().HideModalDIV();
				window.parent.close();
			}
			function btnCopy_Click () {
			    var COPY_CLIPBOARD_SUCCESS = "The information was successfully copied to the clipboard.\nYou can now paste this information into your word processor (e.g. Microsoft Word) or an email window as normal.";
			    var divDetails = GetElement("divDetails");
			    CopyToClipboardHtml(divDetails, COPY_CLIPBOARD_SUCCESS);
			}
			addEvent(window, "unload", DialogUnload);
		</script>
		
		<uc1:AuditLogSelector id="selector" runat="server"></uc1:AuditLogSelector>			
        <div id="divCommands" runat="server">
            <br />
		    <input type="button" id="btnClose" value="Close" style="float:right;margin-right:1em;" onclick="btnClose_Click();" />
		    <input type="button" id="btnPrint" value="Print" style="float:right;" onclick="window.print();" />
		    <input type="button" id="btnCopy" value="Copy" title="Copy this information to the Windows clipboard" style="float:right;" onclick="btnCopy_Click();" />
             <div class="clearer"></div>
		</div>
    </asp:Content>