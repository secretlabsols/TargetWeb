Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.Web.UserControls

Namespace Apps.Dom.ProviderInvoice

    ''' <summary>
    ''' Screen to allow a user to a visit that has been invoiced.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Waqas       20/09/2012  A4WA#7492 - Visit amendment button visibility from invoice visits screen
    '''     Waqas       21/05/2012  A4Wa#7366 - payment schedule link creation correction
    '''     MikeVO      18/08/2010  A4WA#6443 - corrected BackUrl creation.
    '''     ColinD      14/05/2010  D11739 - added actual duration
    '''     MikeVO      20/10/2009  D11546 - added Service Type amendment.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Public Class ViewInvoicedVisit
        Inherits Target.Web.Apps.BasePage

#Region " fields "
        Private _pScheduleId As Integer = 0
        Private _Retracted As Boolean = False
        Private _RetractedInvoiceId As Integer = 0
        Private _WeekEnding As DateTime
        Private _ContractId As Integer = 0
#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim visitID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            _pScheduleId = Target.Library.Utils.ToInt32(Request.QueryString("pscheduleid"))
            Dim strBackUrl As String = Request.QueryString("backurl")
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            Dim style As New StringBuilder
            Dim hasAmendment As Boolean

            If Not Me.IsPostBack Then

                Me.CustomNavAdd(True, strBackUrl)

                Me.CustomNavAdd(False)
            End If

            Const SP_NAME_FETCH_INVOICEDVISIT As String = "spxDomProviderInvoiceVisit_FetchForView"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.InvoicedVisit"), "View Invoiced Visit")

            style.Append("label.label { float:left; width:12em; font-weight:bold; }")
            style.Append("span.label { float:left; width:12em; padding-right:1em; font-weight:bold; }")
            style.Append(".Amendment {padding-left:2em; color:red; font-style:italic; )")
            Me.AddExtraCssStyle(style.ToString)

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_INVOICEDVISIT, False)
                spParams(0).Value = visitID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_INVOICEDVISIT, spParams)

                reader.Read()

                lblProvider.Text = String.Format("{0}/{1}", reader("ProviderReference"), reader("ProviderName"))
                lblContract.Text = String.Format("{0}/{1}", reader("ContractNumber"), reader("ContractTitle"))
                If Not IsDBNull(reader("ClientName")) Then
                    lblServiceUser.Text = String.Format("{0}/{1}", reader("ClientReference"), reader("ClientName"))
                End If
                If Not IsDBNull(reader("OrderReference")) Then lblOrderRef.Text = reader("OrderReference")
                If Not IsDBNull(reader("Weekending")) Then
                    lblWeekending.Text = Convert.ToDateTime(reader("Weekending")).ToString("dd MMM yyyy")
                    _WeekEnding = Convert.ToDateTime(reader("Weekending"))
                End If
                If Not IsDBNull(reader("ServiceType")) Then lblServiceType.Text = reader("ServiceType")
                If Not IsDBNull(reader("VisitDate")) Then lblVisitDate.Text = Convert.ToDateTime(reader("VisitDate")).ToString("dd MMM yyyy")
                If Not IsDBNull(reader("StartTimeClaimed")) Then lblStartTimeClaimed.Text = Convert.ToDateTime(reader("StartTimeClaimed")).ToString("HH:mm")
                If Not IsDBNull(reader("DurationClaimed")) Then lblDurationClaimed.Text = Convert.ToDateTime(reader("DurationClaimed")).ToString("H\h m\m")
                If Not IsDBNull(reader("PreRoundedDurationClaimed")) Then lblPreRoundedDurationClaimed.Text = "[" & Convert.ToDateTime(reader("PreRoundedDurationClaimed")).ToString("H\h m\m") & "]"
                If Not IsDBNull(reader("VisitCode")) Then lblVisitCode.Text = reader("VisitCode")
                If Not IsDBNull(reader("NumberOfCarers")) Then lblNumberCarers.Text = reader("NumberOfCarers").ToString
                If Not IsDBNull(reader("SecondaryVisit")) Then lblSecondaryVisit.Text = IIf(Convert.ToBoolean(reader("SecondaryVisit")), "Yes", "No")
                If Not IsDBNull(reader("MannuallyAmended")) Then lblManuallyAmended.Text = reader("MannuallyAmended")
                If Not IsDBNull(reader("ActualStartTime")) Then lblActualStartTime.Text = Convert.ToDateTime(reader("ActualStartTime")).ToString("HH:mm")
                If Not IsDBNull(reader("ActualDuration")) Then lblActualDuration.Text = Convert.ToDateTime(reader("ActualDuration")).ToString("H\h m\m")

                '' below line is commented out by Waqas.. do not assign _pSchedule id, it should be pased thought query string
                'If Not IsDBNull(reader("PaymentScheduleID")) Then _pScheduleId = reader("PaymentScheduleID")
                If Not IsDBNull(reader("RetractedInvoiceID")) Then _RetractedInvoiceId = reader("RetractedInvoiceID")
                If Not IsDBNull(reader("Retracted")) Then _Retracted = reader("Retracted")
                If Not IsDBNull(reader("ContractID")) Then _ContractId = reader("ContractID")


                'Amendment Requests
                If Not IsDBNull(reader("StartTimeClaimedAmendment")) Then
                    If Convert.ToDateTime(reader("StartTimeClaimedAmendment")).ToString("HH:mm") <> Convert.ToDateTime(reader("StartTimeClaimed")).ToString("HH:mm") Then
                        lblStartTimeClaimedAmendment.Text = String.Format("(Amendment Requested:{0})", Convert.ToDateTime(reader("StartTimeClaimedAmendment")).ToString("HH:mm"))
                        hasAmendment = True
                    End If
                End If
                If Not IsDBNull(reader("DurationClaimedAmendment")) Then
                    If Convert.ToDateTime(reader("DurationClaimedAmendment")).ToString("H\h m\m") <> Convert.ToDateTime(reader("DurationClaimed")).ToString("H\h m\m") Then
                        lblDurationClaimedAmendment.Text = String.Format("(Amendment Requested:{0})", Convert.ToDateTime(reader("DurationClaimedAmendment")).ToString("H\h m\m"))
                        hasAmendment = True
                    End If
                End If
                If Not IsDBNull(reader("ActualDurationAmendment")) Then
                    If Convert.ToDateTime(reader("ActualDurationAmendment")).ToString("H\h m\m") <> Convert.ToDateTime(reader("ActualDuration")).ToString("H\h m\m") Then
                        lblActualDurationAmendment.Text = String.Format("(Amendment Requested:{0})", Convert.ToDateTime(reader("ActualDurationAmendment")).ToString("H\h m\m"))
                        hasAmendment = True
                    End If
                End If
                If Not IsDBNull(reader("VisitCodeAmendment")) Then
                    If reader("VisitCodeAmendment") <> reader("VisitCode") Then
                        lblVisitCodeAmendment.Text = String.Format("(Amendment Requested:{0})", reader("VisitCodeAmendment"))
                        hasAmendment = True
                    End If
                End If
                If Not IsDBNull(reader("ServiceTypeAmendment")) Then
                    If reader("ServiceTypeAmendment") <> reader("ServiceType") Then
                        lblServiceTypeAmendment.Text = String.Format("(Amendment Requested:{0})", reader("ServiceTypeAmendment"))
                        hasAmendment = True
                    End If
                End If

                If Not IsDBNull(reader("SecondaryVisitAmendment")) Then
                    If reader("SecondaryVisitAmendment") <> reader("SecondaryVisit") Then
                        lblSecondaryVisitAmendment.Text = String.Format("(Amendment Requested:{0})", IIf(Convert.ToBoolean(reader("SecondaryVisitAmendment")), "Yes", "No"))
                        hasAmendment = True
                    End If
                End If

                If Not IsDBNull(reader("SecondaryVisitAutoSet")) Then lblSecondaryVisitAutoSet.Text = Utils.BooleanToYesNo(reader("SecondaryVisitAutoSet"))

                btnView.Attributes.Add( _
                    "onclick", _
                    String.Format( _
                        "document.location.href='ViewAmendment.aspx?id={0}&mode={1}&backUrl={2}'", _
                        visitID, _
                        Convert.ToInt32(IIf(hasAmendment, StdButtonsMode.Fetched, StdButtonsMode.AddNew)), _
                        HttpUtility.UrlEncode(Request.Url.PathAndQuery) _
                    ) _
                )

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEDVISIT, "ViewInvoicedVisit.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                SqlHelper.CloseReader(reader)
            End Try

            'Populate Carers Table
            PopulateCarersTable(visitID)


            CType(pSchedules, Apps.UserControls.PaymentScheduleHeader).SingleLiner = True
            CType(pSchedules, Apps.UserControls.PaymentScheduleHeader).PaymentScheduleId = _pScheduleId
            CType(pSchedules, Apps.UserControls.PaymentScheduleHeader).LabelWidth = "14.5em"

            ' should be at the end of this method. because its child method is using a property of user control 
            HandleVisitamendment()

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
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_CARERS, "ViewInvoicedVisit.PopulateCarersTable")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub

        Private Sub HandleVisitamendment()

            If _Retracted Or _RetractedInvoiceId > 0 Or Not IsWeekOpen() Then
                btnView.Visible = False
            End If

        End Sub


        Private Sub btnBack_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.ServerClick
            Me.CustomNavRemoveLast()
            Me.CustomNavGoBack()
        End Sub


        Private Function IsWeekOpen() As Boolean
            Dim open As Boolean = False
            If Utils.ToInt32(_ContractId) <> 0 Then
                Target.Abacus.Library.DomContractBL.IsContractWeekOpen(Me.DbConnection, _ContractId, _WeekEnding, open)
            End If

            Return open
        End Function



    End Class
End Namespace