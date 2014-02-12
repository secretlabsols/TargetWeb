
Imports System.ComponentModel
Imports System.Configuration.Install
Imports System.Reflection.Assembly
Imports Target.Library

<RunInstaller(True)> Public Class ProjectInstaller
    Inherits System.Configuration.Install.Installer

    Private Const APP_NAME As String = "AbacusWeb"
    Private Const CONFIG_FILE As String = "..\web.config"

#Region " Component Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Installer overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    End Sub

#End Region

    Public Overrides Sub Install(ByVal stateSaver As System.Collections.IDictionary)

        Dim msg As ErrorMessage

        Try
            'Let the project installer do its job
            MyBase.Install(stateSaver)

            msg = Utils.CreateStdInstallRegKeys(APP_NAME, GetExecutingAssembly(), CONFIG_FILE)
            If Not msg.Success Then Throw New InstallException(msg.ToString())

        Catch ex As Exception
            Throw New InstallException("An exception was thrown during installation:\n" + ex.ToString())
        End Try
    End Sub

    Public Overrides Sub Uninstall(ByVal savedState As System.Collections.IDictionary)

        Dim msg As ErrorMessage

        Try
            msg = Utils.DeleteStdInstallRegKeys(APP_NAME)
            If Not msg.Success Then Throw New InstallException(msg.ToString())

        Catch ex As Exception
            Throw New InstallException("An exception was thrown during uninstallation:\n" + ex.ToString())
        Finally
            'Let the project installer do its job
            MyBase.Uninstall(savedState)
        End Try

    End Sub

End Class
