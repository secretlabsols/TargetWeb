
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Jobs.Core
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.Controls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Abacus.Library.Core

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control that provides custom inputs for the purge job service data job step.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class PurgeJobServiceDataStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Const CTRL_PREFIX_JOB_STATUS As String = "chkJobStatus"

        Private _jobStatusCheckboxes As List(Of CheckBoxEx)

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            thePage.AddExtraCssStyle(".chkBoxStyle { float:left; margin-right:2em; }")
            PopulateScreen()
        End Sub

        Private Sub PopulateScreen()

            Dim fs As HtmlGenericControl, legend As HtmlGenericControl

            ' job status
            fs = New HtmlGenericControl("FIELDSET")
            fs.ID = "fsJobstatus"
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Job Status"
            fs.Controls.Add(legend)
            _jobStatusCheckboxes = New List(Of CheckBoxEx)
            For Each status As JobStatus In [Enum].GetValues(GetType(JobStatus))
                ' only allow the following status values
                If status = JobStatus.Cancelled OrElse status = JobStatus.Cancelling OrElse _
                    status = JobStatus.Complete OrElse status = JobStatus.Failed OrElse _
                    status = JobStatus.Warnings OrElse status = JobStatus.Exceptions Then

                    Dim chk As CheckBoxEx = New CheckBoxEx()
                    With chk
                        .ID = String.Format("{0}{1}", CTRL_PREFIX_JOB_STATUS, Convert.ToInt32(status))
                        .Text = Utils.SplitOnCapitals([Enum].GetName(GetType(JobStatus), status))
                        .CheckBoxCssClass = "chkBoxStyle"
                        .CheckBox.TextAlign = TextAlign.Right
                        ' only prime the checkbox if we haven't just submitted the values
                        If Request.Form(hidCreatingJob.UniqueID) Is Nothing Then .CheckBox.Checked = True
                        _jobStatusCheckboxes.Add(chk)
                    End With
                    fs.Controls.Add(chk)
                End If
            Next
            phJobstatus.Controls.Add(fs)

        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim value As String

            ' job status values
            For Each status As JobStatus In [Enum].GetValues(GetType(JobStatus))
                ' only allow the following status values
                If status = JobStatus.Cancelled OrElse status = JobStatus.Cancelling OrElse _
                    status = JobStatus.Complete OrElse status = JobStatus.Failed OrElse _
                    status = JobStatus.Warnings OrElse status = JobStatus.Exceptions Then

                    Dim id As String = String.Format("{0}{1}", CTRL_PREFIX_JOB_STATUS, Convert.ToInt32(status))
                    Dim chk As CheckBoxEx = phJobstatus.FindControl(id)
                    value = Request.Form(String.Format("{0}$chkCheckBox", chk.UniqueID))
                    If value Is Nothing Then
                        value = Boolean.FalseString
                    Else
                        value = Boolean.TrueString
                    End If
                    result.Add(New Triplet(jobStepTypeID, String.Format("FilterJobStatus{0}", status.ToString()), value))

                End If
            Next

            ' completion date
            value = Request.Form(String.Format("{0}$txtTextBox", dteCompletionDate.UniqueID))
            result.Add(New Triplet(jobStepTypeID, "FilterCompletionDate", value))

            Return result

        End Function

    End Class

End Namespace

