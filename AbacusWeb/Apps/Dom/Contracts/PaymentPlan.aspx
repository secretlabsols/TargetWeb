<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PaymentPlan.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.Contracts.PaymentPlan" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceExp" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClientGroup" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientGroupSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceClientSubGroup" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSubGroupSelector.ascx" %>
<%@ Register TagPrefix="uc5" TagName="InPlaceTeam" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceTeamSelector.ascx" %>
<%@ Register TagPrefix="uc6" TagName="InPlaceFinanceCode" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
	<asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
<div  id="ButtonsDiv" style="width : 100%; clear : both; float : left;">  
<uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
</div>
			<ajaxToolkit:CollapsiblePanelExtender 
			ID="cpe" 
			runat="server"
			TargetControlID="pnlFinanceComponents"
			CollapsedSize="0"
			ExpandedSize="250"
			Collapsed="True"
			ExpandControlID="LinkButton1"
			CollapseControlID="LinkButton1"
			AutoCollapse="False"
			AutoExpand="False"
			ScrollContents="True"
			CollapsedText="Show Details..."
			ExpandedText="Hide Details" 
			ImageControlID="Image1"
			ExpandedImage="../../../../Images/ColPanel/collapse.jpg"
			CollapsedImage="../../../../Images/ColPanel/expand.jpg"
			ExpandDirection="Vertical"
			SuppressPostBack="true" />
			 <fieldset id="fsControls" runat="server" EnableViewState="false">
				 <fieldset id="fsHeader" runat="server" EnableViewstate="false">
				  <asp:Panel ID="pnlHeader" runat="server"> 
						<div id="HeaderDiv" style="width : 100%; clear : both; float : left;">
							<div style="width : 18.5em;float : left;">
								<asp:LinkButton ID="LinkButton1" runat="server"><asp:Image ID="Image1" runat="server" />Payment Plan Configuration</asp:LinkButton>
							</div> 
							<div class="clearer"></div>
							<br />
							<asp:Panel ID="pnlFirstPaymentDate" runat="server">
							<div style="width : 16.5em;float : left;">
								<cc1:TextBoxEx ID="dteFirstPaymentDate" runat="server" LabelText="1st Payment" LabelWidth="6.5em"  
									Required="true" RequiredValidatorErrMsg="Please enter first payment date" Format="DateFormatJquery"
									ValidationGroup="Save" AllowClear="true" Width="6.5em" />
							</div> 
<%--                            <div style="width : 15.5em;float : left;">
								<asp:radiobutton id="optFirstPayment" groupname="grpPaymentRemainder" TextAlign="left" height="2em" width="7.5em" 
									runat="server" text="First" ToolTip="Assign Payment remainder on First Payment" EnableViewState="true" Checked="true" />
								<asp:radiobutton id="optLastPayment" groupname="grpPaymentRemainder" TextAlign="left" height="2em" width="7.5em" 
									runat="server" text="Last" ToolTip="Assign Payment remainder on Last Payment" EnableViewState="true" />
							</div>--%>
							 <div class="clearer"></div>
							 <br />
							</asp:Panel>
							<div style="width : 16.5em;float : left;">
								<cc1:TextBoxEx ID="dteEndDate" runat="server" LabelText="End Date" LabelWidth="6.5em"  
									Required="true" RequiredValidatorErrMsg="Please enter a start date" Format="DateFormatJquery"
									ValidationGroup="Save" AllowClear="true" Width="6.5em" />
							</div> 
							<div style="width : 15.5em;float : left;">
								<asp:Label ID="lblContractValue" AssociatedControlID="txtContractValue" runat="server" Text="Contract Value" Width="8em" style="float:left;" ></asp:Label>
								<cc1:TextBoxEx ID="txtContractValue" runat="server" MaxLength="12" 
								Width="6em" Format="CurrencyFormat" Required="true" RequiredValidatorErrMsg="Please specify an amount" ValidationGroup="Save" MinimumValue="0" MaximumValue="100000000"/> 
							</div> 
							<div style="padding-left : 0.5em; float : left;">         
								<asp:Label runat="server" id="lblTotal" Text="Total: " ></asp:Label>
								<asp:Label runat="server" id="lblRunningTotal"></asp:Label>
							</div>  
						</div>
				  </asp:Panel>
				  </fieldset>
			  <fieldset id="fsFactors" runat="server" EnableViewstate="false">
				  <asp:Panel ID="pnlFinanceComponents" runat="server"> 
					   <div ID="dvFinanceComponents" style="width : 100%; clear : both; float : left;">
							<asp:Panel ID="pnlExpenditureAccount" runat="server">
							<asp:Label ID="lblExpenditureAccount" AssociatedControlID="expenditureAccount" runat="server" Text="Expenditure Account" Width="11em"  style="float:left;" ></asp:Label>
							<uc2:InPlaceExp id="expenditureAccount" runat="server" required="false"></uc2:InPlaceExp>
							<br />
							</asp:Panel>  
							<div ID="Div1" style="width : 100%; clear : both; float : left;">
								<div style="width : 20.5em; float : left;">
									<asp:Panel ID="pnlRateCategory" runat="server">
									<asp:Label ID="lblRateCategory" AssociatedControlID="cboRateCategory" runat="server" Text="Rate Category" Width="11em" style="float:left;" controltoValidate="cboRateCategory" ></asp:Label>
									<br />
									<cc1:DropDownListEx ID="cboRateCategory" runat="server" Required="true" 
									RequiredValidatorErrMsg="Please select a unit of measure" ValidationGroup="Save"></cc1:DropDownListEx>
									<br />
									</asp:Panel>
								</div>
								<asp:Panel ID="pnlFrequency" runat="server">
									<div style="padding-left : 0.5em; width : 20.5em; float : left;"> 
										<asp:Label ID="lblFrequency" AssociatedControlID="cboFrequency" runat="server" Text="Frequency" Width="6.5em" style="float:left;" controltoValidate="cboFrequency"></asp:Label>
										<br />
										<cc1:DropDownListEx ID="cboFrequency" runat="server"></cc1:DropDownListEx>
										<br />                                    
									</div>
								</asp:Panel>
							</div>
							<asp:Panel ID="pnlClientGroup" runat="server">
							<asp:Label ID="lblClientGroup" AssociatedControlID="clientGroup" runat="server" Text="Client Group" Width="11em" style="float:left;" ></asp:Label>
							<uc3:InPlaceClientGroup id="clientGroup" runat="server" required="false"></uc3:InPlaceClientGroup>
							<br />
							</asp:Panel>
							<asp:Panel ID="pnlClientSubGroup" runat="server">
							<asp:Label ID="lblClientSubGroup" AssociatedControlID="clientSubGroup" runat="server" Text="Client Sub Group" Width="11em" style="float:left;" ></asp:Label>
							<uc4:InPlaceClientSubGroup id="clientSubGroup" runat="server" required="false"></uc4:InPlaceClientSubGroup>
							<br />
							</asp:Panel>
							<asp:Panel ID="pnlTeam" runat="server">
							<asp:Label ID="lblTeam" AssociatedControlID="team" runat="server" Text="Team" Width="11em" style="float:left;" ></asp:Label>
							<uc5:InPlaceTeam id="team" runat="server" required="false"></uc5:InPlaceTeam>
							<br />
							</asp:Panel>     
							<asp:Panel ID="pnlFinanceCode" runat="server">
							<asp:Label ID="lblFinanceCode" AssociatedControlID="financeCode" runat="server" Text="Finance Code" Width="11em" style="float:left;" ></asp:Label>
							<uc6:InPlaceFinanceCode id="financeCode" runat="server" required="false"></uc6:InPlaceFinanceCode>
							<br />
							</asp:Panel>              
					</asp:Panel>
			   </fieldset>
				<div id="extContent" style="width : 100%; clear : both; float : left;"></div>
			 </fieldset>
			 <input type="hidden"  id="hidRunningTotal" runat="server" />
</asp:Content>