
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security
Imports Target.Web.Apps
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.DataClasses
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.Controls
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library
Imports Target.Abacus.Web
Imports System.Configuration.ConfigurationManager
Imports System.Web.Services
Imports Target.Library.Web
Imports Target.Abacus.Web.Apps.WebSvc
Imports Target.Abacus.Library.DomProviderInvoice

Namespace Apps.Actuals.DayCare

    ''' <summary>
    ''' Admin page used to maintain registers.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  19/05/2011  A4WA#6808 - performance improvements.
    ''' MoTahir 25/10/2010  A4WA#6508 - Unexpected Error and Very Slow Operation of Register
    ''' MoTahir 10/11/2009  D11681
    ''' </history>
    Partial Public Class Edit
        Inherits BasePage

#Region " Constants "

        Const VIEWSTATE_KEY_DATA As String = "DataList"
        Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"
        Const CTRL_PREFIX_SUREF As String = "suref"
        Const CTRL_PREFIX_SURNAME As String = "surname"
        Const CTRL_PREFIX_FIRSTNAME As String = "firstname"
        Const CTRL_PREFIX_RATCATEGORY As String = "ratcateg"
        Const CTRL_PREFIX_MON As String = "mon"
        Const CTRL_PREFIX_TUE As String = "tue"
        Const CTRL_PREFIX_WED As String = "wed"
        Const CTRL_PREFIX_THU As String = "thu"
        Const CTRL_PREFIX_FRI As String = "fri"
        Const CTRL_PREFIX_SAT As String = "sat"
        Const CTRL_PREFIX_SUN As String = "sun"
        Const CTRL_PREFIX_RATE As String = "rate"
        Const CTRL_PREFIX_TOTAL_PLANNED As String = "totalplanned"
        Const CTRL_PREFIX_TOTAL_ACTUAL As String = "totalactual"
        Const CTRL_PREFIX_REMOVED As String = "remove"

        Const CTRL_PREFIX_HOUTCOME As String = "houtcome"
        Const CTRL_PREFIX_OUTCOME As String = "outcome"
        Const CTRL_NAME_TXTSTATUS As String = "status"
        Const CTRL_NAME_TXTSTATUS_PREFIX As String = "ctl00_MPContent_stdButtons1_"
        Const CTRL_PREFIX_CHKBOX As String = "ctl00$MPContent$"

        Const CTRL_NAME_PREFIX_HOUTCOME As String = "ctl00$MPContent$houtcomeU_"
        Const HID_CTRL_NAME_SUFFIX As String = "_hidField"
        Const CTRL_TYPE_SUFFIX_CHKBOX As String = "$chkCheckbox"
        Const CTRL_TYPE_SUFFIX_HORIGINAL As String = "$hidOriginalValue"

        Const UNIQUEID_PREFIX_NEW As String = "N"
        Const UNIQUEID_PREFIX_UPDATE As String = "U"
        Const UNIQUEID_PREFIX_DELETE As String = "D"
        Const UNIQUEID_PREFIX_UPDATE_CONTROL As String = "U_"

        Const SP_LOCK_REGISTER As String = "spxLockRegister"

        Const CHK_ON As String = "on"
        Const CHK_OFF As String = "off"

#End Region

#Region " Private variables "

        Private _newCodeIDCounter As Integer
        Private _weekEnding As String
        Private _establishmentID As Integer
        Private _frameworkID As Integer
        Private _domContractID As Integer
        Private _domContractTitle As String
        Private _establishmentName As String
        Private _stdBut As StdButtonsBase
        Private ddView As DropDownList = New DropDownList
        Private btnRefresh As Button = New Button
        Private txtRegisterGroup As New InPlaceSelectors.InPlaceRegisterGroupSelector
        Private _registerDay As String
        Private txtStatus As TextBoxEx = New TextBoxEx
        Private editClick As Boolean = False
        Private _serviceOutComes As ServiceOutcomeCollection = Nothing
        Private _aRegister As Register = Nothing
        Private _establishment As Establishment = Nothing
        Private _contract As Target.Abacus.Library.DataClasses.DomContract = Nothing
        Private _regRows As RegisterRowCollection = Nothing
        Private _regID As Integer
        Private _currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

#End Region

#Region " Properties"
        Public ReadOnly Property WeekEnding() As String
            Get
                Return _weekEnding
            End Get
        End Property
        Public ReadOnly Property EstablishmentName() As String
            Get
                Return _establishmentName
            End Get
        End Property
        Public ReadOnly Property DomContractID() As String
            Get
                Return _domContractID
            End Get
        End Property
        Public ReadOnly Property DomContractTitle() As String
            Get
                Return _domContractTitle
            End Get
        End Property
        Public ReadOnly Property regDay() As String
            Get
                Return _registerDay
            End Get
        End Property
#End Region

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.InitPage(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DayCare"), "Register")
            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            AddHandler _stdBut.AddCustomControls, AddressOf StdButtons_AddCustomControls
            AddHandler btnRefresh.Click, AddressOf btnRefresh_Click
            ddView.AutoPostBack = False
            With txtStatus
                .ID = CTRL_NAME_TXTSTATUS
            End With
        End Sub

#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            _regID = Target.Library.Utils.ToInt32(Request.QueryString("ID"))
            Dim msg As ErrorMessage

            With _stdBut
                .AllowBack = True
                .AllowNew = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCare.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCare.Delete"))
                .AllowFind = False
                .EditableControls.Add(fsControls.Controls)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DayCareRegisterDetails")
                .ReportButtonParameters.Add("intSelectedRegisterID", Convert.ToInt32(_regID))
                .ReportButtonParameters.Add("blnInProgress", True)
                .ReportButtonParameters.Add("blnSubmitted", True)
                .ReportButtonParameters.Add("blnAmended", True)
                .ReportButtonParameters.Add("blnProcessed", True)
                .AuditLogTableNames.Add("Register")
                .AuditLogTableNames.Add("RegisterDay")
                .AuditLogTableNames.Add("RegisterRow")
                .AuditLogTableNames.Add("RegisterColumn")
                .AuditLogTableNames.Add("RegisterCell")
            End With

            btnSubmit.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCare.Submit"))
            btnUnSubmit.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCare.UnSubmit"))

            AddHandler _stdBut.NewClicked, AddressOf ClearViewState
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))

            Me.JsLinks.Add("Edit.js")

            btnAddServiceUsers.Attributes.Add("onclick", String.Format("InPlaceClientSelector_btnFind_Click('{0}');", Me.ClientID))

            Dim settings As SystemSettings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, _
                                                                              SystemSettings.CACHE_DEFAULT_EXPIRATION)
            '' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(ViewableRegisterCell))

            ' store MRU Register
            _aRegister = New Register(Me.DbConnection, String.Empty, String.Empty)
            msg = _aRegister.Fetch(_regID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _establishment = New Establishment(Me.DbConnection)
            msg = _establishment.Fetch(_aRegister.ProviderID)

            If _regRows Is Nothing Then
                msg = RegisterRow.FetchList(Me.DbConnection, _regRows, String.Empty, String.Empty, _regID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            For Each row As RegisterRow In _regRows
                _domContractID = row.DomContractID
                If _domContractID > 0 Then Exit For
            Next

            If _contract Is Nothing Then
                _contract = New Target.Abacus.Library.DataClasses.DomContract(Me.DbConnection, String.Empty, String.Empty)
                msg = _contract.Fetch(_domContractID)
            End If

            msg = ServiceOutcome.FetchList(Me.DbConnection, _serviceOutComes, String.Empty, String.Empty)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            Dim mruManager As Target.Library.Web.MostRecentlyUsedManager = New Target.Library.Web.MostRecentlyUsedManager(HttpContext.Current)
            mruManager("ATTENDANCEREGISTERS")(_regID.ToString()) = String.Format("{0}: {1}: {2}", [String].Format("{0:dd/MM/yyyy}", _aRegister.WeekEnding), _establishment.Name, _contract.Number)
            mruManager.Save(HttpContext.Current)

        End Sub

#End Region

#Region " btnRefresh_Click "

        Protected Sub btnRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim msg As ErrorMessage
            Dim register As vwRegisterWeekCollection
            Dim list As List(Of String)
            Dim registerID As Integer = Nothing
            Dim registerGroup As String = Nothing
            Dim regCells As RegisterCellCollection = Nothing
            Dim regDays As RegisterDayCollection = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim registerDayCollection As vwRegisterDayCollection = New vwRegisterDayCollection
            Dim rateCategoryCollection As DomRateCategoryCollection = Nothing
            Dim rateCategoryIDs As New ArrayList
            Dim rateFrameworkID As Integer = 0

            ' the collections used by getviewable register cell
            msg = RegisterDay.FetchList(Me.DbConnection, regDays, currentUser.ExternalUsername, _
                                        AuditLogging.GetAuditLogTitle(PageTitle, Settings), _stdBut.SelectedItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = RegisterCell.FetchList(Me.DbConnection, regCells, currentUser.ExternalUsername, _
                    AuditLogging.GetAuditLogTitle(PageTitle, Settings), _stdBut.SelectedItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'collection used by GetTotals method
            msg = vwRegisterDay.FetchList(conn:=Me.DbConnection, list:=registerDayCollection, registerID:=_stdBut.SelectedItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ViewState("ddView") = ddView.SelectedValue
            ViewState("txtRegisterGroup") = txtRegisterGroup.RegisterGroupText

            rateFrameworkID = _contract.DomRateFrameworkID

            If CInt(ViewState("ddView")) <> RegisterView.Week Then
                registerID = _stdBut.SelectedItemID
                OutputRegisterControlsByDay(Me.DbConnection, CInt(ViewState("ddView")), CStr(ViewState("txtRegisterGroup")), _
                                            regDays, regCells, registerDayCollection)

                'get the rate category id's
                If CStr(ViewState("txtRegisterGroup")) = "" Then
                    msg = DomRateCategory.FetchList(Me.DbConnection, rateCategoryCollection, String.Empty, String.Empty, , , rateFrameworkID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    For Each rateCategory As DomRateCategory In rateCategoryCollection
                        rateCategoryIDs.Add(rateCategory.ID)
                    Next
                Else
                    msg = DomRateCategory.FetchList(Me.DbConnection, rateCategoryCollection, String.Empty, String.Empty, , , rateFrameworkID, CStr(ViewState("txtRegisterGroup")))
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    For Each rateCategory As DomRateCategory In rateCategoryCollection
                        rateCategoryIDs.Add(rateCategory.ID)
                    Next
                End If

                OutputRegisterColumnTotalsByDay(registerID, True, CStr(ViewState("txtRegisterGroup")), registerDayCollection, rateCategoryIDs)
                OutputRegisterColumnTotalsByDay(registerID, False, CStr(ViewState("txtRegisterGroup")), registerDayCollection, rateCategoryIDs)
                'Hide custom buttons for day view
                btnUnSubmit.Visible = False
                btnAmend.Visible = False
                btnSubmit.Visible = False
                btnAddServiceUsers.Visible = False
            End If

            If CInt(ViewState("ddView")) = RegisterView.Week Then
                register = New vwRegisterWeekCollection

                If CStr(ViewState("txtRegisterGroup")) <> "" Then
                    registerGroup = CStr(ViewState("txtRegisterGroup"))
                End If

                msg = vwRegisterWeek.FetchList(Me.DbConnection, register, , , , , _stdBut.SelectedItemID, , , , , , , , registerGroup)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ClearViewState()
                list = GetUniqueIDsFromViewState()

                phRegisterHeading.Controls.Clear()
                OutputRegisterHeaders()
                phRegisterWeek.Controls.Clear()
                For Each registerWeek As vwRegisterWeek In register
                    Dim id As String = GetUniqueID(registerWeek)
                    OutputRegisterControls(id, registerWeek, Me.DbConnection, CStr(ViewState("txtRegisterGroup")), regDays, regCells, registerDayCollection)
                    list.Add(id)
                    _domContractID = registerWeek.DomContractID
                    _domContractTitle = registerWeek.ContractTitle
                    registerID = registerWeek.RegisterID
                Next
                phRegisterFooting.Controls.Clear()
                OutputRegisterColumnTotalsByWeek(registerID, True, CStr(ViewState("txtRegisterGroup")), registerDayCollection)
                OutputRegisterColumnTotalsByWeek(registerID, False, CStr(ViewState("txtRegisterGroup")), registerDayCollection)
                PersistUniqueIDsToViewState(list)

                'Hide custom buttons for registers dependant on status
                Select Case txtStatus.TextBox.Text
                    Case "In Progress"
                        btnUnSubmit.Visible = False
                        btnAmend.Visible = False
                        btnSubmit.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCare.Submit"))
                        btnAddServiceUsers.Visible = True
                    Case "Submitted"
                        btnSubmit.Visible = False
                        btnAmend.Visible = False
                        btnUnSubmit.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCare.UnSubmit"))
                        btnAddServiceUsers.Visible = False
                    Case "Processed"
                        btnSubmit.Visible = False
                        btnUnSubmit.Visible = False
                        btnAddServiceUsers.Visible = False
                        btnAmend.Visible = True
                    Case "Amended"
                        btnSubmit.Visible = False
                        btnUnSubmit.Visible = False
                        btnAmend.Visible = False
                        btnAddServiceUsers.Visible = True
                    Case Else
                End Select
                btnUncheck.Visible = False
            End If
        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim register As vwRegisterWeekCollection = New vwRegisterWeekCollection
            Dim list As List(Of String)
            Dim regStatus As Target.Abacus.Library.DataClasses.RegisterStatus = Nothing
            Dim registerGroup As String = Nothing
            Dim registerDayCollection As vwRegisterDayCollection = New vwRegisterDayCollection
            Dim regCells As RegisterCellCollection = Nothing
            Dim regDays As RegisterDayCollection = Nothing
            Dim rateCategoryCollection As DomRateCategoryCollection = Nothing
            Dim rateCategoryIDs As New ArrayList
            Dim rateFrameworkID As Integer = 0

            ' get collection used by GetViewableRegisterCell method
            msg = RegisterDay.FetchList(Me.DbConnection, regDays, _currentUser.ExternalUsername, _
                                        AuditLogging.GetAuditLogTitle(PageTitle, Settings), _regID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get collection  used by GetViewableRegisterCell method
            msg = RegisterCell.FetchList(Me.DbConnection, regCells, _currentUser.ExternalUsername, _
                    AuditLogging.GetAuditLogTitle(PageTitle, Settings), _regID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get collection  used OutputRegisterControls method
            msg = vwRegisterWeek.FetchList(Me.DbConnection, register, , , , , e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'collection used by GetTotals method
            msg = vwRegisterDay.FetchList(conn:=Me.DbConnection, list:=registerDayCollection, registerID:=_regID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'set register status label
            With _aRegister
                regStatus = New Target.Abacus.Library.DataClasses.RegisterStatus(Me.DbConnection)
                With regStatus
                    msg = .Fetch(_aRegister.RegisterStatusID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    txtStatus.Text = .Description
                End With
                ' Get the headers 
                _weekEnding = .WeekEnding.ToShortDateString
                _establishmentID = .ProviderID
            End With

            'Get establishment name
            With _establishment
                _establishmentName = .Name
            End With

            'Hide custom buttons for registers dependant on status
            Select Case txtStatus.TextBox.Text
                Case "In Progress"
                    btnUnSubmit.Visible = False
                    btnAmend.Visible = False
                    btnSubmit.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCare.Submit"))
                    btnAddServiceUsers.Visible = True
                Case "Submitted"
                    btnSubmit.Visible = False
                    btnAmend.Visible = False
                    btnUnSubmit.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCare.UnSubmit"))
                    btnAddServiceUsers.Visible = True
                Case "Processed"
                    btnSubmit.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCare.Submit"))
                    btnUnSubmit.Visible = False
                    btnAddServiceUsers.Visible = True
                    btnAmend.Visible = False
                Case "Amended"
                    btnSubmit.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCare.Submit"))
                    btnUnSubmit.Visible = False
                    btnAmend.Visible = False
                    btnAddServiceUsers.Visible = True
                Case Else
            End Select

            rateFrameworkID = _contract.DomRateFrameworkID

            If ViewState("ddView") IsNot Nothing Then
                If CInt(ViewState("ddView")) <> RegisterView.Week Then
                    phRegisterHeading.Controls.Clear()
                    OutputRegisterControlsByDay(Me.DbConnection, CInt(ViewState("ddView")), CStr(ViewState("txtRegisterGroup")), regDays, regCells, registerDayCollection)
                    phRegisterFooting.Controls.Clear()
                    'get the rate category id's
                    If CStr(ViewState("txtRegisterGroup")) = "" Then
                        msg = DomRateCategory.FetchList(Me.DbConnection, rateCategoryCollection, String.Empty, String.Empty, , , rateFrameworkID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        For Each rateCategory As DomRateCategory In rateCategoryCollection
                            rateCategoryIDs.Add(rateCategory.ID)
                        Next
                    Else
                        msg = DomRateCategory.FetchList(Me.DbConnection, rateCategoryCollection, String.Empty, String.Empty, , , rateFrameworkID, CStr(ViewState("txtRegisterGroup")))
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        For Each rateCategory As DomRateCategory In rateCategoryCollection
                            rateCategoryIDs.Add(rateCategory.ID)
                        Next
                    End If
                    OutputRegisterColumnTotalsByDay(_regID, True, CStr(ViewState("txtRegisterGroup")), registerDayCollection, rateCategoryIDs)
                    OutputRegisterColumnTotalsByDay(_regID, False, CStr(ViewState("txtRegisterGroup")), registerDayCollection, rateCategoryIDs)
                    'Hide custom buttons for day view
                    btnUnSubmit.Visible = False
                    btnAmend.Visible = False
                    btnSubmit.Visible = False
                    btnAddServiceUsers.Visible = False
                End If

                If CInt(ViewState("ddView")) = RegisterView.Week Then
                    register = New vwRegisterWeekCollection

                    If CStr(ViewState("txtRegisterGroup")) <> "" Then
                        registerGroup = CStr(ViewState("txtRegisterGroup"))
                    End If

                    msg = vwRegisterWeek.FetchList(Me.DbConnection, register, , , , , _stdBut.SelectedItemID, , , , , , , , registerGroup)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ClearViewState()
                    list = GetUniqueIDsFromViewState()

                    phRegisterHeading.Controls.Clear()
                    OutputRegisterHeaders()
                    phRegisterWeek.Controls.Clear()
                    For Each registerWeek As vwRegisterWeek In register
                        Dim id As String = GetUniqueID(registerWeek)
                        OutputRegisterControls(id, registerWeek, Me.DbConnection, CStr(ViewState("txtRegisterGroup")), regDays, regCells, registerDayCollection)
                        list.Add(id)
                        _domContractID = registerWeek.DomContractID
                        _domContractTitle = registerWeek.ContractTitle
                    Next
                    phRegisterFooting.Controls.Clear()
                    OutputRegisterColumnTotalsByWeek(_regID, True, CStr(ViewState("txtRegisterGroup")), registerDayCollection)
                    OutputRegisterColumnTotalsByWeek(_regID, False, CStr(ViewState("txtRegisterGroup")), registerDayCollection)
                    PersistUniqueIDsToViewState(list)
                    btnUncheck.Visible = False
                End If
            Else
                ClearViewState()
                list = GetUniqueIDsFromViewState()

                phRegisterHeading.Controls.Clear()
                OutputRegisterHeaders()
                For Each registerWeek As vwRegisterWeek In register
                    Dim id As String = GetUniqueID(registerWeek)
                    OutputRegisterControls(id, registerWeek, Me.DbConnection, "", regDays, regCells, registerDayCollection)
                    list.Add(id)
                    _domContractID = registerWeek.DomContractID
                    _domContractTitle = registerWeek.ContractTitle
                Next
                phRegisterFooting.Controls.Clear()
                OutputRegisterColumnTotalsByWeek(_regID, True, "", registerDayCollection)
                OutputRegisterColumnTotalsByWeek(_regID, False, "", registerDayCollection)
                PersistUniqueIDsToViewState(list)
                btnUncheck.Visible = False
            End If
        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If ViewState("ddView") IsNot Nothing Then
                ddView.SelectedValue = CInt(ViewState("ddView"))
            End If

            If ViewState("txtRegisterGroup") IsNot Nothing Then
                txtRegisterGroup.RegisterGroupText = CStr(ViewState("txtRegisterGroup"))
            End If
            ClearViewState(e)
            FindClicked(e)
        End Sub

#End Region

#Region " EditClicked "

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            editClick = True
            FindClicked(e)
        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim allKeysList As String()
            Dim statusControlID As String = Nothing
            Dim statusDescription As String = Nothing
            Dim registerStatusDescription As String = Nothing
            Dim trans As SqlTransaction = Nothing
            Dim conn As SqlConnection = Nothing
            Dim registerStatusID As Integer
            Dim regStatus As Target.Abacus.Library.DataClasses.RegisterStatus
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim regCells As RegisterCellCollection = Nothing
            Dim regDays As RegisterDayCollection = Nothing

            Dim tickedControlsList As ArrayList = New ArrayList
            Dim updatedCellsList As ArrayList = New ArrayList

            Try
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                trans = SqlHelper.GetTransaction(conn)

                ' the collections used by getviewable register cell
                msg = RegisterDay.FetchList(trans, regDays, currentUser.ExternalUsername, _
                                            AuditLogging.GetAuditLogTitle(PageTitle, Settings), _stdBut.SelectedItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                msg = RegisterCell.FetchList(trans, regCells, currentUser.ExternalUsername, _
                        AuditLogging.GetAuditLogTitle(PageTitle, Settings), _stdBut.SelectedItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                Dim spParams1 As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(conn, SP_LOCK_REGISTER, False)
                spParams1(0).Value = e.ItemID
                registerStatusID = SqlHelper.ExecuteScalar(trans, CommandType.StoredProcedure, SP_LOCK_REGISTER, spParams1)
                If Not registerStatusID > 0 Then
                    Response.Write("<script>alert('Unable to retrieve register record. Please check if the register exists');</script>")
                    Response.Redirect(HttpUtility.UrlEncode(Target.Library.Utils.ToString(Request.QueryString("backUrl"))))
                Else
                    regStatus = New Target.Abacus.Library.DataClasses.RegisterStatus(trans)
                    'get week ending date
                    With regStatus
                        msg = .Fetch(registerStatusID)
                        If Not msg.Success Then
                            msg.Message = "Unable to retrieve register status. Please check if the register status exists"
                            WebUtils.DisplayError(msg)
                        End If
                        registerStatusDescription = .Description
                    End With

                    allKeysList = Request.Form.AllKeys()

                    For Each key As String In allKeysList
                        If key.EndsWith(CTRL_TYPE_SUFFIX_HORIGINAL) Then
                            Dim currentValue As String, originalValue As String

                            currentValue = Request.Form(key.Replace(CTRL_TYPE_SUFFIX_HORIGINAL, CTRL_TYPE_SUFFIX_CHKBOX))
                            If String.IsNullOrEmpty(currentValue) Then
                                currentValue = CHK_OFF
                            End If

                            originalValue = Request.Form(key)
                            If originalValue = Boolean.TrueString Then
                                originalValue = CHK_ON
                            Else
                                originalValue = CHK_OFF
                            End If

                            ' currently ticked (the outcome might have changed) or the ticked-ness has changed, we want to look at this cell
                            If currentValue = CHK_ON Or currentValue <> originalValue Then
                                tickedControlsList.Add(key)
                            End If
                        End If
                        If key.Contains(CTRL_NAME_TXTSTATUS) Then
                            statusControlID = key
                            statusDescription = Request.Form(statusControlID)
                        End If
                    Next

                    If statusDescription <> "Processed" And statusDescription <> "Submitted" _
                    And registerStatusDescription = statusDescription Or registerStatusDescription = "Amended" Then
                        For Each item As String In tickedControlsList
                            Dim unPopulatedCellView As ViewableRegisterCell
                            Dim populatedCellView As ViewableRegisterCell
                            Dim uniqueID As String
                            Dim controlName As String, controlValue As String
                            Dim newServiceOutcomeID As Integer

                            uniqueID = item.Substring(item.IndexOf("U_") + 2)
                            uniqueID = uniqueID.Replace(CTRL_TYPE_SUFFIX_HORIGINAL, "")

                            unPopulatedCellView = New ViewableRegisterCell()
                            msg = DomContractBL.GetViewableRegisterCellByUniqueID(unPopulatedCellView, uniqueID, True)

                            populatedCellView = New ViewableRegisterCell()
                            msg = DomContractBL.GetViewableRegisterCell(populatedCellView, String.Copy(uniqueID), unPopulatedCellView.DayOfWeek, regDays, regCells)

                            controlName = item.Replace(CTRL_TYPE_SUFFIX_HORIGINAL, CTRL_TYPE_SUFFIX_CHKBOX)
                            controlValue = Request.Form(controlName)
                            If Not String.IsNullOrEmpty(controlValue) AndAlso controlValue = CHK_ON Then
                                unPopulatedCellView.Attended = True
                            Else
                                unPopulatedCellView.Attended = False
                            End If

                            controlName = String.Format("{0}{1}{2}", CTRL_NAME_PREFIX_HOUTCOME, uniqueID, HID_CTRL_NAME_SUFFIX)
                            newServiceOutcomeID = Target.Library.Utils.ToInt32(Request.Form(controlName))

                            If newServiceOutcomeID <> populatedCellView.ServiceOutcomeID Then
                                unPopulatedCellView.ServiceOutcomeID = newServiceOutcomeID
                                updatedCellsList.Add(unPopulatedCellView)
                            End If

                        Next

                        For Each cell As ViewableRegisterCell In updatedCellsList
                            msg = DomContractBL.UpdateRegisterActuals(trans, "", cell.ServiceOutcomeID, cell.RegisterID, cell.DayOfWeek, _
                                                                      cell.RegisterRowID, cell.RegisterColumnID, cell.Attended, _
                                                                      cell.DomRateCategoryID, cell.ClientID, statusDescription, _
                                                                      Me.PageTitle, Me.Settings)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        Next

                        trans.Commit()

                        msg = New ErrorMessage()
                        msg.Message = "Save Completed"
                        msg.Success = True

                        If ViewState("ddView") IsNot Nothing Then
                            ddView.SelectedValue = CInt(ViewState("ddView"))
                        End If

                        If ViewState("txtRegisterGroup") IsNot Nothing Then
                            txtRegisterGroup.RegisterGroupText = CStr(ViewState("txtRegisterGroup"))
                        End If
                        ClearViewState(e)

                        Dim currentReg As StdButtonEventArgs = New StdButtonEventArgs(False, _stdBut.SelectedItemID, _stdBut)
                        FindClicked(currentReg)
                    Else
                        ClientScript.RegisterStartupScript(Me.GetType(), "myScript", "<script language=JavaScript>saveConfirmation();</script>")
                    End If
                End If

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0001")     ' unexpected
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                SqlHelper.CloseConnection(conn)
            End Try

        End Sub

#End Region

#Region " StdButtons_AddCustomControls "

        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)
            Dim lblLocation As New Label
            Dim msg As ErrorMessage

            lblLocation.Text = "Register Group"
            lblLocation.ForeColor = Color.Blue
            lblLocation.Font.Bold = True
            lblLocation.Style.Add("padding-right", "1em")
            lblLocation.Style.Add("padding-left", "1em")
            controls.Add(lblLocation)

            If _domContractID > 0 Then
                If _contract Is Nothing Then _contract = New  _
                Target.Abacus.Library.DataClasses.DomContract(Me.DbConnection, String.Empty, String.Empty)
                msg = _contract.Fetch(_domContractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            Else
                _regID = Target.Library.Utils.ToInt32(Request.QueryString("ID"))
                msg = RegisterRow.FetchList(Me.DbConnection, _regRows, String.Empty, String.Empty, _regID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                For Each row As RegisterRow In _regRows
                    _domContractID = row.DomContractID
                    If _domContractID > 0 Then Exit For
                Next

                _contract = New Target.Abacus.Library.DataClasses.DomContract(Me.DbConnection, String.Empty, String.Empty)
                msg = _contract.Fetch(_domContractID)
            End If

            txtRegisterGroup = Me.LoadControl(WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceRegisterGroupSelector.ascx"))
            With txtRegisterGroup
                .ID = "txtRegisterGroup"
                .Required = False
                .FrameworkID = _contract.DomRateFrameworkID
            End With
            controls.Add(txtRegisterGroup)

            Dim lblView As New Label
            lblView.Text = "View"
            lblView.ForeColor = Color.Blue
            lblView.Font.Bold = True
            lblView.Style.Add("padding-right", "1em")
            lblView.Style.Add("padding-left", "1em")
            controls.Add(lblView)

            ddView.Items.Add(New ListItem("Week", 7))
            ddView.Items.Add(New ListItem("Mon", 1))
            ddView.Items.Add(New ListItem("Tue", 2))
            ddView.Items.Add(New ListItem("Wed", 3))
            ddView.Items.Add(New ListItem("Thu", 4))
            ddView.Items.Add(New ListItem("Fri", 5))
            ddView.Items.Add(New ListItem("Sat", 6))
            ddView.Items.Add(New ListItem("Sun", 0))
            ddView.ID = "ddView"
            ddView.AutoPostBack = False
            controls.Add(ddView)

            Dim lblSpacer As New Label
            lblSpacer.Style.Add("padding-left", "1em")
            controls.Add(lblSpacer)

            btnRefresh.Text = "Refresh"
            btnRefresh.ID = "btnRefresh"
            controls.Add(btnRefresh)

            Dim lblStatusTitle As New Label
            lblStatusTitle.Text = "Status: "
            lblStatusTitle.ForeColor = Color.Blue
            lblStatusTitle.Font.Bold = True
            lblStatusTitle.Style.Add("padding-left", "1em")
            controls.Add(lblStatusTitle)

            controls.Add(txtStatus)
            txtStatus.TextBox.Attributes.Add("readonly", "readonly")
            txtStatus.TextBox.BorderWidth = 0
            txtStatus.OutputBrAfter = False
            txtStatus.TextBox.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)

        End Sub

#End Region

#Region " OutputRegisterHeadersByDay "
        Private Sub OutputRegisterHeadersByDay(ByVal contractId As Integer, ByVal registerGrp As String)
            Dim rateCategoryCollection As New DomRateCategoryCollection
            Dim msg As ErrorMessage
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim rateDescriptions As New ArrayList
            Dim contract As Target.Abacus.Library.DataClasses.DomContract
            Dim rateFrameWorkId As Integer
            Dim registerGroup As String = Nothing

            'get the rate frameworkid
            contract = New Target.Abacus.Library.DataClasses.DomContract(Me.DbConnection, String.Empty, String.Empty)
            With contract
                msg = .Fetch(contractId)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                rateFrameWorkId = .DomRateFrameworkID
            End With

            'get the rate category descriptions
            If registerGrp <> "" Then
                registerGroup = registerGrp
            End If

            msg = DomRateCategory.FetchList(Me.DbConnection, rateCategoryCollection, String.Empty, String.Empty, , , rateFrameWorkId, registerGroup)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            For Each rateCategory As DomRateCategory In rateCategoryCollection
                rateDescriptions.Add(rateCategory.Description)
            Next

            row = New HtmlTableRow()
            row.ID = "categoriesheader"

            phRegisterHeading.Controls.Add(row)

            ' Service User Reference
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "S/U Ref"

            ' Service User Surname
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Surname"

            ' Service User FirstName
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "First<br/>Name"

            'loop through and add the rate categories
            For Each rate As String In rateDescriptions
                cell = New HtmlTableCell("th")
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cell.Attributes.Add("class", "a")
                cell.InnerHtml = rate
            Next
            ' Total Planned
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Total<br/>Planned"

            ' Total Actual
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Total<br/>Actual"
        End Sub
#End Region

#Region " OutputRegisterHeaders "
        Private Sub OutputRegisterHeaders()
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell

            row = New HtmlTableRow()
            row.ID = "RegisterWeekHeader"

            phRegisterHeading.Controls.Add(row)

            ' Service User Reference
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "S/U Ref"

            ' Service User Surname
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Surname"

            ' Service User FirstName
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "First<br/>Name"

            ' Rate Category
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Rate<br/>Category"

            ' Monday
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Mon"

            ' Tuesday
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Tue"

            ' Wednesday
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Wed"

            ' Thursday
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Thu"

            ' Friday
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Fri"

            ' Saturday
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Sat"

            ' Sunday
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Sun"

            ' Total Planned
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Total<br>Planned"

            ' Total Actual
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "Total<br>Actual"
        End Sub
#End Region

#Region " OutputRegisterControls "

        Private Sub OutputRegisterControls(ByVal uniqueID As String, ByVal register As vwRegisterWeek, _
                                      ByVal conn As SqlConnection, ByVal registerGrp As String, _
                                      ByVal regDays As RegisterDayCollection, ByVal regCells As RegisterCellCollection, _
                                      ByVal vwRegDays As vwRegisterDayCollection)

            Dim msg As ErrorMessage
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim serviceuserref As Label
            Dim surname As Label
            Dim firstname As Label
            Dim ratecategory As Label
            Dim cbkMon As CheckBoxEx

            Dim hidOutcome As HiddenField
            Dim txtOutcome As Label
            Dim cbkTue As CheckBoxEx
            Dim cbkWed As CheckBoxEx
            Dim cbkThu As CheckBoxEx
            Dim cbkFri As CheckBoxEx
            Dim cbkSat As CheckBoxEx
            Dim cbkSun As CheckBoxEx
            Dim totalPlanned As Label
            Dim totalActual As Label
            Dim currentRegCell As ViewableRegisterCell = Nothing
            Dim cellID As String = ""
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

                row = New HtmlTableRow()
                row.ID = uniqueID

                phRegisterWeek.Controls.Add(row)

                ' Service User Reference
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                serviceuserref = New Label()
                With serviceuserref
                    .Width = New Unit(6, UnitType.Em)
                    If Not register Is Nothing Then .Text = register.ClientReference
                End With
                cell.Controls.Add(serviceuserref)

                ' Service User Surname
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                surname = New Label()
                With surname
                    .Width = New Unit(6, UnitType.Em)
                    If Not register Is Nothing Then .Text = register.LastName
                End With
                cell.Controls.Add(surname)

                ' Service User FirstName
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                firstname = New Label()
                With firstname
                    .Width = New Unit(10, UnitType.Em)
                    If Not register Is Nothing Then .Text = register.FirstNames
                End With
                cell.Controls.Add(firstname)

                ' Rate Category Description
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a b")
                ratecategory = New Label()
                With ratecategory
                    .Width = New Unit(8, UnitType.Em)
                    If Not register Is Nothing Then .Text = register.Description
                End With
                cell.Controls.Add(ratecategory)

                ' Monday
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                cbkMon = New CheckBoxEx()
                hidOutcome = New HiddenField
                txtOutcome = New Label
                With cbkMon
                    .Label.Visible = False
                    If Not register Is Nothing Then
                        cellID = uniqueID
                        cellID = uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_CONTROL, String.Empty)
                        msg = DomContractBL.GetViewableRegisterCell(currentRegCell, cellID, DayOfWeek.Monday, regDays, regCells)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        If currentRegCell.Attended AndAlso currentRegCell.RegisterColumnID > 0 Then .CheckBox.Checked = register.onMonday
                        If currentRegCell.Planned Then .CheckBox.BorderStyle = BorderStyle.Groove
                        .ID = CTRL_PREFIX_MON & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID

                        hidOutcome.ID = CTRL_PREFIX_HOUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID & "_hidField"
                        txtOutcome.ID = CTRL_PREFIX_OUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID
                    End If
                End With
                With txtOutcome
                    .Width = New Unit(3, UnitType.Em)
                End With
                If Not currentRegCell Is Nothing Then
                    If ((currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned <> False) Or _
                    (currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned = False And _
                     currentRegCell.Attended = True And currentRegCell.RegisterColumnID > 0)) Then
                        msg = GetServiceOutcomeCode(txtOutcome, currentRegCell.ServiceOutcomeID, _serviceOutComes)
                        hidOutcome.Value = currentRegCell.ServiceOutcomeID
                    End If
                End If

                cell.Controls.Add(cbkMon)
                cell.Controls.Add(hidOutcome)
                cell.Controls.Add(txtOutcome)
                cbkMon.CheckBox.Attributes.Add( _
                    "onclick", _
                    String.Format("checkBox_Clicked('{0}_chkCheckbox','{1}');", cbkMon.ClientID(), cellID) _
                )

                ' Tuesday
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                cbkTue = New CheckBoxEx()
                hidOutcome = New HiddenField
                txtOutcome = New Label
                With cbkTue
                    .Label.Visible = False
                    If Not register Is Nothing Then
                        cellID = uniqueID
                        cellID = uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_CONTROL, String.Empty)
                        msg = DomContractBL.GetViewableRegisterCell(currentRegCell, cellID, DayOfWeek.Tuesday, regDays, regCells)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        If currentRegCell.Attended AndAlso currentRegCell.RegisterColumnID > 0 Then .CheckBox.Checked = register.onTuesday
                        If currentRegCell.Planned Then .CheckBox.BorderStyle = BorderStyle.Groove
                        .ID = CTRL_PREFIX_TUE & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID

                        hidOutcome.ID = CTRL_PREFIX_HOUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID & "_hidField"
                        txtOutcome.ID = CTRL_PREFIX_OUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID
                    End If
                End With
                With txtOutcome
                    .Width = New Unit(3, UnitType.Em)
                End With
                If Not currentRegCell Is Nothing Then
                    If ((currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned <> False) Or _
                    (currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned = False And _
                     currentRegCell.Attended = True And currentRegCell.RegisterColumnID > 0)) Then
                        msg = GetServiceOutcomeCode(txtOutcome, currentRegCell.ServiceOutcomeID, _serviceOutComes)
                        hidOutcome.Value = currentRegCell.ServiceOutcomeID
                    End If
                End If

                cell.Controls.Add(cbkTue)
                cell.Controls.Add(hidOutcome)
                cell.Controls.Add(txtOutcome)
                cbkTue.CheckBox.Attributes.Add( _
                    "onclick", _
                    String.Format("checkBox_Clicked('{0}_chkCheckbox','{1}');", cbkTue.ClientID(), cellID) _
                )

                ' Wednesday
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                cbkWed = New CheckBoxEx()
                hidOutcome = New HiddenField
                txtOutcome = New Label
                With cbkWed
                    .Label.Visible = False
                    If Not register Is Nothing Then
                        cellID = uniqueID
                        cellID = uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_CONTROL, String.Empty)
                        msg = DomContractBL.GetViewableRegisterCell(currentRegCell, cellID, DayOfWeek.Wednesday, regDays, regCells)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        If currentRegCell.Attended AndAlso currentRegCell.RegisterColumnID > 0 Then .CheckBox.Checked = register.onWednesday
                        If currentRegCell.Planned Then .CheckBox.BorderStyle = BorderStyle.Groove
                        .ID = CTRL_PREFIX_WED & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID

                        hidOutcome.ID = CTRL_PREFIX_HOUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID & "_hidField"
                        txtOutcome.ID = CTRL_PREFIX_OUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID
                    End If
                End With
                With txtOutcome
                    .Width = New Unit(3, UnitType.Em)
                End With
                If Not currentRegCell Is Nothing Then
                    If ((currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned <> False) Or _
                    (currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned = False And _
                     currentRegCell.Attended = True And currentRegCell.RegisterColumnID > 0)) Then
                        msg = GetServiceOutcomeCode(txtOutcome, currentRegCell.ServiceOutcomeID, _serviceOutComes)
                        hidOutcome.Value = currentRegCell.ServiceOutcomeID
                    End If
                End If

                cell.Controls.Add(cbkWed)
                cell.Controls.Add(hidOutcome)
                cell.Controls.Add(txtOutcome)
                cbkWed.CheckBox.Attributes.Add( _
                    "onclick", _
                    String.Format("checkBox_Clicked('{0}_chkCheckbox','{1}');", cbkWed.ClientID(), cellID) _
                )

                ' Thursday
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                cbkThu = New CheckBoxEx()
                hidOutcome = New HiddenField
                txtOutcome = New Label
                With cbkThu
                    .Label.Visible = False
                    If Not register Is Nothing Then
                        cellID = uniqueID
                        cellID = uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_CONTROL, String.Empty)
                        msg = DomContractBL.GetViewableRegisterCell(currentRegCell, cellID, DayOfWeek.Thursday, regDays, regCells)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        If currentRegCell.Attended AndAlso currentRegCell.RegisterColumnID > 0 Then .CheckBox.Checked = register.onThursday
                        If currentRegCell.Planned Then .CheckBox.BorderStyle = BorderStyle.Groove
                        .ID = CTRL_PREFIX_THU & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID

                        hidOutcome.ID = CTRL_PREFIX_HOUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID & "_hidField"
                        txtOutcome.ID = CTRL_PREFIX_OUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID
                    End If
                End With
                With txtOutcome
                    .Width = New Unit(3, UnitType.Em)
                End With
                If Not currentRegCell Is Nothing Then
                    If ((currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned <> False) Or _
                    (currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned = False And _
                     currentRegCell.Attended = True And currentRegCell.RegisterColumnID > 0)) Then
                        msg = GetServiceOutcomeCode(txtOutcome, currentRegCell.ServiceOutcomeID, _serviceOutComes)
                        hidOutcome.Value = currentRegCell.ServiceOutcomeID
                    End If
                End If

                cell.Controls.Add(cbkThu)
                cell.Controls.Add(hidOutcome)
                cell.Controls.Add(txtOutcome)
                cbkThu.CheckBox.Attributes.Add( _
                    "onclick", _
                    String.Format("checkBox_Clicked('{0}_chkCheckbox','{1}');", cbkThu.ClientID(), cellID) _
                )

                ' Friday
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                cbkFri = New CheckBoxEx()
                hidOutcome = New HiddenField
                txtOutcome = New Label
                With cbkFri
                    .Label.Visible = False
                    If Not register Is Nothing Then
                        cellID = uniqueID
                        cellID = uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_CONTROL, String.Empty)
                        msg = DomContractBL.GetViewableRegisterCell(currentRegCell, cellID, DayOfWeek.Friday, regDays, regCells)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        If currentRegCell.Attended AndAlso currentRegCell.RegisterColumnID > 0 Then .CheckBox.Checked = register.onFriday
                        If currentRegCell.Planned Then .CheckBox.BorderStyle = BorderStyle.Groove
                        .ID = CTRL_PREFIX_FRI & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID

                        hidOutcome.ID = CTRL_PREFIX_HOUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID & "_hidField"
                        txtOutcome.ID = CTRL_PREFIX_OUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID
                    End If
                End With
                With txtOutcome
                    .Width = New Unit(3, UnitType.Em)
                End With
                If Not currentRegCell Is Nothing Then
                    If ((currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned <> False) Or _
                    (currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned = False And _
                     currentRegCell.Attended = True And currentRegCell.RegisterColumnID > 0)) Then
                        msg = GetServiceOutcomeCode(txtOutcome, currentRegCell.ServiceOutcomeID, _serviceOutComes)
                        hidOutcome.Value = currentRegCell.ServiceOutcomeID
                    End If
                End If

                cell.Controls.Add(cbkFri)
                cell.Controls.Add(hidOutcome)
                cell.Controls.Add(txtOutcome)
                cbkFri.CheckBox.Attributes.Add( _
                    "onclick", _
                    String.Format("checkBox_Clicked('{0}_chkCheckbox','{1}');", cbkFri.ClientID(), cellID) _
                )

                ' Saturday
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                cbkSat = New CheckBoxEx()
                hidOutcome = New HiddenField
                txtOutcome = New Label
                With cbkSat
                    .Label.Visible = False
                    If Not register Is Nothing Then
                        cellID = uniqueID
                        cellID = uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_CONTROL, String.Empty)
                        msg = DomContractBL.GetViewableRegisterCell(currentRegCell, cellID, DayOfWeek.Saturday, regDays, regCells)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        If currentRegCell.Attended AndAlso currentRegCell.RegisterColumnID > 0 Then .CheckBox.Checked = register.onSaturday
                        If currentRegCell.Planned Then .CheckBox.BorderStyle = BorderStyle.Groove
                        .ID = CTRL_PREFIX_SAT & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID

                        hidOutcome.ID = CTRL_PREFIX_HOUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID & "_hidField"
                        txtOutcome.ID = CTRL_PREFIX_OUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID
                    End If
                End With
                With txtOutcome
                    .Width = New Unit(3, UnitType.Em)
                End With
                If Not currentRegCell Is Nothing Then
                    If ((currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned <> False) Or _
                    (currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned = False And _
                     currentRegCell.Attended = True And currentRegCell.RegisterColumnID > 0)) Then
                        msg = GetServiceOutcomeCode(txtOutcome, currentRegCell.ServiceOutcomeID, _serviceOutComes)
                        hidOutcome.Value = currentRegCell.ServiceOutcomeID
                    End If
                End If

                cell.Controls.Add(cbkSat)
                cell.Controls.Add(hidOutcome)
                cell.Controls.Add(txtOutcome)
                cbkSat.CheckBox.Attributes.Add( _
                    "onclick", _
                    String.Format("checkBox_Clicked('{0}_chkCheckbox','{1}');", cbkSat.ClientID(), cellID) _
                )

                ' Sunday
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                cbkSun = New CheckBoxEx()
                hidOutcome = New HiddenField
                txtOutcome = New Label
                With cbkSun
                    .Label.Visible = False
                    If Not register Is Nothing Then
                        cellID = uniqueID
                        cellID = uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_CONTROL, String.Empty)
                        msg = DomContractBL.GetViewableRegisterCell(currentRegCell, cellID, DayOfWeek.Sunday, regDays, regCells)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        If currentRegCell.Attended AndAlso currentRegCell.RegisterColumnID > 0 Then .CheckBox.Checked = register.onSunday
                        If currentRegCell.Planned Then .CheckBox.BorderStyle = BorderStyle.Groove
                        .ID = CTRL_PREFIX_SUN & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID

                        hidOutcome.ID = CTRL_PREFIX_HOUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID & "_hidField"
                        txtOutcome.ID = CTRL_PREFIX_OUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID
                    End If
                End With
                With txtOutcome
                    .Width = New Unit(3, UnitType.Em)
                End With
                If Not currentRegCell Is Nothing Then
                    If ((currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned <> False) Or _
                    (currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned = False And _
                     currentRegCell.Attended = True And currentRegCell.RegisterColumnID > 0)) Then
                        msg = GetServiceOutcomeCode(txtOutcome, currentRegCell.ServiceOutcomeID, _serviceOutComes)
                        hidOutcome.Value = currentRegCell.ServiceOutcomeID
                    End If
                End If

                cell.Controls.Add(cbkSun)
                cell.Controls.Add(hidOutcome)
                cell.Controls.Add(txtOutcome)
                cbkSun.CheckBox.Attributes.Add( _
                    "onclick", _
                    String.Format("checkBox_Clicked('{0}_chkCheckbox','{1}');", cbkSun.ClientID(), cellID) _
                )

                ' total planned
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                totalPlanned = New Label()
                With totalPlanned
                    .ID = CTRL_PREFIX_TOTAL_PLANNED & uniqueID
                    .Width = New Unit(2, UnitType.Em)
                    If Not register Is Nothing Then .Text = register.TotalUnits
                End With
                GetTotals(totalPlanned, False, uniqueID, registerGrp, vwRegDays)
                cell.Controls.Add(totalPlanned)

                ' total actual
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Attributes.Add("class", "a")
                totalActual = New Label()
                With totalActual
                    .ID = CTRL_PREFIX_TOTAL_ACTUAL & uniqueID
                    .Width = New Unit(2, UnitType.Em)
                    If Not register Is Nothing Then .Text = register.TotalUnits
                End With
                GetTotals(totalActual, True, uniqueID, registerGrp, vwRegDays)
                cell.Controls.Add(totalActual)

                'Set the colour to pale orange for actuals that exceed planned
                Dim actualNumber, plannedNumber As Integer
                actualNumber = totalActual.Text
                plannedNumber = totalPlanned.Text

                If actualNumber > plannedNumber Then
                    totalActual.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                    cell.Style.Add("background-color", "#ffddac")
                Else
                    totalActual.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                    cell.Style.Add("background-color", "#FFFFFF")
                End If

            End If

            _registerDay = "MO, TU, WE, TH, FR, SA, SU"
        End Sub

#End Region

#Region " OutputRegisterColumnTotalsByDay "
        Private Sub OutputRegisterColumnTotalsByDay(ByVal registerId As String, ByVal planned As Boolean, _
                                                    ByVal registerGrp As String, ByVal vwRegDays As vwRegisterDayCollection, _
                                                    ByVal rateCategoryIDs As ArrayList)
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim msg As ErrorMessage
            Dim registerDayCollection As vwRegisterDayCollection = New vwRegisterDayCollection
            Dim rateFrameWorkId As Integer
            Dim rateCategoryCollection As New DomRateCategoryCollection
            Dim contractID As Integer = 0
            Dim regRowCollection As RegisterRowCollection = New RegisterRowCollection
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim total As Integer = 0

            contractID = _domContractID

            'get the rate frameworkid
            With _contract
                rateFrameWorkId = .DomRateFrameworkID
            End With

            Dim totalControl As Label

            row = New HtmlTableRow()

            If planned = True Then
                row.ID = "WeeklyTotalPlanned"
            Else
                row.ID = "WeeklyTotalActual"
            End If

            phRegisterFooting.Controls.Add(row)

            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.Style.Add("background-color", "#FFFFFF")
            cell.InnerHtml = "&nbsp"

            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.Style.Add("background-color", "#FFFFFF")
            cell.InnerHtml = "&nbsp"

            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.Style.Add("background-color", "#FFFFFF")
            cell.InnerHtml = "&nbsp"

            If planned = True Then
                cell.InnerHtml = "Total Planned"
            Else
                cell.InnerHtml = "Total Actual"
            End If

            'loop through and add the rate categories
            For Each rate As Integer In rateCategoryIDs
                cell = New HtmlTableCell("td")
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cell.Attributes.Add("class", "a")
                cell.Style.Add("background-color", "#FFFFFF")
                totalControl = New Label()
                With totalControl
                    If planned = True Then
                        .ID = CTRL_PREFIX_TOTAL_PLANNED & "_" & rate & "_column"
                    Else
                        .ID = CTRL_PREFIX_TOTAL_ACTUAL & "_" & rate & "_column"
                    End If
                    .Width = New Unit(2, UnitType.Em)
                End With
                If planned = True Then
                    msg = GetColumnTotals(totalControl, registerId, False, vwRegDays, CInt(ViewState("ddView")), rate, registerGrp)
                Else
                    msg = GetColumnTotals(totalControl, registerId, True, vwRegDays, CInt(ViewState("ddView")), rate, registerGrp)
                End If
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If planned <> True Then
                    Select Case CInt(ViewState("ddView"))
                        Case DayOfWeek.Sunday
                            If registerGrp = "" Then
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnSunday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            Else
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnSunday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.RegisterGroup.Contains(registerGrp) And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            End If
                        Case DayOfWeek.Monday
                            If registerGrp = "" Then
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnMonday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            Else
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnMonday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.RegisterGroup.Contains(registerGrp) And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            End If
                        Case DayOfWeek.Tuesday
                            If registerGrp = "" Then
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnTuesday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            Else
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnTuesday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.RegisterGroup.Contains(registerGrp) And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            End If
                        Case DayOfWeek.Wednesday
                            If registerGrp = "" Then
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnWednesday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            Else
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnWednesday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.RegisterGroup.Contains(registerGrp) And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            End If
                        Case DayOfWeek.Thursday
                            If registerGrp = "" Then
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnThursday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            Else
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnThursday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.RegisterGroup.Contains(registerGrp) And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            End If
                        Case DayOfWeek.Friday
                            If registerGrp = "" Then
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnFriday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            Else
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnFriday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.RegisterGroup.Contains(registerGrp) And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            End If
                        Case DayOfWeek.Saturday
                            If registerGrp = "" Then
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnSaturday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            Else
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.DomRateCategoryID = rate And _
                                       vwRegDay.OnSaturday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.RegisterGroup.Contains(registerGrp) And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                If totalControl.Text > total Then
                                    cell.Style.Add("background-color", "#ffddac")
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                Else
                                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                End If
                                Exit Select
                            End If
                        Case Else
                            cell.Style.Add("background-color", "#FFFFFF")
                    End Select
                Else
                    totalControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                End If
                cell.Controls.Add(totalControl)
            Next

            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.Style.Add("background-color", "#FFFFFF")
            cell.InnerHtml = "&nbsp"

            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.Attributes.Add("class", "a")
            cell.Style.Add("background-color", "#FFFFFF")
            cell.InnerHtml = "&nbsp"

        End Sub
#End Region

#Region " OutputRegisterColumnTotalsByWeek "
        Private Sub OutputRegisterColumnTotalsByWeek(ByVal registerId As String, ByVal planned As Boolean, _
                                                     ByVal registerGrp As String, ByVal vwRegDays As vwRegisterDayCollection)
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim msg As ErrorMessage

            Dim totalControl As Label

            row = New HtmlTableRow()

            If planned = True Then
                row.ID = "WeeklyTotalPlanned"
            Else
                row.ID = "WeeklyTotalActual"
            End If

            phRegisterFooting.Controls.Add(row)

            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "&nbsp"

            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "&nbsp"

            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "&nbsp"

            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "&nbsp"

            If planned = True Then
                cell.InnerHtml = "Total Planned"
            Else
                cell.InnerHtml = "Total Actual"
            End If

            ' Monday
            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            totalControl = New Label()
            With totalControl
                If planned = True Then
                    .ID = CTRL_PREFIX_TOTAL_PLANNED & "_mon_column"
                Else
                    .ID = CTRL_PREFIX_TOTAL_ACTUAL & "_mon_column"
                End If
                .Width = New Unit(2, UnitType.Em)
            End With
            If planned = True Then
                msg = GetColumnTotals(totalControl, registerId, False, vwRegDays, DayOfWeek.Monday, , registerGrp)
            Else
                msg = GetColumnTotals(totalControl, registerId, True, vwRegDays, DayOfWeek.Monday, , registerGrp)
            End If
            If Not msg.Success Then WebUtils.DisplayError(msg)
            cell.Controls.Add(totalControl)

            ' Tuesday
            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            totalControl = New Label()
            With totalControl
                If planned = True Then
                    .ID = CTRL_PREFIX_TOTAL_PLANNED & "_tue_column"
                Else
                    .ID = CTRL_PREFIX_TOTAL_ACTUAL & "_tue_column"
                End If
                .Width = New Unit(2, UnitType.Em)
            End With
            If planned = True Then
                msg = GetColumnTotals(totalControl, registerId, False, vwRegDays, DayOfWeek.Tuesday, , registerGrp)
            Else
                msg = GetColumnTotals(totalControl, registerId, True, vwRegDays, DayOfWeek.Tuesday, , registerGrp)
            End If
            If Not msg.Success Then WebUtils.DisplayError(msg)
            cell.Controls.Add(totalControl)

            ' Wednesday
            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            totalControl = New Label()
            With totalControl
                If planned = True Then
                    .ID = CTRL_PREFIX_TOTAL_PLANNED & "_wed_column"
                Else
                    .ID = CTRL_PREFIX_TOTAL_ACTUAL & "_wed_column"
                End If
                .Width = New Unit(2, UnitType.Em)
            End With
            If planned = True Then
                msg = GetColumnTotals(totalControl, registerId, False, vwRegDays, DayOfWeek.Wednesday, , registerGrp)
            Else
                msg = GetColumnTotals(totalControl, registerId, True, vwRegDays, DayOfWeek.Wednesday, , registerGrp)
            End If
            If Not msg.Success Then WebUtils.DisplayError(msg)
            cell.Controls.Add(totalControl)

            ' Thursday
            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            totalControl = New Label()
            With totalControl
                If planned = True Then
                    .ID = CTRL_PREFIX_TOTAL_PLANNED & "_thu_column"
                Else
                    .ID = CTRL_PREFIX_TOTAL_ACTUAL & "_thu_column"
                End If
                .Width = New Unit(2, UnitType.Em)
            End With
            If planned = True Then
                msg = GetColumnTotals(totalControl, registerId, False, vwRegDays, DayOfWeek.Thursday, , registerGrp)
            Else
                msg = GetColumnTotals(totalControl, registerId, True, vwRegDays, DayOfWeek.Thursday, , registerGrp)
            End If
            If Not msg.Success Then WebUtils.DisplayError(msg)
            cell.Controls.Add(totalControl)

            ' Friday
            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            totalControl = New Label()
            With totalControl
                If planned = True Then
                    .ID = CTRL_PREFIX_TOTAL_PLANNED & "_fri_column"
                Else
                    .ID = CTRL_PREFIX_TOTAL_ACTUAL & "_fri_column"
                End If
                .Width = New Unit(2, UnitType.Em)
            End With
            If planned = True Then
                msg = GetColumnTotals(totalControl, registerId, False, vwRegDays, DayOfWeek.Friday, , registerGrp)
            Else
                msg = GetColumnTotals(totalControl, registerId, True, vwRegDays, DayOfWeek.Friday, , registerGrp)
            End If
            If Not msg.Success Then WebUtils.DisplayError(msg)
            cell.Controls.Add(totalControl)

            ' Saturday
            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            totalControl = New Label()
            With totalControl
                If planned = True Then
                    .ID = CTRL_PREFIX_TOTAL_PLANNED & "_sat_column"
                Else
                    .ID = CTRL_PREFIX_TOTAL_ACTUAL & "_sat_column"
                End If
                .Width = New Unit(2, UnitType.Em)
            End With
            If planned = True Then
                msg = GetColumnTotals(totalControl, registerId, False, vwRegDays, DayOfWeek.Saturday, , registerGrp)
            Else
                msg = GetColumnTotals(totalControl, registerId, True, vwRegDays, DayOfWeek.Saturday, , registerGrp)
            End If
            If Not msg.Success Then WebUtils.DisplayError(msg)
            cell.Controls.Add(totalControl)

            ' Sunday
            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            totalControl = New Label()
            With totalControl
                If planned = True Then
                    .ID = CTRL_PREFIX_TOTAL_PLANNED & "_sun_column"
                Else
                    .ID = CTRL_PREFIX_TOTAL_ACTUAL & "_sun_column"
                End If
                .Width = New Unit(2, UnitType.Em)
            End With
            If planned = True Then
                msg = GetColumnTotals(totalControl, registerId, False, vwRegDays, DayOfWeek.Sunday, , registerGrp)
            Else
                msg = GetColumnTotals(totalControl, registerId, True, vwRegDays, DayOfWeek.Sunday, , registerGrp)
            End If
            If Not msg.Success Then WebUtils.DisplayError(msg)
            cell.Controls.Add(totalControl)

            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "&nbsp"

            cell = New HtmlTableCell("td")
            row.Controls.Add(cell)
            cell.Attributes.Add("class", "a")
            cell.InnerHtml = "&nbsp"

        End Sub
#End Region

#Region " OutputRegisterControlsByDay "

        Private Sub OutputRegisterControlsByDay(ByVal conn As SqlConnection, ByVal day As DayOfWeek, _
                                                ByVal registerGrp As String, ByVal regDays As RegisterDayCollection, _
                                                ByVal regCells As RegisterCellCollection, ByVal vwRegDays As vwRegisterDayCollection)

            Dim msg As ErrorMessage
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim serviceuserref As Label
            Dim surname As Label
            Dim firstname As Label
            Dim cbkRateCategory As CheckBoxEx
            Dim hidOutcome As HiddenField
            Dim txtOutcome As Label
            Dim currentRegCell As ViewableRegisterCell = Nothing
            Dim list As List(Of String) = Nothing
            Dim register As vwRegisterWeekCollection = New vwRegisterWeekCollection
            Dim aRegister As Register = Nothing
            Dim aEstablishment As Establishment = New Establishment
            Dim uniqueID As String
            Dim clientID As Integer = 0
            Dim cellID As String = String.Empty
            Dim clientsAdded As New ArrayList
            Dim last As Integer = 0
            Dim done As Boolean = False
            Dim totalActual As Label
            Dim registerID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("ID"))
            Dim totalPlanned As Label
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim hiddenRowCount As Integer = 0

            If registerGrp = "" Then
                msg = vwRegisterWeek.FetchList(Me.DbConnection, register, , , , , registerID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            Else
                msg = vwRegisterWeek.FetchList(Me.DbConnection, register, , , , , registerID, , , , , , , , registerGrp)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            If register.Count >= 1 Then
                OutputRegisterHeadersByDay(register(0).DomContractID, registerGrp)

                ClearViewState()
                list = GetUniqueIDsFromViewState()

                'get the starting client id
                clientID = register(0).ClientID
                last = register(0).ClientID

                For Each registerWeek As vwRegisterWeek In register
                    uniqueID = GetUniqueID(registerWeek)

                    If clientsAdded.Count >= 1 And clientsAdded.Count >= 2 Then
                        If clientsAdded(clientsAdded.Count - 1) = clientsAdded(clientsAdded.Count - 2) Then
                            done = True
                        End If
                    End If

                    If clientsAdded.Count = 1 _
                    And register(0).ClientID = register(register.Count - 1).ClientID Then
                        done = True
                    End If

                    If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                        If clientID = registerWeek.ClientID And clientID = last And Not done Then
                            row = New HtmlTableRow()
                            row.ID = uniqueID

                            phRegisterWeek.Controls.Add(row)

                            ' Service User Reference
                            cell = New HtmlTableCell()
                            row.Controls.Add(cell)
                            cell.Attributes.Add("class", "a")
                            serviceuserref = New Label()
                            With serviceuserref
                                .Width = New Unit(6, UnitType.Em)
                                If Not register Is Nothing Then .Text = registerWeek.ClientReference
                            End With
                            cell.Controls.Add(serviceuserref)

                            ' Service User Surname
                            cell = New HtmlTableCell()
                            row.Controls.Add(cell)
                            cell.Attributes.Add("class", "a")
                            surname = New Label()
                            With surname
                                .Width = New Unit(6, UnitType.Em)
                                If Not register Is Nothing Then .Text = registerWeek.LastName
                            End With
                            cell.Controls.Add(surname)

                            ' Service User FirstName
                            cell = New HtmlTableCell()
                            row.Controls.Add(cell)
                            cell.Attributes.Add("class", "a")
                            firstname = New Label()
                            With firstname
                                .Width = New Unit(10, UnitType.Em)
                                If Not register Is Nothing Then .Text = registerWeek.FirstNames
                            End With
                            cell.Controls.Add(firstname)

                            ' add rate category checkboxes for each rate category for the day chosen
                            Dim recordsAdded As Boolean = False
                            For Each registerDay As vwRegisterWeek In register
                                If registerDay.ClientID = clientID Then
                                    cell = New HtmlTableCell()
                                    row.Controls.Add(cell)
                                    cell.Attributes.Add("class", "a")
                                    hidOutcome = New HiddenField
                                    txtOutcome = New Label
                                    cbkRateCategory = New CheckBoxEx()
                                    With cbkRateCategory
                                        .Label.Visible = False
                                        If Not register Is Nothing Then
                                            cellID = registerDay.ID.ToString
                                            msg = DomContractBL.GetViewableRegisterCell(currentRegCell, cellID, day, regDays, regCells)
                                            If Not msg.Success Then WebUtils.DisplayError(msg)
                                            If currentRegCell.Attended And _
                                            currentRegCell.RegisterColumnID > 0 And _
                                            currentRegCell.Attended Then
                                                .CheckBox.Checked = GetCellDay(registerDay, day)
                                            End If
                                            If currentRegCell.Planned Then .CheckBox.BorderStyle = BorderStyle.Groove
                                            .ID = CTRL_PREFIX_RATE & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID
                                            hidOutcome.ID = CTRL_PREFIX_HOUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID & "_hidField"
                                            txtOutcome.ID = CTRL_PREFIX_OUTCOME & UNIQUEID_PREFIX_UPDATE_CONTROL & cellID
                                        End If
                                    End With
                                    With txtOutcome
                                        .Width = New Unit(3, UnitType.Em)
                                    End With
                                    If Not currentRegCell Is Nothing Then
                                        If ((currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned <> False) Or _
                                        (currentRegCell.DomRateCategoryID <> 0 And currentRegCell.Planned = False And _
                                         currentRegCell.Attended = True And currentRegCell.RegisterColumnID > 0)) Then
                                            msg = GetServiceOutcomeCode(txtOutcome, currentRegCell.ServiceOutcomeID, _serviceOutComes)
                                            hidOutcome.Value = currentRegCell.ServiceOutcomeID
                                        End If
                                    End If

                                    cell.Controls.Add(cbkRateCategory)
                                    cell.Controls.Add(hidOutcome)
                                    cell.Controls.Add(txtOutcome)
                                    cbkRateCategory.CheckBox.Attributes.Add( _
                                        "onclick", _
                                        String.Format("checkBox_Clicked('{0}_chkCheckbox','{1}');", cbkRateCategory.ClientID(), cellID) _
                                    )
                                    recordsAdded = True
                                Else
                                    If recordsAdded Then
                                        clientID = registerDay.ClientID
                                        Exit For
                                    End If
                                End If
                            Next
                            clientsAdded.Add(clientID)
                            last = clientsAdded(clientsAdded.Count() - 1)
                            list.Add(uniqueID)

                            ' total planned
                            cell = New HtmlTableCell()
                            row.Controls.Add(cell)
                            cell.Attributes.Add("class", "a")
                            totalPlanned = New Label()
                            With totalPlanned
                                .ID = CTRL_PREFIX_TOTAL_PLANNED & ConstructActualPlannedTextID(currentRegCell, uniqueID) 'uniqueID
                                .Width = New Unit(2, UnitType.Em)
                                If Not register Is Nothing Then .Text = registerWeek.TotalUnits
                            End With
                            GetTotals(totalPlanned, False, uniqueID, registerGrp, vwRegDays, day)
                            cell.Controls.Add(totalPlanned)

                            'total actual
                            cell = New HtmlTableCell()
                            row.Controls.Add(cell)
                            cell.Attributes.Add("class", "a")
                            totalActual = New Label()
                            With totalActual
                                .ID = CTRL_PREFIX_TOTAL_ACTUAL & ConstructActualPlannedTextID(currentRegCell, uniqueID) 'uniqueID
                                .Width = New Unit(2, UnitType.Em)
                                If Not register Is Nothing Then .Text = registerWeek.TotalUnits
                            End With
                            GetTotals(totalActual, True, uniqueID, registerGrp, vwRegDays, day)
                            cell.Controls.Add(totalActual)

                            'Set the colour to pale orange for actuals that exceed planned
                            Dim actualNumber, plannedNumber As Integer
                            actualNumber = totalActual.Text
                            plannedNumber = totalPlanned.Text

                            If actualNumber > plannedNumber Then
                                totalActual.BackColor = System.Drawing.Color.FromArgb(255, 221, 172)
                                cell.Style.Add("background-color", "#ffddac")
                            Else
                                totalActual.BackColor = System.Drawing.Color.FromArgb(255, 255, 255)
                                cell.Style.Add("background-color", "#FFFFFF")
                            End If

                             msg = CheckIfServiceOnSpecifiedDay(day, registerID, uniqueID, registerGrp, vwRegDays)
                            If msg.Success = False Then
                                row.Visible = False
                                hiddenRowCount += 1
                            End If
                            _domContractID = registerWeek.DomContractID
                            _domContractTitle = registerWeek.ContractTitle
                        End If
                    End If
                Next

                PersistUniqueIDsToViewState(list)

                Select Case day
                    Case DayOfWeek.Sunday
                        _registerDay = "Sunday"
                    Case DayOfWeek.Monday
                        _registerDay = "Monday"
                    Case DayOfWeek.Tuesday
                        _registerDay = "Tuesday"
                    Case DayOfWeek.Wednesday
                        _registerDay = "Wednesday"
                    Case DayOfWeek.Thursday
                        _registerDay = "Thursday"
                    Case DayOfWeek.Friday
                        _registerDay = "Friday"
                    Case DayOfWeek.Saturday
                        _registerDay = "Saturday"
                    Case Else
                End Select
            End If

            'enter blank rows for display purposes, when displaying 
            If phRegisterWeek.Controls.Count = hiddenRowCount Then
                row = New HtmlTableRow()
                phRegisterWeek.Controls.Add(row)
                cell = New HtmlTableCell()
                cell.InnerHtml = "<br>"
                row.Controls.Add(cell)
            End If

        End Sub

#End Region

#Region " ConstructActualPlannedTextID "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="currCell"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ConstructActualPlannedTextID(ByVal currCell As ViewableRegisterCell, ByVal uniqueID As String) As String
            Dim textID As String = Nothing
            Dim cellId As String
            Dim msg As ErrorMessage
            Dim cellView As ViewableRegisterCell = Nothing

            If currCell.Attended = False And currCell.ClientID = 0 And currCell.DayOfWeek = 0 And currCell.DomContractID = 0 _
                And currCell.DomRateCategoryID = 0 And currCell.ID = 0 And currCell.Planned = False And currCell.RegisterColumnID = 0 _
                And currCell.RegisterDayID = 0 And currCell.RegisterID = 0 And currCell.RegisterRowID = 0 And currCell.ServiceOutcomeID = 0 Then

                cellView = New ViewableRegisterCell
                cellId = uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_CONTROL, String.Empty)
                msg = DomContractBL.GetViewableRegisterCellByUniqueID(cellView, cellId, False)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                textID = "U_" & cellView.ClientID & "_" & cellView.DomContractID & "_0_" & cellView.RegisterID & "_" & cellView.RegisterRowID & "_0_0"
            Else
                textID = "U_" & currCell.ClientID & "_" & currCell.DomContractID & "_0_" & currCell.RegisterID & "_" & currCell.RegisterRowID & "_0_0"
            End If

            Return textID

        End Function
#End Region

#Region " GetCellDay "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="registerRow"></param>
        ''' <param name="day"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCellDay(ByVal registerRow As vwRegisterWeek, ByVal day As DayOfWeek) As Boolean
            Select Case day
                Case DayOfWeek.Sunday
                    Return registerRow.onSunday
                Case DayOfWeek.Monday
                    Return registerRow.onMonday
                Case DayOfWeek.Tuesday
                    Return registerRow.onTuesday
                Case DayOfWeek.Wednesday
                    Return registerRow.onWednesday
                Case DayOfWeek.Thursday
                    Return registerRow.onThursday
                Case DayOfWeek.Friday
                    Return registerRow.onFriday
                Case DayOfWeek.Saturday
                    Return registerRow.onSaturday
                Case Else
                    Return registerRow.onMonday
            End Select
        End Function
#End Region

#Region " CheckIfServiceOnSpecifiedDay "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="day"></param>
        ''' <param name="registerID"></param>
        ''' <param name="uniqueId"></param>
        ''' <param name="registerGrp"></param>
        ''' <param name="vwRegDays"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckIfServiceOnSpecifiedDay(ByVal day As DayOfWeek, ByVal registerID As Integer, _
                                                     ByVal uniqueId As String, ByVal registerGrp As String, _
                                                     ByVal vwRegDays As vwRegisterDayCollection) As ErrorMessage

            Dim cellId As String
            Dim msg As ErrorMessage
            Dim currentRegCell As ViewableRegisterCell = New ViewableRegisterCell
            Dim registerDayCollection As vwRegisterDayCollection = New vwRegisterDayCollection
            Dim total As Integer = 0
            Dim registerGroup As String = Nothing

            cellId = uniqueId.Replace(UNIQUEID_PREFIX_UPDATE_CONTROL, String.Empty)
            msg = DomContractBL.GetViewableRegisterCellByUniqueID(currentRegCell, cellId, False)
            If Not msg.Success Then Return msg

            If registerGrp <> "" Then
                registerGroup = registerGrp
            End If

            Select Case day
                Case DayOfWeek.Sunday
                    If registerGrp = "" Then
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnSunday = 1 Then total += 1
                        Next
                    Else
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnSunday = 1 And _
                               vwRegDay.RegisterGroup = registerGrp Then total += 1
                        Next
                    End If
                    If total = 0 Then
                        msg.Success = False
                        Return msg
                    Else
                        msg.Success = True
                        Return msg
                    End If
                Case DayOfWeek.Monday
                    If registerGrp = "" Then
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnMonday = 1 Then total += 1
                        Next
                    Else
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnMonday = 1 And _
                               vwRegDay.RegisterGroup = registerGrp Then total += 1
                        Next
                    End If
                    If total = 0 Then
                        msg.Success = False
                        Return msg
                    Else
                        msg.Success = True
                        Return msg
                    End If
                Case DayOfWeek.Tuesday
                    If registerGrp = "" Then
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnTuesday = 1 Then total += 1
                        Next
                    Else
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnTuesday = 1 And _
                               vwRegDay.RegisterGroup = registerGrp Then total += 1
                        Next
                    End If
                    If total = 0 Then
                        msg.Success = False
                        Return msg
                    Else
                        msg.Success = True
                        Return msg
                    End If
                Case DayOfWeek.Wednesday
                    If registerGrp = "" Then
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnWednesday = 1 Then total += 1
                        Next
                    Else
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnWednesday = 1 And _
                               vwRegDay.RegisterGroup = registerGrp Then total += 1
                        Next
                    End If
                    If total = 0 Then
                        msg.Success = False
                        Return msg
                    Else
                        msg.Success = True
                        Return msg
                    End If
                Case DayOfWeek.Thursday
                    If registerGrp = "" Then
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnThursday = 1 Then total += 1
                        Next
                    Else
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnThursday = 1 And _
                               vwRegDay.RegisterGroup = registerGrp Then total += 1
                        Next
                    End If
                    If total = 0 Then
                        msg.Success = False
                        Return msg
                    Else
                        msg.Success = True
                        Return msg
                    End If
                Case DayOfWeek.Friday
                    If registerGrp = "" Then
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnFriday = 1 Then total += 1
                        Next
                    Else
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnFriday = 1 And _
                               vwRegDay.RegisterGroup = registerGrp Then total += 1
                        Next
                    End If
                    If total = 0 Then
                        msg.Success = False
                        Return msg
                    Else
                        msg.Success = True
                        Return msg
                    End If
                Case DayOfWeek.Saturday
                    If registerGrp = "" Then
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnSaturday = 1 Then total += 1
                        Next
                    Else
                        For Each vwRegDay As vwRegisterDay In vwRegDays
                            If vwRegDay.ClientID = currentRegCell.ClientID And _
                               vwRegDay.OnSaturday = 1 And _
                               vwRegDay.RegisterGroup = registerGrp Then total += 1
                        Next
                    End If
                    If total = 0 Then
                        msg.Success = False
                        Return msg
                    Else
                        msg.Success = True
                        Return msg
                    End If
                Case Else
                    msg = New ErrorMessage
                    msg.Success = False
                    Return msg
            End Select
        End Function
#End Region

#Region " GetTotals "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="actual"></param>
        ''' <param name="uniqueID"></param>
        ''' <param name="registerGrp"></param>
        ''' <param name="day"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTotals(ByRef control As Label, ByVal actual As Boolean, ByVal uniqueID As String, _
                                  ByVal registerGrp As String, ByVal vwRegDays As vwRegisterDayCollection, _
                                  Optional ByVal day As Integer = RegisterView.Week) As ErrorMessage

            Dim cellId As String
            Dim msg As ErrorMessage
            Dim currentRegCell As ViewableRegisterCell = New ViewableRegisterCell
            Dim registerDayCollection As vwRegisterDayCollection = New vwRegisterDayCollection
            Dim total As Integer

            cellId = uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_CONTROL, String.Empty)
            msg = DomContractBL.GetViewableRegisterCellByUniqueID(currentRegCell, cellId, False)
            If Not msg.Success Then Return msg

            Select Case actual
                Case True 'actual
                    If day = RegisterView.Week Then
                        If registerGrp = "" Then

                            For Each vwRegDay As vwRegisterDay In vwRegDays
                                If vwRegDay.ClientID = currentRegCell.ClientID And _
                                   vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                   vwRegDay.DomRateCategoryID = currentRegCell.DomRateCategoryID And _
                                   vwRegDay.Unit = 1 And _
                                   vwRegDay.Attended = TriState.True Then total += 1
                            Next
                        Else
                            For Each vwRegDay As vwRegisterDay In vwRegDays
                                If vwRegDay.ClientID = currentRegCell.ClientID And _
                                   vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                   vwRegDay.DomRateCategoryID = currentRegCell.DomRateCategoryID And _
                                   vwRegDay.Unit = 1 And _
                                   vwRegDay.Attended = TriState.True And _
                                   vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                            Next
                        End If
                        control.Text = total.ToString
                        Return msg
                    Else
                        Select Case day
                            Case DayOfWeek.Sunday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnSunday = 1 Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnSunday = 1 And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Monday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnMonday = 1 Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnMonday = 1 And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Tuesday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnTuesday = 1 Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnTuesday = 1 And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Wednesday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnWednesday = 1 Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnWednesday = 1 And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Thursday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnThursday = 1 Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnThursday = 1 And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Friday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnFriday = 1 Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnFriday = 1 And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Saturday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnSaturday = 1 Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.ClientID = currentRegCell.ClientID And _
                                           vwRegDay.DomContractID = currentRegCell.DomContractID And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.OnSaturday = 1 And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                Return msg
                            Case Else
                                msg = New ErrorMessage
                                msg.Success = False
                                Return msg
                        End Select
                    End If
                Case False 'planned
                    If day = RegisterView.Week Then
                        If registerGrp = "" Then

                            For Each vwRegDay As vwRegisterDay In vwRegDays
                                If vwRegDay.ClientID = currentRegCell.ClientID And _
                                   vwRegDay.DomRateCategoryID = currentRegCell.DomRateCategoryID And _
                                   vwRegDay.Unit = 1 And _
                                   vwRegDay.Planned = TriState.True Then total += 1
                            Next
                        Else
                            For Each vwRegDay As vwRegisterDay In vwRegDays
                                If vwRegDay.ClientID = currentRegCell.ClientID And _
                                   vwRegDay.DomRateCategoryID = currentRegCell.DomRateCategoryID And _
                                   vwRegDay.Unit = 1 And _
                                   vwRegDay.Planned = TriState.True And _
                                   vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                            Next
                        End If
                        control.Text = total.ToString
                        Return msg
                    Else
                        Select Case day
                            Case DayOfWeek.Sunday
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.ClientID = currentRegCell.ClientID And _
                                       vwRegDay.OnSunday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Monday
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.ClientID = currentRegCell.ClientID And _
                                       vwRegDay.OnMonday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Tuesday
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.ClientID = currentRegCell.ClientID And _
                                       vwRegDay.OnTuesday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Wednesday
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.ClientID = currentRegCell.ClientID And _
                                       vwRegDay.OnWednesday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Thursday
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.ClientID = currentRegCell.ClientID And _
                                       vwRegDay.OnThursday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Friday
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.ClientID = currentRegCell.ClientID And _
                                       vwRegDay.OnFriday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                control.Text = total.ToString
                                Return msg
                            Case DayOfWeek.Saturday
                                For Each vwRegDay As vwRegisterDay In vwRegDays
                                    If vwRegDay.ClientID = currentRegCell.ClientID And _
                                       vwRegDay.OnSaturday = 1 And _
                                       vwRegDay.Unit = 1 And _
                                       vwRegDay.Planned = TriState.True Then total += 1
                                Next
                                control.Text = total.ToString
                                Return msg
                            Case Else
                                msg = New ErrorMessage
                                msg.Success = False
                                Return msg
                        End Select
                    End If
                Case Else
                    msg = New ErrorMessage
                    msg.Success = False
                    Return msg
            End Select

        End Function
#End Region

#Region " GetColumnTotals "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="registerID"></param>
        ''' <param name="actual"></param>
        ''' <param name="day"></param>
        ''' <param name="rateCategoryID"></param>
        ''' <param name="registerGrp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetColumnTotals(ByRef control As Label, ByVal registerID As String, ByVal actual As Boolean, _
                                        ByVal vwRegDays As vwRegisterDayCollection, Optional ByVal day As DayOfWeek = Nothing, _
                                        Optional ByVal rateCategoryID As String = Nothing, _
                                        Optional ByVal registerGrp As String = Nothing) As ErrorMessage

            Dim msg As ErrorMessage
            Dim registerDayCollection As vwRegisterDayCollection = New vwRegisterDayCollection
            Dim total As Integer = 0

            Select Case actual
                Case True 'actual
                    If rateCategoryID <> Nothing Then
                        Select Case day
                            Case DayOfWeek.Sunday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSunday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSunday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Monday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnMonday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnMonday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Tuesday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnTuesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnTuesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Wednesday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnWednesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnWednesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Thursday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnThursday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnThursday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Friday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnFriday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnFriday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Saturday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSaturday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSaturday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case Else
                                msg = New ErrorMessage
                                msg.Success = False
                                Return msg
                        End Select

                    Else
                        Select Case day
                            Case DayOfWeek.Sunday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSunday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSunday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Monday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnMonday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnMonday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Tuesday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnTuesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnTuesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Wednesday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnWednesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnWednesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Thursday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnThursday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnThursday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Friday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnFriday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnFriday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Saturday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSaturday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSaturday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Attended = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case Else
                                msg = New ErrorMessage
                                msg.Success = False
                                Return msg
                        End Select
                    End If
                Case False 'planned
                    If rateCategoryID <> Nothing Then
                        Select Case day
                            Case DayOfWeek.Sunday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSunday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSunday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Monday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnMonday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnMonday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Tuesday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnTuesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnTuesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Wednesday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnWednesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnWednesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Thursday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnThursday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnThursday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Friday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnFriday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnFriday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Saturday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSaturday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.DomRateCategoryID = rateCategoryID And _
                                           vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSaturday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case Else
                                msg = New ErrorMessage
                                msg.Success = False
                                Return msg
                        End Select

                    Else
                        Select Case day
                            Case DayOfWeek.Sunday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSunday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSunday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Monday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnMonday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnMonday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Tuesday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnTuesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnTuesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Wednesday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnWednesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnWednesday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Thursday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnThursday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnThursday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Friday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnFriday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnFriday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case DayOfWeek.Saturday
                                If registerGrp = "" Then
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSaturday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True Then total += 1
                                    Next
                                Else
                                    For Each vwRegDay As vwRegisterDay In vwRegDays
                                        If vwRegDay.RegisterID = registerID And _
                                           vwRegDay.OnSaturday = 1 And _
                                           vwRegDay.Unit = 1 And _
                                           vwRegDay.Planned = TriState.True And _
                                           vwRegDay.RegisterGroup.Contains(registerGrp) Then total += 1
                                    Next
                                End If
                                control.Text = total.ToString
                                msg = New ErrorMessage
                                msg.Success = True
                                Return msg
                            Case Else
                                msg = New ErrorMessage
                                msg.Success = False
                                Return msg
                        End Select
                    End If
                Case Else
                    msg = New ErrorMessage
                    msg.Success = False
                    Return msg
            End Select
        End Function
#End Region

#Region " GetServiceOutcomeCode "
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="serviceOutcomeID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetServiceOutcomeCode(ByRef control As Label, ByVal serviceOutcomeID As Integer, ByVal serviceOutcomes As ServiceOutcomeCollection) As ErrorMessage
            Dim msg As ErrorMessage
            Dim sOutcome As ServiceOutcome

            For Each sOutcome In serviceOutcomes
                If sOutcome.ID = serviceOutcomeID Then
                    control.Text = sOutcome.Code
                    Exit For
                End If
            Next

            msg = New ErrorMessage
            msg.Success = False
            Return msg
        End Function
#End Region

#Region " Remove_Click "

        Private Sub Remove_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim id As String = CType(sender, Button).ID.Replace(CTRL_PREFIX_REMOVED, String.Empty)

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE, UNIQUEID_PREFIX_DELETE)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phRegisterWeek.Controls.Count - 1
                If phRegisterWeek.Controls(index).ID = id Then
                    phRegisterWeek.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " GetUniqueID "

        Private Function GetUniqueID(ByVal registerWeek As vwRegisterWeek) As String

            Dim id As String

            If registerWeek.ID = "0" Then
                id = UNIQUEID_PREFIX_NEW & _newCodeIDCounter
                _newCodeIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE & "_" & registerWeek.ID
            End If

            Return id

        End Function

#End Region

#Region " GetUniqueIDsFromViewState "

        Private Function GetUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER) Is Nothing Then
                _newCodeIDCounter = 0
            Else
                _newCodeIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER), Integer)
            End If

            Return list

        End Function

#End Region

#Region " PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER, _newCodeIDCounter)
        End Sub

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phRegisterWeek.Controls.Clear()
        End Sub

        Private Sub ClearViewState()
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phRegisterWeek.Controls.Clear()
        End Sub
#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim msg As ErrorMessage

            Dim txtStatusID As String
            Dim weekEndingDate As String
            Dim weekEndDay As Date
            Dim weekEndDayNumber As Integer
            Dim sysInfoCollection As SystemInfoCollection = Nothing
            Dim preventEntryOfFutureDates As Boolean = False
            Dim maxWeekEnding As String = String.Empty

            msg = SystemInfo.FetchList(Me.DbConnection, sysInfoCollection)

            For Each sysInfo As SystemInfo In sysInfoCollection
                With sysInfo
                    weekEndDay = .DomServiceWEDate
                End With
            Next

            weekEndDayNumber = weekEndDay.DayOfWeek

            'get week ending date
            With _aRegister
                weekEndingDate = .WeekEnding
            End With

            If Not IsPostBack Then
                txtStatusID = CTRL_NAME_TXTSTATUS_PREFIX & txtStatus.ClientID & "_txtTextBox"
            Else
                txtStatusID = txtStatus.ClientID & "_txtTextBox"
            End If

            btnUncheck.Attributes.Add("onclick", String.Format("unCheckAllClicked('{0}','{1}','{2}','{3}');", ddView.SelectedValue, weekEndDayNumber, weekEndingDate, _domContractID))

            ' determine whether we should allow future periods
            msg = DomProviderInvoiceBL.ShouldPreventEntryOfActualServiceForFuturePeriods(DbConnection, _
                                                                                        preventEntryOfFutureDates)

            If msg.Success Then
                ' if we fetched the setting correctly

                If preventEntryOfFutureDates Then
                    ' if we should prevent future dates then set max week ending

                    maxWeekEnding = DomContractBL.GetWeekEndingDate(CType(Me.Page, BasePage).DbConnection, Nothing).AddDays(-7).ToString("dd/MM/yyyy")

                End If

            Else
                ' else we didn't fetch the setting correctly so throw an error

                WebUtils.DisplayError(msg)

            End If

            Dim js As String
            js = String.Format("registerID='{0}';cboServiceOutcomesID='{1}';statusID='{2}';weekEnding='{3}';weekEndDay='{4}';editClick='{5}';pageTitle='{6}';errorLabelID='{7}';maxWeekEnding='{8}';", _regID, cboServiceOutcomes.ClientID, txtStatusID, weekEndingDate, weekEndDayNumber, editClick, Me.PageTitle, lblError.ClientID, maxWeekEnding)

            Page.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Web.Apps.Actuals.DayCare.Edit.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js))
        End Sub

#End Region

    End Class

End Namespace