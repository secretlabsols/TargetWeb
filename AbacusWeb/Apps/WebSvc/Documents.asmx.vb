
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports DPI = Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.Abacus.Library.Documents

Namespace Apps.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.Documents
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to retrieve document information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     JohnF        17/10/2013  Added 'print queue batch ID' to Print/QueueAllDocuments (#8262)
    '''     Iftikhar     28/01/2011  Created - D11915
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/Documents")> _
    Public Class Documents
        Inherits System.Web.Services.WebService

#Region " Web Services Designer Generated Code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Web Services Designer.
            InitializeComponent()

            'Add your own initialization code after the InitializeComponent() call

        End Sub

        'Required by the Web Services Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Web Services Designer
        'It can be modified using the Web Services Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            components = New System.ComponentModel.Container
        End Sub

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            'CODEGEN: This procedure is required by the Web Services Designer
            'Do not modify it using the code editor.
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region

#Region " FetchDocumentList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of documents..
        ''' </summary>
        ''' <param name="page"></param>
        ''' <param name="pageSize"></param>
        ''' <param name="totalRecords"></param>
        ''' <param name="serviceUserType"></param>
        ''' <param name="documentAssociatorID"></param>
        ''' <param name="filterDescription"></param>
        ''' <param name="filterOrigin"></param>
        ''' <param name="filterDocumentTypes"></param>
        ''' <param name="filterCreatedFromdate"></param>
        ''' <param name="filterCreatedTodate"></param>
        ''' <param name="filterCreatedByUserName"></param>
        ''' <param name="filterNeverQueued"></param>
        ''' <param name="filterQueued"></param>
        ''' <param name="filterBatched"></param>
        ''' <param name="filterSentToPrinter"></param>
        ''' <param name="filterPrintedFromDate"></param>
        ''' <param name="filterPrintedToDate"></param>
        ''' <param name="filterPrintedByUserName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	Iftikhar	11/05/2011	SDS Issue #659 - returning CurrPage value
        ''' 	Iftikhar	28/01/2011	D11915 - Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDocumentList(ByVal page As Integer, _
                                          ByVal pageSize As Integer, _
                                          ByRef totalRecords As Integer, _
                                          ByVal serviceUserType As Nullable(Of Byte), _
                                          ByVal documentAssociatorID As Integer, _
                                          ByVal documentPrintQueueBatchID As Nullable(Of Integer), _
                                          ByVal filterDocumentTypes As String, _
                                          ByVal filterOrigin As Nullable(Of Byte), _
                                          ByVal filterDescription As String, _
                                          ByVal filterCreatedFromdate As Nullable(Of DateTime), _
                                          ByVal filterCreatedTodate As Nullable(Of DateTime), _
                                          ByVal filterCreatedByUserName As String, _
                                          ByVal filterRecipientReference As String, _
                                          ByVal filterRecipientName As String, _
                                          ByVal filterNeverQueued As Nullable(Of Boolean), _
                                          ByVal filterQueued As Nullable(Of Boolean), _
                                          ByVal filterBatched As Nullable(Of Boolean), _
                                          ByVal filterSentToPrinter As Nullable(Of Boolean), _
                                          ByVal filterRemovedFromQueue As Nullable(Of Boolean), _
                                          ByVal filterPrintedFromDate As Nullable(Of DateTime), _
                                          ByVal filterPrintedToDate As Nullable(Of DateTime), _
                                          ByVal filterPrintedByUserName As String) As FetchDocumentsListResult

            Dim msg As ErrorMessage
            Dim documents As List(Of ViewableDocument) = Nothing
            Dim result As FetchDocumentsListResult = New FetchDocumentsListResult
            Dim conn As SqlConnection = Nothing
            Dim webSecurityUserID As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                webSecurityUserID = SecurityBL.GetCurrentUser().ID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of documents
                msg = AbacusClassesBL.FetchDocuments(conn, page, pageSize, totalRecords, webSecurityUserID, _
                                                     serviceUserType, documentAssociatorID, documentPrintQueueBatchID, _
                                                     filterDescription, filterOrigin, filterDocumentTypes, _
                                                     filterCreatedFromdate, filterCreatedTodate, _
                                                     filterCreatedByUserName, filterRecipientReference, _
                                                     filterRecipientName, _
                                                     filterNeverQueued, filterQueued, filterBatched, _
                                                     filterSentToPrinter, filterRemovedFromQueue, filterPrintedFromDate, _
                                                     filterPrintedToDate, filterPrintedByUserName, _
                                                     documents)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Documents = documents
                    .CurrPage = page
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                     "<a href=""javascript:FetchDocumentList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                     page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " PrintDocument "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function PrintDocument(ByVal documentID As Integer) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim trans As SqlTransaction
            Dim jobID As Integer
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim userName As String = SecurityBL.GetCurrentUser().ExternalUsername

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                trans = SqlHelper.GetTransaction(conn)

                msg = DocumentPrinterBL.PrintSingleDocument(trans, documentID, userName, jobID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    SqlHelper.RollbackTransaction(trans)
                    Return result
                End If

                trans.Commit()

                result.List = New List(Of ViewablePair)
                result.List.Add(New ViewablePair(jobID, "JobID"))
                result.ErrMsg = New ErrorMessage
                result.ErrMsg.Success = True

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " QueueDocument "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function QueueDocument(ByVal documentID As Integer) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim trans As SqlTransaction
            Dim documentPrintQueueID As Integer
            Dim result As ViewablePairListResult = New ViewablePairListResult

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                trans = SqlHelper.GetTransaction(conn)

                msg = DocumentPrinterBL.QueueSingleDocumentForBatchPrinting(trans, documentID, DateTime.Now, _
                                                                            SecurityBL.GetCurrentUser().ExternalUsername, _
                                                                            documentPrintQueueID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    SqlHelper.RollbackTransaction(trans)
                    Return result
                End If

                trans.Commit()

                result.List = New List(Of ViewablePair)
                result.List.Add(New ViewablePair(documentPrintQueueID, "DocumentPrintQueueID"))
                result.ErrMsg = New ErrorMessage
                result.ErrMsg.Success = True

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " DequeueDocument "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DequeueDocument(ByVal documentID As Integer) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim trans As SqlTransaction
            Dim result As ViewablePairListResult = New ViewablePairListResult

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                trans = SqlHelper.GetTransaction(conn)

                msg = DocumentPrinterBL.DequeueDocument(trans, documentID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    SqlHelper.RollbackTransaction(trans)
                    Return result
                End If

                trans.Commit()

                result.ErrMsg = New ErrorMessage
                result.ErrMsg.Success = True

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " PrintAllDocuments "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Prints all filtered documents.
        ''' </summary>
        ''' <param name="page"></param>
        ''' <param name="pageSize"></param>
        ''' <param name="totalRecords"></param>
        ''' <param name="serviceUserType"></param>
        ''' <param name="documentAssociatorID"></param>
        ''' <param name="filterDescription"></param>
        ''' <param name="filterOrigin"></param>
        ''' <param name="filterDocumentTypes"></param>
        ''' <param name="filterCreatedFromdate"></param>
        ''' <param name="filterCreatedTodate"></param>
        ''' <param name="filterCreatedByUserName"></param>
        ''' <param name="filterNeverQueued"></param>
        ''' <param name="filterQueued"></param>
        ''' <param name="filterBatched"></param>
        ''' <param name="filterSentToPrinter"></param>
        ''' <param name="filterPrintedFromDate"></param>
        ''' <param name="filterPrintedToDate"></param>
        ''' <param name="filterPrintedByUserName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	Iftikhar	04/04/2011	D11960 - Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function PrintAllDocuments(ByVal page As Integer, _
                                          ByVal pageSize As Integer, _
                                          ByRef totalRecords As Integer, _
                                          ByVal serviceUserType As Nullable(Of Byte), _
                                          ByVal documentAssociatorID As Integer, _
                                          ByVal documentPrintQueueBatchID As Nullable(Of Integer), _
                                          ByVal filterDocumentTypes As String, _
                                          ByVal filterOrigin As Nullable(Of Byte), _
                                          ByVal filterDescription As String, _
                                          ByVal filterCreatedFromdate As Nullable(Of DateTime), _
                                          ByVal filterCreatedTodate As Nullable(Of DateTime), _
                                          ByVal filterCreatedByUserName As String, _
                                          ByVal filterNeverQueued As Nullable(Of Boolean), _
                                          ByVal filterQueued As Nullable(Of Boolean), _
                                          ByVal filterBatched As Nullable(Of Boolean), _
                                          ByVal filterSentToPrinter As Nullable(Of Boolean), _
                                          ByVal filterRemovedFromQueue As Nullable(Of Boolean), _
                                          ByVal filterPrintedFromDate As Nullable(Of DateTime), _
                                          ByVal filterPrintedToDate As Nullable(Of DateTime), _
                                          ByVal filterPrintedByUserName As String) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim documents As List(Of ViewableDocument) = Nothing
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim conn As SqlConnection = Nothing
            Dim trans As SqlTransaction
            Dim webSecurityUserID As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                webSecurityUserID = SecurityBL.GetCurrentUser().ID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of documents
                msg = AbacusClassesBL.FetchDocuments(conn, page, pageSize, totalRecords, webSecurityUserID, _
                                                     serviceUserType, documentAssociatorID, documentPrintQueueBatchID, _
                                                     filterDescription, filterOrigin, filterDocumentTypes, _
                                                     filterCreatedFromdate, filterCreatedTodate, _
                                                     filterCreatedByUserName, Nothing, Nothing, filterNeverQueued, _
                                                     filterQueued, filterBatched, _
                                                     filterSentToPrinter, filterRemovedFromQueue, filterPrintedFromDate, _
                                                     filterPrintedToDate, filterPrintedByUserName, _
                                                     documents)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                trans = SqlHelper.GetTransaction(conn)

                msg = DocumentPrinterBL.PrintAllDocuments(trans, documents)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    SqlHelper.RollbackTransaction(trans)
                    Return result
                End If

                trans.Commit()

                result.List = New List(Of ViewablePair)
                result.List.Add(New ViewablePair(documents.Count, "DocumentsCount"))
                result.ErrMsg = New ErrorMessage
                result.ErrMsg.Success = True

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " QueueAllDocuments "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Queues all filtered documents.
        ''' </summary>
        ''' <param name="page"></param>
        ''' <param name="pageSize"></param>
        ''' <param name="totalRecords"></param>
        ''' <param name="serviceUserType"></param>
        ''' <param name="documentAssociatorID"></param>
        ''' <param name="filterDescription"></param>
        ''' <param name="filterOrigin"></param>
        ''' <param name="filterDocumentTypes"></param>
        ''' <param name="filterCreatedFromdate"></param>
        ''' <param name="filterCreatedTodate"></param>
        ''' <param name="filterCreatedByUserName"></param>
        ''' <param name="filterNeverQueued"></param>
        ''' <param name="filterQueued"></param>
        ''' <param name="filterBatched"></param>
        ''' <param name="filterSentToPrinter"></param>
        ''' <param name="filterPrintedFromDate"></param>
        ''' <param name="filterPrintedToDate"></param>
        ''' <param name="filterPrintedByUserName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	Iftikhar	06/04/2011	D11960 - Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function QueueAllDocuments(ByVal page As Integer, _
                                          ByVal pageSize As Integer, _
                                          ByRef totalRecords As Integer, _
                                          ByVal serviceUserType As Nullable(Of Byte), _
                                          ByVal documentAssociatorID As Integer, _
                                          ByVal documentPrintQueueBatchID As Nullable(Of Integer), _
                                          ByVal filterDocumentTypes As String, _
                                          ByVal filterOrigin As Nullable(Of Byte), _
                                          ByVal filterDescription As String, _
                                          ByVal filterCreatedFromdate As Nullable(Of DateTime), _
                                          ByVal filterCreatedTodate As Nullable(Of DateTime), _
                                          ByVal filterCreatedByUserName As String, _
                                          ByVal filterNeverQueued As Nullable(Of Boolean), _
                                          ByVal filterQueued As Nullable(Of Boolean), _
                                          ByVal filterBatched As Nullable(Of Boolean), _
                                          ByVal filterSentToPrinter As Nullable(Of Boolean), _
                                          ByVal filterRemovedFromQueue As Nullable(Of Boolean), _
                                          ByVal filterPrintedFromDate As Nullable(Of DateTime), _
                                          ByVal filterPrintedToDate As Nullable(Of DateTime), _
                                          ByVal filterPrintedByUserName As String) As ViewablePairListResult

            Dim msg As ErrorMessage
            Dim documents As List(Of ViewableDocument) = Nothing
            Dim result As ViewablePairListResult = New ViewablePairListResult
            Dim conn As SqlConnection = Nothing
            Dim trans As SqlTransaction
            Dim webSecurityUserID As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                webSecurityUserID = SecurityBL.GetCurrentUser().ID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of documents
                msg = AbacusClassesBL.FetchDocuments(conn, page, pageSize, totalRecords, webSecurityUserID, _
                                                     serviceUserType, documentAssociatorID, documentPrintQueueBatchID, _
                                                     filterDescription, filterOrigin, filterDocumentTypes, _
                                                     filterCreatedFromdate, filterCreatedTodate, _
                                                     filterCreatedByUserName, Nothing, Nothing, filterNeverQueued, _
                                                     filterQueued, filterBatched, _
                                                     filterSentToPrinter, filterRemovedFromQueue, filterPrintedFromDate, _
                                                     filterPrintedToDate, filterPrintedByUserName, _
                                                     documents)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                trans = SqlHelper.GetTransaction(conn)

                msg = DocumentPrinterBL.QueueAllDocuments(trans, documents)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    SqlHelper.RollbackTransaction(trans)
                    Return result
                End If

                trans.Commit()

                result.List = New List(Of ViewablePair)
                result.List.Add(New ViewablePair(documents.Count, "DocumentsCount"))
                result.ErrMsg = New ErrorMessage
                result.ErrMsg.Success = True

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchPrintQueueBatches "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of document print batches.
        ''' </summary>
        ''' <param name="page"></param>
        ''' <param name="pageSize"></param>
        ''' <param name="totalRecords"></param>
        ''' <param name="filterCreatedFromdate"></param>
        ''' <param name="filterCreatedTodate"></param>
        ''' <param name="filterCreatedByUserName"></param>
        ''' <param name="filterDocumentPrinterID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	IHS		13/04/2011	D11960 - Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchPrintQueueBatches(ByVal page As Integer, _
                                               ByVal pageSize As Integer, _
                                               ByRef totalRecords As Integer, _
                                               ByVal filterCreatedFromdate As Nullable(Of DateTime), _
                                               ByVal filterCreatedTodate As Nullable(Of DateTime), _
                                               ByVal filterCreatedByUserName As String, _
                                               ByVal filterDocumentPrinterID As Nullable(Of Integer)) _
                                               As FetchDocumentPrintQueueBatchesResult

            Dim msg As ErrorMessage
            Dim documentBatches As List(Of ViewableDocumentPrintQueueBatch) = Nothing
            Dim result As FetchDocumentPrintQueueBatchesResult = New FetchDocumentPrintQueueBatchesResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of document batches
                msg = AbacusClassesBL.FetchDocumentPrintQueueBatches(conn, page, pageSize, totalRecords, _
                                                                     filterCreatedFromdate, filterCreatedTodate, _
                                                                     filterCreatedByUserName, filterDocumentPrinterID, _
                                                                     documentBatches)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PrintQueueBatches = documentBatches
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                     "<a href=""javascript:currentPage = {0}; FetchPrintQueueBatches();"" title=""{2}"">{1}</a>&nbsp;", _
                     page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchPrinterPaperSourceList "

        ''' <summary>
        ''' Retrieves a list of paper sources (i.e. trays) for a printer.
        ''' </summary>
        ''' <param name="documentPrinterID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' Iftikhar  25/03/2011  Created (D11960)
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchPrinterPaperSourceList(ByVal documentPrinterID As Integer) _
                                                    As FetchDocumentPrinterPaperSourceListResult
            Dim msg As New ErrorMessage
            Dim result As FetchDocumentPrinterPaperSourceListResult = New FetchDocumentPrinterPaperSourceListResult
            Dim conn As SqlConnection = Nothing
            Dim paperSourceCollection As New DocumentPrinterPaperSourceCollection
            Dim printer As DocumentPrinter

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DocumentPrinterPaperSource.FetchList(conn:=conn, list:=paperSourceCollection, _
                                                           documentPrinterID:=documentPrinterID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                result.PrinterPaperSources = New List(Of DocumentPrinterPaperSource)
                result.PrinterPaperSources.AddRange(paperSourceCollection.ToArray())

                printer = New DocumentPrinter(conn, String.Empty, String.Empty)
                msg = printer.Fetch(documentPrinterID)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                result.PrinterCanDuplex = printer.CanDuplex

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                End With
            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchPrinterPaperSizeList "

        ''' <summary>
        ''' Retrieves a list of paper sizes for a printer.
        ''' </summary>
        ''' <param name="documentPrinterID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' Iftikhar  25/03/2011  Created (D11960)
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchPrinterPaperSizeList(ByVal documentPrinterID As Integer) _
                                                    As FetchDocumentPrinterPaperSizeListResult
            Dim msg As New ErrorMessage
            Dim result As FetchDocumentPrinterPaperSizeListResult = New FetchDocumentPrinterPaperSizeListResult
            Dim conn As SqlConnection = Nothing
            Dim paperSizeCollection As New DocumentPrinterPaperSizeCollection

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DocumentPrinterPaperSize.FetchList(conn:=conn, list:=paperSizeCollection, _
                                                         documentPrinterID:=documentPrinterID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                result.PrinterPaperSizes = New List(Of DocumentPrinterPaperSize)
                result.PrinterPaperSizes.AddRange(paperSizeCollection.ToArray())

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                End With
            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " DefaultValueForPublishToExtranetChkbx "

        ''' ------------------------------------------------------------------------------------
        ''' <summary>
        '''     Determines if the Publish to Extranet checkbox should default to ticked or not
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[PaulW]	24/10/2011	Created
        ''' </history>
        ''' -------------------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DefaultValueForPublishToExtranetChkbx(ByVal DocumentTypeID As Integer) As Boolean

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser = Nothing
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    Return False
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the current user
                user = SecurityBL.GetCurrentUser()

                ' get the service Order
                Return DocumentTypeBL.GetPublishToExtranet(DocumentTypeID, conn)

            Catch ex As Exception
                Return False
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

        End Function

#End Region


    End Class

End Namespace