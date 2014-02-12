<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UnitCost.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.UnitCost" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
	    <table class="listTable" cellpadding="4" cellspacing="0" width="100%" summary="Lists rates for the contract for the selected period.">
	    <caption>Lists rates for the contract for the selected period.</caption>
	    <tr>
		    <th>Description</th>
		    <th>Measured In</th>
		    <th>Abbreviation</th>
		    <th>Unit Cost</th>
	    </tr>
	    <asp:PlaceHolder ID="phUnitCosts" runat="server"></asp:PlaceHolder>
	    </table>
	    <br />
    </fieldset>
    <br />
</asp:Content>