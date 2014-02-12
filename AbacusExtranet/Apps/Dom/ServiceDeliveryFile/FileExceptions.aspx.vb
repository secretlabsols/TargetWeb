Imports System.Configuration.ConfigurationSettings
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Xml.Xsl
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.FileStore

Namespace Apps.Dom.ServiceDeliveryFile

    ''' <summary>
    ''' Screen to allow a user to view the exceptions found when a service delivery file was processed.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class FileExceptions
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ViewServiceDeliveryFile"), "View Service Delivery File Exceptions")

            Dim exceptionFileID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim remittanceXml As XmlDocument = Nothing
            Dim msg As ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim interfaceFile As MemoryStream = Nothing
            Dim xmlDoc As XmlDocument = New XmlDocument
            Dim fileData As WebFileStoreData = Nothing
            Dim strStyle As New StringBuilder

            Me.AddExtraCssStyle(strStyle.ToString)

            ' get the remittance xml
            fileData = New WebFileStoreData(Me.DbConnection)
            msg = fileData.Fetch(exceptionFileID)
            If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            interfaceFile = New MemoryStream(fileData.Data)
            xmlDoc.Load(interfaceFile)

            ' transform using the XSLT and output to the page
            Dim args As XsltArgumentList = New XsltArgumentList
            ' add the extension utility object
            Dim obj As XsltExtensionUtil = New XsltExtensionUtil
            args.AddExtensionObject("urn:ext-util", obj)
            divExceptions.InnerHtml = Target.Library.Utils.TransformXML(xmlDoc.DocumentElement.OuterXml, _
                Request.MapPath(Me.Settings("DomViewServiceDeliveryExceptionsXsltUrl")), _
                args)
        End Sub

    End Class
End Namespace