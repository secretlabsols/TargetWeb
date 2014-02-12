<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ManualEnterVisitGrid.ascx.vb"
    Inherits="Target.Abacus.Extranet.ManualEnterVisitGrid" %>
    
<p style="text-align: right;">
    <asp:GridView ID="gvVisits" Width="100%" runat="Server" AutoGenerateColumns="False" CssClass="listTable"
        ShowFooter="false" EnableViewState="True" DataKeyNames="ObjectIndex">
        <EmptyDataRowStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" />
        <Columns>
            <%--<asp:BoundField DataField="ObjectIndex" HeaderText="" ReadOnly="True" />--%>
            <asp:TemplateField HeaderText="Service Type" ItemStyle-BackColor="#ffddac">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlServiceType" runat="server" Width="10.5em">
                    </asp:DropDownList>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlServiceType" runat="server" Width="10.5em">
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="ddlServiceType" runat="server" Width="10.5em">
                    </asp:DropDownList>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Start Time" ItemStyle-BackColor="#ffddac">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlStartTimeHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlStartTimeMinutes" runat="server">
                    </asp:DropDownList>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlStartTimeHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlStartTimeMinutes" runat="server">
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="ddlStartTimeHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlStartTimeMinutes" runat="server">
                    </asp:DropDownList>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="End Time" ItemStyle-BackColor="#ffddac">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlEndTimeHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlEndTimeMinutes" runat="server">
                    </asp:DropDownList>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlEndTimeHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlEndTimeMinutes" runat="server">
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="ddlEndTimeHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlEndTimeMinutes" runat="server">
                    </asp:DropDownList>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Duration <br/>Claimed" ItemStyle-BackColor="#fffea6">
                <ItemTemplate >
                    <div style="float:left;">
                        <asp:DropDownList ID="ddlDurationClaimedHours" runat="server">
                        </asp:DropDownList>:<asp:DropDownList ID="ddlDurationClaimedMinutes" runat="server">
                        </asp:DropDownList>
                    </div>
                    <div style="float:left;">
                        <asp:Label ID="lblDCR" Font-Size="Smaller" CssClass="warningText transbg"  runat="Server" 
                        Text='<%# PreRoundedDurationClaimedText(Eval("PreRoundedDurationClaimed")) %>'></asp:Label>
                    </div>
                    <div style="float:left;margin-top:3px;">
                        <asp:Panel ID="ignoreRounding"  runat="server"></asp:Panel>
                    </div>
                    <div class="clearer"></div>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlDurationClaimedHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlDurationClaimedMinutes" runat="server">
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="ddlDurationClaimedHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlDurationClaimedMinutes" runat="server">
                    </asp:DropDownList>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actual <br/> Duration"  ItemStyle-BackColor="#fffea6">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlActualDurationHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlActualDurationMinutes" runat="server">
                    </asp:DropDownList>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlActualDurationHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlActualDurationMinutes" runat="server">
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="ddlActualDurationHours" runat="server">
                    </asp:DropDownList>:<asp:DropDownList ID="ddlActualDurationMinutes" runat="server">
                    </asp:DropDownList>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="No. of <br/>Carers"  ItemStyle-BackColor="#fffea6">
                <ItemTemplate>
                    <asp:TextBox ID="txtNumberOfCarers" Width="50px" runat="Server" Text='<%# Eval("NumberOfCarers") %>'></asp:TextBox>
                </ItemTemplate>
                <EditItemTemplate>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtNumberOfCarers" runat="Server" ></asp:TextBox>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="2nd <br/> Visit"  ItemStyle-BackColor="#fffea6">
                <ItemTemplate>
                   <asp:CheckBox ID="chkSecondaryVisit" runat="server"  />
                </ItemTemplate>
                <EditItemTemplate>
                   <asp:CheckBox ID="chkSecondaryVisit" runat="server"  />
                </EditItemTemplate>
                <FooterTemplate>
                     <asp:CheckBox ID="chkSecondaryVisit" runat="server"  />
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Visit Code"  ItemStyle-BackColor="#fffea6">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlVisitCode" runat="server" Width="12em">
                    </asp:DropDownList>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlVisitCode" runat="server" Width="12em">
                    </asp:DropDownList>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:DropDownList ID="ddlVisitCode" runat="server" Width="12em">
                    </asp:DropDownList>
                    <%--<asp:Button ID="btnInsert" runat="Server" Text="Insert" CommandName="Insert" UseSubmitBehavior="False" />--%>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:ImageButton ID="btnDel" runat="server" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="btnDel" runat="server" />
                </EditItemTemplate>
                <FooterTemplate>
                    <%--<asp:ImageButton ID="btnDelete" runat="server" CommandName="Delete"/>--%>
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
        <FooterStyle BackColor="#CCCC99" />
        <RowStyle BackColor="#FFFFFF" />
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="White" />
        <EmptyDataTemplate>
            <table width="100%" cellpadding="0" cellspacing="0" class="listTable">
            <thead>
                <tr>
                    <th>
                        Service Type
                    </th>
                    <th>
                        Start time
                    </th>
                    <th>
                        End time
                    </th>
                    <th>
                        Duration Claimed
                    </th>
                    <th>
                        Actual Duration
                    </th>
                    <th>
                        Number of Carers
                    </th>
                    <th>
                        Secondary Visit
                    </th>
                    <th>
                        Visit code
                    </th>
                </tr>
                </thead>
                <%--<tr>
                    <td>
                        <asp:DropDownList ID="ddlServiceType" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlStartTimeHours" runat="server">
                        </asp:DropDownList>
                        :
                        <asp:DropDownList ID="ddlStartTimeMinutes" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlEndTimeHours" runat="server">
                        </asp:DropDownList>
                        :
                        <asp:DropDownList ID="ddlEndTimeMinutes" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddldurationClaimedHours" runat="server">
                        </asp:DropDownList>
                        :
                        <asp:DropDownList ID="ddldurationClaimedMinutes" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlActualDurationHours" runat="server">
                        </asp:DropDownList>
                        :
                        <asp:DropDownList ID="ddlActualDurationMinutes" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:TextBox ID="txtNumberOfCarers" runat="Server"></asp:TextBox>
                    </td>
                    <td>
                    <asp:CheckBox ID="chkSecondaryVisit" runat="server" />
                </td>
                    <td>
                    <asp:DropDownList ID="ddlVisitCode" runat="server">
                    </asp:DropDownList>
                </td>
                </tr>--%>
            </table>
        </EmptyDataTemplate>
    </asp:GridView>
    <asp:Button ID="btnAdd" Width="5em" runat="Server" Text="Add" CommandArgument="test" />
    <asp:Button ID="btnCopy" Width="5em" runat="Server" Text="Copy"/>
    <%--<input id="btnCopy" style="width:5em;" type="button" value="Copy" />--%>
    </p>
    
