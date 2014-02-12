<%@ Page Language="vb" AutoEventWireup="false" Codebehind="GenericFinder.aspx.vb" Inherits="Target.Web.Apps.GenericFinder.GenericFinder" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="uc1" TagName="GenericFinder" Src="~/Library/UserControls/GenericFinderResults.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <p style="padding:0.5em;">
		    <uc1:GenericFinder ID="genericFinder1" runat="server"></uc1:GenericFinder>
	    </p>
    </asp:Content>