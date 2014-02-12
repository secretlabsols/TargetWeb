<%@ Page Language="vb" AutoEventWireup="false" Codebehind="PrintRegister.aspx.vb" Inherits="Target.Abacus.Web.Apps.Actuals.DayCare.PrintRegister" MasterPageFile="~/Popup.master" %>    
  <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
  <style type="text/css">
	table.a {
		border: solid #66CC99;
		border-width: 0px 1px 1px 0px;
		width: 100%;
	}
	th.a, td.a 
	{
		border: solid #66CC99;
		border-width: 1px 0px 0px 1px;
		padding: 4px;
	}
	th.a {
		background-color: #6487db;
		color: #FFFFFF;
		text-align:left
	}
	tr.alt td {
		background-color: #EEEEEE;
	}
	print thead { display:table-header-group; }
</style>
	    <div>
		    <input type="button" id="btnClose" value="Close" style="float:right" title="Click here to close this list." onclick="window.close();" />
		    <input type="button" id="btnPrint" value="Print" style="float:right" title="Click here to print this list." onclick="window.print();" />
		    <div class="clearer"></div>
		    <table id="headings">
		        <tr><td><strong><big>Attendance Register Printout</big></strong></td></tr>
		        <tr><td>Provider: &nbsp<input id="txtProvider" type="text" style="border-width: 0px; background-color: rgb(255, 255, 255); width: 20em;"  readonly="readonly" value="" name="txtProvider"/></td></tr>
		        <tr><td>Contract: &nbsp<input id="txtContract" type="text" style="border-width: 0px; background-color: rgb(255, 255, 255); width: 20em;"  readonly="readonly" value="" name="txtContract"/></td></tr>
		        <tr><td>Day: &nbsp<input id="txtDay" type="text" style="border-width: 0px; background-color: rgb(255, 255, 255); width: 14em;"  readonly="readonly" value="" name="txtDay"/></td></tr>
		        <tr><td>WeekEnding: &nbsp<input id="txtWeekEnding" type="text" style="border-width: 0px; background-color: rgb(255, 255, 255); width: 7em;"  readonly="readonly" value="" name="txtWeekEnding"/></td></tr>
            </table>
	        <hr/>
	    </div>
	    <div id="register"></div>
    </asp:Content>
