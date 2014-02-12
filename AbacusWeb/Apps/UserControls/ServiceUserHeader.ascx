<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ServiceUserHeader.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.ServiceUserHeader" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<style type="text/css">
    .suHeaderReadOnlyCss { font-weight:bold; }
    div.suHeaderColumns { float:left;width:25%; }
</style>

<div class="suHeaderColumns">
    <cc1:TextBoxEx ID="txtReference"  runat="server" LabelText="Reference" LabelWidth="6em" IsReadOnly="true" ReadOnlyContentCssClass="suHeaderReadOnlyCss"></cc1:TextBoxEx>
</div>
<div class="suHeaderColumns">
    <cc1:TextBoxEx ID="txtName"  runat="server" LabelText="Name" LabelWidth="3.5em" IsReadOnly="true" ReadOnlyContentCssClass="suHeaderReadOnlyCss"></cc1:TextBoxEx>
</div>
<div class="suHeaderColumns">    
    <cc1:TextBoxEx ID="txtDateOfBirth"  runat="server" LabelText="Date of Birth" LabelWidth="7em" IsReadOnly="true" ReadOnlyContentCssClass="suHeaderReadOnlyCss"></cc1:TextBoxEx>
</div>
<div class="suHeaderColumns">
    <cc1:TextBoxEx ID="txtDateOfDeath"  runat="server" LabelText="Date of Death" LabelWidth="8em" IsReadOnly="true" ReadOnlyContentCssClass="suHeaderReadOnlyCss"></cc1:TextBoxEx>
</div>