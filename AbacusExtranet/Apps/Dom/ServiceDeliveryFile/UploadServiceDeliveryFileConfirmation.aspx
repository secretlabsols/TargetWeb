<%@ Page Language="vb" AutoEventWireup="false" Codebehind="UploadServiceDeliveryFileConfirmation.aspx.vb" Inherits="Target.Abacus.Extranet.Apps.Dom.ServiceDeliveryFile.UploadServiceDeliveryFileConfirmation" %>

<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls"
    TagPrefix="cc1" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Please review the details of the uploaded file below. You must click "Finish" to submit the file for processing.
	</asp:Content>
	<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    </asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <label class="label2" for="lblReference">File Reference</label>
	    <asp:Label id="lblFileRef" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblReference">File Description</label>
	    <asp:Label id="lblFileDesc" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label2" for="lblReference">File Created Date</label>
	    <asp:Label id="lblFileCreatedDate" runat="server" CssClass="content"></asp:Label>
	    <br />
        <asp:Repeater id="rptProviderContracts" runat="server">
	        <HeaderTemplate>
			    <table class="listTable sortable" cellpadding="4" cellspacing="0" width="100%" summary="Lists Contracts for the Provider contained in this file.">
				    <caption>List of Provider/Contracts contained within the interface file.</caption>
				    <tr>
					    <th>Provider Name</th>
					    <th>Provider Ref.</th>
					    <th>Contract No.</th>
					    <th>Contract Title</th>
				    </tr>
		    </HeaderTemplate>
		    <ItemTemplate>
			    <tr>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Name")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Reference")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Number")%>&nbsp;</td>
				    <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Title")%>&nbsp;</td>
				</tr>
		    </ItemTemplate>
		    <FooterTemplate>
			    </table>
		    </FooterTemplate>
        </asp:Repeater>
        <br />
                
        <fieldset id="grpConfirmation" runat="server" style="float:none;width:70%;">
            <legend>Confirmation Needed</legend>
            <asp:Label runat="server" id="lblConfirmationText"></asp:Label>
            <br />
            <cc1:CheckBoxEx ID="chkConfirmation"  text="I understand and accept this action" runat="server"></cc1:CheckBoxEx>
        </fieldset>
        <br /><br />
        
        <input type="button" id="btnBack" value="Back" onclick="history.go(-1);" />
	    <input type="button" id="btnNext" value="Finish" runat="server" />
	    <br />
        
    </asp:Content>
