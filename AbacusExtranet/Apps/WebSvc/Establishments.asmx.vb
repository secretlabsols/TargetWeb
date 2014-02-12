
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.Establishments
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to retrieve establishment information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [MoTahir] 01/10/2009 D11678
    ''' 	[Mikevo]	07/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusExtranet/Apps/WebSvc/Establishments")> _
    Public Class Establishments
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

#Region " FetchResHomeList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of residential homes available to the current user.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="selectedEstablishmentID">The ID of the home to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedEstablishmentID overrides the specified page parameter such that
        '''     the page which contain the requested provider is displayed. 
        ''' </remarks>
        ''' <history>
        ''' 	[MikeVO]	07/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchResHomeList(ByVal page As Integer, ByVal selectedEstablishmentID As Integer, _
                                        ByVal listFilterReference As String, ByVal listFilterName As String) As FetchEstablishmentsListResult

            Dim msg As ErrorMessage
            Dim homes As ArrayList = Nothing
            Dim result As FetchEstablishmentsListResult = New FetchEstablishmentsListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ExternalUserID


                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of homes
                msg = AbacusClassesBL.FetchEstablishments( _
                                        conn, userID, page, pageSize, selectedEstablishmentID, listFilterReference, _
                                        listFilterName, TriState.False, totalRecords, homes, settings.CurrentApplicationID, -1)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Establishments = homes
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchEstabList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchDomProviderList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of Domiciliary Providers available to the current user.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="selectedProviderID">The ID of the Dom Provider to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedProviderID overrides the specified page parameter such that
        '''     the page which contain the requested provider is displayed. 
        ''' </remarks>
        ''' <history>
        '''     [MoTahir] 01/10/2009 D11678
        ''' 	[PaulW]	30/01/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderList(ByVal page As Integer, ByVal selectedProviderID As Integer, _
                                        ByVal listFilterReference As String, ByVal listFilterName As String) As FetchEstablishmentsListResult

            Dim msg As ErrorMessage
            Dim homes As ArrayList = Nothing
            Dim result As FetchEstablishmentsListResult = New FetchEstablishmentsListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ExternalUserID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of homes
                msg = AbacusClassesBL.FetchEstablishments( _
                                        conn, userID, page, pageSize, selectedProviderID, listFilterReference, _
                                        listFilterName, TriState.True, totalRecords, homes, settings.CurrentApplicationID, -1)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Establishments = homes
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchEstabList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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