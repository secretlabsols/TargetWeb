<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DurationClaimedRounding.aspx.vb"
    Inherits="Target.Abacus.Extranet.Apps.Dom.ReferenceData.DurationClaimedRounding" %>
<%@ Register TagPrefix="uc1" TagName="dcr" 
Src="~/AbacusExtranet/Apps/UserControls/DurationClaimedRoundingSelector.ascx" %>

<asp:Content ContentPlaceHolderID="MPContent" runat="server">
       <uc1:dcr id="durationClaimedRounding" runat="server"></uc1:dcr>
       
</asp:Content>