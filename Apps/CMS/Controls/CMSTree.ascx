<%@ Control Language="vb" AutoEventWireup="false" Codebehind="CMSTree.ascx.vb" Inherits="Target.Web.Apps.CMS.Controls.CMSTree" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
	<script type="text/javascript">
		//<![CDATA[
		var parentWindow = GetParentWindow();
		function CleanURL(url) {
			url = RemoveQSParam(url, "showPageID");
			url = RemoveQSParam(url, "showFolderID");
			url = RemoveQSParam(url, "movePageID");
			url = RemoveQSParam(url, "moveFolderID");
			url = RemoveQSParam(url, "moveTargetID");
			return url;
		}
        function NewFolder() {
            var node = TreeView_GetSelectedNode(TreeView1ID);
            var newNode;
            var text = "New Folder";
            if(node != null) {
				if(TreeView_GetTag(TreeView1ID, node.id) == "FOLDER")
					TreeView_AddChildNode(TreeView1ID, node.id, "Please enter the name of the new folder.", "New Folder");
				else
					alert("Please select a folder.");
			} else {
				TreeView_AddRootNode(TreeView1ID, "Please enter the name of the new folder.", "New Folder");
            }
        }
        function Move() {
			var node = TreeView_GetSelectedNode(TreeView1ID);
			if(node != null) { 
				OpenDialog("ModalDialogWrapper.axd?../Admin/SelectItem.aspx?treeMode=<%= Target.Web.Apps.CMS.Controls.WebCMSTreeMode.CMSFolderSelect %>", 26.35, 40.70, window);
			} else { 
				alert("Please select a page/folder to move.");
			}
        }
        function CMSTree_ItemSelected(targetFolderID) {
			var url = CleanURL(document.location.href);
			var node = TreeView_GetSelectedNode(TreeView1ID);
			var nodeDataKey = TreeView_GetDataKey(TreeView1ID, node.id);
			if(TreeView_GetTag(TreeView1ID, node.id) == "FOLDER") {
				url = AddQSParam(url, "showFolderID", nodeDataKey);
				url = AddQSParam(url, "moveFolderID", nodeDataKey);
			} else {
				url = AddQSParam(url, "showPageID", nodeDataKey);
				url = AddQSParam(url, "movePageID", nodeDataKey);
			}
			document.location.href = AddQSParam(url, "moveTargetID", targetFolderID);
        }
        function RemoveNode() {
			var node = TreeView_GetSelectedNode(TreeView1ID);
			if(node == null) {
				alert("Please select a page/folder to delete.");
			} else {
				if(window.confirm("Are you sure you wish to delete this page/folder?")) {
					GetElement("aspnetForm").action = CleanURL(GetElement("aspnetForm").action);
					TreeView_RemoveNode(TreeView1ID, node.id);
				}
			}
		}
        function Refresh() {
			document.location.href = CleanURL(document.location.href);
        }
        function Select() {
			var node = TreeView_GetSelectedNode(TreeView1ID);
			if(node) { 
				<% If Me.TreeMode = Target.Web.Apps.CMS.Controls.WebCMSTreeMode.CMSPageSelect Then %>
					if(TreeView_GetTag(TreeView1ID, node.id) != "PAGE") {
						alert("Please select a page.");
						return;
					}
				<% End If %>
				<% If Me.TreeMode = Target.Web.Apps.CMS.Controls.WebCMSTreeMode.CMSFolderSelect Then %>
					if(TreeView_GetTag(TreeView1ID, node.id) != "FOLDER") {
						alert("Please select a folder.");
						return;
					}
				<% End If %>
				parentWindow.HideModalDIV();
				parentWindow.CMSTree_ItemSelected(TreeView_GetDataKey(TreeView1ID, node.id));
				window.parent.close();
			} else {
				alert("Please select a page/folder.");
			}
        }
        function Cancel() {
			parentWindow.HideModalDIV();
			window.parent.close();
        }
        function TreeView1_NodeClicked(nodeID) {
			TreeView_SelectNode(TreeView1ID, nodeID);
		}
		function TreeView1_NodeDblClicked(nodeID) {
			<% If Not Me.AllowEdit Then Response.Write("return true;") %>
			var node = TreeView_GetSelectedNode(TreeView1ID);
			if(TreeView_GetTag(TreeView1ID, node.id) == "FOLDER")
				TreeView_EditNodeText(TreeView1ID, nodeID, "Please enter the new folder name.");
		}
		addEvent(window, "unload", DialogUnload);
        //]]>
	</script>
	<asp:label id="lblErrorMsg" CssClass="errorText" runat="server"></asp:label>
	
	<asp:Literal ID="litTitle" Runat="server"></asp:Literal>
	
	<div style="width:23.26em;padding-left:0.38em;">
		<hr />
		<span id="spanNewFolder" runat="server">
			<a href="javascript:NewFolder()"><img alt="Create New Folder" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/CMS/Images/NewFolder.gif") %>" border="0"></a>&nbsp;
		</span>
		<span id="spanMove" runat="server">
			<a href="javascript:Move()"><img alt="Move a page or folder" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/CMS/Images/Move.gif") %>" border="0"></a>&nbsp;
		</span>
		<span id="spanDelete" runat="server">
			<a href="javascript:RemoveNode()"><img alt="Delete a page or folder" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/CMS/Images/Delete.gif") %>" border="0"></a>&nbsp;
		</span>
		<span id="spanRefresh" runat="server">
			<a href="javascript:Refresh()"><img alt="Refresh the folder tree" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/CMS/Images/Refresh.gif") %>" border="0"></a>&nbsp;
		</span>
		<span id="spanSelect" runat="server">
			<a href="javascript:Select()"><img alt="Select this page/folder" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/CMS/Images/Select.gif") %>" border="0"></a>&nbsp;
		</span>
		<span id="spanCancel" runat="server">
			<a href="javascript:Cancel()"><img alt="Close this window" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/CMS/Images/Cancel.gif") %>" border="0"></a>&nbsp;
		</span>
		<hr />
	</div>
	
	<cc1:TreeView id="TreeView1" runat="server" Width="23.26" CanClickNode="True" CanDoubleClickNode="True"></cc1:TreeView>
	
	
	