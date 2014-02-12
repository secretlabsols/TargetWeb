Imports Target.Web.Apps
Imports Target.Library.Web
Imports Target.Library
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.Documents

Namespace Apps.Documents

    Partial Public Class PrintQueueBatches
        Inherits BasePage

#Region " Private Variables "

        Private Const _NavigationItemKey As String = "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"
        Private Const _PageTitle As String = "Print Queue Batches"

#End Region

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(ConstantsManager.GetConstant(_NavigationItemKey), _PageTitle)
            Me.UseJQuery = True

            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Documents/PrintQueueBatches.js"))

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Documents))

            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))

            If Not Me.IsPostBack Then BindPrinters()

        End Sub

#End Region

#Region " Private Methods "

        Private Sub BindPrinters()
            Dim msg As New ErrorMessage

            Try
                Dim printers As New DocumentPrinterCollection

                msg = DocumentPrinterBL.FetchList(Me.DbConnection, printers)
                If Not msg.Success Then Exit Sub

                cboPrinter = cpFilters.FindControl("cboPrinter")

                cboPrinter.DropDownList.DataTextField = "PrinterName"
                cboPrinter.DropDownList.DataValueField = "ID"
                cboPrinter.DropDownList.DataSource = printers
                cboPrinter.DropDownList.DataBind()

                ' insert empty item at top
                cboPrinter.DropDownList.Items.Insert(0, New ListItem(String.Empty, String.Empty))

                msg.Success = True
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            Finally
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
            End Try
        End Sub

#End Region

    End Class

End Namespace
