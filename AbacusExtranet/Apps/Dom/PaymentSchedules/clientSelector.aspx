<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="clientSelector.aspx.vb"
    Inherits="Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.clientSelector" %>

<%@ Register TagPrefix="uc4" TagName="clientSelector" Src="../../UserControls/ClientSelector.ascx" %>
<%@ Reference Control="../../UserControls/ClientSelector.ascx" %>
<%@ Register TagPrefix="uc1" TagName="PaymentScheduleHeader" 
Src="../../UserControls/PaymentScheduleHeader.ascx" %>
<asp:content contentplaceholderid="MPPageOverview" runat="server">
		
</asp:content>
<asp:content id="Content3" contentplaceholderid="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:content>
<asp:content contentplaceholderid="MPContent" runat="server">
    <uc1:PaymentScheduleHeader runat="server" ID="pScheduleHeader" />
    <uc4:clientSelector runat="server" ID="clientSelectorWithPaging" />
    
    <div style="float:right;">
    
    <input runat="server" type="button" style="width:5em;"  value="Back" id="btnBack" name="btnBack"
           onclick="btnBack_Click();"  />
         
    <input runat="server" type="button" style="width:5em;" disabled="disabled" value="Next" id="btnNext" name="btnNext"
           onclick="btnNext_Click();"  />
     
    </div>
    <div class="clearer"></div>
   
</asp:content>