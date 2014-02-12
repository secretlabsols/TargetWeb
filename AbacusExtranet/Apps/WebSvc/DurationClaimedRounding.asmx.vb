Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Target.Abacus.Library.SDS
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

    ' -----------------------------------------------------------------------------
    ' Project	 : Target.Abacus.Extranet
    ' Class	     : Apps.WebSvc.DurationClaimedRounding
    ' 
    ' -----------------------------------------------------------------------------
    ' <summary>
    '     Business logic functionality related to Duration Claimed Rounding.
    ' </summary>
    ' <remarks>
    ' </remarks>
    ' <history>
    '     Waqas  06/10/2010  Created D11941A Duration claimed rounding
    ' </history>
    ' 
    ' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    ' <System.Web.Script.Services.ScriptService()> _
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/DurationClaimedRounding")> _
    <System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
    <ToolboxItem(False)> _
    Public Class DurationClaimedRounding
        Inherits System.Web.Services.WebService


#Region " Fetch Duration Claimed Rounding Items "

        ''' <summary>
        ''' Fetch duration clsaimed rounding result
        ''' </summary>
        ''' <param name="page">Requested page</param>
        ''' <param name="listFilterReference">Filter for reference</param>
        ''' <param name="listFilterDescription">Filter for Description</param>
        ''' <param name="listFilterExternalAccount">Filter for External Account</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        '''     Waqas  06/10/2010  Created D11941A Duration claimed rounding
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
           Public Function FetchDurationClaimedRoundingEnquiryResult( _
                   ByVal page As Integer, _
                   ByVal listFilterReference As String, _
                   ByVal listFilterDescription As String, _
                   ByVal listFilterExternalAccount As String, _
                   ByVal selectedID As Integer) _
                   As FetchDurationClaimedRoundingEnquiryListResult

            Dim msg As ErrorMessage
            Dim result As FetchDurationClaimedRoundingEnquiryListResult = New FetchDurationClaimedRoundingEnquiryListResult
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser
            Dim isCouncilUser As Boolean
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)



            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                isCouncilUser = SecurityBL.IsCouncilUser(conn, settings, user.ExternalUserID)


                ' get the list of duration claim Rounding
                msg = DurationClaimedRoundingBL.FetchDurationClaimRoundingEnquiryResults( _
                        conn, _
                        user.ID, _
                        page, _
                        pageSize, _
                        user.ExternalUserID, _
                        isCouncilUser, _
                        listFilterReference, _
                        listFilterDescription, _
                        listFilterExternalAccount, _
                        totalRecords, _
                        result.DcrItems, _
                        selectedID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchDCRItems({0})"" title=""{2}"">{1}</a>&nbsp;", _
                      page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected

            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function


        ''' <summary>
        ''' Fetch External account Enquiry results
        ''' </summary>
        ''' <param name="page">page</param>
        ''' <param name="listFilterExternalAccount">Filter External Account</param>
        ''' <param name="listFilterEmailAddress">Filter Email address</param>
        ''' <param name="listFilterFullName">Filter Full Name</param>
        ''' <returns>FetchInPlaceExternalAccountEnquiryResults</returns>
        ''' <remarks></remarks>
        ''' <history>
        '''     Waqas  06/10/2010  Created D11941A Duration claimed rounding
        ''' </history>
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
         Public Function FetchInPlaceExternalAccountEnquiryResults( _
                 ByVal page As Integer, _
                   ByVal listFilterExternalAccount As String, _
                   ByVal listFilterEmailAddress As String, _
                   ByVal listFilterFullName As String, _
                   ByVal selectedId As Integer) _
                   As FetchInPlaceExternalAccountEnquiryListResults

            Dim msg As ErrorMessage
            Dim result As FetchInPlaceExternalAccountEnquiryListResults = New FetchInPlaceExternalAccountEnquiryListResults
            Dim conn As SqlConnection = Nothing
            Dim totalRecords As Integer
            Dim pageSize As Integer = 10
            Dim user As WebSecurityUser

            Try
                ' check user is logged in
                user = SecurityBL.GetCurrentUser()
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of duration claim Rounding
                msg = DurationClaimedRoundingBL.FetchInPlaceExternalAccountEnquiryResults( _
                        conn, _
                        page, _
                        pageSize, _
                        listFilterExternalAccount, _
                        listFilterEmailAddress, _
                        listFilterFullName, _
                        totalRecords, _
                        result.EAItems, _
                        selectedId)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      "<a href=""javascript:FetchEAItems({0})"" title=""{2}"">{1}</a>&nbsp;", _
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


    End Class

End Namespace