<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DocumentSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.Documents.UserControls.DocumentSelector" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="pd1" TagName="PrintDocumentsButton" Src="~/AbacusWeb/Apps/Documents/UserControls/PrintDocumentsButton.ascx" %>

<style type="text/css">
    .style1
    {
        width: 106px;
    }
    .style2
    {
        width: 33%;
    }
    .DocTabButton
    {
        float:right;
        width: 7em;
        margin: 0 3px 0 0 ;
    }
</style>

<fieldset id="fsPrintQueueBatch" runat="server" style="padding:0.5em;width:37.5em;margin:0.5em;margin-bottom:2em;" visible="false" >
<legend>Print Queue Batch</legend>
	<cc1:TextBoxEx ID="txtCreated" runat="server" LabelText="Created:" LabelWidth="9.5em" Width="37em" IsReadOnly="True" /><br />
	<cc1:TextBoxEx ID="txtDocumentCount" runat="server" LabelText="Document Count:" LabelWidth="9.5em" Width="37em" IsReadOnly="True" /><br />
	<cc1:TextBoxEx ID="txtPrinter" runat="server" LabelText="Printer:" LabelWidth="9.5em" Width="37em" IsReadOnly="True" /><br />
	<cc1:TextBoxEx ID="txtComment" runat="server" LabelText="Comment:" LabelWidth="9.5em" Width="37em" IsReadOnly="True" /><br />
</fieldset>


<cc1:CollapsiblePanel id="cpFilters" runat="server" HeaderLinkText="Filters" >
    <ContentTemplate>
        <fieldset id="fsDocumentType" runat="server" style="padding:0.5em;float:left;width:19em;height:10em;margin:0.5em;" >
        <legend>Document Type</legend>
            <div style="overflow:auto; width:auto; height:8em">
                <asp:CheckBoxList ID="cboDocumentTypes" runat="server" EnableViewState="true" ></asp:CheckBoxList>
            </div>
        </fieldset>

        <fieldset id="fsOrigin" runat="server" style="padding:0.5em;float:left;width:14em;height:10em;margin:0.5em;" >
        <legend>Origin</legend>
            <asp:RadioButtonList ID="rboOrigin" runat="server">
                <asp:ListItem Value="2">System-generated</asp:ListItem>
                <asp:ListItem Value="1">User-uploaded</asp:ListItem>
                <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
            </asp:RadioButtonList>
        </fieldset>

        <fieldset id="fsOther" style="padding:0.5em;float:left;width:14em;height:10em;margin:0.5em;" >
        <legend>Other</legend>
            <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description * " LabelWidth="7em" MaxLength="255" Width="6em" />
        </fieldset>

        <div class="clearer"></div>

        <fieldset id="fsCreated" runat="server" style="padding:0.5em;width:35em;margin:0.5em;" >
        <legend>Created</legend>
            <cc1:TextBoxEx ID="dteCreatedFrom" runat="server" LabelText="From"  Format="DateFormat" LabelWidth="3em" Width="20em" OutputBrAfter="false" />
            <cc1:TextBoxEx ID="dteCreatedTo" runat="server" LabelText="To"  Format="DateFormat" LabelWidth="2em" Width="20em" OutputBrAfter="false" />
            <cc1:TextBoxEx ID="txtCreatedBy"  runat="server"  LabelText="By *" LabelWidth="3em" MaxLength="255"  Width="10em" />
            <br />
        </fieldset>

        <div class="clearer"></div>

        <fieldset id="fsRecipient" runat="server" style="padding:0.5em;width:35em;margin:0.5em;" >
        <legend>Recipient</legend>
            <cc1:TextBoxEx ID="txtRecipientReference"  runat="server"  LabelText="Reference *" LabelWidth="7em" MaxLength="255"  Width="10em" OutputBrAfter="false" />&nbsp;&nbsp;&nbsp;
            <cc1:TextBoxEx ID="txtRecipientName"  runat="server"  LabelText="Name *" LabelWidth="4.5em" MaxLength="255"  Width="10em" />
        </fieldset>

        <div class="clearer"></div>

        <fieldset id="fsPrintStatus" runat="server" style="padding:0.5em;width:35em;margin-left:0.5em;" >
        <legend id="lgdPrintStatus" runat="server">Print Status</legend>
            <span id="spanQueuedFlags" runat="server" style="float:left;margin-right:1em;margin-bottom:1em;" >
                <cc1:CheckBoxEx ID="chkNeverQueued" Text="Never Queued" LabelWidth="7" runat="server" /><div class="clearer"></div>
                <cc1:CheckBoxEx ID="chkQueued" Text="Queued" LabelWidth="7" runat="server" /><div class="clearer"></div>
                <cc1:CheckBoxEx ID="chkRemovedFromQueue" Text="Removed From Queue" LabelWidth="7" runat="server" /><div class="clearer"></div>
            </span>

            <span id="spanBatchedFlags" runat="server" style="float:left;margin-bottom:1em;">
                <cc1:CheckBoxEx ID="chkBatched" Text="Batched" LabelWidth="7" runat="server" /><div class="clearer"></div>
                <cc1:CheckBoxEx ID="chkSentToPrinter" Text="Sent To Printer" LabelWidth="7" runat="server" /><div class="clearer"></div>
            </span>

            <div class="clearer"></div>

			<cc1:TextBoxEx ID="dtePrintedFrom" runat="server" LabelText="From"  Format="DateFormat" LabelWidth="3em" Width="20em" OutputBrAfter="false" />
            <cc1:TextBoxEx ID="dtePrintedTo" runat="server" LabelText="To"  Format="DateFormat" LabelWidth="2em" Width="20em" OutputBrAfter="false" />
            <cc1:TextBoxEx ID="txtPrintedBy"  runat="server"  LabelText="By * " LabelWidth="3em" MaxLength="255"  Width="10em" />
        </fieldset>
        
        <input type="button" id="btnFilter" value="Filter" onclick="FetchDocumentList(currentPage, DocumentSelector_selectedDocumentID);" />
        
        <div class="clearer"></div>

    </ContentTemplate>
</cc1:CollapsiblePanel>  

<table class="listTable sortable" id="tblDocuments" style="table-layout:fixed;margin-top:5px;"  cellpadding="2" cellspacing="0" width="100%" summary="List of service users.">
<thead>
	<tr>
		<th style="width:1.5em;"></th>
		<th id="thRecipientRef" style="width:10%">Recipient Ref</th>
		<th id="thRecipientName" style="width:15%">Recipient Name</th>
		<th id="thType" style="width:15%">Type</th>
		<th id="thDescription" style="width:15%">Description</th>
		<th id="thFilename" style="width:15%">Filename</th>
		<th id="thPrintStatus" style="width:15%">Print Status</th>
		<th class="style1">Status Date\Time</th>
	</tr>
</thead>
<tbody><tr><td></td><td></td><td class="style2"></td><td class="style1"></td><td></td><td></td><td></td><td></td></tr></tbody>
</table>

<div id="Document_PagingLinks" style="float:left;"></div>

<span style="float:right;"><pd1:PrintDocumentsButton id="btnPrintAll" PrintAll="True" HorizontalPosition="Left" VerticalPosition="Top" runat="server" ButtonWidth="65px" /></span>
<span style="float:right;"><pd1:PrintDocumentsButton id="btnPrint" PrintAll="False" HorizontalPosition="Left" VerticalPosition="Top" runat="server" ButtonWidth="65px" /></span>
<input type="button" id="btnCreateBatch" class="DocTabButton"  value="Create Batch" onclick="DocumentSelector_btnCreateBatch_Click();" runat="server" />
<input type="button" id="btnRemove" class="DocTabButton" value="Remove" onclick="DocumentSelector_btnRemove_Click();" runat="server" />
<input type="button" id="btnProperties" class="DocTabButton" value="Properties" onclick="DocumentSelector_btnProperties_Click();" runat="server" />
<input type="button" id="btnView" class="DocTabButton" value="View" onclick="DocumentSelector_btnView_Click();" runat="server" />
<input type="button" id="btnNew" class="DocTabButton" value="New" onclick="DocumentSelector_btnNew_Click();" runat="server" />

<div class="clearer"></div>

<script type="text/javascript">
    // setting Filters' Collapsible Panel ID
    cpFiltersID = '<%=cpFilters.ClientID%>';

    // setting buttons' ClientID
    btnPrintID = '<%=CType(btnPrint, Target.Abacus.Web.Apps.Documents.UserControls.PrintDocumentsButton).PrintButtonID%>';
    btnRemoveID = '<%=btnRemove.ClientID%>';

    // set value for documentPrintQueueBatchID if set by parent page
    documentPrintQueueBatchID = <%=IIf(isPrintQueueBatchDocumentsScreen, DocumentPrintQueueBatchID.ToString(), "null")%>;
</script>
