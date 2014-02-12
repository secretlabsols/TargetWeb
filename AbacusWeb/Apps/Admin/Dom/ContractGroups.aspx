<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ContractGroups.aspx.vb"
    Inherits="Target.Abacus.Web.Apps.Admin.Dom.ContractGroups" EnableViewState="false" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different contract groups.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server">
    </uc1:StdButtons>
    <fieldset id="fsControls" style="padding: 0.5em;" runat="server">
        <div style="float: left;" class="divFieldSeperator">
            <cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description" LabelWidth="8em"
                MaxLength="255" Width="20em" Required="true" RequiredValidatorErrMsg="Please enter a description"
                SetFocus="true" ValidationGroup="Save">
            </cc1:TextBoxEx>
        </div>
        <div class="clearer">
        </div>
        <div>
            <div style="float: left;">
                <asp:Label ID="lblUsage" runat="server" AssociatedControlID="divUsage" Text="Usage"
                    Width="8em" />
            </div>
            <div id="divUsage" runat="server" style="float: left;  width: 19.5em; border: solid 1px silver; padding: 5px;">
                <cc1:CheckBoxEx ID="chkDomContracts" runat="server" Text="Non-Residential Contracts"
                    LabelWidth="15em">
                </cc1:CheckBoxEx>
                <cc1:CheckBoxEx ID="chkDPContracts" runat="server" Text="DP Contracts" LabelWidth="15em">
                </cc1:CheckBoxEx>
            </div>
        </div>
        
        <div class="clearer">
        </div>
        <br />
        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="7.75em">
        </cc1:CheckBoxEx>
    </fieldset>
    <br />
</asp:Content>
