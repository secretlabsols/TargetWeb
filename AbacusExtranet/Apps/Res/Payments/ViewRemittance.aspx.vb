
Imports System.Configuration.ConfigurationSettings
Imports System.Xml
Imports System.Xml.Xsl
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security

Namespace Apps.Res.Payments

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.Res.Payments.ViewRemittance
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen that allow a user to view/print a single remittance.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	07/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewRemittance
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentEnquiry"), "View Remittance")

            Dim remittanceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim remittanceXml As XmlDocument = Nothing
            Dim msg As ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim canView As Boolean

            ' check user can view the remittance
            msg = AbacusClassesBL.UserCanViewRemittance(Me.DbConnection, user.ExternalUserID, remittanceID, canView)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Not canView Then WebUtils.DisplayAccessDenied()

            ' get the remittance xml
            msg = AbacusClassesBL.FetchRemittanceXml(Me.DbConnection, remittanceID, remittanceXml)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' transform using the XSLT and output to the page
            Dim args As XsltArgumentList = New XsltArgumentList
            ' add the extension utility object
            Dim obj As XsltExtensionUtil = New XsltExtensionUtil
            args.AddExtensionObject("urn:ext-util", obj)
            divRemittance.InnerHtml = Target.Library.Utils.TransformXML(remittanceXml.DocumentElement.OuterXml, _
                Request.MapPath(Me.Settings("ResPaymentEnquiryRemittanceXsltUrl")), _
                args)

        End Sub

    End Class

End Namespace