<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BudgetHolders.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.BudgetHolders" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="DS" TagName="DocumentSelector" Src="~/AbacusWeb/Apps/Documents/UserControls/DocumentSelector.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc3" TagName="Notes" Src="~/AbacusWeb/Apps/UserControls/NotesSelector.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain budget holders.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <ajaxToolkit:TabContainer runat="server" ID="tabStrip" OnClientActiveTabChanged="tabStrip_ActiveTabChanged" EnableViewState="false">
        <%--Details Tab--%>
        <ajaxToolkit:TabPanel runat="server" ID="tabDetails" HeaderText="Details">
            <ContentTemplate>
                <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewState="false">
                    <div style="float:left;width:50%;">
                        <cc1:TextBoxEx ID="txtReference"  runat="server"  LabelText="Reference" LabelWidth="11em" MaxLength="255" 
                                Width="18em"  Required="true"  RequiredValidatorErrMsg="Please enter Reference" ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtOrganisationName"  runat="server"  LabelText="Organisation Name" LabelWidth="11em" MaxLength="255" 
                                Width="18em"  Required="false" 
                                ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtTitleInitials"  runat="server"  LabelText="Title & Initial" LabelWidth="11em" MaxLength="255" 
                                Width="18em"  Required="false" 
                                ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtSurname"  runat="server"  LabelText="Surname" LabelWidth="11em" MaxLength="255" 
                                Width="18em"  Required="false" 
                                ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtAddress" textmode="multiline" rows="4" runat="server"  LabelText="Address" LabelWidth="11em" MaxLength="255" 
                                Width="18em"  Required="false" 
                                ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtPostcode"  runat="server"  LabelText="Postcode" LabelWidth="11em" MaxLength="255" 
                                Width="18em"  Required="false"
                                ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtEmailAddress"  runat="server"  LabelText="Email Address" LabelWidth="11em" MaxLength="255" 
                                Width="18em"  Required="false" 
                                ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtTelephone"  runat="server"  LabelText="Telephone" LabelWidth="11em" MaxLength="255" 
                                Width="18em"  Required="false" 
                                ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtFax"  runat="server"  LabelText="Fax" LabelWidth="11em" MaxLength="255" 
                                Width="18em"  Required="false" 
                                ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                    </div>
                    <div style="float:left;width:45%;">
                        <cc1:DropDownListEx ID="cboDirectPaymentMethod" runat="server" LabelText="Direct Pay Method" LabelWidth="11em"
                            ValidationGroup="Save" Required="true" RequiredValidatorErrMsg="Please select Direct Payment Method"></cc1:DropDownListEx>
                        <br />
                        <cc1:TextBoxEx ID="txtCreditorReference"  runat="server"  LabelText="Creditor Reference" LabelWidth="11em" MaxLength="255" 
                            Width="18em"  Required="false" 
                            ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />	
                        <cc1:TextBoxEx ID="txtBacsSortCode"  runat="server"  LabelText="BACS Sort Code" LabelWidth="11em" MaxLength="255" 
                            Width="18em"  Required="false" 
                            ValidationGroup="Save"></cc1:TextBoxEx>
                        <asp:RegularExpressionValidator ID="regexpBacsSortCode" runat="server"     
                                                    ErrorMessage="Please enter sortcode in one of the following formats xx-xx-xx or xxxxxx" 
                                                    ControlToValidate="txtBacsSortCode"     
                                                    ValidationExpression="^(\d){2}-(\d){2}-(\d){2}|(\d){2}(\d){2}(\d){2}$" 
                                                    ValidationGroup="Save"/>
                        <br />	        
                        <cc1:TextBoxEx ID="txtBacsAccountNumber"  runat="server"  LabelText="BACS Acc Number" LabelWidth="11em" MaxLength="255" 
                            Width="18em"  Required="false" 
                            ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />	
                        <cc1:TextBoxEx ID="txtBacsReference"  runat="server"  LabelText="BACS Reference" LabelWidth="11em" MaxLength="255" 
                            Width="18em"  Required="false" 
                            ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />	
                        <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="10.65em"></cc1:CheckBoxEx>
                        <br />
                        <br />
                        <cc1:CheckBoxEx ID="chkIsGlobal" runat="server" Text="Global" LabelWidth="10.65em" />
                        <br />
                    </div>
                    <div class="clearer"></div>
                </fieldset>
                <br />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <%--Service Users Tab--%>
        <ajaxToolkit:TabPanel runat="server" ID="tabServiceUsers" HeaderText="Service Users" EnableViewState="false">
            <ContentTemplate>
                <table class="listTable" id="tblServiceUsers" cellspacing="0" cellpadding="2" width="100%">
				    <thead>
					    <tr>
						    <th class="header" style="width: 97.5%;">Service User</th>
						    <th class="header" style="width: 2.5%;">&nbsp;</th>
					    </tr>
				    </thead>
				    <tbody>
					    <asp:PlaceHolder id="phServiceUsers" runat="server" />
				    </tbody>
			    </table>
                <div style="float: right; clear: right;">
                    <asp:Button id="btnAdd" runat="server" Text="Add" ValidationGroup="Add" />
                </div>
                <br />
                <br />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <%--Documents Tab--%>
        <ajaxToolkit:TabPanel runat="server" ID="tabDocuments" HeaderText="Documents">
            <ContentTemplate>
                <iframe width="100%" height="100%" frameborder="0" id="ifrDocuments">
                  <p>Your browser does not support iframes.</p>
                </iframe>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <%--Notes Tab--%>
        <ajaxToolkit:TabPanel runat="server" ID="tabNotes" HeaderText="Notes">
            <ContentTemplate>
                <uc3:Notes id="Notes1" runat="server"></uc3:Notes>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    <input type="hidden" id="hidSelectedTab" runat="server" />
</asp:Content>
