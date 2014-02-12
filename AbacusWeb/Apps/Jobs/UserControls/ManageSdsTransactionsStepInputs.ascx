<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ManageSdsTransactionsStepInputs.ascx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.ManageSdsTransactionsStepInputs" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceClientSelector" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>

To submit a job that will manage Self-Directed Support (SDS) transactions, set the required filters below and click the "Create New Job" button.
<br />
The filters you set will be applied at the time that the job runs to gather the required data for processing.
<br /><br />
Please note that selecting no 'Transaction Types' will default to creating all.

<br /><br />
<asp:Label ID="lblClient" AssociatedControlID="client" runat="server" Text="Service User" Width="12.5em" />
<uc1:InPlaceClientSelector id="client" runat="server" Required="false" />
<br />
<asp:CheckBox ID="cbForceReconsideration" runat="server" Text="Force reconsideration of the selected transactions?" Checked="false" EnableViewState="true"  />
<br />
<br />
<asp:Label ID="lblTransactionType" AssociatedControlID="cblTransactionTypes" runat="server" Text="Transaction Types" />
(<asp:CheckBox ID="cbSelectAllTransactionTypes" runat="server" Text="All?" ToolTip="Select/Unselect All Transaction Types?" />):
<br />
<cc1:CheckBoxListRequiredValidator ID="reqTransactionTypes" runat="server" ControlToValidate="cblTransactionTypes" 
    ErrorMessage="Please select at least one transaction type.<br />" Display="Dynamic" Font-Bold="true" />
<br />
<asp:CheckBoxList ID="cblTransactionTypes" runat="server" />



