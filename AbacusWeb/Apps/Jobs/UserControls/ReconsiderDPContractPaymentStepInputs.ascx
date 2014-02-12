<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ReconsiderDPContractPaymentStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.ReconsiderDPContractPaymentStepInputs" EnableViewState="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceBudgetHolder" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceBudgetHolderSelector.ascx" %>

To submit a job that will reconsider existing Direct Payments, set the required filters below and click the "Create New Job" button.

<br /><br />
<asp:Label ID="lblClient" AssociatedControlID="ipClient" runat="server" Text="Service User" Width="10em"></asp:Label>
<uc1:InPlaceClient id="ipClient" runat="server" Mode="DomProviders"></uc1:InPlaceClient>
<br />
<asp:Label ID="lblBudgetHolder" AssociatedControlID="ipBudgetHolder" runat="server" Text="Budget Holder" Width="10em"></asp:Label>
<uc2:InPlaceBudgetHolder id="ipBudgetHolder" runat="server"></uc2:InPlaceBudgetHolder>
<br />
<cc1:DropDownListEx ID="cboBudgetCategory" runat="server" LabelText="Budget Category" LabelWidth="10.25em"></cc1:DropDownListEx>
<br />
<cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Date From" LabelWidth="10.25em" Format="DateFormat"></cc1:TextBoxEx>
<br />
<cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="Date To" LabelWidth="10.25em" Format="DateFormat"></cc1:TextBoxEx>
