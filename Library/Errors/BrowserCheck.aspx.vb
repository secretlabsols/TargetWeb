Imports System.Configuration.ConfigurationManager
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.HttpModules.BrowserCheck
Imports Target.Web.Apps.Security
Imports Target.Library.Web.Configuration.BrowserCheckConfiguration

Namespace Library.Errors

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Library.Errors.BrowserCheck
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Page to displays when an unsupported browser is encountered.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MoTahir     04-09-2013 Incompatible browser message being shown incorrectly when using an IE version which is in ‘Compatibility Mode’
    '''     ColinD      22/08/2012  #7507 - Added a call to SecurityBL.CheckIsLocalUrl within PopulateCompatibleBrowsersList to guard against open redirection attacks.
    '''     ColinD      01/08/2012  #7480 - Call Server.HtmlEncode to remove the risk of Cross Site Scripting Attacks.
    '''     ColinD      21/07/2011  D12150 - Updated to fetch compatible browsers from business logic classes.
    ''' 	[Mikevo]	25/07/2007	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class BrowserCheck
        Inherits Target.Web.Apps.BasePage

#Region "Fields"

        Private Const _QsURL As String = "url"
        Private Const COSNT_STRONG_CLOSING_TAG As String = "</strong>"
        Private Const CONST_SECOND_OCCURENCE As Integer = 2

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the URL from the query string.
        ''' </summary>
        ''' <value>Gets the URL from the query string.</value>
        Private ReadOnly Property QsURL() As String
            Get
                Return Request.QueryString(_QsURL)

            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' init the page and setup additional bits
            InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Incompatible Browser")
            RenderMenu = SecurityBL.IsUserLoggedOn()
            EnableTimeout = False
            AddExtraCssStyle(String.Format(CSS_TEXT_SIZE_TEMPLATE, Session(SESSION_TEXT_SIZE)))

            ' populate the list of compatible browsers
            PopulateCompatibleBrowsersList()

            ' populate current browser information
            PopulateCurrentBrowser()

        End Sub

#End Region

#Region "Functions/Methods"

        ''' <summary>
        ''' Populates the compatible browsers list.
        ''' </summary>
        Private Sub PopulateCompatibleBrowsersList()

            Dim browserCheckSettings As BrowserCheckConfigurationSection = BrowserCheckModule.BrowserCheckConfigurationSection

            ' check that token identified in _QsURL is a valid url
            SecurityBL.CheckIsLocalUrl(Request, _QsURL)

            ' populate list with html
            litSupportedBrowsers.Text = browserCheckSettings.GetApplicationConfigurationElementBrowsersAsHTML(BrowserCheckModule.AbacusApplication)

            ' setup the url which allows users to continue if allow to do so
            lnkContinue.HRef = Server.HtmlEncode(HttpUtility.UrlDecode(QsURL))
            divCanContinue.Visible = browserCheckSettings.AllowNotSupported

        End Sub

        ''' <summary>
        ''' Populates the current browser.
        ''' </summary>
        Private Sub PopulateCurrentBrowser()

            Dim formattedString As String = litSupportedBrowsers.Text
            Dim tagPosition As Integer
            tagPosition = Target.Library.Utils.IndexOfNthWord(formattedString, COSNT_STRONG_CLOSING_TAG, CONST_SECOND_OCCURENCE)
            formattedString = formattedString.Insert(tagPosition, " running on ")

            litCurrentBrowser.Text = String.Format("<br/>Your browser has been detected as either running in ‘Compatibility View’ or using a version other than {0} ", formattedString)
            litUserAgent.Text = Server.HtmlEncode(Request.UserAgent)


            ' contact link
            lnkContact.HRef = String.Format("mailto:{1}?subject={2}: Incompatible Browser&amp;body=Url: {3}{0}Detected Browser: {4}{0}User Agent: {5}", _
                "%0D%0A", _
                Settings("SystemEmailAddress"), _
                Settings("SiteName"), _
                Server.HtmlEncode(Request.Url.ToString()), _
                String.Format("{0} - {1} - {2}", Server.HtmlEncode(Request.Browser.Browser), Server.HtmlEncode(Request.Browser.Version), Server.HtmlEncode(Request.Browser("os"))), _
                Server.HtmlEncode(Request.UserAgent) _
            )

        End Sub

#End Region

    End Class

End Namespace