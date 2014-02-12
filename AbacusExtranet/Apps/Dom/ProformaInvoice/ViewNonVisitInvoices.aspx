<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ViewNonVisitInvoices.aspx.vb"  Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.ViewNonVisitInvoices" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<%@ Register TagPrefix="uc3" TagName="psHeader" Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx"%>
<%@ Register TagPrefix="ucVoidPaymentdue" TagName="VoidPaymentdue" Src="~/AbacusExtranet/Apps/UserControls/VoidPaymentDue.ascx"%>
<asp:Content ID="cpOverview" ContentPlaceHolderID="MPPageOverview" runat="server">
    
    This screen allows you to maintain pro forma invoices.
    
</asp:Content>

<asp:Content ID="cpError" ContentPlaceHolderID="MPPageError" runat="server">

    <asp:Label ID="lblError" runat="server" CssClass="errorText" />
    
</asp:Content>

<asp:Content ID="cpContent" ContentPlaceHolderID="MPContent" runat="server">      
    <div style="height:3.2em;">
        <uc1:StdButtons id="stdButtons1" runat="server" />
    </div>
    <div class="clearer"></div>
    <div>
        <div style="float:left; width:98%; margin-left:5px;">
            <cc1:CollapsiblePanel id="cpDetail" runat="server" HeaderLinkText="Details" MaintainClientState="true">
                <ContentTemplate>
                    <uc3:psHeader id="psHeader1" runat="server" />
                </ContentTemplate>
            </cc1:CollapsiblePanel>    
        </div>
        <div style="float:right; margin-right:30px;">
            <ucVoidPaymentdue:VoidPaymentdue runat="server" id="ucVoidPaymentDue" ></ucVoidPaymentdue:VoidPaymentdue>
        </div>
    </div>
    <div class="clearer"></div>
   
    <fieldset id="fsControls" runat="server" enableviewstate="false" style="margin-top:8px;">
        <legend id="fsControlsLegend" runat="server" />                  
        <asp:Repeater ID="rptProformas" runat="server" EnableViewState="false" >
            <HeaderTemplate>
               <label id="lblFilterCriteria" class="errorText FilteringLabel"></label>  
               <table class="tablescroll" id="tblProformas" cellspacing="0" cellpadding="0" summary="List of Proforma Invoice." width="99%">
                    <thead>
                        <tr>
                           <%--<th class="s">&nbsp;</th>--%>
                           <th filterTableType="Custom" style="width: 3%"><input type="checkbox" id="chkAll" onclick="toggleChecked(this.checked)" /></th>
                           <th filterTableType="TextBox" style="width: 12%"  class="sr"><span id="periodicSURef">Service User Ref</span></th>
                           <th filterTableType="TextBox" class="sn" style="width: 23%"><span id="periodicContract">Service User Name</span></th>
                           <th filterTableType="None" class="pr" style="width: 10%" >Payment Ref</th>
                           <th filterTableType="None" class="pr" style="width: 9%">Date</th>
                           <% if (HasUserVerifyUnverify()) THEN %>
                           <th filterTableType="None" class="w" style="width: 6%">Update</th>
                           <% END IF %>
                           <th filterTableType="DropDown" class="w" style="width: 6%">Weeks</th>
                           <th filterTableType="DropDown" class="st"  style="width: 15%">Status</th>
                           <th class="p" style="width: 8%" >Payment</th>
                           <th filterTableType="Custom" class="q" style="width: 5%" >Notes</th>
                           <th filterTableType="Custom" class="es" style="width: 5%" >&nbsp;</th>
                        </tr>
                    </thead>
                    <tbody id="tblBody">
                        <tr style="height: 0px;">
                            <td ></td>
                            <td ></td>
                            <td ></td>
                            <td ></td>
                              <% if (HasUserVerifyUnverify()) THEN %><td ></td><% END IF %>
                            <td ></td>
                            <td ></td>
                            <td ></td>
                            <td ></td>
                            <td ></td>                   
                        </tr> 
            </HeaderTemplate>
            <ItemTemplate>                
                <tr id="tr_<%# DataBinder.Eval(Container.DataItem, "ID")%>">
                    <td class="s"><input type="hidden" id="InvoiceId" value="<%#DataBinder.Eval(Container.DataItem, "ID")%>" />
                                  <input type="hidden" id="voidPayment" name="voidPayment" value="<%#DataBinder.Eval(Container.DataItem, "VoidPayment").ToString().ToLower()%>" />  
                                  <input type="hidden" id="IsBlockGuranteeContract" name="IsBlockGuranteeContract" value="<%#DataBinder.Eval(Container.DataItem, "IsBlockGuranteeContract").ToString().ToLower()%>" />  
                                  <input type="checkbox" id="rbSelected" name="rbSelected" value="" onclick="ProformaSelected(this, <%#DataBinder.Eval(Container.DataItem, "ID")%>);" />
                    </td>
                    <td class="sr"><%#CheckNullOrEmpty(DataBinder.Eval(Container.DataItem, "ServiceUserReference"))%></td>
                    <td class="sn"><a onclick="ShowProforma(<%#DataBinder.Eval(Container.DataItem, "ID")%>, '<%#DataBinder.Eval(Container.DataItem, "ServiceUserReference")%>', '<%#DataBinder.Eval(Container.DataItem, "ServiceUserName")%>', <%#DataBinder.Eval(Container.DataItem, "ServiceUserID")%>)"><%#CheckNullOrEmpty(DataBinder.Eval(Container.DataItem, "ServiceUserName"))%></a></td>
                    <td class="pr"><input onblur="PaymentRefChanged('<%#DataBinder.Eval(Container.DataItem, "ID")%>', this.value);" id="PaymentRef" tabindex="<%# 1 + (Container.ItemIndex) * 3 %>" type="text" value="<%#CheckNullOrEmpty(DataBinder.Eval(Container.DataItem, "OurReference"))%>" style="width:7em;" /></td>
                    <td>  <cc1:TextBoxEx TagId='<%#DataBinder.Eval(Container.DataItem, "ID")%>' tabindex="<%# 2 + (Container.ItemIndex) * 3 %>" ID="dtePaymentRefDate" runat="server" Format="DateFormat"
                             ValidationGroup="Save" OutputBrAfter="false" Width="5em" Text='<%#DataBinder.Eval(Container.DataItem, "InvoiceDate")%>' >
                        </cc1:TextBoxEx>
                    </td>
                    <% if (HasUserVerifyUnverify()) THEN %>
                    <td> <input onclick="InLineStatusChanged('<%#DataBinder.Eval(Container.DataItem, "ID")%>',this.value, this);"  tabindex="<%# 3 + (Container.ItemIndex) * 3 %>"  type="button" id="InlineStatusChange" value="<%#IIF(DataBinder.Eval(Container.DataItem, "BatchStatusDescription") = "Verified", "UnVerify", "Verify")%>" />   </td>
                    <% END IF %>
                    <td class="w"><%#DataBinder.Eval(Container.DataItem, "Weeks")%></td>
                    <td class="st <%#IIF(DataBinder.Eval(Container.DataItem, "BatchStatusDescription") = "Verified", "stv", "stav")%>"><%#CheckNullOrEmpty(DataBinder.Eval(Container.DataItem, "BatchStatusDescription"))%></td>
                    <td class="p">&pound;<%#DataBinder.Eval(Container.DataItem, "CalculatedPayment", "{0:0.00}")%></td>
                    <td class="q q<%#DataBinder.Eval(Container.DataItem, "QueryType")%>" title="<%#DataBinder.Eval(Container.DataItem, "QueryTypeDescription")%>" onclick="ShowNote(<%#DataBinder.Eval(Container.DataItem, "ID")%>)")>&nbsp;</td>
                    <td class="es es<%#DataBinder.Eval(Container.DataItem, "EditStatus")%>" title="<%#DataBinder.Eval(Container.DataItem, "EditStatusDescription")%>">&nbsp;</td>                   
                </tr>                
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <br />  
        <hr />
        <div style="float:left;">
            <label id="lblInvoiceCountSummary" class="errorText ContractSummary" style="float: left"></label>
        </div>
        <div id="divButtonsRight" class="BottomRightButtons" style="float:right;">
            <input type="button" value="Recalculate" onclick="Recalculate_click();" runat="server" id="btnRecalculate" disabled="disabled" title="Recalculate Proforma Invoice" />
            <input type="button" value="Delete" onclick="DeleteProforma();" runat="server" id="btnDeleteProforma" title="Delete Proforma Invoice(s)?" />
            <input type="button" value="Verify" onclick="VerifyProforma();" runat="server" id="btnVerifyProforma" title="Verify Proforma Invoice(s)?" />
            <input type="button" value="UnVerify" onclick="UnVerifyProforma();" runat="server" id="btnUnVerifyProforma" title="UnVerify Proforma Invoice(s)?" />
            <uc2:ReportsButton runat="server" ID="rptPrint" />
        </div> 
        <input type="hidden" name="hidContractBlockGuarantee" id="hidContractBlockGuarantee" value="false" />
    </fieldset>
</asp:Content>
