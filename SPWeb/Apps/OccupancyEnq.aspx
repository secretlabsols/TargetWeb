<%@ Page Language="vb" AutoEventWireup="false" Codebehind="OccupancyEnq.aspx.vb" Inherits="Target.SP.Web.Apps.OccupancyEnq"%>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="OccupancyList" Src="UserControls/OccupancyEnqList.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below is the list of service users present in the specified property who are also receiving the specified service.
		You can use the filters below to better locate the service users you are interested in.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" value="Back" title="Navigates to the previous screen." style="float:left;" onclick="history.go(-1);" />
	    <br />
	    <br />
	    <label class="label" for="lblProperty">Property</label> 
	    <ASP:Label id="lblProperty" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblService">Service</label> 
	    <ASP:Label id="lblService" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <br />
	    <fieldset style="float:left; width:48em; margin-bottom:1em;" id="grpRegDetails" runat="server">
		    <legend>Filter</legend>
		    <div style="float:left;">
			    <cc1:TextBoxEx id="txtDateFrom" LabelText="Date From" LabelWidth="7em" LabelBold="True"
				    ReadOnlyContentCssClass="content" IsReadOnly="False" Format="1" runat="server"></cc1:TextBoxEx>
		    </div>
		    <div style="float:left;margin-left:2em;">
			    <cc1:TextBoxEx id="txtDateTo" LabelText="Date To" LabelWidth="6em" LabelBold="True"
				    ReadOnlyContentCssClass="content" IsReadOnly="False" Format="1" runat="server"></cc1:TextBoxEx>
		    </div>
		    <div class="clearer"></div>
		    <br />
		    <cc1:CheckBoxEx id="chkActive" labelID="lblActive" text="Active" checkBoxCssClass="content2" runat="server"></cc1:CheckBoxEx> 
		    <cc1:CheckBoxEx id="chkProvisional" labelID="lblProvisional" text="Provisional" checkBoxCssClass="content2" runat="server"></cc1:CheckBoxEx> 
		    <cc1:CheckBoxEx id="chkSuspended" labelID="lblSuspended" text="Suspended" checkBoxCssClass="content2" runat="server"></cc1:CheckBoxEx> 
		    <cc1:CheckBoxEx id="chkCancelled" labelID="lblCancelled" text="Cancelled" checkBoxCssClass="content2" runat="server"></cc1:CheckBoxEx> 
		    <cc1:CheckBoxEx id="chkDocumentary" labelID="lblDocumentary" text="Documentary" checkBoxCssClass="content2" runat="server"></cc1:CheckBoxEx> 
	    </fieldset>
	    <input id="btnFilter" type="button" value="Filter" title="Refreshes list using filters applied." style="float:left; margin: 0.5em 1em; width:5em;" onclick="Filter_Click();" />
	    <br /><br />
	    <uc1:OccupancyList id="OccupancyList" runat="server"></uc1:OccupancyList>
    </asp:Content>