<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocumentAdd.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Documents.DocumentAdd" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<asp:content contentplaceholderid="MPPageOverview" runat="server">
</asp:content>

<asp:content id="Content3" contentplaceholderid="MPPageError" runat="server">    
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />    
</asp:content>

<asp:content id="Content4" contentplaceholderid="MPContent" runat="server">       
    <input type="hidden"  id="hidAssoc1TypeID" runat="server" />
    <input type="hidden" id="hidAssoc2TypeID" runat="server" />
    <input type="hidden"  id="hidAssoc3TypeID" runat="server" />
    <input type="hidden"  id="hidAssoc4TypeID" runat="server" />
	<fieldset id="fsAddDocument" style="padding:0.5em;margin:0.5em;">
	    <legend>Add Document</legend>
	    <table><tr><td><label style="width:8.5em;">Document</label></td><td><cc1:FileUpload ID="flDocumentAddFileUpload" runat="server" MaxFiles="1" AddGroupBox="false" Width="32.6em" /></td></tr></table>
        <div class="clearer"></div>
        <br />
        <cc1:DropDownListEx ID="cboDocumentType" runat=server LabelText="Document Type" LabelWidth="9em" Width="15em" Required="true" RequiredValidatorErrMsg="Please select document type" />
        <br />
	    <cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description" LabelWidth="9em" Width="32.25em" />  
	    
	    <br />

        <br />
        <div>
            <span style="float:right;margin-top:0.5em;">
	            <input id="btnAdd" type="button" value="Add" onclick="btnAdd_click();" style="width:5em;" />
	            <input id="btnCancel" type="button" value="Cancel" onclick="btnCancel_Click()" style="width:5em;" />
	        </span>
	    </div>
	</fieldset>

</asp:content>
