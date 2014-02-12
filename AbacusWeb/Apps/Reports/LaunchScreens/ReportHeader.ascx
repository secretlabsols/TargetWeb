<%@ Control Language="vb" AutoEventWireup="false" Codebehind="Reportheader.ascx.vb" Inherits="Target.Abacus.Web.Apps.Reports.LaunchScreens.ReportHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<%--Description--%>
<cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Report" LabelWidth="10.5em" 
        IsReadOnly="true" Format="TextFormat" ></cc1:TextBoxEx>
<br /><br />
<cc1:TextBoxEx ID="txtCategories" runat="server" LabelText="Categories" LabelWidth="10.5em" 
        IsReadOnly="true" Format="TextFormat" ></cc1:TextBoxEx>
