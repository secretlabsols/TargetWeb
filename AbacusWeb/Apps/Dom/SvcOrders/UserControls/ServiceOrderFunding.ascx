<%@ Control Language="vb" AutoEventWireup="false" EnableViewState="true" CodeBehind="ServiceOrderFunding.ascx.vb" Inherits="Target.Abacus.Web.Apps.Dom.SvcOrders.ServiceOrderFunding" %>
<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc6" TagName="InPlaceCareManager" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceCareManagerSelector.ascx" %>
<%@ Register TagPrefix="uc7" TagName="InPlaceTeam" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceTeamSelector.ascx" %>
<%@ Register TagPrefix="uc8" TagName="InPlaceClientGroup" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientGroupSelector.ascx" %>
<%@ Register TagPrefix="uc9" TagName="InPlaceClientSubGroup" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSubGroupSelector.ascx" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    
       <div style="padding-bottom:3px;">
            <asp:Label ID="InfoLabel" AssociatedControlID="chkUserGenerated" CssClass="warningText" runat="server" 
            Text="To edit data on this funding screen, you must use the ‘EDIT’ button at the bottom right of this screen" 
            ></asp:Label>
       </div>
        <asp:Label ID="lblfundingError" runat="server" CssClass="errorText"></asp:Label>
                                <div runat="server" id="fundingRevisionDropDown">
        
            <cc1:DropDownListEx ID="cbofundingEffectiveDate" runat="server"   LabelText="Effective Date" LabelWidth="10.5em"
					         ValidationGroup="dsoFunding"></cc1:DropDownListEx>
	        
        </div>

                    <script type="text/javascript">
            Sys.Application.add_load(BindEvents);
        </script>

            <input type="text" style="display:none;" id="hid_ButtonClicked" runat="server" />
            <input type="text" style="display:none;" id="hidFundingDSOID" runat="server" />
            <input type="text" style="display:none;" id="hidRevisionID" runat="server" />

                                                                                                                                                                                                                        <div runat="server" id="ServiceOrderFundingContainer">
        
            <div runat="server" id="fundingRevisionDatePicker">
                <cc1:TextBoxEx ID="fundingEffectiveDate" runat="server" LabelText="Effective Date" LabelWidth="11em"  
                       Required="true" RequiredValidatorErrMsg="Please enter an effective date" Format="DateFormatJquery"
			           ValidationGroup="fundingSave" AllowClear="true" Width="6.5em" ContaintedWithinAnUpdatePanel="true" />
            </div>
            <br />
            <asp:Label ID="Label1" AssociatedControlID="fundingClientGroup" runat="server" Text="Client Group" Width="10.5em"></asp:Label>
            <uc8:InPlaceClientGroup id="fundingClientGroup"  runat="server"></uc8:InPlaceClientGroup>
            <br />
            <asp:Label ID="Label4" AssociatedControlID="fundingClientSubGroup" runat="server" Text="Client Sub Group" Width="10.5em"></asp:Label>
            <uc9:InPlaceClientSubGroup id="fundingClientSubGroup"  runat="server"></uc9:InPlaceClientSubGroup>
            <br />
            <asp:Label ID="Label2" AssociatedControlID="fundingTeam" runat="server" Text="Team" Width="10.5em"></asp:Label>
            <uc7:InPlaceTeam id="fundingTeam"  runat="server"></uc7:InPlaceTeam>
            <br />
            <asp:Label ID="Label3" AssociatedControlID="fundingCareManager" runat="server" Text="Care Manager" Width="10.5em"></asp:Label>
            <uc6:InPlaceCareManager id="fundingCareManager" runat="server"></uc6:InPlaceCareManager>
            <br />
		    <input type="checkbox" id="chkUserGenerated" runat="server" />
            <asp:Label ID="lblUserGenerated" AssociatedControlID="chkUserGenerated" CssClass="warningText" runat="server" Text="" Width="25em"></asp:Label>
            <br /><br />
            <fieldset id="fldFunding" runat="server" >
                <div id="svcTypeMenuContainer" runat="server" style="float:left;width:16%;height:205px;overflow:auto;overflow-x: hidden;">
                    <span style="font-style: italic;">Planned Service on Order</span>
                    <div id="planned">
                        <ul id="plannedSTList"  style="margin-left:0; margin-top:0;" class="svcType" runat="server">
                        </ul>
                    </div>
                    <span style="font-style: italic;">Other Services by Provider</span>
                    <div id="otherServices">
                        <ul id="otherSTList" style="margin-left:0; margin-top:0;" class="svcType" runat="server">
                        </ul>
                    </div>
                </div>
                <div style="float:left;margin-left:2em;width:80%;height:205px;overflow:auto;">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <asp:RadioButton id="optApportion" Text="Apportion Actual Service Equally" runat="server"  GroupName="type" />
                            <br />
                            <asp:RadioButton id="optCallOff" Text="Call Off Planned Service in Order" runat="server"  GroupName="type" />
                            <br />
        
                            <asp:PlaceHolder ID="phFinanceCodes" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
        
                </div>
                <div class="clearer"></div>
            </fieldset>
        </div>
            <br />
                                                <div style="float:right;" >
            <fieldset id="fldButtons" runat="server" style=" padding:3px 3px 3px 3px">
                <asp:Button id="btnFundingNew" style="width:4.5em;" runat="server" Text="New" ValidationGroup="NewFunding" />
                <asp:Button id="btnFundingSave" runat="server" Text="Save" ValidationGroup="SaveFunding"  />
                <asp:Button id="btnFundingEdit" style="width:4.5em;" runat="server" Text="Edit"  ValidationGroup="EditFunding" />
                <asp:Button id="btnFundingDelete" style="width:4.5em;" runat="server" Text="Delete" OnClientClick="return confirmDelete();" ValidationGroup="EditFunding" />
                <%--<input type="button" id="btnFundingDelete" runat="server" value="Delete" onclick="btnDeleteFunding_Click();" />--%>
                <asp:Button id="btnFundingCancel" runat="server" Text="Cancel" ValidationGroup="CancelFunding"  />
            </fieldset>
        </div>
        
    </ContentTemplate>
</asp:UpdatePanel>

<div id="dsoProportions_dialog" style="display:none;">
    <input type="checkbox" id="chkSplitEqually" name="SplitEqually"  /> Spread the money equally accross all finance codes
    <br />
    <br />

    <table id="tblDSODocuments" cellspacing="0" cellpadding="2" summary="Service Order Documents" width="99%">
        <tbody id="tblProportionsBody">
            <%--Table rows are added Dynamically--%>

        </tbody>
        <tfoot>
        <tr>
          <td style="width:15%;"></td>  
          <td style="width:75%; text-align: right;font-weight:bold;">Total Percent Apportioned</td>
          <td style="padding-left:10px;width:10%;"><span id="TotalPercent">100%</span></td>
        </tr>
  </tfoot>
    </table>
 </div>
        
<script id="financeCodePercentagesTemplate" type="text/html">
    <tr  >
        <td style="width:15%;padding-bottom:10px;">${financeCodeText}</td>
	    <td style="width:75%;padding-bottom:10px;"><div id='slider_${rowIdentifier}'><span style="display:none;" id="defaultValue">${proportion}</span> </div></td>
        <td style="padding-left:10px;padding-bottom:10px; width:10%;"><span id='slider${rowIdentifier}para'>${proportion}%</span></td>
	</tr>
</script>

