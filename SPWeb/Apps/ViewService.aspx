<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewService.aspx.vb" Inherits="Target.SP.Web.Apps.ViewService"
	EnableViewState="True" AspCompat="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="PropertySelector" Src="UserControls/PropertySelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="AddressContact" Src="UserControls/AddressContact.ascx" %>

<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details for the selected service.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" value="Back"  title="Navigates to the previous screen." onclick="document.location.href=unescape(GetQSParam(document.location.search, 'backUrl'));" />
	    <input type="button" id="btnProvider" value="Provider" title="Click here to view the provider of this service." runat="server" />
	    <input type="button" id="btnSubsidies" value="Subsidies" title="Click here to view the subsidies for this service." runat="server" />
	    <input type="button" id="btnEdit" runat="server" value="Edit" title="Request amendments to the data on this screen." onclick="javascript:document.location.href=AddQSParam(RemoveQSParam(document.location.href, 'editMode'), 'editMode', '1');" NAME="btnEdit"/>
	    <input type="button" id="btnCancel" runat="server" value="Cancel" title="Do not proceed with the amendment request." onclick="javascript:document.location.href=AddQSParam(RemoveQSParam(document.location.href, 'editMode'), 'editMode', '0');" NAME="btnCancel"/>
	    <br />
	    <br />
	    <asp:Label id="lblAmendReq" runat="server" CssClass="warningText" Visible="False">
		    Your new <a href="../../Apps/AmendReq/ListAmendReq.aspx">amendment request(s)</a> have been submitted.<br /><br />
	    </asp:Label>
    	
	    <ajaxToolkit:TabContainer runat="server" ID="TabStrip">
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel1" HeaderText="Service Details">
                <ContentTemplate>
                    <asp:Label CssClass="label" runat="server" AssociatedControlID="lblProviderName">Provider Name</asp:Label> 
		            <asp:Label id="lblProviderName" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblProviderAddress">Provider Address</asp:Label> 
		            <asp:Label id="lblProviderAddress" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblServiceName">Service Name</asp:Label> 
		            <asp:Label id="lblServiceName" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblReference">Reference</asp:Label> 
		            <asp:Label id="lblReference" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblSPINTLSID">Service ID</asp:Label> 
		            <asp:Label id="lblSPINTLSID" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <cc1:TextBoxEx id="txtPublicName" LabelText="Public Name" LabelWidth="20em" LabelBold="True" Width="20em" MaxLength="100"
			            Required="True" RequiredValidatorErrMsg="Please enter a Public Name"
			            EditableDataItemConstant="SPamendReqDataItemServicePublicName" 
			            EditableDataFieldConstant="SPamendReqDataFieldServicePublicNamePublicName"
			            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
		            <div class="clearer"></div>
		            <cc1:TextBoxEx id="txtDescription" LabelText="Description" LabelWidth="20em" LabelBold="True" Width="20em"
			            Required="True" RequiredValidatorErrMsg="Please enter a Description"
			            EditableDataItemConstant="SPamendReqDataItemServiceDescription" 
			            EditableDataFieldConstant="SPamendReqDataFieldServiceDescriptionDescription"
			            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
		            <div class="clearer"></div>
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblServiceType">Service Type</asp:Label> 
		            <asp:Label id="lblServiceType" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblAccElement">Acc. Element</asp:Label> 
		            <asp:Label id="lblAccElement" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblVATExempt">VAT Exempt</asp:Label> 
		            <asp:Label id="lblVATExempt" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblPatch">Patch</asp:Label> 
		            <asp:Label id="lblPatch" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblSPService">SP Service</asp:Label> 
		            <asp:Label id="lblSPService" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblIncludeSPLSExtract">Include in SPLS Extract</asp:Label> 
		            <asp:Label id="lblIncludeSPLSExtract" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblPipelineService">Pipeline Service</asp:Label> 
		            <asp:Label id="lblPipelineService" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblCommencementDate">Commencement Date</asp:Label> 
		            <asp:Label id="lblCommencementDate" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblDecommissionDate">Decommission Date</asp:Label> 
		            <asp:Label id="lblDecommissionDate" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblNoWeeksBasis">No. of Weeks Basis</asp:Label> 
		            <asp:Label id="lblNoWeeksBasis" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblUrbanRural">Urban or Rural</asp:Label> 
		            <asp:Label id="lblUrbanRural" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblEmergencyReferral">Emergency Referral</asp:Label> 
		            <asp:Label id="lblEmergencyReferral" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblAccessToInterpreters">Access to Interpreters</asp:Label> 
		            <asp:Label id="lblAccessToInterpreters" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblVisualImpairment">Support for Visual Impairment</asp:Label> 
		            <asp:Label id="lblVisualImpairment" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblHearingImpairment">Support for Hearing Impairment</asp:Label> 
		            <asp:Label id="lblHearingImpairment" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblWaitingList">Waiting List in Operation</asp:Label> 
		            <asp:Label id="lblWaitingList" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblImpactAssessment">Impact Assessment</asp:Label> 
		            <asp:Label id="lblImpactAssessment" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblRiskAssessment">Risk Assessment</asp:Label> 
		            <asp:Label id="lblRiskAssessment" runat="server" CssClass="content"></asp:Label>
		            <br />		
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel2" HeaderText="Manager Address">
                <ContentTemplate>
                    <uc2:AddressContact id="managerAddress" runat="server"></uc2:AddressContact> 
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel3" HeaderText="Emergency Address">
                <ContentTemplate>
                    <uc2:AddressContact id="emergencyAddress" runat="server"></uc2:AddressContact> 
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel4" HeaderText="Contact Address">
                <ContentTemplate>
                    <uc2:AddressContact id="contractAddress" runat="server"></uc2:AddressContact> 
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel5" HeaderText="Self Referral Address">
                <ContentTemplate>
                    <uc2:AddressContact id="selfReferralAddress" runat="server"></uc2:AddressContact> 
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel6" HeaderText="Self Referral Address 2">
                <ContentTemplate>
                    <uc2:AddressContact id="selfReferralAddress2" runat="server"></uc2:AddressContact>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel7" HeaderText="Service Districts">
                <ContentTemplate>
                    <asp:Repeater id="rptServiceDistricts" runat="server">
			    <headertemplate>
				    <table class="listTable" id="ServiceDestricts" style="" cellspacing="0" cellpadding="2" summary="Displays information regarding service districts.">
				    <caption>Service Districts.</caption>
			        <thead>
				        <tr>
					        <th style="width:50%">District</th>
					        <th style="width:50%">Proportion</th>
				        </tr>
			        </thead>
			        <tbody>
			        </headertemplate>
			        <itemtemplate>
				        <tr>
					        <td><%# HttpUtility.HtmlEncode(DataBinder.Eval(Container.DataItem, "District")) %></td>
					        <td><%# HttpUtility.HtmlEncode(DataBinder.Eval(Container.DataItem, "Proportion")) %></td>
				        </tr>
			        </itemtemplate>
			        <footertemplate>
				        </tbody>
				        </table>
			        </footertemplate>
		        </asp:Repeater>
		        <br />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel8" HeaderText="Description">
                <ContentTemplate>
                    <label class="label" for="divSupportProvisions">Support Provisions</label> 
		            <div id="divSupportProvisions" class="content"><asp:Literal id="litSupportProvisions" runat="server" ></asp:Literal></div>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblSupportDuration">Support Duration</asp:Label> 
		            <asp:Label id="lblSupportDuration" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblServiceDelivery">Service Delivery</asp:Label> 
		            <asp:Label id="lblServiceDelivery" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <label class="label" for="divEligibleTasks">Eligible Tasks</label> 
		            <div id="divEligibleTasks" class="content"><asp:Literal id="litEligibleTasks" runat="server" ></asp:Literal></div>
		            <br />
		            <label class="label" for="divNonEligibleTasks">Non-Eligible Tasks</label> 
		            <div id="divNonEligibleTasks" class="content"><asp:Literal id="litNonEligibleTasks" runat="server" ></asp:Literal></div>
		            <br />
		            <span class="label">Level of Service</span> 
		            <asp:Label id="lblLevelofService" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <label class="label" for="divSupportedLanguages">Supported Languages</label> 
		            <div id="divSupportedLanguages" class="content"><asp:Literal id="litSupportedLanguages" runat="server" ></asp:Literal></div>
		            <br />
		            <label class="label" for="divSupportedReligions">Supported Religions</label> 
		            <div id="divSupportedReligions" class="content"><asp:Literal id="litSupportedReligions" runat="server" ></asp:Literal></div>
		            <br />
		            <label class="label" for="divReferralRoutes">Referral Access Routes</label> 
		            <div id="divReferralRoutes" class="content"><asp:Literal id="litReferralRoutes" runat="server" ></asp:Literal></div>
		            <br />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel9" HeaderText="Availability">
                <ContentTemplate>
                    <asp:Label CssClass="label" runat="server" AssociatedControlID="lblPrimaryClientGroup">Primary Service User Group</asp:Label> 
		            <asp:Label id="lblPrimaryClientGroup" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <label class="label" for="divPGAgesSupported">Ages Supported</label> 
		            <div id="divPGAgesSupported" class="content"><asp:Literal id="litPGAgesSupported" runat="server" ></asp:Literal></div>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblSecondaryClientGroup">Secondary Service User Group</asp:Label> 
		            <asp:Label id="lblSecondaryClientGroup" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <label class="label" for="divSGAgesSupported">Ages Supported</label> 
		            <div id="divSGAgesSupported" class="content"><asp:Literal id="litSGAgesSupported" runat="server" ></asp:Literal></div>
		            <br />
		            <span class="label">Ethnic Groups Supported</span>
		            <div id="divEthincGroups" class="content"><asp:Literal id="litEthnicGroups" runat="server" ></asp:Literal></div>
		            <br />
		            <label class="label" for="divHouseholdGroups">Household Groups Supported</label> 
		            <div id="divHouseholdGroups" class="content"><asp:Literal id="litHouseholdGroups" runat="server" ></asp:Literal></div>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblCulturalGroup">Cultural Group</asp:Label> 
		            <asp:Label id="lblCulturalGroup" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblHouseholdUnits">Household Units Available</asp:Label> 
		            <asp:Label id="lblHouseholdUnits" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <label class="label" for="divReferralRoute">Referral Route</label> 
		            <div id="divReferralRoute" class="content"><asp:Literal id="litReferralRoute" runat="server" ></asp:Literal></div>
		            <br />
		            <label class="label" for="divUserExclusions">User Exclusions</label> 
		            <div id="divUserExclusions" class="content"><asp:Literal id="litUserExclusions" runat="server" ></asp:Literal></div>
		            <br />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel10" HeaderText="Staff Details">
                <ContentTemplate>
                    <span class="label">No. Standard Hours per Week</span> 
		            <asp:Label id="lblStandardHours" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblNoPaidManagers">No. Paid Managers (FTE)</asp:Label> 
		            <asp:Label id="lblNoPaidManagers" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblNoPaidFrontLine">No. Paid Front Line Staff (FTE)</asp:Label> 
		            <asp:Label id="lblNoPaidFrontLine" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblNoUnpaidManagers">No. Unpaid Managers (FTE)</asp:Label> 
		            <asp:Label id="lblNoUnpaidManagers" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblNoUnpaidFrontLine">No. Unpaid Front Line Staff (FTE)</asp:Label> 
		            <asp:Label id="lblNoUnpaidFrontLine" runat="server" CssClass="content"></asp:Label>
		            <br />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel11" HeaderText="User Involvement">
                <ContentTemplate>
                    <asp:Label CssClass="label" runat="server" AssociatedControlID="lblMechanismsInPlace">Machanisms in Place</asp:Label> 
		            <asp:Label id="lblMechanismsInPlace" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <label class="label" for="divConsultationMethods">Consultation Methods Used</label> 
		            <div id="divConsultationMethods" class="content"><asp:Literal id="litConsultationMethods" runat="server" ></asp:Literal></div>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblConsultationFrequency">Consultation Frequency</asp:Label> 
		            <asp:Label id="lblConsultationFrequency" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblConsultationStrategy">Current Consultation Strategy</asp:Label> 
		            <asp:Label id="lblConsultationStrategy" runat="server" CssClass="content"></asp:Label>
		            <br />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel12" HeaderText="HIA">
                <ContentTemplate>
                    <asp:Label CssClass="label" runat="server" AssociatedControlID="lblNoEnquiries">No. of Enquiries</asp:Label> 
		            <asp:Label id="lblNoEnquiries" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblNoHouseholdsAssisted">No. Households Assisted</asp:Label> 
		            <asp:Label id="lblNoHouseholdsAssisted" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblFeeIncome">Fee Income</asp:Label> 
		            <asp:Label id="lblFeeIncome" runat="server" CssClass="content"></asp:Label>
		            <br />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel13" HeaderText="Contracting Information">
                <ContentTemplate>
                    <asp:Label CssClass="label" runat="server" AssociatedControlID="lblChargeable">Chargeable Service</asp:Label> 
		            <asp:Label id="lblChargeable" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblExemptionReason">Exemption Reason</asp:Label> 
		            <asp:Label id="lblExemptionReason" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblGrossOrSubsidy">Gross or Subsidy</asp:Label> 
		            <asp:Label id="lblGrossOrSubsidy" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblNationalImportance">Service of National Importance</asp:Label> 
		            <asp:Label id="lblNationalImportance" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblServiceReviewDate">Service Review Date</asp:Label> 
		            <asp:Label id="lblServiceReviewDate" runat="server" CssClass="content"></asp:Label>
		            <br />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel14" HeaderText="Property">
                <ContentTemplate>
                    <uc1:PropertySelector id="PropertySelector1" runat="server"></uc1:PropertySelector>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    	
	    <br />
	    <asp:ValidationSummary ID="valSum" runat="server"
            HeaderText="Please correct the following error(s) before proceeding:"
            />
        <asp:button id="btnSubmit" runat="server" Text="Submit" Title="Click here to submit your amendment requests."></asp:button>
	    <br />
	    <br />
    </asp:Content>