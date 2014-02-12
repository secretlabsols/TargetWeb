<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ServiceGroups.aspx.vb"
    Inherits="Target.Abacus.Web.Apps.Admin.Dom.ServiceGroups" EnableEventValidation="false" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<asp:content id="Content2" contentplaceholderid="MPPageOverview" runat="server">
    This screen allows you to maintain the different service groups.
</asp:content>
<asp:content id="Content3" contentplaceholderid="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:content>
<asp:content id="Content4" contentplaceholderid="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewState="false">
        <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="11em" MaxLength="255" 
            Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a description" SetFocus="true"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
    <cc1:DropDownListEx ID="cboServiceGroupClassification" runat="server" LabelText="Service Group Classification" LabelWidth="11em"
			Required="True" RequiredValidatorErrMsg="Please select a service group classification" ValidationGroup="Save"></cc1:DropDownListEx>
		<br />
	<asp:Label id="lblServiceCategory" AssociatedControlID="rblServiceCategory" runat="server" Text="Service Category" Width="10em" style="float:left; padding-top:8px;" />	
    <asp:RadioButtonList ID="rblServiceCategory" runat="server" RepeatDirection="Horizontal" CellSpacing="5">
            <asp:ListItem Text="Commissioned Residential" Value="2" onclick="javascript:rblServiceCategory_Changed(this);" />
            <asp:ListItem Text="Commissioned Non-Residential" Value="1" onclick="javascript:rblServiceCategory_Changed(this);" />
            <asp:ListItem Text="Cash" Value="3" onclick="javascript:rblServiceCategory_Changed(this);" />
    </asp:RadioButtonList>   
    <asp:RequiredFieldValidator ID="reqServiceCategory" runat="server" ControlToValidate="rblServiceCategory" 
    ErrorMessage="Please select a service category"  Display="Dynamic"  ValidationGroup="Save" /> 
    <div style="padding-left : 12em; clear : both;">
        <asp:RadioButtonList ID="rblTemporaryOrPermanent" runat="server" RepeatDirection="Horizontal" CellSpacing="5" >
                <asp:ListItem Text="Temporary" Value="0" Selected="True" />
                <asp:ListItem Text="Permanent" Value="1" />
        </asp:RadioButtonList>   
    </div>
    <br />
	        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="10.5em"></cc1:CheckBoxEx>
        <br />
    </fieldset>
    <br />
    <asp:HiddenField ID="hidServiceCategory" runat="server" />
</asp:content>
