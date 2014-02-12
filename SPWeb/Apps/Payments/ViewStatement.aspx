<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewStatement.aspx.vb" Inherits="Target.SP.Web.Apps.Payments.ViewStatement" MasterPageFile="~/Popup.master" %>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <div class="header">
		    <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this statement." onclick="window.close();" />
		    <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this statement." onclick="window.print();" />
		    <div class="clearer"></div>
		    <hr />
	    </div>
	    <div id="divStatement" runat="server"></div>
    </asp:Content>