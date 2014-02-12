<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ucServiceUserMinutesCalc.ascx.vb" Inherits="Target.Abacus.Web.Apps.UserControls.ucServiceUserMinutesCalc" %>
<%@ Register Assembly="Target.Library.Web" Namespace="Target.Library.Web.Controls" TagPrefix="cc1" %>

<asp:HiddenField ID="hdnSettingID" runat="server" />
<asp:HiddenField ID="hdnSettingEditable" runat="server" />
<asp:HiddenField ID="hidDisplayMode" runat="server" Value="2" />

<fieldset id="fsSettings" style="padding:0.5em;" runat="server">  
    <legend><asp:Literal ID="litSettingName" text="Service User Minutes Calculation" runat="server" /></legend>  
    <div style="width : 100%;">
        <table style="table-layout:fixed;" cellpadding="2" cellspacing="0" width="100%">
            <thead>
	            <tr>
                    <th style="width:16%;text-align:left;">Mins From</th>
                    <th style="width:14%;text-align:left;">Mins To</th>
                    <th style="width:68%;text-align:left;">Calculation Method</th>
	            </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        <cc1:TextBoxEx ID="txtMinsFrom1" runat="server" LabelWidth="0em"
                        Required="false" Format="IntegerFormat"
                        ValidationGroup="Save" />        
                    </td>
                    <td>
                        <cc1:TextBoxEx ID="txtMinsTo1" runat="server" LabelWidth="0em"
                        Required="false" Format="IntegerFormat"
                        ValidationGroup="Save" />        
                    </td>
                    <td>
                        <cc1:DropDownListEx ID="cboCalcMethod1" runat="server" LabelWidth="0em"></cc1:DropDownListEx>
                    </td>
                </tr>
                <tr>
                    <td>
                        <cc1:TextBoxEx ID="txtMinsFrom2" runat="server" LabelWidth="0em"
                        Required="false" Format="IntegerFormat"
                        ValidationGroup="Save" />        
                    </td>
                    <td>
                        <cc1:TextBoxEx ID="txtMinsTo2" runat="server" LabelWidth="0em"
                        Required="false" Format="IntegerFormat"
                        ValidationGroup="Save" />        
                    </td>
                    <td>
                        <cc1:DropDownListEx ID="cboCalcMethod2" runat="server" LabelWidth="0em"></cc1:DropDownListEx>
                    </td>
                </tr>
                <tr>
                    <td>
                        <cc1:TextBoxEx ID="txtMinsFrom3" runat="server" LabelWidth="0em"
                        Required="false" Format="IntegerFormat"
                        ValidationGroup="Save" />        
                    </td>
                    <td>
                        <cc1:TextBoxEx ID="txtMinsTo3" runat="server" LabelWidth="0em"
                        Required="false" Format="IntegerFormat"
                        ValidationGroup="Save" />        
                    </td>
                    <td>
                        <cc1:DropDownListEx ID="cboCalcMethod3" runat="server" LabelWidth="0em"></cc1:DropDownListEx>
                    </td>
                </tr>
            </tbody>
        </table>
        <asp:CustomValidator id="validatorServiceUserMinutesCalc" runat="server" 
        Display="Dynamic" ErrorMessage="The entry fields are not correctly filled" 
        ClientValidationFunction="validatorServiceUserMinutesCalc_ClientValidate" 
        OnServerValidate="validatorServiceUserMinutesCalc_ServerValidate"
        ValidationGroup="Save"></asp:CustomValidator>   
    </div>
    <div class="clearer"></div>
</fieldset>
<br />
