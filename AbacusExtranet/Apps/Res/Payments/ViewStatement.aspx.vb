
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
    ''' Class	 : Abacus.Extranet.Apps.Res.Payments.ViewStatement
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen that allow a user to view/print a statement of payments for a 
    '''     single service user.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      19/01/2009  A4WA#5161.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	13/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewStatement
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ServiceUserPaymentEnquiry"), "View Statement")

            Dim establishmentID As Integer, clientID As Integer
            Dim dateFrom As Date, dateTo As Date
            Dim statementXml As XmlDocument = Nothing
            Dim msg As ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim canView As Boolean

            establishmentID = Target.Library.Utils.ToInt32(Request.QueryString("estabID"))
            clientID = Target.Library.Utils.ToInt32(Request.QueryString("clientID"))
            If Not Request.QueryString("dateFrom") Is Nothing AndAlso Target.Library.Utils.IsDate(Request.QueryString("dateFrom")) Then
                dateFrom = Convert.ToDateTime(Request.QueryString("dateFrom"))
            End If
            If Not Request.QueryString("dateTo") Is Nothing AndAlso Target.Library.Utils.IsDate(Request.QueryString("dateTo")) Then
                dateTo = Convert.ToDateTime(Request.QueryString("dateTo"))
            End If

            ' check user can view the client
            msg = AbacusClassesBL.UserCanViewClient(Me.DbConnection, user.ExternalUserID, clientID, UserCanViewClientCheckType.ResidentialClients, canView)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Not canView Then WebUtils.DisplayAccessDenied()

            ' get the remittance xml
            msg = AbacusClassesBL.FetchSUStatementXml(Me.DbConnection, user.ExternalUserID, establishmentID, clientID, dateFrom, dateTo, statementXml)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' transform using the XSLT and output to the page
            Dim args As XsltArgumentList = New XsltArgumentList
            ' add the extension utility object
            Dim obj As XsltExtensionUtil = New XsltExtensionUtil
            args.AddExtensionObject("urn:ext-util", obj)
            divRemittance.InnerHtml = Target.Library.Utils.TransformXML(statementXml.DocumentElement.OuterXml, _
                Request.MapPath(Me.Settings("SUResPaymentEnquiryStatementXsltUrl")), _
                args)

        End Sub

    End Class

End Namespace