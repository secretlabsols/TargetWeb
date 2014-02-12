<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ManageDistLists.aspx.vb" Inherits="Target.Web.Apps.Msg.Admin.ManageDistLists" ValidateRequest="False" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		To edit an existing distribution list, select the list in the dropdown list below. 
		To create a new distribution list, click on the 'New' button.
	</asp:Content>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <label for="cboDistLists">Distribution Lists</label>&nbsp;<select id="cboDistLists" name="cboDistLists" onchange="cboDistLists_OnChange();"><option value="0"></option></select>
	    <input type="button" id="btnNew" value="New" title="Click here to create a new distibution list" onclick="btnNew_OnClick();" />
	    <br />
	    <br />
	    <cc1:TextBoxEx id="txtName" runat="server" LabelText="Name" LabelWidth="8.75em" Width="45em" MaxLength="50"
		    Required="True" RequiredValidatorErrMsg="Please enter the distribution list name."></cc1:TextBoxEx>
	    <br />
	    <input type="button" id="btnRecipients" style="float: left;width: 7em;" value="Recipients" title="Click here to select the recipients in this distribution list" onclick="btnRecipients_OnClick();" />
	    <span id="spnRecipients" style="float: left;margin-left: 1.75em;width: 85%;"></span>
	    <input type="hidden" id="txtRecipients" runat="server" name="txtRecipients" />
	    <input type="hidden" id="txtRecipientNames" runat="server" name="txtRecipientNames" />
	    <div class="clearer"></div>
	    <br />
	    <asp:Button id="btnSave" runat="server" text="Save" title="Click here to save the distribution list"></asp:Button>
	    <input type="button" id="btnDelete" value="Delete" disabled="disabled" title="Click here to delete the distribution list" onclick="btnDelete_OnClick();" />
    </asp:Content>