
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.SvcOrders

    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class PlannedAttendance
        Inherits Target.Web.Apps.BasePage

        Private _order As DomServiceOrder = Nothing
        Private _dsoID As Integer

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryServiceOrders"), "Domiciliary Service Order")
            Me.JsLinks.Add("PlannedAttendance.js")

            Dim msg As ErrorMessage

            _dsoID = Utils.ToInt32(Request.QueryString("id"))

            If _dsoID > 0 Then
                _order = New DomServiceOrder(Me.DbConnection, String.Empty, String.Empty)
                msg = _order.Fetch(_dsoID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                msg = PopulateAttendance(_dsoID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If


        End Sub

#End Region

#Region " ShowDayOfWeekColumn "

        Protected Function ShowDayOfWeekColumn() As Boolean
            If _order Is Nothing Then
                Return False
            Else
                Return (_order.SpecifyDayOfWeek)
            End If
        End Function

#End Region

#Region " PopulateAttendance "

        Private Function PopulateAttendance(ByVal orderID As Integer) As ErrorMessage

            Const SP_NAME As String = "spxDomServiceOrderDetail_ConvertToAttendance"

            Dim msg As ErrorMessage
            Dim spParams As SqlParameter()
            Dim attendance As DataTable

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = orderID
                attendance = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams).Tables(0)

                With rptPlannedAttendance
                    .DataSource = attendance
                    .DataBind()
                End With

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg

        End Function

#End Region

    End Class

End Namespace