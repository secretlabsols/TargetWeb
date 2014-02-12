
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security

Imports Target.Abacus.Library
Imports System.Text
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library.Web.Controls
Imports Target.Web.Apps
Imports Target.Abacus.Library.DomProviderInvoice

Namespace Apps.InPlaceSelectors
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class InPlaceInvoiceVisitsCopier
        Inherits Target.Web.Apps.BasePage

#Region " Constants "
        Const Invoice As String = "Invoice"
#End Region

#Region " Private variables "
        Private _serviceTypes As List(Of ViewablePair) = Nothing

        Private _copyToCareWorker As String
        Private _copyToDayOfWeek As eInvoice.DailyVisitDetail.WeekDays
        Private _copyToCareWorkerReference As String
#End Region

#Region " Property "

        'Private _inv As eInvoice.Invoice
        Public Property inv() As eInvoice.Invoice
            Get
                Dim invoice As eInvoice.Invoice = New eInvoice.Invoice
                If Not Session("inv") Is Nothing Then
                    invoice = Session("inv")
                End If
                Return invoice
            End Get
            Set(ByVal value As eInvoice.Invoice)
                Session("inv") = value
            End Set
        End Property

#End Region

#Region " Page events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.EnableTimeout = False
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Duration Claimed Rounding")

            Dim js As String
            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/InPlaceSelectors/InPlaceInvoiceVisitsCopier.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))

            '' fill values from querystring 
            _copyToCareWorker = Utils.ToString(Request.QueryString("copyToCareWorker"))
            _copyToDayOfWeek = Utils.ToInt32(Request.QueryString("copyToDayOfWeek"))
            _copyToCareWorkerReference = Utils.ToString(Request.QueryString("copyToCareWorkerReference"))


            If Not Page.IsPostBack Then
                btnSelected.Disabled = True
                btnSelectAll.Disabled = True
                btnSelectNone.Disabled = True

                PopulateVisitdayDropdown(ddlCopyFromDayofWeek)
                PopulateCareWorkerDropdown(inv, ddlCopyFromCareWorker)

                PopulateVisitdayDropdown(ddlCopyToDayOfWeek, _copyToDayOfWeek)
                PopulateCareWorkerDropdown(inv, ddlCopyToCareWorker, _copyToCareWorker)
                'BindGridData(GetRequestedVisits(_copyFromCareWorker, _copyFromDayOfWeek))
            End If


            js = String.Format("totalChkBoxes={0};gvControlID=""{1}"";hidSelectedObjectIndexID=""{2}"";copyToCareWorker=""{3}"";careWorkerReference=""{4}"";btnSelectedID=""{5}"";", _
                txtGridCount.Value, gvVisits.ClientID, hidSelectedObjectIndex.ClientID, _copyToCareWorker, _copyToCareWorkerReference, btnSelected.ClientID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

#End Region

#Region " Events "
        Private Sub gvVisits_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvVisits.RowDataBound
            If e.Row.RowType = DataControlRowType.DataRow Then
                'e.Row.RowType = DataControlRowType.Footer _
                'e.Row.RowType = DataControlRowType.EmptyDataRow _

                ' if row is footer row then only fill the drop down list and donot try to assign the value. as dataitem is null
                Dim isFooter As Boolean = e.Row.RowType = DataControlRowType.Footer
                Dim isEmptyDataRow As Boolean = e.Row.RowType = DataControlRowType.EmptyDataRow

                If e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Or e.Row.RowState = DataControlRowState.Insert Then
                    Dim selectedValue As String = "0"
                    Dim ddlHours As DropDownList
                    Dim ddlMinutes As DropDownList
                    Dim ddlServiceType As DropDownList
                    Dim ddlVisitCode As DropDownList
                    Dim hiddenObjectIndex As HiddenField
                    Dim chkObjectIndex As CheckBox

                    ' hiddelField
                    hiddenObjectIndex = e.Row.FindControl("hiddenObjectIndex")
                    If Not hiddenObjectIndex Is Nothing Then
                        hiddenObjectIndex.Value = CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                          Target.Abacus.Library.eInvoice.DailyVisitDetail).ObjectIndex
                    End If
                    '' set the value in hidden field to set all the check boxes pre set to checked on load
                    If hidSelectedObjectIndex.Value.ToString().Trim().Length = 0 Then
                        hidSelectedObjectIndex.Value = hiddenObjectIndex.Value
                    Else
                        hidSelectedObjectIndex.Value = hidSelectedObjectIndex.Value & "|" & hiddenObjectIndex.Value
                    End If

                    ' check box 
                    chkObjectIndex = e.Row.FindControl("chkObjectIndex")
                    If Not chkObjectIndex Is Nothing Then
                        chkObjectIndex.Checked = True
                        chkObjectIndex.Attributes.Add("onclick", _
                                                      String.Format("CheckedChanged(""{0}"",""{1}"")", _
                                                                    chkObjectIndex.ClientID, hiddenObjectIndex.ClientID))
                    End If

                    ' service type 
                    ddlServiceType = e.Row.FindControl("ddlServiceType")
                    If Not ddlServiceType Is Nothing Then
                        ddlServiceType.Items.Clear()
                        PopulateServiceTypeDropdown(ddlServiceType, inv.ContractID)
                    End If
                    Dim isNew As Boolean = False
                    If Not isFooter And Not isEmptyDataRow Then
                        If ddlServiceType.Items.Count = 2 Then
                            isNew = CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                          Target.Abacus.Library.eInvoice.DailyVisitDetail).MarkedToAdd
                        End If
                        If isNew Then
                            selectedValue = 1
                        Else
                            selectedValue = _
                          CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                          Target.Abacus.Library.eInvoice.DailyVisitDetail).ServiceTypeID
                        End If
                    End If
                    ddlServiceType.SelectedValue = selectedValue
                    ' start time hours
                    ddlHours = e.Row.FindControl("ddlStartTimeHours")
                    If Not ddlHours Is Nothing Then
                        ddlHours.Items.Clear()
                        PopulateDropDownList(ddlHours, True)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).StartTimeHours
                        ddlHours.SelectedValue = selectedValue
                    End If
                    ' start time minutes
                    ddlMinutes = e.Row.FindControl("ddlStartTimeMinutes")
                    If Not ddlMinutes Is Nothing Then
                        ddlMinutes.Items.Clear()
                        PopulateDropDownList(ddlMinutes, False)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).StartTimeMinutes
                        ddlMinutes.SelectedValue = selectedValue
                    End If
                    ' end Time hours
                    ddlHours = e.Row.FindControl("ddlEndTimeHours")
                    If Not ddlHours Is Nothing Then
                        ddlHours.Items.Clear()
                        PopulateDropDownList(ddlHours, True)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).EndTimeHours
                        ddlHours.SelectedValue = selectedValue
                    End If
                    ' End Time minutes
                    ddlMinutes = e.Row.FindControl("ddlEndTimeMinutes")
                    If Not ddlMinutes Is Nothing Then
                        ddlMinutes.Items.Clear()
                        PopulateDropDownList(ddlMinutes, False)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).EndTimeMinutes
                        ddlMinutes.SelectedValue = selectedValue
                    End If
                    ' Duration Claimed hours

                    ddlHours = e.Row.FindControl("ddlDurationClaimedHours")
                    If Not ddlHours Is Nothing Then
                        ddlHours.Items.Clear()
                        PopulateDropDownList(ddlHours, True)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                                        Target.Abacus.Library.eInvoice.DailyVisitDetail).DurationClaimedHours
                        ddlHours.SelectedValue = selectedValue
                    End If
                    ' Duration claimed minutes
                    ddlMinutes = e.Row.FindControl("ddlDurationClaimedMinutes")
                    If Not ddlMinutes Is Nothing Then
                        ddlMinutes.Items.Clear()
                        PopulateDropDownList(ddlMinutes, False)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).DurationClaimedMinutes
                        ddlMinutes.SelectedValue = selectedValue
                    End If
                    ' Actual duration hours
                    ddlHours = e.Row.FindControl("ddlActualDurationHours")
                    If Not ddlHours Is Nothing Then
                        ddlHours.Items.Clear()
                        PopulateDropDownList(ddlHours, True)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).ActualDurationHours
                        ddlHours.SelectedValue = selectedValue
                    End If
                    ' Actual duration minutes
                    ddlMinutes = e.Row.FindControl("ddlActualDurationMinutes")
                    If Not ddlMinutes Is Nothing Then
                        ddlMinutes.Items.Clear()
                        PopulateDropDownList(ddlMinutes, False)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                        CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                        Target.Abacus.Library.eInvoice.DailyVisitDetail).ActualDurationMinutes
                        ddlMinutes.SelectedValue = selectedValue
                    End If

                    ' Visit Code 
                    ddlVisitCode = e.Row.FindControl("ddlVisitCode")
                    If Not ddlVisitCode Is Nothing Then
                        ddlVisitCode.Items.Clear()
                        PopulateVisitCodeDropdown(ddlVisitCode, 0, inv.ContractID, inv.WETo)
                    End If
                    If Not isFooter And Not isEmptyDataRow Then
                        selectedValue = _
                      CType(CType(CType(e.Row, System.Web.UI.WebControls.GridViewRow).DataItem, System.Object),  _
                      Target.Abacus.Library.eInvoice.DailyVisitDetail).VisitCodeID
                        ddlVisitCode.SelectedValue = selectedValue
                    End If

                End If
            End If
        End Sub

        Private Sub ddlCopyFromCareWorker_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCopyFromCareWorker.SelectedIndexChanged
            hidSelectedObjectIndex.Value = ""
            BindGridData(GetRequestedVisits(ddlCopyFromCareWorker.SelectedValue, ddlCopyFromDayofWeek.SelectedValue))
        End Sub


        Private Sub ddlCopyFromDayofWeek_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCopyFromDayofWeek.SelectedIndexChanged
            hidSelectedObjectIndex.Value = ""
            BindGridData(GetRequestedVisits(ddlCopyFromCareWorker.SelectedValue, ddlCopyFromDayofWeek.SelectedValue))
            btnSelected.Disabled = False
        End Sub

        Private Sub BindGridData(ByVal listSelectedDayVisits As List(Of eInvoice.DailyVisitDetail))
            If listSelectedDayVisits.Count > 0 Then
                btnSelectAll.Disabled = False
                btnSelectNone.Disabled = False
                txtGridCount.Value = listSelectedDayVisits.Count
            End If
            gvVisits.DataSource = listSelectedDayVisits
            gvVisits.DataBind()
        End Sub
#End Region

#Region " Methods "

#Region " Dropdown for hours and minutes "

        Private Sub PopulateDropDownList(ByRef ddl As DropDownList, ByVal hours As Boolean)
            For index As Integer = 0 To 59
                ddl.Items.Add(New ListItem(index.ToString.PadLeft(2, "0"c), index))
                If hours And index = 23 Then
                    Exit For
                End If
            Next
        End Sub

#End Region

#Region " PopulateServiceTypeDropdown "

        Private Sub PopulateServiceTypeDropdown(ByRef dropdown As DropDownList, ByVal ContractId As Integer)

            Dim msg As ErrorMessage

            Dim thePage As Target.Web.Apps.BasePage = CType(Me.Page, Target.Web.Apps.BasePage)

            'SqlHelper.GetConnection(
            If _serviceTypes Is Nothing Then
                msg = DomContractBL.FetchServiceTypesAvailableToContract(thePage.DbConnection, ContractId, _serviceTypes)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            With dropdown
                .Items.Clear()
                .DataSource = _serviceTypes
                .DataTextField = "Text"
                .DataValueField = "Value"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region " PopulateVisitCodeDropdown "

        Private Sub PopulateVisitCodeDropdown(ByRef dropdown As DropDownList, _
                                              ByVal uniqueID As String, _
                                              ByVal ContractId As Integer, _
                                              ByVal WeekEnding As Date)

            Dim msg As ErrorMessage
            Dim visitDate As Date = WeekEnding
            ''Dim postedBackDoW As String = CType(phVisits.FindControl(CTRL_PREFIX_VISIT_DOW & uniqueID), DropDownListEx).GetPostBackValue()
            ''Dim postedBackVisitCode As String = CType(phVisits.FindControl(CTRL_PREFIX_VISIT_CODE & uniqueID), DropDownListEx).GetPostBackValue()
            ''Dim dow As DayOfWeek
            Dim visitCodes As DomVisitCodeCollection = Nothing

            ' if we have a dow then we should use that to work back from the week ending date
            ''If Not postedBackDoW Is Nothing AndAlso postedBackDoW.Length > 0 Then
            '' dow = [Enum].Parse(GetType(DayOfWeek), postedBackDoW)
            ''While visitDate.DayOfWeek <> dow
            '' visitDate = visitDate.AddDays(-1)
            ''End While
            ''End If
            Dim thePage As Target.Web.Apps.BasePage = CType(Me.Page, Target.Web.Apps.BasePage)
            msg = DomContractBL.FetchVisitCodesAvailableForVisit(thePage.DbConnection, ContractId, visitDate, visitCodes)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' load the dropdown
            With dropdown
                .Items.Clear()
                .DataSource = visitCodes
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)

                'If Not postedBackVisitCode Is Nothing AndAlso postedBackVisitCode.Length > 0 Then
                '    ' select posted back value
                '    .SelectedValue = postedBackVisitCode
                'Else
                '    ' select default
                '    For Each code As DomVisitCode In visitCodes
                '        If code.DefaultCode Then
                '            .SelectedValue = code.ID
                '            Exit For
                '        End If
                '    Next
                'End If
            End With

        End Sub

#End Region

#Region " Populate Visit Day Dropdown"

        Private Sub PopulateVisitdayDropdown(ByRef dropdown As DropDownList, _
                                             Optional ByVal selectedValue As Integer = 0)
            dropdown.Items.Add(New ListItem("", 0))
            dropdown.Items.Add(New ListItem("Monday", eInvoice.DailyVisitDetail.WeekDays.Monday))
            dropdown.Items.Add(New ListItem("Tuesday", eInvoice.DailyVisitDetail.WeekDays.Tuesday))
            dropdown.Items.Add(New ListItem("Wednesday", eInvoice.DailyVisitDetail.WeekDays.Wednesday))
            dropdown.Items.Add(New ListItem("Thursday", eInvoice.DailyVisitDetail.WeekDays.Thursday))
            dropdown.Items.Add(New ListItem("Friday", eInvoice.DailyVisitDetail.WeekDays.Friday))
            dropdown.Items.Add(New ListItem("Saturday", eInvoice.DailyVisitDetail.WeekDays.Saturday))
            dropdown.Items.Add(New ListItem("Sunday", eInvoice.DailyVisitDetail.WeekDays.Sunday))

            dropdown.SelectedValue = selectedValue
        End Sub

#End Region

#Region " Populate Care Worker Dropdown"

        Private Sub PopulateCareWorkerDropdown(ByVal inv As eInvoice.Invoice, _
                                               ByRef dropdown As DropDownList, _
                                            Optional ByVal selectedValue As String = "")

            dropdown.Items.Add(New ListItem("", 0))
            For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                dropdown.Items.Add(New ListItem(cProvider.CareProviderName, _
                                                cProvider.CareProviderName _
                                                ))
            Next
            If dropdown.Items.Count = 2 Then
                dropdown.SelectedIndex = dropdown.Items.Count - 1
                dropdown.Enabled = False
            Else
                dropdown.SelectedValue = selectedValue
            End If

        End Sub
#End Region

#Region " Get Requested Visits "

        Private Function GetRequestedVisits(ByVal cProvierName As String, _
                                            ByVal visitDay As eInvoice.DailyVisitDetail.WeekDays) As  _
                                           List(Of eInvoice.DailyVisitDetail)

            Dim listVisitDetail As New List(Of eInvoice.DailyVisitDetail)
            For Each cProvider As eInvoice.CareProvider In inv.ListCareProvider
                If cProvider.CareProviderName.ToLower() = cProvierName.ToLower() Then
                    If visitDay = eInvoice.DailyVisitDetail.WeekDays.Monday Then
                        listVisitDetail = cProvider.listDailyVisitMonday
                    ElseIf visitDay = eInvoice.DailyVisitDetail.WeekDays.Tuesday Then
                        listVisitDetail = cProvider.listDailyVisitTuesday
                    ElseIf visitDay = eInvoice.DailyVisitDetail.WeekDays.Wednesday Then
                        listVisitDetail = cProvider.listDailyVisitWednesday
                    ElseIf visitDay = eInvoice.DailyVisitDetail.WeekDays.Thursday Then
                        listVisitDetail = cProvider.listDailyVisitThursday
                    ElseIf visitDay = eInvoice.DailyVisitDetail.WeekDays.Friday Then
                        listVisitDetail = cProvider.listDailyVisitFriday
                    ElseIf visitDay = eInvoice.DailyVisitDetail.WeekDays.Saturday Then
                        listVisitDetail = cProvider.listDailyVisitSaturday
                    ElseIf visitDay = eInvoice.DailyVisitDetail.WeekDays.Sunday Then
                        listVisitDetail = cProvider.listDailyVisitSunday
                    End If
                    'listVisitDetail = cProvider.GetVisitsByVisitDay(visitDay)
                End If
            Next
            If listVisitDetail.Count = 0 Then
                btnSelectAll.Disabled = True
                btnSelectNone.Disabled = False
            End If
            Return listVisitDetail

        End Function

#End Region

#End Region

    End Class

End Namespace