﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ServiceOutcomes.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.ServiceOutcomes" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different Service Outcome groups.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewState="false">
        <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="8em" MaxLength="255" 
            Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a description" SetFocus="true"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="7.5em"></cc1:CheckBoxEx>
        <div class="clearer"></div>
        <br />
        <fieldset>
			<legend>Codes</legend>
			<table class="listTable" cellspacing="0" cellpadding="2" summary="List of Service Outcomes in this group." width="100%">
            <caption>List of Service Outcomes in this group</caption>
            <thead>
	            <tr>
		            <th valign="top">Description</th>
		            <th valign="top">Code</th>
		            <th valign="top">Provider<br />Paid</th>
		            <th valign="top">Client<br />Charged</th>
		            <th valign="top">Default<br />Code</th>
		            <th valign="top">Type</th>
		            <th valign="top">&nbsp;</th>
	            </tr>
            </thead>
            <tbody>
				<asp:PlaceHolder ID="phServiceOutcomes" runat="server"></asp:PlaceHolder>
            </tbody>
			</table>
			<asp:Button id="btnAddServiceOutcome" runat="server" Text="Add" />
        </fieldset>
    </fieldset>
    <br />
</asp:Content>
