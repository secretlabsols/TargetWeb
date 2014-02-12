<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Terminate.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.DPContracts.Terminate" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="Terminations" Src="UserControls/ucDPContractTerminationProjections.ascx"  %>

<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
    
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:Content>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to terminate a Direct Payment.
</asp:Content>

<asp:Content ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" />    
    <fieldset>
        <legend>Payment Details</legend> 
        <div style="float: left; clear: none; font-weight: bold; padding-right: 2em;">
            <label for="lblNumber">Contract Num.</label>
            <br />
            <label for="lblServiceUser">Service User</label>
            <br />
            <label for="lblBudgetHolder">Budget Holder</label>
            <br />  
            <label for="lblStartDate">Date From</label>
        </div>        
        <div style="float: left; clear: right">
            <asp:Label id="lblNumber" runat="server" />
            <br />
            <asp:Label id="lblServiceUser" runat="server" />
            <br />
            <asp:Label id="lblBudgetHolder" runat="server" />
            <br />
            <asp:Label id="lblStartDate" runat="server" />        
        </div> 
        <div class="clearer"></div>      
	</fieldset>
	<br />
	<fieldset>
        <legend>Terminate Payment</legend>
        <cc1:TextBoxEx ID="dteTerminateDate"  runat="server" LabelText="Required End Date"  Format="DateFormatJquery" LabelBold="true" LabelWidth="12em" Width="7em" />
        <br />
        <cc1:DropDownListEx ID="cboEndReason" runat="server" LabelText="End Reason" LabelBold="true" LabelWidth="12em" />
        <br />
        <uc2:Terminations ID="ucProjectedTerminations" runat="Server"></uc2:Terminations>
		<br />
	    <input type="button" id="btnTerminate" value="Terminate" onclick="btnTerminate_Click(false);" runat="server" style="float:left" title="Terminate?"  />
	    <input type="button" id="btnAutoBalance" value="Auto Balance" onclick="btnTerminate_Click(true);" runat="server" style="float:left" title="Terminate and Auto Balance this Direct Payment?" />
	</fieldset>
</asp:Content>
