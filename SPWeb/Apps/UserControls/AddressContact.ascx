<%@ Control Language="vb" AutoEventWireup="false" Codebehind="AddressContact.ascx.vb" Inherits="Target.SP.Web.Apps.UserControls.AddressContact" 
	TargetSchema="http://schemas.microsoft.com/intellisense/ie5" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<ajaxToolkit:TabContainer runat="server" ID="TabStrip">
    <ajaxToolkit:TabPanel runat="server" ID="addressPanel" HeaderText="Address">
        <ContentTemplate>
            <cc1:TextBoxEx id="txtAddress" LabelText="Address" LabelWidth="15em" Width="20em" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtPostcode" LabelText="Postcode" LabelWidth="15em" LabelBold="True" MaxLength="10" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtAdminAuthority" LabelText="Admin Authority" LabelWidth="15em" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtDistrict" LabelText="District" LabelWidth="15em" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtWard" LabelText="Ward" LabelWidth="15em" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtDirections" LabelText="Directions" LabelWidth="15em" Width="20em" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtUPRN" LabelText="UPRN" LabelWidth="15em" LabelBold="True" MaxLength="12" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtUSRN" LabelText="USRN" LabelWidth="15em" LabelBold="True" MaxLength="8" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <span class="addressContactLabel">Confidential</span>
	        <asp:Label id="litConfidential" runat="server" CssClass="addressContactContent"></asp:Label>
	        <br />
	        <asp:Label id="lblDisabledAccess" class="addressContactLabel" runat="server">Disabled Access</asp:Label> 
	        <cc1:DropDownListEx id="cboDisabledAccess" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:DropDownListEx>
	        <br />
	        <span id="lblAddressAlsoUsedAs" class="addressContactLabel" runat="server">Also Used As</span>
	        <cc1:CheckedListBox id="chklstAlsoUsedAs" Width="15em" IsReadOnly="True" runat="server"></cc1:CheckedListBox>
	        <br />
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="contactPanel" HeaderText="Contact">
        <ContentTemplate>
            <asp:Label id="lblContactType" class="addressContactLabel" runat="server">Contact Type</asp:Label> 
	        <cc1:DropDownListEx id="cboContactType" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:DropDownListEx>
	        <br />
	        <cc1:TextBoxEx id="txtContactOrganisation" LabelText="Organisation" LabelWidth="15em" Width="20em" MaxLength="100" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <asp:Label id="lblContactTitle" class="addressContactLabel" runat="server">Title</asp:Label> 
	        <cc1:DropDownListEx id="cboContactTitle" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:DropDownListEx>
	        <br />
	        <cc1:TextBoxEx id="txtContactForenames" LabelText="Forenames" LabelWidth="15em" Width="20em" MaxLength="50" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtContactSurname" LabelText="Surname" LabelWidth="15em" Width="20em" MaxLength="50" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtContactPosition" LabelText="Position" LabelWidth="15em" Width="20em" MaxLength="80" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtContactTel" LabelText="Telephone No." LabelWidth="15em" Width="20em" MaxLength="20" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtContactFax" LabelText="Fax No." LabelWidth="15em" Width="20em" MaxLength="20" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtContactMobile" LabelText="Mobile No." LabelWidth="15em" Width="20em" MaxLength="20" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtContactPager" LabelText="Pager No." LabelWidth="15em" Width="20em" MaxLength="20" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtContactEmail" LabelText="Email Address" IsLink="True" LabelWidth="15em" Width="20em" MaxLength="50" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <cc1:TextBoxEx id="txtContactWeb" LabelText="Web Address" IsLink="True" LabelWidth="15em" Width="20em" MaxLength="100" LabelBold="True" IsReadOnly="True" ReadOnlyContentCssClass="addressContactContent" runat="server"></cc1:TextBoxEx>
	        <br />
	        <span id="lblContactAlsoUsedAs" class="addressContactLabel" runat="server">Also Used As</span>
	        <cc1:CheckedListBox id="chklstContactAlsoUsedAs" Width="15em" IsReadOnly="True" runat="server"></cc1:CheckedListBox>
	        <div class="clearer"></div>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
