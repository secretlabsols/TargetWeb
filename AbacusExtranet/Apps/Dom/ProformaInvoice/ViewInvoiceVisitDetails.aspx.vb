Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Namespace Apps.Dom.ProformaInvoice

    ''' <summary>
    ''' Screen to allow a user to view the details of an individual visit.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewInvoiceVisitDetails
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Pro forma Invoice - Visit Details")

            Dim visitID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchID"))
            Dim invoiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("invoiceID"))
            Dim pScheduleId As Integer = Target.Library.Utils.ToInt32(Request.QueryString("pScheduleId"))
            Dim invFilterAwait As String = Target.Library.Utils.ToString(Request.QueryString("await"))
            Dim invFilterVer As String = Target.Library.Utils.ToString(Request.QueryString("ver"))
            Dim backUrl As String = Target.Library.Utils.ToString(Request.QueryString("backUrl"))
            Dim _ignoreRounding As Boolean


            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            Dim style As New StringBuilder
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))



            If currentPage <= 0 Then currentPage = 1

            Const SP_NAME_FETCH_INVOICEVISIT As String = "spxDomProformaInvoiceVisit_FetchComponentsForView"



            Me.JsLinks.Add("TargetWeb\Library\JavaScript\Utils.js")
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            style.Append("label.label { float:left; width:13em; font-weight:bold; }")
            style.Append("span.label { float:left; width:13em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            'Set breadcrumb
            'With CType(breadCrumb, InvoiceBatchBreadcrumb)
            '    .BatchID = batchID
            '    .InvoiceID = invoiceID
            '    .VisitID = visitID
            '    .PaymentScheduleId = pScheduleId
            '    .InvFilterAwait = invFilterAwait
            '    .InvFilterVer = invFilterVer
            '    .backUrl = backUrl
            'End With

            If Not Me.IsPostBack Then
                Me.CustomNavAdd(False)
            End If

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_INVOICEVISIT, False)
                spParams(0).Value = visitID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_INVOICEVISIT, spParams)

                While reader.Read
                    lblProvider.Text = String.Format("{0}/{1}", reader("ProviderReference"), reader("ProviderName"))
                    lblContract.Text = String.Format("{0}/{1}", reader("ContractNumber"), reader("ContractTitle"))
                    If Not IsDBNull(reader("ClientName")) Then
                        lblServiceUser.Text = String.Format("{0}/{1}", reader("ClientReference"), reader("ClientName"))
                    End If
                    If Not IsDBNull(reader("OrderReference")) Then lblOrderReference.Text = reader("OrderReference")
                    If Not IsDBNull(reader("WETo")) Then lblWeekending.Text = Convert.ToDateTime(reader("WETo")).ToString("dd MMM yyyy")
                    If Not IsDBNull(reader("ServiceType")) Then lblServiceType.Text = reader("ServiceType")
                    If Not IsDBNull(reader("VisitDate")) Then lblVisitDate.Text = Convert.ToDateTime(reader("VisitDate")).ToString("dd MMM yyyy")
                    If Not IsDBNull(reader("StartTimeClaimed")) Then lblStartTimeClaimed.Text = Convert.ToDateTime(reader("StartTimeClaimed")).ToString("HH:mm")
                    If Not IsDBNull(reader("DurationClaimed")) Then lblDurationClaimed.Text = Convert.ToDateTime(reader("DurationClaimed")).ToString("H\h m\m")
                    If Not IsDBNull(reader("VisitCode")) Then lblVisitCode.Text = reader("VisitCode")
                    If Not IsDBNull(reader("NumberOfCarers")) Then lblNumberOfCarers.Text = reader("NumberOfCarers")
                    If Not IsDBNull(reader("SecondaryVisit")) Then lblSecondaryVisit.Text = Utils.BooleanToYesNo(reader("SecondaryVisit"))
                    If Not IsDBNull(reader("SecondaryVisitAutoSet")) Then lblSecondaryVisitAutoSet.Text = Utils.BooleanToYesNo(reader("SecondaryVisitAutoSet"))
                    If Not IsDBNull(reader("ManuallyAmended")) Then lblManuallyAmended.Text = reader("ManuallyAmended")
                    If Not IsDBNull(reader("ActualStartTime")) Then lblActualStartTime.Text = Convert.ToDateTime(reader("ActualStartTime")).ToString("HH:mm")
                    If Not IsDBNull(reader("ActualDuration")) Then lblActualDuration.Text = Convert.ToDateTime(reader("ActualDuration")).ToString("H\h m\m")
                    If Not IsDBNull(reader("PreRoundedDurationClaimed")) Then lblPreRoundedDurationClaimed.Text = "[" & Convert.ToDateTime(reader("PreRoundedDurationClaimed")).ToString("H\h m\m") & "]"
                    If Not IsDBNull(reader("IgnoreRounding")) Then _ignoreRounding = reader("IgnoreRounding")
                End While
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEVISIT, "ViewInvoiceVisitDetails.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            CType(pScheduleHeader, Apps.UserControls.PaymentScheduleHeader).SingleLiner = True
            CType(pScheduleHeader, Apps.UserControls.PaymentScheduleHeader).LabelWidth = "14.5em"
            'Populate Carers Table
            PopulateCarersTable(visitID)
            HandleIngnorerounding(pScheduleId, _ignoreRounding)

        End Sub

        Private Sub PopulateCarersTable(ByVal VisitID As Long)
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_CARERS As String = "spxDomProformaInvoiceVisit_CareWorker_FetchListForView"
            ' grab the list of titles
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_CARERS, False)
                spParams(0).Value = VisitID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_CARERS, spParams)
                rptCarers.DataSource = reader
                rptCarers.DataBind()

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_CARERS, "ViewInvoiceVisitDetails.PopulateCarersTable")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub


        Private Sub btnBack_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.ServerClick
            Me.CustomNavRemoveLast()
            Me.CustomNavGoBack()
        End Sub

        Private Sub HandleIngnorerounding(ByVal pScheduleId As Integer, ByVal IgnoreRounding As Boolean)
            ' if contract has rounding rules
            Dim msg As New ErrorMessage
            Dim pSchedule As DataClasses.PaymentSchedule = _
            New DataClasses.PaymentSchedule(Me.DbConnection, String.Empty, String.Empty)
            msg = pSchedule.Fetch(pScheduleId)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            Dim dcrContractList As DataClasses.Collections.DurationClaimedRoundingDomContractCollection = _
            New DataClasses.Collections.DurationClaimedRoundingDomContractCollection()
            Dim contractHasRoundingRules As Boolean = False
            msg = New ErrorMessage
            msg = DataClasses.DurationClaimedRoundingDomContract.FetchList(Me.DbConnection, dcrContractList, String.Empty, String.Empty, Nothing, pSchedule.DomContractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If dcrContractList.Count > 0 And IgnoreRounding Then
                contractHasRoundingRules = True
                imgIgnorerounding.ImageUrl = WebUtils.GetVirtualPath("images/Ignorerounding.png")
                imgIgnorerounding.ToolTip = "Duration Claimed Rounding rules are ignored"
                imgIgnorerounding.Visible = True
            Else
                imgIgnorerounding.Visible = False
            End If

            



        End Sub
    End Class
End Namespace