
Imports System.IO

Namespace Apps.CMS.HttpHandlers

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.CMS.HttpHandlers.GetPageHandler
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     HTTP handler to forward CMS GetPage requests to the correct location.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    ''' 	[Mikevo]	02/06/2005	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Public Class GetPageHandler
        Implements IHttpHandler, System.Web.SessionState.IRequiresSessionState

        Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
            context.Server.Execute("~/Apps/CMS/GetPage.aspx", context.Response.Output)
        End Sub

        Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

    End Class

End Namespace