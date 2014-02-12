<%@ Control Language="vb" AutoEventWireup="false" Codebehind="NavTree.ascx.vb" Inherits="Target.Web.Apps.Navigation.Controls.NavTree" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
	<script type="text/javascript">
		//<![CDATA[
		function CleanURL(url) {
			url = RemoveQSParam(url, "moveDownMenuID");
			url = RemoveQSParam(url, "moveUpMenuID");
			return url;
		}
		function NewMenu() {
			var node = TreeView_GetSelectedNode(TreeView1ID);
            var newNode;
            var text = "New Menu";
            if(node != null)
				TreeView_AddChildNode(TreeView1ID, node.id, "Please enter the name of the new menu.", "New Menu");
			else
				TreeView_AddRootNode(TreeView1ID, "Please enter the name of the new menu.", "New Menu");
		}
		function RemoveNode() {
			var node = TreeView_GetSelectedNode(TreeView1ID);
			if(node == null) {
				alert("Please select a menu item to delete.");
			} else {
				if(window.confirm("Are you sure you wish to delete this menu item?")) {
					GetElement("aspnetForm").action = CleanURL(GetElement("aspnetForm").action);
					TreeView_RemoveNode(TreeView1ID, node.id);
				}
			}
		}
		function MoveUp() {
			var prevSiblingID = null;
			var node = TreeView_GetSelectedNode(TreeView1ID);
			if(node != null) {
				prevSiblingID = TreeView_GetPrevSibling(TreeView1ID, node.id);
				if(prevSiblingID != null) {
					var url = CleanURL(document.location.href);
					url = AddQSParam(url, "moveDownMenuID", TreeView_GetDataKey(TreeView1ID, prevSiblingID));
					url = AddQSParam(url, "moveUpMenuID", TreeView_GetDataKey(TreeView1ID, node.id));
					document.location.href = url;
				} else {
					alert("This menu is already at the top.");
				}
			} else {
				alert("Please select a menu to move.");
			}
		}
		function MoveDown() {
			var nextSiblingID = null;
			var node = TreeView_GetSelectedNode(TreeView1ID);
			if(node != null) {
				nextSiblingID = TreeView_GetNextSibling(TreeView1ID, node.id);
				if(nextSiblingID) {
					var url = CleanURL(document.location.href);
					url = AddQSParam(url, "moveDownMenuID", TreeView_GetDataKey(TreeView1ID, node.id));
					url = AddQSParam(url, "moveUpMenuID", TreeView_GetDataKey(TreeView1ID, nextSiblingID));
					document.location.href = url;
				} else {
					alert("This menu is already at the bottom.");
				}
			} else {
				alert("Please select a menu to move.");
			}
		}
		function TreeView1_NodeClicked(nodeID) {
			TreeView_SelectNode(TreeView1ID, nodeID);
		}
		function TreeView1_NodeDblClicked(nodeID) {
			if(TreeView_GetTag(TreeView1ID, nodeID) == "1")
				TreeView_EditNodeText(TreeView1ID, nodeID, "Please enter the new menu name.");
		}
		//]]>
	</script>
	<asp:label id="lblErrorMsg" CssClass="errorText" runat="server"></asp:label>
	<div style="width:23.26em;padding-left:0.38em;">
		<hr />
		<span id="spanNewMenu" runat="server">
			<a href="javascript:NewMenu()"><img alt="Create a new menu item" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/Navigation/Images/newMenu.gif") %>" /></a>&nbsp;
		</span>
		<span id="spanDelete" runat="server">
			<a href="javascript:RemoveNode()"><img alt="Remove this menu item" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/Navigation/Images/Delete.gif") %>" /></a>&nbsp;
		</span>
		<span id="spanMoveUp" runat="server">
			<a href="javascript:MoveUp()"><img alt="Move this menu item up" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/Navigation/Images/MoveUp.gif") %>" /></a>&nbsp;
		</span>
		<span id="spanMoveDown" runat="server">
			<a href="javascript:MoveDown()"><img alt="Move this menu item down" src="<%= Target.Library.Web.Utils.GetVirtualPath("Apps/Navigation/Images/MoveDown.gif") %>" /></a>&nbsp;
		</span>
		<hr />
	</div>
	
	<cc1:TreeView id="TreeView1" runat="server" Width="23.26"></cc1:TreeView>
	
	
