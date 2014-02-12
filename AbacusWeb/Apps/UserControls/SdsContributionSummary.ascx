<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="SdsContributionSummary.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.SdsContributionSummary" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="uc1" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<div id="divForm" runat="server">

    <div style="float : left; width : 100%; margin-bottom : 20px; vertical-align : middle;">
        <div id="divStartStopCollectionInfoWarning" runat="server">
            <asp:Label ID="lblStartStopCollectionInfoWarning" runat="server" CssClass="warningText" />
            <br />
            <br />
        </div>
        <asp:Image ID="imgStartStopCollectionInfo" runat="server" />
        <asp:Literal ID="lblStartStopCollectionInfo" runat="server" Text="Contributions are being collected" />
        &nbsp;
        <asp:Button ID="btnStartStopCollection" runat="server" Text="Stop Collecting" />       
    </div>

    <div class="clearer"></div>

    <div style="float : left; margin-right : 10px; margin-bottom : 20px;">
        <div>
            <asp:Label ID="lblConsideredUpTo" runat="server" Text="Considered up to" />
            &nbsp;
            <asp:Label ID="lblConsideredUpToValue" runat="server" Text="&nbsp;" Width="7.0em" CssClass="boxedLabel" />
        </div>
        <br />
        <div>    
            <asp:Label ID="lblNettedOffDirectPaymentsValue" runat="server" Text="&nbsp;" Width="7.0em" CssClass="boxedLabel" />
            <%--<asp:Label ID="lblNettedOffDirectPayments" runat="server" Text="netted off direct payments" />--%>
            &nbsp;
            <uc1:ReportsButton id="ctlNettedOffDP" ButtonText="netted off direct payments" ShowAsLink="true" runat="server"></uc1:ReportsButton>
        </div>
        <br />
        <div>    
            <asp:Label ID="lblNotYetCollectedConsideredValue" runat="server" Text="&nbsp;" Width="7.0em" CssClass="boxedLabel" />
            &nbsp;
            <%--<asp:Label ID="lblNotYetCollectedConsidered" runat="server" Text="not yet collected" />--%>
            <uc1:ReportsButton id="ctlNotYetCollectedConsidered" ButtonText="not yet collected" ShowAsLink="true" runat="server"></uc1:ReportsButton>
        </div>
    </div>

    <div style="float : left; margin-right : 10px; margin-bottom : 20px;">
        <div>
            <asp:Label ID="lblInvoicedUpTo" runat="server" Text="Invoiced up to" />
            &nbsp;
            <asp:Label ID="lblInvoicedUpToValue" runat="server" Text="&nbsp;" Width="7.0em" CssClass="boxedLabel" />
        </div>
        <br />
        <div>    
            <asp:Label ID="lblRaisedViaInvoiceValue" runat="server" Text="&nbsp;" Width="7.0em" CssClass="boxedLabel" />
            &nbsp;
            <%--<asp:Label ID="lblRaisedViaInvoice" runat="server" Text="raised via Invoice" />--%>
            <uc1:ReportsButton id="ctlRaisedViaInvoice" ButtonText="raised via Invoice" ShowAsLink="true" runat="server"></uc1:ReportsButton>
        </div>
        <br />
        <div>    
            <asp:Label ID="lblNotYetCollectedInvoicedValue" runat="server" Text="&nbsp;" Width="7.0em" CssClass="boxedLabel" />
            &nbsp;
            <%--<asp:Label ID="lblNotYetCollectedInvoiced" runat="server" Text="not yet collected" />--%>
            <uc1:ReportsButton id="ctlNotYetCollectedInvoiced" ButtonText="not yet collected" ShowAsLink="true" runat="server"></uc1:ReportsButton>
        </div>
    </div>

    <div style="float : left; margin-right : 10px; margin-bottom : 20px;">    
        <div style="height : 1.5em"></div>
        <br />
        <div>
            <asp:Label ID="lblNotYetPaidValue" runat="server" Text="" Width="7.0em" CssClass="boxedLabel" />
            &nbsp;            
            <%--<asp:Literal ID="lblNotYetPaid" runat="server" Text="of which has <u>not</u> yet been paid" />--%>
            <uc1:ReportsButton id="ctlNotYetPaid" ButtonText="of which has not yet been paid" ShowAsLink="true" runat="server"></uc1:ReportsButton>
        </div>
    </div>

    <div class="clearer"></div>

</div>

<div id="divError" runat="server">

    <asp:Label ID="lblError" runat="server" CssClass="errorText" />

</div>







