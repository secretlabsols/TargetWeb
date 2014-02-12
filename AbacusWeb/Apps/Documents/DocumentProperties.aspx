<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocumentProperties.aspx.vb"
    Inherits="Target.Abacus.Web.Apps.Documents.DocumentProperties" EnableEventValidation="false" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="BasicAuditDetails" Src="~/Library/UserControls/BasicAuditDetails.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="FL" TagName="DocumentDownloadLink" Src="~/AbacusWeb/Apps/Documents/UserControls/DocumentDownloadLink.ascx" %>

<asp:content contentplaceholderid="MPPageOverview" runat="server">
</asp:content>

<asp:content id="Content3" contentplaceholderid="MPPageError" runat="server">    
    <asp:Label ID="lblError" runat="server" CssClass="errorText" />    
</asp:content>

<asp:content id="Content4" contentplaceholderid="MPContent" runat="server">       

    <asp:Panel ID="pnlForm" runat="server">
                
        <ajaxToolkit:TabContainer runat="server" ID="tabContainer" EnableViewState="true">

            <ajaxToolkit:TabPanel runat="server" ID="tabProperties" HeaderText="Properties">

                <ContentTemplate>  
                    <fieldset id="fsDocument" style="padding:0.5em;float:left;margin-bottom:1em;width:47%;" runat="server" enableviewstate="false">
                    <legend>Document</legend>
                    <cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description:" LabelWidth="7em" IsReadOnly="true" />
                    <br />
                    <label style="width:7em;">Filename:</label><FL:DocumentDownloadLink ID="lnkIconAndDownload" runat="server"></FL:DocumentDownloadLink>
                    <br />
                    <cc1:TextBoxEx ID="txtType" runat="server" LabelText="Type:" LabelWidth="7em" IsReadOnly="true" />  
                    <br />
                    <cc1:TextBoxEx ID="txtCreated" runat="server" LabelText="Created:" LabelWidth="7em" IsReadOnly="true" />  
                    <br />
                    <cc1:TextBoxEx ID="txtCreatedBy" runat="server" LabelText="Created By:" LabelWidth="7em" IsReadOnly="true" />  
                    </fieldset>

                    <fieldset id="fsRepository" style="padding:0.5em;margin-left:1em;float:left;margin-bottom:1em;width:47%;" runat="server" enableviewstate="false">
                    <legend>Repository</legend>
                    <cc1:TextBoxEx ID="txtName" runat="server" LabelText="Name:" LabelWidth="7em" IsReadOnly="true" />  
                    <br />
                    <cc1:TextBoxEx ID="txtReference" runat="server" LabelText="Reference:" LabelWidth="7em" IsReadOnly="true" />                                                   
                    <br />                   
                    </fieldset>
                    
                    <div class="clearer"></div>

                    <fieldset id="fsAssociations" style="padding:0.5em;margin-bottom:1em;" runat="server" enableviewstate="false">
                    <legend>Associations</legend>
                    <asp:GridView ID="gvAssociations" runat="server" AutoGenerateColumns="false" CssClass="listTable" CellPadding="2" CellSpacing="0" Width="100%" Border="0" ShowHeader="true" EmptyDataText="No document associations were found.">
                        <Columns>
                            <asp:CheckBoxField HeaderText="Recipient" DataField="Recipient" ItemStyle-Width="4em" />
                            <asp:BoundField HeaderText="Reference" DataField="Reference" ItemStyle-Width="6em" />
                            <asp:BoundField HeaderText="Name" DataField="Name" ItemStyle-Width="8em" />
                            <asp:BoundField HeaderText="Address" DataField="Address" ItemStyle-Width="16em" />
                            <asp:BoundField HeaderText="Postcode" DataField="Postcode" ItemStyle-Width="6em" />
                            <asp:TemplateField HeaderText="Type">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# GetTypeDesc(DataBinder.Eval(Container.DataItem, "Type").ToString()) %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Width="15%" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    </fieldset>
                    <br />
                </ContentTemplate>
	        </ajaxToolkit:TabPanel>

	        <ajaxToolkit:TabPanel runat="server" ID="tabPrintHistory" HeaderText="Print History" Enabled="True">
                <ContentTemplate> 
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:GridView ID="gvPrintHistory" runat="server" AutoGenerateColumns="false" CssClass="listTable" CellPadding="1" CellSpacing="0" Width="100%" Border="0" ShowHeader="true" EmptyDataText="No document print history found." AllowPaging="True" >
                                <Columns>
                                    <asp:BoundField HeaderText="When" DataField="StatusDate" ItemStyle-Width="11em" />
                                    <asp:BoundField HeaderText="Status" DataField="PrintStatus" ItemStyle-Width="9em" />
                                    <asp:BoundField HeaderText="Who" DataField="StatusSetBy" ItemStyle-Width="8em" />
                                    <asp:BoundField HeaderText="Printer" DataField="PrinterName" ItemStyle-Width="16em" />
                                    <asp:BoundField HeaderText="Comment" DataField="Comment" ItemStyle-Width="10em" />
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
	        </ajaxToolkit:TabPanel>

	    </ajaxToolkit:TabContainer>

        <br />
        <div style="float:right;">
        <asp:button runat="server" text="Publish to Extranet" id="btnPublish" />
        <input id="btnView" type="button" value="View" onClick="DownloadDocument();" />
        <asp:button runat="server" text="Delete" id="btnDelete"
            OnClientClick="return window.confirm('Are you sure you wish to delete this document?');" />
        <input id="btnClose" type="button" value="Close" onclick="btnClose_Click()" />
        </div>
        
        <div class="clearer"></div>
                    
       <script type="text/javascript">
           var documentID = null;
           
           addEvent(window, "unload", DialogUnload);

           function btnClose_Click()
           {
               GetParentWindow().HideModalDIV();
               window.parent.close();
           }

           function RefreshParentAndClose() {
               // refresh document list in parent window
               GetParentWindow().FetchDocumentList(1, 0);
               btnClose_Click();
           }

           function DownloadDocument() {
               document.location.href = 'Documents/DocumentDownloadHandler.axd?id=<%=DocumentID%>&saveas=1';
           }
                      
       </script>

    </asp:Panel>
        
</asp:content>
