<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CreateSDSInvoiceV2StepInputs.ascx.vb" 
                                Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.CreateSDSInvoiceV2StepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceClientSelector" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>

To submit a job that will create invoices, set the required filters below and click the "Create New Job" button.
<br />
The filters you set will be applied at the time that the job runs to gather the required data for processing.

<br /><br />
<%--Date To--%>
<cc1:TextBoxEx ID="dteLastInvDate" runat="server" LabelText="Last Invoiced Up To" LabelWidth="13em" IsReadOnly="true" 
            Format="DateFormatJquery"	ValidationGroup="Save"></cc1:TextBoxEx>
<br />
<br />
<%--Date From--%>
<cc1:TextBoxEx ID="dteInvUpTo" runat="server" LabelText="Invoice Up To" LabelWidth="13em"
	Required="true" RequiredValidatorErrMsg="Please enter a date you wish to invoice up to" Format="DateFormat"
	ValidationGroup="Save"></cc1:TextBoxEx>
<br />
<asp:Label ID="lblClient" AssociatedControlID="client" runat="server" Text="Service User" Width="12.5em"></asp:Label>
<uc1:InPlaceClientSelector id="client" runat="server" Required="false" ></uc1:InPlaceClientSelector>
<br />
<%--<input id="optReportOnly" runat="server" type="radio" name="generate" value="0" />
<label class="label"  for="optReportOnly" >Report Only</label>
<br />
<input id="optGenerate" runat="server" type="radio" name="generate" value="1"  />
<label class="label" for="optGenerate" >Generate Invoices</label>
<br /><br />--%>

<asp:RadioButtonList id="optGenerateOrReport" runat="server">
    <asp:ListItem>Report Only</asp:ListItem>
    <asp:ListItem>Generate Invoices</asp:ListItem>
</asp:RadioButtonList>
<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="optGenerateOrReport"
    ErrorMessage="You must select either 'Report Only' or 'Generate Invoices'." InitialValue=""></asp:RequiredFieldValidator>
<input type="hidden" id="hidCreatingJob" runat="server" value="1" />