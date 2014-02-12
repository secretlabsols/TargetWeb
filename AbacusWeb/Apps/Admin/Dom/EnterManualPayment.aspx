<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EnterManualPayment.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.EnterManualPayment"
	EnableViewState="true" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc4" TagName="BasicAuditDetails" Src="~/Library/UserControls/BasicAuditDetails.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to view existing and create new manual domiciliary payments.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">   
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <uc4:BasicAuditDetails id="auditDetails" runat="server"></uc4:BasicAuditDetails>
    <ajaxToolkit:TabContainer runat="server" ID="tabStrip" EnableViewState="false">
        <ajaxToolkit:TabPanel runat="server" ID="tabHeader" HeaderText="Header">
            <ContentTemplate>
                    <fieldset id="fsHeader" style="padding:0.5em;" runat="server" EnableViewstate="false">
                    <asp:Label AssociatedControlID="provider" runat="server" Text="Provider" Width="10.5em"></asp:Label>
		            <uc2:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders"></uc2:InPlaceEstablishment>
		            <br />
		            <asp:Label AssociatedControlID="domContract" runat="server" Text="Contract" Width="10.5em"></asp:Label>
		            <uc3:InPlaceDomContract id="domContract" runat="server"></uc3:InPlaceDomContract>
		            <br />
                    <cc1:DropDownListEx ID="cboPeriod" runat="server" LabelText="Contract Period" LabelWidth="11em"
                    Required="true" RequiredValidatorErrMsg="Please select a contract period" ValidationGroup="Save"></cc1:DropDownListEx>
                    <br />
                    <cc1:DropDownListEx ID="cboSystemAccount" runat="server" LabelText="System Account" LabelWidth="11em"
                        Required="true" RequiredValidatorErrMsg="Please select a system account" ValidationGroup="Save"></cc1:DropDownListEx>
                    <br />
                    <cc1:TextBoxEx ID="dteWeekending" runat="server" LabelText="Week Ending" LabelWidth="11em"
				            Required="true" RequiredValidatorErrMsg="Please enter a week ending date" Format="DateFormat"
				            ValidationGroup="Save"></cc1:TextBoxEx>
		            <br />
		            <cc1:TextBoxEx ID="txtPaymentRef" runat="server" LabelText="Payment Reference" LabelWidth="11em" MaxLength="50"></cc1:TextBoxEx>
		            <br />
                    <cc1:TextBoxEx ID="txtInvoiceTotal" runat="server" LabelText="Invoice Total" LabelWidth="11em" IsReadOnly="true"></cc1:TextBoxEx>
                </fieldset>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabDetails" HeaderText="Details">
            <ContentTemplate>
                <fieldset id="fsDetails" style="padding:0.5em;" runat="server" EnableViewstate="false">
                    <table class="listTable" id="tblDetails" cellspacing="0" cellpadding="2" summary="List of invoice details" width="100%">
                    <caption>List of invoice details</caption>
                    <thead>
                        <tr>
                            <th style="width:10%;vertical-align:bottom;">Rate<br />Category</th>
                            <th style="width:70%;vertical-align:bottom;">Comment</th>
                            <th style="width:5%;vertical-align:bottom;">Units</th>
                            <th style="width:5%;vertical-align:bottom;">Unit<br />Cost</th>
                            <th style="width:5%;vertical-align:bottom;">Line<br />Value</th>
                            <th style="width:5%;vertical-align:bottom;">Finance<br />Code</th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>
	                    <asp:PlaceHolder ID="phDetails" runat="server"></asp:PlaceHolder>
	                    <tr>
	                        <td colspan="3"></td>
	                        <td style="vertical-align:bottom;"><asp:Label id="lblTotalDesc" runat="server" text="Total"></asp:Label></td>
	                        <td style="vertical-align:bottom;><cc1:TextBoxEx ID="lblLineValueTotal" runat="server" LabelText="" LabelWidth="0em" IsReadOnly="true"></cc1:TextBoxEx></td>
	                        <td></td>
	                    </tr>
                    </tbody>
                    </table>
                    <div>
                        <asp:Button id="btnAdd" runat="server" Text="Add" ValidationGroup="Add" OnClientClick="btnAdd_Click();" />
                    </div>
                </fieldset>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</asp:Content>