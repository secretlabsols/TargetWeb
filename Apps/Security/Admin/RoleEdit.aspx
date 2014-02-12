<%@ Page Language="vb" AutoEventWireup="false" Codebehind="RoleEdit.aspx.vb" Inherits="Target.Web.Apps.Security.Admin.RoleEdit" EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen allows you to view, create, edit and delete security roles.
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
                
                <!-- membership -->
                <div id="divMembership" runat="server" class="availableReport">
                    <uc2:ReportsButton id="ctlMembership" runat="server"></uc2:ReportsButton>
                </div>
                    
            </fieldset>
            <div class="clearer"></div>
            <br />
        </asp:Panel>
        <ajaxToolkit:TabContainer runat="server" ID="TabStrip">
            <ajaxToolkit:TabPanel runat="server" ID="tabDetails" HeaderText="Details">
                <ContentTemplate>
                <fieldset id="fsControls" style="padding:0.5em;" runat="server">
                    <cc1:TextBoxEx ID="txtName" runat="server" LabelText="Name" LabelWidth="7em" Required="true"
                        RequiredValidatorErrMsg="Please enter a Name" MaxLength="50" Width="20em" ValidationGroup="Save"></cc1:TextBoxEx>
        	        <br />
	                <cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description" LabelWidth="7em" Required="true"
                        RequiredValidatorErrMsg="Please enter a Description" MaxLength="255" Width="20em" ValidationGroup="Save"></cc1:TextBoxEx>
		            <br />
        		    		    
		            <asp:TreeView ID="roleTree" runat="server" ShowCheckBoxes="All"></asp:TreeView>
                </fieldset>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="tabServiceGroups" HeaderText="Service Groups">
                <ContentTemplate>
                
                <cc1:DualList ID="dlServiceGroups" runat="server" SrcListCaption="Available" SrcListWidth="20em" DestListCaption="Granted" DestListWidth="20em"></cc1:DualList>
                <div class="clearer"></div>
                
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="tabProviderTypes" HeaderText="Provider Types">
                <ContentTemplate>
                
                <cc1:DualList ID="dlProviderTypes" runat="server" SrcListCaption="Available" SrcListWidth="20em" DestListCaption="Granted" DestListWidth="20em"></cc1:DualList>
                <div class="clearer"></div>
                
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="tabReportCategories" HeaderText="Report Categories">
                <ContentTemplate>
                
                <cc1:DualList ID="dlReportCategories" runat="server" SrcListCaption="Available" SrcListWidth="20em" DestListCaption="Granted" DestListWidth="20em"></cc1:DualList>
                <div class="clearer"></div>
                
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="tabJobTypes" HeaderText="Jobs">
                <ContentTemplate>
                
                <cc1:DualList ID="dlJobTypes" runat="server" SrcListCaption="Available" SrcListWidth="20em" DestListCaption="Granted" DestListWidth="20em"></cc1:DualList>
                <div class="clearer"></div>
                
                </ContentTemplate>
            </ajaxToolkit:TabPanel>            
            <ajaxToolkit:TabPanel runat="server" ID="tabDocumentTypes" HeaderText="Document Types">
                <ContentTemplate>
                
                <cc1:DualList ID="dlDocumentTypes" runat="server" SrcListCaption="Available" SrcListWidth="20em" DestListCaption="Granted" DestListWidth="20em"></cc1:DualList>
                <div class="clearer"></div>
                
                </ContentTemplate>
            </ajaxToolkit:TabPanel>            
        </ajaxToolkit:TabContainer>
    </asp:Content>