<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Edit.aspx.vb" Inherits="Target.Web.Apps.SavedWizardSelections.Edit" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to view, edit or delete existing saved wizard selections.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litPageError" runat="server"></asp:Literal>
	</asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
        <fieldset id="fsControls" runat="server">
            <br />
            <cc1:TextBoxEx ID="txtName" runat="server" LabelText="Name" LabelWidth="13em" MaxLength="50" Width="20em" SetFocus="true"
                Required="true" RequiredValidatorErrMsg="Please enter a Name" ValidationGroup="Save"></cc1:TextBoxEx>
            <br />
            <cc1:TextBoxEx ID="txtScreen" runat="server" LabelText="Screen" LabelWidth="13em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
            <br /><br />
            <cc1:TextBoxEx ID="txtOwner" runat="server" LabelText="Owner" LabelWidth="13em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
            <br /><br />
            <cc1:CheckBoxEx ID="chkGlobal" runat="server" Text="Global Saved Selection" LabelWidth="12.75em"></cc1:CheckBoxEx>
            <br /><br />
        </fieldset>                
    </asp:Content>