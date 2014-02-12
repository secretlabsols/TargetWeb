
Imports System.Configuration.ConfigurationManager
Imports System.IO
Imports System.Text
Imports System.Xml.Serialization
Imports System.Xml.Xsl
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.FileStore
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps.SUNotif

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.SUNotif.ViewSUNotif
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen that allow a user to view/print a single service user notification.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	20/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewSUNotif
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPSUNotifsView"), "View Service User Notification")

            Dim suNotifID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("suNotifID"))
            Dim notifXml As String = String.Empty
            Dim msg As ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim canView As Boolean
            Dim notif As ViewableSUNotif = Nothing
            Dim serialiser As XmlSerializer
            Dim memStream As MemoryStream

            ' check user can view the notif?
            If SecurityBL.UserHasItem(Me.DbConnection, user.ID, ConstantsManager.GetConstant("webSecurityItemSPSUNotifsProcess")) Then
                canView = True
            Else
                msg = SPClassesBL.UserCanViewSUNotif(Me.DbConnection, user.ExternalUserID, suNotifID, canView)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If
            If Not canView Then Response.Redirect("~/Libaray/Errors/AccessDenied.aspx")

            Me.CssLinks.Add("ViewSUNotif.css")

            ' fetch the notif
            msg = SPClassesBL.FetchViewableSUNotif(Me.DbConnection, suNotifID, notif)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' show the view scanned button?
            If notif.WebFileStoreDataID = 0 Then
                btnViewScanned.Visible = False
            Else
                btnViewScanned.Attributes.Add("onclick", _
                    String.Format("document.location.href = '{0}?fileDataID={1}';", _
                        WebUtils.GetVirtualPath("Apps/FileStore/FileStoreGetFile.axd"), notif.WebFileStoreDataID))
            End If

            ' get the notif xml
            serialiser = New XmlSerializer(GetType(ViewableSUNotif))
            memStream = New MemoryStream
            serialiser.Serialize(memStream, notif)
            notifXml = Encoding.UTF8.GetString(memStream.ToArray())
            memStream.Close()

            ' transform using the XSLT and output to the page
            Dim args As XsltArgumentList = New XsltArgumentList
            ' add the processed parameter
            args.AddParam("processed", String.Empty, IIf(notif.StatusID = SUNotifStatus.Accepted Or notif.StatusID = SUNotifStatus.Declined, 1, 0))
            ' add the extension utility object
            Dim obj As XsltExtensionUtil = New XsltExtensionUtil
            args.AddExtensionObject("urn:ext-util", obj)
            divNotif.InnerHtml = Target.Library.Utils.TransformXML(notifXml, Request.MapPath("ViewSUNotif.xslt"), args)

        End Sub

    End Class

End Namespace