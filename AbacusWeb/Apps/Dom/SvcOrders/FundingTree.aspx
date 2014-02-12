<%@ Page Language="vb" AutoEventWireup="false" Codebehind="FundingTree.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.SvcOrder.FundingTree" MasterPageFile="~/Popup.master" %>
	
	<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">

		<asp:TreeView ID="tree" runat="server" 
			EnableClientScript="true" 
			EnableViewState="false"
			ShowLines="false">
			<NodeStyle CssClass="aspTreeView_Node" />
            <SelectedNodeStyle CssClass="aspTreeView_SelectedNode" />
            <HoverNodeStyle CssClass="aspTreeView_HoverNode" />
		</asp:TreeView>
		
	</asp:Content>
