<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocumentPrinters.aspx.vb" Inherits="Target.Abacus.Web.Apps.Documents.DocumentPrinters" EnableViewState="true" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2"  ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to view the document printers.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <div style="height:43em;">
    <fieldset id="fsDetails" style="padding:0.5em;float:left;margin-bottom:1em;width:60em;" runat="server" enableviewstate="false">

        <cc1:TextBoxEx ID="txtPrinterName"  runat="server"  LabelText="Printer Name:" LabelWidth="10.5em" MaxLength="255" 
            Width="20em" IsReadOnly="true"></cc1:TextBoxEx>
        <br /><br />
        
        <cc1:TextBoxEx ID="txtJobServiceInstanceName"  runat="server"  LabelText="Job Service Server:" LabelWidth="10.5em" MaxLength="255" 
            Width="20em" IsReadOnly="true"></cc1:TextBoxEx>
        <br /><br />
        
        <cc1:TextBoxEx ID="txtCanDuplex"  runat="server"  LabelText="Can Duplex:" LabelWidth="10.5em" MaxLength="255" 
            Width="20em" IsReadOnly="true"></cc1:TextBoxEx>
        <br /><br />
        
        <cc1:TextBoxEx ID="txtIsValid"  runat="server"  LabelText="Is Valid:" LabelWidth="10.5em" MaxLength="255" 
            Width="20em" IsReadOnly="true"></cc1:TextBoxEx>
        <br />
        
        <fieldset style="padding:0.5em;width:28.5em;height:15em;" >
            <legend>Paper Sources</legend>
            <asp:ListBox id="lbPaperSources" runat="server" Width="100%" Height="90%"></asp:ListBox>
        </fieldset>
        
        <fieldset style="padding:0.5em;width:28.5em;height:15em;" >
            <legend>Paper Sizes</legend>
            <asp:ListBox id="lbPaperSizes" runat="server" Width="100%" Height="90%"></asp:ListBox>
        </fieldset>
    </fieldset>
    </div>
</asp:Content>