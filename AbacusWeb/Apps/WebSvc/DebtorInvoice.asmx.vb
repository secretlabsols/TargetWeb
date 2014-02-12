Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports Target.Abacus.Jobs.Exports.FinancialExportInterface
Imports Target.Abacus.Library.DebtorInvoices


Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to retrieve debtor Invoice information.
    ''' </summary>
    ''' <remarks></remarks>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/DebtorInvoice")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class DebtorInvoice
        Inherits System.Web.Services.WebService

#Region " DisplayCreateDebtorInvoiceBatchWarning "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Determines if a message is displayed when clicking the create batch button, warning
        '''     the user that not all the invoices will be included in the batch.
        ''' </summary>
        ''' <param name="params"></param>
        ''' <param name="batchID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DisplayCreateDebtorInvoiceBatchWarning(ByVal params As DebtorInvoiceFilterParams, ByVal batchID As String) As BooleanResult

            Dim msg As ErrorMessage
            Dim result As BooleanResult = New BooleanResult
            Dim invoiceCountResult As FetchDebtorInvoiceListCountResult = New FetchDebtorInvoiceListCountResult
            Dim debtorInvoiceCount As Integer, debtorInvoiceBatchCount As Integer
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                '++ Default the start date to a year ago if not specified..
                If Not Utils.IsDate(params.DateFrom) Then
                    params.DateFrom = DateAdd(DateInterval.Year, -1, Date.Now).ToString("yyyy-MM-dd")
                ElseIf Convert.ToDateTime(params.DateFrom) = Convert.ToDateTime("1900-01-01") Then
                    params.DateFrom = DateAdd(DateInterval.Year, -1, Date.Now).ToString("yyyy-MM-dd")
                End If

                'Call once to get list count
                msg = DebtorInvoiceBL.FetchDebtorInvoiceListCount( _
                        conn, params, batchID, 0, _
                        invoiceCountResult.Invoices)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                debtorInvoiceCount = invoiceCountResult.Invoices(0).InvoiceCount
                params.Provisional = Boolean.FalseString.ToLower()
                params.Exclude = Boolean.FalseString.ToLower()
                params.BatchSel = "2"
                'Call Again to get count that would be included on a batch.
                msg = DebtorInvoiceBL.FetchDebtorInvoiceListCount( _
                        conn, params, batchID, 0, _
                        invoiceCountResult.Invoices)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                debtorInvoiceBatchCount = invoiceCountResult.Invoices(0).InvoiceCount

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Value = (debtorInvoiceCount <> debtorInvoiceBatchCount)
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchDebtorInvoiceListCount "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of debtor invoice list info.
        ''' </summary>
        ''' <param name="params"></param>
        ''' <param name="batchID"></param>
        ''' <param name="sectorCodeID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDebtorInvoiceListCount(ByVal params As DebtorInvoiceFilterParams, ByVal batchID As String, ByVal sectorCodeID As String) As FetchDebtorInvoiceListCountResult

            Dim msg As ErrorMessage
            Dim result As FetchDebtorInvoiceListCountResult = New FetchDebtorInvoiceListCountResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 1000000
            Dim page As Int16 = 1
            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                '++ Default the start date to a year ago if not specified..
                If Not Utils.IsDate(params.DateFrom) Then
                    params.DateFrom = DateAdd(DateInterval.Year, -1, Date.Now).ToString("yyyy-MM-dd")
                ElseIf Convert.ToDateTime(params.DateFrom) = Convert.ToDateTime("1900-01-01") Then
                    params.DateFrom = DateAdd(DateInterval.Year, -1, Date.Now).ToString("yyyy-MM-dd")
                End If
                'Default the following values as we will not be including excluded, provisional or items alreaded batched.
                params.Provisional = Boolean.FalseString.ToLower()
                params.Exclude = Boolean.FalseString.ToLower()
                params.BatchSel = "2"


                msg = DebtorInvoiceBL.FetchDebtorInvoiceListCount( _
                        conn, params, batchID, sectorCodeID, _
                        result.Invoices)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDebtorInvoiceListCount({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " DebtorInvoiceToggleExclude "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a single invoice where the 'exclude from debtors' flag is being toggled.
        ''' </summary>
        ''' <param name="invoiceID">The ID of the invoice to select.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DebtorInvoiceToggleExclude(ByVal invoiceID As String) As DebtorInvoiceToggleExcludeResult

            Dim msg As ErrorMessage
            Dim result As DebtorInvoiceToggleExcludeResult = New DebtorInvoiceToggleExcludeResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DebtorInvoiceBL.DebtorInvoiceToggleExclude( _
                        conn, invoiceID, totalRecords, result.Invoices)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:DebtorInvoiceToggleExclude({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      1, 1)
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " CreateDebtorInvoiceBatch "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Batches up Debtor Invoices and Creates a Debtors Interface Job.
        ''' </summary>
        ''' <param name="params"></param>
        ''' <param name="periodNumber"></param>
        ''' <param name="postingDate"></param>
        ''' <param name="postingYear"></param>
        ''' <param name="startDateTime"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CreateDebtorInvoiceBatch(ByVal params As DebtorInvoiceFilterParams, _
                     ByVal startDateTime As DateTime, ByVal postingDate As String, ByVal postingYear As String, _
                     ByVal periodNumber As String, ByVal sectorCode As String, ByVal bytRollback As Byte) As DebtorInvoiceToggleExcludeResult

            Dim result As DebtorInvoiceToggleExcludeResult = New DebtorInvoiceToggleExcludeResult
            Dim conn As SqlConnection = Nothing

            Dim msg As ErrorMessage = Nothing
            Dim currentUser As WebSecurityUser
            Dim jobID As Integer = 0
            Dim interfaceID As Integer = 0, savedBatchRef As String = ""
            Dim lPos As Integer = 0, lCount As Integer = 0
            'Dim bytRollback As Byte
            Dim a4wUser As Target.Abacus.Library.DataClasses.Users

            '*************************************************************************************
            'Override Screen filters for the following params, as these must be as stated below
            params.Provisional = Boolean.FalseString.ToLower()
            params.BatchSel = "2"
            params.Exclude = Boolean.FalseString.ToLower()
            params.ID = 0
            '*************************************************************************************

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                currentUser = SecurityBL.GetCurrentUser()
                a4wUser = New Target.Abacus.Library.DataClasses.Users(conn)
                msg = a4wUser.Fetch(currentUser.ExternalUserID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                'Batch up Debtor Invoices
                msg = DebtorInvoiceBL.CreateDebtorsInterface(conn, _
                                                             params, _
                                                             SecurityBL.GetCurrentUser().ExternalUsername, _
                                                             "", _
                                                             startDateTime, _
                                                             postingDate, _
                                                             periodNumber, _
                                                             postingYear, _
                                                             sectorCode, _
                                                             interfaceID, _
                                                             savedBatchRef)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                '++ Create the job
                msg = Debtors.DebtorInvoicesBL.CreateNewJob(conn, _
                                                            interfaceID, _
                                                            savedBatchRef, _
                                                            True, _
                                                            bytRollback, _
                                                            Convert.ToDateTime(startDateTime), _
                                                            currentUser.ExternalUsername, _
                                                            currentUser.ExternalUserID, _
                                                            a4wUser.EMail, _
                                                            jobID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If



                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:DebtorInvoiceToggleExclude({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      1, 1)
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchSectorCodesList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of sector codes.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSectorCodesList() As FetchSectorCodeListResult

            Dim msg As ErrorMessage
            Dim result As FetchSectorCodeListResult = New FetchSectorCodeListResult
            Dim conn As SqlConnection = Nothing
            Dim page As Int16 = 1
            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DebtorInvoiceBL.FetchSectorCodes(conn, result.SectorCodes)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = ""
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

    End Class

End Namespace