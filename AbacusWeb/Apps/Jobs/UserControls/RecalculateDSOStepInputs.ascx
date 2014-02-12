<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RecalculateDSOStepInputs.ascx.vb" 
    Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.RecalculateDSOStepInputs" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>

To submit a job that will edit and re-save service orders associated with a specific contract,
<br />
select the required contract below and click the "Create New Job" button.

<br /><br />
<asp:Label ID="lblContract" AssociatedControlID="domContract" runat="server" Text="Contract" Width="10.5em"></asp:Label>
<uc1:InPlaceDomContract id="domContract" runat="server"></uc1:InPlaceDomContract>
<br />
<input type="hidden" id="hidCreatingJob" runat="server" value="1" />