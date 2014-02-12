<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VisitAmendmentRequestList.aspx.vb" 
Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.VisitAmendmentRequestList" %>
<%@ Register TagPrefix="uc1" TagName="SelectorWizard" Src="~/Library/UserControls/SelectorWizard.ascx" %>
<%@ Register TagPrefix="uc2" TagName="pScheduleHeader" Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>
<%@ Reference Control="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	   
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <div style="margin-bottom:10px;">
		    <uc2:pScheduleHeader id="pSchedules" runat="server"></uc2:pScheduleHeader>
		    
		</div>
		
		<div>
            <uc1:SelectorWizard id="SelectorWizard1" runat="server"></uc1:SelectorWizard>
        </div>
	<%--<div id="pnlBack" runat="server"> <input type="button" style="width: 5em;" id="btnBack" value="Back"
        title="Back" onclick="btnBack_Click();" />
         </div>      --%>  
    </asp:Content>
