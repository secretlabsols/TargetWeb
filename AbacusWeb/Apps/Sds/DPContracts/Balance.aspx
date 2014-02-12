<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Balance.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.DPContracts.Balance" EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="Terminations" Src="UserControls/ucDPContractTerminationProjections.ascx"  %>

<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
    
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:Content>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to create a balancing payment for an under/over paid Direct Payment Contract.
</asp:Content>

<asp:Content ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" />    
    <fieldset>
        <legend>Contract Details</legend> 
        <div style="float: left; clear: none; font-weight: bold; padding-right: 2em;">
            <label for="lblNumber">Contract Num.</label>
            <br />
            <label for="lblServiceUser">Service User</label>
            <br />
            <label for="lblBudgetHolder">Budget Holder</label>
            <br />  
            <label for="lblStartDate">Date From</label>
            <br />  
            <label for="lblEndDate">Date To</label>
            <br />  
            <label for="lblEndReason">End Reason</label>
            <br />  
            <label for="lblRequiredEndDate">Required End Date</label>
            <br />  
            <label for="lblBalanced">Balanced</label>
        </div>        
        <div style="float: left; clear: right">
            <asp:Label id="lblNumber" runat="server" />
            <br />
            <asp:Label id="lblServiceUser" runat="server" />
            <br />
            <asp:Label id="lblBudgetHolder" runat="server" />
            <br />
            <asp:Label id="lblStartDate" runat="server" />     
            <br />
            <asp:Label id="lblEndDate" runat="server" />     
            <br />
            <asp:Label id="lblEndReason" runat="server" />   
            <br />
            <asp:Label id="lblRequiredEndDate" runat="server" />   
            <br />
            <asp:Label id="lblBalanced" runat="server" /> 
        </div> 
        <div class="clearer"></div>      
	</fieldset>
	<br />
	<fieldset>
        <legend>Balance Contract</legend>
        <uc2:Terminations ID="ucProjectedTerminations" runat="Server"></uc2:Terminations>
		<br />
		<cc1:TextBoxEx ID="txtBalancingAmount" runat="server" LabelText="Balancing Amount" Format="CurrencyFormat" LabelBold="true" LabelWidth="13.75em" Width="7em" />
        <br />
        <cc1:TextBoxEx ID="dtePeriodFrom" runat="server" LabelText="Period From" Format="DateFormatJquery" LabelBold="true" LabelWidth="13.75em" Width="7em" OutputBrAfter="false" />
        &nbsp;
        <cc1:TextBoxEx ID="dtePeriodTo" runat="server" LabelText="To&nbsp;" Format="DateFormatJquery" LabelBold="true" LabelWidth="2em" Width="7em" />
        <br />
        <asp:CheckBox ID="cbExcludeFromCreditors" runat="server" Checked="false" Text="Exclude From Creditors&nbsp;" TextAlign="Left" Font-Bold="true" ToolTip="Exclude From Creditors?" />
        <br />
        <br />
		<div style="float: left;">
		    <input type="button" id="btnCreatePayment" value="Create Payment" onclick="btnCreateBalancingPayment_Click();" runat="server" title="Create Payment?"  />
		</div>
		<div style="float: right;">
	        <input type="button" id="btnCreateLetter" value="Create Letter" runat="server" title="Create Letter?" />
	        <input type="button" id="btnMarkAsBalanced" value="" onclick="btnMarkAsBalanced_Click();" runat="server" />
	    </div>
	    <div class="clearer"></div>
	</fieldset>
</asp:Content>
