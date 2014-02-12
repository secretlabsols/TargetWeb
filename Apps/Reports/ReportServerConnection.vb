
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Net
Imports Microsoft.Reporting.WebForms
Imports Target.Library

Namespace Apps.Reports

    ''' <summary>
    ''' Encapsulates connection to remote reporting services instance
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ReportServerConnection

        Private Shared _configSection As NameValueCollection

        ''' <summary>
        ''' Url of the remote reporting services instance
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property ReportServerUrl() As System.Uri
            Get
                Dim uri As String = GetConfigurationSection()("BaseUrl")
                If String.IsNullOrEmpty(uri) Then
                    Throw New Exception("Missing or zero value 'reportServer/BaseUrl' from the Web.config file")
                End If
                Return New Uri(uri)
            End Get
        End Property

        ''' <summary>
        ''' Base path to the reports held on the server. E.g. "/reports/"
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property ReportServerBasePath() As String
            Get
                Dim path As String = GetConfigurationSection()("BasePath")
                If String.IsNullOrEmpty(path) Then
                    Throw New Exception("Missing or zero value 'reportServer/BasePath' from the Web.config file")
                End If
                Return path
            End Get
        End Property

        ''' <summary>
        ''' Timeout for any reports on the report server
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Timeout() As Integer
            Get
                Dim timeoutValue As Integer = Utils.ToInt32(GetConfigurationSection()("TimeOut"))
                If timeoutValue = 0 Then
                    Throw New Exception("Missing or zero value 'reportServer/TimeOut' from the Web.config file")
                End If
                Return timeoutValue
            End Get
        End Property

        ''' <summary>
        ''' Username for authentication to the report server
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Username() As String
            Get
                Dim user As String = GetConfigurationSection()("UserName")
                If String.IsNullOrEmpty(user) Then
                    Throw New Exception("Missing or empty value 'reportServer/UserName' from the Web.config file")
                End If
                Return user
            End Get
        End Property

        ''' <summary>
        ''' Password for authentication to the report server
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property Password() As String
            Get
                Dim pw As String = GetConfigurationSection()("UserPassword")
                If String.IsNullOrEmpty(pw) Then
                    Throw New Exception("Missing or empty value 'reportServer/UserPassword' from the Web.config file")
                End If
                Return pw
            End Get
        End Property

        ''' <summary>
        ''' The uri to the ReportExecution2005.asmx web service on the server, used for authentication
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property ServiceUri() As String
            Get
                Dim suri As String = GetConfigurationSection()("ServiceUri")
                If String.IsNullOrEmpty(suri) Then
                    Throw New Exception("Missing or empty value 'reportServer/ServiceUri' from the web.config.file")
                End If
                Return suri
            End Get
        End Property

        ''' <summary>
        ''' Gets the reportServer section from web.config
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetConfigurationSection() As NameValueCollection

            If _configSection IsNot Nothing Then
                Return _configSection
            End If

            _configSection = ConfigurationManager.GetSection("reportServer")
            If _configSection Is Nothing Then
                Throw New Exception("Missing 'reportServer' section from the Web.config file")
            End If

            Return _configSection

        End Function

    End Class

End Namespace
