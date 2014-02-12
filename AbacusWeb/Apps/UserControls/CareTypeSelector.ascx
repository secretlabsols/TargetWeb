<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CareTypeSelector.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.CareTypeSelector" %>
<div id="divDetailContainer" style="display:none;">
    <div id="divDetailContent">
        <div id="divRes" runat="server">
            <input id="optRes" runat="server" type="radio" name="type" value="1" style="margin-left:1em;" />
            <label class="label" for="optRes" >Residential</label>
            <br />
        </div>
        <div id="divNonRes" runat="server">
            <input id="optNonRes" runat="server" type="radio" name="type" value="2" style="margin-left:1em;" />
            <label class="label" for="optNonRes" >Non-Residential</label>
            <br />
        </div>
        <div id="divDP" runat="server">
            <input id="optDP" runat="server" type="radio" name="type" value="3" style="margin-left:1em;" />
            <label class="label" for="optDP" >Direct Payment</label>
            <br />
        </div>
    </div>
</div>