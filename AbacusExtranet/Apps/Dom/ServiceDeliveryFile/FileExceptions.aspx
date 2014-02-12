<%@ Page Language="vb" AutoEventWireup="false" Codebehind="FileExceptions.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ServiceDeliveryFile.FileExceptions" MasterPageFile="~/Popup.master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
	    <div class="header">
		    <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this remittance." onclick="window.close();" />
		    <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this remittance." onclick="window.print();" />
		    <div class="clearer"></div>
		    <hr />
	    </div>
	    <div id="divExceptions" runat="server"></div>
    </asp:Content>
