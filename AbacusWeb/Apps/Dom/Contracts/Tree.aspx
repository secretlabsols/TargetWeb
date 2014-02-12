<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Tree.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.Tree" MasterPageFile="~/Popup.master" %>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">

		<asp:TreeView ID="tree" runat="server" 
			EnableClientScript="true" 
			EnableViewState="false"
			ShowLines="false">
			<NodeStyle CssClass="aspTreeView_Node" />
            <SelectedNodeStyle CssClass="aspTreeView_SelectedNode" />
            <HoverNodeStyle CssClass="aspTreeView_HoverNode" />
		</asp:TreeView>
		
	</asp:Content>
	
	