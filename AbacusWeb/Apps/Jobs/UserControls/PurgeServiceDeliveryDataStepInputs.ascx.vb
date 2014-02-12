
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

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control that provides custom inputs for the purge service delivery data amendments job step.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class PurgeServiceDeliveryDataStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            PopulateScreen()
        End Sub

        Private Sub PopulateScreen()
            '++ Default cut-off to 3 months ago..
            dteCutOffDate.Text = DateAdd(DateInterval.Month, -3, Date.Today).ToShortDateString
            With valFutureDates
                .MinimumValue = Convert.ToDateTime("1900-01-01")
                .MaximumValue = Date.Today
                .Type = ValidationDataType.Date
                .ErrorMessage = "The date entered cannot be in the future"
                .SetFocusOnError = True
            End With
        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim value As String

            '++ Cut-off date..
            value = Request.Form(String.Format("{0}$txtTextBox", dteCutOffDate.UniqueID))
            result.Add(New Triplet(jobStepTypeID, "FilterCutOffDate", value))

            Return result

        End Function

        Private Sub dteCutOffDate_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles dteCutOffDate.PreRender
            valFutureDates.Validate()
        End Sub
    End Class

End Namespace

