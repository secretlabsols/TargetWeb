<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RequestPaymentComplete.aspx.vb"
  Inherits="Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.RequestPaymentComplete" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
  

<asp:content contentplaceholderid="MPPageOverview" runat="server">
    
</asp:content>

<asp:content contentplaceholderid="MPContent" runat="server">
    Thank you. Your request to create payments for the selected contracts has been received.
    <br />
    <br />
    If the system has been configured to do so, appropriate notifications will be sent via email once your request has been processed.
    <br />
    <br />
    You will be able to view the results of processing using the <asp:HyperLink id="lnkPSchedule" runat="server" navigateUrl="~/AbacusExtranet/Apps/Dom/PaymentSchedules/PaymentSchedulesEnquiry.aspx" >Payment Schedules</asp:HyperLink> screen.
    <br />
    <br />    
</asp:content>