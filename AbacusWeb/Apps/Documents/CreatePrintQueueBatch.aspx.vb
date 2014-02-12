Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Utils = Target.Library.Utils
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.Documents
Imports Sec = Target.Web.Apps.Security
Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Web.Apps.Jobs.UserControls

Namespace Apps.Documents


    ''' <summary>
    ''' Screen providing the ability to batch queued documents
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     IHS      08/04/2011  D11960 - Created
    ''' </history>
    Partial Class CreatePrintQueueBatch
        Inherits Target.Web.Apps.BasePage

#Region " Private Variables "

        Private Const _NavigationItemKey As String = "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"
        Private Const _PageTitle As String = "Create Print Queue Batch"

#End Region

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(ConstantsManager.GetConstant(_NavigationItemKey), _PageTitle)

            Me.AddExtraCssStyle(Nothing, ".PQBbutton { float:right; width: 5em; margin-left:0.5em; }")

            If Not IsPostBack Then BindPrinters()

            PopulateFilterCriteria()


        End Sub

        Protected Sub btnCreate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreate.Click
            Dim msg As New ErrorMessage
            Dim trans As SqlTransaction = Nothing

            Try
                Dim documentList As New List(Of ViewableDocument)()
                Dim printerID As Integer
                Dim dateNow As DateTime

                Me.Validate("Create")

                If Not Me.IsValid Then Exit Sub

                msg = GetDocumentsToPrint(documentList)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                trans = SqlHelper.GetTransaction(Me.DbConnection)

                printerID = Integer.Parse(cboPrinter.GetPostBackValue())

                dateNow = DateTime.Now

                If CType(CreateJobPointInTime1, ucCreateJobPointInTime).CreateJobNow Then
                    CType(CreateJobPointInTime1, ucCreateJobPointInTime).CreateJobDateTime = dateNow
                End If

                msg = DocumentPrinterBL.BatchDocumentsForPrinting(trans, documentList, printerID, False, dateNow, _
                                                                  CType(CreateJobPointInTime1, ucCreateJobPointInTime).CreateJobDateTime, _
                                                                  txtComment.Text)

                If Not msg.Success Then WebUtils.DisplayError(msg)

                trans.Commit()

                msg.Success = True

                Response.Redirect("PrintQueueBatches.aspx")

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                WebUtils.DisplayError(msg)
            Finally
                If Not msg Is Nothing AndAlso Not msg.Success Then
                    SqlHelper.RollbackTransaction(trans)
                End If
            End Try
        End Sub

        Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnBack.Click

            cboPrinter.RequiredValidator.Enabled = False

            Response.Redirect(String.Format("PrintQueue.aspx?{0}", Request.QueryString))

        End Sub

#End Region

#Region " Private Methods "

        Private Function BindPrinters() As ErrorMessage
            Try
                Dim msg As New ErrorMessage
                Dim printers As New DocumentPrinterCollection

                msg = DocumentPrinterBL.FetchList(Me.DbConnection, printers, TriState.True)
                If Not msg.Success Then Return msg

                cboPrinter.DropDownList.DataTextField = "PrinterName"
                cboPrinter.DropDownList.DataValueField = "ID"
                cboPrinter.DropDownList.DataSource = printers
                cboPrinter.DropDownList.DataBind()

                ' insert empty item at top - to enforce printer selection
                cboPrinter.DropDownList.Items.Insert(0, New ListItem(String.Empty, String.Empty))

                Return msg
            Catch ex As Exception
                Return Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try
        End Function

        Private Sub PopulateFilterCriteria()
            Dim objFilterHelper As New DocumentFilterHelper(Request.QueryString)
            Dim objDocumentType As New DocumentType(Me.DbConnection, String.Empty, String.Empty)
            Dim documentList As New List(Of ViewableDocument)()
            Dim msg As New ErrorMessage
            Dim sbDocTypes As New StringBuilder()

            objFilterHelper.SetFiltersFromQueryString()

            If objFilterHelper.DocumentTypes.Count > 0 Then
                For Each documentTypeID As Integer In objFilterHelper.DocumentTypes
                    msg = DocumentTypeBL.Fetch(documentTypeID, objDocumentType)
                    If msg.Success Then sbDocTypes.AppendFormat(", {0}", objDocumentType.Description)
                Next

                If sbDocTypes.Length > 2 Then sbDocTypes.Remove(0, 2)

                txtDocumentTypes.Text = sbDocTypes.ToString()
            End If

            txtDescription.Text = objFilterHelper.Description
            txtQueuedBy.Text = objFilterHelper.PrintStatusBy
            txtRecipientReference.Text = objFilterHelper.RecipientReference
            txtRecipientName.Text = objFilterHelper.RecipientName

            msg = GetDocumentsToPrint(documentList)

            txtDocumentCount.Text = documentList.Count.ToString()
        End Sub

        Private Function GetDocumentsToPrint(ByRef documentList As List(Of ViewableDocument)) As ErrorMessage
            Try
                Dim msg As New ErrorMessage
                Dim objFilterHelper As New DocumentFilterHelper(Request.QueryString)
                Dim totalDocuments As Integer

                objFilterHelper.SetFiltersFromQueryString()

                ' get the list of documents
                msg = AbacusClassesBL.FetchDocuments(Me.DbConnection, 1, 999, totalDocuments, Sec.SecurityBL.GetCurrentUser().ID, _
                                                     DocumentAssociationType.Any, -1, Nothing, objFilterHelper.Description, _
                                                     Nothing, objFilterHelper.GetDocTypeXML(), _
                                                     Nothing, Nothing, _
                                                     Nothing, objFilterHelper.RecipientReference, objFilterHelper.RecipientName, Nothing, _
                                                     True, Nothing, _
                                                     Nothing, Nothing, Nothing, _
                                                     Nothing, objFilterHelper.PrintStatusBy, _
                                                     documentList)

                Return msg
            Catch ex As Exception
                Return Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

        End Function

#End Region

    End Class

End Namespace