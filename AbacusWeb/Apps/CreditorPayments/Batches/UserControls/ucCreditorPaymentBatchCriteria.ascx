<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ucCreditorPaymentBatchCriteria.ascx.vb" Inherits="Target.Abacus.Web.Apps.CreditorPayments.Batches.ucCreditorPaymentBatchCriteria" %>

<fieldset>
    <legend>Creditor Payment Batch</legend>
    <asp:Label id="lblCreatedDate" runat="server" Text="Created" AssociatedControlID="lblCreatedDateValue" CssClass="ucCreditorPaymentBatchCriteriaLabel" />
    <asp:Label id="lblCreatedDateValue" runat="server" CssClass="content" />
    <br />
    <asp:Label id="lblCreatedBy" runat="server" Text="Created By" AssociatedControlID="lblCreatedByValue" CssClass="ucCreditorPaymentBatchCriteriaLabel" />
    <asp:Label id="lblCreatedByValue" runat="server" CssClass="content" />
    <br />
    <asp:Label id="lblPaymentCount" runat="server" Text="Payment Count" AssociatedControlID="lblPaymentCountValue" CssClass="ucCreditorPaymentBatchCriteriaLabel" />
    <asp:Label id="lblPaymentCountValue" runat="server" CssClass="content" />
    <br />
    <asp:Label id="lblPaymentValueNet" runat="server" Text="Net Value" AssociatedControlID="lblPaymentValueNetValue" CssClass="ucCreditorPaymentBatchCriteriaLabel" />
    <asp:Label id="lblPaymentValueNetValue" runat="server" CssClass="content" />
    <br />
    <asp:Label id="lblPaymentValueVAT" runat="server" Text="VAT" AssociatedControlID="lblPaymentValueVATValue" CssClass="ucCreditorPaymentBatchCriteriaLabel" />
    <asp:Label id="lblPaymentValueVATValue" runat="server" CssClass="content" />
    <br />
    <asp:Label id="lblPaymentValueGross" runat="server" Text="Total Value" AssociatedControlID="lblPaymentValueGrossValue" CssClass="ucCreditorPaymentBatchCriteriaLabel" />
    <asp:Label id="lblPaymentValueGrossValue" runat="server" CssClass="content" />
    <br />
    <asp:Label id="lblPostingDate" runat="server" Text="Posting Date" AssociatedControlID="lblPostingDateValue" CssClass="ucCreditorPaymentBatchCriteriaLabel" />
    <asp:Label id="lblPostingDateValue" runat="server" CssClass="content" />
    <br />
    <asp:Label id="lblPostingYear" runat="server" Text="Posting Year" AssociatedControlID="lblPostingYearValue" CssClass="ucCreditorPaymentBatchCriteriaLabel" />
    <asp:Label id="lblPostingYearValue" runat="server" CssClass="content" />
    <br />
    <asp:Label id="lblPeriodNum" runat="server" Text="Period Number" AssociatedControlID="lblPeriodNumValue" CssClass="ucCreditorPaymentBatchCriteriaLabel" />
    <asp:Label id="lblPeriodNumValue" runat="server" CssClass="content" />
    <br />   
</fieldset>
