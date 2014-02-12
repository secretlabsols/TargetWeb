
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text
Imports AjaxPro
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Utils
Imports Target.Abacus.Jobs.Core
Imports Target.Abacus.Web.Apps.Jobs.WebSvc
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.Core

Namespace Apps.Jobs

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Apps.Jobs.DefaultPage
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Default page used to interact with the Job Service.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MikeVO      11/01/2010  D11435 - user-job security
    '''     MikeVO      16/07/2009  D11627 - extra security for copy/cancel/delete jobs.
    '''     MikeVO      18/06/2009  D11515 - allow a job ID on the querystring to jump straight to a job.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      13/11/2006  Fix to AddTab() extra javascript.
    '''     Phil Walker 04/10/2006  D10883 - Client Interface; SWIFT - Add new Job Type.
    '''     MikeVO      29/08/2006  D10921 - support for config settings in database.
    '''     MikeVO      18/05/2006  D10816 - added tabs to input/progress/results view.
    ''' 	[Mikevo]	20/01/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class DefaultPage
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Const SP_NAME_CREATEDBY As String = "spxJob_FetchCreatedByList"
            Const SP_NAME_USED_JOB_TYPES As String = "spxJobType_FetchUsedTypes"

            InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ViewJobs"), "Job Service - View Jobs")

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reader As SqlDataReader
            Dim dsJobTypes As DataSet
            Dim currentUserHasJobs As Boolean
            Dim canCreateJobs As Boolean, canCopyJobs As Boolean, canCancelJobs As Boolean, canDeleteJobs As Boolean
            Dim jobID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("jobID"))
            Dim selectedJob As Job = Nothing
            Dim startupJS As StringBuilder
            Dim msg As ErrorMessage
            Dim spParamsUsedJobTypes As SqlParameter()

            canCreateJobs = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                           currentUser.ID, _
                                           Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.CreateNewJob"), _
                                           Me.Settings.CurrentApplicationID)
            canCopyJobs = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ViewJobs.CopyJob"))
            canCancelJobs = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ViewJobs.CancelJob"))
            canDeleteJobs = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ViewJobs.DeleteJob"))

            ' load job name combo
            spParamsUsedJobTypes = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_USED_JOB_TYPES, False)
            spParamsUsedJobTypes(0).Value = currentUser.ID
            dsJobTypes = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_USED_JOB_TYPES, spParamsUsedJobTypes)
            With cboJobName.DropDownList
                .DataSource = dsJobTypes.Tables(0)
                .DataTextField = "Name"
                .DataValueField = "ID"
                .DataBind()
                ' insert a blank item at the top
                .Items.Insert(0, New ListItem("", 0))
                ' add client script
                .Attributes.Add("onchange", "FilterJobList()")
            End With

            ' load user combo
            reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_CREATEDBY)
            With cboUser.DropDownList
                .DataSource = reader
                .DataTextField = "CreatedBy"
                .DataValueField = "CreatedBy"
                .DataBind()
                reader.Close()
                ' insert a blank item at the top
                .Items.Insert(0, New ListItem("", ""))
                If Not .Items.FindByValue(currentUser.ExternalUsername) Is Nothing Then
                    currentUserHasJobs = True
                    .SelectedValue = currentUser.ExternalUsername
                End If
                ' add client script
                .Attributes.Add("onchange", "FilterJobList()")
            End With

            ' load status combo
            With cboJobStatus.DropDownList
                For Each status As String In [Enum].GetNames(GetType(JobStatus))
                    .Items.Add(New ListItem(Utils.SplitOnCapitals(status), [Enum].Parse(GetType(JobStatus), status)))
                Next
                ' insert a blank item at the top
                .Items.Insert(0, New ListItem("", "0"))
                .SelectedIndex = 0
                ' add client script
                .Attributes.Add("onchange", "FilterJobList()")
            End With


            ' add WebSvc utils JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/WebSvcUtils.js"))
            ' add table sorting JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/sorttable.js"))
            ' add dat utilsity JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add page JS
            Me.JsLinks.Add("Default.js")
            ' add progress bar JS
            Me.JsLinks.Add(GetVirtualPath("Library/JavaScript/ProgressBar.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(JobService))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(JobStatus))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(JobStepStatus))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(JobStepXml))

            ' javascript to execute for window.onload
            startupJS = New StringBuilder()
            startupJS.AppendFormat( _
                "canCreateJobs={0};canCopyJobs={1};canCancelJobs={2};canDeleteJobs={3};Init();", _
                canCreateJobs.ToString().ToLower(), _
                canCopyJobs.ToString().ToLower(), _
                canCancelJobs.ToString().ToLower(), _
                canDeleteJobs.ToString().ToLower() _
            )
            If jobID > 0 Then
                selectedJob = New Job(Me.DbConnection)
                msg = selectedJob.Fetch(jobID)
                If Not msg.Success Then jobID = 0
            End If
            If jobID = 0 Then
                startupJS.AppendFormat( _
                    "ChangePageView(""JobList"");FetchJobList(""{0}"",0,0,1);", _
                    IIf(currentUserHasJobs, currentUser.ExternalUsername, String.Empty) _
                )
            Else
                startupJS.AppendFormat("JobList_ViewSteps(null, new Array({0},'{1}','{2}'));", _
                                       jobID, _
                                       selectedJob.JobName, _
                                       selectedJob.CreatorComment _
                )
            End If
            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", startupJS.ToString(), True)

        End Sub

    End Class

End Namespace