<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UnitsOfMeasure.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.UnitsOfMeasure"
    EnableViewState="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different units of measure.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <div class="divFieldSeperator"> <cc1:TextBoxEx ID="txtDescription"  runat="server" LabelText="Description" LabelWidth="12em" MaxLength="15" 
            Width="10em"  Required="true" RequiredValidatorErrMsg="Please enter a description" SetFocus="true"
            ValidationGroup="Save"></cc1:TextBoxEx></div>
       <div class="clearer"></div>
       <div class="divFieldSeperator"> <cc1:TextBoxEx ID="txtComment"  runat="server" LabelText="Comment" LabelWidth="12em" MaxLength="100" 
            Width="30em"  Required="true" RequiredValidatorErrMsg="Please enter a comment"
            ValidationGroup="Save"></cc1:TextBoxEx></div>
       <div class="clearer"></div>
        <div style="padding-bottom:4em;">
        <cc1:CheckBoxEx ID="chkAllowUseServiceRegisters" runat="server" Text="Allow use with Service Registers" LabelWidth="11.75em"></cc1:CheckBoxEx>
        </div>
        <div class="clearer"></div>
        <div style="padding-bottom:3.5em;">
        <cc1:CheckBoxEx ID="chkUnitsDisplayedAsHoursMins" runat="server" Text="Units displayed as hours and minutes" LabelWidth="11.75em"></cc1:CheckBoxEx>
        </div>
        <div class="clearer"></div>
        <div class="divFieldSeperator">
         <cc1:TextBoxEx ID="txtMinutesPerUnit"  runat="server" LabelText="Minutes Per Unit" LabelWidth="12em" Format="IntegerFormat" Width="3em"
            Required="true" RequiredValidatorErrMsg="Please enter the number of minutes per unit" ValidationGroup="Save"></cc1:TextBoxEx>
        </div>
       
        
    </fieldset>
    <br />
</asp:Content>