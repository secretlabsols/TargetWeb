<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="View.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.DPPayments.View" EnableEventValidation="false" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="BasicAuditDetails" Src="~/Library/UserControls/BasicAuditDetails.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:content contentplaceholderid="MPPageOverview" runat="server">
	
	This screen allows you to view direct payments.
	
</asp:content>

<asp:content id="Content3" contentplaceholderid="MPPageError" runat="server">
    
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:content>

<asp:content id="Content4" contentplaceholderid="MPContent" runat="server">
       
    <asp:Panel ID="pnlForm" runat="server">
        
        <uc1:StdButtons id="stdButtons1" runat="server" />
        <uc2:BasicAuditDetails id="auditDetails" runat="server" />
        
        <ajaxToolkit:TabContainer runat="server" ID="tbTabs" EnableViewState="true">
            <ajaxToolkit:TabPanel runat="server" ID="tpHeader" HeaderText="Header">
                <ContentTemplate>  
                    <br />  
                    <cc1:TextBoxEx ID="tbeContractNumber" runat="server" LabelText="Contract Number:" LabelWidth="14em" IsReadOnly="true" />  
                    <br />
                    <cc1:TextBoxEx ID="tbeSvcUserRefAndName" runat="server" LabelText="Service User:" LabelWidth="14em" IsReadOnly="true" />  
                    <br />
                    <cc1:TextBoxEx ID="tbeBudgetHolderRefAndName" runat="server" LabelText="Budget Holder:" LabelWidth="14em" IsReadOnly="true" />  
                    <br />
                    <cc1:TextBoxEx ID="tbeDateFrom" runat="server" LabelText="Date From:" LabelWidth="14em" IsReadOnly="true" />  
                    <br />
                    <cc1:TextBoxEx ID="tbeDateTo" runat="server" LabelText="Date To:" LabelWidth="14em" IsReadOnly="true" />  
                    <br />
                    <cc1:TextBoxEx ID="tbePaymentNumber" runat="server" LabelText="Payment Number:" LabelWidth="14em" IsReadOnly="true" />  
                    <br />
                    <cc1:TextBoxEx ID="tbeStatusAndStatusDate" runat="server" LabelText="Status &amp; Status Date:" LabelWidth="14em" IsReadOnly="true" />                                                   
                    <br />                   
                </ContentTemplate>
	        </ajaxToolkit:TabPanel>
	        <ajaxToolkit:TabPanel runat="server" ID="tpDetails" HeaderText="Details">
                <ContentTemplate> 
                    <br />                                               
                    <asp:GridView ID="gvDetails" runat="server" AutoGenerateColumns="false" CssClass="listTable" CellPadding="2" CellSpacing="0" Width="100%" Border="0"  EmptyDataText="There are currently no payment details." Caption="List of payment detail lines">
                        <Columns>
                            <asp:BoundField HeaderText="Date From" DataField="DateFrom" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" ItemStyle-Width="15%" />
                            <asp:BoundField HeaderText="Date To" DataField="DateTo" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" ItemStyle-Width="15%" />
                            <asp:BoundField HeaderText="Text" DataField="TextLine" ItemStyle-Width="55%" />
                            <asp:BoundField HeaderText="Value" DataField="LineValue" DataFormatString="{0:n}" ItemStyle-Width="15%" />
                        </Columns>
                    </asp:GridView>
                    <asp:Label id="lblNoDetails" runat="server" CssClass="content" />
                    <br />
                </ContentTemplate>
	        </ajaxToolkit:TabPanel>
	        <ajaxToolkit:TabPanel runat="server" ID="tpSummary" HeaderText="Summary">
                <ContentTemplate>   
                    <br />
                    <cc1:TextBoxEx ID="tbeTotalGrossPayments" runat="server" LabelText="Total Gross Payments:" LabelWidth="20em" IsReadOnly="true" />  
                    <br />     
                    <cc1:TextBoxEx ID="tbeTotalServiceUserContributions" runat="server" LabelText="Total Service User Contributions:" LabelWidth="20em" IsReadOnly="true" />  
                    <br />      
                    <cc1:TextBoxEx ID="tbeNetPayment" runat="server" LabelText="Net Payment:" LabelWidth="20em" IsReadOnly="true" />  
                    <br />      
                </ContentTemplate>
	        </ajaxToolkit:TabPanel>
	    </ajaxToolkit:TabContainer>
       
        <br />  
    
    </asp:Panel>
        
</asp:content>
