<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ExpenditureAccount.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.ExpenditureAccount" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClientTP" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientThirdPartySelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlacePct" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlacePctSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceOrg" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceOtherFundingOrganizationSelector.ascx" %>
<%@ Register TagPrefix="uc5" TagName="InPlaceOLA" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceOtherFundingOrganizationSelector.ascx" %>
<%@ Register TagPrefix="uc6" TagName="BasicAuditDetails" Src="~/Library/UserControls/BasicAuditDetails.ascx" %>
<%@ Register TagPrefix="uc7" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain details of Expenditure Accounts.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <uc6:BasicAuditDetails id="auditDetails" runat="server"></uc6:BasicAuditDetails>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" >
    
        <input id="optCouncil" runat="server" type="radio" name="type" value="1" style="float:left; margin-left:12em;" onclick="javascript:optType_Click();" />
        <label class="label" style="float:left" for="optCouncil" >Council</label>
        <input id="optPCT" runat="server" type="radio" name="type" value="2" style="float:left; margin-left:1em;" onclick="javascript:optType_Click();" />
        <label class="label" style="float:left" for="optPCT" >Clinical Commissioning Group</label>
        <input id="optClient" runat="server" type="radio" name="type" value="3" style="float:left; margin-left:1em;" onclick="javascript:optType_Click();" />
        <label class="label" style="float:left" for="optClient" >Client-specific Third Party</label>
        <input id="optOLA" runat="server" type="radio" name="type" value="4" style="float:left; margin-left:1em;" onclick="javascript:optType_Click();" />
        <label class="label" style="float:left" for="optOLA" >Other Local Authority (OLA)</label>
        <input id="optOther" runat="server" type="radio" name="type" value="5" style="float:left; margin-left:1em;" onclick="javascript:optType_Click();" />
        <label class="label" for="optOther" style="float:left" >Other Organisation</label>
        <br />
        <br />
        <div id="divPCT" style="display:none;">
            <asp:Label AssociatedControlID="pct" runat="server" Text="Clinical Commissioning Group" Width="12em"></asp:Label>
		    <uc2:InPlacePct id="pct" runat="server"></uc2:InPlacePct>
		    <br />
		</div>
		
		<div id="divClient" style="display:none;">
            <asp:Label AssociatedControlID="client" runat="server" Text="Service User" Width="12em"></asp:Label>
	        <uc3:InPlaceClientTP id="client" ThirdPartyLabelWidth="12em" runat="server"></uc3:InPlaceClientTP>
	        <br />
	    </div>
                
        <div id="divOLA" style="display:none;">
            <asp:Label AssociatedControlID="ola" runat="server" Text="Other Local Authority" Width="12em"></asp:Label>
	        <uc5:InPlaceOLA id="ola" ThirdPartyLabelWidth="11em" OrgType="4" runat="server"></uc5:InPlaceOLA>
	        <br />
	    </div>  
	    
	    <div id="divORG" style="display:none;">
            <asp:Label AssociatedControlID="org" runat="server" Text="Organisation" Width="12em"></asp:Label>
	        <uc4:InPlaceOrg id="org" ThirdPartyLabelWidth="11em" OrgType="5" runat="server"></uc4:InPlaceOrg>
	        <br />
	    </div>      
        <div runat="server" enableviewstate="false">
            <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="12em" MaxLength="50" 
                Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a description." SetFocus="true"
                ValidationGroup="Save"></cc1:TextBoxEx>
            <br />
            
            <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="11.5em"></cc1:CheckBoxEx>
            <br /><br /><div class="clearer"></div>
        </div>
        <table class="listTable" cellspacing="0" cellpadding="2" summary="Service Types" width="100%">
		    <caption>Service Types</caption>
			<thead>
			<tr>
				<th>Description</th>
				<th>&nbsp;</th>
			</tr>
			</thead>
			<tbody>
				<asp:PlaceHolder ID="phSvcTypes" runat="server"></asp:PlaceHolder>
			</tbody>
		</table>
		<div><asp:Button id="btnAddSvcTypes" runat="server" Text="Add" ValidationGroup="AddSummary" /></div>
		
        <br />
        <table class="listTable" cellspacing="0" cellpadding="2" summary="Finance Codes" width="100%">
		    <caption>Finance Codes</caption>
			<thead>
			<tr>
				<th>Expenditure Code</th>
				<th>Income Due Code</th>
				<th>&nbsp;</th>
			</tr>
			</thead>
			<tbody>
				<asp:PlaceHolder ID="phFinanceCodes" runat="server"></asp:PlaceHolder>
			</tbody>
		</table>
		<div><asp:Button id="btnAddFinanceCodes" runat="server" Text="Add" ValidationGroup="AddSummary" /></div>
		<br />
		<div style="float:right;"><uc7:ReportsButton id="ctlPrint" runat="server" ButtonText="Print" VerticalPosition="1" HorizontalPosition="1"></uc7:ReportsButton></div>
		<div class="clearer" />
        
    </fieldset>
    <br />
</asp:Content>

