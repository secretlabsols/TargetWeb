<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CreateReEvaluateVisitBasedClientChargesStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.CreateReEvaluateVisitBasedClientChargesStepInputs" EnableViewState="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>

To submit a job that will re-evaluate visit based client charges, set the required filters below and click the "Create New Job" button.

<br /><br />
<asp:RadioButton ID="rdbProvider" Text="Provider" runat="server" Width="11em" GroupName="ReEval" />
<%--<asp:Label ID="lblProvider" AssociatedControlID="provider" runat="server" Text="Provider" Width="11em"></asp:Label>--%>
<uc1:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders"></uc1:InPlaceEstablishment>
<br />
<asp:Label ID="lblContract" AssociatedControlID="domContract" runat="server" Text=" Contract" Width="9em" style="margin-left:2em;" ></asp:Label>
<uc2:InPlaceDomContract id="domContract" runat="server"></uc2:InPlaceDomContract>
<br />
<asp:RadioButton ID="rdbServiceUser" Text="Service User" runat="server" GroupName="ReEval"   Width="11em" />
<uc3:InPlaceClient id="serviceUser" runat="server"></uc3:InPlaceClient>
<br />
<cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Week Ending From" LabelWidth="11em" Format="DateFormatJquery" AllowClear="true"></cc1:TextBoxEx>
<br />
<cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="Week Ending To" LabelWidth="11em"  Format="DateFormatJquery" AllowClear="true"></cc1:TextBoxEx>
