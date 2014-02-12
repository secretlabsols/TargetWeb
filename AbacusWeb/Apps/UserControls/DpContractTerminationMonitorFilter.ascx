<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DpContractTerminationMonitorFilter.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.DpContractTerminationMonitorFilter" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<br />

<fieldset id="fsContractTypes" style="padding:0.5em;" runat="server"> 
    <legend>Contract Type</legend>   
    <asp:RadioButtonList ID="rblContractTypes" runat="server" RepeatDirection="Vertical">
        <asp:ListItem Text="Do not filter by this item" Value="" Selected="True" />
        <asp:ListItem Text="Show SDS contracts" Value="true" />
        <asp:ListItem Text="Show non-SDS contracts" Value="false" />
    </asp:RadioButtonList>    
</fieldset>

<fieldset id="fsUnderOrOverPayments" style="padding:0.5em;" runat="server"> 
    <legend>Under/Over Payments</legend>   
    <asp:RadioButtonList ID="rblUnderOrOverPayments" runat="server" RepeatDirection="Vertical">
        <asp:ListItem Text="Do not filter by this item" Value="" Selected="True" />
        <asp:ListItem Text="Show under-paid contracts" Value="true" />
        <asp:ListItem Text="Show over-paid contracts" Value="false" />
    </asp:RadioButtonList>    
</fieldset>

<fieldset id="fsBalanced" style="padding:0.5em;" runat="server"> 
    <legend>Balanced</legend>   
    <asp:RadioButtonList ID="rblBalanced" runat="server" RepeatDirection="Vertical">
        <asp:ListItem Text="Do not filter by this item" Value="" />
        <asp:ListItem Text="Show contracts that have been balanced" Value="true" />
        <asp:ListItem Text="Show contracts that have not been balanced" Value="false" Selected="True" />
    </asp:RadioButtonList>    
</fieldset>

<fieldset id="fsTerminationPeriod" style="padding:0.5em;" runat="server"> 
    <legend>Termination Period</legend>
    <cc1:TextBoxEx ID="dteTerminatedFrom" runat="server" LabelText="Show contracts terminated from"  Format="DateFormatJquery" LabelBold="false" LabelWidth="18em" Width="7em" OutputBrAfter="false" AllowClear="true" />
    &nbsp;
    <cc1:TextBoxEx ID="dteTerminatedTo" runat="server" LabelText="to &nbsp;"  Format="DateFormatJquery" LabelBold="false" LabelWidth="7" Width="7em" OutputBrAfter="true" AllowClear="true" />
</fieldset>

<br />
