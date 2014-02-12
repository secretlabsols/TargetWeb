<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Edit.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.Edit" AspCompat="true"
    EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>
<%@ Register TagPrefix="uc5" TagName="BasicAuditDetails" Src="~/Library/UserControls/BasicAuditDetails.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>


<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
 This screen allows you to create new summary-level Non-Residential provider invoices and view/edit existing invoices.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <div style="float:left;">
        <uc1:StdButtons id="stdButtons1" runat="server"  OnBackClientClick="btnBack_Click();" ></uc1:StdButtons>
    </div>
    <div id="imgNotes"  style="float:left;padding-left:10px;padding-top:5px;">
        <%-- Add Image at run time to display notes --%>	   
	</div>
    <div class="clearer"></div>
    <asp:Label id="lblSusWarning" runat="server" CssClass="warningText" visible="false"></asp:Label>
    <uc5:BasicAuditDetails id="auditDetails" runat="server"></uc5:BasicAuditDetails>
    <ajaxToolkit:TabContainer runat="server" ID="tabStrip" OnClientActiveTabChanged="tabStrip_ActiveTabChanged" EnableViewState="false">
        <ajaxToolkit:TabPanel runat="server" ID="tabHeader" HeaderText="Header">
            <ContentTemplate>                
                <asp:Label id="lblProvider" AssociatedControlID="provider" runat="server" Text="Provider" Width="13.18em"></asp:Label>
                <uc2:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders"></uc2:InPlaceEstablishment>
                <br />                
                <asp:Label id="lblContract" AssociatedControlID="domContract" runat="server" Text="Contract" Width="13.18em"></asp:Label>
                <uc3:InPlaceDomContract id="domContract" runat="server"></uc3:InPlaceDomContract>
                <br />
                <asp:Label id="lblClient" AssociatedControlID="client" runat="server" Text="Service User" Width="13.18em"></asp:Label>
                <uc4:InPlaceClient id="client" runat="server"></uc4:InPlaceClient>
                <br />
                <div >
                    <div style="float:left;">
                        <cc1:TextBoxEx id="txtWeekEndingFrom" runat="server" Format="DateFormatJquery" LabelText="Invoice Period" LabelWidth="13.5em"
                            Required="true" RequiredValidatorErrMsg="Please enter a period start date" ValidationGroup="Save"  
                            Width="6.75em" AllowClear="true" />               
                        <asp:RangeValidator id="rvWeekEndingFrom" runat="server" ValidationGroup="Save" Display="Dynamic" EnableClientScript="True" ErrorMessage="The natural period of the invoice (Period From/To) may not cover a future date" SetFocusOnError="True" />
                    </div>
                    <div style="float:left;margin-left:20px;">
                        <cc1:TextBoxEx id="txtWeekEndingTo" runat="server" Format="DateFormatJquery" LabelText="to" LabelWidth="2em" Width="6.75em" 
                            Required="true" RequiredValidatorErrMsg="Please enter a period end date" ValidationGroup="Save" AllowClear="true" />
                        <asp:RangeValidator id="rvWeekEndingTo" runat="server" ValidationGroup="Save" Display="Dynamic" EnableClientScript="True" ErrorMessage="The natural period of the invoice (Period From/To) may not cover a future date" SetFocusOnError="True" />
                   </div>
                    <div class="clearer"></div>
                </div>
                <br />
                <div>
                    <div style="float:left;">
                        <cc1:TextBoxEx id="txtReference" runat="server" LabelText="Reference" LabelWidth="13.5em" MaxLength="50" Width="12em" />
                    </div>
                    <div style="float:left;margin-top:4px;margin-left:20px;vertical-align:baseline;" >
                        <asp:Panel id="pnlHeaderExistingInvoice" runat="server">
                            <cc1:TextBoxEx id="txtInvoiceNumber" runat="server" LabelText="Invoice Number" LabelWidth="10em" MaxLength="22" 
                                IsReadOnly="true" ReadOnlyContentCssClass="disabled" OutputBrAfter="false" />
                        </asp:Panel>
                    </div>
                </div>
                <div class="clearer"></div>
                <asp:Panel id="pnlHeaderExistingInvoiceStaus" runat="server">
                    <div style="float:left;margin-top:1.3em;">
                        <cc1:TextBoxEx id="txtStatus" runat="server" LabelText="Status" LabelWidth="13.5em" 
                                IsReadOnly="true" ReadOnlyContentCssClass="disabled"  />
                     </div>
                </asp:Panel>
               <div class="clearer"></div>
               
                <div style="float:left;margin-top:1.3em;">  
                   <cc1:TextBoxEx id="txtInvoiceDate" runat="server" Format="DateFormatJquery" LabelText="Invoice Date" LabelWidth="13.5em" Width="6.75em" 
                        Required="true" RequiredValidatorErrMsg="Please enter an invoice date" ValidationGroup="Save" AllowClear="true"></cc1:TextBoxEx>
                    <asp:RangeValidator id="rvInvoiceDate" runat="server" ValidationGroup="Save" Display="Dynamic" EnableClientScript="True" ErrorMessage="The Invoice Date may not be in the future" SetFocusOnError="True" />
                </div>
                <div class="clearer"></div>
                <div style="float:left;margin-top:1.3em;">  
                <cc1:TextBoxEx id="txtDateReceived" runat="server" Format="DateFormatJquery" LabelText="Date Received" LabelWidth="13.5em" Width="6.75em" 
                    Required="true" RequiredValidatorErrMsg="Please enter a date received" ValidationGroup="Save" AllowClear="true"></cc1:TextBoxEx>
                </div>    
                <div class="clearer"></div> 
                <div style="float:left;margin-top:1.3em;">           
                    <asp:Button id="btnCreate" runat="server" text="Create" ValidationGroup="Save" />
                </div>    
                <div class="clearer"></div>  
            </ContentTemplate>
	    </ajaxToolkit:TabPanel>
	    <ajaxToolkit:TabPanel runat="server" ID="tabDetails" HeaderText="Details">
            <ContentTemplate>
	            <asp:Panel id="pnlDetailsSummaryVisitLevel" runat="server">
                    <table class="listTable" id="tblDetailsSummary" cellspacing="0" cellpadding="2" summary="List of invoice details" width="100%">
                        <colgroup id="grpWE"></colgroup>
                        <colgroup id="grpRateCategory"></colgroup>
                        <colgroup id="grpPlannedUnits"></colgroup>
                        <colgroup id="grpPlannedRate"></colgroup>
                        <colgroup id="grpPlannedCost"></colgroup>
                        <colgroup id="grpOtherUnits"></colgroup>
                        <colgroup id="grpOtherCost"></colgroup>
                        <colgroup id="grpThisUnits"></colgroup>
                        <colgroup id="grpThisSUUnits"></colgroup>
                        <colgroup id="grpThisRate"></colgroup>
                        <colgroup id="grpThisCost"></colgroup>
		                <caption>List of invoice details</caption>
		                <thead>
		                    <tr>
		                        <td colspan="2" class="headerGroup" style="border-width:0px;">&nbsp;</td>
		                        <td colspan="3" class="headerGroup con" style="border-width:0px;">Planned</td>
		                        <td colspan="2" class="headerGroup otherInvoiceCell" style="border-width:0px;">Other Invoices</td>
		                        <td colspan="4" class="headerGroup" style="border-width:0px;">This Invoice</td>
		                        <td class="headerGroup" style="border-width:0px;">&nbsp;</td>
		                    </tr>
			                <tr>
				                <th class="header">Week Ending</th>
				                <th class="header">Rate Category</th>
				                <th class="header" style="text-align:left;">Units</th>
				                <th class="header" style="text-align:right;">Rate(£)</th>
				                <th class="header" style="text-align:right;">Cost(£)</th>
				                <th class="header" style="text-align:right;">Units</th>
				                <th class="header" style="text-align:right;">Cost(£)</th>
				                <th class="header" style="text-align:right;">Units</th>
						        <% If InvoiceIsSummaryBased() Then %>
				                <th class="header" style="text-align:right;">S/U Units</th>
						        <% End If %>
				                <th class="header" style="text-align:right;">Rate(£)</th>
				                <th class="header" style="text-align:right;">Cost(£)</th>
				                <th class="header">&nbsp;</th>
			                </tr>
		                </thead>
		                <tbody>
            			    <asp:PlaceHolder id="phDetailsSummaryVisitLevel" runat="server"></asp:PlaceHolder>
		                </tbody>
		            </table>
		            <asp:Button id="btnAddDetail" runat="server" visible="false" text="Add" onclientclick="return btnAddDetail_Click();" />
		            
		            <div id="divAddDetailDialogContentContainer" style="display:none;">
                        <div id="divAddDetailDialogContent">
                            <!-- hidden elements used in AddDetail dialog -->
                            Please enter the week ending date and rate category and rate.
                            <br /><br />
                            <cc1:TextBoxEx ID="dteAddDetailWeekEnding" runat="server" LabelText="Week Ending" LabelWidth="10em" Format="DateFormatJquery"
                                Required="true" RequiredValidatorErrMsg="Please enter a week ending date" ValidationGroup="AddDetail"></cc1:TextBoxEx>
                            <asp:RangeValidator id="rvAddDetailWeekEnding" runat="server" ValidationGroup="AddDetail" Display="Dynamic" EnableClientScript="True" ErrorMessage="Actual service for future periods may not be entered" SetFocusOnError="True" />
                            <br />
                            <cc1:DropDownListEx id="cboAddDetailRateCategory" runat="server" LabelText="Rate Category" LabelWidth="10em"
                                Required="true" RequiredValidatorErrMsg="Please select a rate category" ValidationGroup="AddDetail"></cc1:DropDownListEx>
                            <br />
                            <cc1:DropDownListEx id="cboAddDetailRate" runat="server" LabelText="Rate" LabelWidth="10em"
                                Required="true" RequiredValidatorErrMsg="Please select a rate" ValidationGroup="AddDetail"></cc1:DropDownListEx>
                            <br />
                        </div>
                    </div>
                    
                    <script type="text/javascript">
	                    function AddDetail_DoPostBack() {
		                    <asp:Literal ID="litAddDetailDoPostBackJS" runat="server" />
	                    }
                    </script>
		            
                </asp:Panel>
                <asp:Panel id="pnlDetailsManualPayment" runat="server">
                    <table class="listTable" cellspacing="0" cellpadding="2" summary="List of invoice details" width="100%">
		            <caption>List of invoice details</caption>
		            <thead>
			            <tr>
				            <th style="width:20%;">Week Ending</th>
				            <th style="width:60%;">Description</th>
				            <th style="width:20%;text-align:right;">Cost(£)</th>
			            </tr>
		            </thead>
		            <tbody>
            			<asp:PlaceHolder id="phDetailsManualPayment" runat="server"></asp:PlaceHolder>
		            </tbody>
		            </table>
                </asp:Panel>
                
                <asp:Panel id="pnlDetailsSummaryLevelExtranet" runat="server">
                    <table border="0" class="listTable" id="Table1" cellspacing="0" cellpadding="2" summary="List of invoice details" width="100%">
                    <caption>List of invoice details</caption>
                    <thead>
                        <tr>
                            <td colspan="2" class="headerGroup" style="border-width:0px;">&nbsp;</td>
                            <td colspan="3" class="headerGroup plannedCell" style="border-width:0px;width:24%" >Planned</td>
                            <% If Me.ShowNonDelivery Then%>
                            <td colspan="4" class="headerGroup" style="border-width:0px;">This Invoice</td>
                            <% Else%>
                            <td colspan="3" class="headerGroup" style="border-width:0px;">This Invoice</td>
                            <% End If%>
                            <td class="headerGroup" style="border-width:0px;">&nbsp;</td>
                        </tr>
	                    <tr>
		                    <th class="header">Week Ending</th>
		                    <th class="header">Rate Category</th>
		                    <th class="header" style="text-align:left;">Units</th>
		                    <th class="header" style="text-align:right;">Rate(£)</th>
		                    <th class="header" style="text-align:right;">Cost(£)</th>
		                    <th class="header" style="text-align:right;">Delivered</th>
		                     <% If Me.ShowNonDelivery Then%>
		                        <th class="header" style="text-align:right;">Non-Delivery</th>
		                      <% End If%>
		                    <th class="header" style="text-align:right;">Rate(£)</th>
		                    <th class="header" style="text-align:right;">Cost(£)</th>
		                    <th class="header">&nbsp;</th>
	                    </tr>
                    </thead>
                    <tbody>
    			        <asp:PlaceHolder id="phDetailsSummaryLevelExtranet" runat="server"></asp:PlaceHolder>
                    </tbody>
                    </table>
                </asp:Panel>
        
        	</ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabSummary" HeaderText="Summary">
            <ContentTemplate>
                    <div style="float: left;">
                        <div id="tabSummaryContainer" style="float: left;">
                            <asp:Panel id="pnlSummarySummaryLevel" runat="server">
                                <cc1:TextBoxEx id="txtSummaryActualCost" runat="server" LabelText="Actual Cost" Format="CurrencyFormat" LabelWidth="11.5em"
                                    Width="5em"></cc1:TextBoxEx>
		                        <br />
		                        <cc1:CheckBoxEx id="chkSummaryVat" runat="server" LabelWidth="9em" Text="VAT" />
		                        <cc1:TextBoxEx id="txtSummaryVat" runat="server" Format="CurrencyFormat" LabelText="" LabelWidth="1em"
		                            Width="5em"></cc1:TextBoxEx>
		                        <br />
		                        <div id="divSummaryPenalty" runat="server"> 
		                            <cc1:TextBoxEx id="txtSummaryPenalty" runat="server" LabelText="Penalty Payment" Format="CurrencyFormat" LabelWidth="11.5em"
		                                Width="5em"></cc1:TextBoxEx>
		                            <br />
		                        </div>		            
		                        <cc1:TextBoxEx id="txtSummaryClientContrib" runat="server" LabelText="Client Contribution" Format="CurrencyFormat" LabelWidth="11.5em"
		                            Width="5em"></cc1:TextBoxEx>
		                        <br />
		                        <cc1:TextBoxEx id="txtSummaryNetCost" runat="server" LabelText="Net Cost" Format="CurrencyFormat" LabelWidth="11.5em"
		                            Width="5em"></cc1:TextBoxEx>
		                        <br />
		                        <hr style="width: 17em; clear: none;" />
		                        <cc1:TextBoxEx id="txtSummaryInvoiceTotal" runat="server" LabelBold="true" LabelText="Invoice Total" Format="CurrencyFormat" LabelWidth="11.5em"
		                            Width="5em"></cc1:TextBoxEx>
		                        <br />
                            </asp:Panel>
                            <asp:Panel id="pnlSummaryVisitLevel" runat="server">
                                <cc1:TextBoxEx id="txtVisitActualCost" runat="server" LabelText="Actual Cost" Format="CurrencyFormat" LabelWidth="11.5em"
                                    IsReadOnly="true" ReadOnlyContentCssClass="disabled" Width="5em"></cc1:TextBoxEx>
		                        <br /><br />
		                        <cc1:TextBoxEx id="txtVisitVat" runat="server" LabelText="VAT" Format="CurrencyFormat" LabelWidth="11.5em"
		                            IsReadOnly="true" ReadOnlyContentCssClass="disabled" Width="5em"></cc1:TextBoxEx>
		                        <br /><br />
		                        <cc1:TextBoxEx id="txtVisitClientContrib" runat="server" LabelText="Client Contribution" Format="CurrencyFormat" LabelWidth="11.5em"
		                            IsReadOnly="true" ReadOnlyContentCssClass="disabled" Width="5em"></cc1:TextBoxEx>
		                        <br /><br />
		                        <cc1:TextBoxEx id="txtVisitNetCost" runat="server" LabelText="Net Cost" Format="CurrencyFormat" LabelWidth="11.5em"
		                            IsReadOnly="true" ReadOnlyContentCssClass="disabled" Width="5em"></cc1:TextBoxEx>
		                        <br />
		                        <hr style="width: 17em; clear: none;" />
		                        <cc1:TextBoxEx id="txtVisitInvoiceTotal" runat="server" LabelBold="true" LabelText="Invoice Total" Format="CurrencyFormat" LabelWidth="11.5em"
		                            IsReadOnly="true" ReadOnlyContentCssClass="disabled" Width="5em"></cc1:TextBoxEx>
		                        <br /><br />
                            </asp:Panel>
                            <asp:Panel id="pnlSummaryManualPayment" runat="server">
                                <cc1:TextBoxEx id="txtManualNetCost" runat="server" LabelText="Net Cost" Format="CurrencyFormat" LabelWidth="11.5em"
                                    IsReadOnly="true" ReadOnlyContentCssClass="disabled" Width="5em"></cc1:TextBoxEx>
		                        <br /><br />
		                        <cc1:TextBoxEx id="txtManualVat" runat="server" LabelText="VAT" Format="CurrencyFormat" LabelWidth="11.5em"
		                            IsReadOnly="true" ReadOnlyContentCssClass="disabled" Width="5em"></cc1:TextBoxEx>
		                        <br />
		                        <hr style="width: 17em; clear: none;" />
		                        <cc1:TextBoxEx id="txtManualInvoiceTotal" runat="server" LabelBold="true" LabelText="Invoice Total" Format="CurrencyFormat" LabelWidth="11.5em"
		                            IsReadOnly="true" ReadOnlyContentCssClass="disabled" Width="5em"></cc1:TextBoxEx>
		                        <br /><br />
                            </asp:Panel>
                        </div>
                        <div style="float: left; margin: 0em 0em 0em 2em;">
                            <cc1:CheckBoxEx id="chkSuspend" runat="server" LabelWidth="11em" Text="Suspension Reason" />
                            <cc1:DropDownListEx id="cboSuspensionReason" runat="server" LabelText="" LabelWidth="1em"
                                Required="true" RequiredValidatorErrMsg="Please select a suspension reason" ValidationGroup="Save"></cc1:DropDownListEx>
                            <br />
                            <cc1:TextBoxEx id="txtNotes" runat="server" LabelText="Notes" LabelWidth="13.5em" Width="50%"></cc1:TextBoxEx>
                            <br />                
                        </div>
                    </div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
	</ajaxToolkit:TabContainer>
	<input type="hidden" id="hidSelectedTab" runat="server" />
    <br />
        
          <div>
            <%--<invoiceNotesControl:invoiceNotes id="ctrlInvoiceNotes" runat="server"></invoiceNotesControl:invoiceNotes>--%>
            <asp:Panel ID="IsPopUp" runat="server">
                <div id="invNotesPopup">
                    Invoice Note:
                    <div style="border:solid 1px #7F9DB9; overflow:auto; height:260px; padding:10px;" >
                        <asp:Panel ID="pnlNotes" runat="server"></asp:Panel>
                    </div>
                    <div style="padding-top:10px;">
                        Entered on <asp:Label runat="server" id="lblDate" Text=""></asp:Label>
                        &nbsp; at&nbsp;
                        <asp:Label runat="server" id="lblat" Text=""></asp:Label>
                    </div>         
                </div>
            </asp:Panel>    
        </div>
</asp:Content>