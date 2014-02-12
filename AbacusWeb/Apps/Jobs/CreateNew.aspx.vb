
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Reflection
Imports System.Text
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Jobs.Core
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.Core
Imports Target.Abacus.Library.JobService
Imports Target.Library.Collections

Namespace Apps.Jobs

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Apps.Jobs.CreateNew
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Page to allow the manual creation of a new job.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     JohnF   12/02/2013  Ensure that the 'Publish Debtor Invoices and Statements' job is selectively unavailable (D12092B)
    '''     MikeVO  09/09/2011  A4WA#7013 - hide Create Job button until a job is selected.
    '''     MikeVO  26/04/2011  SDS issue #607 - corrected behavior when displayed in popup window.
    '''     MoTahir 09/02/2011  D11934 - Password Maintenance
    '''     MikeVO  20/08/2010  Enhanced screen to allow primign of required JobType using QS_JOBTYPEID querystring.
    '''     JohnF   09/03/2010  D11624 - Ensure that step inputs contained within separate frames
    '''     MikeVO  08/01/2010  D11435 - user-job security amd some simple UI improvements.
    '''     MikeVO  15/06/2009  D11515 - added support for email notifications.
    '''     MikeVO  01/12/2008  D11444 - security overhaul.
    ''' 	MikeVO	14/03/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class CreateNew
        Inherits Target.Web.Apps.BasePage

        Const CTRL_INPUT As String = "ctrlInput{0}"
        Const QS_JOBTYPEID As String = "jobTypeID"
        Const SCRIPT_STARTUP As String = "Startup"

        Private _jobTypes As DataSet
        Private _useOverriddenJobType As Boolean

#Region " Properties "

        Private ReadOnly Property OverriddenJobTypeID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString(QS_JOBTYPEID))
            End Get
        End Property

        Private ReadOnly Property UseOverriddenJobType() As Boolean
            Get
                Return _useOverriddenJobType
            End Get
        End Property

        Private ReadOnly Property JobTypeID() As Integer
            Get
                Dim result As Integer
                Dim rows As DataRow()

                ' override with querystring job type ID?
                If Me.OverriddenJobTypeID > 0 Then
                    rows = _jobTypes.Tables(0).Select(String.Format("ID = {0}", Me.OverriddenJobTypeID))
                    If rows.Length > 0 Then
                        ' if the job type ID was found in the list
                        _useOverriddenJobType = True
                        result = Me.OverriddenJobTypeID
                    End If
                End If

                If Me.UseOverriddenJobType = False Then
                    result = Utils.ToInt32(cboJobType.GetPostBackValue())
                End If

                Return result
            End Get
        End Property

#End Region

#Region " Page_Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim msg As ErrorMessage

            InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.CreateNewJob"), "Job Service - Create New Job")

            msg = FetchJobTypes()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Not IsPostBack Then
                PopulateDropdowns()
                PrimeDateTime()
                PopulateInputs(False)
            End If

            AddJQuery()


            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                    String.Format("btnCreateJob='{0}';", _
                        btnCreate.ClientID), _
                    True _
                )
            End If

        End Sub

#End Region

#Region " FetchJobTypes "

        Private Function FetchJobTypes() As ErrorMessage

            Const SP_NAME_JOBTYPES As String = "spxJobType_FetchListForManualCreation"

            Dim msg As ErrorMessage
            Dim spParams As SqlParameter()
            Dim sysInfo As SystemInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_JOBTYPES, False)
                spParams(0).Value = sysInfo.LicenceNo
                spParams(1).Value = currentUser.ID
                _jobTypes = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_JOBTYPES, spParams)

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg

        End Function

#End Region

#Region " PopulateDropdowns "

        Private Sub PopulateDropdowns()

            ' load job type combo
            With cboJobType.DropDownList
                .DataSource = _jobTypes.Tables(0)
                .DataTextField = "Name"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)
                .AutoPostBack = True
            End With

            WebUtils.SetDropdownListValue(cboJobType.DropDownList, Me.JobTypeID)

            btnCreate.Visible = Me.JobTypeID > 0

        End Sub

#End Region

#Region " PrimeDateTime "

        Private Sub PrimeDateTime()
            Dim theDate As Date = DateTime.Now
            dteStartDate.Text = theDate.ToString("dd/MM/yyyy")
            tmeStartTime.Text = theDate.ToString("HH:mm:ss")
        End Sub

#End Region

#Region " PopulateInputs "

        Private Sub PopulateInputs(ByVal populateWithPostedBackValues As Boolean)

            Dim msg As ErrorMessage
            Dim ctrlType As Type
            Dim ctrl As Control
            Dim propInfo As PropertyInfo
            Dim input As vwJobStepInputs
            Dim inputs As vwJobStepInputsCollection = Nothing
            Dim value As String, currentJobStepTypeName As String = String.Empty
            Dim fieldset As HtmlGenericControl = Nothing, legend As HtmlGenericControl

            If Me.JobTypeID > 0 Then

                ' get the list of inputs
                msg = vwJobStepInputs.FetchList(Me.DbConnection, inputs, Me.JobTypeID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If inputs.Count > 0 Then

                    For Each input In inputs
                        With input
                            If .Visible Then
                                ' create the control
                                If Not .UserControlVirtualPath Is Nothing AndAlso .UserControlVirtualPath.Trim().Length > 0 Then
                                    ctrl = Me.LoadControl(.UserControlVirtualPath)
                                    ctrlType = ctrl.GetType()
                                Else
                                    ctrlType = Type.GetType(String.Format("{0},{1}", .ClassName, .AssemblyName))
                                    ctrl = Activator.CreateInstance(ctrlType)
                                End If
                                ctrl.ID = String.Format(CTRL_INPUT, .JobInputID)

                                ' set properties
                                propInfo = ctrlType.GetProperty("LabelText")
                                If Not propInfo Is Nothing Then
                                    propInfo.SetValue(ctrl, .LabelText, BindingFlags.SetProperty, Nothing, Nothing, Nothing)
                                End If
                                propInfo = ctrlType.GetProperty("LabelWidth")
                                If Not propInfo Is Nothing Then
                                    propInfo.SetValue(ctrl, .LabelWidth, BindingFlags.SetProperty, Nothing, Nothing, Nothing)
                                End If
                                propInfo = ctrlType.GetProperty("Required")
                                If Not propInfo Is Nothing Then
                                    propInfo.SetValue(ctrl, Convert.ToBoolean(.Required), BindingFlags.SetProperty, Nothing, Nothing, Nothing)
                                End If
                                If .Required Then
                                    propInfo = ctrlType.GetProperty("RequiredValidatorErrMsg")
                                    If Not propInfo Is Nothing Then
                                        propInfo.SetValue(ctrl, .RequiredErrMsg, BindingFlags.SetProperty, Nothing, Nothing, Nothing)
                                    End If
                                End If
                                propInfo = ctrlType.GetProperty("MaxLength")
                                If Not propInfo Is Nothing AndAlso .MaxLength > 0 Then
                                    propInfo.SetValue(ctrl, .MaxLength, BindingFlags.SetProperty, Nothing, Nothing, Nothing)
                                End If
                                propInfo = ctrlType.GetProperty("Width")
                                If Not propInfo Is Nothing AndAlso Not .Width Is Nothing AndAlso .Width.Length > 0 Then
                                    propInfo.SetValue(ctrl, Unit.Parse(.Width), BindingFlags.SetProperty, Nothing, Nothing, Nothing)
                                End If
                                '++ Dynamic defaults for NonResidentialServiceVariance extract
                                '++ under Reporting job - set to the current week..
                                If input.JobStepTypeName = "Reporting Extract - Non-Res Service Variance" Then
                                    Dim defWeekEnding As Date, curDate As Date
                                    Dim sysInfo As SystemInfo

                                    sysInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)
                                    defWeekEnding = sysInfo.DomServiceWEDate
                                    curDate = Date.Today
                                    If input.InputName = "WeekCommencing" Then
                                        Do Until Weekday(curDate) = Weekday(defWeekEnding)
                                            curDate = DateAdd(DateInterval.Day, 1, curDate)
                                        Loop
                                        curDate = DateAdd(DateInterval.Day, -6, curDate)
                                    Else 'If input.InputName = "WeekEnding" Then
                                        Do Until Weekday(curDate) = Weekday(defWeekEnding)
                                            curDate = DateAdd(DateInterval.Day, 1, curDate)
                                        Loop
                                    End If
                                    propInfo = ctrlType.GetProperty("Text")
                                    If Not propInfo Is Nothing AndAlso Not .DefaultValue Is Nothing Then
                                        propInfo.SetValue(ctrl, curDate.ToShortDateString, BindingFlags.SetProperty, Nothing, Nothing, Nothing)
                                    End If
                                Else
                                    propInfo = ctrlType.GetProperty("Text")
                                    If Not propInfo Is Nothing AndAlso Not .DefaultValue Is Nothing Then
                                        propInfo.SetValue(ctrl, .DefaultValue, BindingFlags.SetProperty, Nothing, Nothing, Nothing)
                                    End If
                                End If
                                propInfo = ctrlType.GetProperty("Format")
                                If Not propInfo Is Nothing AndAlso .JobStepTypeInputFieldID = 1 Then
                                    propInfo.SetValue(ctrl, _
                                        [Enum].Parse(GetType(Target.Library.Web.Controls.TextBoxEx.TextBoxExFormat), .FieldFormat), _
                                        BindingFlags.SetProperty, _
                                        Nothing, Nothing, Nothing)
                                End If

                                If input.JobStepTypeName <> currentJobStepTypeName Then
                                    currentJobStepTypeName = input.JobStepTypeName
                                    If fieldset IsNot Nothing Then
                                        fsInputs.Controls.Add(New LiteralControl("<br />"))
                                    End If
                                    fieldset = New HtmlGenericControl("fieldset")
                                    legend = New HtmlGenericControl("legend")
                                    legend.InnerText = input.JobStepTypeName
                                    fieldset.Controls.Add(legend)
                                    fsInputs.Controls.Add(fieldset)
                                End If
                                If fieldset IsNot Nothing Then
                                    If fieldset.Controls.Count > 1 Then
                                        fieldset.Controls.Add(New LiteralControl("<br />"))
                                    End If
                                    fieldset.Controls.Add(ctrl)
                                End If

                                ' set postback value
                                If populateWithPostedBackValues Then
                                    value = Nothing
                                    If TypeOf ctrl Is Target.Library.Web.Controls.TextBoxEx Then
                                        value = GetInputValue(ctrl)
                                        If Not value Is Nothing Then CType(ctrl, Target.Library.Web.Controls.TextBoxEx).Text = value
                                    End If
                                End If
                            End If
                        End With
                    Next

                End If

                fsInputs.Visible = True

            End If

        End Sub

#End Region

#Region " GetInputValue "

        Private Function GetInputValue(ByVal ctrl As Control) As String

            Dim result As String = Nothing

            If Not ctrl Is Nothing Then
                If TypeOf ctrl Is Target.Library.Web.Controls.TextBoxEx Then
                    Dim key As String = String.Format("{0}$txtTextBox", ctrl.UniqueID)
                    If Not Request.Form(key) Is Nothing Then
                        result = Request.Form(key)
                    End If
                End If
            End If

            Return result

        End Function

#End Region

#Region " cboJobType_SelectedIndexChanged "

        Private Sub cboJobType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboJobType.SelectedIndexChanged
            PopulateDropdowns()
            PrimeDateTime()
            PopulateInputs(False)
        End Sub

#End Region

#Region " btnCreate_Click "

        Private Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click

            Dim msg As ErrorMessage = Nothing
            Dim trans As SqlTransaction = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim input As vwJobStepInputs
            Dim inputs As vwJobStepInputsCollection = Nothing
            Dim newInputs As List(Of Triplet)
            Dim scheduledStart As Date
            Dim inputCtrl As Control
            Dim customInputs As ICustomJobStepInputs
            Dim jobID As Integer
            Dim a4wUser As Target.Abacus.Library.DataClasses.Users

            PopulateDropdowns()
            PopulateInputs(True)

            Me.Validate()

            If Me.IsValid Then

                Try
                    a4wUser = New Target.Abacus.Library.DataClasses.Users(Me.DbConnection)
                    msg = a4wUser.Fetch(currentUser.ExternalUserID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    ' get the list of inputs with their default values for this job type
                    msg = JobServiceBL.GetNewJobInputs(trans, Me.JobTypeID, inputs)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' build the inputs
                    newInputs = New List(Of Triplet)
                    For Each input In inputs
                        With input
                            inputCtrl = fsInputs.FindControl(String.Format(CTRL_INPUT, .JobInputID))
                            If .CustomInputs Then
                                ' Job steps that have custom inputs must implement the ICustomJobStepInputs interface.
                                ' We get the required values from there.
                                customInputs = CType(inputCtrl, ICustomJobStepInputs)
                                newInputs.AddRange(customInputs.GetCustomInputs(trans.Connection, trans, .JobStepTypeID))
                            ElseIf Not .Visible Then
                                ' the input is not visible so just take the default value
                                newInputs.Add(New Triplet(.JobStepTypeID, .InputName, .DefaultValue))
                            Else
                                ' take whatever the user entered on screen
                                Dim value As String = GetInputValue(inputCtrl)
                                newInputs.Add(New Triplet(.JobStepTypeID, .InputName, value))
                            End If
                        End With
                    Next

                    ' get the scheduled start
                    scheduledStart = DateTime.Parse(tmeStartTime.ToString(dteStartDate.Text))

                    ' create the job with the new inputs
                    msg = JobServiceBL.CreateNewJob(trans, _
                                                    Me.JobTypeID, _
                                                    Nothing, _
                                                    currentUser.ExternalUsername, _
                                                    scheduledStart, _
                                                    txtComment.Text, _
                                                    currentUser.ExternalUserID, _
                                                    a4wUser.EMail, _
                                                    newInputs, _
                                                    jobID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    trans.Commit()
                    msg = New ErrorMessage()
                    msg.Success = True

                    Response.Redirect( _
                        String.Format("Default.aspx?autopopup={0}", _
                                      Utils.ToInt32(Request.QueryString("autopopup")) _
                        ) _
                    )

                Catch ex As Exception
                    WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
                Finally
                    If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                End Try

            End If

        End Sub

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.UseOverriddenJobType Then
                Me.PageTitle = cboJobType.DropDownList.SelectedItem.Text
                fsJobType.Visible = False
            End If

            If Me.JobTypeID = JobTypes.PublishDebtorInvoicesAndStatements Then
                '++ Disable the 'Create New' button when necessary..
                Dim appSettings As ApplicationSettingCollection = Nothing
                Dim msg As New ErrorMessage

                msg = ApplicationSetting.FetchList(Me.DbConnection, appSettings, "", "", 2, "PrintInvoicesUsingPublishJob")
                If msg.Success AndAlso appSettings IsNot Nothing AndAlso appSettings.Count > 0 Then
                    btnCreate.Enabled = (Utils.ToInt32(appSettings(0).SettingValue) = 1)
                Else
                    btnCreate.Enabled = False
                End If
            Else
                btnCreate.Enabled = True
            End If
        End Sub

#End Region

#Region " Use JQuery "
        Private Sub AddJQuery()

            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            UseJqueryUI = True

            ' add in the table filter library 
            UseJqueryTableFilter = True

            ' add the table scroller library as we might have large amounts of data
            UseJqueryTableScroller = True

            ' add the searchable menu
            UseJquerySearchableMenu = True

            ' add the jquery tooltip
            UseJqueryTooltip = True

            UseJqueryTemplates = True
        End Sub
#End Region
    End Class

End Namespace