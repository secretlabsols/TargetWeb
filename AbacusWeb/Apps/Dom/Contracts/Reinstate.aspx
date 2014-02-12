<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Reinstate.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.Reinstate" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
	
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to re-instate a terminated domiciliary contract.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<input type="button" id="btnBack" value="Back" onclick="btnBack_Click();" />
		<br /><br />
	    
	    <label class="label" for="lblNumber">Number</label>
	    <asp:Label id="lblNumber" runat="server" CssClass="content"></asp:Label>
        <br />
        <label class="label" for="lblTitle">Title</label>
	    <asp:Label id="lblTitle" runat="server" CssClass="content"></asp:Label>
	    <br />  
        <label class="label" for="lblDescription">Description</label>
	    <asp:Label id="lblDescription" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblProvider">Provider</label>
	    <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblStartDate">Start Date</label>
	    <asp:Label id="lblStartDate" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblEndDate">End Date</label>
	    <asp:Label id="lblEndDate" runat="server" CssClass="content"></asp:Label>
	    <br />
		<label class="label" for="lblEndReason">End Reason</label>
	    <asp:Label id="lblEndReason" runat="server" CssClass="content"></asp:Label>
        <br />
        <label class="label" for="lblContractType">Contract Type</label>
	    <asp:Label id="lblContractType" runat="server" CssClass="content"></asp:Label>
        <br />
        <label class="label" for="lblServiceUser">Service User</label>
	    <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
	    <input type="button" id="btnReinstate" value="Re-instate" runat="server" onclick="if(!window.confirm('Are you sure you wish to re-instate this contract?')) return false;" style="float:right"  />
	    <br />	    
		<div class="clearer"></div>
	</asp:Content>