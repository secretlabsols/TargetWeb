<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="svcOrderHeader.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.SvcOrder.svcOrderHeader" MasterPageFile="~/Popup.master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <label class="label" for="lblOrderRef">Order Ref.</label>
	<asp:Label id="lblOrderRef" runat="server" CssClass="content"></asp:Label>
    <br />
    <label class="label" for="lblProviderRef">Provider</label>
	<asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
    <br />
    <label class="label" for="lblContract">Contract</label>
	<asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
    <br />
    <label class="label" for="lblSvcUser">Service User</label>
	<asp:Label id="lblSvcUser" runat="server" CssClass="content"></asp:Label>
    <br />
    <label class="label" for="lblPeriod">Period</label>
	<asp:Label id="lblPeriod" runat="server" CssClass="content"></asp:Label>
    <br />
</asp:Content>
