<%@ Page Language="vb" AutoEventWireup="false" AspCompat="true" CodeBehind="Edit.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.SpendPlans.Edit" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceCareManager" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceCareManagerSelector.ascx" %>
<%@ Register TagPrefix="DS" TagName="DocumentSelector" Src="~/AbacusWeb/Apps/Documents/UserControls/DocumentSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceTeamSelector" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceTeamSelector.ascx" %>
<%@ Register TagPrefix="uc5" TagName="InPlaceClientGroupSelector" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientGroupSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="Notes" Src="~/AbacusWeb/Apps/UserControls/NotesSelector.ascx" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to view and edit the details of a Spend Plan.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" OnSaveClientClick="return spendPlan_CanSave();" ></uc1:StdButtons>
        
	<asp:Label ID="lblReconsideredCosts" runat="server" CssClass="warningText">
	    This Spend Plan has one or more costs – marked with a <asp:Image id="imgReconsiderWarningText" runat="server"></asp:Image> – 
	    that need to be reconsidered due to changes with associated Budget Categories.<br /><br />
	</asp:Label>

    <ajaxToolkit:TabContainer runat="server" ID="tabStrip" OnClientActiveTabChanged="tabStrip_ActiveTabChanged" EnableViewState="false">
        <%--Plan Tab--%>
        <ajaxToolkit:TabPanel runat="server" ID="tabPlan" HeaderText="Plan">
            <ContentTemplate>
                <%--Service User Selector--%>
                <asp:Label AssociatedControlID="client" runat="server" Text="Service User" Width="10.5em" style="float:left;" ></asp:Label>
				<uc2:InPlaceClient id="client" runat="server"></uc2:InPlaceClient>
				<br />
				<%--Spend Plan Reference--%>
				<cc1:TextBoxEx ID="txtReference" runat="server" LabelText="Reference" LabelWidth="10.5em"
					Required="true" RequiredValidatorErrMsg="Please enter a reference" MaxLength="25"
					ValidationGroup="Save"></cc1:TextBoxEx>
				<br />
				<%--Date From--%>
				<cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Date From" LabelWidth="10.5em"
					Required="true" RequiredValidatorErrMsg="Please enter a start date" Format="DateFormat"
					ValidationGroup="Save"></cc1:TextBoxEx>
				<br />
				
			    <%--Date To--%>
			    <div style="float:left;">
                    <cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="Date To" LabelWidth="10.5em" IsReadOnly="true"
				    Required="true" RequiredValidatorErrMsg="Please enter an end date" Format="DateFormat"
				    ValidationGroup="Save"></cc1:TextBoxEx>
			        <br /><br />
                </div>
                <div style="float:left;">
                    &nbsp;&nbsp;<asp:Label id="lblEndReason" AssociatedControlID="dteDateTo" runat="server" Text="" Width="30em" />	
                </div>
				<div style="clear:both;"></div>				
				<%--Status--%>
				<div id="divStatus" runat="server">
				    <label class="label" style="float:left" >Status</label>
				    <input id="optAwaitingApproval" runat="server" type="radio" name="status" value="1" style="float:left; margin-left:7em;"  />
                    <label class="label" style="float:left" for="optProvisional" >Awaiting Approval</label>
                    <input id="optApproved" runat="server" type="radio" name="status" value="2" style="float:left; margin-left:1em;"  />
                    <label class="label" style="float:left" for="optApproved" >Approved</label>
                    <input id="optCancelled" runat="server" type="radio" name="status" value="3" style="float:left; margin-left:1em;"  />
                    <label class="label" style="float:left" for="optCancelled" >Cancelled</label>
                    <br />
                    <br />
                    <%--Status Date--%>
                    <div class="clearer"></div>
                    <cc1:TextBoxEx ID="dteStatusDate" runat="server" LabelText="Status Date" LabelWidth="10.5em" IsReadOnly="true"></cc1:TextBoxEx>
				    <br />
				</div>
				<br />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <%--Financial Tab--%>
        <ajaxToolkit:TabPanel runat="server" ID="tabFinancial" HeaderText="Financial">
            <ContentTemplate>
            	<cc1:TextBoxEx id="txtFinanceCode1" runat="server" LabelText="Finance Code 1" LabelWidth="13.5em" Width="26em" MaxLength="20" Required="false" ValidationGroup="Save" />
                <br />
                <cc1:TextBoxEx id="txtFinanceCode2" runat="server" LabelText="Finance Code 2" LabelWidth="13.5em" Width="26em" MaxLength="20" Required="false" ValidationGroup="Save" />
                <br />
                <%--Care Manager--%>
				<asp:Label AssociatedControlID="careManager" runat="server" Text="Care Manager" Width="13.5em" style="float:left;" ></asp:Label>
				<uc3:InPlaceCareManager id="careManager" runat="server"></uc3:InPlaceCareManager>
				<br />
				<%--Team --%>
				<asp:Label AssociatedControlID="team" runat="server" Text="Team" Width="13.5em" style="float:left;" />
				<uc4:InPlaceTeamSelector id="team" runat="server" />
				<br />
				<%--Client Group --%>
				<asp:Label AssociatedControlID="clientGroup" runat="server" Text="Client Group" Width="13.5em" style="float:left;" />
				<uc5:InPlaceClientGroupSelector id="clientGroup" runat="server" />
				<br />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <%--Details Tab--%>
        <ajaxToolkit:TabPanel runat="server" ID="tabDetails" HeaderText="Details">
            <ContentTemplate>
                <%--Table of Planned Spend--%>
                <table class="listTable" id="tblDetails" cellspacing="0" cellpadding="2" summary="Spend plan detail" width="100%">
				<caption>Spend plan detail</caption>
				<thead>
					<tr>
						<th style="vertical-align:bottom;">Budget Category</th>
						<th style="vertical-align:bottom;">Service Delivered Via</th>
						<th style="vertical-align:bottom;">Units</th>
						<th style="vertical-align:bottom;">Measured In</th>
						<th style="vertical-align:bottom;">Frequency</th>
						<th style="vertical-align:bottom;">Annual Units</th>
						<th style="vertical-align:bottom;">Unit Cost</th>
						<th style="vertical-align:bottom;">Gross Annual Cost</th>
						<th>&nbsp;</th>
					</tr>
				</thead>
				<tbody>
					<asp:PlaceHolder ID="phDetails" runat="server"></asp:PlaceHolder>
				</tbody>
				</table>
				<asp:Button id="btnAddDetail" runat="server" Text="Add" ValidationGroup="AddDetail" />
                <br /><br />
                <%--Summarise the plan--%>
                <asp:Label ID="lblDetailsSummary" runat="server" CssClass="warningText">
                    Total planned cost as at <asp:Label ID="lblDetailsSummaryDateFrom" runat="server" CssClass="warningText"></asp:Label>
                    = <asp:Label ID="lblDetailsSummaryTotalCost" runat="server" CssClass="warningText"></asp:Label>
                    <br />
                    <asp:Label ID="lblDetailSummaryIndicativeBudget" runat="server" CssClass="warningText" />
                </asp:Label>
            
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <%--Costs Tab--%>
        <ajaxToolkit:TabPanel runat="server" ID="tabCosts" HeaderText="Costs">
            <ContentTemplate>
                <table class="listTable" cellspacing="0" cellpadding="2" summary="Spend plan costs" width="100%">
				<caption>List of spend plan costs</caption>
				<thead>
					<tr>
					    <th></th>
						<th>Date From</th>
						<th>Gross Cost</th>
						<th>Chargeable</th>
						<th>Additional</th>
						<th>Subsidised</th>
					</tr>
				</thead>
				<tbody>
					<asp:Repeater id="rptCosts" runat="server">
					    <ItemTemplate>
					        <tr>
					            <td><asp:Image id="imgCostReconsider" runat="server"></asp:Image></td>
					            <td><%#FormatCostsDateFrom(DataBinder.Eval(Container.DataItem, "DateFrom"))%></td>
					            <td><%#FormatCostsCurrency(DataBinder.Eval(Container.DataItem,"GrossAnnualCost"))%></td>
					            <td><%#FormatCostsCurrency(DataBinder.Eval(Container.DataItem, "PotentiallyChargeableCost"))%></td>
					            <td><%#FormatCostsCurrency(DataBinder.Eval(Container.DataItem, "AdditionalCost"))%></td>
					            <td><%#FormatCostsCurrency(DataBinder.Eval(Container.DataItem, "SubsidisedCost"))%></td>
					        </tr>
					    </ItemTemplate>
					</asp:Repeater>
				</tbody>
				</table>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabDocuments" HeaderText="Documents" EnableViewState="true">
            <ContentTemplate>
                <DS:DocumentSelector id="docSelector" runat="server"  EnableViewState="true"></DS:DocumentSelector>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabNotes" HeaderText="Notes">
            <ContentTemplate>
                <div id="notesdiv" runat="server"><uc3:Notes id="Notes1" runat="server"></uc3:Notes></div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    <br />
    <input type="button" id="btnTerminate" style="float:right; width:6.5em;" runat="server" value="Terminate" onclick="btnTerminate_Click();" />
    <%--<asp:Button id="btnTerminate" style="float:right; width:5.5em;" runat="server" text="Terminate" onclick="btnTerminate_Click();" />--%>
    <asp:Button id="btnReconsiderCosts" style="float:right; width:10em;" runat="server" text="Reconsider Costs" />
    <br />
    <input type="hidden" id="hidSelectedTabIndex" runat="server" />
    <input type="hidden" id="hidSelectedTab" runat="server" /> 
    <input type="hidden" id="hidSpendPlanDateFrom" runat="server" /> 
    <input type="hidden" id="hidSpendPlanDateTo" runat="server" />
    <input type="hidden" id="hidDefaultBudgetPeriod" runat="server" />
</asp:Content>
