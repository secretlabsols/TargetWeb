<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Edit.aspx.vb" Inherits="Target.Abacus.Web.Apps.Actuals.DayCare.Edit"
    EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain registers.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
<style type="text/css">
	table.a {
		border: solid #66CC99;
		border-width: 1px 1px 0px 0px;
		width: 100%;
	}
	td.a, td.b {
		border: solid #66CC99;
		border-width: 0px 0px 1px 1px;
		padding: 4px;
		vertical-align:top;
		background-color: #FFFFFF;
	}
	td.b {
	    background-color: #fffea6;
	}
	th.a {
		background-color: #6487db;
		color: #FFFFFF;
		text-align:left;
		border: solid #66CC99;
		border-width: 0px 0px 1px 1px;
		padding: 4px;
	}
	tr.alt td {
		background-color: #EEEEEE;
	}
	tbody.a {
		height: 260px;
		overflow-y: auto;
		overflow-x: hidden;
	}
</style>
<!--[if IE]>
	<style type="text/css">
        div.a {
        	position: relative;
        	height: 260px;
            width: 100%;
            overflow-y: scroll;
            overflow-x: hidden;
			border: solid #66CC99;
			border-width: 0px 0px 1px 0px;
			     
        }  
		table.a {
			border-width: 1px 1px 0px 0px;
			text-align: "center";
		}      
    	thead.a tr {
        	position: absolute;
        	top: expression(this.offsetParent.scrollTop);
        }      
		tbody.a {
			height: auto;
		}
        table.a tbody tr:first-child td {
        	padding: 49px 0px 0px 4px;
        } 
    </style>
<![endif]-->

<uc1:StdButtons id="stdButtons1" runat="server" OnSaveClientClick="return Save_Validation();" ></uc1:StdButtons>
<div class="clearer"></div>
<fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewstate="false">
<legend><% If regDay <> "" Then response.write("Day:" & regDay)%><%response.write("" & "&nbsp" & "&nbsp" & " WeekEnding:" & WeekEnding & "&nbsp" & "&nbsp" & " Provider:" & EstablishmentName & "&nbsp" & "&nbsp" &  " Contract:" & DomContractTitle) %></legend>
<input type="hidden" value=<%response.write("'" & EstablishmentName & "'")%> id="hidProvider" />
<input type="hidden" value=<%response.write("'" & DomContractTitle & "'")%> id="hidContract"/>
<input type="hidden" value=<%response.write("'" & regDay & "'")%> id="hidDay"/>
<input type="hidden" value=<%response.write("'" & WeekEnding & "'")%> id="hidWeekEnding"/>
<div class="a" id="dvRegister">	
	<table class="a" cellspacing="0" cellpadding="0" summary="List of Service Users in this Register." width="100%" id="tbRegister">
    <thead class="a">
            <asp:PlaceHolder ID="phRegisterHeading" runat="server"></asp:PlaceHolder>
    </thead>
            <tbody class="a">
				<asp:PlaceHolder ID="phRegisterWeek" runat="server"></asp:PlaceHolder>
            </tbody>
    <tfoot class="a">
         <asp:PlaceHolder ID="phRegisterFooting" runat="server"></asp:PlaceHolder>   
    </tfoot>
    </table>
</div>
<br />
<input type="button" id="btnAddServiceUsers" runat="server" style="float:right;width:10em;" value="Add Service User" />
    <input type="button" id="btnUncheck" runat="server" style="float:right;width:7em;" value="UnCheck All" />
    <input type="button" id="btnSubmit" runat="server" style="float:right;width:5em;" value="Submit" onclick="btnSubmit_Click('ctl00_MPContent_btnSubmit');"/>
    <input type="button" id="btnUnSubmit" runat="server" style="float:right;width:6em;" value="UnSubmit" onclick="btnSubmit_Click('ctl00_MPContent_btnUnSubmit');"/>
    <input type="button" id="btnAmend" runat="server" style="float:right;width:5em;" value="Amend" onclick="btnSubmit_Click('ctl00_MPContent_btnAmend');"/>
    <input type="text" style="display:none;" id="hidID" runat="server" />
</fieldset>
<div class="clearer"></div>
    <input type="button" id="btnPrintReport" runat="server" style="float:right;width:8em;" value="Print Report" onclick="btnPrint_Click('ctl00_MPContent_btnPrintReport');"/>
    <input type="button" id="btnPrintRegister" runat="server" style="float:right;width:8em;" value="Print Register" onclick="btnPrint_Click('ctl00_MPContent_btnPrintRegister');"/>
    <br />
<div id="divServiceOutcomesDialogContentContainer" style="display:none;">
    <div id="divDatesDialogContent">
        <!-- hidden elements used in copy dialog -->
        Please select a service outcome.
        <br /><br />
        <cc1:DropDownListEx ID="cboServiceOutcomes" runat="server" LabelText="Service Outcome" LabelWidth="10em"
			Required="true" RequiredValidatorErrMsg="Please select a service outcome" ValidationGroup="ServiceOutcomes"></cc1:DropDownListEx>
        <br /><br />
    </div>
</div>
</asp:Content>
