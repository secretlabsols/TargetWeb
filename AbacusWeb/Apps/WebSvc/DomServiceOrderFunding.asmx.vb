Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library.ServiceOrder
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.WebServices.Responses
Imports Target.Abacus.Library.DataClasses

Namespace Apps.WebSvc

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    '''  <history>
    '''  
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/DomServiceOrderFunding")> _
 <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
 <ToolboxItem(False)> _
    Public Class DomServiceOrderFunding
        Inherits System.Web.Services.WebService

        Private _serviceType As String = String.Empty

#Region " SetSvcTypeAsDefault "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)> _
        Public Function SetSvcTypeAsDefault(ByVal serviceType As String) As WebServiceResponseBase
            Dim result As WebServiceResponseBase = New WebServiceResponseBase
            Dim det As ServiceOrderFundingDetail = New ServiceOrderFundingDetail

            Dim fundingDetails As List(Of ServiceOrderFundingDetail) = IIf(Session("fundingDetail") IsNot Nothing, Session("fundingDetail"), Nothing)

            For Each item As ServiceOrderFundingDetail In fundingDetails
                item.UseAsDefault = False
                If item.ServiceType = serviceType Then
                    item.UseAsDefault = True
                End If
            Next

            Session("fundingDetail") = fundingDetails

            result.ErrMsg.Success = True
            Return result
        End Function

#End Region

#Region " GetItemsForSvcType "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)> _
        Public Function GetItemsForSvcType(ByVal serviceType As String) As WebServiceReponseWithItems(Of ServiceOrderFundingDetail)
            Dim msg As ErrorMessage = New ErrorMessage
            Dim response As New WebServiceReponseWithItems(Of ServiceOrderFundingDetail)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' check the requesting user is logged in
            response.ErrMsg = SecurityBL.ValidateLogin()
            If Not response.ErrMsg.Success Then
                Return response
            End If

            _serviceType = serviceType

            Dim fundingDetails As List(Of ServiceOrderFundingDetail) = IIf(Session("fundingDetail") IsNot Nothing, Session("fundingDetail"), Nothing)

            response.Items = fundingDetails.FindAll(AddressOf FindForServiceType)

            response.ErrMsg.Success = True
            Return response
        End Function

#End Region

#Region " UpdateRowWithProportion "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)> _
        Public Function UpdateRowWithProportion(ByVal serviceType As String, ByVal rowIdentifier As String, ByVal proportion As String) As WebServiceResponseBase
            Dim msg As ErrorMessage = New ErrorMessage
            Dim response As New WebServiceResponseBase
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' check the requesting user is logged in
            response.ErrMsg = SecurityBL.ValidateLogin()
            If Not response.ErrMsg.Success Then
                Return response
            End If

            Dim fundingDetails As List(Of ServiceOrderFundingDetail) = IIf(Session("fundingDetail") IsNot Nothing, Session("fundingDetail"), Nothing)

            For Each item As ServiceOrderFundingDetail In fundingDetails
                If item.ServiceType = serviceType And item.rowIdentifier = Convert.ToInt64(rowIdentifier) Then
                    item.proportion = Convert.ToDecimal(proportion)
                End If
            Next

            Session("fundingDetail") = fundingDetails

            response.ErrMsg.Success = True
            Return response
        End Function

#End Region

#Region " UpdateSessionSoControlsGetRecreated "

        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)> _
        Public Function UpdateSessionSoControlsGetRecreated() As WebServiceResponseBase
            Dim result As WebServiceResponseBase = New WebServiceResponseBase

            Session("RecreateControls") = True

            result.ErrMsg.Success = True
            Return result
        End Function

#End Region

#Region " FindForServiceType "

        Private Function FindForServiceType(ByVal item As ServiceOrderFundingDetail) As Boolean
            Return (item.ServiceType = _serviceType)
        End Function

#End Region

#Region " FindDefaultItems "

        Private Function FindDefaultItems(ByVal item As ServiceOrderFundingDetail) As Boolean
            Return (item.UseAsDefault = True)
        End Function

#End Region

    End Class

End Namespace