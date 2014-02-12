<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewRemittance.aspx.vb" Inherits="Target.SP.Web.Apps.Payments.ViewRemittance" AspCompat="True" MasterPageFile="~/Popup.master" %>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <div class="header">
		    <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this remittance." onclick="window.close();" />
		    <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this remittance." onclick="window.print();" />
		    <div class="clearer"></div>
		    <hr />
	    </div>
	    <div id="divRemittance" runat="server"></div>
    </asp:Content>