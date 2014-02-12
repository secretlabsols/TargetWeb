<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RatePreclusions.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.RatePreclusions"
    EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain preclusions against specific rate categories.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewstate="false">
		<cc1:TextBoxEx ID="txtRateFramework" runat="server" LabelText="Rate Framework" LabelWidth="14em" Width="20em" IsReadOnly="true"></cc1:TextBoxEx>
		<br /><br />
		<cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Rate Category" LabelWidth="14em" Width="20em" IsReadOnly="true"></cc1:TextBoxEx>
        <br />
        <br />
        <fieldset>
			<legend>Precluded Rate Categories</legend>
			<table class="listTable" cellspacing="0" cellpadding="2" summary="List of rate categories precluded from rate category above." width="100%">
            <caption>List of rate categories precluded from rate category above</caption>
            <thead>
	            <tr>
		            <th valign="top"  colspan="2">Rate Category</th>
	            </tr>
            </thead>
            <tbody>
				<asp:PlaceHolder ID="phRateCategories" runat="server"></asp:PlaceHolder>
            </tbody>
			</table>
			<asp:Button id="btnAddRateCategory" runat="server" Text="Add" />
        </fieldset>
    </fieldset>
    <br />
</asp:Content>