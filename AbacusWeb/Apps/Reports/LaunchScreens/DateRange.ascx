<%@ Control Language="vb" AutoEventWireup="false" Codebehind="DateRange.ascx.vb" Inherits="Target.Abacus.Web.Apps.Reports.LaunchScreens.DateRange" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<%--Date From--%>
<cc1:TextBoxEx ID="dteDateFrom" runat="server" LabelText="Date From" LabelWidth="10.5em"
    Required="true" RequiredValidatorErrMsg="Please enter the date you want to report from." Format="DateFormat"
    ValidationGroup="dateRange"></cc1:TextBoxEx>
<br />
<%--Date From--%>
<cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="Date To" LabelWidth="10.5em"
    Required="true" RequiredValidatorErrMsg="Please enter the date you want to report upto." Format="DateFormat"
    ValidationGroup="dateRange"></cc1:TextBoxEx>
<br />