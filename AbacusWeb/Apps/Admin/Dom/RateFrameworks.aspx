<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RateFrameworks.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.RateFrameworks" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc3" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different rate frameworks.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewState="false">
        <div style="float: left; padding-right: 2em;">
            <cc1:TextBoxEx ID="txtDescription"  runat="server" LabelText="Description" LabelWidth="15em" MaxLength="255" 
                Width="20em"  Required="true" RequiredValidatorErrMsg="Please enter a description" SetFocus="true"
                ValidationGroup="Save"></cc1:TextBoxEx>
            <br />
            <cc1:DropDownListEx ID="cboFrameworkType" runat="server" LabelText="Framework Type" LabelWidth="15em"
			    Required="True" RequiredValidatorErrMsg="Please select a framework type" ValidationGroup="Save"></cc1:DropDownListEx>
		    <br />   
		    <cc1:CheckBoxEx ID="chkAddManualPrc" runat="server" Text="Add Manual Payment Rate Category" LabelWidth="22em"></cc1:CheckBoxEx>
            <br />     
            <br />
            <cc1:CheckBoxEx ID="chkUseEnhancedRateDays" runat="server" Text="Use Enhanced Rate Days" LabelWidth="22em"></cc1:CheckBoxEx>
            <br />
            <br />
            <cc1:CheckBoxEx ID="chkRedundant" runat="server" Text="Redundant" LabelWidth="22em"></cc1:CheckBoxEx>        
        </div>    
        <div style="float: left;" id="divt" runat="server">
            <cc1:TextBoxEx ID="txtAbbreviation"  runat="server" LabelText="Abbreviation" LabelWidth="15.75em" MaxLength="50"></cc1:TextBoxEx>
            <br />  
            <fieldset>
			    <legend>Contract Numbering</legend>
                <cc1:TextBoxEx ID="txtContractNoPrefix"  runat="server" LabelText="Contract Number Prefix" LabelWidth="15em" MaxLength="7"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtNextContractNo"  runat="server" LabelText="Next Available Contract No." LabelWidth="15em" MaxLength="10"></cc1:TextBoxEx>
                <br />			    
			</fieldset>        
        </div>
        <div class="clearer"></div>
        <br />
        <fieldset>
			<legend>Details</legend>
			<table class="listTable" id="tblDetails" cellspacing="0" cellpadding="2" summary="List of details in this framework." width="100%">
            <caption>List of details in this framework</caption>
            <thead>
	            <tr>
		            <th>Day of Week</th>
		            <th>Standard Day Category</th>
		            <th>Enhanced Day Category</th>
	            </tr>
            </thead>
            <tbody>
				<asp:PlaceHolder ID="phDetails" runat="server"></asp:PlaceHolder>
            </tbody>
			</table>
        </fieldset>
        <br />
        <input type="button" id="btnViewCategories" style="float:right;width:12em; margin-left:3px;" value="View Rate Categories" runat="server" />
        <div style="float:right;"><uc3:ReportsButton id="ctlPrint" runat="server" ButtonText="Print" VerticalPosition="1" ButtonWidth="5em" ></uc3:ReportsButton></div>
        <div class="clearer"></div>
    </fieldset>
    <br />

</asp:Content>