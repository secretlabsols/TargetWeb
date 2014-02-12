Imports System.Text
Imports Target.Library.Web
Imports Target.Library.Web.Configuration.BrowserCheckConfiguration
Imports Target.Web.Apps.Security
Imports Target.Library.Web.HttpModules.BrowserCheck

''' -----------------------------------------------------------------------------
''' Project	 : Target.Web
''' Class	 : Web.Accessibility
''' 
''' -----------------------------------------------------------------------------
''' <summary>
'''     Displays accessibility information for the site.
''' </summary>
''' <remarks>
''' </remarks>
''' <history><![CDATA[
'''     ColinD      21/07/2011  D12150 - Updated to fetch compatible browsers from business logic classes.
'''     MikeVO      14/09/2009  D11602 - menu improvements.
'''     MikeVO      01/12/2008  D11444 - security overhaul.
''' 	[Mikevo]	??/??/????	Created
''' ]]></history>
''' -----------------------------------------------------------------------------
Partial Class Accessibility
    Inherits Target.Web.Apps.BasePage

#Region "Events"

    ''' <summary>
    ''' Handles the Load event of the Page control.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Accessibility Statement")
        lblYourBrowser.Text = String.Format("{0} version {1} running on {2}", _
                                            Convert.ToString(Request.Browser.Browser), _
                                            Convert.ToString(Request.Browser.Version), _
                                            Convert.ToString(Request.Browser("os")))
        RenderMenu = SecurityBL.IsUserLoggedOn()
        PopulateCompatibleBrowsersList()

    End Sub

#End Region

#Region "Functions/Methods"

    ''' <summary>
    ''' Populates the compatible browsers list.
    ''' </summary>
    Private Sub PopulateCompatibleBrowsersList()

        Dim browserCheckSettings As BrowserCheckConfigurationSection = BrowserCheckModule.BrowserCheckConfigurationSection

        ' populate list with html
        litSupportedBrowsers.Text = browserCheckSettings.GetApplicationConfigurationElementBrowsersAsHTML(BrowserCheckModule.AbacusApplication)

    End Sub

#End Region

End Class
