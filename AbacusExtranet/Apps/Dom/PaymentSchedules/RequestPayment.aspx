<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RequestPayment.aspx.vb"
  Inherits="Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.RequestPayment" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>


<asp:content contentplaceholderid="MPPageOverview" runat="server">
    This screen allows you to request payments for one or more Contract. If configured, you will receive an email confirmation once your request has been processed.
</asp:content>

<asp:content contentplaceholderid="MPContent" runat="server">            
    <cc1:TextBoxEx ID="txtPayUpTo" runat="server" Format="DateFormatJquery" LabelText="Pay Up To" LabelWidth="7em" ></cc1:TextBoxEx>
    <br />           
    <legend id="fsControlsLegend" runat="server" />                  
    <label id="lblFilterCriteria" class="errorText DialogTableFiltering"></label>  
    <table id="tblContracts" class="tablescroll" cellspacing="0" cellpadding="2" summary="List of Contracts" width="99%">
        <thead>
            <tr>
               <th filterTableType="Custom" style="width: 3%"><input type="checkbox" id="chkAll" onclick="toggleChecked(this.checked)" /></th>
               <th filterTableType="TextBox" style="width: 10%">Provider Ref</th>
               <th filterTableType="TextBox" style="width: 35%">Provider Name</th>
               <th filterTableType="TextBox" style="width: 20%">Contract Number</th>
               <th filterTableType="TextBox" style="width: 35%">Contract Title</th>
            </tr>
        </thead>
        <tbody id="tblBody">
            <%--Table rows are added Dynamically--%>
            <tr style="height:0px;">
               <td></td>
               <td></td>
               <td></td>
               <td></td>
               <td></td>
            </tr>
        </tbody>
    </table>
     <div id="divFooter" style="width: 99%">
        <br />
        <label id="lblContractCountSummary" class="errorText ContractSummary" style="float: left; font-weight: bold !important; "></label>  
        <input type="button" id="btnSubmit" runat="server" style="float:right;width:5em;" value="Submit" onclick="btnSubmit_Click();"/>
        <br />
    </div>
</asp:content>
