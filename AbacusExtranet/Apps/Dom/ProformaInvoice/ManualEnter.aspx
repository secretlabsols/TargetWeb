<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ManualEnter.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.Proforma.ManualEnter" 
    EnableEventValidation="false" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		To manually enter visit information and create a domiciliary proforma invoice, enter the details below.
	</asp:Content>
    <asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <uc1:StdButtons id="stdButtons1" runat="server" OnCancelClientClick="return btnCancel_Click();"  
	    OnBackClientClick="return btnBack_Click();" OnDeleteClientClick="return btnDelete_Click();" ></uc1:StdButtons>
	    <fieldset id="fsControls" style="padding:0.5em;" runat="server" >
            <cc1:TextBoxEx ID="txtProvider"  runat="server" IsReadOnly="true" LabelBold="true" LabelText="Provider" LabelWidth="10em"></cc1:TextBoxEx>
            <br />
            <cc1:TextBoxEx ID="txtContract" runat="server" IsReadOnly="true" LabelBold="true" LabelText="Contract" LabelWidth="10em"></cc1:TextBoxEx>
            <br />
            <cc1:TextBoxEx ID="txtClient" runat="server" IsReadOnly="true" LabelBold="true" LabelText="Service User" LabelWidth="10em"></cc1:TextBoxEx>
            <br /><br />
            <cc1:TextBoxEx ID="dteWeekEnding"  runat="server" LabelText="Week Ending" LabelWidth="10em" Format="DateFormat"
                Required="true" RequiredValidatorErrMsg="Please enter a week ending date" ValidationGroup="Save"></cc1:TextBoxEx>
            <asp:RangeValidator id="rvWeekEnding" runat="server" ValidationGroup="Save" Display="Dynamic" EnableClientScript="True" ErrorMessage="Actual service for future periods may not be entered" SetFocusOnError="True" />
            <br />
            <cc1:TextBoxEx ID="txtReference" runat="server" LabelText="Reference" LabelWidth="10em" MaxLength="50"></cc1:TextBoxEx>
            <br />
            <cc1:TextBoxEx ID="txtPaymentClaimed" runat="server" LabelText="Payment Claimed" LabelWidth="10em" Format="CurrencyFormat"></cc1:TextBoxEx>
            <br />
            <fieldset runat="server" EnableViewState="true">
                <legend>Visits</legend>
                <table id="tblVisits" class="listTable" cellspacing="0" cellpadding="2" summary="List of visits" width="100%">
				    <caption>List of visits</caption>
				    <thead>
					    <tr>
						    <th style="vertical-align:top;">Service Type</th>
						    <th style="vertical-align:top;">Visit Day</th>
						    <th style="vertical-align:top;">Start Time</th>
						    <th style="vertical-align:top;">Duration Claimed</th>
						    <th style="vertical-align:top;">Actual Duration</th>
						    <th style="vertical-align:top;">Number<br />of Carers</th>
						    <th style="vertical-align:top;">Secondary<br />Carers</th>
						    <th style="vertical-align:top;">Visit Code</th>
						    <th>&nbsp;</th>
					    </tr>
				    </thead>
				    <tbody>
					    <asp:PlaceHolder ID="phVisits" runat="server"></asp:PlaceHolder>
				    </tbody>
				    </table>
				    <div><asp:Button id="btnAddVisit" runat="server" Text="Add" ValidationGroup="AddVisit" /></div>
            </fieldset>
            <br />
            <%--<asp:Button id="btnSave" runat="server" Text="Save" ValidationGroup="Save" />--%>
             <asp:HiddenField ID="OriginalValueChanged" runat="server" />
        
        </fieldset>
    </asp:Content>
