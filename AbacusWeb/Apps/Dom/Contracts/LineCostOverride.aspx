<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LineCostOverride.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.LineCostOverride" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="conError" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>

<asp:Content ID="conContent" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" />    
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        Where indicated in the table below the calculated cost of a line on a summary-level Pro forma or Provider Invoice may be manually overriden by the user.
        <br />
        <br />
	    <asp:GridView ID="gvOverrides" runat="server" AutoGenerateColumns="false" DataKeyNames="ID" Border="0" CssClass="listTable" CellPadding="4" CellSpacing="0" Width="100%">
	        <Columns>
	            <asp:BoundField DataField="DomRateCategoryDescription" HeaderText="Rate Category" HeaderStyle-Width="42.5%" />
	            <asp:BoundField DataField="DomUnitOfMeasureDescription" HeaderText="Measured In" HeaderStyle-Width="42.5%" />
	            <asp:CheckBoxField DataField="AllowOverride" HeaderText="Allow Override" HeaderStyle-Width="15%" />
	        </Columns>
	    </asp:GridView>
	    <br />
    </fieldset>
    <br />
</asp:Content>