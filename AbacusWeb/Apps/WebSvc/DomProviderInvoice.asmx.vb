Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security


Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to retrieve domiciliary contract information.
    ''' </summary>
    ''' <remarks></remarks>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/DomProviderInvoice")> _
 <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
 <ToolboxItem(False)> _
 Public Class DomProviderInvoice
        Inherits System.Web.Services.WebService


#Region " Dom Provider Invoice Detail Non Delivery Unit Based Enquiry Results "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderInvoiceDetailNonDeliveryUnitBasedEnquiryResults(ByVal invoiceDetailsId As Integer) As  _
       NonDelivery.FetchDomProviderInvoiceDetailNonDeliveryUnitBasedListResult

            Dim msg As ErrorMessage
            Dim result As NonDelivery.FetchDomProviderInvoiceDetailNonDeliveryUnitBasedListResult = _
            New NonDelivery.FetchDomProviderInvoiceDetailNonDeliveryUnitBasedListResult()
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser

            Try

                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.FetchProviderInvoiceDetailNonDeliveryUnitBased(conn, invoiceDetailsId, result.NonDeliveryUnits)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                Else
                    result.ErrMsg = msg
                End If

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result
        End Function


#End Region

#Region " Dom Provider Invoice Detail Non Delivery Visit Based Enquiry Results "
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
      Public Function FetchDomProviderInvoiceDetailNonDeliveryVisitBasedEnquiryResults(ByVal invoiceDetailsId As Integer) As  _
      NonDelivery.FetchDomProviderInvoiceDetailNonDeliveryVisitBasedListResult

            Dim msg As ErrorMessage
            Dim result As NonDelivery.FetchDomProviderInvoiceDetailNonDeliveryVisitBasedListResult = _
            New NonDelivery.FetchDomProviderInvoiceDetailNonDeliveryVisitBasedListResult
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser

            Try

                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.FetchProviderInvoiceDetailNonDeliveryVisitBased(conn, invoiceDetailsId, result.NonDeliveryVisits)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                Else
                    result.ErrMsg = msg
                End If


            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result
        End Function
#End Region

#Region " Get Current Provider Invoice "
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
      Public Function FetchNonDeliveryCurrentProviderInvoice(ByVal invoiceId As Integer) As NonDelivery.NonDeliveryProviderInvoiceResult
            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Dim result As NonDelivery.NonDeliveryProviderInvoiceResult = New NonDelivery.NonDeliveryProviderInvoiceResult()

            Try
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.FetchNonDeliveryCurrentProviderInvoice(conn, invoiceId, result.CurrentProforma)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                Else
                    result.ErrMsg = msg
                End If


            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try
            Return result
        End Function

#End Region

#Region " Get Current Provider Invoice Detail"
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
      Public Function FetchNonDeliveryCurrentProviderInvoiceDetail(ByVal invoiceDetailId As Integer) As NonDelivery.NonDeliveryProviderInvoiceDetailResult
            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim user As WebSecurityUser
            Dim result As NonDelivery.NonDeliveryProviderInvoiceDetailResult = New NonDelivery.NonDeliveryProviderInvoiceDetailResult()

            Try
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                msg = DomContractBL.FetchNonDeliveryCurrentProviderInvoiceDetail(conn, invoiceDetailId, result.CurrentProformaDetail)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                Else
                    result.ErrMsg = msg
                End If

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
