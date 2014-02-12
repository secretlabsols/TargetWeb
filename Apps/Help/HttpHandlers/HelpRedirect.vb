
Imports System.Xml.XPath
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Help.HttpHandlers

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.Help.HttpHandlers.HelpRedirect
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Handler to detect the current page and display the correect context-sensitve help.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history><![CDATA[
    '''     MikeVO      06/10/2008  Changed config file cache to be dependent on the file itself rather that a fixed time.
    ''' 	[Mikevo]	02/10/2008	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class HelpRedirect
        Implements IHttpHandler

        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest

            Const CONFIG_PATH As String = "HelpRedirect.config"
            Const CACHE_KEY As String = "Target.Web.Apps.Help.HelpRedirect.config"

            Dim config As XPathDocument
            Dim nav As XPathNavigator, tempNav As XPathNavigator
            Dim referer As String = context.Request.QueryString("r")
            Dim configPath As String, helpRoot As String, helpUrl As String

            Try
                ' get the redirect config from disk or from the cache
                If HttpRuntime.Cache.Item(CACHE_KEY) Is Nothing Then
                    configPath = context.Server.MapPath(WebUtils.GetVirtualPath(CONFIG_PATH))
                    config = New XPathDocument( _
                        configPath, _
                        System.Xml.XmlSpace.Default _
                    )
                    WebUtils.PopulateCache(CACHE_KEY, config, configPath)
                Else
                    config = CType(HttpRuntime.Cache.Item(CACHE_KEY), XPathDocument)
                End If

                nav = config.CreateNavigator()

                ' start with the default
                tempNav = nav.SelectSingleNode("/HelpRedirect")
                helpRoot = tempNav.GetAttribute("helpRoot", String.Empty)
                helpUrl = tempNav.GetAttribute("defaultHelpUrl", String.Empty)

                ' find the specified referer
                tempNav = Nothing
                tempNav = nav.SelectSingleNode(String.Format("/HelpRedirect/Item[@referer = '{0}']", referer))
                If Not tempNav Is Nothing Then
                    helpUrl = tempNav.GetAttribute("helpUrl", String.Empty)
                End If

                context.Response.Redirect(String.Format("{0}{1}", helpRoot, helpUrl), False)

            Catch ex As Exception
                context.Response.Write(ex.ToString())
            End Try

        End Sub

        Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

    End Class

End Namespace