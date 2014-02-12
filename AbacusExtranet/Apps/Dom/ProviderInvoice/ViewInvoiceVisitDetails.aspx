<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewInvoiceVisitDetails.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.ViewInvoiceVisitDetails" %>
<%@ Register TagPrefix="uc1" TagName="pScheduleHeader" 
Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the visit details for the selected visit.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	<div>
        <%--<input type="button" id="btnBack" runat="server" value="Back"  />--%>
        <input type="button" id="btnBack" runat="server" value="Back" style="width:5em;" />
        <input type="button" id="btnView" runat="server" style="width:9em;" value="Visit Amendment" runat="server" />
	</div>
	<div style="margin-top:5px;">
	    <uc1:pScheduleHeader ID="pSchedule" runat="server"></uc1:pScheduleHeader>
	    <label class="label" for="lblProvider" style="width:11.5em">Provider</label>
	    <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	    <br />
		<label class="label" for="lblContract" style="width:11.5em">Contract</label>
	    <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
		<br />
        <label class="label" for="lblServiceUser" style="width:11.5em">Service User</label>
	    <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblOrderReference" style="width:11.5em">Order Reference</label>
	    <asp:Label id="lblOrderReference" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblWeekending" style="width:11.5em">Week Ending</label>
	    <asp:Label id="lblWeekending" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblServiceType" style="width:11.5em">Service Type</label>
	    <asp:Label id="lblServiceType" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblVisitDate" style="width:11.5em">Visit Date</label>
	    <asp:Label id="lblVisitDate" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblStartTimeClaimed" style="width:11.5em">Start Time Claimed</label>
	    <asp:Label id="lblStartTimeClaimed" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblDurationClaimed" style="width:11.5em">Duration Claimed</label>
	    <asp:Label id="lblDurationClaimed" runat="server" CssClass="content"></asp:Label>
	    <asp:Label id="lblPreRoundedDurationClaimed" runat="server" CssClass="warningText"></asp:Label>
	    <br />
	    <label class="label" for="lblVisitCode" style="width:11.5em">Visit Code</label>
	    <asp:Label id="lblVisitCode" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblNumberOfCarers" style="width:11.5em">Number of Carers</label>
	    <asp:Label id="lblNumberOfCarers" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblSecondaryVisit" style="width:11.5em">Secondary Visit</label>
	    <asp:Label id="lblSecondaryVisit" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblManuallyAmended" style="width:11.5em">Manually Amended</label>
	    <asp:Label id="lblManuallyAmended" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblActualStartTime" style="width:11.5em">Actual Start Time</label>
	    <asp:Label id="lblActualStartTime" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label class="label" for="lblActualDuration" style="width:11.5em">Actual Duration</label>
	    <asp:Label id="lblActualDuration" runat="server" CssClass="content"></asp:Label>
	</div>   
	<div style="margin-top:5px;">
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
     </div>
    </asp:Content>


