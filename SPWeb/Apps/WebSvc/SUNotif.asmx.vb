
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Web.Services
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.WebSvc.SUNotif
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web service to allow interface with the service user notifications application.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	23/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/TargetWeb/SPWeb/Apps/WebSvc/SUNotif")> _
    Public Class SUNotif
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

#Region " FetchSUNotifList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves a list of service user notifications for the specified filters.
        ''' </summary>
        ''' <param name="page">The current page in the results.</param>
        ''' <param name="fromDate">The date from filter.</param>
        ''' <param name="toDate">The date to filter.</param>
        ''' <param name="status">The status filter.</param>
        ''' <param name="externalUserID">The external user filter.</param>
        ''' <param name="listFilterReference">The custom list filter string to apply to the reference column.</param>
        ''' <param name="listFilterServiceUser">The custom list filter string to apply to the service user column.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        '''     MikeVO      06/03/2007  Allow user to navigate back to enter details step once notif has been created.
        '''     MikeVO      14/12/2006  Added support for Reference and Service User list filters.
        ''' 	[Mikevo]	23/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchSUNotifList(ByVal page As Integer, _
                                        ByVal fromDate As Date, ByVal toDate As Date, _
                                        ByVal status As Byte, ByVal externalUserID As Integer, _
                                        ByVal listFilterReference As String, ByVal listFilterServiceUser As String) As FetchSUNotifListResult

            Const STEP_INDEX_PRINT_DOC As Integer = 3

            Dim msg As ErrorMessage
            Dim notifs As ArrayList = Nothing
            Dim result As FetchSUNotifListResult = New FetchSUNotifListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim user As WebSecurityUser
            Dim isAdminUser As Boolean

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                user = SecurityBL.GetCurrentUser()

                ' get the notification list
                msg = SPClassesBL.FetchSUNotifications( _
                                    conn, _
                                    fromDate, _
                                    toDate, _
                                    status, _
                                    externalUserID, _
                                    page, _
                                    pageSize, _
                                    listFilterReference, _
                                    listFilterServiceUser, _
                                    totalRecords, _
                                    notifs)

                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' build the url to use for each notif
                isAdminUser = SecurityBL.UserHasItem(conn, user.ID, Target.Library.Web.ConstantsManager.GetConstant("webSecurityItemSPSUNotifsProcess"))
                For Each notif As ViewableListedSUNotif In notifs
                    If isAdminUser Then
                        If notif.StatusID = SUNotifStatus.Submitted Then
                            notif.LinkUrl = String.Format("Admin/ProcessSUNotif.aspx?suNotifID={0}", notif.ID)
                        ElseIf notif.TypeID = SUNotifType.NewNotification Then
                            notif.LinkUrl = String.Format("ViewSUNotif.aspx?suNotifID={0}", notif.ID)
                            notif.LinkInNewWindow = True
                        Else
                            notif.LinkUrl = String.Format("../ListSubsidies/ViewSubsidy.aspx?id={0}", notif.SPSubsidyAgreementID)
                        End If
                    Else
                        If notif.StatusID = SUNotifStatus.Provisional Then
                            notif.LinkUrl = String.Format("NewSUNotif.aspx?suNotifID={0}&currentStep={1}", notif.ID, STEP_INDEX_PRINT_DOC)
                        ElseIf notif.TypeID = SUNotifType.NewNotification Then
                            notif.LinkUrl = String.Format("ViewSUNotif.aspx?suNotifID={0}", notif.ID)
                            notif.LinkInNewWindow = True
                        Else
                            notif.LinkUrl = String.Format("../ListSubsidies/ViewSubsidy.aspx?id={0}", notif.SPSubsidyAgreementID)
                        End If
                    End If
                Next

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Notifications = notifs
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchNotifList({0})"" title=""{2}"">{1}</a>&nbsp;", _
                        page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, "E0001")   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

    End Class

End Namespace