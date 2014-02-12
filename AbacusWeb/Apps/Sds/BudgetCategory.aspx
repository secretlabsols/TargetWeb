<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="BudgetCategory.aspx.vb" Inherits="Target.Abacus.Web.Apps.Sds.BudgetCategory" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc9" TagName="InPlaceServiceType" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceServiceTypeSelector.ascx" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to create new and view/edit existing budget categories.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <ajaxToolkit:CollapsiblePanelExtender 
        ID="cpe" 
        runat="server"
        TargetControlID="pnlReports"
        ExpandDirection="Vertical"
        />
    <asp:Panel id="pnlReports" runat="server">
        <fieldset class="availableReports">
            <legend>Available Reports</legend>
            <asp:ListBox ID="lstReports" runat="server"></asp:ListBox>
        </fieldset>

        <fieldset id="fsSelectedReport" class="selectedReport">
            <legend>Selected Report</legend>
            <div id="divDefault">Please select a report from the list</div>
            
            <!-- budget categories -->
            <div id="divCategories" runat="server" class="availableReport">
                <uc2:ReportsButton id="ctlCategories" runat="server"></uc2:ReportsButton>
            </div>
            
            <!-- budget categories without service type -->
            <div id="divCategoriesWithoutServiceType" runat="server" class="availableReport">
                <uc2:ReportsButton id="ctlCategoriesWithoutServiceType" runat="server"></uc2:ReportsButton>
            </div>
            
            <!-- budget category rates -->
            <div id="divRates" runat="server" class="availableReport">
                <uc2:ReportsButton id="ctlRates" runat="server"></uc2:ReportsButton>
            </div>                
        </fieldset>
        <div class="clearer"></div>
        <br />
    </asp:Panel>
    <ajaxToolkit:TabContainer runat="server" ID="tabStrip" OnClientActiveTabChanged="tabStrip_ActiveTabChanged" EnableViewState="false">
        <ajaxToolkit:TabPanel runat="server" ID="tabDetails" HeaderText="Details">
            <ContentTemplate>               
                <cc1:TextBoxEx id="txtReference" runat="server" LabelText="Reference" LabelWidth="13.5em" 
                    Width="10em" MaxLength="20" Required="true" RequiredValidatorErrMsg="Please specify a reference number" ValidationGroup="Save"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx id="txtDescription" runat="server" LabelText="Description" LabelWidth="13.5em" 
                    Width="20em" MaxLength="50" Required="true" RequiredValidatorErrMsg="Please specify a description" ValidationGroup="Save"></cc1:TextBoxEx>
                <br />
                <div style="display:none">
                    <cc1:DropDownListEx ID="cboBudgetCategoryType" runat="server" LabelText="Budget Category Type" LabelWidth="13.5em"></cc1:DropDownListEx>
			        <br />
			    </div>
			    <div style="float:left">
                    <asp:Label id="lblServiceType" runat="server" Text="Service Type" Width="13.25em" />
                    <uc9:InPlaceServiceType id="txtServiceType" runat="server" />	                     	
	            </div>
	            <div style="float:left;">
	                &nbsp;&nbsp;<asp:Label id="lblServiceCategory" runat="server" Text="" />	
	            </div>
	            <div style="clear:both">                
			        <br />
                    <cc1:DropDownListEx ID="cboUnitOfMeasure" runat="server" LabelText="Measured In" LabelWidth="13.5em" Required="true" RequiredValidatorErrMsg="Please specify an unit of measure" ValidationGroup="Save"></cc1:DropDownListEx>
	                <input type="hidden" id="hidUnitOfMeasure" runat="server" />
			        <br />
			        <div style="float:left; width : 100%;">
			            <div style="float:left">
			                <cc1:DropDownListEx ID="cboBudgetCategoryGroup" runat="server" LabelText="Budget Category Group" LabelWidth="13.5em"></cc1:DropDownListEx>
			                <br />
			            </div>
			            <div style="float:left;">
	                    &nbsp;&nbsp;<asp:Label id="lblBudgetCategoryGroupInfo" AssociatedControlID="cboBudgetCategoryGroup" runat="server" Text="" />	
	                    </div>
	                </div>
	                <cc1:TextBoxEx id="txtFinanceCode1" runat="server" LabelText="Finance Code 1" LabelWidth="13.5em" Width="20em" MaxLength="20" Required="false" ValidationGroup="Save" />
                    <br />
	                <cc1:TextBoxEx id="txtFinanceCode2" runat="server" LabelText="Finance Code 2" LabelWidth="13.5em" Width="20em" MaxLength="20" Required="false" ValidationGroup="Save" />
                    <br />
                    <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="13.2em"></cc1:CheckBoxEx>
                    <br />
                </div>
            </ContentTemplate>
	    </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabRates" HeaderText="Rates">
            <ContentTemplate>
                <asp:Panel id="pnlRates" runat="server">
                    <table  border="0" class="listTable" id="tblRates" cellspacing="0" cellpadding="2" summary="List of invoice details" width="100%">
                        <colgroup id="grpDateFromRate"></colgroup>
                        <colgroup id="grpDateToRate"></colgroup>
                        <colgroup id="grpExpUnitRate"></colgroup>
                        <colgroup id="grpIncUnitRate"></colgroup>
                        <colgroup id="grpExcUnitOverride"></colgroup>
		                <caption>List of Rates</caption>
		                <thead>
		                    <tr>
		                        <td colspan="1" style="border-width:0px;">&nbsp;</td>
		                        <td colspan="1" style="border-width:0px;">&nbsp;</td>
		                        <td colspan="1" style="border-width:0px;" align="center" class="con">Expenditure</td>
		                        <td colspan="4" style="border-width:0px;" align="center" class="income">Income</td>
		                        <td class="headerGroup" style="border-width:0px;">&nbsp;</td>
		                    </tr>
			                <tr>
				                <th class="header" style="width : 14%">Date From</th>
				                <th class="header" style="width : 14%">Date To</th>
				                <th class="header" style="text-align:left; width : 10%; padding-left : 20px;">Unit Rate(£)</th>
                                <th class="header" style="text-align:left; width : 8%; padding-left : 20px;">Use Actual Cost</th>
				                <th class="header" style="text-align:left; width : 10%; padding-left : 20px;">Unit Rate(£)</th>
				                <th class="header" style="text-align:left; width : 14%;">Max. Charge(£)</th>
				                <th class="header" style="text-align:left; width : 14%;">Additional Cost</th>				                
				                <th class="header" style="width : 2.5%;">&nbsp;</th>
			                </tr>
		                </thead>
		                <tbody>
            			    <asp:PlaceHolder id="phRates" runat="server"></asp:PlaceHolder>
		                </tbody>
		            </table>
		            <asp:Button id="btnAddRate" runat="server" text="Add" ValidationGroup="Save" CausesValidation="True" />
		            <br />
		            <br />
		            <asp:Label id="lblRateNotFullWarning" runat="server" CssClass="warningText" />
                </asp:Panel>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
	</ajaxToolkit:TabContainer>
	<input type="hidden" id="hidSelectedTab" runat="server" /> 
    <br />
        
</asp:Content>