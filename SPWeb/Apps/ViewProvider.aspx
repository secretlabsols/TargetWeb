<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="AddressContact" Src="UserControls/AddressContact.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewProvider.aspx.vb" Inherits="Target.SP.Web.Apps.ViewProvider" 
	EnableViewState="True" AspCompat="True" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">Displayed below are the details of the selected provider.</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" value="Back" title="Navigates to the previous screen." onclick="document.location.href=unescape(GetQSParam(document.location.search, 'backUrl'));" />
	    <input type="button" value="Services" title="Display the services for this provider." onclick="document.location.href='ListServices.aspx?providerID=' + GetQSParam(document.location.search, 'id') + '&amp;currentStep=1&amp;backUrl=' + escape(document.location.href);" />
	    <input type="button" id="btnEdit" runat="server" value="Edit" title="Request amendments to the data on this screen." onclick="javascript:document.location.href=AddQSParam(RemoveQSParam(document.location.href, 'editMode'), 'editMode', '1');" />
	    <input type="button" id="btnCancel" runat="server" value="Cancel" title="Do not proceed with the amendment request." onclick="javascript:document.location.href=AddQSParam(RemoveQSParam(document.location.href, 'editMode'), 'editMode', '0');" />
	    <br />
	    <br />
	    <asp:Label id="lblAmendReq" runat="server" CssClass="warningText" Visible="False">
		    Your new <a href="../../Apps/AmendReq/ListAmendReq.aspx">amendment request(s)</a> have been submitted.<br /><br />
	    </asp:Label>
    	
        <ajaxToolkit:TabContainer runat="server" ID="TabStrip">
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel1" HeaderText="Provider Details">
                <ContentTemplate>
                    <asp:Label CssClass="label" runat="server" AssociatedControlID="litReference">Reference</asp:Label> 
		            <asp:Label id="litReference" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <cc1:TextBoxEx id="txtLocalProviderID" LabelText="Local Provider ID" LabelWidth="15em" LabelBold="True"
			            EditableDataItemConstant="SPamendReqDataItemProviderLocalProviderID" 
			            EditableDataFieldConstant="SPamendReqDataFieldProviderLocalProviderIDLocalProviderID"
			            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="litName">Name</asp:Label> 
		            <asp:Label id="litName" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <cc1:TextBoxEx id="txtWebsite" LabelText="Website" LabelWidth="15em" LabelBold="True"
			            EditableDataItemConstant="SPamendReqDataItemProviderWebsite" 
			            EditableDataFieldConstant="SPamendReqDataFieldProviderWebsiteWebsite"
			            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="litOrgType">Organisation Type</asp:Label> 
		            <asp:Label id="litOrgType" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="litCreditorsRef">Creditors Reference</asp:Label> 
		            <asp:Label id="litCreditorsRef" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="litVATExempt">VAT Exempt</asp:Label> 
		            <asp:Label id="litVATExempt" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="litVATNumber">VAT Number</asp:Label> 
		            <asp:Label id="litVATNumber" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <cc1:TextBoxEx id="dteYearEndDate" LabelText="Year End Date" LabelWidth="15em" LabelBold="True" Format="DateFormat"
			            EditableDataItemConstant="SPamendReqDataItemProviderYearEndDate" 
			            EditableDataFieldConstant="SPamendReqDataFieldProviderYearEndDateYearEndDate"
			            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
		            <br />
		            <label class="label" for="divProviderType">Provider Type</label> 
		            <div id="divProviderType" class="content"><asp:Literal id="litProviderType" runat="server" ></asp:Literal></div>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="litAccreditationExempt">Accreditation Exempt</asp:Label> 
		            <asp:Label id="litAccreditationExempt" runat="server" CssClass="content"></asp:Label>
		            <div class="clearer"></div>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel2" HeaderText="Provider Address">
                <ContentTemplate>
                    <uc1:AddressContact id="providerAddress" runat="server"></uc1:AddressContact>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel3" HeaderText="Contact Address">
                <ContentTemplate>
                    <uc1:AddressContact id="contactAddress" runat="server"></uc1:AddressContact>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
             <ajaxToolkit:TabPanel runat="server" ID="TabPanel4" HeaderText="Billing Address">
                <ContentTemplate>
                    <uc1:AddressContact id="billingAddress" runat="server"></uc1:AddressContact>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel5" HeaderText="Ethnicity">
                <ContentTemplate>
                    <asp:Label CssClass="label" runat="server" AssociatedControlID="litBMESpecific">BME Specific Provider</asp:Label> 
		            <asp:Label id="litBMESpecific" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <label class="label" for="divEthnicOrigins">Supported Ethnic Origins</label> 
		            <div id="divEthnicOrigins" class="content"><asp:Literal id="litEthnicOrigins" runat="server" ></asp:Literal></div>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="litCulturalGroup">Cultural Group</asp:Label> 
		            <asp:Label id="litCulturalGroup" runat="server" CssClass="content"></asp:Label>
		            <br />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel6" HeaderText="Registration Details">
                <ContentTemplate>
                    <asp:Repeater id="Repeater1" runat="server">
			            <headertemplate>
				            <table class="listTable" id="RegistrationDetails" style="" cellspacing="0" cellpadding="2" summary="Displays your providers.">
				            <caption>Registration Details.</caption>
				            <thead>
					            <tr>
						            <th style="width:40%">Registration Type</th>
						            <th style="width:40%">Registration Number</th>
						            <th style="width:20%">Date of Registration</th>
					            </tr>
				            </thead>
				            <tbody>
					            <% If Not HaveRegistrationDetails() Then %>
						            <tr><td></td><td></td><td></td></tr>
					            <% End If %>
			            </headertemplate>
			            <itemtemplate>
				            <tr>
					            <td><%# HttpUtility.HtmlEncode(DataBinder.Eval(Container.DataItem, "RegType")) %></td>
					            <td><%# HttpUtility.HtmlEncode(DataBinder.Eval(Container.DataItem, "RegNumber")) %></td>
					            <td><%# GetRegistrationDate(DataBinder.Eval(Container.DataItem, "RegDate")) %>&nbsp;</td>
				            </tr>
			            </itemtemplate>
			            <footertemplate>
				            </tbody>
				            </table>
			            </footertemplate>
		            </asp:Repeater>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    	
	    <br />
	    <asp:ValidationSummary ID="valSum" runat="server"
            HeaderText="Please correct the following error(s) before proceeding:"
            />
	    <asp:button id="btnSubmit" runat="server" Text="Submit" title="Click here to submit your amendment requests."></asp:button>
	    <br />
	    <br />
    </asp:Content>