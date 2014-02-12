<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="RateOrdering.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Dom.RateOrdering"
    EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %> 

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to order rate categories.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
<style type="text/css">
  .DragHandleClass
 {
 width: 21px;
 height: 24px;
 background-image:url(../../../../images/hmove.jpg);
 }
 
.OrderedList li  
{  
list-style:none;
}  
</style>
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewstate="false">
		<cc1:TextBoxEx ID="txtRateFramework" runat="server" LabelText="Rate Framework" LabelWidth="14em" Width="20em" IsReadOnly="true"></cc1:TextBoxEx>
		<br /><br />
        <fieldset>
			<legend>Order of Rate Categories - To re-order drag and drop the items to the desired position in the list below</legend>
	         <div class="OrderedList" id="OrderedList">  
	         <ajaxToolkit:ReorderList ID="reOrderListRateCategories" runat="server"            
	            AllowReorder="True"
	            LayoutType="Table"            
	            OnItemReorder="OrderListRateCategories_ItemReorder"
	            PostBackOnReorder="true"
	            DataKeyField="ID"
	            SortOrderField="SortOrder"> 	
	            <DragHandleTemplate>
	            <% if reOrderListRateCategories.Enabled then %>
                    <div class="DragHandleClass"
                    onmousedown="this.style.cursor='url(../../../../images/closedhand.cur)'" 
                    onmouseup="this.style.cursor='url(../../../../images/openhand.cur)'"
                    onmousemove="this.style.cursor='url(../../../../images/openhand.cur)'">
                    </div>
                <% else %>
                <div class="DragHandleClass"></div>
                <% end if %>
                </DragHandleTemplate>                
	                <ItemTemplate>
                        <span>
                                <asp:label id="Description" runat="server" text='<%# Eval("Description") %>' />
                        </span>
                    </ItemTemplate>      
	          </ajaxToolkit:ReorderList>
	          </div>
        </fieldset>
    </fieldset>
    <br />
</asp:Content>