<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Validation.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.SystemSettings.VisitValidation" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different visit validation rules that are enforced when visit information is entered.
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server">
        <cc1:TextBoxEx ID="txtMinCarersPerVisit"  runat="server" LabelText="Minimum No. of Carers per Visit" Format="IntegerFormat" LabelWidth="18em"
            ValidationGroup="Save" IsReadOnly="true"></cc1:TextBoxEx>
        <br /><br />
        <cc1:DropDownListEx ID="cboMaxCarersPerVisit" runat="server" LabelText="Maximum No. of Carers per Visit" LabelWidth="18em" 
			Width="5em" Required="true" RequiredValidatorErrMsg="Please select a value" ValidationGroup="Save"></cc1:DropDownListEx>
        <br />
        <cc1:TextBoxEx ID="txtMinimumVisitDuration"  runat="server" LabelText="Minimum Visit Duration (mins)" Format="IntegerFormat" LabelWidth="18em"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TimePicker id="tmeMaximumVisitDuration" runat="server" LabelText="Maximum Visit Duration (hrs:mins)" ShowSeconds="false" LabelWidth="18em"></cc1:TimePicker>
        <br /><br />
        <cc1:TextBoxEx ID="txtMaxVisitsPerWeek"  runat="server" LabelText="Maximum No. of Visits per Week" Format="IntegerFormat" LabelWidth="18em"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
        <cc1:TextBoxEx ID="txtMaxHoursPerWeek"  runat="server" LabelText="Maximum No. of Hours per Week" Format="IntegerFormat" LabelWidth="18em"
            ValidationGroup="Save"></cc1:TextBoxEx>
        <br />
    </cc1:UpDownControl>
    
        
    </fieldset>
    <br />
</asp:Content>
