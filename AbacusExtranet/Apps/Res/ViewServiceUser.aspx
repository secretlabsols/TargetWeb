<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewServiceUser.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Res.ViewServiceUser" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details for the selected service user.
	</asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" id="btnBack" value="Back" onclick="btnback_Click();" />
	    <br />
	    <br />
	    <div class="header">
		    <ajaxToolkit:TabContainer runat="server" ID="TabStrip">
                <ajaxToolkit:TabPanel runat="server" ID="TabPanel1" HeaderText="Basic Details">
                    <ContentTemplate>
			            <label class="label" for="lblReference">Reference</label> 
			            <asp:Label id="lblReference" runat="server" CssClass="content"></asp:Label>
			            <br />
			            <label class="label" for="cboTitle">Title</label> 
			            <cc1:DropDownListEx id="cboTitle" IsReadOnly="True" ReadOnlyContentCssClass="content" 
				            runat="server"></cc1:DropDownListEx>
            			
			            <cc1:TextBoxEx id="txtFirstName" LabelText="First Name(s)" LabelWidth="9.5em" LabelBold="True"
				            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			            <br />
			            <cc1:TextBoxEx id="txtSurname" LabelText="Surname" LabelWidth="9.5em" LabelBold="True"
				            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			            <br />
			            <label class="label" for="cboGender">Gender</label> 
			            <cc1:DropDownListEx id="cboGender" IsReadOnly="True" ReadOnlyContentCssClass="content" 
				            runat="server"></cc1:DropDownListEx>
            				
			            <cc1:TextBoxEx id="txtNINo" LabelText="NI No." LabelWidth="9.5em" LabelBold="True"
				            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
			            <br />
			            <cc1:TextBoxEx id="txtDateOfBirth" LabelText="Date of Birth" LabelWidth="9.5em" LabelBold="True"
				            ReadOnlyContentCssClass="content" IsReadOnly="True" Format="DateFormat" runat="server"></cc1:TextBoxEx>
			            <br />
			            <cc1:TextBoxEx id="txtDateOfDeath" LabelText="Date of Death" LabelWidth="9.5em" LabelBold="True"
				            ReadOnlyContentCssClass="content" IsReadOnly="True" Format="DateFormat" runat="server"></cc1:TextBoxEx>
			            <br />
			            <br />
			            <fieldset id="grpCareManager" style="width:50%;">
				            <legend>Care Manager</legend>
				            <cc1:TextBoxEx id="txtCareManagerName" LabelText="Name" LabelWidth="9.5em" LabelBold="True"
					            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
				            <br />
				            <cc1:TextBoxEx id="txtCareManagerPhone" LabelText="Phone" LabelWidth="9.5em" LabelBold="True"
					            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
				            <br />
				            <cc1:TextBoxEx id="txtCareManagerFax" LabelText="Fax" LabelWidth="9.5em" LabelBold="True"
					            ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
				            <br />
			            </fieldset>
			            <br />
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="TabPanel2" HeaderText="Residential Care">
                    <ContentTemplate>
                        <div id="divResCare" runat="server"></div>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
		    </ajaxToolkit:TabContainer>	    
	    </div>
	    <div class="clearer"></div>
	    <br />

    </asp:Content>