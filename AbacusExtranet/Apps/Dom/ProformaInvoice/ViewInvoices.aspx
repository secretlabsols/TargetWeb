<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ViewInvoices.aspx.vb"  Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.ViewInvoices" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls"   TagPrefix="cc1" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc1" TagName="BreadCrumb" Src="~/AbacusExtranet/Apps/Dom/ProformaInvoice/InvoiceBatchBreadcrumb.ascx" %>
<%@ Register TagPrefix="uc2" TagName="psHeader" Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx"%>
<%@ Reference Control="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>
<%@ Register TagPrefix="uc5" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<asp:content contentplaceholderid="MPPageOverview" runat="server">
	
</asp:content>
    
<asp:content contentplaceholderid="MPContent" runat="server">
        <div>
	        <input type="button" style="width:5em;" id="btnBack" value="Back" title="Back" onclick="btnBack_Click();" />
	    </div>
	    <div style="margin-top:5px;">
	        <uc2:psHeader id="psHeader1" runat="server"></uc2:psHeader>
	    </div>
	    
	 
	    
		<%--<label class="label" for="lblProvider">Provider</label>
    	
	    <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	    <br />
		<label class="label" for="lblContract">Contract</label>
	    <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
		<br />
        <label class="label" for="lblServiceUser">Service User</label>
	    <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblStatus">Status</label>
	    <asp:Label id="lblStatus" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblInvoiceCount">Invoice Count</label>
	    <asp:Label id="lblInvoiceCount" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblNetPayment">Net Payment</label>
	    <asp:Label id="lblNetPayment" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <br />--%>
	   <div style="margin-top:5px;">
	    
	    <cc1:CollapsiblePanel ID="cpe" runat="server" HeaderLinkText="Filters" MaintainClientState="true"  >
	    <ContentTemplate>
	    <asp:Panel runat="server" ID="pnlfilters">
	        <div>
              <div style="float:left;width:80%;"> 
                    <fieldset><legend>General</legend>
                    <div>
                        <div style="float:left;">
                            <cc1:DropDownListEx ID="cboMismatches" LabelText="Payment Mismatches" LabelWidth="12em" runat="server"></cc1:DropDownListEx>
                        </div>
                        <div style="float:left;margin-left:5em;">
                            <cc1:DropDownListEx ID="cboQueried" LabelText="Notes" LabelWidth="6em" runat="server"></cc1:DropDownListEx>
                        </div>
                    </div>
                        <div class="clearer;"></div>
                        <br /><br />
                    <div >
                            <cc1:TextBoxEx id="txtTolerance" LabelText="Mismatch Tolerance" LabelWidth="12em" runat="server" Width="3em" Format="CurrencyFormat"></cc1:TextBoxEx>
                    </div>
                    <div class="clearer;"></div>
                </fieldset>
         
                    <fieldset ><legend>Duration Claimed Rounding</legend>
                    <div >
                       <asp:RadioButton ID="rdbDcrFilterDefault" runat="server" GroupName="dcrFilter" text="Do not filter by this item" Checked="true" ></asp:RadioButton><br /><br />
                       <asp:RadioButton ID="rdbDcrFilterYes" runat="server" GroupName="dcrFilter"  text="Show Provider Invoices having one or more visit with a rounded Duration Claimed"></asp:RadioButton><br /><br />
                       <asp:RadioButton ID="rdbDcrFilterNo" runat="server" GroupName="dcrFilter"  text="Show Provider Invoices where all visits do not have rounded Duration Claimed"></asp:RadioButton><br /><br />
                    </div>
               </fieldset>
           
                    <fieldset ><legend>Invoice Status</legend>
              <div style="float:left;" >
                        <input type="checkbox" id="chkAwait" value="Awaiting Verification" onclick="UpdateInvoiceStatus();" />
                        Awaiting Verification
                        </div>
              <div  style="float:left; margin-left:5em;">
                        <input type="checkbox" id="chkVer" value="Verified" onclick="UpdateInvoiceStatus();" />
                        Verified
              </div>
              <div class="clearer;" ></div>
            </fieldset>
              </div>
              
              <div style="float:left;width:20%;"> 
                    <center>
                    <input type="button" id="btnApplyFilters" name="btnApplyFilters"
                     value="Apply Filters" style="margin-left:5px; margin-top:5px;" onclick="btnApplyFilters_Click()" />
                    </center>
              </div>
            <div class="clearer">         </div>
              
       </asp:Panel>
	    </ContentTemplate>
	    </cc1:CollapsiblePanel>
	    
       </div>
       <div style="margin-top:5px;">
            <table class="listTable" id="tblInvoices" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" 
                   border="0"  width="100%" summary="List of available pro forma invoices.">
                <caption>List of available pro forma invoices.</caption>
                <thead>
	                <tr>
		                <th style="width:1.5em;"><input type="radio" name="headerRadio" title="Deselect All" onclick="UnSelectAll();" /> </th>
		                <th id="thSUName" style="width:10em;">Service User</th>
		                <th id="thSURef" style="width:8em;">S/U Reference</th>
		                <th style="width:11em;">Status</th>
		                <th style="width:6em;">Payment</th>
		                <th style="width:6em;">Payment Ref</th>
		                <th style="width:5em;">Claimed</th>
		                <th style="width:5em;">Notes</th>
	                </tr>
                </thead>
                <tbody>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                        
                    </tr>
               </tbody>
            </table>
            <div id="Invoice_PagingLinks" style="float:left;"></div>
            <div style="float:right; margin-bottom:4px;">
                
                <input type="button" style="width:7.5em;" id="btnRecalculate" runat="server" value="Recalculate" title="Recalculate the selected Invoice" onclick="btnRecalculate_Click();" />
                <%--<input type="button" style="width:9.5em;" id="btnRemove" runat="server" value="Remove" title="Remove the selected Invoice from the batch" onclick="if(!window.confirm('Are you sure you wish to remove this invoice?')) return false;" />--%>
                
                <input type="button" style="width:7.5em;" id="btnViewInvoiceLines" value="Invoice Lines" title="View the Invoice Lines of the selected Invoice" onclick="btnViewInvoiceLines_Click();" />
                <input type="button" style="width:7.5em;" id="btnViewCostedVisits" value="Costed Visits" title="View the Costed Visits of the selected Invoice" onclick="btnViewVisits_Click();" />
                <input type="button" style="width:4.7em;" id="btnVisits" value="Visits" title="View the Visits of the selected Invoice" onclick="btnVisits_Click();" />
            </div>
            <div class="clearer"></div>
            <div style="float:right;">
                <input type="button" style="width:7.5em;" id="btnDelete" value="Delete" title="Delete invoice(s)" runat="server" onclick="btnDelete_Click();" />
                <input type="button" style="width:7.5em;" id="btnVerify" value="Verify" title="Verify filtered invoice(s)" runat="server" onclick="btnVerify_Click();" />
                <input type="button" style="width:7.5em; margin-right:1px;" id="btnUnVerify" value="UnVerify" title="UnVerify selected invoice" runat="server" onclick="btnUnVerify_Click();" />
                <uc5:ReportsButton runat="server" ID="rptPrint" ButtonWidth="4.5em" ></uc5:ReportsButton>
            </div>
            <div class="clearer"></div>
        </div> 
        <input id="hidNotesType" type="hidden" value = "0" />
    </asp:content>
