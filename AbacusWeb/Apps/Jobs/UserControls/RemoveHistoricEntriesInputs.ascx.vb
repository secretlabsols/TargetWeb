Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.Jobs.UserControls
    ''' <summary>
    ''' User control that provides custom inputs for the delete records from external client table job step.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir      01/07/2013   D12490 - Remove Historic Entries from Extrenal Client Table
    '''</history>
    Public Class RemoveHistoricEntriesInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

#Region "Fields"
        Private Const CONST_DATE_FORMAT As String = "dd/MM/yyyy"
#End Region

#Region "Page Events"
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            dteDeleteBeforeDate.MaximumValue = Date.Today
            dteDeleteBeforeDate.Text = Date.Today.ToString(CONST_DATE_FORMAT)
        End Sub
#End Region

#Region "Must Implements"
        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
        ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim batchTypesDesc As StringBuilder = New StringBuilder()
            Dim value As String

            'delete before date
            value = Request.Form(String.Format("{0}$txtTextBox", dteDeleteBeforeDate.UniqueID))
            If Not value Is Nothing AndAlso value.Trim().Length > 0 Then
                result.Add(New Triplet(jobStepTypeID, "FilterDeleteBeforeDate", value))
            End If

            Return result

        End Function
#End Region

    End Class
End Namespace
