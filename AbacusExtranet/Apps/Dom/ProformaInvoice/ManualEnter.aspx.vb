
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.DomProviderInvoice

Namespace Apps.Dom.Proforma

    ''' <summary>
    ''' Screen to allow the manual entry of visits used to create a domiciliary proforma invoice.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD  14/05/2010  D11739 - included actual duration in visit details
    '''     ColinD  10/05/2010  D11746 - Prevent entry of future dates if configured in Intranet
    '''     MikeVO  30/04/2010  A4WA#6276 - correct swallowing of validation error when saving.
    '''     JohnF   15/12/2009  Ensure 'payment claimed' shown to 2dps (#5966)
    '''     MikeVO  16/01/2009  D11490 - catch more errors when saving pro forma invoice.
    '''     MikeVO  22/12/2008  ElecMon issue 133 - when copying use invoice ID, not batch ID.
    '''     MikeVO  01/12/2008  D11444 - security overhaul.
    '''     MikeVO  13/03/2008  Save DomProformaInvoice.InvoiceDate without time component.
    ''' </history>
    Partial Public Class ManualEnter
        Inherits BasePage

#Region " Consts "

        Const VIEWSTATE_KEY_DATA_VISITS As String = "VisitsDataList"
        Const VIEWSTATE_KEY_COUNTER_NEW_VISITS As String = "NewVisitCounter"

        Const CTRL_PREFIX_SVC_TYPE As String = "svcType"
        Const CTRL_PREFIX_VISIT_DOW As String = "visitDoW"
        Const CTRL_PREFIX_START_TIME As String = "startTime"
        Const CTRL_PREFIX_DURATION As String = "duration"
        Const CTRL_PREFIX_PRE_ROUNDED_DURATION As String = "preRoundedDuration"
        Const CTRL_PREFIX_ACTUAL_DURATION As String = "actualDuration"
        Const CTRL_PREFIX_CARERS As String = "carers"
        Const CTRL_PREFIX_SECONDARY_CARERS As String = "secondaryCarers"
        Const CTRL_PREFIX_VISIT_CODE As String = "visitCode"
        Const CTRL_PREFIX_REMOVED As String = "remove"

        Const UNIQUEID_PREFIX_NEW_VISITS As String = "visitN"
        Const UNIQUEID_PREFIX_UPDATE_VISITS As String = "visitU"
        Const UNIQUEID_PREFIX_DELETE_VISITS As String = "visitD"

#End Region

#Region " Private variables "

        Private _originalInvoiceID As Integer
        Private _providerID As Integer
        Private _contractID As Integer
        Private _clientID As Integer
        Private _newVisitIDCounter As Integer
        Private _serviceTypes As List(Of ViewablePair) = Nothing
        Private _client As ClientDetail
        Private _copyFromID As Integer
        Private _copyFromWE As Date
        Private _stdBut As StdButtonsBase
        Private _mode As String
        Private _batchId As Integer
        Private _dynamicJs As StringBuilder = New StringBuilder()
        Private _auditUserName As String
        Private _auditLogtitle As String
#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ManuallyEnteredVisits"), "Manually Entered Visits")
            'Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ManuallyEnteredVisits"), "Manual Domiciliary Pro forma Invoice")

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            _auditUserName = user.ExternalUsername
            _auditLogtitle = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)

            Dim visitList As List(Of String)
            Dim canDoNew As Boolean, canDoCopy As Boolean
            Dim preventEntryOfFutureDates As Boolean = False
            Dim msg As ErrorMessage

            _originalInvoiceID = Utils.ToInt32(Request.QueryString("id"))
            _providerID = Utils.ToInt32(Request.QueryString("estabID"))
            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _clientID = Utils.ToInt32(Request.QueryString("clientID"))
            _copyFromID = Utils.ToInt32(Request.QueryString("copyFromID"))
            If Utils.IsDate(Request.QueryString("copyFromWE")) Then
                _copyFromWE = Convert.ToDateTime(Request.QueryString("copyFromWE"))
            End If
            _mode = Utils.ToString(Request.QueryString("mode"))

            ' check security
            canDoNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.ManuallyEnteredVisits.AddNew"))
            canDoCopy = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.ManuallyEnteredVisits.Copy"))
            If Not (canDoNew Or canDoCopy) Then
                WebUtils.DisplayAccessDenied()
            End If
            If _copyFromID = 0 Then
                If Not canDoNew Then
                    WebUtils.DisplayAccessDenied()
                End If
            Else
                If Not canDoCopy Then
                    WebUtils.DisplayAccessDenied()
                End If
            End If

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .ShowNew = False
                .AllowBack = True
                .AllowFind = False
                .AllowDelete = IsInvoiceBatchManuallyEntered(_originalInvoiceID) 'True
                .AllowEdit = IsInvoiceBatchManuallyEntered(_originalInvoiceID)
                .EditableControls.Add(fsControls.Controls)
            End With

            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            Me.JsLinks.Add("ManualEnter.js")

            ' copy from?
            If _copyFromID > 0 Then
                LoadCopyFromData(Not Me.IsPostBack, _copyFromID, _copyFromWE)
            End If

            LoadReadonlyData()

            If Me.IsPostBack Then
                If _mode = "1" Then
                    Dim invoice As DomProformaInvoice = New DomProformaInvoice(Me.DbConnection, String.Empty, String.Empty)
                    msg = invoice.Fetch(_originalInvoiceID)
                    dteWeekEnding.Text = invoice.WETo
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                ' re-create the list of visits (from view state)
                visitList = GetVisitUniqueIDsFromViewState()
                For Each id As String In visitList
                    OutputVisitControls(id, Nothing)
                Next
            End If

            msg = DomProviderInvoiceBL.ShouldPreventEntryOfActualServiceForFuturePeriods(DbConnection, _
                                                                                        preventEntryOfFutureDates)

            If msg.Success Then
                ' if we have determined whether to prevent entry of future dates

                If preventEntryOfFutureDates Then

                    SetUpPreventEntryOfFutureDatesRangeValidator(rvWeekEnding, dteWeekEnding)

                Else

                    rvWeekEnding.Visible = False

                End If

            Else

                WebUtils.DisplayError(msg)

            End If

        End Sub

#End Region

#Region " LoadCopyFromData "

        Private Sub LoadCopyFromData(ByVal createControls As Boolean, ByVal invID As Integer, ByVal weekEnding As Date)

            Dim msg As ErrorMessage
            Dim batch As DomProformaInvoiceBatch
            Dim inv As DomProformaInvoice
            Dim invoiceVisits As DomProformaInvoiceVisitCollection = Nothing
            Dim uniqueID As String
            Dim visitList As List(Of String)

            ' get the invoice to copy
            inv = New DomProformaInvoice(Me.DbConnection, _auditUserName, _auditLogtitle)
            msg = inv.Fetch(invID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _clientID = inv.ClientID

            ' set batch id to use for delete method
            _batchId = inv.DomProformaInvoiceBatchID
            ' get the batch
            batch = New DomProformaInvoiceBatch(Me.DbConnection, _auditUserName, _auditLogtitle)
            With batch
                msg = .Fetch(inv.DomProformaInvoiceBatchID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _providerID = .ProviderID
                _contractID = .DomContractID
            End With


            If createControls Then
                dteWeekEnding.Text = weekEnding

                ' get the visits
                msg = DomProformaInvoiceVisit.FetchList(Me.DbConnection, _
                                                        invoiceVisits, _
                                                        _auditUserName, _
                                                        _auditLogtitle, _
                                                        inv.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' output the visits
                visitList = GetVisitUniqueIDsFromViewState()
                For Each iv As DomProformaInvoiceVisit In invoiceVisits
                    ' remove the ID so we fool the screen into thinking its a new visit
                    iv.ID = 0
                    uniqueID = GetVisitUniqueID(iv)
                    ' create the controls
                    OutputVisitControls(uniqueID, iv)
                    ' persist the data into view state
                    visitList.Add(uniqueID)
                Next
                ' save list into viewstate
                PersistVisitUniqueIDsToViewState(visitList)
            End If

        End Sub

#End Region

#Region " LoadReadonlyData "

        Private Sub LoadReadonlyData()

            Dim msg As ErrorMessage
            Dim provider As Establishment
            Dim contract As DomContract

            provider = New Establishment(Me.DbConnection)
            With provider
                msg = .Fetch(_providerID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtProvider.Text = String.Format("{0}: {1}", .AltReference, .Name)
            End With

            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            With contract
                msg = .Fetch(_contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtContract.Text = String.Format("{0}: {1}", .Number, .Title)
            End With

            _client = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
            With _client
                msg = .Fetch(_clientID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtClient.Text = String.Format("{0}: {1} {2}", .Reference, .FirstNames, .LastName)
            End With

        End Sub

#End Region

#Region " btnAddVisit_Click "

        Private Sub btnAddVisit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddVisit.Click

            If Utils.IsDate(dteWeekEnding.Text) Then
                lblError.Text = String.Empty
            Else
                lblError.Text = "Please enter a week ending date before adding any visits."
                Exit Sub
            End If

            Dim id As String
            Dim list As List(Of String) = GetVisitUniqueIDsFromViewState()
            Dim newVisit As DomProformaInvoiceVisit = New DomProformaInvoiceVisit(_auditUserName, _auditLogtitle)

            ' add a new row to the visit list
            id = GetVisitUniqueID(newVisit)
            ' create the controls
            OutputVisitControls(id, newVisit)
            ' persist the data into view state
            list.Add(id)
            PersistVisitUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " btnRemoveVisit_Click "

        Private Sub btnRemoveVisit_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetVisitUniqueIDsFromViewState()
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_PREFIX_REMOVED, String.Empty)

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW_VISITS) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE_VISITS, UNIQUEID_PREFIX_DELETE_VISITS)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phVisits.Controls.Count - 1
                If phVisits.Controls(index).ID = id Then
                    phVisits.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistVisitUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " OutputVisitControls "

        Private Sub OutputVisitControls(ByVal uniqueID As String, ByVal visit As DomProformaInvoiceVisit)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim cboServiceType As DropDownListEx
            Dim cboDoW As DropDownListEx
            Dim ctlStartTime As TimePicker
            Dim ctlDuration As TimePicker
            Dim ctlActualDuration As TimePicker
            Dim txtCarers As TextBoxEx
            Dim chkSecondaryCarers As CheckBoxEx
            Dim cboVisitCode As DropDownListEx
            Dim btnRemove As HtmlInputImage
            Dim preRounded As TextBox

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_VISITS) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                phVisits.Controls.Add(row)

                ' service type
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboServiceType = New DropDownListEx()
                With cboServiceType
                    .DropDownList.Attributes.Add("onchange", "valuechanged()")
                    .ID = CTRL_PREFIX_SVC_TYPE & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    LoadServiceTypeDropdown(cboServiceType)
                    If Not visit Is Nothing Then
                        .DropDownList.SelectedValue = visit.DomServiceTypeID
                    End If
                End With
                cell.Controls.Add(cboServiceType)

                ' DoW
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboDoW = New DropDownListEx()
                With cboDoW
                    .ID = CTRL_PREFIX_VISIT_DOW & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    LoadDayOfWeekDropdown(cboDoW)
                    If Not visit Is Nothing AndAlso Utils.IsDate(visit.VisitDate) Then
                        .DropDownList.SelectedValue = visit.VisitDate.DayOfWeek
                    End If
                    .DropDownList.Attributes.Add("onchange", String.Format("FetchVisitCodeList(""{0}"");valuechanged();", uniqueID))
                End With
                cell.Controls.Add(cboDoW)

                ' start time
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                ctlStartTime = New TimePicker()
                With ctlStartTime
                    .ID = CTRL_PREFIX_START_TIME & uniqueID
                    .ShowSeconds = False
                    If Not visit Is Nothing Then
                        .Hours = visit.StartTimeClaimed.Hour
                        .Minutes = visit.StartTimeClaimed.Minute
                    End If
                End With
                cell.Controls.Add(ctlStartTime)

                ' duration
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                ctlDuration = New TimePicker()
                With ctlDuration
                    .ID = CTRL_PREFIX_DURATION & uniqueID
                    .ShowSeconds = False
                    If Not visit Is Nothing Then
                        .Hours = visit.DurationClaimed.Hour
                        .Minutes = visit.DurationClaimed.Minute
                    End If
                End With
                cell.Controls.Add(ctlDuration)
                ' display prerounded values if any
                preRounded = New TextBox()
                With preRounded
                    .ID = CTRL_PREFIX_PRE_ROUNDED_DURATION & uniqueID
                    .CssClass = "borderless"
                    If Not visit Is Nothing Then
                        If visit.PreRoundedDurationClaimed.Minute <> 0 Or visit.PreRoundedDurationClaimed.Hour <> 0 Then
                            .Text = _
                            String.Format("[{0}:{1}]", visit.PreRoundedDurationClaimed.Hour, _
                                         visit.PreRoundedDurationClaimed.Minute)
                        End If
                    End If
                End With

                cell.Controls.Add(preRounded)

                ' actual duration
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                ctlActualDuration = New TimePicker()
                With ctlActualDuration
                    .ID = CTRL_PREFIX_ACTUAL_DURATION & uniqueID
                    .ShowSeconds = False
                    If Not visit Is Nothing Then
                        .Hours = visit.ActualDuration.Hour
                        .Minutes = visit.ActualDuration.Minute
                    End If
                End With
                cell.Controls.Add(ctlActualDuration)
                ctlDuration.OnChangeJavascript = String.Format("ctlDuration_Change(""{0}"");valuechanged();", uniqueID)
                ctlActualDuration.OnChangeJavascript = "valuechanged();"
                ' carers
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                txtCarers = New TextBoxEx()
                With txtCarers
                    .ID = CTRL_PREFIX_CARERS & uniqueID
                    .Width = New Unit(5, UnitType.Em)
                    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    If Not visit Is Nothing AndAlso visit.NumberOfCarers > 0 Then
                        .Text = visit.NumberOfCarers
                    Else
                        .Text = "1"
                    End If
                End With
                'txtCarers.TextBox.Attributes.Add()
                cell.Controls.Add(txtCarers)

                _dynamicJs.AppendFormat("function {0}_Changed(id) {{ valuechanged(); }};", txtCarers.ID)

                ' secondary carers
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                chkSecondaryCarers = New CheckBoxEx()
                With chkSecondaryCarers
                    .CheckBox.Attributes.Add("onClick", "return valuechanged();")
                    .ID = CTRL_PREFIX_SECONDARY_CARERS & uniqueID
                    If Not visit Is Nothing Then
                        .CheckBox.Checked = visit.SecondaryVisit
                    End If
                End With
                cell.Controls.Add(chkSecondaryCarers)

                ' visit code
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboVisitCode = New DropDownListEx()
                With cboVisitCode
                    .DropDownList.Attributes.Add("onchange", "valuechanged();")
                    .ID = CTRL_PREFIX_VISIT_CODE & uniqueID
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                End With
                cell.Controls.Add(cboVisitCode)
                LoadVisitCodeDropdown(cboVisitCode, uniqueID)
                If Not visit Is Nothing AndAlso visit.DomVisitCodeID > 0 Then
                    Dim item As ListItem = cboVisitCode.DropDownList.Items.FindByValue(visit.DomVisitCodeID)
                    If Not item Is Nothing Then cboVisitCode.DropDownList.SelectedValue = item.Value
                End If

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                btnRemove = New HtmlInputImage()
                With btnRemove
                    .ID = CTRL_PREFIX_REMOVED & uniqueID
                    .ValidationGroup = "RemoveVisit"
                    .Src = WebUtils.GetVirtualPath("Images/delete.png")
                    .Alt = "Remove this entry"
                    AddHandler .ServerClick, AddressOf btnRemoveVisit_Click
                    .Attributes.Add("onclick", "return btnRemoveVisit_Click();")
                End With
                cell.Controls.Add(btnRemove)

            End If

        End Sub

#End Region

#Region " LoadServiceTypeDropdown "

        Private Sub LoadServiceTypeDropdown(ByVal dropdown As DropDownListEx)

            Dim msg As ErrorMessage

            If _serviceTypes Is Nothing Then
                msg = DomContractBL.FetchServiceTypesAvailableToContract(Me.DbConnection, _contractID, _serviceTypes)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = _serviceTypes
                .DataTextField = "Text"
                .DataValueField = "Value"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region " LoadDayOfWeekDropdown "

        Private Sub LoadDayOfWeekDropdown(ByVal dropdown As DropDownListEx)

            With dropdown.DropDownList.Items
                .Clear()
                .Add(String.Empty)
                For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                    .Add(New ListItem([Enum].GetName(GetType(DayOfWeek), day), Convert.ToInt32(day)))
                Next
            End With

        End Sub

#End Region

#Region " LoadVisitCodeDropdown "

        Private Sub LoadVisitCodeDropdown(ByVal dropdown As DropDownListEx, ByVal uniqueID As String)

            Dim msg As ErrorMessage
            Dim visitDate As Date = dteWeekEnding.Text
            Dim postedBackDoW As String = CType(phVisits.FindControl(CTRL_PREFIX_VISIT_DOW & uniqueID), DropDownListEx).GetPostBackValue()
            Dim postedBackVisitCode As String = CType(phVisits.FindControl(CTRL_PREFIX_VISIT_CODE & uniqueID), DropDownListEx).GetPostBackValue()
            Dim dow As DayOfWeek
            Dim visitCodes As DomVisitCodeCollection = Nothing

            ' if we have a dow then we should use that to work back from the week ending date
            If Not postedBackDoW Is Nothing AndAlso postedBackDoW.Length > 0 Then
                dow = [Enum].Parse(GetType(DayOfWeek), postedBackDoW)
                While visitDate.DayOfWeek <> dow
                    visitDate = visitDate.AddDays(-1)
                End While
            End If

            msg = DomContractBL.FetchVisitCodesAvailableForVisit(Me.DbConnection, _contractID, visitDate, visitCodes)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' load the dropdown
            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = visitCodes
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)

                If Not postedBackVisitCode Is Nothing AndAlso postedBackVisitCode.Length > 0 Then
                    ' select posted back value
                    .SelectedValue = postedBackVisitCode
                Else
                    ' select default
                    For Each code As DomVisitCode In visitCodes
                        If code.DefaultCode Then
                            .SelectedValue = code.ID
                            Exit For
                        End If
                    Next
                End If
            End With

        End Sub

#End Region

#Region " GetVisitUniqueIDsFromViewState "

        Private Function GetVisitUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_VISITS) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_VISITS), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_VISITS) Is Nothing Then
                _newVisitIDCounter = 0
            Else
                _newVisitIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_NEW_VISITS), Integer)
            End If

            Return list

        End Function

#End Region

#Region " GetVisitUniqueID "

        Private Function GetVisitUniqueID(ByVal visit As DomProformaInvoiceVisit) As String

            Dim id As String

            If visit.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_VISITS & _newVisitIDCounter
                _newVisitIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_VISITS & visit.ID
            End If

            Return id

        End Function

#End Region

#Region " PersistVisitUniqueIDsToViewState "

        Private Sub PersistVisitUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_VISITS, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_NEW_VISITS, _newVisitIDCounter)
        End Sub

#End Region

#Region " ManualEnter_PreRenderComplete "

        Private Sub ManualEnter_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
                String.Format("CTRL_PREFIX_VISIT_DOW=""{0}"";CTRL_PREFIX_VISIT_CODE=""{1}"";contractID={2};CTRL_PREFIX_DURATION=""{3}"";CTRL_PREFIX_ACTUAL_DURATION=""{4}"";CTRL_PREFIX_PRE_ROUNDED_DURATION=""{5}""; OriginalValueChanged=""{6}""; selectedInvoiceBatchID={7};", CTRL_PREFIX_VISIT_DOW, CTRL_PREFIX_VISIT_CODE, _contractID, CTRL_PREFIX_DURATION, CTRL_PREFIX_ACTUAL_DURATION, CTRL_PREFIX_PRE_ROUNDED_DURATION, OriginalValueChanged.ClientID, _batchId), _
                True)

            ClientScript.RegisterStartupScript(Me.GetType(), "JsChange", _dynamicJs.ToString(), True)

        End Sub

#End Region

#Region " Std Buttons Events "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim invoice As DomProformaInvoice = New DomProformaInvoice(Me.DbConnection, String.Empty, String.Empty)
            Dim msg As ErrorMessage

            msg = invoice.Fetch(_originalInvoiceID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With invoice
                txtReference.Text = .OurReference
                txtPaymentClaimed.Text = .PaymentClaimed.ToString("0.00")
            End With

            LoadCopyFromData(Not Me.IsPostBack, e.ItemID, invoice.WETo)
            LoadReadonlyData()
        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            'LoadReadonlyData()
        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim batch As DomProformaInvoiceBatch = New DomProformaInvoiceBatch(Me.DbConnection, _auditUserName, _auditLogtitle)
            Dim invoice As DomProformaInvoice = New DomProformaInvoice(Me.DbConnection, _auditUserName, _auditLogtitle)
            Dim visit As DomProformaInvoiceVisit
            Dim visits As List(Of DomProformaInvoiceVisitEx) = Nothing
            Dim visitList As List(Of String), visitToDelete As List(Of String)
            Dim time As TimePicker
            Dim dow As DayOfWeek
            Dim visitDate As Date
            Dim indicator As DomManuallyAmendedIndicator = Nothing

            If Me.IsValid Then

                ' check week ending date
                msg = DomContractBL.ValidateWeekEndingDate(Me.DbConnection, dteWeekEnding.Text)
                If Not msg.Success Then
                    If msg.Number = DomContractBL.ERR_INVALID_WEEK_ENDING Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        Exit Sub
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                End If

                If _stdBut.ButtonsMode = StdButtonsMode.AddNew Then
                    ' create the batch
                    With batch
                        .ProviderID = _providerID
                        .DomContractID = _contractID
                        .UserID = currentUser.ExternalUserID
                        .DateCreated = DateTime.Now
                        .CreatedBy = String.Format("{0} {1}", currentUser.FirstName, currentUser.Surname)
                        If .CreatedBy.Length > 50 Then .CreatedBy = .CreatedBy.Substring(0, 50)
                        .DomProformaInvoiceBatchTypeID = DomProformaInvoiceBatchType.ManuallyEntered
                        .VisitBasedReturn = TriState.True
                        .DomProformaInvoiceBatchStatusID = DomProformaInvoiceBatchStatus.AwaitingVerification
                        .StatusDate = .DateCreated
                        .StatusChangedBy = .CreatedBy
                    End With

                ElseIf _stdBut.ButtonsMode = StdButtonsMode.Edit Then
                    'Fetch the Original Invoice, as we need to use the batch id off it
                    'to find the batch.
                    msg = invoice.Fetch(_originalInvoiceID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    'Fetch the existing Batch
                    msg = batch.Fetch(invoice.DomProformaInvoiceBatchID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                With invoice
                    .ClientID = _clientID
                    .ServiceUserDetails = String.Format("{0}: {1} {2}", _client.Reference, _client.FirstNames, _client.LastName)
                    .WETo = dteWeekEnding.Text
                    .OurReference = txtReference.Text
                    .ServiceUserContribution = 0
                    Decimal.TryParse(txtPaymentClaimed.Text, .PaymentClaimed)
                    .Query = TriState.False
                    .InvoiceDate = DateTime.Today
                End With

                ' create the visits
                visits = New List(Of DomProformaInvoiceVisitEx)
                visitToDelete = New List(Of String)
                visitList = GetVisitUniqueIDsFromViewState()
                For Each uniqueID As String In visitList
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_VISITS) Then
                        ' we are deleting
                        visitToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE_VISITS, String.Empty))
                    Else
                        ' create the empty visit record
                        visit = New DomProformaInvoiceVisitEx(_auditUserName, _auditLogtitle)
                        visit.DbConnection = Me.DbConnection
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE_VISITS) Then
                            ' we are updating
                            msg = visit.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE_VISITS, String.Empty)))
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End If
                        ' set the visit properties
                        With visit
                            .DomServiceTypeID = Utils.ToInt32(CType(phVisits.FindControl(CTRL_PREFIX_SVC_TYPE & uniqueID), DropDownListEx).GetPostBackValue())

                            ' start with the w/e date and wind back to the specified dow
                            visitDate = invoice.WETo
                            dow = [Enum].Parse(GetType(DayOfWeek), CType(phVisits.FindControl(CTRL_PREFIX_VISIT_DOW & uniqueID), DropDownListEx).GetPostBackValue())
                            While visitDate.DayOfWeek <> dow
                                visitDate = visitDate.AddDays(-1)
                            End While
                            .VisitDate = visitDate

                            time = CType(phVisits.FindControl(CTRL_PREFIX_START_TIME & uniqueID), TimePicker)
                            .StartTimeClaimed = DateTime.Parse(time.ToString(DomContractBL.TIME_ONLY_DATE))
                            time = CType(phVisits.FindControl(CTRL_PREFIX_DURATION & uniqueID), TimePicker)
                            .DurationClaimed = DateTime.Parse(time.ToString(DomContractBL.TIME_ONLY_DATE))
                            .NumberOfCarers = Convert.ToInt32(CType(phVisits.FindControl(CTRL_PREFIX_CARERS & uniqueID), TextBoxEx).Text)
                            .SecondaryVisit = CType(phVisits.FindControl(CTRL_PREFIX_SECONDARY_CARERS & uniqueID), CheckBoxEx).CheckBox.Checked
                            .DomVisitCodeID = Utils.ToInt32(CType(phVisits.FindControl(CTRL_PREFIX_VISIT_CODE & uniqueID), DropDownListEx).GetPostBackValue())
                            .ActualStartTime = .StartTimeClaimed
                            time = CType(phVisits.FindControl(CTRL_PREFIX_ACTUAL_DURATION & uniqueID), TimePicker)
                            .ActualDuration = DateTime.Parse(time.ToString(DomContractBL.TIME_ONLY_DATE))
                            msg = DomContractBL.GetManuallyAmendedIndicatorForManualVisit(Me.DbConnection, _contractID, .VisitDate, indicator)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            .DomManuallyAmendedIndicatorID = indicator.ID

                        End With
                        ' add to the collection
                        visits.Add(visit)
                    End If
                Next

                ' save the invoice
                msg = DomContractBL.SaveProformaInvoice(Me.DbConnection, currentUser.ExternalUsername, _
                    AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), batch, invoice, visits, Nothing, Me.Settings.CurrentApplicationID)
                If Not msg.Success Then
                    If msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_PROFORMA_INVOICE Or _
                            msg.Number = DomContractConvertTimeToUnits.ERR_COULD_NOT_CATEGORISE Or _
                            msg.Number = DomContractConvertTimeToUnits.ERR_COULD_NOT_DETERMINE_COSTS Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                    Else
                        WebUtils.DisplayError(msg)
                    End If
                Else
                    'Response.Redirect(HttpUtility.UrlDecode(Request.QueryString("backUrl")))
                End If
            Else
                e.Cancel = True
            End If
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            Dim url As String = Request.Url.GetLeftPart(UriPartial.Path)
            Dim backurl As String = Request.QueryString("backUrl")
            Dim queryBuilder As StringBuilder = New StringBuilder()
            For Each key As String In Request.QueryString.AllKeys
                If key <> "backUrl" Then
                    queryBuilder.AppendFormat("&{0}={1}", key, Request.QueryString(key))
                End If
            Next
            queryBuilder.AppendFormat("&backurl={0}", HttpUtility.UrlEncode(backurl))
            url = url & "?=null" & queryBuilder.ToString()
            Response.Redirect(url)
            'Response.Redirect(HttpUtility.UrlDecode(Request.QueryString("backUrl")))
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)
            Dim user As WebSecurityUser
            user = SecurityBL.GetCurrentUser()

            Dim inv As DomProformaInvoice
            Dim invoiceVisits As DomProformaInvoiceVisitCollection = Nothing
            Dim msg As ErrorMessage
            msg = New ErrorMessage()
            ' get the invoice to copy
            inv = New DomProformaInvoice( _
            Me.DbConnection, _
            user.ExternalUsername, _
            AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings) _
            )
            msg = inv.Fetch(_originalInvoiceID)

            DomContractBL.ChangeProformaInvoiceBatchStatus(Me.DbConnection, _
                                                           inv.DomProformaInvoiceBatchID, _
                                                           DomProformaInvoiceBatchStatus.Deleted, _
                                                           user.ExternalUsername, _
                                                           "DomContractWebSvc.ChangeProformaInvoiceBatchStatus()" _
                                                           )
            Response.Redirect(HttpUtility.UrlDecode(Request.QueryString("backUrl")))
        End Sub
#End Region

#Region "SetUpPreventEntryOfFutureDatesRangeValidator"

        ''' <summary>
        ''' Sets up prevent entry of future dates range validator.
        ''' </summary>
        ''' <param name="validator">The validator.</param>
        ''' <param name="controlToValidate">The control to validate.</param>
        Private Sub SetUpPreventEntryOfFutureDatesRangeValidator(ByVal validator As RangeValidator, _
                                                                 ByVal controlToValidate As TextBoxEx)

            With validator
                .MinimumValue = DateTime.MinValue.ToString("dd/MM/yyyy")
                .MaximumValue = DateTime.Now.ToString("dd/MM/yyyy")
                .ControlToValidate = String.Format("{0}${1}", controlToValidate.ID, controlToValidate.TextBox.ID)
                .Type = ValidationDataType.Date
                .Visible = True
            End With

        End Sub

#End Region

#Region " Check Invoice batch Type "
        Private Function IsInvoiceBatchManuallyEntered(ByVal invoiceId As Integer) As Boolean
            Dim invoices As vwDomProformaInvoiceCollection = Nothing
            Dim domProformaInvoiceBatchTypeId As Integer = 2 ' for mannually entered batch
            Dim domProformaInvoiceBatchStatusId As Integer = 1 ' for Awaiting verification
            vwDomProformaInvoice.FetchList(Me.DbConnection, invoices, invoiceId, domProformaInvoiceBatchTypeId, domProformaInvoiceBatchStatusId)
            If invoices.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End Function
#End Region



    End Class

End Namespace