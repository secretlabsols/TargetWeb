Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Extensions.AjaxControlToolkit
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps
Imports RecurrenceGenerator

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.RecurrencePattern
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of domiciliary provider invoices.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	PaulW   	24/06/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class RecurrencePattern
        Inherits System.Web.UI.UserControl
        Private _friendlyDesc As String
        Private _mode As Int32
        Private _updatingFriendlyText As Boolean = False
        Public Sub InitControl(ByVal thePageMode As Int32)
            _mode = thePageMode
        End Sub


        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

            With CType(Me.Page, BasePage)
                .JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
                .JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
                .JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/RecurrencePattern.js"))
            End With

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Const OPT_STYLE As String = "float:left;"
            Dim tempString As String
            Dim js As String

            With CType(Me.Page, BasePage)

                js = String.Format("recurrencePatternID='{0}';txtDailyNoDaysID='{1}';txtMonthlyDayNoID='{2}';txtMonthlyDayMonthsID='{3}';" & _
                                   "cboMonthlyTypeID='{4}';cboMonthlyDayID='{5}';txtMonthlyDayMonths2ID='{6}';txtMonthlyPlusMinusID='{7}';" & _
                                   "cboYearlyEveryMonthID='{8}';txtYearlyEveryMonthNoID='{9}';cboYearlyTypeID='{10}';cboYearlyType2ID='{11}';" & _
                                   "cboYearlyMonthID='{12}';txtYearlyPlusMinusID='{13}';optDailyEveryXDayID='{14}';optDailyEveryWeekDayID='{15}';" & _
                                   "optMonthlyDayofMonthID='{16}';optMonthlyPatternID='{17}';optYearlyEveryID='{18}';optEndOnID='{19}';" & _
                                   "dteDateToID='{20}';txtOccurrencesID='{21}';RecurrencePattern_mode={22};", _
                                   Me.ClientID, txtDailyDays.ClientID, txtMonthlyDayNo.ClientID, txtMonthlyMonth.ClientID, _
                                   cboMonthlyDayType.ClientID, cboMonthlyTypeDesc.ClientID, txtMonthlyMonth2.ClientID, txtMonthlyPlusMinus.ClientID, _
                                   cboYearlyEveryMonth.ClientID, txtYearlyMonthNo.ClientID, cboYearlyDayType.ClientID, cboYearlyTypedesc.ClientID, _
                                   cboYearlyMonth.ClientID, txtYearlyPluMinus.ClientID, optDailyEveryXDay.ClientID, optDailyEveryWeekDay.ClientID, _
                                   optMonthlyDay.ClientID, optMonthlyPatternDays.ClientID, optYearlyEvery.ClientID, optEndOn.ClientID, _
                                   dteDateTo.ClientID, txtOccurrences.ClientID, _mode)

                .ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))
            End With


            With optDailyEveryXDay
                .InputAttributes.Add("style", OPT_STYLE)
                .LabelAttributes.Add("style", OPT_STYLE)
            End With

            With optDailyEveryWeekDay
                .InputAttributes.Add("style", OPT_STYLE)
                .LabelAttributes.Add("style", OPT_STYLE)
            End With

            With optMonthlyDay
                .InputAttributes.Add("style", OPT_STYLE)
                .LabelAttributes.Add("style", OPT_STYLE)
            End With

            With optMonthlyPatternDays
                .InputAttributes.Add("style", OPT_STYLE)
                .LabelAttributes.Add("style", OPT_STYLE)
            End With

            With optYearlyEvery
                .InputAttributes.Add("style", OPT_STYLE)
                .LabelAttributes.Add("style", OPT_STYLE)
            End With

            With optYearlyPattern
                .InputAttributes.Add("style", OPT_STYLE)
                .LabelAttributes.Add("style", OPT_STYLE)
            End With


            With optEndOn
                .InputAttributes.Add("style", OPT_STYLE)
                .LabelAttributes.Add("style", OPT_STYLE)
            End With

            With optEndAfter
                .InputAttributes.Add("style", OPT_STYLE)
                .LabelAttributes.Add("style", OPT_STYLE)
            End With

            With chkWeeklyMonday.CheckBox
                .InputAttributes.Add("style", "float:left;")
                .Text = "Monday"
                .TextAlign = TextAlign.Right
                .Width = New Unit(10, UnitType.Em)
            End With
            With chkWeeklyTuesday.CheckBox
                .InputAttributes.Add("style", "float:left;")
                .Text = "Tuesday"
                .TextAlign = TextAlign.Right
                .Width = New Unit(10, UnitType.Em)
            End With
            With chkWeeklyWednesday.CheckBox
                .InputAttributes.Add("style", "float:left;")
                .Text = "Wednesday"
                .TextAlign = TextAlign.Right
                .Width = New Unit(10, UnitType.Em)
            End With
            With chkWeeklyThursday.CheckBox
                .InputAttributes.Add("style", "float:left;")
                .Text = "Thursday"
                .TextAlign = TextAlign.Right
                .Width = New Unit(10, UnitType.Em)
            End With
            With chkWeeklyFriday.CheckBox
                .InputAttributes.Add("style", "float:left;")
                .Text = "Friday"
                .TextAlign = TextAlign.Right
                .Width = New Unit(10, UnitType.Em)
            End With
            With chkWeeklySaturday.CheckBox
                .InputAttributes.Add("style", "float:left;")
                .Text = "Saturday"
                .TextAlign = TextAlign.Right
                .Width = New Unit(10, UnitType.Em)
            End With
            With chkWeeklySunday.CheckBox
                .InputAttributes.Add("style", "float:left;")
                .Text = "Sunday"
                .TextAlign = TextAlign.Right
                .Width = New Unit(10, UnitType.Em)
            End With


            If _mode <> StdButtonsMode.AddNew And _mode <> StdButtonsMode.Edit Then
                WebUtils.RecursiveDisable(Me.Controls, True)
            Else
                WebUtils.RecursiveDisable(Me.Controls, False)
                InitialEnable()
            End If

            If Not IsPostBack Then
                If _mode = StdButtonsMode.AddNew Then
                    hidSelectedTab.Value = String.Empty
                    SetDefaults()
                End If
                tabStrip.GetTabByHeaderText(hidSelectedTab.Value).Enabled = True
                tabStrip.SetActiveTabByHeaderText(hidSelectedTab.Value)
                tempString = GetRecurringPattern()
                lblFriendly.Text = _friendlyDesc
                lblnextRunDate.Text = String.Format("The next calculated occurrence date is '{0}'", RecurrenceHelper.GetNextDate(Date.Now, tempString))
            Else
                tabStrip.SetActiveTabByHeaderText(hidSelectedTab.Value)
                If _mode = StdButtonsMode.Edit Then
                    InitialEnable()
                End If

            End If

        End Sub

        Private Sub PopulateCombos()

            With cboMonthlyDayType.DropDownList
                .DataSource = [Enum].GetNames(GetType(MonthlySpecificDatePartOne))
                .DataBind()
            End With
            With cboMonthlyTypeDesc.DropDownList
                .DataSource = [Enum].GetNames(GetType(MonthlySpecificDatePartTwo))
                .DataBind()
            End With


            With cboYearlyEveryMonth.DropDownList
                .DataSource = [Enum].GetNames(GetType(YearlySpecificDatePartThree))
                .DataBind()
            End With
            With cboYearlyDayType.DropDownList
                .DataSource = [Enum].GetNames(GetType(YearlySpecificDatePartOne))
                .DataBind()
            End With
            With cboYearlyTypedesc.DropDownList
                .DataSource = [Enum].GetNames(GetType(YearlySpecificDatePartTwo))
                .DataBind()
            End With
            With cboYearlyMonth.DropDownList
                .DataSource = [Enum].GetNames(GetType(YearlySpecificDatePartThree))
                .DataBind()
            End With
        End Sub

        Public Function GetRecurringPattern() As String
            Dim tabHeaderText As String = hidSelectedTab.Value
            Dim values As RecurrenceValues = Nothing
            Dim dateTo As Date
            Dim t As TimeSpan = New TimeSpan(Convert.ToInt32(tmeStartDate.Hours), Convert.ToInt32(tmeStartDate.Minutes), 0)

            hidSelectedTab.Value = tabHeaderText

            If dteDateTo.Text = "" Then
                dateTo = Convert.ToDateTime("31/12/2099")
            Else
                dateTo = Convert.ToDateTime(dteDateTo.Text)
            End If

            Select Case tabHeaderText
                Case RecurrenceGenerator.RecurrenceType.Daily.ToString()
                    Dim daily As DailyRecurrenceSettings

                    If optEndAfter.Checked Then
                        daily = New DailyRecurrenceSettings(Convert.ToDateTime(dteDateFrom.Value), t, Convert.ToInt32(txtOccurrences.Value))
                    Else
                        daily = New DailyRecurrenceSettings(Convert.ToDateTime(dteDateFrom.Value), t, dateTo)
                    End If

                    If optDailyEveryXDay.Checked Then
                        values = daily.GetValues(Convert.ToInt32(txtDailyDays.Text))
                    Else
                        values = daily.GetValues(1, DailyRegenType.OnEveryWeekday)
                    End If

                Case RecurrenceGenerator.RecurrenceType.Weekly.ToString()
                    Dim weekly As WeeklyRecurrenceSettings
                    Dim selectedValues As SelectedDayOfWeekValues = New SelectedDayOfWeekValues

                    If optEndAfter.Checked Then
                        weekly = New WeeklyRecurrenceSettings(Convert.ToDateTime(dteDateFrom.Value), t, Convert.ToInt32(txtOccurrences.Value))
                    Else
                        weekly = New WeeklyRecurrenceSettings(Convert.ToDateTime(dteDateFrom.Value), t, dateTo)
                    End If

                    selectedValues.Sunday = chkWeeklySunday.CheckBox.Checked
                    selectedValues.Monday = chkWeeklyMonday.CheckBox.Checked
                    selectedValues.Tuesday = chkWeeklyTuesday.CheckBox.Checked
                    selectedValues.Wednesday = chkWeeklyWednesday.CheckBox.Checked
                    selectedValues.Thursday = chkWeeklyThursday.CheckBox.Checked
                    selectedValues.Friday = chkWeeklyFriday.CheckBox.Checked
                    selectedValues.Saturday = chkWeeklySaturday.CheckBox.Checked

                    values = weekly.GetValues(Convert.ToInt32(txtWeeklyNo.Text), selectedValues)

                Case RecurrenceGenerator.RecurrenceType.Monthly.ToString()
                    Dim monthly As MonthlyRecurrenceSettings

                    If optEndAfter.Checked Then
                        monthly = New MonthlyRecurrenceSettings(Convert.ToDateTime(dteDateFrom.Value), t, Convert.ToInt32(txtOccurrences.Value))
                    Else
                        monthly = New MonthlyRecurrenceSettings(Convert.ToDateTime(dteDateFrom.Value), t, dateTo)
                    End If

                    If optMonthlyDay.Checked Then
                        values = monthly.GetValues(Convert.ToInt32(txtMonthlyDayNo.Text), Convert.ToInt32(txtMonthlyMonth.Text))
                    Else
                        monthly.AdjustmentValue = Convert.ToInt32(txtMonthlyPlusMinus.Text)
                        If Me.IsPostBack And Not (_mode = StdButtonsMode.Fetched) Then

                            values = monthly.GetValues([Enum].Parse(GetType(MonthlySpecificDatePartOne), cboMonthlyDayType.GetPostBackValue()), _
                                                       [Enum].Parse(GetType(MonthlySpecificDatePartTwo), cboMonthlyTypeDesc.GetPostBackValue()), _
                                                       Convert.ToInt32(txtMonthlyMonth2.Text))
                        Else

                            values = monthly.GetValues([Enum].Parse(GetType(MonthlySpecificDatePartOne), cboMonthlyDayType.DropDownList.SelectedValue), _
                                                       [Enum].Parse(GetType(MonthlySpecificDatePartTwo), cboMonthlyTypeDesc.DropDownList.SelectedValue), _
                                                       Convert.ToInt32(txtMonthlyMonth2.Text))
                        End If

                    End If


                Case RecurrenceGenerator.RecurrenceType.Yearly.ToString()
                    Dim yearly As YearlyRecurrenceSettings

                    If optEndAfter.Checked Then
                        yearly = New YearlyRecurrenceSettings(Convert.ToDateTime(dteDateFrom.Value), t, Convert.ToInt32(txtOccurrences.Value))
                    Else
                        yearly = New YearlyRecurrenceSettings(Convert.ToDateTime(dteDateFrom.Value), t, dateTo)
                    End If

                    If optYearlyEvery.Checked Then
                        If Me.IsPostBack And Not (_mode = StdButtonsMode.Fetched) Then
                            values = yearly.GetValues(Convert.ToInt32(txtYearlyMonthNo.Text), _
                                                      [Enum].Parse(GetType(YearlySpecificDatePartThree), cboYearlyEveryMonth.GetPostBackValue()))
                        Else
                            values = yearly.GetValues(Convert.ToInt32(txtYearlyMonthNo.Text), _
                                                      [Enum].Parse(GetType(YearlySpecificDatePartThree), cboYearlyEveryMonth.DropDownList.SelectedValue))
                        End If
                    Else
                        'Get the adjusted value
                        yearly.AdjustmentValue = Convert.ToInt32(txtYearlyPluMinus.Text)

                        If Me.IsPostBack And Not (_mode = StdButtonsMode.Fetched) Then

                            values = yearly.GetValues([Enum].Parse(GetType(YearlySpecificDatePartOne), cboYearlyDayType.GetPostBackValue()), _
                                                      [Enum].Parse(GetType(YearlySpecificDatePartTwo), cboYearlyTypedesc.GetPostBackValue()), _
                                                      [Enum].Parse(GetType(YearlySpecificDatePartThree), cboYearlyMonth.GetPostBackValue()))
                        Else

                            values = yearly.GetValues([Enum].Parse(GetType(YearlySpecificDatePartOne), cboYearlyDayType.DropDownList.SelectedValue), _
                                                      [Enum].Parse(GetType(YearlySpecificDatePartTwo), cboYearlyTypedesc.DropDownList.SelectedValue), _
                                                      [Enum].Parse(GetType(YearlySpecificDatePartThree), cboYearlyMonth.DropDownList.SelectedValue))

                        End If
                    End If


            End Select

            If values Is Nothing Then
                Return String.Empty
            Else
                _friendlyDesc = RecurrenceHelper.GetRecurrenceDescription(values.GetSeriesInfo())
                Return values.GetSeriesInfo()
            End If


        End Function

        Public Sub SetRecurrencePattern(ByVal recPattern As String)

            Dim recInfo As RecurrenceInfo
            Dim strTemp As String

            'Set the defaults to start with

            SetDefaults()

            recInfo = RecurrenceHelper.GetFriendlySeriesInfo(recPattern)
            hidSelectedTab.Value = recInfo.RecurrenceType.ToString()

            With recInfo
                Select Case .RecurrenceType
                    Case RecurrenceType.Daily
                        tabStrip.ActiveTabIndex = RecurrenceType.Daily
                        If .DailyRegenType = DailyRegenType.OnEveryXDays Then
                            optDailyEveryXDay.Checked = True
                            txtDailyDays.Text = .DailyRegenEveryXDays
                        Else
                            optDailyEveryWeekDay.Checked = True
                        End If
                    Case RecurrenceType.Weekly
                        tabStrip.ActiveTabIndex = RecurrenceType.Weekly
                        txtWeeklyNo.Text = .WeeklyRegenEveryXWeeks
                        chkWeeklyMonday.CheckBox.Checked = .WeeklySelectedDays.Monday
                        chkWeeklyTuesday.CheckBox.Checked = .WeeklySelectedDays.Tuesday
                        chkWeeklyWednesday.CheckBox.Checked = .WeeklySelectedDays.Wednesday
                        chkWeeklyThursday.CheckBox.Checked = .WeeklySelectedDays.Thursday
                        chkWeeklyFriday.CheckBox.Checked = .WeeklySelectedDays.Friday
                        chkWeeklySaturday.CheckBox.Checked = .WeeklySelectedDays.Saturday
                        chkWeeklySunday.CheckBox.Checked = .WeeklySelectedDays.Sunday


                    Case RecurrenceType.Monthly
                        tabStrip.ActiveTabIndex = RecurrenceType.Monthly
                        If .MonthlyRegenType = MonthlyRegenType.OnSpecificDayOfMonth Then
                            txtMonthlyDayNo.Text = .MonthlyRegenerateOnSpecificDateDayValue
                            txtMonthlyMonth.Text = .MonthlyRegenEveryXMonths
                            optMonthlyDay.Checked = True
                            optMonthlyPatternDays.Checked = False
                        ElseIf .MonthlyRegenType = MonthlyRegenType.OnCustomDateFormat Then
                            cboMonthlyDayType.DropDownList.SelectedValue = .MonthlySpecificDatePartOne.ToString()
                            cboMonthlyTypeDesc.DropDownList.SelectedValue = .MonthlySpecificDatePartTwo.ToString()
                            txtMonthlyMonth2.Text = .MonthlyRegenEveryXMonths
                            txtMonthlyPlusMinus.Text = .AdjustmentValue
                            optMonthlyDay.Checked = False
                            optMonthlyPatternDays.Checked = True
                        End If
                    Case RecurrenceType.Yearly
                        tabStrip.ActiveTabIndex = RecurrenceType.Yearly
                        If .YearlyRegenType = YearlyRegenType.OnSpecificDayOfYear Then
                            cboYearlyEveryMonth.DropDownList.SelectedValue = .SpecificDateMonthValue.ToString()
                            txtYearlyMonthNo.Text = .SpecificDateDayValue
                            optYearlyEvery.Checked = True
                            optYearlyPattern.Checked = False
                        ElseIf .YearlyRegenType = YearlyRegenType.OnCustomDateFormat Then
                            cboYearlyDayType.DropDownList.SelectedValue = .YearlySpecificDatePartOne.ToString()
                            cboYearlyTypedesc.DropDownList.SelectedValue = .YearlySpecificDatePartTwo.ToString()
                            cboYearlyMonth.DropDownList.SelectedValue = .YearlySpecificDatePartThree.ToString()
                            txtYearlyPluMinus.Text = .AdjustmentValue
                            optYearlyEvery.Checked = False
                            optYearlyPattern.Checked = True
                        End If

                End Select

                dteDateFrom.Text = .StartDate
                tmeStartDate.Hours = .EventTime.Hours
                tmeStartDate.Minutes = .EventTime.Minutes
                tmeStartDate.Seconds = .EventTime.Seconds

                If .HasEndDate = True Then
                    optEndOn.Checked = True
                    dteDateTo.Text = .EndDate
                Else
                    optEndAfter.Checked = True
                    txtOccurrences.Text = .NumberOfOccurrences
                End If

                strTemp = GetRecurringPattern()
                lblFriendly.Text = _friendlyDesc
                lblnextRunDate.Text = String.Format("The next calculated occurrence date is '{0}'", RecurrenceHelper.GetNextDate(Date.Now, strTemp))
            End With

        End Sub

        Public Sub SetDefaults()

            PopulateCombos()
            tabStrip.ActiveTabIndex = RecurrenceType.Daily
            hidSelectedTab.Value = RecurrenceType.Daily.ToString()

            'tabStrip.ActiveTabIndex = RecurrenceType.Daily
            optDailyEveryXDay.Checked = True
            txtDailyDays.Text = "1"

            txtWeeklyNo.Text = "1"
            chkWeeklyMonday.CheckBox.Checked = True
            chkWeeklyTuesday.CheckBox.Checked = False
            chkWeeklyWednesday.CheckBox.Checked = False
            chkWeeklyThursday.CheckBox.Checked = False
            chkWeeklyFriday.CheckBox.Checked = False
            chkWeeklySaturday.CheckBox.Checked = False
            chkWeeklySunday.CheckBox.Checked = False

            optMonthlyDay.Checked = True
            txtMonthlyDayNo.Text = Date.Today.Day
            txtMonthlyMonth.Text = "1"
            txtMonthlyMonth2.Text = "1"
            cboMonthlyDayType.DropDownList.SelectedIndex = MonthlySpecificDatePartOne.First
            cboMonthlyTypeDesc.DropDownList.SelectedIndex = MonthlySpecificDatePartTwo.Day
            txtMonthlyPlusMinus.Text = "0"


            optYearlyEvery.Checked = True
            txtYearlyMonthNo.Text = Date.Today.Day
            cboYearlyEveryMonth.DropDownList.SelectedIndex = Date.Today.Month - 1
            cboYearlyDayType.DropDownList.SelectedIndex = YearlySpecificDatePartOne.First
            cboYearlyTypedesc.DropDownList.SelectedIndex = YearlySpecificDatePartTwo.Day
            cboYearlyMonth.DropDownList.SelectedIndex = Date.Today.Month - 1
            txtYearlyPluMinus.Text = "0"


            dteDateFrom.Text = Date.Today.Date.ToString("dd/MM/yyyy")
            tmeStartDate.Hours = Date.Now.Hour
            tmeStartDate.Minutes = Date.Now.Minute
            tmeStartDate.ShowSeconds = False
            optEndOn.Checked = True
            optEndAfter.Checked = False
            dteDateTo.Text = ""
            txtOccurrences.Text = "10"

            InitialEnable()

        End Sub

        Public Function IsValid(Optional ByRef message As String = "") As Boolean
            Dim tabHeaderText As String = hidSelectedTab.Value
            Select Case tabHeaderText
                Case RecurrenceGenerator.RecurrenceType.Daily.ToString()

                    If optDailyEveryXDay.Checked Then
                        If Not IsNumeric(txtDailyDays.Text) Then
                            message = "Every x Days must be numeric."
                            Return False
                        Else
                            If Convert.ToInt32(txtDailyDays.Text) < 1 Then
                                message = "Every x Days must be greater than zero."
                                Return False
                            End If
                        End If
                    End If

                Case RecurrenceGenerator.RecurrenceType.Weekly.ToString()

                    If Not IsNumeric(txtWeeklyNo.Text) Then
                        message = "Every x weeks must be numeric."
                        Return False
                    Else
                        If Convert.ToInt32(txtWeeklyNo.Text) < 1 Then
                            message = "Every x weeks must be greater than zero."
                            Return False
                        End If
                    End If


                    If (chkWeeklySunday.CheckBox.Checked = False) And (chkWeeklyMonday.CheckBox.Checked = False) And _
                            (chkWeeklyTuesday.CheckBox.Checked = False) And (chkWeeklyWednesday.CheckBox.Checked = False) And _
                            (chkWeeklyThursday.CheckBox.Checked = False) And (chkWeeklyFriday.CheckBox.Checked = False) And _
                            (chkWeeklySaturday.CheckBox.Checked = False) Then
                        message = "At least one day must be selected."
                        Return False
                    End If


                Case RecurrenceGenerator.RecurrenceType.Monthly.ToString()

                    If optMonthlyDay.Checked Then
                        If Not IsNumeric(txtMonthlyDayNo.Text) Then
                            message = "Monthly Day Number must be numeric."
                            Return False
                        Else
                            If Convert.ToInt32(txtMonthlyDayNo.Text) < 1 Or Convert.ToInt32(txtMonthlyDayNo.Text) > 31 Then
                                message = "Monthly Day Number must be between 1 and 31."
                                Return False
                            End If
                        End If

                        If Not IsNumeric(txtMonthlyMonth.Text) Then
                            message = "Every x months must be numeric."
                            Return False
                        Else
                            If Convert.ToInt32(txtMonthlyMonth.Text) < 1 Then
                                message = "Every x months must greater than zero."
                                Return False
                            End If
                        End If
                    Else

                        If Not IsNumeric(txtMonthlyMonth2.Text) Then
                            message = "Every x months must be numeric."
                            Return False
                        Else
                            If Convert.ToInt32(txtMonthlyMonth2.Text) < 1 Then
                                message = "Every x months must greater than zero."
                                Return False
                            End If
                        End If

                        If Not IsNumeric(txtMonthlyPlusMinus.Text) Then
                            message = "Plus/Minus x days must be numeric."
                            Return False
                        End If

                    End If


                Case RecurrenceGenerator.RecurrenceType.Yearly.ToString()

                    If optYearlyEvery.Checked Then
                        If Not IsNumeric(txtYearlyMonthNo.Text) Then
                            message = "Day of month Number must be numeric."
                            Return False
                        Else
                            If Convert.ToInt32(txtYearlyMonthNo.Text) < 1 Or Convert.ToInt32(txtYearlyMonthNo.Text) > 31 Then
                                message = "Day of month Number must be between 1 and 31."
                                Return False
                            End If
                        End If
                    Else
                        'Get the adjusted value
                        If Not IsNumeric(txtYearlyPluMinus.Text) Then
                            message = "Plus/Minus x days must be numeric."
                            Return False
                        End If
                    End If

            End Select

            If optEndOn.Checked = True Then
                If Not IsNumeric(txtOccurrences.Text) Then
                    message = "Number of occurrences must be numeric."
                    Return False
                Else
                    If txtOccurrences.Text < 1 Then
                        message = "Number of occurrences must be greater than zero."
                        Return False
                    End If
                End If
            End If


            Return True
        End Function

        Private Sub InitialEnable()

            If optDailyEveryWeekDay.Checked Then
                txtDailyDays.Enabled = False
            End If

            If optMonthlyDay.Checked Then
                cboMonthlyDayType.Enabled = False
                cboMonthlyTypeDesc.Enabled = False
                txtMonthlyMonth2.Enabled = False
                txtMonthlyPlusMinus.Enabled = False
            Else
                txtMonthlyDayNo.Enabled = False
                txtMonthlyMonth.Enabled = False
            End If

            If optYearlyEvery.Checked Then
                cboYearlyDayType.Enabled = False
                cboYearlyMonth.Enabled = False
                cboYearlyTypedesc.Enabled = False
                txtYearlyPluMinus.Enabled = False
            Else
                cboYearlyEveryMonth.Enabled = False
                txtYearlyMonthNo.Enabled = False
            End If

            If optEndOn.Checked Then
                txtOccurrences.Enabled = False
            Else
                dteDateTo.Enabled = False
            End If

        End Sub
        Private Sub btntest_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btntest.ServerClick
            If IsValid() Then
                _updatingFriendlyText = True
                Dim strTemp As String = GetRecurringPattern()
                lblFriendly.Text = _friendlyDesc
                lblnextRunDate.Text = String.Format("The next calculated occurrence date is '{0}'", RecurrenceHelper.GetNextDate(Date.Now, strTemp))
                _updatingFriendlyText = False
            End If
        End Sub

    End Class
End Namespace