<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Edit.aspx.vb" EnableEventValidation="false" Inherits="Target.Abacus.Web.Apps.Dom.SvcOrders.Edit" AspCompat="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceEstablishment" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceDomContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>
<%@ Register TagPrefix="uc5" TagName="InPlacePct" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlacePctSelector.ascx" %>
<%@ Register TagPrefix="uc6" TagName="InPlaceCareManager" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceCareManagerSelector.ascx" %>
<%@ Register TagPrefix="uc7" TagName="InPlaceTeam" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceTeamSelector.ascx" %>
<%@ Register TagPrefix="uc8" TagName="InPlaceClientGroup" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientGroupSelector.ascx" %>
<%@ Register TagPrefix="uc9" TagName="InPlaceFinanceCode" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.ascx" %>
<%@ Register TagPrefix="uc10" TagName="DSOBasics" Src="~/Library/UserControls/DSOBasicDetails.ascx" %>
<%@ Register TagPrefix="uc11" TagName="DSOFunding" Src="~/AbacusWeb/Apps/Dom/SvcOrders/UserControls/ServiceOrderFunding.ascx" %>
<%@ Register TagPrefix="uc12" TagName="InPlaceClientSubGroup" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSubGroupSelector.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>


<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to view and edit the details of a service order.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <div style="width:49%;">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
        <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <%--hidden fields--%>
    <input type="text" style="display:none;" id="hidAttendanceHasDetail" runat="server" />
    <input type="text" style="display:none;" id="hid_AttendanceEditClicked" runat="server" />
    <input type="text" style="display:none;" id="hid_AttendanceNewClicked" runat="server" />
    <input type="text" style="display:none;" id="hidCurrentEffectiveDate" runat="server" />
    <input type="text" style="display:none;" id="hidOriginalEffectiveDate" runat="server" />
    <%--End hidden fields--%>



    <script type="text/javascript">
        Sys.Application.add_load(BindEvents);
     </script>

    <div >
        <div style="float:left;"><uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons></div>
        <div id="divDSOAdditionalDetails" runat="server" style="float:right; margin-top: -15px; width:50%;">
                <uc10:DSOBasics id="DSOAdditionalDetails" runat="server"></uc10:DSOBasics>
        </div>
    </div>
        
    <div class="clearer"></div>
    <div id="divFilterDate" runat="server" >
        <cc1:TextBoxEx ID="txtVisitsFilterDate" runat="server" LabelText="Show service plan in force on: " LabelWidth="16.5em"  
		    Format="DateFormatJquery" ValidationGroup="Save"></cc1:TextBoxEx>
        <asp:TextBox runat="server" ID="txtDummyFilterDate" Value="" Style="display:none;" AutoPostBack="true"></asp:TextBox>
        <br />
    </div>
    <ajaxToolkit:TabContainer runat="server" ID="tabStrip" OnClientActiveTabChanged="tabStrip_ActiveTabChanged" EnableViewState="false">
        <ajaxToolkit:TabPanel runat="server" ID="tabHeader"  HeaderText="Order">
            <ContentTemplate>
				<asp:Label AssociatedControlID="provider" runat="server" Text="Provider" Width="10.5em" style="float:left;" ></asp:Label>
				<uc2:InPlaceEstablishment id="provider" runat="server" Mode="DomProviders"></uc2:InPlaceEstablishment>
				<br />
				<asp:Label AssociatedControlID="domContract" runat="server" Text="Contract" Width="10.5em" style="float:left;" ></asp:Label>
				<uc3:InPlaceDomContract id="domContract" runat="server"></uc3:InPlaceDomContract>
				<br />
				<asp:Label AssociatedControlID="client" runat="server" Text="Service User" Width="10.5em" style="float:left;" ></asp:Label>
				<uc4:InPlaceClient id="client" runat="server"></uc4:InPlaceClient>
				<br />
				<cc1:TextBoxEx ID="txtOrderRef" runat="server" LabelText="Order Reference" LabelWidth="10.5em" 
					Required="true" RequiredValidatorErrMsg="Please enter an order reference" MaxLength="25"
					ValidationGroup="Save"></cc1:TextBoxEx>
				<br />
				<div style="float:left;">
				    <cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Date From" LabelWidth="10.5em"  
					    Required="true" RequiredValidatorErrMsg="Please enter a start date" Format="DateFormatJquery"
					    ValidationGroup="Save" AllowClear="true" Width="6.5em" />
				</div>
				<div style="float:left;margin-left:20px;">
				    <cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="&nbsp;Date To" LabelWidth="5em"
					    Required="true" RequiredValidatorErrMsg="Please enter an end date" Format="DateFormatJquery"
					    ValidationGroup="Save" AllowClear="true" Width="6.5em" />
				</div>
				<div style="float:left;margin-left:20px;">
				    <cc1:DropDownListEx ID="cboEndReason" runat="server" LabelText="&nbsp;End Reason" LabelWidth="7em"
					    Required="true" RequiredValidatorErrMsg="Please select an end reason" ValidationGroup="Save" />
				</div>
				<div class="clearer"></div>
				<br />
				<div style="float:left;">
				    <cc1:DropDownListEx ID="cboProjectCode" runat="server" LabelText="Project Code" LabelWidth="10.5em"
					     ValidationGroup="Save"></cc1:DropDownListEx>
				</div>
				<div style="float:left;margin-left:20px;">
		            <cc1:CheckBoxEx ID="chkExcludeFromAnomalies" runat="server" LabelWidth="25em" Text="Exclude this order from Assessment Anomalies" />
				</div>
				<div class="clearer"></div>
				<br />
				<cc1:TextBoxEx ID="txtComment" runat="server" LabelText="Comment" LabelWidth="10.5em" Width="50%"></cc1:TextBoxEx>
				<br />
            </ContentTemplate>
		</ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="tabSuspensions" HeaderText="Order Suspension">
            <ContentTemplate>
				<table class="listTable" cellspacing="0" cellpadding="2" summary="List of order suspensions" width="100%">
				<caption>List of order suspensions</caption>
				<thead>
					<tr>
						<th>Date From</th>
						<th>Date To</th>
						<th>Reason for Suspension</th>
					</tr>
				</thead>
				<tbody>
					<asp:Repeater id="rptSuspensions" runat="server">
					    <ItemTemplate>
					        <tr>
					            <td><%#FormatSuspensionDate(Container.DataItem("DateFrom"))%></td>
					            <td><%#FormatSuspensionDate(Container.DataItem("DateTo"))%></td>
					            <td><%#Container.DataItem("Description")%></td>
					        </tr>
					    </ItemTemplate>
					</asp:Repeater>
				</tbody>
				</table>
            </ContentTemplate>
		</ajaxToolkit:TabPanel>
		<ajaxToolkit:TabPanel runat="server" ID="tabSummary" HeaderText="Summary" EnableViewState="true">
            <ContentTemplate>
                <asp:CheckBox id="chkDontFilterCommitmentSummary" Text="Show all service &nbsp;" TextAlign="left" AutoPostBack="True" runat="server" />
                <div  id="divDontFilterCommitmentSummary" runat="server">
                    <br />
                </div>
				<table class="listTable" cellspacing="0" cellpadding="2" summary="Summarised order detail" width="100%">
				<caption>Summarised order detail</caption>
				<thead>
					<tr>
                        <th style="vertical-align:bottom;">Line From</th>
                        <th style="vertical-align:bottom;">Line To</th>
						<th style="vertical-align:bottom;">Rate Category</th>
						<% If ShowDayOfWeekColumn() Then %>
						<th style="vertical-align:bottom;">Days</th>
						<% End If %>
						<th style="vertical-align:bottom;">Units</th>
						<th style="vertical-align:bottom;">Measured In</th>
						<% If ShowSvcUserMinutesColumn() Then %>
						<th style="vertical-align:bottom;">S/U<br />Minutes</th>
						<% End If %>
						<% If ShowVisitsColumn() Then %>
						<th style="vertical-align:bottom;">Visits</th>
						<% End If %>
						<th style="vertical-align:bottom;">Frequency</th>
						<th>&nbsp;</th>
					</tr>
				</thead>
				<tbody>
					<asp:PlaceHolder ID="phSummary" runat="server"></asp:PlaceHolder>
				</tbody>
				</table>
				<asp:Button id="btnAddSummary" runat="server" Text="Add" ValidationGroup="AddSummary" />				
            </ContentTemplate>
		</ajaxToolkit:TabPanel>
		<ajaxToolkit:TabPanel runat="server" ID="tabVisits" HeaderText="Visits">
            <ContentTemplate>
                <asp:CheckBox id="chkDontFilterCommitmentVisit" Text="Show all service &nbsp;" TextAlign="left" AutoPostBack="True" runat="server" />
                <div id="divDontFilterCommitmentVisit" runat="server">                   
                    <br />
                </div>
				<table class="listTable" cellspacing="0" cellpadding="2" summary="List of planned visits" width="100%">
				<caption>List of planned visits</caption>
				<thead>
					<tr>
                        <th style="vertical-align:bottom;">Line From</th>
                        <th style="vertical-align:bottom;">Line To</th>
						<th style="vertical-align:bottom;">Service Type</th>
						<th style="vertical-align:bottom;">Start Time</th>
						<th style="vertical-align:bottom;">Duration</th>
						<th style="vertical-align:bottom;">Carers</th>
						<th style="vertical-align:bottom;">Days</th>
						<th style="vertical-align:bottom;">Frequency</th>
						<th colspan="2" style="vertical-align:bottom;">Primary</th>
					</tr>
				</thead>
				<tbody>
					<asp:PlaceHolder ID="phVisits" runat="server"></asp:PlaceHolder>
				</tbody>
				</table>
				<asp:Button id="btnAddVisit" runat="server" Text="Add" ValidationGroup="AddVisit" />
            </ContentTemplate>
		</ajaxToolkit:TabPanel>
		<ajaxToolkit:TabPanel runat="server" ID="tabAttendanceSummary" HeaderText="Summary">
            <ContentTemplate>
                <div class="clearer"></div>
                <div>
                    <table class="listTable" id="tblAttendanceSummary" cellspacing="0" cellpadding="2" summary="Summarised order detail" width="100%">
                        <%--Adding this caption screws up the screen layout in firefox due to the margin style on the table--%>
                        <caption >Summarised order detail</caption> 
                        <thead>
                            <tr>
                                <th style="vertical-align:bottom;">Rate Category</th>
                                <% If ShowDayOfWeekColumn() Then %>
		                            <th style="vertical-align:bottom;">Days</th>
	                            <% End If %>
                                <th style="vertical-align:bottom;">Units</th>
                                <th style="vertical-align:bottom;">Measured In</th>
                                <th style="vertical-align:bottom;">Frequency</th>
                                <th style="vertical-align:bottom;" colspan="2">First<br />Week</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:PlaceHolder ID="phAttendanceSummary" runat="server"></asp:PlaceHolder>
                        </tbody>
                    </table>
                    <br />
                    <div style="float:left;" >
                        <div>
                            <asp:Button id="btnAddAttendanceSummary" runat="server" Text="Add" ValidationGroup="AddAttendanceSummary" />
                        </div>
                    </div> 
                </div>
                <div class="clearer"></div>
            </ContentTemplate>
		</ajaxToolkit:TabPanel>
		<ajaxToolkit:TabPanel runat="server" ID="tabAttendance" HeaderText="Attendance" >
            <ContentTemplate>
                <%--<input type="text" style="display:none;" id="hid_AttendanceEditClicked" runat="server" />
                <input type="text" style="display:none;" id="hid_AttendanceNewClicked" runat="server" />
                <input type="text" style="display:none;" id="hidCurrentEffectiveDate" runat="server" />
                <input type="text" style="display:none;" id="hidOriginalEffectiveDate" runat="server" />--%>
                <div>
                    <div id="divEffectiveFrom" runat="server" style="float:left; padding:0.25em 2em 0em 1em;">
                        <cc1:TextBoxEx ID="dteEffectiveFrom" runat="server" LabelText="Effective From" LabelWidth="110px"
			                        Required="true" RequiredValidatorErrMsg="Please enter an effective date" Format="DateFormat"
			                        ValidationGroup="Save"></cc1:TextBoxEx>
                    </div>
                    
                    <div style="float:left;">
                        <div>
                            <div style="padding-top:5px; width:130px; float:left;">
                                <asp:Label ID="lblSOPlan" runat="server" Text="Service Order Plan" />
                            </div>
                            <div id="divPlan" runat="server" style="padding:5px 0em 5px 2em; border:solid 1px silver; float:left;">
                                
                            </div>
                            <input type="button" id="btnAttendanceViewPlan" runat="server" style="margin:2px 0px 0px 5px;" value="View Plan" onclick="javascript:btnAttendanceViewPlan_click();" ValidationGroup="SaveAttendance"  />
                        </div>
                        <div class="clearer"></div>
                        <div id="divRevised" runat="server">
                            <div style="margin-top:1em; width:130px; float:left;">
                                <asp:Label ID="lblRevisedPlan" runat="server" Text="Revised Plan" />
                            </div>
                            <div id="divRevisedPlan" style="padding:5px 0em 5px 2em; margin-top:0.5em; border:solid 1px silver; float:left;">
                                
                            </div>
                        </div>
                    </div>                    
                </div>
                <div class="clearer"></div>
                <div>
                    <table class="listTable" id="tblAttendance" cellspacing="0" cellpadding="2" summary="List of planned Attendance" width="100%">
                        <%--Adding this caption screws up the screen layout in firefox due to the margin style on the table--%>
                        <caption >List of planned attendance</caption> 
                        <thead>
                            <tr>
                                <th style="vertical-align:bottom;">Rate Category</th>
		                        <th style="vertical-align:bottom;">Days</th>
                                <th style="vertical-align:bottom;">Units</th>
                                <th style="vertical-align:bottom;">Measured In</th>
                                <th style="vertical-align:bottom;">Frequency</th>
                                <th style="vertical-align:bottom;" colspan="2">First<br />Week</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:PlaceHolder ID="phAttendance" runat="server"></asp:PlaceHolder>
                        </tbody>
                    </table>
                    <br />
                    <div style="float:left;" >
                        <div>
                            <asp:Button id="btnAddAttendance" runat="server" Text="Add" ValidationGroup="AddAttendance" />
                        </div>
                    </div>
                    <div style="float:right;" >
                        <fieldset id="fldButtons" runat="server" style=" padding:3px 3px 3px 3px">
                            <%--<input type="button" id="btnAttendanceDates" runat="server" value="Dates" onclick="btnDates_Click();" />--%>
                            <input type="button" style="width:4.5em;" id="btnAttendanceDates" runat="server" value="Dates" title="Move the effective date of the current revision." onclick="btnDates_Click();" />
                            <asp:Button id="btnAttendanceNew" style="width:4.5em;" runat="server" Text="New" ValidationGroup="NewAttendance" />
                            <asp:Button id="btnAttendanceSave" runat="server" Text="Save" ValidationGroup="SaveAttendance"  />
                            <asp:Button id="btnAttendanceEdit" style="width:4.5em;" runat="server" Text="Edit" ValidationGroup="EditAttendance" />
                            <%--<asp:Button id="btnAttendanceDelete" runat="server" Text="Delete" ValidationGroup="DeleteAttendance" OnClientClick="if(!window.confirm('Are you sure you wish to delete this revision?')) return false;" />--%>
                            <input type="button" id="btnAttendanceDelete" runat="server" value="Delete" onclick="btnDeleteAttendance_Click();" />
                            <asp:Button id="btnAttendanceCancel" runat="server" Text="Cancel" ValidationGroup="CancelAttendance"  />
                            
                        </fieldset>
                        
                    </div>
                        
                    <div id="divAttendanceNavigation" runat="server" style="width: 250px;  display: block;  margin-left: auto;  margin-right: auto;">
                            <asp:ImageButton  id="btnPreviousSchedule" ImageUrl="../../../../Images/ArrowBack.png" 
                                    style="float:left; cursor:pointer;" 
                                    AlternateText="Display the previous attendance schedule" runat="server" />
                            <div style="float:left; padding:0em 1em 0em 1em;">
                                Record&nbsp;
                                <asp:Label ID="lblCurrentRecordNo" 
                                        runat="server" Text="x" CssClass="warningText" 
                                        style="padding-right:10px;" />of<asp:Label ID="lblTotalNoRecords" 
                                                                                    runat="server" Text="x" CssClass="warningText" 
                                                                                    style="padding-left:10px;" />
                            </div>
                            <asp:ImageButton id="btnNextSchedule" ImageUrl="../../../../Images/ArrowForward.png" 
                                    style="float:left; cursor:pointer;" 
                                    AlternateText="Display the next attendance schedule" runat="server" />
                    </div>
                </div>
                <div class="clearer"></div>
            </ContentTemplate>
		</ajaxToolkit:TabPanel>
		
		<ajaxToolkit:TabPanel runat="server" ID="tabFunding" HeaderText="Funding" EnableViewState="true">
            <ContentTemplate>

                <a id="fundingButton" runat="server" class="trig" style="float:left;height:100%;">
                    <img id="fundingImg" src="../../../../Images/ChangeFundingDetailbutton.png" >
                </a>
                             
                
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<div id="fundingPanel" runat="server" class="fundingpanel" style="float:left;padding-left:1em;">
                    
                                    
                    <asp:Label id="lblFinanceCode" AssociatedControlID="txtFinanceCode" runat="server" Text="Finance Code" Width="10.5em"></asp:Label>
				    <uc9:InPlaceFinanceCode id="txtFinanceCode" runat="server"></uc9:InPlaceFinanceCode>
			        <br />
			        <asp:Label id="lblClientGroup" AssociatedControlID="clientGroup" runat="server" Text="Client Group" Width="10.5em"></asp:Label>
			        <uc8:InPlaceClientGroup id="clientGroup" runat="server"></uc8:InPlaceClientGroup>
			        <br />
			        <asp:Label id="lblClientSubGroup" AssociatedControlID="clientSubGroup" runat="server" Text="Client Sub Group" Width="10.5em"></asp:Label>
			        <uc12:InPlaceClientSubGroup id="clientSubGroup" runat="server"></uc12:InPlaceClientSubGroup>
			        <br />
			        <asp:Label id="lblTeam" AssociatedControlID="team" runat="server" Text="Team" Width="10.5em"></asp:Label>
			        <uc7:InPlaceTeam id="team" runat="server"></uc7:InPlaceTeam>
			        <br />
			        <asp:Label id="lblCareManager" AssociatedControlID="careManager" runat="server" Text="Care Manager" Width="10.5em"></asp:Label>
			        <uc6:InPlaceCareManager id="careManager" runat="server"></uc6:InPlaceCareManager>
			        <br />
			    </div>
			        <div class="panel" id="svcOrderFundingPanel" runat="server" style="float:left;padding-left:1em; width:95%;">

                        <uc11:DSOFunding id="dsoFunding" runat="server" ></uc11:DSOFunding>
                 </div>
			    <div class="clearer"></div>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
		<ajaxToolkit:TabPanel runat="server" ID="tabExpenditure" HeaderText="Expenditure">
            <ContentTemplate>
				<table class="listTable" id="tblExpenditure" cellspacing="0" cellpadding="2" summary="List of expenditure by rate category" width="100%">
				<caption>List of expenditure by rate category</caption>
				<thead>
					<tr>
						<th style="vertical-align:bottom;" align="left">Rate Category</th>
						<th style="vertical-align:bottom;" align="left">Units</th>
						<th style="vertical-align:bottom;" align="left">Measured In</th>
						<th style="vertical-align:bottom;" align="left">Unit Cost</th>
						<th style="vertical-align:bottom;" align="left">Overridden</th>
						<th style="vertical-align:bottom;text-align:right;width:8em;">Total Cost</th>
						<th style="vertical-align:bottom;text-align:right;">Expenditure<br />Code</th>
						<th>&nbsp;</th>
					</tr>
				</thead>
				<tbody>
					<asp:PlaceHolder ID="phExpenditure" runat="server"></asp:PlaceHolder>
				</tbody>
				<tfoot>
				    <tr>
				        <td colspan="5" style="text-align:right;font-weight:bold;">Overall Cost</td>
				        <td style="text-align:right;font-weight:bold;">
				            <asp:Literal id="litExpOverallCost" runat="server"></asp:Literal>
				        </td>
				        <td colspan="2">&nbsp;</td>
				    </tr>
				</tfoot>
				</table>
                <div style="float:right;" >
                    <input type="button" id="btnOverrideUnitCosts" runat="server" value="Override Unit Costs" onclick="btnOverrideUnitCosts_Click();" />
                </div>

                <br />
				<em><asp:Literal id="litExpStatement" runat="server"></asp:Literal></em>
            </ContentTemplate>
		</ajaxToolkit:TabPanel>
		<ajaxToolkit:TabPanel runat="server" ID="tabPaymentTolerances" HeaderText="Payment Tolerances" >
            <ContentTemplate> 
                <iframe width="100%" height="100%" frameborder="0" id="ifrPaymentTolerances">
                  <p>Your browser does not support iframes.</p>
                </iframe>
            </ContentTemplate>
		</ajaxToolkit:TabPanel>
		<ajaxToolkit:TabPanel runat="server" ID="tabDocuments" HeaderText="Documents" EnableViewState="true">
            <ContentTemplate> 
                <iframe width="100%" height="100%" frameborder="0" id="ifrDocuments">
                  <p>Your browser does not support iframes.</p>
                </iframe>
            </ContentTemplate>
		</ajaxToolkit:TabPanel>
	</ajaxToolkit:TabContainer>
	<input type="hidden" id="hidSelectedTab" runat="server" />
    <br />
    
    <div id="divDatesDialogContentContainer" style="display:none;">
    <div id="divDatesDialogContent">
        <!-- hidden elements used in copy dialog -->
        Please enter a new effective date for this attendance schedule.
        <br /><br />
        <cc1:TextBoxEx ID="dteDatesEffectiveDate" runat="server" LabelText="Effective Date" LabelWidth="10em" Format="DateFormat"
            Required="true" RequiredValidatorErrMsg="Please enter a valid effective date" ValidationGroup="Dates"></cc1:TextBoxEx>
        <br />
    </div>
</div>
</asp:Content>