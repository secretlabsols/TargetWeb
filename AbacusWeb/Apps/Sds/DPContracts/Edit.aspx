<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Edit.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.DPContracts.Edit" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceBudgetHolder" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceBudgetHolderSelector.ascx" %>
<%@ Register TagPrefix="DS" TagName="DocumentSelector" Src="~/AbacusWeb/Apps/Documents/UserControls/DocumentSelector.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc3" TagName="Notes" Src="~/AbacusWeb/Apps/UserControls/NotesSelector.ascx" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to create new and view/edit existing Direct Payment.
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtonBack" runat="server"></uc1:StdButtons>
    <ajaxToolkit:TabContainer runat="server" ID="tabStrip" OnClientActiveTabChanged="tabStrip_ActiveTabChanged" EnableViewState="false">
        <ajaxToolkit:TabPanel runat="server" ID="tabContract" HeaderText="Contract">
            <ContentTemplate>                
                <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
                <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
			    <div style="clear:both;"></div>				
                <asp:Panel id="pnlContract" runat="server">
                    <div style="float:left;">
                        <cc1:TextBoxEx id="txtContractNum" runat="server" LabelText="Number" LabelWidth="13.5em" 
                            Width="10em" MaxLength="10" Required="true" RequiredValidatorErrMsg="Please specify a contract number" ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                    </div>
                    <div style="float:left; vertical-align:middle;">
                        &nbsp;&nbsp;<asp:Label id="lblSDS" AssociatedControlID="txtContractNum" runat="server" Text="(Self Directed Support)" Width="25em"></asp:Label>	
                    </div>
				    <div style="clear:both;"></div>				
                    <asp:Label id="lblClient" AssociatedControlID="client" runat="server" Text="Service User" Width="13.2em"></asp:Label>
                    <uc3:InPlaceClient id="client" runat="server"></uc3:InPlaceClient>
                    <br />
                    <asp:Label id="lblBudgetHolder" AssociatedControlID="budgetholder" runat="server" Text="Budget Holder" Width="13.2em"></asp:Label>
                    <uc4:InPlaceBudgetHolder id="budgetholder" runat="server"></uc4:InPlaceBudgetHolder>
                    <br />
                    <br />
                    <cc1:TextBoxEx id="txtAltRef" runat="server" LabelText="Alt Reference" LabelWidth="13.5em" Width="10em" MaxLength="10"></cc1:TextBoxEx>
                    <br />
                    <cc1:TextBoxEx id="txtDateFrom" runat="server" Format="DateFormat" LabelText="Date From" LabelWidth="13.5em"
                        Required="true" RequiredValidatorErrMsg="Please enter a contract start date" ValidationGroup="Save"></cc1:TextBoxEx>
                    <br />
                    <div style="float:left;">
                        <cc1:TextBoxEx id="txtDateTo" runat="server" IsReadOnly="true" Format="DateFormat" LabelText="Date To" LabelWidth="13.5em"></cc1:TextBoxEx>
	                    <input type="hidden" id="hidDateTo" runat="server" />
	                    <br /><br />
                    </div>
                    <div style="float:left; vertical-align:middle;">
                        <cc1:TextBoxEx id="txtEndReason" runat="server" IsReadOnly="true" LabelText="" LabelWidth="2em" Width="13em"></cc1:TextBoxEx>
	                    <input type="hidden" id="hidEndReason" runat="server" />
	                </div>
				    <div style="clear:both;"></div>				
                    <cc1:DropDownListEx ID="cboContractGroup" runat="server" LabelText="Contract Group" LabelWidth="13.5em"></cc1:DropDownListEx>
	                <input type="hidden" id="hidContractGroup" runat="server" />
			        <br />
			        <cc1:DropDownListEx ID="cboServiceGroup" runat="server" LabelText="Service Group" LabelWidth="13.5em" Required="true" RequiredValidatorErrMsg="Please select a Service Group." ValidationGroup="Save" />
                </asp:Panel>
            </ContentTemplate>
	    </ajaxToolkit:TabPanel>
	    <ajaxToolkit:TabPanel runat="server" ID="tabPeriods" HeaderText="Periods">
            <ContentTemplate>
	            <asp:Panel id="pnlPeriods" runat="server">
		            <asp:Button id="btnAddPeriod" runat="server" text="Add" />
                    <asp:Label ID="lblError2" runat="server" CssClass="errorText"></asp:Label>
		            <hr />
    			    <asp:PlaceHolder id="phPeriods" runat="server"></asp:PlaceHolder>
                </asp:Panel>
        	</ContentTemplate>
        </ajaxToolkit:TabPanel>
	    <ajaxToolkit:TabPanel runat="server" ID="tabPayments" HeaderText="Payments">
            <ContentTemplate>
	            <asp:Panel id="pnlPayments" runat="server">
		            <asp:Button id="btnAddPayment" runat="server" text="Add" />
                    <asp:Label ID="lblError3" runat="server" CssClass="errorText"></asp:Label>
		            <hr />
    			    <asp:PlaceHolder id="phPayments" runat="server"></asp:PlaceHolder>
                </asp:Panel>
        	</ContentTemplate>
        </ajaxToolkit:TabPanel>
	    <ajaxToolkit:TabPanel runat="server" ID="tabDocuments" HeaderText="Documents">
            <ContentTemplate>
	            <asp:Panel id="pnlDocuments" runat="server">
                    <DS:DocumentSelector id="docSelector" runat="server"></DS:DocumentSelector>
	            </asp:Panel>
            </ContentTemplate>
	    </ajaxToolkit:TabPanel>
	    <ajaxToolkit:TabPanel runat="server" ID="tabAuditLog" HeaderText="Audit Log">
            <ContentTemplate>
	            <asp:Panel id="pnlAuditLog" runat="server">
    			    <asp:PlaceHolder id="phAuditLog" runat="server"></asp:PlaceHolder>
	            </asp:Panel>
            </ContentTemplate>
	    </ajaxToolkit:TabPanel>
	    <ajaxToolkit:TabPanel runat="server" ID="tabNotes" HeaderText="Notes">
            <ContentTemplate>
	            <asp:Panel id="pnlNotes" runat="server">
	               <div id="notesdiv" runat="server"><uc3:Notes id="Notes1" runat="server"></uc3:Notes></div>
	            </asp:Panel>
            </ContentTemplate>
	    </ajaxToolkit:TabPanel>
	</ajaxToolkit:TabContainer>
	<input type="hidden" id="hidSelectedTabIndex" runat="server" />
    <br />
        
</asp:Content>