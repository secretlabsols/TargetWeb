<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="InPlaceInvoiceVisitsCopier.aspx.vb"
    Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.InPlaceInvoiceVisitsCopier"
    MasterPageFile="~/Popup.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
    <div>
        <fieldset>
            <legend>Copy To</legend>
            <div style="float: left;width:50%;">
                <asp:Label ID="Label1" runat="server" Text="Care Worker"></asp:Label>
                <asp:DropDownList ID="ddlCopyToCareWorker" runat="server" Enabled="false">
                </asp:DropDownList>
            </div>
            <div style="float: left;">
                <asp:Label ID="Label2" runat="server" Text="Day Of Week"></asp:Label>
                <asp:DropDownList ID="ddlCopyToDayOfWeek" runat="server" Enabled="false">
                </asp:DropDownList>
            </div>
            <div class="clearer">
            </div>
        </fieldset>
        
        <fieldset>
            <legend>Copy From</legend>
            <div style="float: left;width:50%;">
                <asp:Label ID="Label3" runat="server" Text="Care Worker"></asp:Label>
                &nbsp;<asp:DropDownList ID="ddlCopyFromCareWorker" runat="server" AutoPostBack="true">
                </asp:DropDownList>
            </div>
            <div style="float: left;">
                <asp:Label ID="Label4" runat="server" Text="Day Of Week"></asp:Label>
                &nbsp;<asp:DropDownList ID="ddlCopyFromDayofWeek" runat="server" AutoPostBack="true">
                </asp:DropDownList>
            </div>
            <div class="clearer">
            </div>
        </fieldset>
        
        <div id="gvScroll"  style="overflow:scroll; height:200px;">

        
        <asp:GridView ID="gvVisits" runat="Server" AutoGenerateColumns="False" CssClass="listTable"
        Width="100%" ShowFooter="false" EnableViewState="True" DataKeyNames="ObjectIndex">
        <EmptyDataRowStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" />
        <Columns>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:HiddenField ID="hiddenObjectIndex" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            
             <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:CheckBox ID="chkObjectIndex" runat="server"  />
                </ItemTemplate>
            
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Service <br/>Type">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlServiceType" runat="server">
                    </asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Start <br/>Time">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlStartTimeHours" runat="server">
                    </asp:DropDownList>
                    :
                    <asp:DropDownList ID="ddlStartTimeMinutes" runat="server">
                    </asp:DropDownList>
                </ItemTemplate>
              
            </asp:TemplateField>
            <asp:TemplateField HeaderText="End <br/>Time">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlEndTimeHours" runat="server">
                    </asp:DropDownList>
                    :
                    <asp:DropDownList ID="ddlEndTimeMinutes" runat="server">
                    </asp:DropDownList>
                </ItemTemplate>
              
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Duration <br/> Claimed">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlDurationClaimedHours" runat="server">
                    </asp:DropDownList>
                    :
                    <asp:DropDownList ID="ddlDurationClaimedMinutes" runat="server">
                    </asp:DropDownList>
                </ItemTemplate>
              
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actual <br/>Duration">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlActualDurationHours" runat="server">
                    </asp:DropDownList>
                    :
                    <asp:DropDownList ID="ddlActualDurationMinutes" runat="server">
                    </asp:DropDownList>
                </ItemTemplate>
              
            </asp:TemplateField>
            <asp:TemplateField HeaderText="No. of <br/>Carers">
                <ItemTemplate>
                    <asp:TextBox ID="txtNumberOfCarers" Width="50px" runat="Server" Text='<%# Eval("NumberOfCarers") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Secondary <br/>Visit">
                <ItemTemplate>
                    <asp:CheckBox ID="chkSecondaryVisit" runat="server" Checked='<%#Eval("SecondaryVisit")%>' />
                </ItemTemplate>
            
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Visit <br/>Code">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlVisitCode" runat="server">
                    </asp:DropDownList>
                </ItemTemplate>
            
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
    
    </div>
        <div style="float:left;">
            <input id="btnSelectAll" runat="server" type="button" value="Select All" style="width:7em;" onclick="checkAllBoxes()"  />
            <input id="btnSelectNone" runat="server" type="button" value="Select None" style="width:7em;" onclick="UnCheckAllBoxes()"  />
            <asp:HiddenField ID="txtGridCount" runat="server" Value="0" />
        </div>
        <div style="float:right;">

        <input id="btnSelected" runat="server" type="button" value="OK" style="width:7em;" onclick="btnSelected_Click()"  />
        <input id="btnClose" runat="server" type="button" value="Cancel" style="width:7em;" onclick="btnClose_Click()"  />

        </div>
    </div>
    <asp:HiddenField ID="hidSelectedObjectIndex" runat="server" />
</asp:Content>
