<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewSUNotif.aspx.vb" Inherits="Target.SP.Web.Apps.SUNotif.ViewSUNotif" AspCompat="True" MasterPageFile="~/Popup.master" %>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <div class="header">
		    <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this notification." onclick="window.close();" />
		    <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this notification." onclick="window.print();" />
		    <input type="button" id="btnViewScanned" value="View Scanned Notification" style="float:right" title="Click here to view the uploaded scanned notification document." runat="server" />
		    <div class="clearer"></div>
		    <hr />
	    </div>
	    <div id="divNotif" runat="server"></div>
    </asp:Content>