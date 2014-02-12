<%@ Page Language="vb" AutoEventWireup="false" Codebehind="CreatePrintQueueBatch.aspx.vb"
	Inherits="Target.Abacus.Web.Apps.Documents.CreatePrintQueueBatch" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="CJ" TagName="CreateJobPointInTime" Src="~/AbacusWeb/Apps/Jobs/UserControls/ucCreateJobPointInTime.ascx" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">

</asp:Content>

<asp:Content ContentPlaceHolderID="MPContent" runat="server">

	<fieldset id="fsFilterCriteria" style="padding:0.5em;width:50em;margin:0.5em;" >
	<legend>Filter Criteria</legend>
    <div style="border: 1px solid red;">
		<cc1:TextBoxEx ID="txtDocumentTypes" runat="server" LabelText="Document Types:" LabelWidth="12em" Width="37em" IsReadOnly="True" /><br />
    </div>
		<cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description:" LabelWidth="12em" Width="37em" IsReadOnly="True" /><br />
		<cc1:TextBoxEx ID="txtQueuedBy" runat="server" LabelText="Queued By:" LabelWidth="12em" Width="37em" IsReadOnly="True" /><br />
		<cc1:TextBoxEx ID="txtRecipientReference" runat="server" LabelText="Recipient Reference:" LabelWidth="12em" Width="37em" IsReadOnly="True" /><br />
		<cc1:TextBoxEx ID="txtRecipientName" runat="server" LabelText="Recipient Name:" LabelWidth="12em" Width="37em" IsReadOnly="True" /><br />
		<cc1:TextBoxEx ID="txtDocumentCount" runat="server" LabelText="Document Count:" LabelWidth="12em" Width="37em" IsReadOnly="True" /><br />
	</fieldset>

	<fieldset id="fsCreateBatch" style="padding:0.5em;width:50em;margin:0.5em;" >
	<legend>Create Batch</legend>
	    <cc1:DropDownListEx ID="cboPrinter" LabelText="Printer" runat="server" LabelWidth="12em" Required="true" RequiredValidatorErrMsg="Please select printer" ValidationGroup="Create" /><br />
		<CJ:CreateJobPointInTime id="CreateJobPointInTime1" runat="server" />
		<cc1:TextBoxEx ID="txtComment" runat="server" LabelText="Comment" LabelWidth="12em" Width="37em" />
	</fieldset>

    <div style="width:51.75em;padding-top:0.5em;">
        <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="PQBbutton" />
        <asp:Button ID="btnCreate" runat="server" Text="Create" CssClass="PQBbutton" OnClientClick="return ValidateDocumentCount();" />
    </div>

    <script type="text/javascript">

        function ValidateDocumentCount() {
            var strDocCount = '<%=txtDocumentCount.Text%>';
            var intDocCount = 0;

            if (!isNaN(strDocCount)) intDocCount = parseInt(strDocCount);

            if (intDocCount > 0) return true;

            alert("There are no documents to print.");

            return false;
        }

    </script>

</asp:Content>
