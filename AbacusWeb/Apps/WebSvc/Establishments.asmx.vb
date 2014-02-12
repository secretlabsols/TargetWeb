
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security

Namespace Apps.WebSvc

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
	''' Class	 : Abacus.Web.Apps.WebSvc.Establishments
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to retrieve establishment information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [MoTahir]   01/10/2009 D11678
	''' 	[Mikevo]	08/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
	<System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/Establishments")> _
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

#Region " FetchDomProviderList "

		''' -----------------------------------------------------------------------------
		''' <summary>
		'''     Retrieves a list of domiciliry providers.
		''' </summary>
		''' <param name="page">The page in the resultset to view.</param>
		''' <param name="selectedEstablishmentID">The ID of the provider to select.</param>
		''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
		''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
		''' <returns></returns>
		''' <remarks>
		'''     Specifying a selectedEstablishmentID overrides the specified page parameter such that
		'''     the page which contain the requested provider is displayed. 
		''' </remarks>
        ''' <history>
        '''     [MoTahir]   01/10/2009  D11678
		''' 	[MikeVO]	08/01/2008	Created
		''' </history>
		''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchDomProviderList(ByVal page As Integer, ByVal selectedEstablishmentID As Integer, _
          ByVal listFilterReference As String, ByVal listFilterName As String, ByVal includeRedundant As Boolean) As FetchEstablishmentsListResult

            Dim msg As ErrorMessage
            Dim providers As ArrayList = Nothing
            Dim result As FetchEstablishmentsListResult = New FetchEstablishmentsListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim webUser As WebSecurityUser
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                webUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of providers
                msg = AbacusClassesBL.FetchEstablishments( _
                   conn, -1, page, pageSize, selectedEstablishmentID, listFilterReference, _
                   listFilterName, TriState.True, totalRecords, providers, settings.CurrentApplicationID, webUser.ID, includeRedundant)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Establishments = providers
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

#Region " FetchProviderList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of providers.
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="selectedEstablishmentID">The ID of the provider to select.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedEstablishmentID overrides the specified page parameter such that
        '''     the page which contain the requested provider is displayed. 
        ''' </remarks>
        ''' <history>
        ''' 	[PaulW]	30/06/2010	Created D11795 - Generic Contracts/Service Orders
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchProviderList(ByVal page As Integer, ByVal selectedEstablishmentID As Integer, _
          ByVal listFilterReference As String, ByVal listFilterName As String, ByVal includeRedundant As Boolean) As FetchEstablishmentsListResult

            Dim msg As ErrorMessage
            Dim providers As ArrayList = Nothing
            Dim result As FetchEstablishmentsListResult = New FetchEstablishmentsListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim webUser As WebSecurityUser
            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin()
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                webUser = SecurityBL.GetCurrentUser()

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of providers
                msg = AbacusClassesBL.FetchEstablishments( _
                   conn, -1, page, pageSize, selectedEstablishmentID, listFilterReference, _
                   listFilterName, TriState.UseDefault, totalRecords, providers, settings.CurrentApplicationID, webUser.ID, includeRedundant)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Establishments = providers
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