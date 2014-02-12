<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PreviewAttendanceRegisters.aspx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.PreviewAttendanceRegisters" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">

        <div class="main">
            <div class="header">
                <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this preview screen." onclick="window.close();" />
		        <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this preview screen." onclick="window.print();" />
		        <div class="clearer"></div>
		        <hr />
            </div>
		    
            <h3>Create Domiciliary Provider Invoices From Service Registers - Preview of Service Registers</h3>
            
            The list below displays the service registers that currently match the filter criteria you have entered.
            These filters will be applied at the time the job runs to gather the service registers that will be processed to
            create provider invoices.
            <br /><br />
            
            <fieldset>
                <legend>Filters</legend>
                <cc1:TextBoxEx ID="txtNewRegister" runat="server" LabelText="New Registers" LabelWidth="12em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtAmendedRegister" runat="server" LabelText="Amended Registers" LabelWidth="12em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtProvider" runat="server" LabelText="Provider" LabelWidth="12em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtContract" runat="server" LabelText="Contract" LabelWidth="12em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
                <br />
            </fieldset>
            <br />
            <cc1:TextBoxEx ID="txtNow" runat="server" LabelText="Current Date/Time" LabelWidth="12em" LabelBold="true" IsReadOnly="true"></cc1:TextBoxEx>
            <br />
            
            <asp:Repeater id="rptRegisters" runat="server">
		        <HeaderTemplate>
                    <table class="listTable" cellpadding="2" cellspacing="0" width="100%" summary="List .">
                    <caption>List of service registers.</caption>
                    <thead>
	                    <tr>
		                    <th>Provider Ref</th>
		                    <th>Provider Ref</th>
		                    <th>Contract Number</th>
		                    <th>Contract Title</th>
		                    <th>Week Ending</th>
	                    </tr>
                    </thead>
                    <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td valign="top"><%# DataBinder.Eval(Container.DataItem, "ProviderRef") %></td>
                        <td valign="top"><%# DataBinder.Eval(Container.DataItem, "ProviderName") %></td>
                        <td valign="top"><%# DataBinder.Eval(Container.DataItem, "ContractNo") %></td>
                        <td valign="top"><%# DataBinder.Eval(Container.DataItem, "ContractTitle") %></td>
                        <td valign="top"><%# Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "WeekEnding")).ToString("dd/MM/yyyy") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
			        </tbody>
                    </table>
		        </FooterTemplate>
	        </asp:Repeater>
            
        </div>

    </asp:Content>
