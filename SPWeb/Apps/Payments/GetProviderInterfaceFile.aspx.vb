
Imports System.Configuration.ConfigurationManager
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps.Payments

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.Payments.GetProviderInterfaceFile
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Allows a user to open/download a provider or service user payment file.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	12/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class GetProviderInterfaceFile
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False

            Dim msg As ErrorMessage
            Dim user As WebSecurityUser
            Dim canView As Boolean
            Dim securityConst As String
            Dim fileContents As Byte() = Nothing
            Dim fileName As String = Nothing
            Dim logID As Integer
            Dim format As String
            Dim serviceID As Integer
            Dim clientID As Integer
            Dim dateFrom As Date, dateTo As Date

            logID = Target.Library.Utils.ToInt32(Request.QueryString("logID"))
            format = Request.QueryString("format")
            serviceID = Target.Library.Utils.ToInt32(Request.QueryString("serviceID"))
            clientID = Target.Library.Utils.ToInt32(Request.QueryString("clientID"))
            If Target.Library.Utils.IsDate(Request.QueryString("dateFrom")) Then dateFrom = Convert.ToDateTime(Request.QueryString("dateFrom"))
            If Target.Library.Utils.IsDate(Request.QueryString("dateTo")) Then dateTo = Convert.ToDateTime(Request.QueryString("dateTo"))

            If logID > 0 Then
                ' called from provider interface screen
                securityConst = "webSecurityItemSPPaymentsInterface"
            Else
                ' called from service user payment enquiry screen
                securityConst = "webSecurityItemSPPaymentsSUPaymentEnquiry"
            End If

            Me.InitPage(ConstantsManager.GetConstant(securityConst), "Payment File")

            user = SecurityBL.GetCurrentUser()

            If logID > 0 Then
                ' check user can view the interface log
                msg = SPClassesBL.UserCanViewProviderInterfaceFile(Me.DbConnection, user.ExternalUserID, logID, canView)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If Not canView Then Response.Redirect("~/Libaray/Errors/AccessDenied.aspx")
            Else
                ' should really check user is allowed to see the client (via the service ID)? - might have to do this when populating?
            End If

            If logID > 0 Then
                ' we are after an entire payment interface
                Select Case format
                    Case "cap"
                        msg = SPClassesBL.GetCapProviderInterfaceFile( _
                                                    Me.DbConnection, _
                                                    ConnectionStrings("Abacus").ConnectionString, _
                                                    AppSettings("SiteID"), _
                                                    user.ExternalUsername, _
                                                    logID, _
                                                    fileContents, _
                                                    fileName)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                    Case "target"

                        msg = SPClassesBL.GetTargetProviderInterfaceFile( _
                                                        Me.DbConnection, _
                                                        ConnectionStrings("Abacus").ConnectionString, _
                                                        AppSettings("SiteID"), _
                                                        user.ExternalUsername, _
                                                        user.ID, _
                                                        user.ExternalUserID, _
                                                        logID, _
                                                        serviceID, _
                                                        clientID, _
                                                        dateFrom, _
                                                        dateTo, _
                                                        fileContents, _
                                                        fileName)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                    Case Else
                        Throw New ArgumentOutOfRangeException("format", format, "Invalid payment file format requested.")
                End Select

            Else
                ' we are after a file of remittance lines for a service/client/date range
                ' NOTE: only available in Target format
                If format <> "target" Then
                    Throw New ArgumentOutOfRangeException("format", format, "Invalid payment file format requested.")
                Else

                    msg = SPClassesBL.GetTargetProviderInterfaceFile( _
                                                        Me.DbConnection, _
                                                        ConnectionStrings("Abacus").ConnectionString, _
                                                        AppSettings("SiteID"), _
                                                        user.ExternalUsername, _
                                                        user.ID, _
                                                        user.ExternalUserID, _
                                                        logID, _
                                                        serviceID, _
                                                        clientID, _
                                                        dateFrom, _
                                                        dateTo, _
                                                        fileContents, _
                                                        fileName)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                End If

            End If

            ' output the file
            If Not fileContents Is Nothing AndAlso fileContents.Length > 0 Then
                With Response
                    .Clear()
                    .ContentType = "text/comma-separated-values"
                    .Charset = "iso-8859-1"
                    .AddHeader("Content-Length", fileContents.Length)
                    .AddHeader("Content-Disposition", "attachment; filename=" & fileName)
                    .BinaryWrite(fileContents)
                    .End()
                End With
            Else
                Throw New ApplicationException("The payment file returned is empty.")
            End If

        End Sub

    End Class

End Namespace