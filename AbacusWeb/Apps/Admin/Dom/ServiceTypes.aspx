<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ServiceTypes.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.ServiceTypes" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceFinanceCode" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.ascx" %>
<%@ Register TagPrefix="uc9" TagName="InPlaceServiceGroup" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceServiceGroupSelector.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different service types.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <ajaxToolkit:TabContainer runat="server" ID="tabStrip" OnClientActiveTabChanged="tabStrip_ActiveTabChanged" EnableViewState="false">
     <ajaxToolkit:TabPanel runat="server" ID="tabDetails" HeaderText="Details">
            <ContentTemplate>
                <div style="float:left">
                    <asp:Label AssociatedControlID="txtServiceGroup" runat="server" Text="Service Group" Width="10.5em"></asp:Label>
	                <uc9:InPlaceServiceGroup id="txtServiceGroup" runat="server"></uc9:InPlaceServiceGroup>	            
	                <br /><asp:HiddenField ID="hidServiceGroup" runat="server"></asp:HiddenField>
	            </div>
	            <div style="float:left;">
	                &nbsp;&nbsp;<asp:Label id="lblServiceCategory" AssociatedControlID="txtServiceGroup" runat="server" Text="" />	
	            </div>
	            <div style="clear:both">
                    <cc1:TextBoxEx ID="txtDescription"  runat="server"  LabelText="Description" LabelWidth="11em" MaxLength="255" 
                        Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a description"
                        ValidationGroup="Save"></cc1:TextBoxEx>
                    <br />
                    <cc1:TextBoxEx ID="txtAbbreviation"  runat="server"  LabelText="Abbreviation" LabelWidth="11em" MaxLength="50" 
                        Width="20em" ValidationGroup="Save"></cc1:TextBoxEx>
                    <br />
                    <asp:Label AssociatedControlID="txtFinanceCode" runat="server" Text="Finance Code" Width="10.5em"></asp:Label>
	                <uc2:InPlaceFinanceCode id="txtFinanceCode" runat="server"></uc2:InPlaceFinanceCode>
	                <br />
                    <asp:Label AssociatedControlID="txtFinanceCode2" runat="server" Text="Finance Code 2" Width="10.5em"></asp:Label>
                    <uc2:InPlaceFinanceCode id="txtFinanceCode2" runat="server"></uc2:InPlaceFinanceCode>
                    <br />

                    <fieldset>
                        <legend>Case Management Import Options</legend>
                        <cc1:DropDownListEx ID="cboConvertToServiceType" runat="server" LabelText="Convert to Service Type" LabelWidth="13em" ValidationGroup="Save">
                        </cc1:DropDownListEx>
                        <br />
                        <cc1:CheckBoxEx ID="chkDoubleNoCarers" runat="server" Text="Double No. of Carers" LabelWidth="13em"></cc1:CheckBoxEx>
                        <br /><br />
                        <cc1:CheckBoxEx ID="chkHalveDuration" runat="server" Text="Halve Duration" LabelWidth="13em"></cc1:CheckBoxEx>

                    </fieldset>
                    <br />

                    <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="10.5em"></cc1:CheckBoxEx>
                    <br />
                </div>
            </ContentTemplate>
	    </ajaxToolkit:TabPanel>
	    <ajaxToolkit:TabPanel runat="server" ID="tabLinkedServices" HeaderText="Linked Services" Visible="false">
            <ContentTemplate>   
                <asp:Panel id="pnlLinkedServicesList" runat="server">
                    <table class="listTable" id="tblLinkedServices" cellspacing="0" cellpadding="2" summary="Services" width="100%">
	                    <caption>Linked Services</caption>
			            <thead>
		                    <tr>
		                        <td colspan="2" style="border-width:0px; text-align : center;" class="domService">Domiciliary Service</td>
		                        <td colspan="3" style="border-width:0px; text-align : center;"  class="budgetCategories">Budget Category</td>
		                        <td class="headerGroup" style="border-width:0px;">&nbsp;</td>
		                    </tr>
			                <tr>
				                <th class="header" style="text-align:left; width : 20%">Title</th>
				                <th class="header" style="text-align:left; width : 20%">Measured In</th>
				                <th class="header" style="width : 20%">Description</th>
				                <th class="header" style="width : 10%">Measured In</th>
				                <th class="header" style="width : 40%">Conversion Factor</th>
			                </tr>
		                </thead>
			            <tbody>
				            <asp:PlaceHolder ID="phLinkedServices" runat="server"></asp:PlaceHolder>
			            </tbody>
		            </table>
		            <div>
		                <asp:Button id="btnAddLinkedServices" runat="server" Text="Add"/>
		            </div>
                    <br />
                </asp:Panel>
        	</ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
	<input type="hidden" id="hidSelectedTab" runat="server" />
    <br />
</asp:Content>