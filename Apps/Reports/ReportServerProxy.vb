Imports Microsoft.Reporting.WebForms
Imports System.Collections.Specialized
Imports System.Net

Namespace Apps.Reports

    ''' <summary>
    ''' Proxy class for accessing the ReportExecution2005.asmx web service and passing authentication details
    ''' </summary>
    ''' <remarks>This class essentially wraps the web service proxy for ReportExecution2005 and overrides
    ''' appropriate methods for implementing forms authentication</remarks>
    Public Class ReportServerProxy
        Inherits ReportExecutionService

        Private _authCookie As Cookie = Nothing

        ''' <summary>
        ''' The authentication cookie that is set when authentication is successful
        ''' </summary>
        ''' <value></value>
        ''' <returns>Instance of Cookie</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property AuthCookie As Cookie
            Get
                Return _authCookie
            End Get
        End Property

        ''' <summary>
        ''' Authenticates against the reporting services instance by providing the username
        ''' and password configured in web.config
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AuthenticateReportServerAccess() As HttpCookie

            Dim configuration As NameValueCollection = Nothing
            Dim hCookie As HttpCookie = Nothing
            Dim authCookie As Cookie = Nothing
            Dim service As ReportServerProxy = Nothing

            service = New ReportServerProxy()
            service.Url = ReportServerConnection.ServiceUri

            Try
                service.LogonUser(ReportServerConnection.Username, _
                                  ReportServerConnection.Password, _
                                  Nothing)
                authCookie = service.AuthCookie

                If authCookie IsNot Nothing Then
                    hCookie = New HttpCookie(authCookie.Name, authCookie.Value)
                    HttpContext.Current.Response.Cookies.Add(hCookie)
                End If

            Catch ex As Exception

                authCookie = Nothing
                Throw

            End Try

            Return hCookie

        End Function

        ''' <summary>
        ''' Overrides the web request method in the web service proxy by handling the authentication cookie
        ''' </summary>
        ''' <param name="uri">The uri of the remote web service to access</param>
        ''' <returns>Instance of System.Net.WebRequest</returns>
        ''' <remarks></remarks>
        Protected Overrides Function GetWebRequest(uri As System.Uri) As System.Net.WebRequest

            Dim request As HttpWebRequest = CType(HttpWebRequest.Create(uri), HttpWebRequest)
            request.CookieContainer = New CookieContainer()

            If AuthCookie IsNot Nothing Then
                request.CookieContainer.Add(AuthCookie)
            End If

            Return request

        End Function

        ''' <summary>
        ''' Overrides the web response method in the web service proxy by handling the authentication cookie
        ''' </summary>
        ''' <param name="request">The web request that this is the response object for</param>
        ''' <returns>Instance of System.Net.WebResponse</returns>
        ''' <remarks></remarks>
        Protected Overrides Function GetWebResponse(request As System.Net.WebRequest) As System.Net.WebResponse

            Dim response As WebResponse = MyBase.GetWebResponse(request)
            Dim cookieName As String = response.Headers("RSAuthenticationHeader")
            If Not String.IsNullOrEmpty(cookieName) Then

                Dim webResponse As HttpWebResponse = CType(response, HttpWebResponse)
                _authCookie = webResponse.Cookies(cookieName)

            End If

            Return response

        End Function

    End Class

End Namespace