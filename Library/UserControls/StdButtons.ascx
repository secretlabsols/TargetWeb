<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="StdButtons.ascx.vb" Inherits="Target.Web.Library.UserControls.StdButtons" EnableViewState="false" %>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<input type="hidden" id="hidMode" runat="server" />
<input type="hidden" id="hidSearchBy" runat="server" />
<input type="hidden" id="hidSelectedItemID" runat="server" />
<fieldset class="stdButtons" id="divBack" runat="server">
    <div>
	    <input type="button" id="btnBack" runat="server" value="Back" onclick="document.location.href=unescape(GetQSParam(document.location.search, 'backUrl'));" />
    </div>
</fieldset>
<fieldset class="stdButtons" id="divButtons" runat="server">
    <div>
        <asp:Button ID="btnNew" runat="server" Text="New" ValidationGroup="New" />
        <asp:Button ID="btnEdit" runat="server" Text="Edit"  ValidationGroup="Edit" />
        <asp:Button ID="btnSave" runat="server" Text="Save" ValidationGroup="Save"  />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel"  ValidationGroup="Cancel" />
        <asp:Button ID="btnDelete" runat="server" Text="Delete"  ValidationGroup="Delete" />
    </div>
</fieldset>
<fieldset class="stdButtons" id="divFinder" runat="server">
    <asp:Panel runat="server" DefaultButton="btnFind">
        <asp:DropDownList ID="cboSearchBy" runat="server" style="height:22px;" ></asp:DropDownList>
        <asp:TextBox ID="txtSearchFor" runat="server" style="height:17px;" ></asp:TextBox>
        <asp:Button ID="btnFind" runat="server" Text="Find"  ValidationGroup="Find" />
    </asp:Panel>
</fieldset>
<fieldset class="stdButtons" id="divAudit" runat="server">
    <div>
	    <input type="button" id="btnAudit" class="stdButton" style="width:6em;" value="Audit Log" runat="server" />
    </div>
</fieldset>
<fieldset class="stdButtons" id="divReports" runat="server" visible="false">
    <div>
	    <uc1:ReportsButton id="ctlReports" runat="server" ></uc1:ReportsButton>
    </div>
</fieldset>
<fieldset class="stdButtons" id="fsCustomControls" runat="server">
    <div>
        <asp:PlaceHolder id="phCustomControls" runat="server"></asp:PlaceHolder>
    </div>
</fieldset>

<script type="text/javascript">
	function StdButtons_DoFindPostBack() {
		<asp:Literal ID="litDoFindPostBackJS" runat="server" />
	}
</script>
<div class="clearer"></div>