Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Screen to allow a user to view the applicable rates for a domiciliary contract.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewDomContractRates
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ViewDomiciliaryContractRates"), "View Contract Rates")
            Dim domContractID As Long = Target.Library.Utils.ToInt32(Request.QueryString("dcid"))
            Dim periodID As Long = Target.Library.Utils.ToInt32(Request.QueryString("pid"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim strStyle As New StringBuilder
            Dim reader As SqlDataReader = Nothing
            Me.JsLinks.Add("TargetWeb\Library\JavaScript\Utils.js")
            Me.JsLinks.Add("ViewDomContractRates.js")
            strStyle.Append("label.label { float:left; width:6em; font-weight:bold; }")
            strStyle.Append("span.label { float:left; width:6em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(strStyle.ToString)

            PopulateContractDescription(domContractID)

            PopulatePeriodDatesCombo(domContractID, periodID)
            cboPeriodDates.DropDownList.Attributes.Add("onchange", "cboPeriodDates_Click();")
            PopulateRatesTable(domContractID, periodID)

        End Sub

        ''' <summary>
        ''' This procedure populates the PeriodDates combo box
        ''' </summary>
        ''' <param name="domContractID">Dom Contract ID</param>
        ''' <param name="periodID">Period ID</param>
        ''' <remarks></remarks>
        ''' <history>
        ''' PaulW   13-Feb-2008    Initial Version
        ''' </history>
        Private Sub PopulatePeriodDatesCombo(ByVal domContractID As Long, ByRef periodID As Long)
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            Dim currentPeriod As Long

            Const SP_NAME_FETCH_PERIOD As String = "spxDomContractPeriod_FetchList"
            ' grab the list of titles
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_PERIOD, False)
                spParams(0).Value = domContractID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_PERIOD, spParams)

                cboPeriodDates.DropDownList.Items.Clear()

                With cboPeriodDates.DropDownList
                    While reader.Read
                        .Items.Add(New ListItem(WebUtils.EncodeOutput(reader("DateFrom")) & " - " & WebUtils.EncodeOutput(reader("DateTo")), WebUtils.EncodeOutput(reader("ID"))))
                        If WebUtils.EncodeOutput(reader("DateFrom")) <= Today And WebUtils.EncodeOutput(reader("DateTo")) >= Today Then
                            currentPeriod = WebUtils.EncodeOutput(reader("ID"))
                        End If
                    End While
                End With
                If PeriodID <> 0 Then
                    cboPeriodDates.DropDownList.SelectedValue = PeriodID
                Else
                    If currentPeriod <> 0 Then
                        cboPeriodDates.DropDownList.SelectedValue = currentPeriod
                        periodID = currentPeriod
                    End If
                End If

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_PERIOD, "ViewDomContractRates.PopulatePeriodDatesCombo")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub
        ''' <summary>
        ''' Procedure to populate the rates table on screen
        ''' </summary>
        ''' <param name="domContractID">Dom contract id</param>
        ''' <param name="periodID">period id</param>
        ''' <remarks></remarks>
        ''' <history>
        ''' PaulW   13-Feb-2008    Initial Version
        ''' </history>
        Private Sub PopulateRatesTable(ByVal domContractID As Long, ByVal periodID As Long)
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_RATES As String = "spxDomContractViewRates_FetchList"
            ' grab the list of titles
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_RATES, False)
                spParams(0).Value = domContractID
                spParams(1).Value = PeriodID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_RATES, spParams)
                rptContractRates.DataSource = reader
                rptContractRates.DataBind()

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_RATES, "ViewDomContractRates.PopulateRatesTable")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub
        ''' <summary>
        ''' Used to populate contract details on the page
        ''' </summary>
        ''' <param name="domContractID"></param>
        ''' <remarks></remarks>
        ''' <history>
        ''' PaulW   13-Feb-2008    Initial Version
        ''' </history>
        Private Sub PopulateContractDescription(ByVal domContractID As Long)
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Const SP_NAME_FETCH_RATES As String = "spxDomContractView_Fetch"
            ' grab the list of titles
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_RATES, False)
                spParams(0).Value = domContractID
                spParams(1).Value = currentUser.ExternalUserID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_RATES, spParams)

                If reader.Read() Then
                    lblContract.Text = String.Format("{0} - {1}", reader("Number"), reader("Title"))
                Else
                    WebUtils.DisplayAccessDenied()
                End If

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_RATES, "ViewDomContractRates.PopulateRatesTable")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub

    End Class
End Namespace
