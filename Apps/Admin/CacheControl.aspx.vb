
Imports Target.Library.Web

Namespace Apps.Admin

    Partial Class CacheControl
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemAdminCacheControl"), "Admin: Cache Control")

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/sorttable.js"))

            ' if a single remove is specified
            If Not Request.QueryString("remove") Is Nothing Then
                Try
                    Cache.Remove(Request.QueryString("remove"))
                Catch ex As Exception
                    ' ignore any remove errors
                End Try
            End If

            Dim cacheObjects As New Hashtable
            For Each item As Object In Cache
                Dim name As String = item.Key
                'Comment the If..Then if you want to see ALL (System, etc.) items the cache
                'We don't want to see ASP.NET cached system items or ASP.NET Worker Processes
                '                If (Left(name, 7) <> "System.") And (Left(name, 7) <> "ISAPIWo") Then
                Try
                    If Not Cache(name) Is Nothing Then cacheObjects.Add(name, Cache(name).GetType().ToString())
                Catch ex As Exception
                    ' ignore errors here are object can be evicted from the cache at any point
                End Try

                '               End If
            Next

            btnRemoveAll.Style.Add("float", "left")
            btnRemoveAll.Attributes.Add("onclick", "return window.confirm(""Are you sure you wish to remove ALL items from the cache?"");")

            rptCacheItems.DataSource = cacheObjects
            rptCacheItems.DataBind()

        End Sub

        Private Sub btnRemoveAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveAll.Click
            Try
                For Each item As Object In Cache
                    Cache.Remove(item.Key)
                Next
            Catch ex As Exception
                ' ignore any remove errors
            Finally
                Response.Redirect("CacheControl.aspx")
            End Try
        End Sub
    End Class

End Namespace