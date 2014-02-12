<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Edit.aspx.vb" Inherits="Target.Abacus.Web.Apps.FinanceCodeMatrix.Enquiry.Edit" EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceClientGroup" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientGroupSelector.ascx" %>
<%@ Register TagPrefix="uc3" TagName="InPlaceClientSubGroup" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSubGroupSelector.ascx" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceTeam" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceTeamSelector.ascx" %>
<%@ Register TagPrefix="uc5" TagName="InPlaceProvider" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceEstablishmentSelector.ascx" %>
<%@ Register TagPrefix="uc6" TagName="InPlaceServiceType" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceServiceTypeSelector.ascx" %>
<%@ Register TagPrefix="uc7" TagName="InPlaceContract" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceDomContractSelector.ascx" %>
<%@ Register TagPrefix="uc9" TagName="InPlaceFinanceCode" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceFinanceCodeSelector.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to maintain the different finance code matrixs.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" OnSaveClientClick="return btnSave_Click();"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewstate="false">
        <fieldset>
            <legend>Finance Code Matrix Elements</legend>
                <div style="float:left;">
                    <cc1:TextBoxEx ID="dteEffectiveDateFrom" runat="server" LabelText="Effective From" LabelWidth="11em" Format="DateFormatJquery"  
                    Width="6em" AllowClear="true" required="true" RequiredValidatorErrMsg="Please enter an Effective From date" ValidationGroup="Save"/>                   
				</div>
                 <div class="clearer"></div>
                 <br />
				<div style="float:left;">
                    <cc1:TextBoxEx ID="dteEffectiveDateTo" runat="server" LabelText="Effective To" LabelWidth="11em" Format="DateFormatJquery" 
                    Width="6em" AllowClear="true" />
				</div> 
                 <div class="clearer"></div> 
             <br />
             
             <div class="clearer"></div> 
		     <div style="float:left;">            
                <input id="optIncome" runat="server" type="radio" name="type" style="float:left;" />
                <label id="lblIncome" runat="server" class="label" style="float:left" for="optIncome" >Income</label>
                <input id="optExpenditure" runat="server" type="radio" name="type" style="float:left; margin-left:2em;" />
                <label class="label" for="optExpenditure" style="float:left">Expenditure</label>
             </div>
             <br /><br />
             <div class="clearer"></div> 
             <div style="float:left;">

                     <asp:Label id="lblFinanceCode" AssociatedControlID="txtFinanceCode" runat="server" Text="Finance Code" Width="10.5em"></asp:Label>
				    <uc9:InPlaceFinanceCode id="txtFinanceCode"  runat="server"></uc9:InPlaceFinanceCode>
                    
             </div> 
             <div style="float:left;">
                <input type="button" id="btnNewFinanceCode" class="newButton" title="Create a new finance code." runat="server" onclick="btnNewFinanceCode_Click();" />
             </div>   
       </fieldset >
             <fieldset id="fsFactors" style="padding:0.5em;" runat="server" EnableViewstate="false">
			<legend>Factors from which finance codes are derived</legend>
            <asp:Panel ID="pnlClientGroup" runat="server">
                <asp:Label AssociatedControlID="txtClientGroup" runat="server" Text="Client Group" Width="13.5em"></asp:Label>
	            <uc2:InPlaceClientGroup id="txtClientGroup" runat="server"></uc2:InPlaceClientGroup>
                <br />
            </asp:Panel>
		    <asp:Panel id="pnlClientSubGroup" runat="Server">
                <asp:Label AssociatedControlID="txtClientSubGroup" runat="server" Text="Client Sub Group" Width="13.5em"></asp:Label>
	            <uc3:InPlaceClientSubGroup id="txtClientSubGroup" runat="server"></uc3:InPlaceClientSubGroup>
                <br />
		    </asp:Panel>		
		    <asp:Panel ID="pnlAgeRange" runat="server">
                <cc1:TextBoxEx ID="txtAgeRangeFrom" runat="server" LabelText="Age Range From" LabelWidth="14em" 
                    MaxLength="2" Width="6em" MaximumValue="100" MinimumValue="1" Format="IntegerFormat"></cc1:TextBoxEx>
                <br />
                <cc1:TextBoxEx ID="txtAgeRangeTo" runat="server" LabelText="Age Range To" LabelWidth="14em" 
                    MaxLength="3" Width="6em"  MaximumValue="999" MinimumValue="1" Format="IntegerFormat"></cc1:TextBoxEx>
		        <br />
		    </asp:Panel>
		    <asp:Panel ID="pnlTeam" runat="server">
                <asp:Label AssociatedControlID="txtTeam" runat="server" Text="Team" Width="13.5em"></asp:Label>
	            <uc4:InPlaceTeam id="txtTeam" runat="server"></uc4:InPlaceTeam>
                <br />
            </asp:Panel>
		    <asp:Panel ID="pnlProvider" runat="server">
                <asp:Label AssociatedControlID="txtProvider" runat="server" Text="Provider" Width="13.5em"></asp:Label>
	            <uc5:InPlaceProvider id="txtProvider" runat="server" Mode="DomProviders"></uc5:InPlaceProvider>
                <br />
            </asp:Panel>
            <asp:Panel ID="pnlPrivateOrLA" runat="server">
                 <input id="optPrivate" runat="server" type="radio" name="providerType" style="float:left;" />
                 <label class="label" style="float:left" for="optPrivate" >Private</label>
                 <input id="optLA" runat="server" type="radio" name="providerType" style="float:left; margin-left:2em;" />
                 <label class="label" for="optLA" style="float:left" >Local Authority</label>
                 <input id="optBoth" runat="server" type="radio" name="providerType" style="float:left; margin-left:2em;" />
                 <label class="label" for="optBoth" style="float:left" >None Selected</label>
                <br />
                <br />
            </asp:Panel>
            <div class="clearer"></div> 
            <asp:Panel ID="pnlServiceType" runat="server">
                <asp:Label AssociatedControlID="txtServiceType" runat="server" Text="Service Type" Width="13.5em"></asp:Label>
	            <uc6:InPlaceServiceType id="txtServiceType" runat="server"></uc6:InPlaceServiceType>
                <br />
		    </asp:Panel>
		    <asp:Panel ID="pnlContractNo" runat="server">
                <asp:Label AssociatedControlID="txtContract" runat="server" Text="Contract" Width="13.5em"></asp:Label>
	            <uc7:InPlaceContract id="txtContract" runat="server"></uc7:InPlaceContract>
                <br />
		    </asp:Panel>
		    <asp:Panel ID="pnlServiceOrderRef" runat="server">
                <cc1:TextBoxEx ID="txtServiceOrderRef"  runat="server"  LabelText="Service Order Ref" LabelWidth="14em" MaxLength="10" 
                    Width="10em"></cc1:TextBoxEx>
		        <br />
		    </asp:Panel>
       </fieldset>
    </fieldset>
    <br />
</asp:Content>