<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ProcessAmendReq.aspx.vb" Inherits="Target.Web.Apps.AmendReq.Admin.ProcessAmendReq" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details of this amendment request. Use the buttons below to accept or decline the changes contained in this request as appropriate.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server"></asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <label for="spnReference" class="label">Reference</label><span id="spnReference" class="content"></span>
	    <br />
	    <br />
	    <label for="spnRequested" class="label">Requested</label><span id="spnRequested" class="content"></span>
	    <br />
	    <br />
	    <label for="spnRequestedBy" class="label">Requested By</label><span id="spnRequestedBy" class="content"></span>
	    <br />
	    <br />
	    <label for="spnRequest" class="label">Request</label><span id="spnRequest" class="content"></span>
	    <br />
	    <br />
	    <label for="spnOldValue" class="label">Value(s) When Requested</label><span id="spnOldValue" class="content"></span>
	    <br />
	    <br />
	    <label for="spnCurrentValue" class="label">Current Value(s)</label><span id="spnCurrentValue" class="content"></span>
	    <br />
	    <br />
	    <label for="spnNewValue" class="label">New Value(s)</label><span id="spnNewValue" class="content"></span>
	    <br />
	    <br />
	    <cc1:TextBoxEx id="txtComment" runat="server" LabelText="Comment" LabelWidth="16em" LabelBold="True" Width="45em" SetFocus="True"></cc1:TextBoxEx>
	    <br />
    		
	    <div class="clearer"></div>
    	
	    <input type="button" id="btnAccept" value="Accept" title="Click here to accept the changes in this request." disabled="true" onclick="btnAccept_OnClick();" />
	    <input type="button" id="btnDecline" value="Decline" title="Click here to decline the changes in this request." disabled="true" onclick="btnDecline_OnClick();" />	
	    <input type="button" id="btnCancel" value="Cancel" title="Click here to go back to the previous screen." onclick="btnCancel_OnClick();" />	
    </asp:Content>