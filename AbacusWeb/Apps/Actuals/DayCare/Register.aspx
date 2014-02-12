<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Register.aspx.vb" Inherits="Target.Abacus.Web.Apps.Actuals.DayCare.Register" %>

<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<asp:Content ID="cpOverview" ContentPlaceHolderID="MPPageOverview" runat="server">
    
    This screen allows you to maintain service registers.
    
</asp:Content>

<asp:Content ID="cpError" ContentPlaceHolderID="MPPageError" runat="server">

    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:Content>

<asp:Content ID="cpContent" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" />
    <ajaxToolkit:CollapsiblePanelExtender 
        ID="cpeReportsContainer" 
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
            
            <!--printable register blank -->
            <div id="divPrintableRegisterBlank" runat="server" class="availableReport">
                <input type="button" value="Show" id="rptBtnPrintableRegisterBlank" runat="server" title="Display a blank printable version of this service register?" />
            </div>
            
            <!-- printable register complete -->
            <div id="divPrintableRegisterComplete" runat="server" class="availableReport">
                <input type="button" value="Show" id="rptBtnPrintableRegisterComplete" runat="server" title="Display a complete printable version of this service register?"  />
            </div>
            
            <!-- register report -->
            <div id="divRegisterReport" runat="server" class="availableReport">
                <uc2:ReportsButton id="rptBtnRegisterReport" runat="server"></uc2:ReportsButton>
            </div>                
        </fieldset>
        <div class="clearer"></div>
        <br />
    </asp:Panel>
    <fieldset id="fsControls" runat="server" enableviewstate="false">
        <legend id="fsControlsLegend" runat="server" />                  
        <asp:Repeater ID="rptRegisterClientStatuses" runat="server" EnableViewState="false">
            <HeaderTemplate>
               <label id="lblFilterCriteria" class="errorText ServiceUserDialogTableFiltering"></label>  
               <table class="tablescroll" id="tblServiceUsers" cellspacing="0" cellpadding="2" summary="List of Service Users in this Register." width="99%">
                    <thead>
                        <tr>
                           <th filterTableType="TextBox" style="width: 25%">Service User Ref</th>
                           <th filterTableType="TextBox" style="width: 72%">Service User Name</th>
                           <th filterTableType="Custom" style="width: 3%">&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>                
                <tr id="tsu_tr_<%# DataBinder.Eval(Container.DataItem, "ClientID") %>" onclick="ShowServiceUser(<%# DataBinder.Eval(Container.DataItem, "ClientID") %>, '<%# DataBinder.Eval(Container.DataItem, "ClientName") %>', '<%# DataBinder.Eval(Container.DataItem, "ClientReference") %>');" title="View\Edit this Service User">
                    <td style="width: 25%"><%# DataBinder.Eval(Container.DataItem, "ClientReference") %></td>
                    <td style="width: 72%"><a><%# DataBinder.Eval(Container.DataItem, "ClientName") %></a></td>
                    <td style="width: 3%" class="<%# DataBinder.Eval(Container.DataItem, "RegisterClientStatus").ToString() %>">&nbsp;</td>                    
                </tr>                
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>   
        <br />
        <hr />
        <div id="divButtonsLeft" class="BottomLeftButtons">
            <input type="button" value="Clear Attendance" onclick="ShowClearAttendance();" runat="server" id="btnClearAttendance" title="Clear Attendance for this Service Register" />
        </div>  
        <div id="divButtonsRight" class="BottomRightButtons">
            <input type="button" value="Submit" runat="server" id="btnSubmit" />
            <input type="button" value="Add Service User" onclick="AddServiceUser();" runat="server" id="btnAddServiceUser" title="Add a Service User to this Service Register" />
        </div> 
    </fieldset>
</asp:Content>