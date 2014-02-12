<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Terminate.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.Terminate" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>
	
	<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to terminate a domiciliary contract.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<input type="button" id="btnBack" value="Back" onclick="btnBack_Click();" />
		<br /><br />
	    <fieldset>
            <legend>Contract Details</legend>         
            <label class="label" for="lblNumber">Number</label>
	        <asp:Label id="lblNumber" runat="server" CssClass="content"></asp:Label>
            <br />
            <label class="label" for="lblTitle">Title</label>
	        <asp:Label id="lblTitle" runat="server" CssClass="content"></asp:Label>
	        <br />  
            <label class="label" for="lblDescription">Description</label>
	        <asp:Label id="lblDescription" runat="server" CssClass="content"></asp:Label>
	        <br />
            <label class="label" for="lblProvider">Provider</label>
	        <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	        <br />
            <label class="label" for="lblStartDate">Start Date</label>
	        <asp:Label id="lblStartDate" runat="server" CssClass="content"></asp:Label>
	        <br />
		    <label class="label" for="lblContractType">Contract Type</label>
	        <asp:Label id="lblContractType" runat="server" CssClass="content"></asp:Label>
            <br />
            <label class="label" for="lblServiceUser">Service User</label>
	        <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
	    </fieldset>
	    <br />
	    <fieldset>
            <legend>Terminate Contract</legend>
	        <cc1:TextBoxEx ID="dteTerminateDate"  runat="server" LabelText="End Date"  Format="DateFormat" LabelBold="true" LabelWidth="12em"
                Width="20em" Required="true" RequiredValidatorErrMsg="Please enter a termination date for the contract"
                ValidationGroup="Save"></cc1:TextBoxEx>
            <br />
	        <cc1:DropDownListEx ID="cboEndReason" runat="server" LabelText="End Reason" LabelBold="true" LabelWidth="12em"
			    Required="true" RequiredValidatorErrMsg="Please select a end reason for the contract." ValidationGroup="Save"></cc1:DropDownListEx>
		    <input type="submit" id="btnTerminate" value="Terminate" onclick="if(!window.confirm('Are you sure you wish to terminate this contract?')) return false;" runat="server" style="float:right"  />
		</fieldset>
		<div class="clearer"></div>
	</asp:Content>
