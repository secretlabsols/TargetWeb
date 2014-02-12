<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Reinstate.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.DPContracts.Reinstate" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
	
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to re-instate a terminated Direct Payment.
</asp:Content>
	
<asp:Content ContentPlaceHolderID="MPContent" runat="server">   
    <uc1:StdButtons id="stdButtons1" runat="server" />    
    <fieldset>
        <legend>Payment Details</legend>         
        <label class="label" for="lblNumber">Contract Num.</label>
        <asp:Label id="lblNumber" runat="server" CssClass="content"></asp:Label>
        <br />
        <label class="label" for="lblServiceUser">Service User</label>
        <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
        <br />
        <label class="label" for="lblBudgetHolder">Budget Holder</label>
        <asp:Label id="lblBudgetHolder" runat="server" CssClass="content"></asp:Label>
        <br />  
        <label class="label" for="lblStartDate">Date From</label>
        <asp:Label id="lblStartDate" runat="server" CssClass="content"></asp:Label>
        <br />
        <label class="label" for="lblEndDate">Date To</label>
        <asp:Label id="lblEndDate" runat="server" CssClass="content"></asp:Label>
        <br />
	    <label class="label" for="lblEndReason">End Reason</label>
        <asp:Label id="lblEndReason" runat="server" CssClass="content"></asp:Label>
    </fieldset>
    <br />
    <fieldset>
        <legend>Re-instate Payment</legend>
        <input type="button" id="btnReinstate" value="Re-instate" runat="server" onclick="btnReinstate_Click(); return false;" style="float:left" title="Re-instate?" />
    </fieldset>
    <br />	    
	<div class="clearer"></div>
</asp:Content>