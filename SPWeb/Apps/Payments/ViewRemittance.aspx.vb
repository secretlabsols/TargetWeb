
Imports System.Configuration.ConfigurationManager
Imports System.Xml.Xsl
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.FileStore
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps.Payments

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.Payments.ViewRemittance
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen that allow a user to view/print a single remittance.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      22/03/2007  Support for Xslt, Css and logo paths stored in ApplicationSettings.
    '''     MikevO      17/11/2006  Fix to access denied path.
    ''' 	[Mikevo]	09/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewRemittance
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPPaymentsViewRemittance"), "View Remittance")

            Dim remittanceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim remittanceXml As String = String.Empty
            Dim msg As ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim canView As Boolean

            ' check user can view the remittance
            msg = SPClassesBL.UserCanViewRemittance(Me.DbConnection, user.ExternalUserID, remittanceID, canView)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Not canView Then Response.Redirect("~/Library/Errors/AccessDenied.aspx")

            Me.CssLinks.Add(Me.Settings("PaymentEnquiryRemittanceCssUrl"))

            ' get the remittance xml
            msg = SPClassesBL.FetchRemittanceXml(ConnectionStrings("Abacus").ConnectionString, remittanceID, remittanceXml)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' transform using the XSLT and output to the page
            Dim args As XsltArgumentList = New XsltArgumentList
            ' add the logo param
            args.AddParam("logoUrl", String.Empty, Me.Settings("PaymentEnquiryRemittanceHeaderLogoUrl"))
            ' add the extension utility object
            Dim obj As XsltExtensionUtil = New XsltExtensionUtil
            args.AddExtensionObject("urn:ext-util", obj)
            divRemittance.InnerHtml = Target.Library.Utils.TransformXML(remittanceXml, _
                Request.MapPath(Me.Settings("PaymentEnquiryRemittanceXsltUrl")), _
                args)

        End Sub

    End Class

End Namespace