<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="PlannedAttendance.aspx.vb" Inherits="Target.Abacus.Web.Apps.SvcOrders.PlannedAttendance" MasterPageFile="~/Popup.master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
        
        
        <table class="listTable" id="tblPlannedAttendance" cellspacing="0" cellpadding="2" summary="List of planned Attendance" width="100%">
            <%--Adding this caption screws up the screen layout in firefox due to the margin style on the table--%>
            <caption >List of planned attendance</caption> 
            <thead>
                <tr>
                    <th style="vertical-align:bottom;">Rate Category</th>
                    <% If ShowDayOfWeekColumn() Then %>
                        <th style="vertical-align:bottom;">On<br />Mon</th>
                        <th style="vertical-align:bottom;">On<br />Tue</th>
                        <th style="vertical-align:bottom;">On<br />Wed</th>
                        <th style="vertical-align:bottom;">On<br />Thu</th>
                        <th style="vertical-align:bottom;">On<br />Fri</th>
                        <th style="vertical-align:bottom;">On<br />Sat</th>
                        <th style="vertical-align:bottom;">On<br />Sun</th>
                    <% End If %>
                    <th style="vertical-align:bottom;">Units</th>
                    <th style="vertical-align:bottom;">Measured In</th>
                    <th style="vertical-align:bottom;">Frequency</th>
                    <th style="vertical-align:bottom;" colspan="2">First<br />Week</th>
                </tr>
            </thead>
            <tbody>
				<asp:Repeater id="rptPlannedAttendance" runat="server">
				    <ItemTemplate>
				        <tr>
				            <td><%#Container.DataItem("RateCategory")%></td>
				            <% If ShowDayOfWeekColumn() Then %>
				                <td><asp:Checkbox runat="server" Enabled="false" checked='<%# Container.DataItem("OnMonday") %>' /></td>
				                <td><asp:Checkbox runat="server" Enabled="false" checked='<%# Container.DataItem("OnTuesday") %>' /></td>
				                <td><asp:Checkbox runat="server" Enabled="false" checked='<%# Container.DataItem("OnWednesday") %>' /></td>
				                <td><asp:Checkbox runat="server" Enabled="false" checked='<%# Container.DataItem("OnThursday") %>' /></td>
				                <td><asp:Checkbox runat="server" Enabled="false" checked='<%# Container.DataItem("OnFriday") %>' /></td>
				                <td><asp:Checkbox runat="server" Enabled="false" checked='<%# Container.DataItem("OnSaturday") %>' /></td>
				                <td><asp:Checkbox runat="server" Enabled="false" checked='<%# Container.DataItem("OnSunday") %>' /></td>
				            <% End If %>
				            <td><%#Container.DataItem("Units")%></td>
				            <td><%#Container.DataItem("UOM")%></td>
				            <td><%#Container.DataItem("Frequency")%></td>
				            <td><%#Container.DataItem("FirstWeekOfService")%></td>
				        </tr>
				    </ItemTemplate>
				</asp:Repeater>
			</tbody>
        </table>
        
        
	    <input type="button" id="btnCancel" value="Cancel" style="float:right;margin-right:1em;" onclick="btnCancel_Click();" />

    </asp:Content>
