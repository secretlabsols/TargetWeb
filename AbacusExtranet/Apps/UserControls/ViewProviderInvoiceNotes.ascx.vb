
Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports System.IO

Namespace Apps.UserControls


    Partial Public Class ViewProviderInvoiceNotes
        Inherits System.Web.UI.UserControl

#Region " Properties "

        Private _InvoiceID As Integer
        Public Property InvoiceID() As Integer
            Get
                Return _InvoiceID
            End Get
            Set(ByVal value As Integer)
                _InvoiceID = value
                Dim thePage As BasePage = CType(Me.Page, BasePage)
                GetInvoiceNotes(thePage, _InvoiceID)
            End Set
        End Property

        Private _InvoiceHasNotes As Boolean
        Public Property InvoiceHasNotes() As Boolean
            Get
                Return _InvoiceHasNotes
            End Get
            Set(ByVal value As Boolean)
                _InvoiceHasNotes = value
            End Set
        End Property


        Private _displayAsPopUp As Boolean = True
        Public Property displayAsPopUp() As Boolean
            Get
                Return _displayAsPopUp
            End Get
            Set(ByVal value As Boolean)
                _displayAsPopUp = value
            End Set
        End Property

#End Region

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/ViewProviderInvoiceNotes.js"))
        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'Dim thePage As BasePage = CType(Me.Page, BasePage)
            'If Target.Library.Utils.ToInt32(InvoiceID) > 0 Then
            '    GetInvoiceNotes(thePage, InvoiceID)
            'End If
        End Sub

        Private Sub GetInvoiceNotes(ByVal thePage As BasePage, ByVal id As Integer)
            Dim msg As New ErrorMessage
            Dim invoice As DomProviderInvoice = _
            New DomProviderInvoice(thePage.DbConnection)
            msg = invoice.Fetch(id)
            If Not msg.Success Then
                WebUtils.DisplayError(msg)
                Return
            End If
            If Target.Library.Utils.ToInt32(invoice.ProviderInvoiceNoteID) <> 0 Then
                Dim invNotes As DomProviderInvoiceNotes = New DomProviderInvoiceNotes(thePage.DbConnection)
                msg = New ErrorMessage
                msg = invNotes.Fetch(invoice.ProviderInvoiceNoteID)
                If Not msg.Success Then
                    WebUtils.DisplayError(msg)
                    Return
                End If
                invNotes.Notes = HandleLineFeedCarriageReturns(invNotes.Notes)

                Dim js As String = String.Empty
                Dim ctrl As New LiteralControl
                ctrl.Text = invNotes.Notes
                pnlNotes.Controls.Add(ctrl)
                lblDate.Text = invNotes.Date.ToString("dd/MM/yyyy")
                lblat.Text = invNotes.Time.ToString("hh:mm:ss")
                IsPopUp.Visible = displayAsPopUp

                Dim ctrlEmbeded As New LiteralControl
                ctrlEmbeded.Text = invNotes.Notes
                pnlNotesEmbeded.Controls.Add(ctrlEmbeded)
                lblDateEmbeded.Text = invNotes.Date.ToString("dd/MM/yyyy")
                lblatEmbeded.Text = invNotes.Time.ToString("hh:mm:ss")
                IsNotPopUp.Visible = Not displayAsPopUp

                thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))
                InvoiceHasNotes = True
            End If
        End Sub

        Private Function HandleLineFeedCarriageReturns(ByVal notes As String) As String
            Dim regExp As RegularExpressions.Regex = New RegularExpressions.Regex("(\r\n|\r|\n)+")
            notes = regExp.Replace(notes, "<br/>")
            Return notes
        End Function


    End Class

End Namespace