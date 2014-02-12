<%@ Control Language="vb" AutoEventWireup="false" EnableViewState="true" CodeBehind="FinanceCodeTable.ascx.vb" Inherits="Target.Abacus.Web.Apps.Dom.SvcOrders.FinanceCodeTable" %>
<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register TagPrefix="uc1" TagName="InPlaceExp" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceExpenditureAccountSelector.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceFinCode" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.ascx" %>


    <input type="text" style="display:none;" id="hidFinCodeApportionEqually" runat="server" />
    <asp:GridView ID="gvFinanceCodes" Width="100%" runat="Server" AutoGenerateColumns="False" CssClass="listTable"
            ShowFooter="false" EnableViewState="True" CellPadding="3"  >
        <EmptyDataRowStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" />
        <Columns>
            <asp:TemplateField HeaderText="Expenditure Account" ItemStyle-Width="25em" ItemStyle-BackColor="#ffddac">
                <ItemTemplate>
                    <input type="text" style="display:none;" id="hidDSOFundingID" runat="server" value='<%# Eval("DSOFundingID") %>' />
                    <input type="text" style="display:none;" id="hidDSOFundingDetailID" runat="server" value='<%# Eval("DSOFundingDetailID") %>' />
                    <input type="text" style="display:none;" id="hidExpAccType" runat="server" />
                    <uc1:InPlaceExp id="expenditureAccount" runat="server" enableAccountTypeCombo="false"  ExpenditureAccountGroupID='<%# Eval("expenditureAccountGroupID") %>'></uc1:InPlaceExp>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Finance Code" ItemStyle-Width="17em" ItemStyle-BackColor="#ffddac">
                <ItemTemplate>
                    <uc2:InPlaceFinCode id="financeCode" FinanceCodeID='<%# Eval("FinanceCodeID") %>' runat="server"></uc2:InPlaceFinCode>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Balancing" ItemStyle-Width="7em" ItemStyle-HorizontalAlign="Center" ItemStyle-BackColor="#fffea6">
                <ItemTemplate>
                    <asp:RadioButton id= "optBalancing"  runat="server" AutoPostBack="false"  enabled='<%# enableBalancing() %>' checked='<%# Eval("balancing") %>'  />
                    <input type="text" style="display:none;" id="hidoptBalancing" runat="server" value='<%# Eval("balancing") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Call Off" ItemStyle-Width="6em" ItemStyle-BackColor="#fffea6">
                <ItemTemplate >
                    <asp:DropDownList ID="ddlCallOff" runat="server"  Enabled='<%# enableCallOff() %>' Width="5em">
                    </asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Funded By" ItemStyle-Width="12em" ItemStyle-BackColor="#fffea6">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlFundedBy" runat="server" Width="10.5em">
                    </asp:DropDownList>
                    <input type="text" style="display:none;" id="hidexpAccountType" runat="server" value='<%# Eval("ExpenditureAccountType") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Proportion" ItemStyle-Width="8em" ItemStyle-HorizontalAlign="Center"  ItemStyle-BackColor="#fffea6">
                <ItemTemplate>
                    <span id="fundingRowIdentifier" style="display:none;" ><%# Eval("rowIdentifier")%></span>
                    <asp:Label ID="txtProportion" Width="50px" runat="Server" Text='<%# GetProportionText(Eval("proportion")) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" ItemStyle-BackColor="#fffea6" >
                <ItemTemplate>
                    <asp:ImageButton ID="btnDel" OnCommand="DeleteRow" OnClientClick="javascript:FlagControlsToBeRecreated();"  CommandArgument='<%# Eval("rowIdentifier") %>' 
                            ToolTip="Delete this Row?" runat="server" ImageUrl="../../../../../Images/Cross.png" ValidationGroup="AddFinanceCode" />
  
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
            <table width="100%" cellpadding="10px" cellspacing="0" class="listTable">
            <thead>
                <tr>
                    <th>
                        Expenditure Account
                    </th>
                    <th>
                        Finance Code
                    </th>
                    <th>
                        Balancing
                    </th>
                    <th>
                        Call Off
                    </th>
                    <th>
                        Funded By
                    </th>
                    <th>
                        Proportion
                    </th>
                </tr>
                </thead>

            </table>
        </EmptyDataTemplate>
    </asp:GridView>
    <div style="float:left;" >
        <asp:Button id="btnAdd" runat="server" Text="Add" OnClientClick="javascript:AddFundingRow();" Enabled='<%#enableDefaultButton()%>' ValidationGroup="AddFinanceCode" />
    </div>
    <div style="float:right;" >
        <asp:Button id="btnSetDefault" runat="server" Text="Default" ToolTip="Set as the Default Funding Detail"  ValidationGroup="AddFinanceCode" />
        <asp:Button id="btnAmendProportions" runat="server" OnClientClick="javascript:FlagControlsToBeRecreated();" Text="Amend Proportions" ToolTip="Amend Finance Code Proportions" ValidationGroup="AddFinanceCode" />
    </div>
    
    

