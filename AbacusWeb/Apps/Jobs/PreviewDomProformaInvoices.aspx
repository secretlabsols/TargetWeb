<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PreviewDomProformaInvoices.aspx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.PreviewDomProformaInvoices" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">

        <div class="main">
            <div class="header">
                <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this remittance." onclick="window.close();" />
		        <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this remittance." onclick="window.print();" />
		        <div class="clearer"></div>
		        <hr />
            </div>
		    
            <h3>Create Provider Invoices - Preview of Pro forma Invoices</h3>
            
            This list below displays the pro forma invoices that currently match the filter criteria you have entered.
            These filters will be applied at the time the job runs to gather the pro forma invoices that will be processed to
            create provider invoices.
            <br /><br />
            
            <fieldset>
                <legend>Filters</legend>
                <cc1:TextBoxEx ID="txtBatchTypes" runat="server" LabelText="Batch Types" LabelWidth="10em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtProvider" runat="server" LabelText="Provider" LabelWidth="10em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtContractType" runat="server" LabelText="Contract Type" LabelWidth="10em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtContractGroup" runat="server" LabelText="Contract Group" LabelWidth="10em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtContract" runat="server" LabelText="Contract" LabelWidth="10em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
                <br />
            </fieldset>
            <br />
            <cc1:TextBoxEx ID="txtNow" runat="server" LabelText="Current Date/Time" LabelWidth="12em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
            <br />
            
            <asp:Repeater id="rptInvoices" runat="server">
		        <HeaderTemplate>
                    <table class="listTable" cellpadding="2" cellspacing="0" width="100%" summary="List of pro forma invoices.">
                    <caption>List of pro forma invoices.</caption>
                    <thead>
	                    <tr>
		                    <th>Provider</th>
		                    <th>Contract</th>
		                    <th>Service User</th>
		                    <th>Period From</th>
		                    <th>Period To</th>
		                    <th>Payment</th>
		                    <th>Contract Type</th>
		                    <th>Contract Group</th>
		                    <th>Batch Type</th>
	                    </tr>
                    </thead>
                    <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td valign="top"><%# DataBinder.Eval(Container.DataItem, "ProviderName") %></td>
                        <td valign="top"><%# DataBinder.Eval(Container.DataItem, "ContractNumber") %></td>
                        <td valign="top"><%# DataBinder.Eval(Container.DataItem, "ClientName") %></td>
                        <td valign="top"><%#Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "WEFrom")).ToString("dd/MM/yyyy")%></td>
                        <td valign="top"><%#Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "WETo")).ToString("dd/MM/yyyy")%></td>
                        <td valign="top"><%# GetCalculatedPayment(DataBinder.Eval(Container.DataItem, "CalculatedPayment")) %>&nbsp;</td>
                        <td valign="top"><%# DataBinder.Eval(Container.DataItem, "ContractType") %></td>
                        <td valign="top"><%# DataBinder.Eval(Container.DataItem, "ContractGroup") %>&nbsp;</td>
                        <td valign="top"><%# DataBinder.Eval(Container.DataItem, "BatchType") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
			        </tbody>
                    </table>
		        </FooterTemplate>
	        </asp:Repeater>
            
        </div>

    </asp:Content>