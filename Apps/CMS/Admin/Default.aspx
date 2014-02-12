<%@ Register TagPrefix="uc1" TagName="PageWizardHeader" Src="../Controls/PageWizardHeader.ascx" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Default.aspx.vb" Inherits="Target.Web.Apps.CMS.Admin.DefaultPage" EnableViewState="True" %>
	
		<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">Please select an action to perform.</asp:Content>
		<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		    <uc1:PageWizardHeader id="wizHeader" runat="server"></uc1:PageWizardHeader>
		    <asp:Label id="lblError" runat="server" CssClass="errorText"></asp:Label>
		    <script type="text/javascript">
			    function Next() {
				    if(GetElement("rdoCreate").checked) {
					    document.location.href = "EditPage.aspx";
					    return;
				    }
				    if(GetElement("rdoEdit").checked) {
					    OpenDialog(selectPageUrl, 26.35, 40.70, window);
					    return;
				    }
				    if(GetElement("rdoDelete").checked) {
					    OpenDialog(selectPageUrl, 26.35, 40.70, window);
					    return;
				    }
				    if(GetElement("rdoEditSelected")) {
					    if(GetElement("rdoEditSelected").checked) {
						    document.location.href = editSelectedUrl;
						    return;
					    }
				    }
			    }
			    function CMSTree_ItemSelected(pageID) {
				    if(GetElement("rdoEdit").checked) {
					    document.location.href = "EditPage.aspx?pageID=" + pageID;
					    return;
				    }
				    if(GetElement("rdoDelete").checked) {
					    if(window.confirm("Are you sure you wish to delete this page?")) {
						    document.location.href = AddQSParam(RemoveQSParam(document.location.href, "deletePageID"), "deletePageID", pageID);
					    }
					    return;
				    }
			    }
		    </script>

		    <asp:RadioButton id=rdoCreate runat="server" GroupName="options" Checked="True"></asp:RadioButton>
		    <asp:Label id="lblCreate" AssociatedControlID="rdoCreate" runat="server" CssClass="inline">Create a new page</asp:Label>
		    <br />
		    <asp:RadioButton id=rdoEdit runat="server" GroupName="options"></asp:RadioButton>
		    <asp:Label id="lblEdit" AssociatedControlID="rdoEdit" runat="server" CssClass="inline">Edit an existing page</asp:Label>
		    <br />
		    <asp:RadioButton id=rdoDelete runat="server" GroupName="options"></asp:RadioButton>
		    <asp:Label id="lblDelete" AssociatedControlID="rdoDelete" runat="server" CssClass="inline">Delete an existing page</asp:Label>
		    <br />
		    <asp:RadioButton id=rdoEditSelected runat="server" GroupName="options"></asp:RadioButton>
		    <asp:Label id="lblEditSelected" AssociatedControlID="rdoEditSelected" runat="server" CssClass="inline">Edit [Page Title Goes Here]</asp:Label>
		    <br />
		    <input type="button" onclick="Next()" value=" Next " /> 
    </asp:Content>