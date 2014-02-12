<%@ Page Language="vb" AutoEventWireup="false" Codebehind="UserEdit.aspx.vb" Inherits="Target.Web.Apps.Security.Admin.UserEdit" EnableViewState="True" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to view and/or edit existing users.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPPageError" runat="server">
		<asp:Literal id="litPageError" runat="server"></asp:Literal>
	</asp:Content>
	
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
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
                
                <!-- permissions -->
                <div id="divPermissions" runat="server" class="availableReport">
                    <uc2:ReportsButton id="ctlPermissions" runat="server"></uc2:ReportsButton>
                </div>
                    
            </fieldset>
            <div class="clearer"></div>
            <br />
        </asp:Panel>
        
	    <ajaxToolkit:TabContainer runat="server" ID="TabStrip">
                <ajaxToolkit:TabPanel runat="server" ID="tabDetails" HeaderText="Details">
                    <ContentTemplate>
                    
                        <cc1:TextBoxEx ID="txtFirstName" runat="server" LabelText="First Name" LabelWidth="10em" MaxLength="50" Width="20em"
                            Required="true" RequiredValidatorErrMsg="Please enter a First Name" ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtSurname" runat="server" LabelText="Surname" LabelWidth="10em" MaxLength="50" Width="20em"
                            Required="true" RequiredValidatorErrMsg="Please enter a Surname" ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtEmail" runat="server" LabelText="Email/Username" LabelWidth="10em" MaxLength="50" Width="20em"
                            Required="true" RequiredValidatorErrMsg="Please enter an Email/Username" ValidationGroup="Save"></cc1:TextBoxEx>
                        <br />
                        <cc1:TextBoxEx ID="txtStatus" runat="server" LabelText="Status" LabelWidth="10em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
                        <br /><br />
                        
                        <fieldset id="pnlChangeStatus" runat="server" style="width:50%;">
                            <legend>Change Status</legend>
                            <div>
                                <asp:button id="btnChangePassword" runat="server" commandargument="ChangePassword" oncommand="ChangeStatus_Command" text="Change Password" width="10em" />
                                <asp:button id="btnActive" runat="server" commandargument="Active" oncommand="ChangeStatus_Command" text="Active" width="4em" />
                                <asp:button id="btnSuspended" runat="server" commandargument="Suspended" oncommand="ChangeStatus_Command" text="Suspended" width="7em" />
                                <asp:button id="btnLocked" runat="server" commandargument="Locked" oncommand="ChangeStatus_Command" text="Locked" width="5em" />
                                <br /><br />
                                <asp:button id="btnReactivate" runat="server" text="Reactivate" width="10em" />
                            </div>
                        </fieldset>                        
                        
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="tabRoles" HeaderText="Roles">
                    <ContentTemplate>
                        
                        <cc1:DualList ID="dlRoles" runat="server" 
                            SrcListCaption="Available" SrcListWidth="20em"
                            DestListCaption="Granted" DestListWidth="20em">
                        </cc1:DualList>
                        <div class="clearer"></div>
                    
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="tabInfo" HeaderText="Information">
                    <ContentTemplate>
                    
                        <cc1:TextBoxEx ID="txtCreateDate" runat="server" LabelText="Create Date" LabelWidth="14em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
                        <br /><br />
                        <cc1:TextBoxEx ID="txtLastLoginDate" runat="server" LabelText="Last Login Date" LabelWidth="14em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
                        <br /><br />
                        <cc1:TextBoxEx ID="txtLastLoginUserAgent" runat="server" LabelText="Last Login User Agent" LabelWidth="14em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
                        <br /><br />
                        <cc1:TextBoxEx ID="txtLastLoginIPAddress" runat="server" LabelText="Last Login IP Address" LabelWidth="14em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
                        <br /><br />
                        <cc1:TextBoxEx ID="txtPasswordDate" runat="server" LabelText="Password Date" LabelWidth="14em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
                        <br /><br />
                        <cc1:TextBoxEx ID="txtLastFailedLoginDate" runat="server" LabelText="Last Failed Login Date" LabelWidth="14em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
                        <br /><br />
                        <cc1:TextBoxEx ID="txtRejectedLogins" runat="server" LabelText="Rejected Logins" LabelWidth="14em" IsReadOnly="true" ReadOnlyContentCssClass="disabled"></cc1:TextBoxEx>
                        <br /><br />
                        
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="tabAdministrativeArea" HeaderText="Administrative Areas">
                    <ContentTemplate>
                        
                        <cc1:DualList ID="dlAdministrativeAreas" runat="server" 
                            SrcListCaption="Available" SrcListWidth="20em"
                            DestListCaption="Granted" DestListWidth="20em">
                        </cc1:DualList>
                        <div class="clearer"></div>
                    
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
	    </ajaxToolkit:TabContainer>

    </asp:Content>