Imports Target.Library.Web.UserControls
Imports Target.Library
Imports Target.Web.Apps.Security

Namespace Apps.Dom.ProviderInvoices

    ''' <summary>
    ''' Screen used to Maintain Dom Provider Invoice Notes.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD      03/03/2011  D11874 - Security changes for introduction of Creditor Payments. Small changes to the way the window behaves if in popup window.
    '''     PaulW       09/06/2009  Created (D11550)
    ''' </history>
    Partial Class Notes
        Inherits Target.Web.Apps.BasePage

        Private _stdBut As StdButtonsBase


#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

        End Sub

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.Notes"), _
                                            "Non Residential Payment Notes")
            Dim js As String
            Dim invoiceID As Integer = Utils.ToInt32(Request.QueryString("id"))
            CType(headerDetails, DomProviderInvoiceHeaderDetails).InvoiceID = invoiceID

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add("Notes.js")

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

            js = String.Format("txtNoteID='{0}';", txtNote.ClientID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), _
                        "Notes.Startup", Target.Library.Web.Utils.WrapClientScript(js))

            txtNote.TextBox.TextMode = TextBoxMode.MultiLine
            txtNote.TextBox.Rows = 5
            txtNote.TextBox.Width = New Unit(25, UnitType.Em)

            With _stdBut
                .AllowDelete = False
                .AllowEdit = False
                .AllowFind = False
                .AllowNew = False
                .AllowBack = True
                If Utils.ToInt32(Request.QueryString("autopopup")) = 1 Then
                    .AllowBack = False
                End If
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DomiciliaryProviderInvoiceNotes")
                .ReportButtonParameters.Add("invoiceID", invoiceID)
            End With

            'Check user Security and hide buttons where necessary
            btnEdit.Visible = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                    currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.Notes.Edit"), _
                    Me.Settings.CurrentApplicationID)

            btnNew.Visible = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                    currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.Notes.AddNew"), _
                    Me.Settings.CurrentApplicationID)

            btnDelete.Visible = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                    currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Payment.CreditorPayments.Notes.Delete"), _
                    Me.Settings.CurrentApplicationID)

        End Sub

    End Class

End Namespace