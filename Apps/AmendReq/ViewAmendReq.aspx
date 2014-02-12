<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewAmendReq.aspx.vb" Inherits="Target.Web.Apps.AmendReq.ViewAmendReq" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details of this amendment request.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server"></asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" id="btnBack" value="Back" title="Click here to go back to the list of requests." onclick="history.go(-1);" />
	    <input type="button" id="btnStartConv" value="Start Conversation" style="width:10em;" title="Click here to start a new messaging conversation." onclick="btnStartConv_OnClick();" />
	    <input type="button" id="btnProcess" value="Process" title="Click here to process this request." runat="server" onclick="btnProcess_OnClick();" />
	    <br />
	    <br />
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
	    <label for="spnNewValue" class="label">New Value(s)</label><span id="spnNewValue" class="content"></span>
	    <br />
	    <br />
	    <label for="spnStatus" class="label">Status</label><span id="spnStatus" class="content"></span>
	    <br />
	    <br />
	    <label for="spnProcessed" class="label">Processed By</label><span id="spnProcessed" class="content">&nbsp;</span>
	    <br />
	    <br />
	    <label for="spnComment" class="label">Comment</label><span id="spnComment" class="content">&nbsp;</span>
	    <br />
    		
	    <div class="clearer"></div>
    </asp:Content>