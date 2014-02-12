<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ucPaymentPreviewOptions.ascx.vb" Inherits="Target.Abacus.Web.Apps.Jobs.UserControls.ucPaymentPreviewOptions" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>

<div style="width : 100%;">
    <asp:radiobutton id="optReportOnly" groupname="grpCreateJobOption" TextAlign="left" height="2em" width="100%" 
        runat="server" text="Report Only" onclick="javascript:optMode_Click();" ToolTip="Create reports only and do no generate payments" />
        
    <asp:radiobutton id="optGeneratePayments" groupname="grpCreateJobOption" TextAlign="left" height="2em" width="100%" 
        runat="server" text="Generate Payments" onclick="javascript:optMode_Click();" ToolTip="Generate payments and reports" />
    <asp:CustomValidator id="CustomValidatorPaymentPreviewOptions" runat="server" 
    Display="Dynamic" ErrorMessage="You must select either 'Report Only' or 'Generate Payments' option to create this job" 
    ClientValidationFunction="CustomValidatorPaymentPreviewOptions_ClientValidate" 
    OnServerValidate="CustomValidatorPaymentPreviewOptions_ServerValidate"
    ValidationGroup="Save"></asp:CustomValidator>   
</div>
<div class="clearer" />  
<asp:HiddenField ID="hidGenerateMode" runat="server" Value="3" />


 



 