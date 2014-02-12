<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RegisterReport.aspx.vb" Inherits="Target.Abacus.Web.Apps.Actuals.DayCare.RegisterReport" MasterPageFile="~/Popup.master" %>

<asp:Content ID="conMain" ContentPlaceHolderID="MPContent" runat="server">

    <h2>Service Register Printout</h2>
    <fieldset>
        <legend>Options</legend>
        <br />
        <div class="optionsLabels">
            <label>Provider:</label>
            <br />
            <label>Contract:</label>
            <br />
            <label>Day:</label>
            <br />
            <label>Week Ending:</label>
            <br />        
        </div>
        <div class="optionsValues">
            <label id="lblProvider" runat="server" />
            <br />
            <label id="lblContract" runat="server" />
            <br />
            <select id="selDays" runat="server" onchange="javascript:FilterReportByDay(this);" title="Filter Service Register by Day" />
            <div id="divPrint" class="print" title="Print Report" onclick="window.print();">&nbsp;</div>
            <div class="clearer"></div>
            <label id="lblWeekEnding" runat="server" />
            <br />        
        </div>    
    </fieldset>
    <br />
    <div id="divRegisterClientStatuses" runat="server">
        <div id="divAll">
            <asp:Repeater ID="rptRegisterClientStatuses" runat="server" EnableViewState="false">
                <HeaderTemplate>
                   <table id="tblServiceUsers" class="listTable" cellspacing="0" cellpadding="2" summary="List of Service Users in this Register." width="100%">
                        <thead>
                            <tr>
                                <th width="10%">Service User Ref</th>
                                <th width="20%">Service User Name</th>
                                <th width="20%">Rate Category</th>
                                <th width="5.7%" class="d mon">Mon</th>
                                <th width="5.7%" class="d tue">Tue</th>
                                <th width="5.7%" class="d wed">Wed</th>
                                <th width="5.7%" class="d thu">Thu</th>
                                <th width="5.7%" class="d fri">Fri</th>
                                <th width="5.7%" class="d sat">Sat</th>
                                <th width="5.7%" class="d sun">Sun</th>
                                <th width="5%">Total<br />Planned</th>
                                <th width="5%">Total<br />Actual</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Container.DataItem.ClientStatus.ClientReference %></td>
                        <td><%# Container.DataItem.ClientStatus.ClientName %></td>
                        <td><%# Container.DataItem.RateCategory.Description %></td>
                        <td <%# ConfigureDay("Mon", Container.DataItem) %>><%# ConfigureDayCell("Mon", Container.DataItem) %></td>
                        <td <%# ConfigureDay("Tue", Container.DataItem) %>><%# ConfigureDayCell("Tue", Container.DataItem) %></td>
                        <td <%# ConfigureDay("Wed", Container.DataItem) %>><%# ConfigureDayCell("Wed", Container.DataItem) %></td>
                        <td <%# ConfigureDay("Thu", Container.DataItem) %>><%# ConfigureDayCell("Thu", Container.DataItem) %></td>
                        <td <%# ConfigureDay("Fri", Container.DataItem) %>><%# ConfigureDayCell("Fri", Container.DataItem) %></td>
                        <td <%# ConfigureDay("Sat", Container.DataItem) %>><%# ConfigureDayCell("Sat", Container.DataItem) %></td>
                        <td <%# ConfigureDay("Sun", Container.DataItem) %>><%# ConfigureDayCell("Sun", Container.DataItem) %></td>
                        <td class="tp"><%#ConfigureTotalPlannedCell(Container.DataItem.ClientStatus.ClientID, Container.DataItem.RateCategory.ID)%></td>
                        <td <%# ConfigureTotalActual(Container.DataItem.ClientStatus.ClientID, Container.DataItem.RateCategory.ID) %>><%#ConfigureTotalActualCell(Container.DataItem.ClientStatus.ClientID, Container.DataItem.RateCategory.ID)%></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td colspan="3" class="totalPlanned">Total Planned: </td>
                                <td class="d mon"><%#ConfigureTotalPlannedCell("Mon")%></td>
                                <td class="d tue"><%#ConfigureTotalPlannedCell("Tue")%></td>
                                <td class="d wed"><%#ConfigureTotalPlannedCell("Wed")%></td>
                                <td class="d thu"><%#ConfigureTotalPlannedCell("Thu")%></td>
                                <td class="d fri"><%#ConfigureTotalPlannedCell("Fri")%></td>
                                <td class="d sat"><%#ConfigureTotalPlannedCell("Sat")%></td>
                                <td class="d sun"><%#ConfigureTotalPlannedCell("Sun")%></td>
                                <td class="tp"><%#ConfigureTotalPlannedCell("")%></td>
                                <td class="ta">N/A</td>
                            </tr>
                            <tr>
                                <td colspan="3" class="totalActual">Total Actual: </td>
                                <td class="d mon"><%#ConfigureTotalActualCell("Mon")%></td>
                                <td class="d tue"><%#ConfigureTotalActualCell("Tue")%></td>
                                <td class="d wed"><%#ConfigureTotalActualCell("Wed")%></td>
                                <td class="d thu"><%#ConfigureTotalActualCell("Thu")%></td>
                                <td class="d fri"><%#ConfigureTotalActualCell("Fri")%></td>
                                <td class="d sat"><%#ConfigureTotalActualCell("Sat")%></td>
                                <td class="d sun"><%#ConfigureTotalActualCell("Sun")%></td>
                                <td class="tp">N/A</td>
                                <td class="ta"><%#ConfigureTotalActualCell("")%></td>
                            </tr>
                        </tfoot>
                    </table>
                </FooterTemplate>
            </asp:Repeater> 
        </div>
        <div id="divDays">
            <div id="divDayMon" class="dayPageBreakAfter">
                <h3>Monday</h3>
            </div>  
            <div id="divDayTue" class="dayPageBreakAfter">
                <h3>Tuesday</h3>
            </div>  
            <div id="divDayWed" class="dayPageBreakAfter">
                <h3>Wednesday</h3>
            </div>
            <div id="divDayThu" class="dayPageBreakAfter">
                <h3>Thursday</h3>
            </div>
            <div id="divDayFri" class="dayPageBreakAfter">
                <h3>Friday</h3>
            </div>
            <div id="divDaySat" class="dayPageBreakAfter">
                <h3>Saturday</h3>
            </div>  
            <div id="divDaySun" class="dayPageBreakAfter">
                <h3>Sunday</h3>
            </div>  
        </div>
        <asp:Repeater ID="rptRegisterClientStatusesDaily" runat="server" EnableViewState="false">
            <HeaderTemplate>
               <table id="tblServiceUsersDaily" class="listTable" cellspacing="0" cellpadding="2" summary="List of Service Users in this Register." width="100%">
                    <thead>
                        <tr>
                            <th width="10%">Service User Ref</th>
                            <th width="20%">Service User Name</th>
            </HeaderTemplate>
            <ItemTemplate>
                <th><%#Container.DataItem.Description%></th>
            </ItemTemplate>
            <FooterTemplate>
                            <%#IIf(ReportMode = ReportModes.Complete, "<th width=""5%"">Total<br />Planned</th><th width=""5%"">Total<br />Actual</th>", "")%>                            
                        </tr>
                    </thead>
                    <tbody>
                    </tbody> 
                    <tfoot>
                    </tfoot>                  
                </table>
            </FooterTemplate>
        </asp:Repeater> 
    </div>
    <div id="divNoRegisterClientStatuses" runat="server">
        <strong style="color: Red;">No data is currently available for the selected Service Register.</strong>
    </div>
</asp:Content>
