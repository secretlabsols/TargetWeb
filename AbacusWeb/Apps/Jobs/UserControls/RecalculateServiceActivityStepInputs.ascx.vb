Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control that provides custom inputs for the Recalculate Service Activity job step.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' PaulW      15/02/2013   D11826 Recalculate Service Activity
    '''</history>
    Partial Public Class RecalculateServiceActivityStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

#Region " Properties "
        ''' <summary>
        ''' Gets base page.
        ''' </summary>
        ''' <value>Base page.</value>
        Private ReadOnly Property MyBasePage() As BasePage
            Get
                Return CType(Page, BasePage)
            End Get
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim weekEndingDate As DateTime = Target.Abacus.Library.DomContractBL.GetWeekEndingDate(MyBasePage.DbConnection, Nothing)
            Dim weekCommencingDate As DateTime = Target.Abacus.Library.DomContractBL.GetWeekCommencingDate(MyBasePage.DbConnection, Nothing)
            Const SCRIPT_STARTUP As String = "StartupRecalActuals"

            MyBasePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/Jobs/UserControls/RecalculateServiceActivityStepInputs.js"))

            If Not MyBasePage.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                MyBasePage.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                                    String.Format("RecalcActuals_dateToID='{0}';RecalcActuals_dateFromID='{1}';", dteDateTo.ClientID, dteDateFrom.ClientID), True)
            End If


            ' setup date from date picker
            With dteDateFrom
                .AllowableDays = weekCommencingDate.DayOfWeek
            End With

            ' setup date to date picker
            With dteDateTo
                .AllowableDays = weekEndingDate.DayOfWeek
            End With


        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim msg As ErrorMessage
            Dim client As ClientDetail
            Dim serviceUserID As Integer
            Dim value As String

            ' provider
            serviceUserID = Utils.ToInt32(CType(serviceUser, InPlaceClientSelector).GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "FilterServiceUserID", serviceUserID))
            If serviceUserID > 0 Then
                If Not trans Is Nothing Then
                    client = New ClientDetail(trans, String.Empty, String.Empty)
                Else
                    client = New ClientDetail(conn, String.Empty, String.Empty)
                End If
                msg = client.Fetch(serviceUserID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterServiceUser", String.Format("{0}:{1}", client.Reference, client.Name)))
            End If

            ' period from
            value = Request.Form(String.Format("{0}$txtTextBox", dteDateFrom.UniqueID))
            If Not value Is Nothing AndAlso value.Trim().Length > 0 Then
                result.Add(New Triplet(jobStepTypeID, "FilterPeriodFrom", value))
            Else
                result.Add(New Triplet(jobStepTypeID, "FilterPeriodFrom", Convert.ToDateTime("01-Jan-1900")))
            End If

            ' period to
            value = Request.Form(String.Format("{0}$txtTextBox", dteDateTo.UniqueID))
            If Not value Is Nothing AndAlso value.Trim().Length > 0 Then
                result.Add(New Triplet(jobStepTypeID, "FilterPeriodTo", value))
            Else
                result.Add(New Triplet(jobStepTypeID, "FilterPeriodTo", Convert.ToDateTime("31-DEC-9999")))
            End If

            ' Force Reconsideration
            value = Request.Form(String.Format("{0}$chkCheckBox", chkForceRecon.UniqueID))
            If value Is Nothing Then
                value = Boolean.FalseString
            ElseIf value = "on" Then
                value = Boolean.TrueString
            Else
                value = Boolean.FalseString
            End If
            result.Add(New Triplet(jobStepTypeID, "FilterForceReconsideration", value))
            Return result

        End Function

    End Class

End Namespace