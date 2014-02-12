Imports System.Text
Imports Target.Abacus.Library
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Controls
Imports Target.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.ServiceUsers.SpendPlans

    ''' <summary>
    ''' Screen used to terminate a Spend Plan.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD     14/10/2010  D11918 - altered page to work as a popup
    '''     PaulW      20/07/2010  D11796 - SDS Spend Plans.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class Terminate
        Inherits BasePage

#Region "Fields"

        ' constants
        Private Const _DateFormat As String = "dd/MM/yyyy"

        ' locals
        Private _endReasons As SpendPlanEndReasonCollection = Nothing
        Private _spendPlanID As Integer = 0
        Private _spendPlan As SpendPlan = Nothing
        Private _terminated As Boolean = False

#End Region

#Region " Page_Load "

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.SpendPlans"), "Spend Plan Termination")
            Me.JsLinks.Add("Terminate.js")

            Dim msg As ErrorMessage
            Dim plan As New SpendPlan(Me.DbConnection, String.Empty, String.Empty)

            _spendPlanID = Utils.ToInt32(Request.QueryString("id"))

            With plan
                msg = .Fetch(_spendPlanID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                txtDateFrom.Text = .DateFrom
                txtReference.Text = .Reference

                If Me.IsPostBack Then
                    dteDateTo.GetPostBackValue()
                Else
                    If .DateTo <> DataUtils.MAX_DATE Then
                        dteDateTo.Text = .DateTo
                    End If
                End If

                PopulateDropdowns(.ID)
                If Me.IsPostBack Then
                    cboEndReason.DropDownList.SelectedValue = cboEndReason.GetPostBackValue()
                Else
                    cboEndReason.DropDownList.SelectedValue = .EndReasonID
                End If

                Dim svcUser As New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
                msg = svcUser.Fetch(.ClientID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                txtServiceUser.Text = String.Format("{0} : {1}", svcUser.Reference, svcUser.Name)

            End With

            _spendPlan = plan
            SetupValidators(plan)

        End Sub

#End Region

#Region " PopulateDropdowns "

        ''' <summary>
        ''' Populates the dropdowns.
        ''' </summary>
        ''' <param name="planID">The plan ID.</param>
        Private Sub PopulateDropdowns(ByVal planID As Integer)

            Dim msg As ErrorMessage

            With cboEndReason
                ' get a list of reasons available to the order
                If _endReasons Is Nothing Then
                    msg = SpendPlanBL.FetchEndReasonsAvailableToPlan(Me.DbConnection, planID, TriState.False, _endReasons)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                With .DropDownList
                    .Items.Clear()
                    .DataSource = _endReasons
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .SelectedValue = Nothing
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With

            End With

        End Sub

#End Region

#Region " btnTerminate_Click "

        ''' <summary>
        ''' Handles the Click event of the btnTerminate control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub btnTerminate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTerminate.Click

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Me.Validate("Terminate")

            If Me.IsValid Then
                msg = SpendPlanBL.TerminateSpendPlan(Me.DbConnection, currentUser.ExternalUsername, _
                                                    AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _spendPlanID, dteDateTo.Text, _
                                                    Utils.ToInt32(CType(cboEndReason, DropDownListEx).DropDownList.SelectedValue()))
                If Not msg.Success Then
                    If msg.Number = SpendPlanBL.ERR_COULD_NOT_TERMINATE_SPENDPLAN Then
                        lblError.Text = msg.Message
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                Else
                    _terminated = True
                End If

            End If

        End Sub

#End Region

#Region "Page_PreRenderComplete"

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim js As New StringBuilder()

            js.AppendFormat("dteDateToID='{0}_txtTextBox';", dteDateTo.ClientID)
            js.AppendFormat("cboEndReasonID='{0}_cboDropDownList';", cboEndReason.ClientID)
            js.AppendFormat("isTerminated={0};", _terminated.ToString().ToLower())
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

        End Sub

#End Region

#Region "SetupValidators"

        ''' <summary>
        ''' Setups the pages validators.
        ''' </summary>
        ''' <param name="sp">The sp.</param>
        Private Sub SetupValidators(ByVal sp As SpendPlan)

            Dim msg As New ErrorMessage()
            Dim spendPlanDateToMin As DateTime = DateTime.MinValue
            Dim spendPlanDateToMinKey As String = "vsSpendPlanDateToMinKey"
            Dim spendPlanDateToMax As DateTime = DateTime.MaxValue
            Dim spendPlanDateToMaxKey As String = "vsSpendPlanDateToMaxKey"

            If Not IsPostBack Then
                ' if first page hit then get data from db and store in view state
                ' to avoid further db calls on subsequent postbacks

                ' get the min and max date to dates 
                msg = SpendPlanBL.GetMinAndMaxSpendPlanDateFromAndDateTo(DbConnection, _
                                                                         sp, _
                                                                         New DateTime(), _
                                                                         New DateTime(), _
                                                                         spendPlanDateToMin, _
                                                                         spendPlanDateToMax, _
                                                                         New Integer())
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' add these items into view state so we only do this once per page hit
                ViewState.Add(spendPlanDateToMinKey, spendPlanDateToMin)
                ViewState.Add(spendPlanDateToMaxKey, spendPlanDateToMax)

            Else
                ' else subsequent hit so fetch from view state

                spendPlanDateToMin = ViewState(spendPlanDateToMinKey)
                spendPlanDateToMax = ViewState(spendPlanDateToMaxKey)

            End If

            ' setup range validator for date to field
            With dteDateTo
                .MinimumValue = spendPlanDateToMin.ToString(_DateFormat)
                .MaximumValue = spendPlanDateToMax.ToString(_DateFormat)
                .SetupRangeValidator()
            End With

        End Sub

#End Region

    End Class

End Namespace