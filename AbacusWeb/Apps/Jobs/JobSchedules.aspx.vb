Imports Target.Web.Apps.Security


Namespace Apps.Jobs

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Apps.Jobs.JobSchedules
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Page to List Job Schedules.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	PaulW	23/06/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class JobSchedules
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.JobSchedules"), "Job Service - Job Schedules")
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add("JobSchedules.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))

            btnNew.Visible = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.JobScheduleMaintenance.AddNew"), _
                    Me.Settings.CurrentApplicationID)

            btnView.Visible = SecurityBL.UserHasMenuItem(Me.DbConnection, currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.JobScheduleMaintenance"), _
                    Me.Settings.CurrentApplicationID)

        End Sub

    End Class

End Namespace