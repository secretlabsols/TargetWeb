<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewProviderInvoiceNotes.ascx.vb" 
Inherits="Target.Abacus.Web.Apps.UserControls.ViewProviderInvoiceNotes" 
TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
 <asp:Panel ID="IsPopUp" runat="server">
 <div id="invNotesPopup">
            Invoice Note:
            <div style="border:solid 1px #7F9DB9; overflow:auto; height:260px; padding:10px;" >
                <asp:Panel ID="pnlNotes" runat="server"></asp:Panel>

            </div>
            <div style="padding-top:10px;">
                Entered on <asp:Label runat="server" id="lblDate" Text=""></asp:Label>
                &nbsp; at&nbsp;
                <asp:Label runat="server" id="lblat" Text=""></asp:Label>
            </div>         
      
  </div>
  </asp:Panel>
  
   <asp:Panel ID="IsNotPopUp" Visible="false" runat="server">
   <div id="Div1">
   <br />
            <fieldset>
            <legend> Provider-entered Invoice Note</legend>
                
                <div style="border:solid 1px #7F9DB9; overflow:auto; height:150px; padding:10px;" >
                    <asp:Panel ID="pnlNotesEmbeded" runat="server"></asp:Panel>

                </div>
                <div style="padding-top:10px;">
                    Entered on <asp:Label runat="server" id="lblDateEmbeded" Text=""></asp:Label>
                    &nbsp; at&nbsp;
                    <asp:Label runat="server" id="lblatEmbeded" Text=""></asp:Label>
                </div>    
            </fieldset>     
      
  </div>
  </asp:Panel>