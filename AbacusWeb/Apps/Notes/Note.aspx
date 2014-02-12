<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Note.aspx.vb" Inherits="Target.Abacus.Web.Apps.Notes.Note" EnableViewState="false" MasterPageFile="~/Popup.master"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MPContent" runat="server">
<h3 style="padding: 0.5em 0em 0em 0.5em; margin: 0em 0em 0.5em 0em;" id="noteHeader" runat="Server"></h3>
<div style="padding: 0.5em 0.5em 0em 0.5em; margin: 0em 0em 0.5em 0em;">
    <uc1:StdButtons id="stdButtons1" runat="server" />   
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <legend>Notes</legend>
        <asp:Label ID="lblNoteCategories" runat="server" AssociatedControlID="ddNoteCategories" Text="Category:" />
        <br />
        <cc1:DropDownListEx ID="ddNoteCategories" runat="server" LabelWidth="0em" Width="99%"
        Required="true" RequiredValidatorErrMsg="* Please select a category" ValidationGroup="Save" />
        <br /> 
        <asp:Label ID="lblNote" runat="server" AssociatedControlID="txtNote" Text="Note:" />
        <br />         
        <cc1:TextBoxEx ID="txtNote" runat="server" LabelWidth="0em" MaxLength="15" 
            Width="99%" Required="true" RequiredValidatorErrMsg="* Please enter a note" SetFocus="true" ValidationGroup="Save" />
    </fieldset>
    <br />
</div>
</asp:Content>
