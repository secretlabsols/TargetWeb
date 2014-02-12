<%@ Register TagPrefix="uc1" TagName="PageWizardHeader" Src="../Controls/PageWizardHeader.ascx" %>
<%@ Register TagPrefix="ftb" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="EditPage.aspx.vb" Inherits="Target.Web.Apps.CMS.Admin.EditPage" ValidateRequest="False" EnableViewState="True" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">Please make the required changes to the page below.</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:PageWizardHeader id=wizHeader runat="server"></uc1:PageWizardHeader>
	    <script type="text/javascript">
		    //<![CDATA[
	        var ftbContentID = "ctl00_MPContent_ftbContent";
		    
		    function LinkToFileStoreFile() {
			    OpenDialog(linkToFileStoreFileUrl, 26.35, 40.70, window);
		    }
		    function FileStoreTree_ItemSelected(nodeDataKey) {
			    var baseUrl = "../../FileStore/FileStoreGetFile.axd?id=";
			    var sel = FTB_API[ftbContentID].GetSelection();
			    var range = FTB_API[ftbContentID].CreateRange(sel);
			    if((ie && range.text.length == 0) ||(!ie && range.toString().length == 0)) {
				    FTB_API[ftbContentID].InsertHtml("<a href=\"" + baseUrl + nodeDataKey + "\">New Link</a>");
			    } else {
				    FTB_API[ftbContentID].ExecuteCommand("createlink", false, baseUrl + nodeDataKey);
			    }
		    }
		    function LinkToCMSPage() {
			    OpenDialog(selectPageUrl, 26.35, 40.70, window);
		    }
		    function CMSTree_ItemSelected(pageID) {
			    var baseUrl = "CMSGetPage.axd?id=";
			    var sel = FTB_API[ftbContentID].GetSelection();
			    var range = FTB_API[ftbContentID].CreateRange(sel);
			    if((ie && range.text.length == 0) ||(!ie && range.toString().length == 0)) {
				    FTB_API[ftbContentID].InsertHtml("<a href=\"" + baseUrl + pageID + "\">New Link</a>");
			    } else {
				    FTB_API[ftbContentID].ExecuteCommand("createlink", false, baseUrl + pageID);
			    }
		    }
		    function InsertImageFromFileStore() {
			    OpenDialog("ModalDialogWrapper.axd?../../FileStore/Admin/ImageGallery.aspx", 64, 48, window);
		    }
		    function FileStoreImageGallery_InsertImage(html) {
			    HideModalDIV();
			    FTB_API[ftbContentID].InsertHtml(html);
		    }
		    function FTB_Custom_Window_Onload() {
			    var objNext = GetElement("btnNext");
			    objNext.value = " Save ";
			    objNext.disabled = false;
		    }
		    //]]>
	    </script>

        <asp:Label id=lblTitle runat="server" AssociatedControlID="txtTitle" Style="width:6.20em;">Title</asp:Label>
	    <asp:TextBox id=txtTitle runat="server" width="38.76em" MaxLength="255"></asp:TextBox>
	    <br />
	    <asp:RequiredFieldValidator id=reqTitle runat="server" ErrorMessage="Please enter a Title for the page." ControlToValidate="txtTitle" Display="Dynamic"></asp:RequiredFieldValidator>
	    <br />
	    <asp:Label id="lblSubTitle" runat="server" AssociatedControlID="txtSubTitle" Style="width:6.20em;">Sub-Title</asp:Label>
	    <asp:TextBox id=txtSubTitle runat="server" Rows="3" Columns="80" TextMode="MultiLine" Width="38.76em"></asp:TextBox>
	    <br /><br />
	    <FTB:FreeTextBox id="ftbContent" runat="server" 
	        AutoHideToolbar="True" 
	        AutoGenerateToolbarsFromString="False" 
	        SupportFolder="~/Apps/CMS/" 
	        StartMode="DesignMode" 
	        StripAllScripting="False" 
	        Focus="False" 
	        JavaScriptLocation="ExternalFile"
	        Width="99%">
        </FTB:FreeTextBox>
	    <br />
	    <asp:Button id=btnNext runat="server" Enabled="true" Text=" Save "></asp:Button>
    </asp:Content>    	

