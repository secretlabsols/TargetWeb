Imports System
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps.Security
Imports Target.SP.Library

Namespace Apps

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.OccupancyEnq
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen to view service user occupancy in a property and service.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      31/01/2007  SPBG-307 - fix to previous change to force date format.
    '''     MikeVO      18/12/2006  Default DateFrom to todays date.
    '''     MikeVO      27/11/2006  Ensure user can view the specified service and property.
    ''' 	[Mikevo]	??/??/????	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class OccupancyEnq
        Inherits Target.Web.Apps.BasePage

        Protected WithEvents occupancyList As Target.SP.Web.Apps.UserControls.OccupancyEnqList

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim strStyle As New StringBuilder
            Dim PropertyID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("pid"))
            Dim ServiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("sid"))
            Dim dtDateFrom As Date, dtDateTo As Date
            Dim intStatus As Integer
            Dim canViewService As Boolean, canViewProperty As Boolean
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemSPOccupancyEnquiry"), "Occupancy Enquiry")

            ' check to ensure the user is allowed to see the service and the property
            msg = SPClassesBL.UserCanViewService(Me.DbConnection, user.ExternalUserID, ServiceID, canViewService)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            msg = SPClassesBL.UserCanViewProperty(Me.DbConnection, user.ExternalUserID, PropertyID, canViewProperty)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Not canViewService Or Not canViewProperty Then Response.Redirect("~/Library/Errors/AccessDenied.aspx")

            strStyle.Append("label.label { float:left; width:9em; padding-right:1em; font-weight: bold; }")
            strStyle.Append("span.label { float:left; width:9em; padding-right:1em; font-weight: bold; }")
            strStyle.Append(".content { float:left; width:60%; }")
            strStyle.Append(".content2 { float:left; width:6em; }")
            Me.AddExtraCssStyle(strStyle.ToString)

            lblProperty.Text = getPropertyName(PropertyID)
            lblService.Text = getServiceName(ServiceID)

            chkActive.CheckBox.Checked = True
            chkProvisional.CheckBox.Checked = True

            ' default dateFrom to today
            If txtDateFrom.Text.Length = 0 Then txtDateFrom.Text = Now.ToString("dd/MM/yyyy")

            GetFilterDates(dtDateFrom, dtDateTo)

            intStatus = GetStatusValue()

            occupancyList.InitControl(Me.Page, PropertyID, ServiceID, dtDateFrom, dtDateTo, intStatus)
        End Sub

        Private Function GetStatusValue() As Integer
            'Work out the value to be calculated for all the selected statuses.
            Dim intStatus As Integer = 0

            If chkActive.CheckBox.Checked = True Then intStatus = intStatus + 1
            If chkCancelled.CheckBox.Checked = True Then intStatus = intStatus + 2
            If chkProvisional.CheckBox.Checked = True Then intStatus = intStatus + 4
            If chkDocumentary.CheckBox.Checked = True Then intStatus = intStatus + 8
            If chkSuspended.CheckBox.Checked = True Then intStatus = intStatus + 16

            GetStatusValue = intStatus
        End Function

        Private Sub GetFilterDates(ByRef dteDateFrom As Date, ByRef dteDateTo As Date)
            'Calculate the dates using an early or late date if values entered on page are blank
            If txtDateFrom.Text = "" Then
                dteDateFrom = CDate("01/01/1900")
            Else
                dteDateFrom = txtDateFrom.Text
            End If
            If txtDateTo.Text = "" Then
                dteDateTo = CDate("31/12/2079")
            Else
                dteDateTo = txtDateTo.Text
            End If
        End Sub

        Private Function getPropertyName(ByVal PropertyID As Integer) As String
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage = New ErrorMessage
            Dim strPropertyName As String = Nothing
            Const SP_NAME As String = "spxSPProperty_Fetch"

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = PropertyID

                ' execute
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                While reader.Read
                    strPropertyName = WebUtils.EncodeOutput(reader("PropertyName"))
                End While

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME, "OccupancyEnquiry.getPropertyName")     ' error reading data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
            End Try

            Return strPropertyName

        End Function

        Private Function getServiceName(ByVal ServiceID As Integer) As String
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage = New ErrorMessage
            Dim strServiceName As String = Nothing
            Const SP_NAME As String = "spxSPService_Fetch"

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = ServiceID

                ' execute
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                While reader.Read
                    strServiceName = WebUtils.EncodeOutput(reader("ServiceName"))
                End While

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME, "OccupancyEnquiry.getServiceName")     ' error reading data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
            End Try

            Return strServiceName

        End Function

    End Class
End Namespace
