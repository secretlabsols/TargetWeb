<%@ Control Language="vb" AutoEventWireup="false" Codebehind="FileStoreTree.ascx.vb" Inherits="Target.Web.Apps.FileStore.Controls.FileStoreTree" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
	<script type="text/javascript">
		//<![CDATA[
		var parentWindow = GetParentWindow();
		function CleanURL(url) {
			url = RemoveQSParam(url, "showFileID");
			url = RemoveQSParam(url, "showFolderID");
			url = RemoveQSParam(url, "moveFileID");
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
        function NewFile() {
            var node = TreeView_GetSelectedNode(TreeView1ID);
            var folderID;
            if(node != null) {
				if(TreeView_GetTag(TreeView1ID, node.id) == "FOLDER") {
					folderID = TreeView_GetDataKey(TreeView1ID, node.id);
				} else {
					alert("Please select a folder to upload the file to.");
					return;
				}
			} else {
				folderID = -1;
			}
			OpenDialog("ModalDialogWrapper.axd?EditFile.aspx?folderID=" + folderID, 55, 35, window);
        }
        function EditFileComplete(fileID) {
			document.location.href = AddQSParam(CleanURL(document.location.href), "showFileID", fileID);
        }
        function Move() {
			var node = TreeView_GetSelectedNode(TreeView1ID);
			if(node != null) { 
				OpenDialog("ModalDialogWrapper.axd?../Admin/SelectItem.aspx?treeMode=<%= Target.Web.Apps.FileStore.Controls.WebFileStoreTreeMode.FileStoreFolderSelect %>", 30, 40, window);
			} else { 
				alert("Please select a file/folder to move.");
			}
        }
        function FileStoreTree_ItemSelected(targetFolderID) {
			var url = CleanURL(document.location.href);
			var node = TreeView_GetSelectedNode(TreeView1ID);
			var nodeDataKey = TreeView_GetDataKey(TreeView1ID, node.id);
			if(TreeView_GetTag(TreeView1ID, node.id) == "FOLDER") {
				url = AddQSParam(url, "showFolderID", nodeDataKey);
				url = AddQSParam(url, "moveFolderID", nodeDataKey);
			} else {
				url = AddQSParam(url, "showFileID", nodeDataKey);
				url = AddQSParam(url, "moveFileID", nodeDataKey);
			}
			document.location.href = AddQSParam(url, "moveTargetID", targetFolderID);
        }
        function RemoveNode() {
			var node = TreeView_GetSelectedNode(TreeView1ID);
			if(node == null) {
				alert("Please select a file/folder to delete.");
			} else {
				if(window.confirm("Are you sure you wish to delete this file/folder?")) {
					GetElement("aspnetForm").action = CleanURL(GetElement("frmMain").action);
					TreeView_RemoveNode(TreeView1ID, node.id);
				}
			}
		}
        function Refresh() {
			document.location.href = CleanURL(document.location.href);
        }
        function Select() {
			var node = TreeView_GetSelectedNode(TreeView1ID);
			if(node != null) { 
				<% If Me.TreeMode = Target.Web.Apps.FileStore.Controls.WebFileStoreTreeMode.FileStoreFileSelect Then %>
					if(TreeView_GetTag(TreeView1ID, node.id) != "FILE") {
						alert("Please select a file.");
						return;
					}
				<% End If %>
				<% If Me.TreeMode = Target.Web.Apps.FileStore.Controls.WebFileStoreTreeMode.FileStoreFolderSelect Then %>
					if(TreeView_GetTag(TreeView1ID, node.id) != "FOLDER") {
						alert("Please select a folder.");
						return;
					}
				<% End If %>
				parentWindow.HideModalDIV();
				parentWindow.FileStoreTree_ItemSelected(TreeView_GetDataKey(TreeView1ID, node.id));
				window.parent.close();
			} else {
				alert("Please select a file/folder.");
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
				TreeView_EditNodeText(TreeView1ID, nodeID, "Please enter the new folder name.")
			else
				OpenDialog("ModalDialogWrapper.axd?EditFile.aspx?fileID=" + TreeView_GetDataKey(TreeView1ID, node.id), 55, 35, window);
		}
		addEvent(window, "unload", DialogUnload);
        //]]>
	</script>
	<asp:label id="lblErrorMsg" CssClass="errorText" runat="server"></asp:label>
	
	<asp:Literal ID="litTitle" Runat="server"></asp:Literal>
	
	<div style="width:23.26em;padding-left:0.38em;">
		<hr />
		<span id="spanNewFolder" runat="server">
			<a href="javascript:NewFolder()"><img alt="Create New Folder" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/FileStore/Images/NewFolder.gif") %>" /></a>&nbsp;
		</span>
		<span id="spanNewFile" runat="server">
			<a href="javascript:NewFile()"><img alt="Upload New File" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/FileStore/Images/NewFile.gif") %>" /></a>&nbsp;
		</span>
		<span id="spanMove" runat="server">
			<a href="javascript:Move()"><img alt="Move a file or folder" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/FileStore/Images/Move.gif") %>" /></a>&nbsp;
		</span>
		<span id="spanDelete" runat="server">
			<a href="javascript:RemoveNode()"><img alt="Delete a file or folder" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/FileStore/Images/Delete.gif") %>" /></a>&nbsp;
		</span>
		<span id="spanRefresh" runat="server">
			<a href="javascript:Refresh()"><img alt="Refresh the folder tree" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/FileStore/Images/Refresh.gif") %>" /></a>&nbsp;
		</span>
		<span id="spanSelect" runat="server">
			<a href="javascript:Select()"><img alt="Select this file/folder" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/FileStore/Images/Select.gif") %>" /></a>&nbsp;
		</span>
		<span id="spanCancel" runat="server">
			<a href="javascript:Cancel()"><img alt="Close this window" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/FileStore/Images/Cancel.gif") %>" /></a>&nbsp;
		</span>
		<hr />
	</div>

	<cc1:TreeView id="TreeView1" runat="server" Width="23.26" CanClickNode="True" CanDoubleClickNode="True"></cc1:TreeView>
	
	