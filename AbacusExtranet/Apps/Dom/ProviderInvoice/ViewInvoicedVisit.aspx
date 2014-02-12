<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewInvoicedVisit.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ProviderInvoice.ViewInvoicedVisit" %>
<%@ Register TagPrefix="uc2" TagName="pScheduleHeader" Src="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>
<%@ Reference Control="~/AbacusExtranet/Apps/UserControls/PaymentScheduleHeader.ascx" %>
	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		This screen displays the details of a invoiced visit.
	
        <script language="javascript" type="text/javascript">
// <![CDATA[

           

// ]]>
        </script>
</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
        <input type="button" id="btnBack" runat="server" value="Back" />
        <input type="button" id="btnView" runat="server" style="width:9em;" value="Visit Amendment" runat="server" />
	    <br />
	    <br />
	    <uc2:pScheduleHeader ID="pSchedules" runat="server"></uc2:pScheduleHeader>
	    <label  style="width:15em;" class="label" for="lblProvider">Provider</label>
	    <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	    <br />
		<label  style="width:15em;" class="label" for="lblContract">Contract</label>
	    <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
		<br />
		<label  style="width:15em;" class="label" for="lblOrderRef">Order Reference</label>
	    <asp:Label id="lblOrderRef" runat="server" CssClass="content"></asp:Label>
	    <br />
		<label style="width:15em;"  class="label" for="lblServiceUser">Service User</label>
	    <asp:Label id="lblServiceUser" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label  style="width:15em;" class="label" for="lblWeekending">Week Ending</label>
	    <asp:Label id="lblWeekending" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label  style="width:15em;" class="label" for="lblServiceType">Service Type</label>
	    <asp:Label id="lblServiceType" runat="server" CssClass="content"></asp:Label>
	    <asp:Label id="lblServiceTypeAmendment" runat="server" CssClass="Amendment" ></asp:Label>
	    <br />
        <label style="width:15em;"  class="label" for="lblVisitDate">Visit Date</label>
	    <asp:Label id="lblVisitDate" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label  style="width:15em;" class="label" for="lblStartTimeClaimed">Start Time Claimed</label>
	    <asp:Label id="lblStartTimeClaimed" runat="server" CssClass="content"></asp:Label>
	    <asp:Label id="lblStartTimeClaimedAmendment" runat="server" CssClass="Amendment" ></asp:Label>
	    <br />
        <label  style="width:15em;" class="label" for="lblDurationClaimed">Duration Claimed</label>
	    <asp:Label id="lblDurationClaimed" runat="server" CssClass="content"></asp:Label>
	    <asp:Label id="lblPreRoundedDurationClaimed" runat="server" CssClass="warningText"></asp:Label>
	    <asp:Label id="lblDurationClaimedAmendment" runat="server" CssClass="Amendment"></asp:Label>
	    <br />
	    <label  style="width:15em;" class="label" for="lblVisitCode">Visit Code</label>
	    <asp:Label id="lblVisitCode" runat="server" CssClass="content"></asp:Label>
	    <asp:Label id="lblVisitCodeAmendment" runat="server" CssClass="Amendment"></asp:Label>
	    <br />
        <label  style="width:15em;" class="label" for="lblNumberCarers">Number of Carers</label>
	    <asp:Label id="lblNumberCarers" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label  style="width:15em;" class="label" for="lblSecondaryVisit">Secondary Visit</label>
	    <asp:Label id="lblSecondaryVisit" runat="server" CssClass="content"></asp:Label>
	    <asp:Label id="lblSecondaryVisitAmendment" runat="server" CssClass="Amendment"></asp:Label>
	     <br />
	    <label  style="width:15em;" class="label" for="lblSecondaryVisitAutoSet" style="width:12em;">Secondary Visit Auto Set</label>
	    <asp:Label id="lblSecondaryVisitAutoSet" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label  style="width:15em;" class="label" for="lblManuallyAmended">Manually Amended</label>
	    <asp:Label id="lblManuallyAmended" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label  style="width:15em;" class="label" for="lblActualStartTime">Actual Start Time</label>
	    <asp:Label id="lblActualStartTime" runat="server" CssClass="content"></asp:Label>
	    <br />
        <label  style="width:15em;" class="label" for="lblActualDuration">Actual Duration</label>
	    <asp:Label id="lblActualDuration" runat="server" CssClass="content"></asp:Label>
	    <asp:Label id="lblActualDurationAmendment" runat="server" CssClass="Amendment"></asp:Label>
	   
	    <br /> 
	    <br />
	    
	    <asp:Repeater id="rptCarers" runat="server">
	        <HeaderTemplate>
			    <table class="listTable" id="tblCarers" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" summary="List of Carers.">
				    <caption>Care Workers.</caption>
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
        <br /> 
        <br /> 
        <div class="clearer"></div>     
    </asp:Content>