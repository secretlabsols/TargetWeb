
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls

Namespace Apps.Security.UserControls

    ''' <summary>
    ''' User control to encapsulate the listing and selectiing of security roles.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      14/09/2009  D11602 - menu improvements.
    '''     MikeVO      10/07/2009  D11630 - added list report.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Class RoleSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal selectedRoleID As Integer, _
                               ByVal showNewButton As Boolean, _
                               ByVal showViewButton As Boolean)

            Dim reportID As Integer
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            btnNew.Visible = showNewButton
            btnView.Visible = showViewButton

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add list sorter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListSorter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/Security/UserControls/RoleSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Security.WebSvc.Security))

            reportID = Utils.ToInt32( _
                Target.Library.Web.ConstantsManager.GetConstant( _
                    String.Format("{0}.WebReport.SecurityRoles", thePage.Settings.CurrentApplication) _
                ) _
            )
            If reportID <= 0 Then
                ctlList.Visible = False
            Else
                With CType(ctlList, IReportsButton)
                    .ButtonText = "List"
                    .ReportID = reportID
                    .Position = SearchableMenu.SearchableMenuPosition.TopRight
                    .Parameters.Add("intApplicationID", Convert.ToInt32(thePage.Settings.CurrentApplicationID))
                End With
            End If

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format( _
              "currentPage={0};RoleSelector_selectedRoleID={1};btnViewID='{2}';showViewButton={3};", _
              currentPage, selectedRoleID, btnView.ClientID, showViewButton.ToString().ToLower()) _
             ) _
            )

        End Sub

    End Class

End Namespace

