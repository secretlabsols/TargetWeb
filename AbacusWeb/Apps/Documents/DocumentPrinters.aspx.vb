Imports Target.Web.Apps
Imports Target.Library.Web.UserControls
Imports Target.Library
Imports Target.Web.Apps.Security
Imports DAL = Target.Abacus.Library.DataClasses
Imports Docs = Target.Abacus.Library.Documents
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library.Documents
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library


Namespace Apps.Documents

    ''' <summary>
    ''' Read-only screen displaying all printers for document printing
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  23/03/2011  D11960 - Created
    ''' </history>
    Partial Public Class DocumentPrinters
        Inherits BasePage

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
                Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Documents.ReferenceData.DocumentPrinters"), "Document Printers")

                Dim _stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

                With _stdBut
                    .AllowEdit = False
                    .AllowDelete = False
                    .AllowNew = False
                    .SearchBy.Add("Printer Name", "PrinterName")
                    .GenericFinderTypeID = GenericFinderType.DocumentPrinters
                    .AuditLogTableNames.Add("DocumentPrinter")
                End With

                AddHandler _stdBut.FindClicked, AddressOf FindClicked
            Catch ex As Exception
                WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))
            End Try
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Try
                Dim msg As New ErrorMessage
                Dim printer As DAL.DocumentPrinter
                Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

                printer = New DAL.DocumentPrinter(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                msg = DocumentPrinterBL.Fetch(e.ItemID, printer)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                txtPrinterName.Text = printer.PrinterName
                txtJobServiceInstanceName.Text = printer.JobServiceInstanceName
                txtCanDuplex.Text = IIf(printer.CanDuplex, "Yes", "No")
                txtIsValid.Text = IIf(printer.IsValid, "Yes", "No")

                msg = BindPaperSources(printer.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                msg = BindPaperSizes(printer.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            Catch ex As Exception
                WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))
            End Try
        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Binds printer's paper sources (i.e. trays) to ListBox
        ''' </summary>
        ''' <param name="documentPrinterID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' Iftikhar  24/03/2011  Created (D11960)
        ''' </history>
        Private Function BindPaperSources(ByVal documentPrinterID As Integer) As ErrorMessage
            Try
                Dim msg As New ErrorMessage
                Dim sources As New DocumentPrinterPaperSourceCollection

                msg = DocumentPrinterPaperSourceBL.FetchList(Me.DbConnection, sources, documentPrinterID)
                If Not msg.Success Then Return msg

                lbPaperSources.DataTextField = "SourceName"
                lbPaperSources.DataValueField = "ID"
                lbPaperSources.DataSource = sources
                lbPaperSources.DataBind()

                Return msg
            Catch ex As Exception
                Return Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try
        End Function

        ''' <summary>
        ''' Binds printer's paper sizes to ListBox
        ''' </summary>
        ''' <param name="documentPrinterID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' Iftikhar  24/03/2011  Created (D11960)
        ''' </history>
        Private Function BindPaperSizes(ByVal documentPrinterID As Integer) As ErrorMessage
            Try
                Dim msg As New ErrorMessage
                Dim sizes As New DocumentPrinterPaperSizeCollection

                msg = DocumentPrinterPaperSizeBL.FetchList(Me.DbConnection, sizes, documentPrinterID)
                If Not msg.Success Then Return msg

                lbPaperSizes.DataTextField = "PaperName"
                lbPaperSizes.DataValueField = "ID"
                lbPaperSizes.DataSource = sizes
                lbPaperSizes.DataBind()

                Return msg
            Catch ex As Exception
                Return Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try
        End Function

#End Region

    End Class


End Namespace
