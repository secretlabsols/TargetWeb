<%@ Page Language="vb" AutoEventWireup="false" Codebehind="EditPayment.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.DPContracts.EditPayment" EnableViewState="true" MasterPageFile="~/popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceFinanceCode" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.ascx" %>

	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <cc1:CollapsiblePanel ID="cp" runat="server" Expanded="false" MaintainClientState="true">
            <ContentTemplate>
                                
                <asp:Panel id="pnlPayment" runat="server">
                    <div style="float:left; width:50%;" >
                        <uc1:StdButtons id="stdButtons1" runat="server" ></uc1:StdButtons>
                        <input type="hidden" id="hidTitle" runat="server" />
                        <input type="hidden" id="hidBudgetPeriod" runat="server" />
                        <input type="hidden" id="hidISSDS" runat="server" />
                        <input type="hidden" id="hidClientID" runat="server" />
                        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
                        <br />  
                        <div id="divExternalReference" runat="server">
                            <cc1:TextBoxEx ID="txtExternalReference" runat="server" LabelText="Reference" LabelWidth="7em" Format="TextFormat" Width="10em" IsReadOnly="true" />                       
                            <br />
                            <br />
                        </div>
                        <cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Date From" LabelWidth="7em"
                        Format="DateFormatJquery" Width="10em" Required="true" RequiredValidatorErrMsg="Please specify a payment start date" ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                    <input type="hidden" id="hidDateFrom" runat="server" />
                    </div>
                    <div style="float:right;" >
	                    <%--<fieldset id="fldPaymentsInBudgetPeriods" runat="server" style="padding-top:0em;">
                            <legend>Payments in Budget Periods</legend>--%>
                            <div style="float:left;">
                                <fieldset id="fldCurrentPeriod" runat="server" style="padding-top:0em;">
                                    <legend>Current Period</legend>
                                    <asp:Label ID="lblCurrentDateRange" runat="server" ></asp:Label>
                                    <br />
                                    <asp:Label ID="lblCurrentPaymentsHaveBeedPaid" runat="server" ></asp:Label>
                                    <br />
                                    <asp:Label ID="lblCurrentPaymentsWillBePaid" runat="server" ></asp:Label>
                                    <br />
                                </fieldset>                            
                            </div>
                            <div style="float:left; padding-left:0.5em;">
                                <fieldset id="fldNextPeriod" runat="server" style="padding-top:0em;">
                                    <legend>Next Period</legend>
                                    <asp:Label ID="lblNextDateRange" runat="server" ></asp:Label>
                                    <br />
                                    <asp:Label ID="lblNextPaymentsHaveBeedPaid" runat="server" ></asp:Label>
                                    <br />
                                    <asp:Label ID="lblNextPaymentsWillBePaid" runat="server" ></asp:Label>
                                    <br />
                                </fieldset>
                            </div>
                            
                            <div class="clearer"></div>
                            <asp:Label ID="lblOverlappingPayments" runat="server" CssClass="errorText transbg"></asp:Label>
                        <%--</fieldset>--%>
	                </div>
                    
                    <div class="clearer"></div>
                    <table id="tblDetailsSummary" cellspacing="0" cellpadding="0" width="100%">
                        <tr valign="middle">
                            <td align="left" style="width: 7em;">Frequency</td>
                            <td align="left">                                
                                <cc1:DropDownListEx ID="cboFrequency" runat="server" Width="10em" />
                                <input type="hidden" id="hidFrequency" runat="server" />
                            </td>
                            <td align="left">
                                <cc1:TextBoxEx id="txtNumPayments" runat="server" LabelText="No. Payments" LabelWidth="8em" Width="2em" MaxLength="3"></cc1:TextBoxEx>
                                <input type="hidden" id="hidNumPayments" runat="server" />
                            </td>
                            <td align="left">
                                <%--<asp:Label id="lblDateTo" runat="server" Text="Date To" Width="4.8em"></asp:Label>
                                <span id="spnDateTo" runat="server" visible="true"></span>--%>
                                <cc1:TextBoxEx ID="txtDateTo" runat="server" LabelText="Date To" LabelWidth="4.8em" Format="TextFormat" IsReadOnly="true" Width="8em" ></cc1:TextBoxEx>
                                <input type="hidden" id="hidDateTo" runat="server" />
                                
                            </td>
                            <td align="left" style="width: 7em;">End Reason</td>
                            <td align="left">
                                <cc1:DropDownListEx ID="cboEndReason" runat="server" Width="12em"></cc1:DropDownListEx>
                                <input type="hidden" id="hidEndReason" runat="server" />
                            </td>
                        </tr>
                    </table>
	                <div class="clearer"></div>	             
	                <br />
                    <fieldset id="grpBreakdown" runat="server" style="padding-top:0.75em;">
                        <legend>Payment Breakdown</legend>
                        <table class="listTable" id="tblBreakdown" cellspacing="0" cellpadding="2" width="100%">
                            <thead>
	                            <tr>
		                            <th class="header">Budget Category</th>
		                            <th class="header">Units</th>
		                            <th class="header">Measured In</th>
		                            <th class="header">Frequency</th>
		                            <th class="header">Units Paid</th>
		                            <th class="header">Unit Cost(£)</th>
		                            <th class="header">Amount (£)</th>
		                            <th class="header">&nbsp;</th>
	                            </tr>
                            </thead>
                            <tbody>
    			                <asp:PlaceHolder id="phPaymentBreakdown" runat="server"></asp:PlaceHolder>
                            </tbody>
                        </table>
                        <asp:Button id="btnAddBreakdown" runat="server" text="Add" OnClientClick="btnAddBreakdown_Click();" />
                        <br />
                        <br />
                        <asp:Panel ID="pnlLegend" runat="server"> 
                            <asp:Panel id="pnlLegendNotInSpendPlan" runat="server">
                                <asp:Literal ID="lblLegendNotInSpendPlan" runat="server" />
                            </asp:Panel> 
                            <asp:Panel id="pnlLegendInSpendPlanNotDP" runat="server">
                                <asp:Literal ID="lblLegenedInSpendPlanNotDP" runat="server" />
                            </asp:Panel>                        
                        </asp:Panel>
                    </fieldset>
                    
                    <div style="float:left;width:50%;padding-top:0.75em;">
                        <div style="float:left;">
                            <%--<asp:Label id="lblAmount"  runat="server" Text="Amount" Width="6.5em"></asp:Label>
                            <span id="spnAmount" runat="server" visible="true" style="display:inline-block; width:8em;"></span>--%>
                            
                            <cc1:TextBoxEx ID="txtAmount" runat="server" LabelText="Amount" LabelWidth="6.5em" Format="TextFormat" IsReadOnly="true" Width="8em" ></cc1:TextBoxEx>
                            <input type="hidden" id="hidAmount" runat="server" />
                        </div>
                        <div style="float:left;">
                            <asp:CheckBox ID="chkForceGross" runat="server" Text="Paid Gross" TextAlign="right" />
                            <input type="hidden" id="hidForceGross" runat="server" />
                        </div>
				        <div style="clear:both;"></div>				
                        <div style="float:left;">
                            <asp:Label id="lblStatus" AssociatedControlID="optStatusProv" runat="server" Text="Status" Width="6.1em"></asp:Label>
	                        <asp:radiobutton id="optStatusProv" groupname="grpStatus" TextAlign="right" runat="server" text="Provisional" Width="8em" />
                            <asp:radiobutton id="optStatusActive" groupname="grpStatus" TextAlign="right" runat="server" text="Active" Width="6em" />
                            <asp:radiobutton id="optStatusSuspended" groupname="grpStatus" TextAlign="right" runat="server" text="Suspended" Width="8em" />
                            <br />
                            <br />
                            <asp:Label ID="lblFinCode1" AssociatedControlID="txtFinCode1" runat="server" Text="Finance Code" Width="9em"></asp:Label>
                            <uc4:InPlaceFinanceCode id="txtFinCode1" runat="server"></uc4:InPlaceFinanceCode>
                            <input type="hidden" id="hdnFinanceCode1" runat="server" />                            
                            <br />
                        </div>
                    </div>
                    <div style="float:right;width:50%;padding-top:0.50em;">
                        <fieldset id="grpPaymentSummary" runat="server" style="padding-top:0em;">
                            <legend>Payment Summary</legend>
                            <span id="spnPaidUpTo" runat="server" visible="true"></span>
                            <input type="hidden" id="hidPaidUpTo" runat="server" />
                            <input type="hidden" id="hidPaymentsMade" runat="server" />
                            <br />
                            <br />
                            <br />
                            <br />
                        </fieldset>
                    </div>
                    <div class="clearer"></div>
                </asp:Panel>
            </ContentTemplate>
        </cc1:CollapsiblePanel>
    </asp:Content>
