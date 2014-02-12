<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DSOBasicDetails.ascx.vb"
    Inherits="Target.Web.Library.UserControls.DSOBasicDetails" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<fieldset id="fdlDSODetails" runat="server">
    <legend>
        <cc1:TextBoxEx ID="dsoOrderRef" runat="server" LabelText="Order Ref:" LabelWidth="6em"
            IsReadOnly="true" ReadOnlyContentCssClass="suHeaderReadOnlyCss">
        </cc1:TextBoxEx>
    </legend>
    <table style="width:100%">
        <tr>
            <td style="width:40%">
                <cc1:TextBoxEx ID="dsoReference" runat="server" LabelText="Reference:" LabelWidth="6em"
                    IsReadOnly="true" ReadOnlyContentCssClass="suHeaderReadOnlyCss">
                </cc1:TextBoxEx>
            </td>
            <td  style="width:60%">
                <cc1:TextBoxEx ID="dsoName" runat="server" LabelText="Name:" LabelWidth="7em" IsReadOnly="true"
                    ReadOnlyContentCssClass="suHeaderReadOnlyCss">
                </cc1:TextBoxEx>
            </td>
        </tr>
        <tr>
            <td>
                <cc1:TextBoxEx ID="dsoDateOfBirth" runat="server" LabelText="Birth Date:" LabelWidth="6em"
                    IsReadOnly="true" ReadOnlyContentCssClass="suHeaderReadOnlyCss">
                </cc1:TextBoxEx>
            </td>
            <td>
                <cc1:TextBoxEx ID="dsoDateOfDeath" runat="server" LabelText="Death Date:" LabelWidth="7em"
                    IsReadOnly="true" ReadOnlyContentCssClass="suHeaderReadOnlyCss">
                </cc1:TextBoxEx>
            </td>
        </tr>
    </table>
</fieldset>
