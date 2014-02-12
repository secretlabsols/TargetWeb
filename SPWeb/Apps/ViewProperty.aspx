<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewProperty.aspx.vb" Inherits="Target.SP.Web.Apps.ViewProperty"
	EnableViewState="True" AspCompat="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="AddressContact" Src="UserControls/AddressContact.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">Displayed below are the details of the selected property.</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" value="Back"  title="Navigates to the previous screen." onclick="javascript:history.back()" />
	    <input type="button" style="width:10em;" value="Occupancy Enquiry" title="View the Occupancy of a Provider for a Particular Service." onclick="document.location.href='OccupancyEnq.aspx?pid=' + GetQSParam(document.location.search, 'pid') + '&amp;sid=' + GetQSParam(document.location.search, 'sid') ;" />
	    <input type="button" id="btnEdit" runat="server" value="Edit" title="Request amendments to the data on this screen." onclick="javascript:document.location.href=AddQSParam(RemoveQSParam(document.location.href, 'editMode'), 'editMode', '1');" NAME="btnEdit"/>
	    <input type="button" id="btnCancel" runat="server" value="Cancel" title="Do not proceed with the amendment request." onclick="javascript:document.location.href=AddQSParam(RemoveQSParam(document.location.href, 'editMode'), 'editMode', '0');" NAME="btnCancel"/>
	    <br />
	    <br />
	    <asp:Label id="lblAmendReq" runat="server" CssClass="warningText" Visible="False">
		    Your new <a href="../../Apps/AmendReq/ListAmendReq.aspx">amendment request(s)</a> have been submitted.<br /><br />
	    </asp:Label>
    	
	    <ajaxToolkit:TabContainer runat="server" ID="TabStrip">
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel1" HeaderText="Property Details">
                <ContentTemplate>
                    <asp:Label CssClass="label" runat="server" AssociatedControlID="lblPropertyName">Property Name</asp:Label> 
		            <asp:Label id="lblPropertyName" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblReference">Reference</asp:Label> 
		            <asp:Label id="lblReference" runat="server" CssClass="content"></asp:Label>
		            <br />
		            <cc1:TextBoxEx id="txtAltReference" LabelText="Alt Reference" LabelWidth="20em" LabelBold="True" MaxLength="20"
			            EditableDataItemConstant="SPamendReqDataItemPropertyAltReference" 
			            EditableDataFieldConstant="SPamendReqDataFieldPropertyAltReferenceAltReference"
			            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
		            <br /><br />
		            <fieldset style="width:31.00em;" id="grpRegDetails" runat="server">
			            <legend>Registration Details</legend>
			            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblRegistrationStatus">Registration Status</asp:Label> 
			            <asp:Label id="lblRegistrationStatus" runat="server" CssClass="content3"></asp:Label>
			            <br />
			            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblRegistrationDate">Date of Registration</asp:Label> 
			            <asp:Label id="lblRegistrationDate" runat="server" CssClass="content3"></asp:Label>
			            <br />
			            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblDeRegistered">De-Registered in 12 Months</asp:Label> 
			            <asp:Label id="lblDeRegistered" runat="server" CssClass="content3"></asp:Label>
			            <br />
			            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblFurnishingType">Furnishing Type</asp:Label> 
			            <asp:Label id="lblFurnishingType" runat="server" CssClass="content3"></asp:Label>
			            <br />
		            </fieldset>
		            <br /><br />
		            <fieldset style="width:31.00em;" id="grpHouseholdUnits" runat="server">
			            <legend>No. of Household Units Available</legend>
			            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblWheelchair">Wheelchair</asp:Label> 
			            <asp:Label id="lblWheelchair" runat="server" CssClass="content3"></asp:Label>
			            <br />
			            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblMobility">Mobility</asp:Label> 
			            <asp:Label id="lblMobility" runat="server" CssClass="content3"></asp:Label>
			            <br />
			            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblAdaptions">Aids and Adaptions</asp:Label> 
			            <asp:Label id="lblAdaptions" runat="server" CssClass="content3"></asp:Label>
			            <br />
			            <asp:Label CssClass="label" runat="server" AssociatedControlID="lblTotal">Total</asp:Label> 
			            <asp:Label id="lblTotal" runat="server" CssClass="content3"></asp:Label>
			            <br />
		            </fieldset>
		            <br />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel2" HeaderText="Information">
                <ContentTemplate>
                    <fieldset style="width:31.00em;" id="grpFacilities" runat="server">
			        <legend>Facilities Available</legend>
			        <cc1:CheckBoxEx id="chkMealsSupplied" labelID="lblMealsSupplied" text="Meals Supplied" 
					        checkBoxCssClass="content2" EditableDataItemConstant="SPamendReqDataItemPropertyMealsSupplied" 
					        EditableDataFieldConstant="SPamendReqDataFieldPropertyMealsSuppliedMealsSupplied"
					        IsReadOnly="True" runat="server"></cc1:CheckBoxEx> 
			        <br />
			        <br />
			        <cc1:CheckBoxEx id="chkCanteen" labelID="lblCanteen" text="Canteen on Site" 
					        checkBoxCssClass="content2" EditableDataItemConstant="SPamendReqDataItemPropertyCanteenOnSite" 
					        EditableDataFieldConstant="SPamendReqDataFieldPropertyCanteenOnSiteCanteenOnSite"
					        IsReadOnly="True" runat="server"></cc1:CheckBoxEx> 
			        <br />
			        <br />
			        <cc1:CheckBoxEx id="chkSelfCatering" labelid="lblSelfCatering" text="Self Catering" 
					        checkBoxCssClass="content2" EditableDataItemConstant="SPamendReqDataItemPropertySelfCatering" 
					        EditableDataFieldConstant="SPamendReqDataFieldPropertySelfCateringSelfCatering"
					        IsReadOnly="True"  runat="server"></cc1:CheckBoxEx> 
			        <br />
			        <br />				
		        </fieldset>
		        <br /><br />
		        <fieldset style="width:31.00em;" id="grpDistanceTo" runat="server">
			        <legend>Distance To</legend>
			        <cc1:TextBoxEx id="Shops" LabelText="Shops" LabelWidth="12em" LabelBold="True" Format="CurrencyFormat"
				        Required="True" RequiredValidatorErrMsg="Please enter Distance to Shops"
				        EditableDataItemConstant="SPamendReqDataItemPropertyDistances" 
				        EditableDataFieldConstant="SPamendReqDataFieldPropertyDistancesShops"
				        ReadOnlyContentCssClass="content3" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			        <cc1:TextBoxEx id="TrainStation" LabelText="Train Station" LabelWidth="12em" LabelBold="True" Format="CurrencyFormat"
				        Required="True" RequiredValidatorErrMsg="Please enter Distance to Train Station"
				        EditableDataItemConstant="SPamendReqDataItemPropertyDistances" 
				        EditableDataFieldConstant="SPamendReqDataFieldPropertyDistancesTrainStation"
				        ReadOnlyContentCssClass="content3" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			        <cc1:TextBoxEx id="TownCentre" LabelText="Town Centre" LabelWidth="12em" LabelBold="True" Format="CurrencyFormat"
				        Required="True" RequiredValidatorErrMsg="Please enter Distance to Town Centre"
				        EditableDataItemConstant="SPamendReqDataItemPropertyDistances" 
				        EditableDataFieldConstant="SPamendReqDataFieldPropertyDistancesTownCentre"
				        ReadOnlyContentCssClass="content3" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			        <cc1:TextBoxEx id="GP" LabelText="GP" LabelWidth="12em" LabelBold="True" Format="CurrencyFormat"
				        Required="True" RequiredValidatorErrMsg="Please enter Distance to GP"
				        EditableDataItemConstant="SPamendReqDataItemPropertyDistances" 
				        EditableDataFieldConstant="SPamendReqDataFieldPropertyDistancesGP"
				        ReadOnlyContentCssClass="content3" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			        <cc1:TextBoxEx id="SecondarySchool" LabelText="Secondary School" LabelWidth="12em" LabelBold="True" Format="CurrencyFormat"
				        Required="True" RequiredValidatorErrMsg="Please enter Distance to Secondary School"
				        EditableDataItemConstant="SPamendReqDataItemPropertyDistances" 
				        EditableDataFieldConstant="SPamendReqDataFieldPropertyDistancesSecondarySchool"
				        ReadOnlyContentCssClass="content3" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			        <cc1:TextBoxEx id="PostOffice" LabelText="Post Office" LabelWidth="12em" LabelBold="True" Format="CurrencyFormat"
				        Required="True" RequiredValidatorErrMsg="Please enter Distance to Post Office"
				        EditableDataItemConstant="SPamendReqDataItemPropertyDistances" 
				        EditableDataFieldConstant="SPamendReqDataFieldPropertyDistancesPostOffice"
				        ReadOnlyContentCssClass="content3" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			        <cc1:TextBoxEx id="BusStop" LabelText="Bus Stop" LabelWidth="12em" LabelBold="True" Format="CurrencyFormat"
				        Required="True" RequiredValidatorErrMsg="Please enter Distance to Bus Stop"
				        EditableDataItemConstant="SPamendReqDataItemPropertyDistances" 
				        EditableDataFieldConstant="SPamendReqDataFieldPropertyDistancesBusStop"
				        ReadOnlyContentCssClass="content3" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			        <cc1:TextBoxEx id="SocialCentre" LabelText="Social Centre" LabelWidth="12em" LabelBold="True" Format="CurrencyFormat"
				        Required="True" RequiredValidatorErrMsg="Please enter Distance to Social Centre"
				        EditableDataItemConstant="SPamendReqDataItemPropertyDistances" 
				        EditableDataFieldConstant="SPamendReqDataFieldPropertyDistancesSocialCentre"
				        ReadOnlyContentCssClass="content3" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			        <cc1:TextBoxEx id="PrimarySchool" LabelText="Primary School" LabelWidth="12em" LabelBold="True" Format="CurrencyFormat"
				        Required="True" RequiredValidatorErrMsg="Please enter Distance to Primary School"
				        EditableDataItemConstant="SPamendReqDataItemPropertyDistances" 
				        EditableDataFieldConstant="SPamendReqDataFieldPropertyDistancesPrimarySchool"
				        ReadOnlyContentCssClass="content3" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
		        </fieldset>
		        <br />
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel3" HeaderText="Location">
                <ContentTemplate>
                    <uc1:AddressContact id="propertyAddress" runat="server"></uc1:AddressContact> 
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="TabPanel4" HeaderText="Providers">
                <ContentTemplate>
                    <fieldset style="width:51.00em;" id="grpServiceProvider" runat="server">
			        <legend>Service Provider</legend>
			        <asp:Label CssClass="label" runat="server" AssociatedControlID="lblServiceProvider">Service Provider</asp:Label> 
			        <asp:Label id="lblServiceProvider" runat="server" CssClass="content4"></asp:Label>
			        <br />
			        <asp:Label CssClass="label" runat="server" AssociatedControlID="lblSPAddress">Address</asp:Label> 
			        <asp:Label id="lblSPAddress" runat="server" CssClass="content4"></asp:Label>
			        <br />
			        <asp:Label CssClass="label" runat="server" AssociatedControlID="lblSPPostCode">PostCode</asp:Label> 
			        <asp:Label id="lblSPPostCode" runat="server" CssClass="content4"></asp:Label>
			        <br />
		        </fieldset>
		        <br /><br />
		        <fieldset style="width:51.00em;" id="grpAccomManager" runat="server">
			        <legend>Accomodation Manager</legend>
			        <asp:Label CssClass="label" runat="server" AssociatedControlID="lblAccomManager">Accomodation Manager</asp:Label> 
			        <asp:Label id="lblAccomManager" runat="server" CssClass="content4"></asp:Label>
			        <br />
			        <asp:Label CssClass="label" runat="server" AssociatedControlID="lblAMAddress">Address</asp:Label> 
			        <asp:Label id="lblAMAddress" runat="server" CssClass="content4"></asp:Label>
			        <br />
			        <asp:Label CssClass="label" runat="server" AssociatedControlID="lblAMPostCode">PostCode</asp:Label> 
			        <asp:Label id="lblAMPostCode" runat="server" CssClass="content4"></asp:Label>
			        <br />
		        </fieldset>
		        <br /><br />
		        <fieldset style="width:51.00em;" id="grpLandlord" runat="server">
			        <legend>Landlord</legend>
			        <asp:Label CssClass="label" runat="server" AssociatedControlID="lblLandlord">Landlord</asp:Label> 
			        <asp:Label id="lblLandlord" runat="server" CssClass="content4"></asp:Label>
			        <br />
			        <asp:Label CssClass="label" runat="server" AssociatedControlID="lblLAddress">Address</asp:Label> 
			        <asp:Label id="lblLAddress" runat="server" CssClass="content4"></asp:Label>
			        <br />
			        <asp:Label CssClass="label" runat="server" AssociatedControlID="lblLPostCode">PostCode</asp:Label> 
			        <asp:Label id="lblLPostCode" runat="server" CssClass="content4"></asp:Label>
			        <br />
		        </fieldset>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
        </ajaxToolkit:TabContainer>
    	
	    <br />
	    <asp:ValidationSummary ID="valSum" runat="server"
            HeaderText="Please correct the following error(s) before proceeding:"
            />
	    <asp:button id="btnSubmit" runat="server" Text="Submit" Title="Click here to submit your amendment requests."></asp:button>
	    <br />
    </asp:Content>