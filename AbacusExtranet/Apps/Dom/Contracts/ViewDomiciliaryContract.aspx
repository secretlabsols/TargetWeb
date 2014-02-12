<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewDomiciliaryContract.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.Contracts.ViewDomiciliaryContract" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details for the selected Contract.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" id="btnBack" value="Back" onclick="btnback_Click();" />
	    <input type="button" id="btnViewRates" runat="server" style="width:11em;" value="View Contract Rates" onclick="btnView_Click();" />
	    <br />
	    <br />
	    <label class="label" for="lblReference">Provider Reference</label>
	    <asp:Label id="lblProviderRef" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblProviderName">Provider Name</label>
	    <asp:Label id="lblProviderName" runat="server" CssClass="content"></asp:Label>
	    <br />
		<label class="label" for="lblContractNo">Contract Number</label>
	    <asp:Label id="lblContractNo" runat="server" CssClass="content"></asp:Label>
		<br />
        <label class="label" for="lblContractTitle">Contract Title</label>
	    <asp:Label id="lblContractTitle" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblContractDesc">Contract Description</label>
	    <asp:Label id="lblContractDesc" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblServiceUserRef">Service User Reference</label>
	    <asp:Label id="lblServiceUserRef" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblServiceUserName">Service User Name</label>
	    <asp:Label id="lblServiceUserName" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblStartDate">Start Date</label>
	    <asp:Label id="lblStartDate" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblEndDate">End Date</label>
	    <asp:Label id="lblEndDate" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label"  for="lblDcr">Duration Claimed Rounding</label>
	    <asp:Label id="lblDcr" runat="server" CssClass="content"></asp:Label>
	    <br />
    </asp:Content>
