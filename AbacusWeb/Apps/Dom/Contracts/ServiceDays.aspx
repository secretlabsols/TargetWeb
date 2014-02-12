<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ServiceDays.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.ServiceDays" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
	    <table class="listTable" cellpadding="4" cellspacing="0" width="30%" summary="Lists the days that service is delivered.">
	    <caption>Lists the days that service is delivered.</caption>
	    <tr>
		    <th>Days</th>
	    </tr>
	    <asp:PlaceHolder ID="phUnitCosts" runat="server"></asp:PlaceHolder>
	    </table>
	    <br />
    </fieldset>
    <br />
</asp:Content>
