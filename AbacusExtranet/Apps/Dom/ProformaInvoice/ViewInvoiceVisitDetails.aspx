<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewInvoiceVisitDetails.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProformaInvoice.ViewInvoiceVisitDetails" %>
<%@ Register TagPrefix="uc1" TagName="BreadCrumb" Src="~/AbacusExtranet/Apps/Dom/ProformaInvoice/InvoiceBatchBreadcrumb.ascx" %>
<%@ Register TagPrefix="uc2" TagName="pSchedules" Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>
<%@ Reference Control="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the visit details for the selected visit.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" id="btnBack" runat="server" value="Back" style="width:5em;" />
	    <br /><br />
	    <%--<uc1:BreadCrumb id=breadCrumb runat="server"></uc1:BreadCrumb>--%>

	    <uc2:pSchedules id="pScheduleHeader" runat="server"></uc2:pSchedules>
	    <div style="float:left;margin-top:2px;">
	        <label style="width:15em;"  class="label" for="lblProvider">Provider</label>
	        <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
		    <label  style="width:15em;" class="label" for="lblContract">Contract</label>
	        <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
		</div>
		<div class="clearer"></div>
		<div style="float:left;margin-top:2px;">
            <label  style="width:15em;" class="label" for="lblServiceUser">Service User</label>
	        <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
	        <label style="width:15em;"  class="label" for="lblOrderReference">Order Reference</label>
	        <asp:Label id="lblOrderReference" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
            <label  style="width:15em;" class="label" for="lblWeekending">Week Ending</label>
	        <asp:Label id="lblWeekending" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
	        <label  style="width:15em;" class="label" for="lblServiceType">Service Type</label>
	        <asp:Label id="lblServiceType" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
            <label  style="width:15em;" class="label" for="lblVisitDate">Visit Date</label>
	        <asp:Label id="lblVisitDate" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
            <label  style="width:15em;" class="label" for="lblStartTimeClaimed">Start Time Claimed</label>
	        <asp:Label id="lblStartTimeClaimed" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div >
	        <div style=float:left;margin-top:2px;">
                <label style="width:15em;"  class="label" for="lblDurationClaimed">Duration Claimed</label>
	            <asp:Label id="lblDurationClaimed" runat="server" CssClass="content"></asp:Label>
	            <asp:Label id="lblPreRoundedDurationClaimed" runat="server" CssClass="warningText"></asp:Label>
	        </div>
	        <div style=float:left;margin-left:3px;margin-top:2px;">
	            <asp:image id="imgIgnorerounding" runat="server"></asp:image>
	        </div>
	        <div class="clearer"></div>
	    </div>
	    <div class="clearer"></div>
	    <div  style="float:left;margin-top:2px;">
	        <label  style="width:15em;" class="label" for="lblVisitCode">Visit Code</label>
	        <asp:Label id="lblVisitCode" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
	        <label  style="width:15em;" class="label" for="lblNumberOfCarers">Number of Carers</label>
	        <asp:Label id="lblNumberOfCarers" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
	        <label  style="width:15em;" class="label" for="lblSecondaryVisit">Secondary Visit</label>
	        <asp:Label id="lblSecondaryVisit" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
	        <label style="width:15em;" class="label" for="lblSecondaryVisitAutoSet">Secondary Visit Auto Set</label>
	        <asp:Label id="lblSecondaryVisitAutoSet" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
	        <label  style="width:15em;" class="label" for="lblManuallyAmended">Manually Amended</label>
	        <asp:Label id="lblManuallyAmended" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
            <label  style="width:15em;" class="label" for="lblActualStartTime">Actual Start Time</label>
	        <asp:Label id="lblActualStartTime" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    <div style="float:left;margin-top:2px;">
            <label  style="width:15em;" class="label" for="lblActualDuration">Actual Duration</label>
	        <asp:Label id="lblActualDuration" runat="server" CssClass="content"></asp:Label>
	    </div>
	    <div class="clearer"></div>
	    
	    <br />  
	    
	    <asp:Repeater id="rptCarers" runat="server">
	        <HeaderTemplate>
			    <table class="listTable" id="tblCarers" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" summary="List of Carers.">
				    <caption>List of Carers.</caption>
				    <tr>
		                <th style="width:15em;">Reference</th>
		                <th style="width:35em;">Name</th>
	                </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td valign="top" ><%#DataBinder.Eval(Container.DataItem, "Reference")%>&nbsp;</td>
				    <td valign="top" ><%#DataBinder.Eval(Container.DataItem, "Name")%>&nbsp;</td>
				</tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
        </asp:Repeater>           
        <div class="clearer"></div>
    </asp:Content>

