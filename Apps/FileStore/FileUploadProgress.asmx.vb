
Imports System.Web.Services
Imports Krystalware.SlickUpload
Imports Krystalware.SlickUpload.Status

Namespace Apps.FileStore

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.FileStore.FileUploadProgress
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to retrieve ClickUpload file upload progress information.
    ''' </summary>
    ''' <remarks>
    '''     NOTE this is currently not used as there are issues with the call not returning 
    '''     when called via AjaxPro.NET, hence progress information is not returned in a
    '''     timely fashion. Currently using the HttpHandler and Sarissa library method.
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      26/11/2008  Support for new version of SlickUpload.
    ''' 	[Mikevo]	01/12/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/Apps/FileStore/FileUploadProgress")> _
    Public Class FileUploadProgress
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Get the progress of the specified upload from the SlickUpload module.
        ''' </summary>
        ''' <param name="uploadID">The GUID used to identify the upload.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	21/06/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(), AjaxPro.AjaxMethod()> _
        Public Function GetProgress(ByVal uploadID As String) As UploadProgress

            Dim status As UploadStatus = HttpUploadModule.GetUploadStatus(uploadID)
            Dim progress As UploadProgress = New UploadProgress

            If Not status Is Nothing Then
                With progress
                    .State = status.State.ToString()
                    .PositionRaw = status.Position.ToString()
                    .ContentLengthRaw = status.ContentLength.ToString()
                    .RemainingLengthRaw = (status.ContentLength - status.Position).ToString()
                    .TransferredLengthRaw = (status.ContentLength - (status.ContentLength - status.Position)).ToString()
                    .PositionText = FormatBytes(status.Position)
                    .ContentLengthText = FormatBytes(status.ContentLength)
                    .RemainingLengthText = FormatBytes(status.ContentLength - status.Position)
                    .TransferredLengthText = FormatBytes(status.ContentLength - (status.ContentLength - status.Position))
                    If status.State = UploadState.Complete OrElse status.State = UploadState.ReceivingData Then
                        Dim elapsed As TimeSpan = DateTime.Now.Subtract(status.Start)
                        .ElapsedTimeText = elapsed.Minutes & " mins " & elapsed.Seconds & " secs"
                        Dim remaining As TimeSpan = New TimeSpan(((elapsed.Ticks / status.Position) * (status.ContentLength - status.Position) + elapsed.Ticks))
                        .RemainingTimeText = remaining.Minutes & " mins " & remaining.Seconds & " secs"
                    End If
                End With
            End If

            Return progress

        End Function

        Private Function FormatBytes(ByVal bytes As Long) As String
            Const ONE_KB As Double = 1024
            Const ONE_MB As Double = ONE_KB * 1024
            Const ONE_GB As Double = ONE_MB * 1024
            Const ONE_TB As Double = ONE_GB * 1024
            Const ONE_PB As Double = ONE_TB * 1024
            Const ONE_EB As Double = ONE_PB * 1024
            Const ONE_ZB As Double = ONE_EB * 1024
            Const ONE_YB As Double = ONE_ZB * 1024

            If bytes <= 999 Then
                Return bytes.ToString() & " bytes"
            ElseIf bytes <= ONE_KB * 999 Then
                Return ThreeNonZeroDigits(bytes / ONE_KB) & " KB"
            ElseIf bytes <= ONE_MB * 999 Then
                Return ThreeNonZeroDigits(bytes / ONE_MB) & " MB"
            ElseIf bytes <= ONE_GB * 999 Then
                Return ThreeNonZeroDigits(bytes / ONE_GB) & " GB"
            ElseIf bytes <= ONE_TB * 999 Then
                Return ThreeNonZeroDigits(bytes / ONE_TB) & " TB"
            ElseIf bytes <= ONE_PB * 999 Then
                Return ThreeNonZeroDigits(bytes / ONE_PB) & " PB"
            ElseIf bytes <= ONE_EB * 999 Then
                Return ThreeNonZeroDigits(bytes / ONE_EB) & " EB"
            ElseIf bytes <= ONE_ZB * 999 Then
                Return ThreeNonZeroDigits(bytes / ONE_ZB) & " ZB"
            Else
                Return ThreeNonZeroDigits(bytes / ONE_YB) & " YB"
            End If
        End Function

        Private Function ThreeNonZeroDigits(ByVal value As Double) As String
            If value >= 100 Then
                Return CType(value, Integer).ToString()
            ElseIf value >= 10 Then
                Return value.ToString("0.0")
            Else
                Return value.ToString("0.00")
            End If
        End Function

    End Class

    Public Structure UploadProgress
        Public State As String
        Public PositionRaw As String
        Public ContentLengthRaw As String
        Public RemainingLengthRaw As String
        Public TransferredLengthRaw As String
        Public PositionText As String
        Public ContentLengthText As String
        Public RemainingLengthText As String
        Public TransferredLengthText As String
        Public ElapsedTimeText As String
        Public RemainingTimeText As String
    End Structure

End Namespace
