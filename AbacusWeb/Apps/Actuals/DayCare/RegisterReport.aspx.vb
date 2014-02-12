Imports System.Collections.Generic
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.ServiceRegisters
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Library
Imports System.Text
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library.LinqExtensions.IEnumerableExtensions
Imports System.Web.Script.Serialization

Namespace Apps.Actuals.DayCare

    ''' <summary>
    ''' Web Form for Viewing Service Register Reports
    ''' </summary>
    ''' <history>
    ''' ColinD   15/08/2011 #6979  - Updated - Additional updates after Amrik requested changes.
    ''' ColinD   12/08/2011 #6979  - Updated - Added additional 'All - Day by Day View' to 'Day' options and altered day tables to list Service Users on rows and Rate Categories on headers i.e. only one row per Service User.
    ''' ColinD   05/07/2011 D11240 - Created
    ''' </history>
    Partial Public Class RegisterReport
        Inherits Target.Web.Apps.BasePage

#Region "Enumerations"

        ''' <summary>
        ''' Represents an enumeration for Report Modes.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Enum ReportModes As Byte
            Blank = 0
            Complete = 1
        End Enum

#End Region

#Region "Fields"

        ' constants
        Private Const _AdditionalLines As Integer = 4
        Private Const _ClassAttended As String = "a"
        Private Const _ClassAttendedExceeded As String = "ae"
        Private Const _ClassDay As String = "d"
        Private Const _ClassPlanned As String = "p"
        Private Const _ClassNotAttended As String = "na"
        Private Const _ClassNotPlanned As String = "np"
        Private Const _ClassTotalAttended As String = "ta"
        Private Const _ClassTotalPlanned As String = "tp"
        Private Const _DateFormat As String = "dd/MM/yyyy"
        Private Const _DaysAll As String = "All"
        Private Const _DaysAllDayByDay As String = "AllDays"
        Private Const _HtmlBlankSpace As String = "&nbsp;"
        Private Const _ImgPlannedAttended As String = "PlannedAttended.png"
        Private Const _ImgPlannedNotAttended As String = "PlannedNotAttended.png"
        Private Const _ImgTick As String = "Tick.png"
        Private Const _ImgCheckBoxUnChecked As String = "CheckBoxUnChecked.png"
        Private Const _ImgClipboard As String = "Clipboard.png"
        Private Const _ImgCross As String = "Cross.png"
        Private Const _PageTitle As String = "Service Register Printout"
        Private Const _QsRegisterID As String = "id"
        Private Const _QsReportMode As String = "rMode"
        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.DayCare"

        ' locals 
        Private _LastTotalActualCount As Integer = 0
        Private _LastTotalPlannedCount As Integer = 0
        Private _Register As ViewableRegister = Nothing
        Private _RegisterClientStatuses As List(Of ViewableRegisterClientStatus) = Nothing
        Private _RegisterDays As List(Of ViewableRegisterDay) = Nothing
        Private _RegisterPlannedOrAttendedService As List(Of ViewablePlannedOrAttendedService) = Nothing
        Private _RegisterRateCategories As List(Of ViewableRateCategory) = Nothing

#End Region

#Region "Properties"

#Region "QueryString Properties"

        ''' <summary>
        ''' Gets the register ID from the query string.
        ''' </summary>
        ''' <value>The register ID.</value>
        Private ReadOnly Property RegisterID() As Integer
            Get
                Return Utils.ToInt32(Request.QueryString(_QsRegisterID))
            End Get
        End Property

        ''' <summary>
        ''' Gets the parameter report mode from the query string.
        ''' </summary>
        ''' <value>The parameter report mode.</value>
        Protected ReadOnly Property ReportMode() As ReportModes
            Get
                Return Utils.ToInt32(Request.QueryString(_QsReportMode))
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Gets the register.
        ''' </summary>
        ''' <value>The register.</value>
        Private ReadOnly Property Register() As ViewableRegister
            Get
                If _Register Is Nothing Then
                    ' if we havent fetched the item then do so, throw error if encountered
                    Dim msg As ErrorMessage = ServiceRegisterBL.GetRegister(DbConnection, RegisterID, _Register)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _Register
            End Get
        End Property

        ''' <summary>
        ''' Gets the register client statuses.
        ''' </summary>
        ''' <value>The register client statuses.</value>
        Private ReadOnly Property RegisterClientStatuses() As List(Of ViewableRegisterClientStatus)
            Get
                If _RegisterClientStatuses Is Nothing Then
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim msg As ErrorMessage = ServiceRegisterBL.GetRegisterClientStatuses(DbConnection, RegisterID, _RegisterClientStatuses)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _RegisterClientStatuses
            End Get
        End Property

        ''' <summary>
        ''' Gets the register days.
        ''' </summary>
        ''' <value>The register days.</value>
        Private ReadOnly Property RegisterDays() As List(Of ViewableRegisterDay)
            Get
                If _RegisterDays Is Nothing Then
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim msg As ErrorMessage = ServiceRegisterBL.GetRegisterDays(DbConnection, RegisterID, _RegisterDays)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _RegisterDays
            End Get
        End Property

        ''' <summary>
        ''' Gets the registers planned or attended service.
        ''' </summary>
        ''' <value>The register planned or attended service.</value>
        Private ReadOnly Property RegisterPlannedOrAttendedService() As List(Of ViewablePlannedOrAttendedService)
            Get
                If _RegisterPlannedOrAttendedService Is Nothing Then
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim msg As ErrorMessage = ServiceRegisterBL.GetRegisterPlannedOrAttendedService(DbConnection, RegisterID, 0, 0, String.Empty, String.Empty, _RegisterPlannedOrAttendedService)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _RegisterPlannedOrAttendedService

            End Get
        End Property

        ''' <summary>
        ''' Gets the registers rate categories.
        ''' </summary>
        ''' <value>The registers rate categories.</value>
        Private ReadOnly Property RegisterRateCategories() As List(Of ViewableRateCategory)
            Get
                If _RegisterRateCategories Is Nothing Then
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim rateCategories As DomRateCategoryCollection = Nothing
                    Dim msg As ErrorMessage = DomRateCategory.FetchList(conn:=DbConnection, list:=rateCategories, domRateFrameworkID:=Register.DomRateFrameworkID, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                    _RegisterRateCategories = New List(Of ViewableRateCategory)()
                    For Each rateCategory As DomRateCategory In rateCategories
                        _RegisterRateCategories.Add(New ViewableRateCategory(rateCategory.Description, rateCategory.ID))
                    Next
                End If
                Return _RegisterRateCategories
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' disable time out for this popup
            EnableTimeout = False

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), _PageTitle)

            ' populate the register reports data
            PopulateRegisterReport()

            ' setup js/reports
            SetupJavaScript()

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim js As New StringBuilder()

            ' setup js to be output to client
            js.AppendFormat("mode = {0}; modeBlank = {1}; modeComplete = {2}; modeBlankAdditionalLines = {3};", _
                            CType(ReportMode, Byte), _
                            CType(ReportModes.Blank, Byte), _
                            CType(ReportModes.Complete, Byte), _
                            _AdditionalLines)
            js.AppendFormat("classTotalPlanned = '{0}'; classTotalAttended = '{1}'; classPlanned = '{2}'; classAttended = '{3}'; classAttendedExceeded = '{4}';", _
                            _ClassTotalPlanned, _
                            _ClassTotalAttended, _
                            _ClassPlanned, _ClassAttended, _
                            _ClassAttendedExceeded)
            js.AppendFormat("daysAll = '{0}'; daysAllDayByDay = '{1}';", _
                            _DaysAll, _
                            _DaysAllDayByDay)
            js.AppendFormat("hasService = {0};", RegisterClientStatuses.HasItems().ToString().ToLower())
            js.AppendFormat("imgCheckBoxUnChecked = '{0}';", Target.Library.Web.Utils.GetVirtualPath(String.Format("Images/{0}", _ImgCheckBoxUnChecked)))
            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

        End Sub

#End Region

#Region "Functions/Methods"

        ''' <summary>
        ''' Configures the day.
        ''' </summary>
        ''' <param name="reporter">The reporter object.</param>
        ''' <returns></returns>
        Protected Function ConfigureDay(ByVal dayKey As String, ByVal reporter As RegisterReporter) As String

            Dim cellClasses As String = String.Format("class=""{0} {1}", _ClassDay, dayKey.ToLower())
            Dim daysService As ViewablePlannedOrAttendedService = Nothing

            If reporter.Days.ContainsKey(dayKey) Then
                ' if we have a matching key

                daysService = reporter.Days(dayKey)

                If daysService.IsPlanned Then

                    If daysService.IsAttended Then

                        cellClasses += String.Format(" {0} {1}", _ClassPlanned, _ClassAttended)

                    Else

                        cellClasses += String.Format(" {0} {1}", _ClassPlanned, _ClassNotAttended)

                    End If

                Else

                    If daysService.IsAttended Then

                        cellClasses += String.Format(" {0} {1}", _ClassNotPlanned, _ClassAttended)

                    Else

                        cellClasses += String.Format(" {0} {1}", _ClassNotPlanned, _ClassNotAttended)

                    End If

                End If

            Else

                cellClasses += String.Format(" {0} {1}", _ClassNotPlanned, _ClassNotAttended)

            End If

            cellClasses += """"

            Return cellClasses

        End Function

        ''' <summary>
        ''' Configures the day cell.
        ''' </summary>
        ''' <param name="reporter">The reporter object.</param>
        ''' <returns></returns>
        Protected Function ConfigureDayCell(ByVal dayKey As String, ByVal reporter As RegisterReporter) As String

            Dim imgSrc As String = String.Empty
            Dim imgPlanned As String = String.Empty
            Dim cellText As String = _HtmlBlankSpace
            Dim daysService As ViewablePlannedOrAttendedService = Nothing

            If reporter.Days.ContainsKey(dayKey) Then
                ' if we have a matching key

                daysService = reporter.Days(dayKey)

                If ReportMode = ReportModes.Complete Then
                    ' if report mode is complete then use ticks and crosses etc

                    If daysService.IsPlanned Then

                        If daysService.IsAttended Then

                            imgSrc = _ImgPlannedAttended

                        Else

                            imgSrc = _ImgPlannedNotAttended

                        End If

                    Else

                        If daysService.IsAttended Then

                            imgSrc = _ImgTick

                        Else

                            imgSrc = _ImgCross

                        End If

                    End If

                Else

                    If daysService.IsPlanned Then

                        imgSrc = _ImgClipboard

                    Else

                        imgSrc = _ImgCheckBoxUnChecked

                    End If


                End If

                If Utils.ToString(daysService.ServiceOutcomeCode).Length > 0 _
                    AndAlso (ReportMode = ReportModes.Complete _
                             OrElse (ReportMode = ReportModes.Blank AndAlso daysService.IsPlanned)) Then

                    cellText = daysService.ServiceOutcomeCode

                End If

            Else

                If ReportMode = ReportModes.Complete Then

                    imgSrc = _ImgCross

                Else

                    imgSrc = _ImgCheckBoxUnChecked

                End If

            End If

            imgSrc = Target.Library.Web.Utils.GetVirtualPath(String.Format("Images/{0}", imgSrc))

            Return String.Format("<img src='{0}' /><span>{1}</span>", imgSrc, cellText)

        End Function

        ''' <summary>
        ''' Configures the total actual cell.
        ''' </summary>
        ''' <param name="dayKey">The day key.</param>
        ''' <returns></returns>
        Protected Function ConfigureTotalActualCell(ByVal dayKey As String) As String

            If ReportMode = ReportModes.Complete Then

                Dim sourceData As List(Of RegisterReporter) = rptRegisterClientStatuses.DataSource
                Dim actualForDay As List(Of RegisterReporter) = (From tmpReporter As RegisterReporter In sourceData, _
                                                                    tmpDays As KeyValuePair(Of String, ViewablePlannedOrAttendedService) In tmpReporter.Days _
                                                                    Where _
                                                                        (dayKey = "" OrElse tmpDays.Key = dayKey) _
                                                                        AndAlso tmpDays.Value.IsAttended = True _
                                                                    Select tmpReporter).ToList()

                Return actualForDay.Count.ToString()

            Else

                Return _HtmlBlankSpace

            End If

        End Function

        ''' <summary>
        ''' Configures the total actual.
        ''' </summary>
        ''' <param name="clientId">The client id.</param>
        ''' <param name="rateCategoryId">The rate category id.</param>
        ''' <returns></returns>
        Protected Function ConfigureTotalActual(ByVal clientId As Integer, ByVal rateCategoryId As Integer) As String

            If ReportMode = ReportModes.Complete Then

                Dim sourceData As List(Of RegisterReporter) = rptRegisterClientStatuses.DataSource
                Dim actualForDay As List(Of RegisterReporter) = (From tmpReporter As RegisterReporter In sourceData, _
                                                                    tmpDays As KeyValuePair(Of String, ViewablePlannedOrAttendedService) In tmpReporter.Days _
                                                                    Where _
                                                                        tmpReporter.ClientStatus.ClientID = clientId _
                                                                        AndAlso tmpReporter.RateCategory.ID = rateCategoryId _
                                                                        AndAlso tmpDays.Value.IsAttended = True _
                                                                    Select tmpReporter).ToList()
                Dim tdAttributes As String = String.Format("class=""{0}", _ClassTotalAttended)

                _LastTotalActualCount = actualForDay.Count

                If _LastTotalActualCount > _LastTotalPlannedCount Then

                    tdAttributes += String.Format(" {0}", _ClassAttendedExceeded)

                End If

                tdAttributes += """"

                Return tdAttributes

            Else

                Return _HtmlBlankSpace

            End If

        End Function

        ''' <summary>
        ''' Configures the total actual cell.
        ''' </summary>
        ''' <param name="clientId">The client id.</param>
        ''' <param name="rateCategoryId">The rate category id.</param>
        ''' <returns></returns>
        Protected Function ConfigureTotalActualCell(ByVal clientId As Integer, ByVal rateCategoryId As Integer) As String

            If ReportMode = ReportModes.Complete Then

                Return _LastTotalActualCount

            Else

                Return _HtmlBlankSpace

            End If

        End Function

        ''' <summary>
        ''' Configures the total planned cell.
        ''' </summary>
        ''' <param name="dayKey">The day key.</param>
        ''' <returns></returns>
        Protected Function ConfigureTotalPlannedCell(ByVal dayKey As String) As String

            Dim sourceData As List(Of RegisterReporter) = rptRegisterClientStatuses.DataSource
            Dim plannedForDay As List(Of RegisterReporter) = (From tmpReporter As RegisterReporter In sourceData, _
                                                                tmpDays As KeyValuePair(Of String, ViewablePlannedOrAttendedService) In tmpReporter.Days _
                                                                Where _
                                                                    (dayKey = "" OrElse tmpDays.Key = dayKey) _
                                                                    AndAlso tmpDays.Value.IsPlanned = True _
                                                                Select tmpReporter).ToList()

            Return plannedForDay.Count.ToString()

        End Function

        ''' <summary>
        ''' Configures the total planned cell.
        ''' </summary>
        ''' <param name="clientId">The client id.</param>
        ''' <param name="rateCategoryId">The rate category id.</param>
        ''' <returns></returns>
        Protected Function ConfigureTotalPlannedCell(ByVal clientId As Integer, ByVal rateCategoryId As Integer) As String

            Dim sourceData As List(Of RegisterReporter) = rptRegisterClientStatuses.DataSource
            Dim plannedForDay As List(Of RegisterReporter) = (From tmpReporter As RegisterReporter In sourceData, _
                                                                tmpDays As KeyValuePair(Of String, ViewablePlannedOrAttendedService) In tmpReporter.Days _
                                                                Where _
                                                                    tmpReporter.ClientStatus.ClientID = clientId _
                                                                    AndAlso tmpReporter.RateCategory.ID = rateCategoryId _
                                                                    AndAlso tmpDays.Value.IsPlanned = True _
                                                                Select tmpReporter).ToList()

            _LastTotalPlannedCount = plannedForDay.Count
            Return _LastTotalPlannedCount.ToString()

        End Function

        ''' <summary>
        ''' Populates the register report.
        ''' </summary>
        Private Sub PopulateRegisterReport()

            Dim clients As List(Of ViewableRegisterClientStatus) = RegisterClientStatuses
            Dim days As List(Of ViewableRegisterDay) = RegisterDays
            Dim plannedOrAttendedServices As List(Of ViewablePlannedOrAttendedService) = RegisterPlannedOrAttendedService
            Dim rateCategories As List(Of ViewableRateCategory) = RegisterRateCategories

            ' setup headings
            lblProvider.InnerText = Register.ProviderName
            lblContract.InnerText = Register.DomContractTitle
            lblWeekEnding.InnerText = Register.WeekEnding.ToString(_DateFormat)

            ' setup the days drop down list
            selDays.Items.Clear()
            selDays.Items.Add(New ListItem("All", _DaysAll))
            selDays.Items.Add(New ListItem("All - Day by Day", _DaysAllDayByDay))

            ' setup div visibility 
            divNoRegisterClientStatuses.Visible = True
            divRegisterClientStatuses.Visible = False

            For Each regDay As ViewableRegisterDay In RegisterDays
                ' loop each day and add into drop down list

                With selDays
                    .Items.Add(New ListItem(String.Format("{0} ({1})", regDay.DayOfWeekName, regDay.DayOfWeekDateFormatted), regDay.DayOfWeekNameShort))
                End With

            Next

            If clients.HasItems() Then
                ' if we have at least one client then explode the rate categories with them

                ' join the clients with the rate categories i.e. get a cartesion product of the two
                Dim jointClientsAndRateCategories As List(Of RegisterReporter) = (From tmpClient As ViewableRegisterClientStatus In clients, _
                                                                                    tmpRateCategory In rateCategories _
                                                                                        Select New RegisterReporter(tmpClient, _
                                                                                                                tmpRateCategory)).ToList()

                If plannedOrAttendedServices.HasItems() Then
                    ' if we have some planned or attended service then attach to the reporter

                    Dim currentRegisterReporter As RegisterReporter = Nothing
                    Dim currentClientId As Integer = 0
                    Dim currentDay As ViewableRegisterDay = Nothing
                    Dim currentDayId As Integer = 0
                    Dim currentDomRateCategoryId As Integer = 0

                    For Each plannedOrAttendedService As ViewablePlannedOrAttendedService In (From tmpPlannedOrAttendedService In plannedOrAttendedServices _
                                                                                                Order By tmpPlannedOrAttendedService.ClientID, _
                                                                                                            tmpPlannedOrAttendedService.DomRateCategoryID _
                                                                                                Select tmpPlannedOrAttendedService).ToList()

                        If currentRegisterReporter Is Nothing _
                            OrElse (currentRegisterReporter.RateCategory.ID <> plannedOrAttendedService.DomRateCategoryID _
                                    OrElse currentRegisterReporter.ClientStatus.ClientID <> plannedOrAttendedService.ClientID) Then
                            ' get the register reporter for this rate caegory

                            currentClientId = plannedOrAttendedService.ClientID
                            currentDomRateCategoryId = plannedOrAttendedService.DomRateCategoryID
                            currentRegisterReporter = (From tmpCurrentRegisterReporter As RegisterReporter In jointClientsAndRateCategories _
                                                        Where _
                                                            tmpCurrentRegisterReporter.RateCategory.ID = currentDomRateCategoryId _
                                                            AndAlso tmpCurrentRegisterReporter.ClientStatus.ClientID = currentClientId _
                                                        Select tmpCurrentRegisterReporter).FirstOrDefault()

                        End If

                        ' determine the current day for the service
                        currentDayId = plannedOrAttendedService.RegisterDayID
                        currentDay = (From tmpDay As ViewableRegisterDay In days Where tmpDay.ID = currentDayId Select tmpDay).First()

                        ' add the service to the day for the reporter
                        With currentRegisterReporter
                            .Days.Add(currentDay.DayOfWeekNameShort, plannedOrAttendedService)
                        End With

                    Next

                    If ReportMode = ReportModes.Blank Then
                        ' if the report mode is blank then we need to add some extra lines

                        For i As Integer = 1 To _AdditionalLines
                            ' create any extra lines on the report

                            Dim client As New ViewableRegisterClientStatus(0, _HtmlBlankSpace, ServiceRegisterBL.RegisterClientStatus.Read, _HtmlBlankSpace)
                            Dim rateCategory As New ViewableRateCategory(_HtmlBlankSpace, 0)

                            jointClientsAndRateCategories.Add(New RegisterReporter(client, rateCategory))

                        Next

                    End If

                End If

                If jointClientsAndRateCategories.HasItems() Then
                    ' if we have some items display em

                    divNoRegisterClientStatuses.Visible = False
                    divRegisterClientStatuses.Visible = True

                    ' bind data to repeater
                    With rptRegisterClientStatuses
                        .DataSource = jointClientsAndRateCategories
                        .DataBind()
                    End With

                    ' bind data to repeater for day view
                    With rptRegisterClientStatusesDaily
                        .DataSource = RegisterRateCategories
                        .DataBind()
                    End With

                End If

            End If

        End Sub

     

        ''' <summary>
        ''' Setups the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            ' add in js link for handlers
            JsLinks.Add("RegisterReport.js")

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add dialog js
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))

            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))

            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            UseJqueryUI = True

            ' add in jquery async for async loops so ui appears responsive during expensive functions
            UseJqueryAsync = True

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.ServiceRegisters))

        End Sub

#End Region

#Region "RegisterReporter Class"

        Public Class RegisterReporter

#Region "Fields"

            Private _ClientStatus As ViewableRegisterClientStatus = Nothing
            Private _RateCategory As ViewableRateCategory = Nothing
            Private _Days As New Dictionary(Of String, ViewablePlannedOrAttendedService)()

#End Region

#Region "Constructors"

            ''' <summary>
            ''' Initializes a new instance of the <see cref="RegisterReporter" /> class.
            ''' </summary>
            Public Sub New()

                MyBase.New()

            End Sub

            ''' <summary>
            ''' Initializes a new instance of the <see cref="RegisterReporter" /> class.
            ''' </summary>
            Public Sub New(ByVal clientStatus As ViewableRegisterClientStatus, ByVal rateCategory As ViewableRateCategory)

                MyBase.New()
                With Me
                    .ClientStatus = clientStatus
                    .RateCategory = rateCategory
                End With

            End Sub

#End Region

#Region "Properties"

            ''' <summary>
            ''' Gets or sets the client status.
            ''' </summary>
            ''' <value>The client status.</value>
            Public Property ClientStatus() As ViewableRegisterClientStatus
                Get
                    Return _ClientStatus
                End Get
                Set(ByVal value As ViewableRegisterClientStatus)
                    _ClientStatus = value
                End Set
            End Property

            ''' <summary>
            ''' Gets or sets the days.
            ''' </summary>
            ''' <value>The days.</value>
            Public Property Days() As Dictionary(Of String, ViewablePlannedOrAttendedService)
                Get
                    Return _Days
                End Get
                Set(ByVal value As Dictionary(Of String, ViewablePlannedOrAttendedService))
                    _Days = value
                End Set
            End Property

            ''' <summary>
            ''' Gets or sets the rate category.
            ''' </summary>
            ''' <value>The rate category.</value>
            Public Property RateCategory() As ViewableRateCategory
                Get
                    Return _RateCategory
                End Get
                Set(ByVal value As ViewableRateCategory)
                    _RateCategory = value
                End Set
            End Property

#End Region

        End Class

#End Region

    End Class

End Namespace
