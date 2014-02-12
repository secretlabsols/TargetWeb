
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library.Collections
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Admin page used to maintain domiciliary rate frameworks.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' CD   21/11/2011  I378 - Changes to control manual payment categories in a more graceful manner.
    ''' CD   21/11/2011  I377 - Changed creation of manual payment rate category to use new bl function DomContractBL.SaveRateCategory.
    ''' MvO  26/03/2010  A4WA#6163 - use Me.Settings instead of ApplicationSetting.FetchList().
    ''' MvO  25/03/2010  A4WA#6166 - fixed calls to vwDomRateCategory.FetchList().
    ''' JAF  01/02/2010  Prevent disabled framework type throwing Save error (#6062)
    ''' MvO  15/12/2009  A4WA#5967 - enhanced rate days should only be available for "visit" framework type. 
    ''' JAF  12/11/2009  Prevent edit of Framework Type if Framework already
    '''                  has linked Rate Categories (#5882)
    ''' MoTahir 01/09/2009  D11675 - added concept of frameworktype
    ''' MvO  12/05/2009  D11549 - added reporting support.
    ''' MvO  23/04/2009  A4WA#5395 - When saving and framework is in use, also disabled enhanced day category validators.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' MvO  31/03/2008  Allow restricted edit when framework is in use.
    ''' </history>
    Partial Public Class RateFrameworks
        Inherits BasePage

        Const VIEWSTATE_KEY_DATA As String = "DataList"

        Private _dayCategories As DomDayCategoryCollection
        Private _dayCategoriesFiltered As DomDayCategoryCollection
        Private _enableViewCategoriesButton As Boolean = False
        Private _frameworkID As Integer
        Private _useEnhancedRateDays As Boolean = False
        Private _canEdit As Boolean
        Private _prefixType As Integer
        Private _autoGenContractNos As Boolean
        Private _prefixMinLen As Integer
        Private _prefixMaxLen As Integer
        Private _contractNoLength As Integer
        Private _details As DomRateFrameworkDetailCollection

#Region "Properties"

        ''' <summary>
        ''' Gets a value indicating whether this instance is solo i.e. cannot create new records or find existing records
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if this instance is solo; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property IsSolo() As Boolean
            Get
                Return (QsBackURL.Length > 0)
            End Get
        End Property

        ''' <summary>
        ''' Gets qs back url from the query string.
        ''' </summary>
        Private ReadOnly Property QsBackURL() As String
            Get
                Return Utils.ToString(Request.QueryString("backUrl")).Trim()
            End Get
        End Property

        ''' <summary>
        ''' Gets the standard buttons.
        ''' </summary>
        ''' <value>The standard buttons.</value>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return CType(stdButtons1, StdButtonsBase)
            End Get
        End Property

#End Region

#Region " Page_Load "

        Private Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            txtContractNoPrefix.UpperCase = True
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.RateFrameworks"), "Rate Frameworks")

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim appSetting As ApplicationSettingCollection = Nothing
            Dim qsId As Integer = Utils.ToInt32(Request.QueryString("id"))

            _contractNoLength = Me.Settings(ApplicationName.AbacusIntranet, DomContractBL.SETTING_CONTRACT_NO_LENGTH)
            _prefixType = Me.Settings(ApplicationName.AbacusIntranet, DomContractBL.SETTING_CONTRACT_PREFIX_TYPE)
            _autoGenContractNos = Convert.ToBoolean(Me.Settings(ApplicationName.AbacusIntranet, DomContractBL.SETTING_AUTO_GENERATE_CONTRACT_NOS))
            _prefixMinLen = Me.Settings(ApplicationName.AbacusIntranet, DomContractBL.SETTING_CONTRACT_PREFIX_MIN_LEN)
            _prefixMaxLen = Me.Settings(ApplicationName.AbacusIntranet, DomContractBL.SETTING_CONTRACT_PREFIX_MAX_LEN)

            With StandardButtonsControl
                .AllowBack = True
                .AllowNew = (Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RateFrameworks.AddNew")) AndAlso Not IsSolo)
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RateFrameworks.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.RateFrameworks.Delete"))
                .AllowFind = Not IsSolo
                If Not IsSolo Then
                    With .SearchBy
                        .Add("Description", "Description")
                    End With
                    .GenericFinderTypeID = GenericFinderType.DomRateFramework
                    .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.RateFrameworks")
                End If
                .EditableControls.Add(fsControls.Controls)
                .AuditLogTableNames.Add("DomRateFramework")
                .AuditLogTableNames.Add("DomRateFrameworkDetail")
                AddHandler .NewClicked, AddressOf NewClicked
                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf EditClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
                AddHandler .AfterModeChanged, AddressOf ModeChanged
            End With

            btnViewCategories.Visible = SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                                                   currentUser.ID, _
                                                                   Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.RateCategories"), _
                                                                   Me.Settings.CurrentApplicationID)

            Me.JsLinks.Add("RateFrameworks.js")

            cboFrameworkType.DropDownList.Attributes.Add("onchange", "cboFrameworkType_OnChange();")

            chkUseEnhancedRateDays.CheckBox.InputAttributes.Add("onclick", "chkUseEnhancedRateDays_Click();")
            _useEnhancedRateDays = chkUseEnhancedRateDays.CheckBox.Checked

            ' get non-redundant day categories
            msg = DomDayCategory.FetchList(Me.DbConnection, _dayCategories, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), TriState.UseDefault)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Not IsPostBack AndAlso qsId > 0 Then
                ' if first hit and id is in url

                With StandardButtonsControl
                    .SelectedItemID = qsId
                    .InitialMode = StdButtonsMode.Fetched
                End With
                FindClicked(New Target.Library.Web.UserControls.StdButtonEventArgs(False, qsId, StandardButtonsControl))

            Else

                ' re-create the list of details (from view state)
                Dim list As List(Of String) = GetUniqueIDsFromViewState()
                For Each id As String In list
                    Dim day As DayOfWeek = [Enum].Parse(GetType(DayOfWeek), id)
                    OutputDetailControls(day, 0, 0)
                Next

            End If

        End Sub

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phDetails.Controls.Clear()
            _frameworkID = 0
        End Sub

#End Region

#Region " NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            Dim list As List(Of String)
            ClearViewState(e)

            _useEnhancedRateDays = True

            list = GetUniqueIDsFromViewState()
            For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                ' add an id for each day
                list.Add(day.ToString())
                OutputDetailControls(day, 0, 0)
            Next
            ' persist the data into view state
            PersistUniqueIDsToViewState(list)
            PopulateFrameworkType()
            chkAddManualPrc.CheckBox.Checked = True

        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            ClearViewState(e)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                txtAbbreviation.Text = String.Empty
                txtContractNoPrefix.Text = String.Empty
                txtNextContractNo.Text = String.Empty
                chkUseEnhancedRateDays.CheckBox.Checked = False
                chkRedundant.CheckBox.Checked = False
                cboFrameworkType.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim rf As DomRateFramework
            _details = New DomRateFrameworkDetailCollection()
            Dim stdDetailDayCatID As Integer, enhDetailDayCatID As Integer
            Dim list As List(Of String)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()


            rf = New DomRateFramework(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With rf
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                txtAbbreviation.Text = .Abbreviation
                chkUseEnhancedRateDays.CheckBox.Checked = .UseEnhancedRateDays
                chkRedundant.CheckBox.Checked = .Redundant
                chkAddManualPrc.CheckBox.Checked = .AddManualPaymentRateCategory
                _useEnhancedRateDays = .UseEnhancedRateDays
                PopulateFrameworkType(.FrameworkTypeId)
                If _prefixType = 1 And _autoGenContractNos Then
                    txtContractNoPrefix.Text = .ContractNoPrefix
                    txtNextContractNo.Text = .NextContractNo.ToString.PadLeft(_contractNoLength - Len(.ContractNoPrefix), "0")
                End If
            End With

            ' get the details
            msg = DomRateFrameworkDetail.FetchList(Me.DbConnection, _details, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), rf.ID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' refresh the list of existing details and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()

            For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                If Not list.Contains(day.ToString()) Then
                    stdDetailDayCatID = 0
                    enhDetailDayCatID = 0
                    ' add an id for each day
                    list.Add(day.ToString())
                    ' locate the std and enh details for the current day
                    For Each detail As DomRateFrameworkDetail In _details
                        If detail.DayOfWeek = day Then
                            If detail.Enhanced Then
                                enhDetailDayCatID = detail.DomDayCategoryID
                                If stdDetailDayCatID > 0 Then Exit For
                            Else
                                stdDetailDayCatID = detail.DomDayCategoryID
                                If enhDetailDayCatID > 0 Then Exit For
                            End If
                        End If
                    Next
                    OutputDetailControls(day, stdDetailDayCatID, enhDetailDayCatID)
                End If
            Next

            PersistUniqueIDsToViewState(list)

            _frameworkID = e.ItemID

            ' print button
            With CType(ctlPrint, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.RateFramework")
                .Parameters.Add("intDomRateFrameworkID", _frameworkID)
                .Position = SearchableMenu.SearchableMenuPosition.TopLeft
            End With

        End Sub

#End Region

#Region " EditClicked "

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            msg = DomContractBL.CanEditRateFramework(Me.DbConnection, e.ItemID, _canEdit)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            FindClicked(e)

        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim backUrl As String = QsBackURL
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim details As DomRateFrameworkDetailCollection = Nothing
            Dim framework As DomRateFramework = Nothing

            Try

                Dim manualRateCategoryToDeleteID As Integer = 0

                trans = SqlHelper.GetTransaction(Me.DbConnection)

                ' get the framework to delete
                framework = New DomRateFramework(trans:=trans, auditLogTitle:=currentUser.ExternalUsername, auditUserName:=AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                msg = framework.Fetch(e.ItemID)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    SqlHelper.RollbackTransaction(trans)
                    FindClicked(e)
                    Exit Sub
                End If

                If framework.ManualPaymentRateCategoryID > 0 Then
                    ' if we have a manual rc then reset the id to 0

                    manualRateCategoryToDeleteID = framework.ManualPaymentRateCategoryID

                    ' set the rate category id to 0
                    With framework
                        .ManualPaymentRateCategoryID = 0
                        ' attempt to save the rate framework
                        msg = .Save()
                        If Not msg.Success Then
                            lblError.Text = msg.Message
                            e.Cancel = True
                            SqlHelper.RollbackTransaction(trans)
                            FindClicked(e)
                            Exit Sub
                        End If
                    End With

                End If

                ' delete all of the details first
                msg = DomRateFrameworkDetail.FetchList(trans, details, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                For Each detail As DomRateFrameworkDetail In details
                    msg = DomRateFrameworkDetail.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), detail.ID, e.ItemID)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        SqlHelper.RollbackTransaction(trans)
                        FindClicked(e)
                        Exit Sub
                    End If
                Next

                If manualRateCategoryToDeleteID > 0 Then
                    ' if we have a manual rate category to delete then do so

                    Dim manualRateCategoryToDelete As New DomRateCategory(trans:=trans, auditLogTitle:=currentUser.ExternalUsername, auditUserName:=AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                    ' fetch the manual rate category to delete
                    msg = manualRateCategoryToDelete.Fetch(manualRateCategoryToDeleteID)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        SqlHelper.RollbackTransaction(trans)
                        FindClicked(e)
                        Exit Sub
                    End If

                    ' delete the rate category
                    msg = DomContractBL.DeleteRateCategory(trans, manualRateCategoryToDelete)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                        SqlHelper.RollbackTransaction(trans)
                        FindClicked(e)
                        Exit Sub
                    End If

                End If

                ' delete the framework
                msg = DomRateFramework.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    SqlHelper.RollbackTransaction(trans)
                    FindClicked(e)
                    Exit Sub
                End If

                trans.Commit()

                ClearViewState(e)

                _useEnhancedRateDays = True

                msg.Success = True

            Catch ex As Exception

                msg = Utils.CatchError(ex, "E0503", "DomRateFramework/DomRateFrameworkDetail")      ' could not delete
                WebUtils.DisplayError(msg)

            Finally

                If Not msg.Success Then
                    ' rollback transaction if failed for any reason

                    SqlHelper.RollbackTransaction(trans)

                Else
                    ' else try and redirect if needs be

                    If backUrl.Length > 0 Then
                        ' if we have a back url then redirect to it

                        Dim builder As New Target.Library.Web.UriBuilder(backUrl)

                        ' remove the framework id from the qs and then redirect back
                        builder.QueryItems.Remove("frameworkid")
                        Response.Redirect(builder.ToString())

                    End If

                End If

            End Try

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim rf As DomRateFramework
            Dim details As DomRateFrameworkDetailCollection = New DomRateFrameworkDetailCollection
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim detail As DomRateFrameworkDetail
            Dim dayCategoryDropdown As DropDownListEx
            Dim useEnhancedRateDaysChanged As Boolean, abbreviationChanged As Boolean
            Dim category As DomRateCategory
            Dim rateCategories As vwDomRateCategoryCollection = Nothing
            Dim vwCat As vwDomRateCategory
            Dim inUseOnContract As Boolean = False
            Dim PrefixUsedOnAnotherFramework As Boolean = False

            If Me.IsPostBack Then
                PopulateFrameworkType()
                cboFrameworkType.SelectPostBackValue()
                cboFrameworkType.RequiredValidator.Enabled = False
            End If

            Try
                msg = DomContractBL.CanEditRateFramework(Me.DbConnection, e.ItemID, _canEdit)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' if we can only edit desc/abbrev/redundant then don't validate the day categories
                If Not _canEdit Then
                    For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                        dayCategoryDropdown = Me.Master.FindControl("MPContent").FindControl(GetDropdownID(day, False))
                        dayCategoryDropdown.RequiredValidator.Enabled = False
                        dayCategoryDropdown = Me.Master.FindControl("MPContent").FindControl(GetDropdownID(day, True))
                        dayCategoryDropdown.RequiredValidator.Enabled = False
                    Next
                Else
                    If chkUseEnhancedRateDays.CheckBox.Checked = False Then
                        For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                            dayCategoryDropdown = Me.Master.FindControl("MPContent").FindControl(GetDropdownID(day, True))
                            dayCategoryDropdown.RequiredValidator.Enabled = False
                        Next
                    End If
                End If

                Me.Validate()

                If Me.IsValid Then

                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    ' first load up the framework and the details for validation
                    rf = New DomRateFramework(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    If e.ItemID > 0 Then
                        ' update
                        With rf
                            msg = .Fetch(e.ItemID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End With
                    Else
                        'if this is a new framework, and we are auto generating contract nos from framework
                        If _prefixType = 1 And _autoGenContractNos Then
                            rf.NextContractNo = 1
                        End If
                    End If
                    With rf
                        If _canEdit Then
                            useEnhancedRateDaysChanged = (chkUseEnhancedRateDays.CheckBox.Checked <> .UseEnhancedRateDays)
                            .UseEnhancedRateDays = chkUseEnhancedRateDays.CheckBox.Checked
                            _useEnhancedRateDays = .UseEnhancedRateDays
                        End If
                        .Description = txtDescription.Text
                        abbreviationChanged = (.Abbreviation <> txtAbbreviation.Text)
                        .Abbreviation = txtAbbreviation.Text
                        .Redundant = chkRedundant.CheckBox.Checked
                        If Utils.ToString(cboFrameworkType.DropDownList.SelectedValue) <> "" Then
                            .FrameworkTypeId = cboFrameworkType.DropDownList.SelectedValue
                        End If

                        If rf.ID > 0 Then
                            'Check to see if the framework is already in use on one or more contracts
                            msg = DomContractBL.FrameworkInUseOnContract(trans, rf.ID, inUseOnContract)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End If
                        If Not inUseOnContract Then
                            .AddManualPaymentRateCategory = chkAddManualPrc.CheckBox.Checked
                        End If
                        'If its not in use on a contract store the entered value
                        If _prefixType = 1 And _autoGenContractNos Then
                            If Len(txtContractNoPrefix.Text) < _prefixMinLen Then
                                lblError.Text = String.Format("The Contract Number Prefix must be between {0} and {1} characters in length.", _prefixMinLen, _prefixMaxLen)
                                e.Cancel = True
                                msg.Success = False
                                Exit Sub
                            End If
                            If Len(txtContractNoPrefix.Text) > _prefixMaxLen Then
                                lblError.Text = String.Format("The Contract Number Prefix must be between {0} and {1} characters in length.", _prefixMinLen, _prefixMaxLen)
                                e.Cancel = True
                                msg.Success = False
                                Exit Sub
                            End If

                            msg = PrefixUnique(trans, rf.ID, txtContractNoPrefix.Text, PrefixUsedOnAnotherFramework)
                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            If PrefixUsedOnAnotherFramework Then
                                lblError.Text = "The Contract Number Prefix must be unique across all existing frameworks."
                                e.Cancel = True
                                msg.Success = False
                                Exit Sub
                            End If


                            .ContractNoPrefix = txtContractNoPrefix.Text
                        End If
                    End With
                    If e.ItemID > 0 Then
                        ' we are updating
                        ' fetch all existing details
                        msg = DomRateFrameworkDetail.FetchList(trans, details, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        If _canEdit Then
                            For Each detail In details
                                With detail
                                    If useEnhancedRateDaysChanged Then
                                        ' the flag has changed so delete all details and re-create below
                                        .DbTransaction = trans
                                        msg = .Delete(e.ItemID)
                                        If Not msg.Success Then WebUtils.DisplayError(msg)
                                    Else
                                        ' otherwise set the new day category
                                        dayCategoryDropdown = Me.Master.FindControl("MPContent").FindControl(GetDropdownID(.DayOfWeek, .Enhanced))
                                        .DomDayCategoryID = Utils.ToInt32(dayCategoryDropdown.GetPostBackValue())
                                    End If
                                End With
                            Next
                        End If
                    End If

                    If _canEdit AndAlso (e.ItemID = 0 Or useEnhancedRateDaysChanged) Then
                        details = New DomRateFrameworkDetailCollection()
                        ' creating new details
                        For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                            ' standard
                            detail = New DomRateFrameworkDetail(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                            With detail
                                .DayOfWeek = day
                                .Enhanced = TriState.False
                                dayCategoryDropdown = Me.Master.FindControl("MPContent").FindControl(GetDropdownID(.DayOfWeek, .Enhanced))
                                .DomDayCategoryID = Utils.ToInt32(dayCategoryDropdown.GetPostBackValue())
                            End With
                            ' add to the collection
                            details.Add(detail)

                            If rf.UseEnhancedRateDays Then
                                ' enhanced
                                detail = New DomRateFrameworkDetail(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                With detail
                                    .DayOfWeek = day
                                    .Enhanced = TriState.True
                                    dayCategoryDropdown = Me.Master.FindControl("MPContent").FindControl(GetDropdownID(.DayOfWeek, .Enhanced))
                                    .DomDayCategoryID = Utils.ToInt32(dayCategoryDropdown.GetPostBackValue())
                                End With
                                ' add to the collection
                                details.Add(detail)
                            End If
                        Next
                    End If

                    ' validate the framework
                    msg = DomContractBL.ValidateRateFramework(rf, details)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        e.Cancel = True
                    Else
                        ' save the header first
                        With rf
                            msg = .Save()
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            e.ItemID = .ID
                            _frameworkID = .ID
                        End With

                        If rf.ManualPaymentRateCategoryID <= 0 _
                            AndAlso rf.AddManualPaymentRateCategory = TriState.True Then
                            ' if we havent linked this framework to a manual payment rate category

                            Dim defaultDomService As DomService = Nothing

                            ' get the default dom service
                            msg = DomContractBL.GetDefaultManualPaymentDomiciliaryService(trans, defaultDomService)
                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            If defaultDomService Is Nothing OrElse defaultDomService.ID = 0 Then
                                ' if we havent determined a valid dom service then just display and error

                                lblError.Text = "The System Setting ‘Default Manual Payment Service’ must be set correctly before ‘Add Manual Payment Rate Category’ may be ticked.<br /><br />"
                                e.Cancel = True
                                Exit Sub

                            Else

                                Dim rcs As DomRateCategoryCollection = Nothing
                                Dim rc As DomRateCategory = Nothing
                                Dim uoms As DomUnitsOfMeasureCollection = Nothing
                                Const uomMoney As Byte = 2

                                ' get manual payment rate categories for this framework
                                msg = DomRateCategory.FetchList(trans:=trans, list:=rcs, allowUseWithManualPayments:=TriState.True, domRateFrameworkID:=rf.ID, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                                If Not msg.Success Then WebUtils.DisplayError(msg)

                                ' get money uoms
                                msg = DomUnitsOfMeasure.FetchList(trans:=trans, list:=uoms, systemType:=uomMoney, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                                If Not msg.Success Then WebUtils.DisplayError(msg)

                                If rcs.Count = 0 Then
                                    ' create a manual payment rate category

                                    rc = New DomRateCategory(trans:=trans, auditUserName:=currentUser.ExternalUsername, auditLogTitle:=AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                                    With rc
                                        .Description = "Manual Payment"
                                        .DomUnitsOfMeasureID = uoms(0).ID
                                        .DomRateFrameworkID = rf.ID
                                        .AllowUseWithManualPayments = TriState.True
                                        .AbbreviationSuffix = "MANUAL"
                                        .DomServiceID = defaultDomService.ID
                                        ' save the changes using the bl method
                                        msg = DomContractBL.SaveRateCategory(trans, rc)
                                        If Not msg.Success Then

                                            If msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_RATE_CATEGORY _
                                                OrElse msg.Number = DomContractBL.ERR_RATE_CATEGORY_ABBREV_INVALID Then
                                                ' if this is a known error then display on screen

                                                lblError.Text = msg.Message
                                                e.Cancel = True
                                                Exit Sub

                                            Else
                                                ' else this is an unknown error so throw a wobbly

                                                WebUtils.DisplayError(msg)

                                            End If
                                        End If
                                    End With

                                Else
                                    ' else use the first item in the list

                                    rc = rcs(0)

                                End If

                                With rf
                                    .ManualPaymentRateCategoryID = rc.ID
                                    ' save the changes
                                    msg = .Save()
                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                End With

                            End If

                        ElseIf rf.ManualPaymentRateCategoryID > 0 _
                            AndAlso rf.AddManualPaymentRateCategory = TriState.False Then
                            ' if there was a manual rate category and it is no longer required

                            Dim rc As New DomRateCategory(trans:=trans, auditUserName:=currentUser.ExternalUsername, auditLogTitle:=AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                            ' fetch back the rate category to delete
                            msg = rc.Fetch(rf.ManualPaymentRateCategoryID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)

                            ' unlink this rate framework
                            With rf
                                .ManualPaymentRateCategoryID = 0
                                ' save the changes
                                msg = .Save()
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End With

                            ' delete the rate category
                            msg = DomContractBL.DeleteRateCategory(trans, rc)
                            If Not msg.Success Then WebUtils.DisplayError(msg)

                        End If

                        If _canEdit Then
                            ' save/delete each of the details
                            For Each detail In details
                                detail.DbTransaction = trans
                                detail.DomRateFrameworkID = rf.ID
                                detail.AuditLogOverriddenParentID = rf.ID
                                msg = detail.Save()
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            Next
                        End If

                        ' if the abbreviation has changed, update the rate categories
                        If abbreviationChanged Then
                            ' get all non-redundant rate categories in this rate framework
                            msg = vwDomRateCategory.FetchList(trans:=trans, _
                                                              list:=rateCategories, _
                                                              domRateFrameworkID:=rf.ID, _
                                                              redundant:=TriState.False)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            ' update the constructed abbreviation
                            For Each vwCat In rateCategories
                                category = New DomRateCategory(trans, rf.AuditUserName, rf.AuditLogTitle)
                                With category
                                    msg = .Fetch(vwCat.ID)
                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                    msg = DomContractBL.ConstructRateCategoryAbbreviation(Nothing, trans, .DomRateFrameworkID, .DomServiceTypeID, .DomDayCategoryID, .DomTimeBandID, .ConstructedAbbreviation)
                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                    msg = .Save()
                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                End With
                            Next
                            ' get all non-redundant rate categories regardless of the rate framework
                            msg = vwDomRateCategory.FetchList(trans:=trans, _
                                                              list:=rateCategories, _
                                                              redundant:=TriState.False)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            ' validate the abbreviations
                            For Each vwCat In rateCategories
                                msg = DomContractBL.ValidateRateCategoryAbbreviation(Nothing, trans, vwCat)
                                If Not msg.Success Then
                                    e.Cancel = True
                                    lblError.Text = msg.Message
                                End If
                            Next
                        End If

                        trans.Commit()

                        msg.Success = True

                        If _prefixType = 1 And _autoGenContractNos Then
                            txtNextContractNo.Text = rf.NextContractNo.ToString.PadLeft(_contractNoLength - Len(rf.ContractNoPrefix), "0")
                        End If


                        ' re-find to sort out the chnages to the enhanced flag
                        If Not _canEdit OrElse useEnhancedRateDaysChanged Then FindClicked(e)

                    End If
                Else
                    e.Cancel = True
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0502", "DomRateFramework/DomRateFrameworkDetail")  ' could not save
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

#End Region

#Region " PopulateFrameworkType "

        ''' <summary>
        ''' Populates the framework type drop down list.
        ''' </summary>
        ''' <param name="frameworkTypeId">The framework type id.</param>
        Private Sub PopulateFrameworkType(Optional ByVal frameworkTypeId As Integer = 0)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim frameworkTypes As FrameworkTypeCollection = Nothing
            Dim frameworkTypesItem As ListItem = Nothing

            ' get the frameowrk types
            msg = FrameworkType.FetchList(Me.DbConnection, frameworkTypes, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), , TriState.False)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' setup drop down list as default i.e. only default blank item to select
            With cboFrameworkType.DropDownList
                .Items.Clear()
                .Items.Add(New ListItem(String.Empty))
            End With

            For Each ft As FrameworkType In frameworkTypes.ToArray().OrderBy(Function(tmpFt As FrameworkType) tmpFt.SortOrder)
                ' loop each item and add into the drop down list

                frameworkTypesItem = New ListItem(ft.Description, ft.ID)

                If frameworkTypeId <> 0 AndAlso frameworkTypeId = ft.ID Then
                    ' if the ids match then set as selected

                    frameworkTypesItem.Selected = True

                End If

                cboFrameworkType.DropDownList.Items.Add(frameworkTypesItem)

            Next

            If frameworkTypeId <> 0 AndAlso cboFrameworkType.DropDownList.SelectedItem.Value = String.Empty Then
                ' the id is specified but is not in the returned list so it must be redundant...fecth it back

                Dim ft As New FrameworkType(Me.DbConnection, String.Empty, String.Empty)

                ' fetch the item from db
                msg = ft.Fetch(frameworkTypeId)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' create the item and add into the drop down list as the selected item
                frameworkTypesItem = New ListItem(ft.Description, ft.ID)
                frameworkTypesItem.Selected = True
                cboFrameworkType.DropDownList.Items.Add(frameworkTypesItem)

            End If

        End Sub
#End Region

#Region " ModeChanged "

        Private Sub ModeChanged(ByVal e As StdButtonAfterModeEventArgs)
            If e.Mode = StdButtonsMode.Fetched Or e.Mode = StdButtonsMode.Edit Then
                _enableViewCategoriesButton = True
            Else
                _enableViewCategoriesButton = False
            End If
        End Sub

#End Region

#Region " OutputDetailControls "

        Private Sub OutputDetailControls(ByVal day As DayOfWeek, ByVal stdDomDayCategoryID As Integer, ByVal enhDomDayCategoryID As Integer)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim standard As DropDownListEx
            Dim enhanced As DropDownListEx

            row = New HtmlTableRow()
            phDetails.Controls.Add(row)

            ' day of week label
            cell = New HtmlTableCell()
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerText = day.ToString()

            ' standard
            cell = New HtmlTableCell()
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            standard = New DropDownListEx()
            With standard
                .ID = GetDropdownID(day, False)
                .Required = True
                .RequiredValidatorErrMsg = "* Required"
                .ValidationGroup = "Save"
                .Width = New Unit(15, UnitType.Em)
                LoadDayCategoryDropdown(standard, day, False)
                If stdDomDayCategoryID > 0 Then .DropDownList.SelectedValue = stdDomDayCategoryID
            End With
            cell.Controls.Add(standard)

            'If ShowEnhanced() Then
            ' enhanced
            cell = New HtmlTableCell()
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            enhanced = New DropDownListEx()
            With enhanced
                .ID = GetDropdownID(day, True)
                .Required = True
                .RequiredValidatorErrMsg = "* Required"
                .ValidationGroup = "Save"
                .Width = New Unit(15, UnitType.Em)
                LoadDayCategoryDropdown(enhanced, day, True)
                If enhDomDayCategoryID Then .DropDownList.SelectedValue = enhDomDayCategoryID
            End With
            cell.Controls.Add(enhanced)
            'End If

        End Sub

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

            Return list

        End Function

#End Region

#Region " PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA, list)
        End Sub

#End Region

#Region " GetDropdownID "

        Private Function GetDropdownID(ByVal day As DayOfWeek, ByVal enhanced As String) As String
            Return String.Format("{0}_{1}", day.ToString(), IIf(enhanced, "enh", "std"))
        End Function

#End Region

#Region " LoadDayCategoryDropdown "

        Private Sub LoadDayCategoryDropdown(ByVal dropdown As DropDownListEx, ByVal day As DayOfWeek, ByVal enhanced As Boolean)

            ' load the dropdown with categories that are applicable for the specified day of the week and std/enh
            _dayCategoriesFiltered = New DomDayCategoryCollection()

            For Each category As DomDayCategory In _dayCategories
                '' filter only when redundant and not assigned to current Rate Framework
                If (category.Redundant = TriState.False) Or (category.Redundant = TriState.True AndAlso Not _details Is Nothing AndAlso _details.ToArray().Where(Function(c) c.DomDayCategoryID = category.ID).Count() > 0) Then
                    _dayCategoriesFiltered.Add(category)
                End If
            Next

            Dim addCategory As Boolean

            With dropdown.DropDownList.Items
                .Clear()
                For Each category As DomDayCategory In _dayCategoriesFiltered
                    addCategory = False
                    If (Not enhanced AndAlso category.Standard) Or (enhanced AndAlso category.Enhanced) Then
                        Select Case day
                            Case DayOfWeek.Sunday
                                If category.Sunday Then addCategory = True
                            Case DayOfWeek.Monday
                                If category.Monday Then addCategory = True
                            Case DayOfWeek.Tuesday
                                If category.Tuesday Then addCategory = True
                            Case DayOfWeek.Wednesday
                                If category.Wednesday Then addCategory = True
                            Case DayOfWeek.Thursday
                                If category.Thursday Then addCategory = True
                            Case DayOfWeek.Friday
                                If category.Friday Then addCategory = True
                            Case DayOfWeek.Saturday
                                If category.Saturday Then addCategory = True
                        End Select
                    End If
                    If addCategory Then
                        .Add(New ListItem(category.Description, category.ID))
                    End If
                Next
                .Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region " Page_PreRenderComplete "

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim rateCategories As DomRateCategoryCollection = Nothing
            Dim msg As New ErrorMessage
            Dim inUseOnContract As Boolean = False

            ' set whether we should enable view categories button
            _enableViewCategoriesButton = StandardButtonsControl.ButtonsMode = StdButtonsMode.Fetched _
                                            OrElse StandardButtonsControl.ButtonsMode = StdButtonsMode.Edit

            btnViewCategories.Disabled = Not _enableViewCategoriesButton
            If _enableViewCategoriesButton Then
                btnViewCategories.Attributes.Add("onclick", String.Format("document.location.href='RateCategories/Lister.aspx?drfID={0}&currentStep=1';", _frameworkID))
            End If

            'Next Contract No is always disabled
            txtNextContractNo.Enabled = False

            With CType(stdButtons1, StdButtonsBase)
                If .ButtonsMode = StdButtonsMode.Edit Then
                    'Check to see if the framework is already in use on one or more contracts
                    msg = DomContractBL.FrameworkInUseOnContract(Me.DbConnection, _frameworkID, inUseOnContract)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    'if contracts are autogenerated and the prefix is determined from the framework
                    If _prefixType = 1 And _autoGenContractNos Then
                        'If its in use disable the textbox
                        If inUseOnContract Then
                            txtContractNoPrefix.Enabled = False
                        End If
                    Else
                        txtContractNoPrefix.Enabled = False
                    End If

                    If Not _canEdit Then
                        ' only enable desc, abbrev and redundant
                        .ToggleEditableFields(False)
                        txtDescription.Enabled = True
                        txtAbbreviation.Enabled = True
                        chkRedundant.CheckBox.Enabled = True
                        chkRedundant.Label.Enabled = True
                        chkAddManualPrc.CheckBox.Enabled = Not inUseOnContract
                        chkAddManualPrc.Label.Enabled = Not inUseOnContract
                    Else
                        '++ Disable the Framework Type dropdown if the parent
                        '++ Framework already has associated Rate Categories..
                        msg = DomRateCategory.FetchList(Me.DbConnection, rateCategories, String.Empty, String.Empty, , , _frameworkID)
                        If msg.Success Then
                            If rateCategories IsNot Nothing AndAlso rateCategories.Count > 0 Then
                                cboFrameworkType.Enabled = False
                            End If
                        End If
                    End If
                ElseIf .ButtonsMode = StdButtonsMode.AddNew Then
                    If _prefixType <> 1 And Not _autoGenContractNos Then
                        txtContractNoPrefix.Enabled = False
                    End If
                End If
            End With

            Me.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                  "Startup", _
                                                  String.Format("stdButtonsMode={0};", Convert.ToInt32(StandardButtonsControl.ButtonsMode)), _
                                                  True _
            )



        End Sub

#End Region

#Region " ShowEnhanced "

        Protected Function ShowEnhanced() As Boolean
            Return _useEnhancedRateDays
        End Function

#End Region

#Region " PrefixUnique "

        ''' <summary>
        ''' Determines if the specified prefix is already in use by one or more framework.
        ''' </summary>
        ''' <param name="trans">An already open database transaction.</param>
        ''' <param name="frameworkID">The ID of the framework to check.</param>
        ''' <param name="prefix">The prefix we are validating</param>
        ''' <param name="inUse">Upon success, indicates if the framework is in use on one or more contracts.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function PrefixUnique(ByVal trans As SqlTransaction, ByVal frameworkID As Integer, _
                                            ByVal prefix As String, ByRef inUse As Boolean) As ErrorMessage

            Const SP_NAME As String = "spxDomRateFramework_ContractPrefixUnique"

            Dim msg As ErrorMessage = Nothing
            Dim spParams As SqlParameter()

            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(trans.Connection, SP_NAME, False)
                spParams(0).Value = frameworkID
                spParams(1).Value = prefix
                spParams(2).Direction = ParameterDirection.InputOutput

                SqlHelper.ExecuteNonQuery(trans, CommandType.StoredProcedure, SP_NAME, spParams)

                inUse = Convert.ToBoolean(spParams(2).Value)

                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try

            Return msg

        End Function

#End Region


    End Class

End Namespace
