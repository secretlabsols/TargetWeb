<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Notes.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.ProviderInvoices.Notes" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>

<%@ Register TagPrefix="uc1" TagName="Header" 
Src="~/AbacusWeb/Apps/UserControls/DomProviderInvoiceHeaderDetails.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows users to maintain Non Residential Payment Notes.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
	     <uc1:Header id="headerDetails" runat="server"></uc1:Header>
	     <br />
        <fieldset id="grpInvNotes" runat="server">
            <legend>Notes</legend>		           
            <table class="listTable" id="tblInvNotes" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of non residential payment notes.">
                <caption>List of non residential payment notes.</caption>
                <thead>
	                <tr>
		                <th style="width:1.5em;"></th>
		                <%--<th></th>--%>
                        <th style="width:12%;">Date</th>
                        <th style="width:10%;">Time</th>
                        <th style="width:10%; vertical-align:'bottom';">Entered<br/>By</th>
                        <th>Note</th>
                        <th style="width:17%; vertical-align:'bottom';">Date<br/>Amended</th>
                        <th style="width:10%; vertical-align:'bottom';">Amended<br/>By</th>
	                </tr>
                </thead>
                <tbody>
                        <tr>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            
                        </tr>
                    </tbody>
            </table>
            <br />
            <input type="button" id="btnDelete" runat="server" style="float:right;width:7em;" value="Delete" onclick="btnDelete_Click();" />
            <input type="button" id="btnEdit" runat="server" style="float:right;width:7em;" value="Edit" onclick="btnEdit_Click();" />
            <input type="button" id="btnNew" runat="server" style="float:right;width:7em;" value="New" onclick="btnNew_Click();" />
            <input type="button" id="btnView" style="float:right;width:7em;" value="View" onclick="btnView_Click();" />
        </fieldset>
        <div class="clearer"></div>
        <br />
        <div id="divCopyDialogContentContainer" style="display:none;">
            <div id="divCopyDialogContent">
                <!-- hidden elements used in copy dialog -->
                <cc1:TextBoxEx ID="txtNote" runat="server" LabelText="Note" LabelWidth="3em" 
                    Required="true" RequiredValidatorErrMsg="Please enter your note." ValidationGroup="Save"></cc1:TextBoxEx>
                <br />
            </div>
        </div>
    </asp:Content>