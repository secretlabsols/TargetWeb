
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
    ''' Class	 : SP.Web.Apps.Payments.ViewStatement
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen that allow a user to view/print a service user statement.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      22/03/2007  Support for Xslt, Css and logo paths stored in ApplicationSettings.
    ''' 	[Mikevo]	12/03/2007	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewStatement
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPPaymentsViewRemittance"), "View Statement")

            Dim serviceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("serviceID"))
            Dim clientID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("clientID"))
            Dim dateFrom As Date, dateTo As Date
            Dim statementXml As String = String.Empty
            Dim msg As ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()

            serviceID = Target.Library.Utils.ToInt32(Request.QueryString("serviceID"))
            clientID = Target.Library.Utils.ToInt32(Request.QueryString("clientID"))
            If Not Request.QueryString("dateFrom") Is Nothing AndAlso Target.Library.Utils.IsDate(Request.QueryString("dateFrom")) Then
                dateFrom = Convert.ToDateTime(Request.QueryString("dateFrom"))
            End If
            If Not Request.QueryString("dateTo") Is Nothing AndAlso Target.Library.Utils.IsDate(Request.QueryString("dateTo")) Then
                dateTo = Convert.ToDateTime(Request.QueryString("dateTo"))
            End If

            Me.CssLinks.Add(Me.Settings("SUPaymentEnquiryStatementCssUrl"))

            ' get the statement xml
            msg = SPClassesBL.FetchStatementXml(Me.DbConnection, serviceID, clientID, user.ExternalUserID, dateFrom, dateTo, statementXml)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' transform using the XSLT and output to the page
            Dim args As XsltArgumentList = New XsltArgumentList
            ' add the logo param
            args.AddParam("logoUrl", String.Empty, Me.Settings("SUPaymentEnquiryStatementHeaderLogoUrl"))
            ' add the extension utility object
            Dim obj As XsltExtensionUtil = New XsltExtensionUtil
            args.AddExtensionObject("urn:ext-util", obj)
            divStatement.InnerHtml = Target.Library.Utils.TransformXML(statementXml, _
                Request.MapPath(Me.Settings("SUPaymentEnquiryStatementXsltUrl")), _
                args)

        End Sub

    End Class

End Namespace