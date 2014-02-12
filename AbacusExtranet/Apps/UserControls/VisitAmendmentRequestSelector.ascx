<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="VisitAmendmentRequestSelector.ascx.vb"
    Inherits="Target.Abacus.Extranet.Apps.UserControls.VisitAmendmentRequestSelector"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="uc5" TagName="ReportsButton" Src="~/Library/UserControls/ReportsButton.ascx" %>
<table class="listTable" id="tblAmendments" cellpadding="2" cellspacing="0" width="100%"
    summary="List of Visit Amendments.">
    <caption>
        List of visit amendment requests.</caption>
    <thead>
        <tr>
            <th style="width: 1.5em;">
            </th>
            <th id="thServiceUser">
                Service User
            </th>
            <th id="thServiceUserReference">
                S/U Reference
            </th>
            <th>
                Visit Date
            </th>
            <th>
                Start Time
            </th>
            <th>
                Status
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
            <td>
            </td>
            <td>
            </td>
        </tr>
    </tbody>
</table>
    <div id="Amendments_PagingLinks" style="float: left;">
    </div>

    <input type="button" id="btnView" runat="server" style="float: right; width: 11.6em; margin-bottom:5px;"
        value="View/Amend Visit" onclick="btnView_Click();" />
    <div style="float: right; margin-right:4px;">
        <uc5:ReportsButton runat="server" ID="rptPrint" Buttonwidth="4em" ButtonHeight="22px;" >
        </uc5:ReportsButton>
    </div>
    

<div class="clearer">
</div>
