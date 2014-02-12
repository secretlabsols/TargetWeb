<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PrintQueueBatches.aspx.vb" Inherits="Target.Abacus.Web.Apps.Documents.PrintQueueBatches" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">

</asp:Content>

<asp:Content ContentPlaceHolderID="MPContent" runat="server" >

    <cc1:CollapsiblePanel id="cpFilters" runat="server" HeaderLinkText="Filters" >

        <ContentTemplate>

            <fieldset style="padding:1em;width:35.95em;margin:0.5em;" >
            <legend>Created</legend>
                <cc1:TextBoxEx ID="dteCreatedFrom" runat="server" LabelText="From"  Format="DateFormat" LabelWidth="3em" Width="20em" OutputBrAfter="false" />&nbsp;&nbsp;&nbsp;
                <cc1:TextBoxEx ID="dteCreatedTo" runat="server" LabelText="To"  Format="DateFormat" LabelWidth="2em" Width="20em" OutputBrAfter="false" />&nbsp;&nbsp;&nbsp;
                <cc1:TextBoxEx ID="txtCreatedBy"  runat="server"  LabelText="By *" LabelWidth="3em" MaxLength="255"  Width="10em" />        
            </fieldset>
            
            <br />

            <fieldset style="padding:1em;width:35.95em;margin:0.5em;" >
	        <legend>Printer</legend>
	            <cc1:DropDownListEx ID="cboPrinter" runat="server" Width="35.85em" />
	        </fieldset>

    	    <input type="button" style="width:7em;margin:0.35em;" value="Filter" onclick="currentPage = 1; FetchPrintQueueBatches();" />

        </ContentTemplate>
        
    </cc1:CollapsiblePanel>  

    <table class="listTable sortable" id="tblPrintQueueBatches" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of Print Queue Batches.">
        <thead>
	        <tr>
		        <th id="thSelect" style="width:1.5em;"></th>
		        <th id="thCreated" style="width:9em;">Created</th>
		        <th id="thCreatedBy" style="width:8em">Created By</th>
		        <th id="thComments" style="width:20em">Comments</th>
		        <th id="thDocuments" style="width:5em"># Docs.</th>
		        <th id="thPrinter" style="width:25em">Printer</th>
		        <th id="thJobStatus" style="width:7em">Job Status</th>
	        </tr>
        </thead>
        <tbody>
            <tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>
        </tbody>
    </table>

    <div id="PrintQueueBatches_PagingLinks" style="float:left;"></div>

    <input type="button" style="float:right;margin-left:0.5em;margin-bottom:0.5em;" id="btnViewJob" value="View Job" onclick="PrintQueueBatches_btnViewJob_Click();" />
    <input type="button" style="float:right;" id="btnViewDocuments" value="View Documents" onclick="PrintQueueBatches_btnViewDocuments_Click();" />

    <script language="javascript" type="text/javascript">

        // set filter controls' IDs
        createdFromID = '<%=dteCreatedFrom.ClientID%>_txtTextBox';
        createdToID   = '<%=dteCreatedTo.ClientID%>_txtTextBox';
        createdByID   = '<%=txtCreatedBy.ClientID%>_txtTextBox';
        cboPrinterID  = '<%=cboPrinter.ClientID%>_cboDropDownList';

    </script>

    <br /><br />
</asp:Content>
