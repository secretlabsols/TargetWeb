<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocumentAdd.aspx.vb"
    Inherits="Target.Abacus.Web.Apps.Documents.DocumentAdd" EnableEventValidation="false" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceServiceOrder" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceGenericServiceOrderSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>
<%@ Register TagPrefix="uc5" TagName="InPlaceBudgetHolder" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceBudgetHolderSelector.ascx" %>
<%@ Register TagPrefix="uc6" TagName="InPlaceCreditor" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceGenericCreditorSelector.ascx" %>

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
	    
	    <fieldset id="fsDetails" >
            <legend>Associations</legend>
            <div>
                <button id="btnAssociateWith1" style="width:12em; float:left; padding-top:1px; top:2px;">Associate With</button>
                <span id="spanclientSelector1" style="display:none; padding-left:5px;"><uc4:InPlaceClient id="clientSelector1" runat="server" Required="true" /></span>
                <span id="spanbudgetholderSelector1" style="display:none; padding-left:5px;"><uc5:InPlaceBudgetHolder id="budgetholderSelector1" runat="server" Required="true" /></span>
                <span id="spancreditorSelector1" style="display:none; padding-left:5px;"><uc6:InPlaceCreditor id="creditorSelector1" runat="server" Required="true" /></span>
                <span id="spanserviceOrderSelector1" style="display:none; padding-left:5px;"><uc3:InPlaceServiceOrder id="serviceOrderSelector1" runat="server" Required="true" /></span>
            </div>
            <div class="clearer"></div>
            <div style="padding-top:3px;">
                <button id="btnAssociateWith2" style="width:12em; float:left; padding-top:1px; top:2px;">Associate With</button>
                <span id="spanclientSelector2" style="display:none; padding-left:5px;"><uc4:InPlaceClient id="clientSelector2" runat="server" Required="true" /></span>
                <span id="spanbudgetholderSelector2" style="display:none; padding-left:5px;"><uc5:InPlaceBudgetHolder id="budgetholderSelector2" runat="server" Required="true" /></span>
                <span id="spancreditorSelector2" style="display:none; padding-left:5px;"><uc6:InPlaceCreditor id="creditorSelector2" runat="server" Required="true" /></span>
                <span id="spanserviceOrderSelector2" style="display:none; padding-left:5px;"><uc3:InPlaceServiceOrder id="serviceOrderSelector2" runat="server" Required="true" /></span>
            </div>
            <div class="clearer"></div>
            <div style="padding-top:3px;">
                <button id="btnAssociateWith3" style="width:12em; float:left; padding-top:1px; top:2px;">Associate With</button>
                <span id="spanclientSelector3" style="display:none; padding-left:5px; padding-left:5px;"><uc4:InPlaceClient id="clientSelector3" runat="server" Required="true"  /></span>
                <span id="spanbudgetholderSelector3" style="display:none; padding-left:5px;"><uc5:InPlaceBudgetHolder id="budgetholderSelector3" runat="server" Required="true" /></span>
                <span id="spancreditorSelector3" style="display:none; padding-left:5px;"><uc6:InPlaceCreditor id="creditorSelector3" runat="server" Required="true" /></span>
                <span id="spanserviceOrderSelector3" style="display:none; padding-left:5px;"><uc3:InPlaceServiceOrder id="serviceOrderSelector3" runat="server" Required="true" /></span>
            </div>
            <div class="clearer"></div>
            <div style="padding-top:3px;">
                <button id="btnAssociateWith4" style="width:12em; float:left; padding-top:1px; top:2px;">Associate With</button>
                <span id="spanclientSelector4" style="display:none; padding-left:5px;"><uc4:InPlaceClient id="clientSelector4" runat="server" Required="true"  /></span>
                <span id="spanbudgetholderSelector4" style="display:none; padding-left:5px;"><uc5:InPlaceBudgetHolder id="budgetholderSelector4" runat="server" Required="true" /></span>
                <span id="spancreditorSelector4" style="display:none; padding-left:5px;"><uc6:InPlaceCreditor id="creditorSelector4" runat="server" Required="true" /></span>
                <span id="spanserviceOrderSelector4" style="display:none; padding-left:5px;"><uc3:InPlaceServiceOrder id="serviceOrderSelector4" runat="server" Required="true" /></span>
            </div>
        </fieldset>
        <br />
        <div>
            <cc1:CheckBoxEx ID="chkPublish" runat="server" Text="Publish to Extranet" LabelWidth="10.5em" ></cc1:CheckBoxEx>
        </div>
        <br />
        <div>
            <span style="float:right;margin-top:0.5em;">
	            <input id="btnAdd" type="button" value="Add" onclick="btnAdd_click();" style="width:5em;" />
	            <input id="btnCancel" type="button" value="Cancel" onclick="btnCancel_Click()" style="width:5em;" />
	        </span>
	    </div>
	</fieldset>

    <script type="text/javascript">
       // setting in-place-selectors' IDs
       clientSelector1ID = '<%=clientSelector1.ClientID%>';
       budgetholderSelector1ID = '<%=budgetholderSelector1.ClientID%>';
       creditorSelector1ID = '<%=creditorSelector1.ClientID%>';

       clientSelector2ID = '<%=clientSelector2.ClientID%>';
       budgetholderSelector2ID = '<%=budgetholderSelector2.ClientID%>';
       creditorSelector2ID = '<%=creditorSelector2.ClientID%>';
    </script>

</asp:content>
