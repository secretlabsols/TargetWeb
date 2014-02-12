
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
	''' Class	 : Abacus.Web.Apps.WebSvc.Clients
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to retrieve client information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MoTahir     27/08/2010  D11814
	''' 	[Mikevo]	09/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
	<System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/AbacusWeb/Apps/WebSvc/Clients")> _
	Public Class Clients
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

#Region " FetchClientList "

		''' -----------------------------------------------------------------------------
		''' <summary>
		'''     Retrieves a list of clients..
		''' </summary>
		''' <param name="page">The page in the resultset to view.</param>
		''' <param name="selectedClientID">The ID of the client to select.</param>
		''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
		''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
		''' <returns></returns>
		''' <remarks>
		'''     Specifying a selectedClientID overrides the specified page parameter such that
		'''     the page which contain the requested client is displayed. 
		''' </remarks>
		''' <history>
		''' 	[MikeVO]	09/01/2008	Created
		''' </history>
		''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchClientList(ByVal page As Integer, ByVal selectedClientID As Integer, _
         ByVal listFilterReference As String, ByVal listFilterName As String, ByVal listFilterDebtorNumber As String, _
         ByVal listFilterCreditorReference As String) As FetchClientsListResult

            Dim msg As ErrorMessage
            Dim clients As ArrayList = Nothing
            Dim result As FetchClientsListResult = New FetchClientsListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of homes
                msg = AbacusClassesBL.FetchClients( _
                   conn, page, pageSize, selectedClientID, _
                   listFilterReference, listFilterName, listFilterDebtorNumber, listFilterCreditorReference, totalRecords, clients)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Clients = clients
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                     "<a href=""javascript:FetchClientList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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

#Region " FetchThirdPartyList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of Third Partys..
        ''' </summary>
        ''' <param name="page">The page in the resultset to view.</param>
        ''' <param name="ClientID">The ID of the client selected.</param>
        ''' <param name="selectedThirdPartyID">The ID of the third party to select.</param>
        ''' <param name="listFilterName">The custom list filter string to apply to the name column.</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Specifying a selectedThirdPartyID overrides the specified page parameter such that
        '''     the page which contain the requested third party is displayed. 
        ''' </remarks>
        ''' <history>
        ''' 	[Paul]	06/02/2009	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchThirdPartyList(ByVal page As Integer, ByVal ClientID As Integer, _
          ByVal selectedThirdPartyID As Integer, ByVal listFilterName As String) As FetchThirdPartyListResult

            Dim msg As ErrorMessage
            Dim thirdPartys As ArrayList = Nothing
            Dim result As FetchThirdPartyListResult = New FetchThirdPartyListResult
            Dim conn As SqlConnection = Nothing
            Dim userID As Integer
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the current user
                userID = SecurityBL.GetCurrentUser().ID

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get the list of homes
                msg = AbacusClassesBL.FetchThirdPartys( _
                   conn, page, pageSize, ClientID, selectedThirdPartyID, _
                   listFilterName, totalRecords, thirdPartys)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .ThirdPartys = thirdPartys
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                     "<a href=""javascript:FetchThirdPartyList({0})"" title=""{2}"">{1}</a>&nbsp;", _
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