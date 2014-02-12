
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls

Namespace Apps.Security.UserControls

    ''' <summary>
    ''' User control to encapsulate the listing and selectiing of security users.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      14/09/2009  D11602 - menu improvements.
    '''     MikeVO      14/07/2009  D11630 - added list report.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Class UserSelector
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal selectedUserID As Integer)

            Dim usersReportID As Integer, userRolesReportID As Integer
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add list sorter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListSorter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/Security/UserControls/UserSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Security.WebSvc.Security))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Web.Apps.Security.WebSecurityUserStatus))

            ' users report
            usersReportID = Utils.ToInt32( _
                Target.Library.Web.ConstantsManager.GetConstant( _
                    String.Format("{0}.WebReport.SecurityUsers", thePage.Settings.CurrentApplication) _
                ) _
            )
            If usersReportID <= 0 Then
                ctlUsers.Visible = False
            Else
                With CType(ctlUsers, IReportsButton)
                    .ButtonText = "List Users"
                    .ReportID = usersReportID
                    .Position = SearchableMenu.SearchableMenuPosition.TopLeft
                    .Parameters.Add("intApplicationID", Convert.ToInt32(thePage.Settings.CurrentApplicationID))
                End With
            End If

            ' user roles report
            userRolesReportID = Utils.ToInt32( _
                Target.Library.Web.ConstantsManager.GetConstant( _
                    String.Format("{0}.WebReport.SecurityUserRoles", thePage.Settings.CurrentApplication) _
                ) _
            )
            If userRolesReportID <= 0 Then
                ctlUserRoles.Visible = False
            Else
                With CType(ctlUserRoles, IReportsButton)
                    .ButtonText = "List User Roles"
                    .ReportID = userRolesReportID
                    .Position = SearchableMenu.SearchableMenuPosition.TopLeft
                    .Parameters.Add("intApplicationID", Convert.ToInt32(thePage.Settings.CurrentApplicationID))
                End With
            End If


            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format( _
              "currentPage={0};UserSelector_selectedUserID={1};", _
              currentPage, selectedUserID) _
             ) _
            )

        End Sub

    End Class

End Namespace

