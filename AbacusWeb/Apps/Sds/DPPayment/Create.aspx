<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Create.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.DPPayments.Create"
    EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Assembly="Target.Abacus.Web" Namespace="Target.Abacus.Web" TagPrefix="cc1" %>
<%@ Register Src="../../Jobs/UserControls/ucCreateJobPointInTime.ascx" TagName="ucCreateJobPointInTime"
    TagPrefix="uc1" %>
<%@ Register Src="../../Jobs/UserControls/ucPaymentPreviewOptions.ascx" TagName="ucPaymentPreviewOptions"
    TagPrefix="uc2" %>
<asp:content contentplaceholderid="MPPageOverview" runat="server">
	
	This screen allows you to create direct payments.
	
</asp:content>
<asp:content id="Content3" contentplaceholderid="MPPageError" runat="server">
    
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:content>
<asp:content id="Content4" contentplaceholderid="MPContent" runat="server">
       
    <asp:Panel ID="pnlForm" runat="server">
        
        <fieldset>
	        <legend>Filter Criteria</legend>
		    <table>
		        <tr style="line-height:1em">
		            <td style="font-weight:bold; width: 10em">Service User</td>
		            <td><asp:Label id="lblFilterServiceUser" runat="server" CssClass="content" /></td>
		        </tr>
		        <tr style="line-height:1em">
		            <td style="font-weight:bold">Budget Holder</td>
		            <td><asp:Label id="lblFilterBudgetHolder" runat="server" CssClass="content" /></td>
		        </tr>
		         <tr style="line-height:1em">
		            <td style="font-weight:bold">SDS?</td>
		            <td><asp:Label id="lblFilterSds" runat="server" CssClass="content" /></td>
		        </tr>
		        <tr style="line-height:1em">
		            <td style="font-weight:bold">Last Payment Due Date</td>
		            <td><asp:Label id="lblLastPaidUpTo" runat="server" CssClass="content" /></td>
		        </tr>
            </table>
        </fieldset> 
        
        <br />
        <br />
        <fieldset id="grpCreateInterface" runat="server">
            <legend>Create Payments</legend>
				<cc1:TextBoxEx ID="dtePayUpTo" runat="server" LabelText="Payment Due Date:" LabelWidth="17em"
                Required="true" RequiredValidatorErrMsg="Please enter a valid pay upto date" Format="DateFormat"
				ValidationGroup="Save" />
				<asp:RangeValidator ID="valDates" ControlToValidate="dtePayUpTo$txtTextBox" runat="server" ValidationGroup="Save" />
				<br />
				<asp:CheckBox ID="chkDoNotCollectReconsideredPayments" runat="server" Text="Do not collect payments from DP Contract Payments that are marked for reconsideration" Checked="true" TextAlign="Left" />
				<br />
				<br />
				<uc1:ucCreateJobPointInTime ID="ucCreateJobPointInTime1" runat="server" />
				<uc2:ucPaymentPreviewOptions ID="ucPaymentPreviewOptions1" runat="server" />
        </fieldset>
        <br />
        <div style="float:right;">
            <asp:Button id="btnCreate" runat="server" text="Create" tooltip="Create Direct Payments Job" width="6em" ValidationGroup="Save" />
            <input type="button" style="width:6em;" id="btnBack" value="Back" title="Return to the previous screen" onclick="btnBack_Click();" />
        </div>
        <div class="clearer" />
        <br />        
    
    </asp:Panel>
        
</asp:content>
