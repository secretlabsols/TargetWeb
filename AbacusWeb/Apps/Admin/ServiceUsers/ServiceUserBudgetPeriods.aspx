<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ServiceUserBudgetPeriods.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.ServiceUsers.ServiceUserBudgetPeriods" EnableViewState="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the budget periods for individual service users.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewState="false">
        <cc1:TextBoxEx ID="txtReference"  runat="server"  LabelText="S/U Reference" LabelWidth="8em" MaxLength="255" 
            Width="20em"  Required="false" SetFocus="true" IsReadOnly="true"
            ValidationGroup="Save"></cc1:TextBoxEx>   
        <br />
          <br />
        <cc1:TextBoxEx ID="txtName"  runat="server"  LabelText="S/U Name" LabelWidth="8em" MaxLength="255" 
            Width="20em"  Required="false" SetFocus="true" IsReadOnly="true"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <div class="clearer"></div>
        <br />
        <fieldset>
		    <legend>Periods</legend>
		    <table class="listTable" cellspacing="0" cellpadding="2" summary="List of periods for this type" width="100%">
            <caption>List of service user budget periods</caption>
            <thead>
                <tr>
	                <th>Date From</th>
	                <th>Date To</th>
	                <th>&nbsp;</th>
                </tr>
            </thead>
            <tbody>
			    <asp:PlaceHolder ID="phPeriods" runat="server"></asp:PlaceHolder>
            </tbody>
		    </table>
		    <asp:Button id="btnAddPeriod" runat="server" Text="Add" />
        </fieldset>
    </fieldset>
</asp:Content>