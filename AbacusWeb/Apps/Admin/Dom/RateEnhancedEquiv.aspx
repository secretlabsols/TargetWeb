<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RateEnhancedEquiv.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.RateEnhancedEquiv" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain enhanced rate category equivalents.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewState="false">
		<table class="listTable" id="tblDetails" cellspacing="0" cellpadding="2" summary="List of details in this framework." width="100%">
        <%--<caption>List of details in this framework</caption>--%>
        <thead>
            <tr>
	            <th>Standard</th>
	            <th>Enhanced</th>
            </tr>
        </thead>
        <tbody>
			<asp:PlaceHolder ID="phDetails" runat="server"></asp:PlaceHolder>
        </tbody>
		</table>
        <br />
        <div class="clearer"></div>
    </fieldset>
    <br />

</asp:Content>
