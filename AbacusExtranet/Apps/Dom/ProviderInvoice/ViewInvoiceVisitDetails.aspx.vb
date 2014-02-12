Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls

Namespace Apps.Dom.ProviderInvoice

    ''' <summary>
    ''' Screen to allow a user to view the details of an individual visit.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      18/08/2010  A4WA#6443 - corrected BackUrl creation.
    '''     JohnF       17/12/2009  Allow for flexible back navigation (#5966)
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewInvoiceVisitDetails
        Inherits Target.Web.Apps.BasePage

#Region " fields "
        Private _pScheduleId As String = String.Empty
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim visitID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchID"))
            Dim invoiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("invoiceID"))
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            Dim style As New StringBuilder
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            Dim hasAmendment As Boolean = False

            If currentPage <= 0 Then currentPage = 1

            CType(pSchedule, Apps.UserControls.PaymentScheduleHeader).SingleLiner = True

            Const SP_NAME_FETCH_INVOICEVISIT As String = "spxDomProviderInvoiceVisit_FetchComponentsForView"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ProviderInvoiceEnquiry"), "Provider Invoice - Visit Details")

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            style.Append("label.label { float:left; width:13em; font-weight:bold; }")
            style.Append("span.label { float:left; width:13em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

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
                    If Not IsDBNull(reader("Weekending")) Then lblWeekending.Text = Convert.ToDateTime(reader("Weekending")).ToString("dd MMM yyyy")
                    If Not IsDBNull(reader("ServiceType")) Then lblServiceType.Text = reader("ServiceType")
                    If Not IsDBNull(reader("VisitDate")) Then lblVisitDate.Text = Convert.ToDateTime(reader("VisitDate")).ToString("dd MMM yyyy")
                    If Not IsDBNull(reader("StartTimeClaimed")) Then lblStartTimeClaimed.Text = Convert.ToDateTime(reader("StartTimeClaimed")).ToString("HH:mm")
                    If Not IsDBNull(reader("DurationClaimed")) Then lblDurationClaimed.Text = Convert.ToDateTime(reader("DurationClaimed")).ToString("H\h m\m")
                    If Not IsDBNull(reader("VisitCode")) Then lblVisitCode.Text = reader("VisitCode")
                    If Not IsDBNull(reader("NumberOfCarers")) Then lblNumberOfCarers.Text = reader("NumberOfCarers")
                    If Not IsDBNull(reader("SecondaryVisit")) Then lblSecondaryVisit.Text = Utils.BooleanToYesNo(reader("SecondaryVisit"))
                    If Not IsDBNull(reader("ManuallyAmended")) Then lblManuallyAmended.Text = reader("ManuallyAmended")
                    If Not IsDBNull(reader("ActualStartTime")) Then lblActualStartTime.Text = Convert.ToDateTime(reader("ActualStartTime")).ToString("HH:mm")
                    If Not IsDBNull(reader("ActualDuration")) Then lblActualDuration.Text = Convert.ToDateTime(reader("ActualDuration")).ToString("H\h m\m")
                    If Not IsDBNull(reader("PreRoundedDurationClaimed")) Then lblPreRoundedDurationClaimed.Text = "[" & Convert.ToDateTime(reader("PreRoundedDurationClaimed")).ToString("H\h m\m") & "]"
                    'Amendment Requests
                    If Not IsDBNull(reader("StartTimeClaimedAmendment")) Then
                        If Convert.ToDateTime(reader("StartTimeClaimedAmendment")).ToString("HH:mm") <> Convert.ToDateTime(reader("StartTimeClaimed")).ToString("HH:mm") Then
                            hasAmendment = True
                        End If
                    End If
                    If Not IsDBNull(reader("DurationClaimedAmendment")) Then
                        If Convert.ToDateTime(reader("DurationClaimedAmendment")).ToString("H\h m\m") <> Convert.ToDateTime(reader("DurationClaimed")).ToString("H\h m\m") Then
                            hasAmendment = True
                        End If
                    End If
                    If Not IsDBNull(reader("VisitCodeAmendment")) Then
                        If reader("VisitCodeAmendment") <> reader("VisitCode") Then
                            hasAmendment = True
                        End If
                    End If
                    If Not IsDBNull(reader("ServiceTypeAmendment")) Then
                        If reader("ServiceTypeAmendment") <> reader("ServiceType") Then
                            hasAmendment = True
                        End If
                    End If

                End While


                _pScheduleId = Utils.ToInt32(Request.QueryString("pScheduleId"))
                btnView.Attributes.Add( _
                    "onclick", _
                    String.Format( _
                        "document.location.href='ViewAmendment.aspx?pScheduleId={0}&id={1}&mode={2}&backUrl={3}'", _
                        _pScheduleId, _
                        visitID, _
                        Convert.ToInt32(IIf(hasAmendment, StdButtonsMode.Fetched, StdButtonsMode.AddNew)), _
                        HttpUtility.UrlEncode(Request.Url.PathAndQuery) _
                    ) _
                )

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEVISIT, "ViewInvoiceVisitDetails.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            'Populate Carers Table
            PopulateCarersTable(visitID)

        End Sub

        Private Sub PopulateCarersTable(ByVal VisitID As Long)
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_CARERS As String = "spxDomProviderInvoiceVisit_CareWorker_FetchListForView"
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

        'Private Sub btnBack_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.ServerClick
        '    Me.CustomNavGoBack()
        'End Sub

        Private Sub btnBack_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.ServerClick
            Me.CustomNavRemoveLast()
            Me.CustomNavGoBack()
        End Sub

    End Class
End Namespace