<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CareWorkerSelectorManualInvoice.aspx.vb"
    Inherits="Target.Abacus.Extranet.Apps.InPlaceSelectors.CareWorkerSelectorManualInvoice"
    MasterPageFile="~/Popup.master" %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
    <table border="0" style="width: 98%;">
        <tr>
            <td valign="top">
                <table border="0" style="width: 100%;">
                    <tr>
                        <td>
                        <asp:RadioButton ID="rdbtnSelectExisting" GroupName="careWorker" 
                        Font-Bold="true" runat="server" Text="Select an existing Care Worker" Checked="true" />
                            <blockquote>
                                <div style="width: 100%; height: 180px; overflow: auto;">
                                <table class="listTable" id="tblCareWorkers" style="table-layout: fixed;" cellpadding="2"
                                    cellspacing="0" width="100%" summary="List of available Care workers.">
                                    <thead>
                                        <tr>
                                            <th style="width: 1.5em;">
                                            </th>
                                            <th id="thRef" style="width: 20%">
                                                Reference
                                            </th>
                                            <th id="thName">
                                                Name
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
                                <div id="CareWorker_PagingLinks" style="float: left;">
                                </div>
                            </div>
                            </blockquote>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButton ID="rdbtnNew" runat="server" GroupName="careWorker" 
                            Font-Bold="true"  Text="Enter New Care Worker"  onclick="RdbCheckedChanged();" />
                    
                            <blockquote>
                            <div>
                                <cc1:TextBoxEx ID="txtReference" runat="server" LabelText="Reference" LabelWidth="8.75em"
                                    Width="10em" MaxLength="50" Required="True" RequiredValidatorErrMsg="Please enter the reference number.">
                                </cc1:TextBoxEx>
                            </div>
                            <br />
                            <div>                                
                                <cc1:TextBoxEx ID="txtName" runat="server" LabelText="Name" LabelWidth="8.75em" Width="20em"
                                    MaxLength="50" Required="True" RequiredValidatorErrMsg="Please enter the name.">
                                </cc1:TextBoxEx>
                             </div>
                            </blockquote>
                            
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:RadioButton ID="rdbtnNotSpecified" runat="server" GroupName="careWorker" 
                            Font-Bold="true" Text="Care Worker not specified" onclick="RdbCheckedChanged();" />
                        </td>
                    </tr>
                </table>
            </td>
            <td align="left" valign="top">
                <br />
                <br />
                        <input type="button" id="btnSelectContract" value="OK" style="width: 5em;" onclick="btnSelectCareWorker_Click();" />
                <br />
                        <input type="button" id="btnCancel" value="Cancel" style="width: 5em;" onclick="btnCancel_Click();" />
                
            </td>
        </tr>
    </table>

</asp:Content>
