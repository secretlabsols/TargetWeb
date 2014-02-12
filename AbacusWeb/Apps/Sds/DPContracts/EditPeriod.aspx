<%@ Page Language="vb" AutoEventWireup="false" Codebehind="EditPeriod.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.DPContracts.EditPeriod" MasterPageFile="~/popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceTeam" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceTeamSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClientGroup" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientGroupSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceFinanceCode" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <cc1:CollapsiblePanel ID="cp" runat="server" Expanded="false" MaintainClientState="true">
            <ContentTemplate>
                <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
                <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
                <div style="clear:both;"></div>
                <asp:Panel id="pnlPeriod" runat="server">
                    <div style="float:left;">
                        <cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Date From" LabelWidth="13.5em"
                            Format="DateFormat" Required="true" RequiredValidatorErrMsg="Please specify a period start date" ValidationGroup="AddPeriod"></cc1:TextBoxEx>
                        <br />
                    </div>
                    <div style="float:left;">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label id="lblGrossNet" AssociatedControlID="cboGrossNet" runat="server" Text="Gross/Net" Width="8em"></asp:Label>	
                        <cc1:DropDownListEx ID="cboGrossNet" runat="server" Width="6em"></cc1:DropDownListEx>
                    </div>
                    <div style="float:left;">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label id="lblDateTo" AssociatedControlID="txtDateTo" runat="server" Text="Date To" Width="9em"></asp:Label>	
                        <cc1:TextBoxEx id="txtDateTo" runat="server" IsReadOnly="true" Format="DateFormat"></cc1:TextBoxEx>
                    </div>
				    <div style="clear:both;"></div>				
                    <input type="hidden" id="hidDateTo" runat="server" />
                    <div style="float:left;">
	                    <asp:Label ID="lblTeam" AssociatedControlID="team" runat="server" Text="Team" Width="13.1em"></asp:Label>
	                    <uc2:InPlaceTeam id="team" runat="server"></uc2:InPlaceTeam>
	                    <br />
	                </div>
                    <div style="float:left;">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
	                    <asp:Label ID="lblFinCode1" AssociatedControlID="txtFinCode1" runat="server" Text="Finance Code 1" Width="9em"></asp:Label>
	                    <uc4:InPlaceFinanceCode id="txtFinCode1" runat="server"></uc4:InPlaceFinanceCode>
                    </div>
				    <div style="clear:both;"></div>				
                    <div style="float:left;">
	                    <asp:Label ID="lblClientGroup" AssociatedControlID="clientGroup" runat="server" Text="Client Group" Width="13.1em"></asp:Label>
	                    <uc3:InPlaceClientGroup id="clientGroup" runat="server"></uc3:InPlaceClientGroup>
	                    <br />
	                </div>
                    <div style="float:left;">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
	                    <asp:Label ID="lblFinCode2" AssociatedControlID="txtFinCode2" runat="server" Text="Finance Code 2" Width="9em"></asp:Label>
	                    <uc4:InPlaceFinanceCode id="txtFinCode2" runat="server"></uc4:InPlaceFinanceCode>
	                </div>
				    <div style="clear:both;"></div>				
                </asp:Panel>
            </ContentTemplate>
        </cc1:CollapsiblePanel>
    </asp:Content>
