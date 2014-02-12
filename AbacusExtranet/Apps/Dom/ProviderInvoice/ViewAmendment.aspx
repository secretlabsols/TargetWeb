<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewAmendment.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.ViewAmendment" %>

<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<%@ Register TagPrefix="uc1" TagName="pScheduleHeader" 
Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>
<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen displays the details of the existing amendment request and/or allows you create a new visit amendment request.
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
<asp:Content ContentPlaceHolderID="MPContent" runat="server">

    <div>
    <fieldset class="stdButtons" id="divBack" runat="server">
    <div>
           
                <input type="button" id="btnBack" runat="server" value="Back" style="width:3.7em;" />
           
    </div>
</fieldset>
        <div style="height:3.0em; float:left">
            <uc1:StdButtons id="StdButtons1" AllowFind="false" runat="server" />
        </div>
    </div>
    <div class="clearer"></div>
    <asp:Panel id="pnlRetractionPending" runat="server" CssClass="warningText" Visible="false">
        This visit cannot be amended at this time because the invoice it appears on 
        (<asp:Literal id="litRetractionPendingInvoiceNumber" runat="server"></asp:Literal>) 
        is already in the process of being amended.<br />
        Further amendments to visits on this invoice can be made once the current amendment process has been completed.
        <br /><br />
    </asp:Panel>
    
    <uc1:pScheduleHeader ID="pSchedule" runat="server"></uc1:pScheduleHeader><div class="clearer"></div>
    <label class="label" for="lblProvider">Provider</label>
    <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
    <br />
	<label class="label" for="lblContract">Contract</label>
    <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
	<br />
    <label class="label" for="lblServiceUser">Service User</label>
    <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
    <br />
    <label class="label" for="lblVisitDate">Visit Date</label>
    <asp:Label id="lblVisitDate" runat="server" CssClass="content"></asp:Label>
    <br />
    <asp:Panel id="pnlInvoicedPanel" runat="server">
        <label class="label" for="lblInvoiceNo">Provider Invoice No.</label>
        <asp:Label id="lblInvoiceNo" runat="server" CssClass="content"></asp:Label>
        <br />
    </asp:Panel>
    <br />
    <asp:Repeater id="rptCarers" runat="server">
        <HeaderTemplate>
		    <table class="listTable" id="tblCarers" style="table-layout:fixed; " cellpadding="2" cellspacing="0" summary="List of Carers.">
			    <caption>Care Workers.</caption>
			    <tr>
	                <th style="width:15em;">Reference</th>
	                <th style="width:35em;">Name</th>
                </tr>
	    </HeaderTemplate>
	    <ItemTemplate>
		    <tr>
			    <td valign="top" ><%#DataBinder.Eval(Container.DataItem, "Reference")%>&nbsp;</td>
			    <td valign="top" ><%#DataBinder.Eval(Container.DataItem, "Name")%>&nbsp;</td>
			</tr>
	    </ItemTemplate>
	    <FooterTemplate>
		    </table>
	    </FooterTemplate>
    </asp:Repeater>
    <asp:Panel id="pnlAmendmentPanel" runat="server">
        <br />
        <fieldset id="grpAmendment" runat="server" style="float:none;width:70%;">
            <legend>Amendment</legend>
            <asp:Panel id="pnlRequest" runat="server">
                <table class="listTable" id="tblAmendment" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of available domiciliary.">
                    <thead>
	                    <tr>
		                    <th style="width:15em;"></th>
		                    <th>Original Value</th>
		                    <th>Amendment</th>
	                    </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Start Time Claimed</td>
                            <td><asp:Label id="lblStartTimeClaimedOriginal" runat="server" CssClass="content"></asp:Label></td>
                            <td><cc1:TimePicker ID="txtStartTimeClaimedAmendment" ShowSeconds="false" runat=server></cc1:TimePicker></td>
                        </tr>
                        <tr>
                            <td>Duration Claimed</td>
                            <td>
                            <asp:Label id="lblDurationClaimedOriginal" runat="server" CssClass="content"></asp:Label>
                              <asp:Label id="lblPreRoundedDurationClaimed" runat="server" CssClass="warningText"></asp:Label>
                            </td>
                            <td><cc1:TimePicker ID="txtDurationClaimedAmendment"  ShowSeconds="false" runat=server></cc1:TimePicker></td>
                        </tr>
                        <tr>
                            <td>Actual Duration</td>
                            <td><asp:Label id="lblActualDurationOriginal" runat="server" CssClass="content"></asp:Label></td>
                            <td><cc1:TimePicker ID="txtActualDurationAmendment"  ShowSeconds="false" runat=server></cc1:TimePicker></td>
                        </tr>
                         <tr>
                            <td>Secondary Visit</td>
                            <td><asp:Label id="lblActualSecondaryVisit" runat="server" CssClass="content"></asp:Label></td>
                            <td><cc1:DropDownListEx ID="cboSecondaryVisit" Width="7em"  runat="server" validationgroup="Save" 
                                Required="true" RequiredValidatorErrMsg="Secondart visit type must be entered.">
                            </cc1:DropDownListEx></td>
                        </tr>
                        <tr>
                            <td>Visit Code</td>
                            <td><asp:Label id="lblVisitCodeOriginal" runat="server" CssClass="content"></asp:Label></td>
                            <td><cc1:DropDownListEx ID="cboVisitCodeAmendment"  runat="server" validationgroup="Save" Required="true" RequiredValidatorErrMsg="A visit code must be entered."></cc1:DropDownListEx></td>
                        </tr>
                        <tr>
                            <td>Service Type</td>
                            <td><asp:Label id="lblServiceTypeOriginal" runat="server" CssClass="content"></asp:Label></td>
                            <td><cc1:DropDownListEx ID="cboServiceTypeAmendment"  runat="server" validationgroup="Save" Required="true" RequiredValidatorErrMsg="A service type must be entered."></cc1:DropDownListEx></td>
                        </tr>
                    </tbody>
                </table>
                <label class="label" style="float:left;" for="txtReason">Reason</label>
                <cc1:TextBoxEx ID="txtReason"  runat=server validationgroup="Save" Required="true" RequiredValidatorErrMsg="An amendment reason must be entered." MaxLength="255" Width="75%"></cc1:TextBoxEx>
                
            </asp:Panel>
            <label class="label" for="lblStatus">Status</label>
            <asp:Label id="lblStatus" runat="server" CssClass="content"></asp:Label>
            <br />
            <asp:Panel id="pnlAmendmentDetailsPanel" runat="server">
                <label class="label" for="lblRequestDate">Request Date</label>
                <asp:Label id="lblRequestDate" runat="server" CssClass="content"></asp:Label>
                <br />
                <label class="label" for="lblRequestBy">Request By</label>
                <asp:Label id="lblRequestBy" runat="server" CssClass="content"></asp:Label>
                <br />
                <label class="label" for="lblOriginator">Originator</label>
                <asp:Label id="lblOriginator" runat="server" CssClass="content"></asp:Label>
                <br />
            </asp:Panel>
            <div class="clearer"></div> 
            <asp:Panel id="pnlVerification" runat="server">
                <br />
                
                <fieldset id="grpVerification" runat="server" style="float:none;width:97%;">
                    <legend>Amendment Verification</legend>
                    <input id="optVerify" runat="server" style="float:left;" name="Verification" title="Verify" type="radio" onclick="javascript:ValidatorEnable(GetElement(providerFeedbackID + '_valRequired'), false);" />
                    <label class="label" style="float:left; padding-left:0.5em; width:5em;" for="optVerify">Verify</label>
                    <input id="optDecline" runat="server" style="float:left; margin-left:1em;" name="Verification"  title="Decline" type="radio" onclick="javascript:ValidatorEnable(GetElement(providerFeedbackID + '_valRequired'), true);" />
                    <label class="label" style="float:left; padding-left:0.5em; width:5em;" for="optDecline">Decline</label>
                    <br /><br />
                    <div class="clearer"></div> 
                    <input type="button" id="btnVerification" value="Verify/Decline" style="float:right; width:9em;" runat="server" />
                    <label class="label" for="txtFeedback">Provider Feedback</label>
                    <cc1:TextBoxEx ID="txtFeedback" runat=server validationgroup="Verify" Required="true" RequiredValidatorErrMsg="Provider Feedback must be entered."></cc1:TextBoxEx>
                    <asp:Panel id="pnlVerificationDetails" runat="server">
                        <br />
                        <label class="label" for="lblVerificationDate">Verification Date</label>
                        <asp:Label id="lblVerificationDate" runat="server" CssClass="content"></asp:Label>
                        <br />
                        <label class="label" for="lblVerifiedBy">Verification By</label>
                        <asp:Label id="lblVerifiedBy" runat="server" CssClass="content"></asp:Label>
                        <br />
                    </asp:Panel>
                </fieldset>
            </asp:Panel>
            <input type="hidden" id="hidContractID" runat="server" />
            <input type="hidden" id="hidVisitDate" runat="server" />
            <input type="hidden" id="hidAmendmentID" runat="server" />
        </fieldset>
    </asp:Panel> 
    
</asp:Content>
