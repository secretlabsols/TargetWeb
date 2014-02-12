
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections

Namespace Apps.Security

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.SecurityBL
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Abacus Web security helper class.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      29/08/2006  D10921 - support for config settings in database.
    ''' 	[Mikevo]	20/01/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SecurityBL

#Region " Private Variables "

        ' session variables
        Private Shared SESSION_ABACUSWEB_USER As String = "AbacusWebUser"

#End Region

#Region " GetCurrentUser "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets the currently logged on user.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	20/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetCurrentUser() As AbacusUser
            Dim user As AbacusUser = DirectCast(HttpContext.Current.Session(SESSION_ABACUSWEB_USER), AbacusUser)
            If user Is Nothing Then user = New AbacusUser
            Return user
        End Function

#End Region

#Region " IsUserLoggedOn "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Returns if the user is currently logged on.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        '''     Calls GetCurrentUser() to retrieve the currently logged on user.
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	20/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function IsUserLoggedOn() As Boolean
            Return IsUserLoggedOn(GetCurrentUser().ID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Returns if the user is currently logged on.
        ''' </summary>
        ''' <param name="userID">A user ID.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	20/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function IsUserLoggedOn(ByVal userID As Integer) As Boolean
            Return (userID <> 0)
        End Function

#End Region

#Region " ValidateLogin "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checks to see if the current user is logged in.
        ''' </summary>
        ''' <returns>
        '''     Returns the approriate error message if they are not.
        ''' </returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	25/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function ValidateLogin() As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage
            If Not IsUserLoggedOn() Then
                msg.Number = "E0505"
                msg.Message = String.Format(msg.Message, "Your login credentials could not be validated. Please login again and retry the operation.")
            Else
                msg.Success = True
            End If
            Return msg
        End Function

#End Region

#Region " Login "

        Public Shared Sub Login(ByVal conn As SqlConnection, ByVal username As String, ByVal password As String)

            Dim msg As ErrorMessage
            Dim userList As UsersCollection = Nothing
            Dim user As Users = Nothing
            Dim liteUser As AbacusUser

            ' is the user already logged in?
            If IsUserLoggedOn() Then Exit Sub

            ' check username was input
            If username Is Nothing Then
                HttpContext.Current.Response.Redirect(AppSettings("AccessDeniedURL"))
            End If
            ' password can be blank
            If password Is Nothing Then password = String.Empty

            ' get users with user name
            msg = Users.FetchList(conn, userList, username)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)

            ' if we have none returned then login failed
            If userList.Count = 0 Then
                HttpContext.Current.Response.Redirect(AppSettings("AccessDeniedURL"))
            End If

            ' find the first user with the specified username (there should only ever be one anyway)
            For Each u As Users In userList
                If u.Name.ToUpper() = username.ToUpper() Then
                    user = u
                    Exit For
                End If
            Next

            ' check password
            If Utils.AbacusEncrypt(True, password.ToUpper()) <> user.Password Then
                HttpContext.Current.Response.Redirect(AppSettings("AccessDeniedURL"))
            End If

            ' is the account blocked?
            If user.BlockLogin Then
                HttpContext.Current.Response.Redirect(AppSettings("AccessDeniedURL"))
            End If

            ' username and password are good
            ' put user info into session
            liteUser = New AbacusUser(user.ID, user.Name)
            HttpContext.Current.Session(SESSION_ABACUSWEB_USER) = liteUser
            Target.Web.Apps.Security.SecurityBL.StoreAnyAppLoginID(liteUser.ID)

        End Sub

#End Region

    End Class

#Region " AbacusUser "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.AbaucsUser
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple container class to hold Abacus User details.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	20/01/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> _
    Public Class AbacusUser

        Private _id As Integer
        Private _username As String

        Public ReadOnly Property ID() As Integer
            Get
                Return _id
            End Get
        End Property

        Public ReadOnly Property Username() As String
            Get
                Return _username
            End Get
        End Property

        Public Sub New()
        End Sub

        Public Sub New(ByVal id As Integer, ByVal username As String)
            _id = id
            _username = username
        End Sub

    End Class

#End Region

End Namespace