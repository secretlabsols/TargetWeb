<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocumentTypes.aspx.vb" Inherits="Target.Abacus.Web.Apps.Documents.DocumentTypes" EnableViewState="true" EnableEventValidation="false" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2"  ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different document types.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>

    <div style="height:22em;">
    <fieldset id="fsDetails" style="padding:0.5em;float:left;margin-bottom:1em;" runat="server" enableviewstate="false">
        <legend>Details</legend>

        <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="10.5em" MaxLength="255" 
            Width="23em"  Required="true" RequiredValidatorErrMsg="Please enter a description"
            ValidationGroup="Save" ></cc1:TextBoxEx>
        <br />
        
        <cc1:TextBoxEx ID="txtSystemType"  runat="server"  LabelText="System Type" LabelWidth="10.5em" MaxLength="255" 
            Width="23em"  IsReadOnly="true" Text=""  ></cc1:TextBoxEx>
        <br /><br />
        
        <cc1:TextBoxEx ID="txtFileNamePattern"  runat="server"  LabelText="Filename Pattern" LabelWidth="10.5em" MaxLength="255" 
            Width="23em"  Required="true" RequiredValidatorErrMsg="Please enter a filename pattern" 
            ValidationGroup="Save" ></cc1:TextBoxEx>
        <br />

        <cc1:TextBoxEx ID="txtRePrintWatermark"  runat="server"  LabelText="Re-print Watermark" LabelWidth="10.5em" MaxLength="255" 
            Width="23em"  ValidationGroup="Save" ></cc1:TextBoxEx>
        <div style="color:#FF8040;font-size:x-small;margin-top:0.3em;">Please note: Watermarking Excel workbooks is not currently supported.</div>
        <br />
        <cc1:CheckBoxEx ID="chkPublish" runat="server" Text="Publish to Extranet" LabelWidth="10.5em" ></cc1:CheckBoxEx>
        <br /><br />
        <cc1:CheckBoxEx ID="chkUpload" runat="server" Text="Upload via Extranet" LabelWidth="10.5em" ></cc1:CheckBoxEx>
        <br /><br />
        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="10.5em" ></cc1:CheckBoxEx>
    </fieldset>

    <fieldset id="fsPrinting" style="padding:0.5em;margin-left:1em;float:left;margin-bottom:1em;" runat="server" enableviewstate="false">
        <legend>Printing</legend>
        <label for="cboPrinter" style="width:9.5em;float:left;">When printing on</label>
        <cc1:DropDownListEx ID="cboPrinter" runat="server" Width="25em" />
        <br />
        
        <label for="cboPaperTray" style="width:9.5em;float:left;">Use paper from</label>
        <cc1:DropDownListEx ID="cboPaperTray" runat="server" Width="25em" 
            Required="true" ValidationGroup="Save" RequiredValidatorErrMsg="Please select a paper tray" />
        <br />
        
        <label for="cboPaperSize" style="width:9.5em;float:left;">Which is of size</label>
        <cc1:DropDownListEx ID="cboPaperSize" runat="server" Width="25em"
            Required="true" ValidationGroup="Save" RequiredValidatorErrMsg="Please select paper size" />
        <br />

        <label for="cboPrintOn" style="width:9.5em;float:left;">Print on</label>
        <cc1:DropDownListEx ID="cboPrintOn" runat="server" Width="25em"
            Required="true" ValidationGroup="Save" RequiredValidatorErrMsg="Please select print side" />
        <br />
    </fieldset>
   
   </div>
    <br />

   <script type="text/javascript">
       validIcon = '<% =ValidIcon %>';
       invalidIcon = '<% =InvalidIcon %>';

       documentPrinterID = <% =documentPrinterID %>;
       paperSourceID = <% =paperSourceID %>;
       paperSizeID = <% =paperSizeID %>;
       printOnID = <% =printOnID %>;
       
       editMode = <% =IIf(EditMode, "true", "false") %>;
   </script>
</asp:Content>