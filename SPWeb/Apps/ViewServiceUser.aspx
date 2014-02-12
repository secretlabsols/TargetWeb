<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewServiceUser.aspx.vb" Inherits="Target.SP.Web.Apps.ViewServiceUser"
	EnableViewState="True" AspCompat="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="AddressContact" Src="UserControls/AddressContact.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">Displayed below are the details of the selected service user.</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" value="Back"  title="Navigates to the previous screen." onclick="javascript:history.back()" />
	    <input type="button" id="btnEdit" runat="server" value="Edit" title="Request amendments to the data on this screen." onclick="javascript:document.location.href=AddQSParam(RemoveQSParam(document.location.href, 'editMode'), 'editMode', '1');" NAME="btnEdit"/>
	    <input type="button" id="btnCancel" runat="server" value="Cancel" title="Do not proceed with the amendment request." onclick="javascript:document.location.href=AddQSParam(RemoveQSParam(document.location.href, 'editMode'), 'editMode', '0');" NAME="btnCancel"/>
	    <br />
	    <br />
	    <asp:Label id="lblAmendReq" runat="server" CssClass="warningText" Visible="False">
		    Your new <a href="../../Apps/AmendReq/ListAmendReq.aspx">amendment request(s)</a> have been submitted.<br /><br />
	    </asp:Label>
	    <label class="label" for="lblReference">Reference</label> 
	    <ASP:Label id="lblReference" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="cboTitle">Title</label> 
	    <cc1:DropDownListEx id="cboTitle" IsReadOnly="True" ReadOnlyContentCssClass="content" 
		    EditableDataItemConstant="SPamendReqDataItemClientTitle" 
		    EditableDataFieldConstant="SPamendReqDataFieldClientTitleTitle"
		    runat="server"></cc1:DropDownListEx>
    	
	    <cc1:TextBoxEx id="txtFirstName" LabelText="First Name(s)" LabelWidth="9.5em" LabelBold="True"
		    Required="True" RequiredValidatorErrMsg="Please enter a First Name(s)"
		    EditableDataItemConstant="SPamendReqDataItemClientForenames" 
		    EditableDataFieldConstant="SPamendReqDataFieldClientForenamesForenames"
		    ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
	    <br />
	    <cc1:TextBoxEx id="txtSurname" LabelText="Surname" LabelWidth="9.5em" LabelBold="True"
		    Required="True" RequiredValidatorErrMsg="Please enter a Surname"
		    EditableDataItemConstant="SPamendReqDataItemClientSurname" 
		    EditableDataFieldConstant="SPamendReqDataFieldClientSurnameSurname"
		    ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
	    <br />
	    <cc1:TextBoxEx id="txtDateOfBirth" LabelText="Date of Birth" LabelWidth="9.5em" LabelBold="True"
		    Required="True" RequiredValidatorErrMsg="Please enter a Date of Birth"
		    EditableDataItemConstant="SPamendReqDataItemClientDateOfBirth" 
		    EditableDataFieldConstant="SPamendReqDataFieldClientDateOfBirthDateOfBirth"
		    ReadOnlyContentCssClass="content" IsReadOnly="True" Format="DateFormat" runat="server"></cc1:TextBoxEx>
	    <br />
	    <cc1:TextBoxEx id="txtNINo" LabelText="NI No." LabelWidth="9.5em" LabelBold="True"
		    Required="True" RequiredValidatorErrMsg="Please enter a National Insurance Number"
		    EditableDataItemConstant="SPamendReqDataItemClientNINumber" 
		    EditableDataFieldConstant="SPamendReqDataFieldClientNINumberNINumber"
		    ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
	    <br />
	    <br />
    	
	    <fieldset id="grpAddress" runat="server">
		    <legend>Property Address</legend>
		    <uc1:AddressContact id="propertyAddress" runat="server"></uc1:AddressContact> 
	    </fieldset>
    		
	    <div class="clearer"></div>
	    <br />
	    <asp:ValidationSummary ID="valSum" runat="server"
            HeaderText="Please correct the following error(s) before proceeding:"
            />
	    <asp:button id="btnSubmit" runat="server" Text="Submit" title="Click here to submit your amendment requests."></asp:button>
	    <br />
	    <br />
    </asp:Content>