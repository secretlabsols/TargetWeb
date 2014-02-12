<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="BasicAuditDetails.ascx.vb" Inherits="Target.Web.Library.UserControls.BasicAuditDetails" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<ajaxToolkit:CollapsiblePanelExtender 
    ID="cpe" 
    runat="server"
    TargetControlID="pnlContent"
    ExpandDirection="Vertical"
    />
<asp:Panel id="pnlContent" runat="server">
    <fieldset>
        <legend>Audit Details</legend>
        <asp:Panel ID="pnlEntered" runat="server">
            <cc1:TextBoxEx ID="txtEnteredBy" runat="server" LabelText="Entered By" LabelWidth="12em" IsReadOnly="true"></cc1:TextBoxEx>
            <br /><br />
            <cc1:TextBoxEx ID="txtDateEntered" runat="server" LabelText="Date Entered" LabelWidth="12em" IsReadOnly="true"></cc1:TextBoxEx>
            <br /><br />
        </asp:Panel>
        <asp:Panel ID="pnlAmended" runat="server">
            <cc1:TextBoxEx ID="txtLastAmendedBy" runat="server" LabelText="Last Amended By" LabelWidth="12em" IsReadOnly="true"></cc1:TextBoxEx>
            <br /><br />
            <cc1:TextBoxEx ID="txtDateLastAmended" runat="server" LabelText="Date Last Amended" LabelWidth="12em" IsReadOnly="true"></cc1:TextBoxEx>
            <br /><br />
        </asp:Panel>
    </fieldset>
    <br />
</asp:Panel>