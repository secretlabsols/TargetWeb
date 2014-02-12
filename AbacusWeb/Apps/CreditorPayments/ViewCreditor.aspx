<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewCreditor.aspx.vb" Inherits="Target.Abacus.Web.Apps.CreditorPayments.ViewCreditor" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="DS" TagName="DocumentSelector" Src="~/AbacusWeb/Apps/Documents/UserControls/DocumentSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="Notes" Src="~/AbacusWeb/Apps/UserControls/NotesSelector.ascx" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to view the details of a Creditor.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" ></uc1:StdButtons>
    
    <ajaxToolkit:TabContainer runat="server" ID="tabStrip" OnClientActiveTabChanged="tabStrip_ActiveTabChanged" EnableViewState="false">
        <%--Details Tab--%>
        <ajaxToolkit:TabPanel runat="server" ID="tabDetails" HeaderText="Details">
            <ContentTemplate>
               <cc1:TextBoxEx ID="txtType" runat="server" LabelText="Type" LabelWidth="10.5em"
					 IsReadOnly="true" MaxLength="25"></cc1:TextBoxEx>
				<br />
                <cc1:TextBoxEx ID="txtCreditorReference" runat="server" LabelText="Creditor Reference" LabelWidth="10.5em"
					 IsReadOnly="true" MaxLength="25"></cc1:TextBoxEx>
				<br />
				<cc1:TextBoxEx ID="txtName" runat="server" LabelText="Name" LabelWidth="10.5em"
					 IsReadOnly="true" MaxLength="25"></cc1:TextBoxEx>
				<br />
				<cc1:TextBoxEx ID="txtAddress" runat="server" LabelText="Address" LabelWidth="10.5em"
					 ReadOnlyContentCssClass = "roLabel" IsReadOnly="true" MaxLength="25"></cc1:TextBoxEx>
				<br />
				<div class="clearer"></div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabDocuments" HeaderText="Documents" EnableViewState="true">
            <ContentTemplate>
                <DS:DocumentSelector id="docSelector" runat="server"  EnableViewState="true"></DS:DocumentSelector>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabNotes" HeaderText="Notes">
            <ContentTemplate>
                <uc3:Notes id="Notes1" runat="server"></uc3:Notes>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    <br />
    <input type="hidden" id="hidSelectedTab" runat="server" /> 
</asp:Content>