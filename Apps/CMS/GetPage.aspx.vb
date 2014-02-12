
Imports System.Text.RegularExpressions
Imports Target.Library
Imports Target.Library.Web

Namespace Apps.CMS

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Web.Apps.CMS.GetPage
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Displays the specified CMS page.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      17/11/2006  Lower case HTML tags generated by the HTML editor.
    '''     MikeVO      30/08/2006  Support for record not found error message.
    ''' 	[mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class GetPage
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
            If Target.Library.Utils.ToInt32(Request.QueryString("asEmail")) > 0 Then
                Me.EnableViewState = False
                Me.EnableTimeout = False
                Me.RenderMenu = False
                Me.MasterPageFile = "~/PageAsEmail.master"
            End If
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"))

            Dim pageID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim cmsPage As WebCMSPage
            Dim msg As ErrorMessage

            If pageID = 0 Then
                Response.Redirect("~/Library/Errors/404.aspx")
            Else
                cmsPage = New WebCMSPage(Me.DbConnection)
                msg = cmsPage.Fetch(pageID)
                If Not msg.Success Then
                    If msg.Number = "E0513" Then    ' record not found
                        Response.Redirect("~/Library/Errors/404.aspx")
                    Else
                        Target.Library.Web.Utils.DisplayError(msg)
                    End If
                End If
                Me.PageTitle = cmsPage.Title
                litPageOverview.Text = cmsPage.SubTitle

                Dim re As Regex = New Regex("<(.|\n)+?>", RegexOptions.IgnoreCase)
                litContent.Text = re.Replace(cmsPage.Content, New MatchEvaluator(AddressOf HtmlTagMatch))

            End If

        End Sub

        Private Shared Function HtmlTagMatch(ByVal m As Match) As String
            Return m.Value.ToLower()
        End Function

    End Class

End Namespace