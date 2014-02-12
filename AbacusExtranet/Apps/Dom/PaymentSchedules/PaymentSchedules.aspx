<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PaymentSchedules.aspx.vb"
    Inherits="Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.PaymentSchedules" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceProviderSelector" Src="~/AbacusExtranet/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceContractSelector" Src="~/AbacusExtranet/Apps/InPlaceSelectors/InPlaceContractSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="AddProformaInvoice" Src="~/AbacusExtranet/Apps/UserControls/EnterCopyInvoiceButton.ascx" %>
<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ContentPlaceHolderID="MPContent" runat="server">
    <div style="height: 3.0em;">
        <div style="float: left;">
            <uc1:StdButtons id="stdButtons1" runat="server" OnCancelClientClick="return btnCancel_Click();"
                OnBackClientClick="return btnBack_Click();" OnDeleteClientClick="return btnDelete_Click();">
            </uc1:StdButtons>
        </div>
        <div style="float: left; margin-left: 0.2em;">
            <asp:Panel ID="pnlReferences" runat="server">
                <fieldset style="padding: 0.2em;">
                    <input runat="server" type="button" style="width: 7em;" value="References" id="btnEditInvoiceReferences"
                        name="btnEditInvoiceReferences" onclick="btnEditInvoiceReferences_click();" />
                </fieldset>
            </asp:Panel>
        </div>
        <div class="clearer">
        </div>
    </div>
    <div class="clearer">
    </div>
    <asp:Panel runat="server" ID="pnlPaymentSchedules">
        <fieldset>
            <legend>Summary </legend>
            <table border="0">
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Provider" Width="11em"></asp:Label>
                        <uc2:InPlaceProviderSelector runat="server" ID="ProviderSelector">
                        </uc2:InPlaceProviderSelector>
                        <asp:HiddenField runat="server" ID="txtProviderId"></asp:HiddenField>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Contract" Width="11em"></asp:Label>
                        <uc3:InPlaceContractSelector runat="server" ID="ContractSelector">
                        </uc3:InPlaceContractSelector>
                        <asp:HiddenField runat="server" ID="txtContractId"></asp:HiddenField>
                    </td>
                </tr>
                <tr>
                    <td>
                        <cc1:TextBoxEx ID="txtReference" runat="server" LabelText="Reference" Format="TextFormat"
                            LabelWidth="11.35em" EnableViewState="true" Required="true" RequiredValidatorErrMsg="Please enter a reference"
                            ValidationGroup="Save" Width="25.8em">
                        </cc1:TextBoxEx>
                    </td>
                </tr>
                <tr>
                    <td>
                        <cc1:TextBoxEx ID="dtePaymentFrom" runat="server" LabelText="Payment From" Format="DateFormatJquery"
                            LabelWidth="11.35em" EnableViewState="true" Required="true" RequiredValidatorErrMsg="Please enter a payment from date"
                            ValidationGroup="Save" OutputBrAfter="false" Width="7em" AllowClear="true">
                        </cc1:TextBoxEx>
                        &nbsp;&nbsp;
                        <cc1:TextBoxEx ID="dtePaymentTo" runat="server" LabelText="To" Format="DateFormatJquery"
                            LabelWidth="2.5em" EnableViewState="true" Required="true" RequiredValidatorErrMsg="Please enter a payment to date"
                            ValidationGroup="Save" OutputBrAfter="false" Width="7em" AllowClear="true">
                        </cc1:TextBoxEx>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div style="margin-top: 4px;">
                            <asp:Label runat="server" Text="Visit Based" Width="10.7em"></asp:Label>
                            <asp:CheckBox runat="server" ID="chkVisitBased" Enabled="false"></asp:CheckBox>
                            <input type="hidden" id="hidVisitBased" runat="server" value="false" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel runat="server" ID="pnlVoidContract">
                            <div style="margin-top: 6px;">
                                <asp:Label ID="VoidPaymentMade" runat="server" Text="Void Payment Made" Width="11.35em"></asp:Label>
                                <asp:Label ID="lblVoidPaymentMade" runat="server" Text="" ></asp:Label>
                            </div>
                            <div style="margin-top: 9px;">
                                <asp:Label ID="lblVoidPaymentDue" runat="server" Text="Void Payment Due" Width="11.35em"></asp:Label>
                                <asp:HyperLink ID="lnkVoidPaymentDue" runat="server" onclick="javascript:lnkPaymentDue_click();" ></asp:HyperLink>
                            </div>
                            <div style="margin-top: 9px;">
                                 <asp:Label ID="AgreedAmount" runat="server" Text="Agreed Payment" Width="11.35em"></asp:Label>
                                 <asp:Label ID="lblAgreedAmount" runat="server" Text="" ></asp:Label>
                            </div>
                        </asp:Panel>
                        <div style="margin-top: 8px;">
                            <cc1:TextBoxEx ID="txtTotalValue" runat="server" LabelText="Total Value" Format="CurrencyFormat"
                                LabelWidth="11.7em" Width="5em" EnableViewState="true" IsReadOnly="true">
                            </cc1:TextBoxEx>
                        </div>
                    </td>
                </tr>
            </table>
        </fieldset>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlUnProcessedInvoices" Style="margin-top: 5px;">
        <fieldset>
            <legend>Unprocessed Pro forma Invoices</legend>
            <div style="width: 425px; float: left;">
                <asp:CheckBox runat="server" Checked="true" ID="chkProformaInvoiceAwaiting"></asp:CheckBox>
                <strong>
                    <asp:Label ID="lblProformaInvoiceAwaitingCount" runat="server" Text="n"></asp:Label>&nbsp;</strong>
                Invoices 'Awaiting Verification', totalling <strong>£<asp:Label ID="lblProformaInvoiceAwaitingAmount"
                    runat="server" Text="000.0"></asp:Label></strong>
                <br />
                <br />
                <asp:CheckBox runat="server" Checked="true" ID="chkProformaInvoiceVerified"></asp:CheckBox>
                <strong>
                    <asp:Label ID="lblProformaInvoiceVerifiedCount" runat="server" Text="n"></asp:Label>&nbsp;</strong>
                'Verified' Invoices, totalling <strong>£<asp:Label ID="lblProformaInvoiceVerifiedAmount"
                    runat="server" Text="000.0"></asp:Label></strong>
            </div>
            <div style="width: 126px; float: left;">
                <input runat="server" type="button" style="width: 5em;" value="List" id="btnProformaInvoiceList"
                    name="btnProformaInvoiceList" onclick="btnProformaInvoiceList_Click();" />
                <br />
                <br />
                <uc4:AddProformaInvoice runat="server" ID="AddProformaInvoice1" />
                <input type="button" style="width: 5em;" value="Add" id="btnAddNonVisitBasedInvoices"
                    name="btnAddNonVisitBasedInvoices" />
            </div>
            <div class="clearer">
            </div>
        </fieldset>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlProviderInvoices" Style="margin-top: 5px;">
        <fieldset>
            <legend>Provider Invoice</legend>
            <div style="width: 425px; float: left;">
                <asp:CheckBox runat="server" Checked="true" ID="chkProviderInvoiceUnpaid"></asp:CheckBox>
                <strong>
                    <asp:Label ID="lblProviderInvoiceUnpaidCount" runat="server" Text="n"></asp:Label>&nbsp;</strong>
                'Unpaid' Invoices, totalling <strong>£<asp:Label ID="lblProviderInvoiceUnpaidAmount"
                    runat="server" Text="000.0"></asp:Label></strong>
                <br />
                <br />
                <asp:CheckBox runat="server" Checked="true" ID="chkProviderInvoiceSuspended"></asp:CheckBox>
                <strong>
                    <asp:Label ID="lblProviderInvoicesuspendedCount" runat="server" Text="n"></asp:Label>&nbsp;</strong>
                'Suspended' Invoices, totalling <strong>£<asp:Label ID="lblProviderInvoiceSuspendedAmount"
                    runat="server" Text="000.0"></asp:Label></strong>
                <br />
                <br />
                <asp:CheckBox runat="server" Checked="true" ID="chkProviderInvoiceAuthorised"></asp:CheckBox>
                <strong>
                    <asp:Label ID="lblProviderInvoiceAuthorisedCount" runat="server" Text="n"></asp:Label>&nbsp;</strong>
                'Authorised' Invoices, totalling <strong>£<asp:Label ID="lblProviderInvoiceAuthorisedAmount"
                    runat="server" Text="000.0"></asp:Label></strong>
                <br />
                <br />
                <asp:CheckBox runat="server" Checked="true" ID="chkProviderInvoicePaid"></asp:CheckBox>
                <strong>
                    <asp:Label ID="lblProviderInvoicePaidCount" runat="server" Text="n"></asp:Label>&nbsp;</strong>
                'Paid' Invoices, totalling <strong>£<asp:Label ID="lblProviderInvoicePaidAmount"
                    runat="server" Text="000.0"></asp:Label></strong>
            </div>
            <div style="width: 126px; float: left;">
                <input runat="server" type="button" style="width: 5em;" value="List" id="btnProviderInvoiceList"
                    name="btnProviderInvoiceList" onclick="btnProviderInvoiceList_Click();" />
            </div>
            <div class="clearer">
            </div>
        </fieldset>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlUnProcessedVisitamendmentRequest" Style="margin-top: 5px;">
        <fieldset>
            <legend>Unprocessed Visit Amendment Request</legend>
            <div style="width: 425px; float: left;">
                <asp:CheckBox runat="server" Checked="true" ID="chkVisitAmendmentAwaitingVerification">
                </asp:CheckBox>
                <strong>
                    <asp:Label ID="lblVisitAmendmentAwaitingVerificationCount" runat="server" Text="n"></asp:Label>&nbsp;</strong>
                Amendment Requests 'Awaiting Verification'
                <br />
                <br />
                <asp:CheckBox runat="server" Checked="true" ID="chkVisitAmendmentVerified"></asp:CheckBox>
                <strong>
                    <asp:Label ID="lblVisitAmendmentVerifiedCount" runat="server" Text="n"></asp:Label>&nbsp;</strong>
                'Verified' Amendment Requests
            </div>
            <div style="width: 126px; float: left;">
                <input runat="server" type="button" style="width: 5em;" value="List" id="btnVisitAmendmentList"
                    name="btnVisitAmendmentList" onclick="btnVisitAmendmentList_Click();" />
            </div>
            <div class="clearer">
            </div>
        </fieldset>
    </asp:Panel>
    <asp:HiddenField runat="server" ID="EnableDisableContractSelector"></asp:HiddenField>
    <asp:HiddenField runat="server" ID="hidPaymentScheduleId"></asp:HiddenField>
    <asp:HiddenField runat="server" ID="OriginalValueChanged" Value="false"></asp:HiddenField>
    <asp:HiddenField runat="server" ID="RequestMode"></asp:HiddenField>
    <div id="invoiceRef">
        <%--<div style="overflow:auto;height:28.5em;width:61em;border: 1px solid #666;">--%>
        <label id="lblFilterCriteria" class="errorText DialogTableFiltering">
        </label>
        <table id="tblInvoices" class="listTable" cellspacing="0" cellpadding="2" summary="List of Invoices"
            width="97%" border="0">
            <thead>
                <tr>
                    <th filtertabletype="TextBox" style="width: 20%">
                        Service User Ref
                    </th>
                    <th filtertabletype="TextBox" style="width: 20%">
                        Service User Name
                    </th>
                    <th filtertabletype="DropDown" style="width: 15%">
                        Type
                    </th>
                    <th filtertabletype="TextBox" style="width: 22%">
                        Invoice Number
                    </th>
                    <th style="width: 20%">
                        Payment Ref
                    </th>
                    <th filtertabletype="Custom" style="width: 3%">
                    </th>
                </tr>
            </thead>
            <tbody id="tblBody">
                <%--Table rows are added Dynamically--%>
            </tbody>
        </table>
        <%-- </div>--%>
    </div>
</asp:Content>
