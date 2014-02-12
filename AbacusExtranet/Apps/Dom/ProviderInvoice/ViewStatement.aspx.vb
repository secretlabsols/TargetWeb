
Imports System.Configuration.ConfigurationSettings
Imports System.Xml
Imports System.Xml.Xsl
Imports Target.Abacus.Library
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security

Namespace Apps.Dom.ProviderInvoice

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.Dom.ProviderInvoice.ViewStatement
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen that allow a user to view/print a statement of payments for a 
    '''     single domciliary contract, optionally filtered by various criteria.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' 	[Mikevo]	25/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewStatement
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ProviderInvoiceEnquiry"), "View Statement")

            Dim providerID As Integer, contractID As Integer, clientID As Integer
            Dim invoiceNumber As String = Nothing
            Dim weekendingFrom As Date, weekendingTo As Date
            Dim unPaid As Boolean = True, paid As Boolean = True, authorised As Boolean = True, suspended As Boolean = True
            Dim statusDateFrom As Date, statusDateTo As Date
            Dim statementXml As XmlDocument = Nothing
            Dim msg As ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim canView As Boolean

            providerID = Utils.ToInt32(Request.QueryString("estabID"))
            contractID = Utils.ToInt32(Request.QueryString("contractID"))
            clientID = Utils.ToInt32(Request.QueryString("clientID"))
            If Not Request.QueryString("invoiceNumber") Is Nothing AndAlso Request.QueryString("invoiceNumber").Trim.Length > 0 Then
                invoiceNumber = Request.QueryString("invoiceNumber")
            End If
            If Utils.IsDate(Request.QueryString("weFrom")) Then weekendingFrom = Convert.ToDateTime(Request.QueryString("weFrom"))
            If Utils.IsDate(Request.QueryString("weTo")) Then weekendingTo = Convert.ToDateTime(Request.QueryString("weTo"))
            If Not Request.QueryString("unPaid") Is Nothing AndAlso Request.QueryString("unPaid").Trim.Length > 0 Then
                unPaid = (Request.QueryString("unPaid").ToLower() = Boolean.TrueString.ToLower())
            End If
            If Not Request.QueryString("paid") Is Nothing AndAlso Request.QueryString("paid").Trim.Length > 0 Then
                paid = (Request.QueryString("paid").ToLower() = Boolean.TrueString.ToLower())
            End If
            If Not Request.QueryString("authorised") Is Nothing AndAlso Request.QueryString("authorised").Trim.Length > 0 Then
                authorised = (Request.QueryString("authorised").ToLower() = Boolean.TrueString.ToLower())
            End If
            If Not Request.QueryString("suspended") Is Nothing AndAlso Request.QueryString("suspended").Trim.Length > 0 Then
                suspended = (Request.QueryString("suspended").ToLower() = Boolean.TrueString.ToLower())
            End If
            If Utils.IsDate(Request.QueryString("dateFrom")) Then statusDateFrom = Convert.ToDateTime(Request.QueryString("dateFrom"))
            If Utils.IsDate(Request.QueryString("dateTo")) Then statusDateTo = Convert.ToDateTime(Request.QueryString("dateTo"))

            ' check user can view the contract?
            msg = DomContractBL.UserCanViewContract(Me.DbConnection, user.ExternalUserID, contractID, canView)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Not canView Then WebUtils.DisplayAccessDenied()

            ' get the statement xml
            msg = DomContractBL.FetchProviderInvoiceStatementXml(Me.DbConnection, user.ExternalUserID, _
                providerID, contractID, clientID, unPaid, paid, authorised, suspended, invoiceNumber, _
                weekendingFrom, weekendingTo, statusDateFrom, statusDateTo, statementXml)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' transform using the XSLT and output to the page
            Dim args As XsltArgumentList = New XsltArgumentList
            ' add the extension utility object
            Dim obj As XsltExtensionUtil = New XsltExtensionUtil
            args.AddExtensionObject("urn:ext-util", obj)
            divStatement.InnerHtml = Utils.TransformXML(statementXml.DocumentElement.OuterXml, _
                Request.MapPath(Me.Settings("DomProviderInvoiceStatementXsltUrl")), _
                args)

        End Sub

    End Class

End Namespace