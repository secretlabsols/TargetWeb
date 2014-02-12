Imports Microsoft.Reporting.WebForms
Imports System.Collections.Specialized
Imports System.Net

Namespace Apps.Reports

    ''' <summary>
    ''' Class encapsulating the reporting services credentials
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class ReportCredentials
        Implements IReportServerCredentials

        Private Property AuthCookie As Cookie = Nothing

        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Create a new instance of the ReportCredentials holding reference to the reporting services authentication cookie
        ''' </summary>
        ''' <param name="authCookie">The authentication cookie for the authenticated user</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal authCookie As Cookie)
            Me.AuthCookie = authCookie
        End Sub

        ''' <summary>
        ''' Gets the credentials to use for forms authentication to the web service
        ''' </summary>
        ''' <param name="authCookie">The authentication cookie that will hold the authentication ticket</param>
        ''' <param name="userName">The username used to access reporting services</param>
        ''' <param name="password">The password used to access reporting services</param>
        ''' <param name="authority"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFormsCredentials(ByRef authCookie As System.Net.Cookie, _
                                            ByRef userName As String, _
                                            ByRef password As String, _
                                            ByRef authority As String) As Boolean _
                                        Implements IReportServerCredentials.GetFormsCredentials

            Dim configuration As NameValueCollection = ConfigurationManager.GetSection("reportServer")

            authCookie = Nothing
            userName = configuration("UserName")
            password = configuration("UserPassword")
            authority = String.Empty

            Return True

        End Function

        ''' <summary>
        ''' The user to use for impersonation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>We are using forms authentication, therefore this method always returns null</remarks>
        Public ReadOnly Property ImpersonationUser As System.Security.Principal.WindowsIdentity _
            Implements IReportServerCredentials.ImpersonationUser
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' The network credentials to use for accessing reporting services
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>We are using forms authentication, therefore this method always returns null</remarks>
        Public ReadOnly Property NetworkCredentials As System.Net.ICredentials _
            Implements IReportServerCredentials.NetworkCredentials
            Get
                ' We are using forms authentication
                Return Nothing
            End Get
        End Property
    End Class

End Namespace