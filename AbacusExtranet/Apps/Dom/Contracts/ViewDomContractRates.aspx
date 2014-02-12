<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewDomContractRates.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.Contracts.ViewDomContractRates" %>

<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls"
    TagPrefix="cc1" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the rates for the selected Period.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" id="btnBack" value="Back" onclick="btnback_Click();" />
	    <br />
	    <br />
	    <label class="label" for="lblContract">Contract</label>
	    <asp:Label id="lblContract" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <br />
	    <label class="label" for="cboPeriodDates">Period</label>
	    <cc1:DropDownListEx ID="cboPeriodDates" runat="server" ></cc1:DropDownListEx>
	    <br />
	    <asp:Repeater id="rptContractRates" runat="server">
	        <HeaderTemplate>
			    <table class="listTable sortable" cellpadding="4" cellspacing="0" width="100%" summary="Lists rates for the contract for the selected period.">
				    <caption>Lists rates for the contract for the selected period.</caption>
				    <tr>
					    <th>Description</th>
					    <th>Abbreviation</th>
					    <th>Unit Cost</th>
				    </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Description")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Abbreviation")%>&nbsp;</td>
				    <td valign="top"><%#CType(DataBinder.Eval(Container.DataItem, "UnitCost"), Decimal).ToString("C")%>&nbsp;</td>
				</tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
        </asp:Repeater>
	</asp:Content>
