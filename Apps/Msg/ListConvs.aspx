<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ListConvs.aspx.vb" Inherits="Target.Web.Apps.Msg.ListConvs" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
    Displayed below is a list of your conversations. You can use the filters below to
    restrict the list to better locate the conversations you are interested in.
</asp:Content>
<asp:Content ID="content" ContentPlaceHolderID="MPContent" runat="server">
    <fieldset style="width:60%;" >
        <legend>Filters</legend>
        <div style="margin-bottom: 5px;">
            <label for="cboLabel" style="width: 6.5em; float: left;">
                Label</label>
            <select id="cboLabel" style="width: 27.75em;" runat="server">
                <option value=""></option>
            </select>
        </div>
        <div style="margin-bottom: 5px;">
            <label for="cboStatus" style="width: 6.1em;">
                Status</label>
            <select id="cboStatus" style="width: 27.75em;" runat="server">
                <option value=""></option>
                <option value="0">Unread</option>
                <option value="1">Read</option>
            </select>
        </div>
        <div style="margin-bottom: 5px;">
            <asp:Panel ID="pnlStartedBy" runat="server">
                <label for="cboStartedBy" style="width: 6.5em;">
                    Started By</label><select id="cboStartedBy" style="width: 27.75em;" runat="server"><option
                        value=""></option>
                    </select>
                <br />
            </asp:Panel>
        </div>
        <div >
            <asp:Panel ID="pnlInvolving" runat="server">
                <label for="cboInvolving" style="width: 6.5em;">
                    Involving</label><select id="cboInvolving" style="width: 27.75em;" runat="server"><option
                        value=""></option>
                    </select>
            </asp:Panel>
        </div>
        <div id="customDate" style="float:left;" >
            <cc1:TextBoxEx ID="dteLastSentFrom" runat="server" LabelText="Last message sent between"
                Format="DateFormatJquery" LabelWidth="16em" Width="8em" AllowClear="true" >
            </cc1:TextBoxEx>
        </div>
        <div id="customDate" style="float:left; ">
            <cc1:TextBoxEx ID="dteLastSentTo" runat="server" LabelText="and" Format="DateFormatJquery"
                LabelWidth="3.5em" AllowClear="true" Width="8em">
            </cc1:TextBoxEx>
        </div>
        <div style="float: left;">
        </div>
        <div class="clearer">
        </div>
        <div id="customDate" style="float: left;">
            <cc1:TextBoxEx ID="dteStartedFrom" runat="server" LabelText="Conversation started between"
                Format="DateFormatJquery" LabelWidth="16em" Width="8em" AllowClear="true">
            </cc1:TextBoxEx>
        </div>
        <div id="customDate" style="float: left;">
            <cc1:TextBoxEx ID="dteStartedTo" runat="server" LabelText="and" Format="DateFormatJquery"
                LabelWidth="3.5em" Width="8em" AllowClear="true">
            </cc1:TextBoxEx>
        </div>
        <div class="clearer">
        </div>
    </fieldset>
    <input type="button" id="btnFilter" value="Filter" style="float: left; margin: 0.5em 0.25em;"
        title="Click here to filter the conversation List" onclick="btnFilter_OnClick();" />
    <input type="button" id="btnReset" value="Reset" style="float: left; margin: 0.5em 0.25em;"
        title="Click here to reset filters" onclick="btnReset_OnClick();" />
    <input type="button" id="btnNewConv" runat="server" value="New Conversation" style="float: left;
        margin: 0.5em 0.25em; width: 9.5em;" title="Click here to create a new conversation"
        onclick="btnNew_OnClick();" />
    <table class="listTable sortable" id="Conversations_Table" cellpadding="2" cellspacing="0"
        width="100%" summary="Displays your conversations.">
        <caption style="margin-top: 2em;">
            Displays your conversations.</caption>
        <thead>
            <tr>
                <th style="width: 1em;">
                </th>
                <th>
                    Subject
                </th>
                <th style="width: 18em;">
                    Conversation Started
                </th>
                <th style="width: 18em;">
                    Last Message Sent
                </th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
        </tbody>
    </table>
    <div id="Conversations_PagingLinks" style="float: left;">
    </div>
    <div class="clearer">
    </div>
</asp:Content>
