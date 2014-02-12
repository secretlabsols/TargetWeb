<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ManualEnterInvoice.aspx.vb"
    Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.ManualEnterInvoice" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register Src="../../UserControls/ManualEnterVisits.ascx" TagName="ManualEnterVisits"
    TagPrefix="uc2" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Reference Control="~/AbacusExtranet/Apps/UserControls/ManualEnterVisits.ascx" %>
<asp:content contentplaceholderid="MPPageOverview" runat="server">
<%@ Register TagPrefix="uc3" TagName="PaymentScheduleHeader" 
Src="../../UserControls/PaymentScheduleHeader.ascx" %>
		
</asp:content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
    
<asp:content contentplaceholderid="MPContent" runat="server">
    <div>
         <div style="float:left;" >
            <uc1:StdButtons id="stdButtons1" runat="server" OnCancelClientClick="return btnBack_Click();"  
	        OnBackClientClick="return btnBack_Click(true);" OnDeleteClientClick="return btnDelete_Click();"></uc1:StdButtons>
	     </div>   
         <div style="float:left;margin-left:0.2em;" >
            <fieldset style="padding:0.2em;" >
                <input runat="server" type="button" style="width:7em;" value="Invoice Lines" 
                        id="btnInvoiceLines" name="btnInvoiceLines" 
                        onclick="btnInvoiceLines_click();" />
            </fieldset>
         </div>
         <div class="clearer"></div>  
    </div>
        <fieldset>
            <legend>Invoice Header</legend>
            <uc3:PaymentScheduleHeader runat="server" ID="pScheduleHeader" />
              <asp:Panel runat="server" id="pnlInvoice">
            <div style="width:50em; margin-bottom: 5px; margin-top: 5px;" >
                <cc1:TextBoxEx ID="txtProvider" runat="server" IsReadOnly="true" LabelBold="false" LabelText="Provider" LabelWidth="11.8em"></cc1:TextBoxEx>
            </div>
            <div style="width:50em; margin-bottom: 5px;" >
                <cc1:TextBoxEx ID="txtContract" runat="server" IsReadOnly="true" LabelBold="false" LabelText="Contract" LabelWidth="11.8em"></cc1:TextBoxEx>
            </div>
            <div style="width:50em; margin-bottom: 5px;" >
                <cc1:TextBoxEx ID="txtClient" runat="server" IsReadOnly="true" LabelBold="false" LabelText="Service User" LabelWidth="11.8em"></cc1:TextBoxEx>
            </div>
            <div style="width:50em;" >
                <div style="margin-top:5px;">
                    <div style="float:left;" >
                        <cc1:TextBoxEx ID="dteWeekEnding" runat="server" LabelText="Week Ending" Format="TextFormat"
                         LabelWidth="11.8em" EnableViewState="true" Required="true" RequiredValidatorErrMsg="Please enter a week ending date"
                         ValidationGroup="Save"  >
                         </cc1:TextBoxEx>
                     </div>
                     <div style="float:right;" >
                        <cc1:TextBoxEx ID="txtReference" runat="server" LabelText="Reference" Format="TextFormat"
                            LabelWidth="11.8em" EnableViewState="true" MaxLength="50" Required ="true" RequiredValidatorErrMsg="Please enter a Reference number"
                            ValidationGroup="Save">
                        </cc1:TextBoxEx>
                     </div>
                     <div class="clearer"></div>
                 </div>
              <%--   <br />--%>
                 <div style="margin-top:5px;">
                     <div style="float:left;" >
                        <cc1:TextBoxEx ID="txtPaymentClaimed" runat="server" LabelText="Payment Claimed" Format="CurrencyFormat"
                        LabelWidth="11.8em">
                        </cc1:TextBoxEx>
                     </div>
                     <div style="float:right;" >
                        <cc1:TextBoxEx ID="txtDirectIncome" runat="server" LabelText="Direct Income" Format="CurrencyFormat"
                            LabelWidth="11.8em">
                        </cc1:TextBoxEx>
                    </div>
                    <div class="clearer"></div>
                </div>
               <%-- <br />--%>
                <div style="margin-top:5px;">
                    <div style="float:left;" >
                        <cc1:TextBoxEx ID="txtNoOfVisits" runat="server" LabelText="No. Of Visits" Format="TextFormat"
                      LabelWidth="11.8em">
                        </cc1:TextBoxEx>
                    </div>
            
                    <div style="float:right;" >
                        <cc1:TextBoxEx ID="txtNoOfHours" runat="server" LabelText="No. of hours" Format="TextFormat"
                            LabelWidth="11.8em">
                        </cc1:TextBoxEx>
                    </div>
                 </div>
           </div>
            
          </asp:Panel>  
        </fieldset>
  
        <fieldset>
            <legend>Visits</legend>
               <asp:Panel runat="server" id="PnlVisits">
                    <ajaxToolkit:TabContainer runat="server" ID="tabStripVisits" >
                        <ajaxToolkit:TabPanel  runat="server" ID="tobeRemovedAtRuntime" >
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                </asp:Panel>
        </fieldset>
            <input type="text" style="display: none;" id="txtHidReference" runat="server" />
            <input type="text" style="display: none;" id="txtHidName" runat="server" />
            <input type="text" style="display: none;" id="txtHidId" runat="server" />
            <%--edit  reference holder--%>
            <asp:HiddenField  ID="txtEditCareProviderRef" runat="server" />
            <asp:HiddenField  ID="txtEditCareProviderName" runat="server" />
            <asp:HiddenField  ID="txtEditCareProviderId" runat="server" />
            <asp:HiddenField  ID="txtExistingCareWorkerList" runat="server" />
            <%--edit  reference holder--%>
            
        
        
        <input type="text" style="display: none;" id="txtCopyToName" runat="server" />
        <input type="text" style="display: none;" id="txtCopyToReference" runat="server" />
        <input type="text" style="display: none;" id="txtCopyVisits" runat="server" />
        <input type="text" style="display: none;" id="txtCopyToWeekDay" runat="server" />
        <input type="text" style="display: none;" id="txtCollapsablePanel" runat="server" />
        
        <input type="text" style="display: none;" id="txthidtabCount" runat="server" />
        
        
        <asp:HiddenField ID="OriginalValueChanged" runat="server" />
        <asp:HiddenField ID="EnableAddTab" runat="server" value="0" />
        <asp:HiddenField ID="CollapsablePanel" runat="server" value="0" />
        <asp:HiddenField ID="AddNewRow" runat="server" value="True" />
        
 </asp:content>
