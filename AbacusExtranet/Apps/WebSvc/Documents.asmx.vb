Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data.SqlClient
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.ServiceOrder
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security


Namespace Apps.WebSvc

    ''' <summary>
    ''' Web service to retrieve document information.
    ''' </summary>
    ''' <remarks></remarks>
    '''  <history>
    '''  [PaulW]  13/10/2011  Created D11945A
    ''' </history>
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/Documents")> _
    Public Class Documents
        Inherits System.Web.Services.WebService

#Region " FetchDocumentList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of Documents.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[PaulW]	13/10/2011	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDocumentList(ByVal page As Integer, _
                                          ByVal pageSize As Integer, _
                                          ByRef totalRecords As Integer, _
                                          ByVal serviceUserType As Nullable(Of Byte), _
                                          ByVal documentAssociatorID As Integer) As FetchDocumentsListResult

            Dim msg As ErrorMessage
            'Dim documents As List(Of ViewableDocument) = Nothing
            Dim result As FetchDocumentsListResult = New FetchDocumentsListResult
            Dim conn As SqlConnection = Nothing
            Dim webSecurityUserID As Integer = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                'webSecurityUserID = SecurityBL.GetCurrentUser().ID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of documents
                msg = AbacusClassesBL.FetchDocuments(conn, page, pageSize, totalRecords, webSecurityUserID, _
                                                     serviceUserType, documentAssociatorID, Nothing, _
                                                     Nothing, Nothing, Nothing, _
                                                     Nothing, Nothing, _
                                                     Nothing, Nothing, _
                                                     Nothing, _
                                                     True, True, True, _
                                                     True, True, Nothing, _
                                                     Nothing, Nothing, _
                                                     True, _
                                                     result.Documents)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .CurrPage = page
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                      String.Concat("<a href=""javascript:FetchDocumentList({0},", documentAssociatorID.ToString, ")"" title=""{2}"">{1}</a>&nbsp;"), _
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