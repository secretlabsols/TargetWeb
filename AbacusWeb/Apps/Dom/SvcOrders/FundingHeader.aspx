<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FundingHeader.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.SvcOrder.FundingHeader" MasterPageFile="~/Popup.master" EnableViewState="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="BasicAuditDetails" Src="~/Library/UserControls/BasicAuditDetails.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <uc2:BasicAuditDetails id="auditDetails" runat="server"></uc2:BasicAuditDetails>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewState="false">
        <cc1:DropDownListEx ID="cboServiceType" runat="server" LabelText="Service Type" LabelWidth="22em"
			    Width="10em" Required="true" RequiredValidatorErrMsg="Please select a Service Type"
			    ValidationGroup="Save"></cc1:DropDownListEx>
	    <br />
	    <input id="optApportion" runat="server" type="radio" name="type" checked="True" onclick="javascript:optOption_Click();" style="float:left; margin-left:19.5em;" />
        <label class="label" style="float:left" for="optApportion" >Apportion Actual Service Equally</label>
        <br /><br />
        <input id="optOffPlan" runat="server" type="radio" name="type" onclick="javascript:optOption_Click();" style="float:left; margin-left:19.5em;" />
        <label class="label" for="optOffPlan" style="float:left" >Call Off Planned Service in Order</label>
        <br /><br />
        <cc1:CheckBoxEx ID="chkDefault" runat="server" Text="Use as Default for other Service Types" LabelWidth="22em"></cc1:CheckBoxEx>
        <br /><br />
        <table class="listTable" id="tblProportions" cellspacing="0" cellpadding="2" summary="Proportions" width="100%">
		    <caption>Proportions</caption>
			<thead>
			<tr>
			    <th>Income Code</th>
				<th>Numerator</th>
				<th>Denominator</th>
				<th>Call Off Order</th>
				<th>&nbsp;</th>
			</tr>
			</thead>
			<tbody>
				<asp:PlaceHolder ID="phProportions" runat="server"></asp:PlaceHolder>
			</tbody>
		</table>
		<br />
    </fieldset>
</asp:Content>
