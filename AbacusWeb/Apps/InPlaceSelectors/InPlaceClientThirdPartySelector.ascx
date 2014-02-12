<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InPlaceClientThirdPartySelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceClientThirdPartySelector" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceClient" Src="InPlaceClientSelector.ascx" %>


<uc1:InPlaceClient id="clientSelector"  runat="server"></uc1:InPlaceClient>
<br />
<asp:Label ID="lblThirdParty" AssociatedControlID="txtTPName" runat="server" text="Third Party" width="5em"></asp:Label>
<input type="text" id="txtTPName" style="width:22em;" runat="server" />
<input type="button" id="btnTPFind" runat="server" value="..." />
<input type="text" style="display:none;" id="hidID" runat="server" />
<br />
<asp:RequiredFieldValidator id="valRequired" ControlToValidate="hidID" runat="server"></asp:RequiredFieldValidator>