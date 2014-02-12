
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Controls
Imports Target.Library.Web.Extensions.AjaxControlToolkit
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Text
Imports Target.Abacus.Web.Apps.InPlaceSelectors

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Admin page used to maintain domiciliary service types.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir  31/07/2013  A8072 - Unable to save a Service Type with a linked Service that is not visit based as unit of measure is not set and it is 
    '''                      not possible to select a unit of measure for the Domiciliary Service.
    ''' MoTahir  07/11/2012  D12343 - Remove Framework Type from Service Group.
    ''' MoTahir  23/02/2012  A4WA#7220 - When attempting to save a new service type, and error is encountered.
    ''' MoTahir  05/12/2011  BTI448 - Attempting to change the One unit per order indicator results in a trappable error under certain circumstances  
    ''' ColinD   07/10/2011  I66 - Changes to set conversion factor to Automatic for visit based items.
    ''' ColinD   09/08/2011  D11965 - Chnages to handle the removal of DomServiceType.VisitBased.
    ''' ColinD   08/12/2010  D11964A - Changes to Linked Services tab to add in place selectors for DomService and BudgetCategory, other small ui changes.
    ''' ColinD   22/11/2010  D11924C - Changes to remove 'Residential' linked services
    ''' ColinD   22/10/2010  D11924A - Changes to account for addition of Permanent field on DomServiceType table
    ''' ColinD   11/10/2010  D11918 - numerous changes for addition of service category and client side enhancements
    ''' MikeVO   01/09/2010  Added validation summary.
    ''' Mo Tahir 29/09/2010 Issue 59 SDS Sharepoint - Various issues with screen
    ''' Mo Tahir 27/09/2010 Issue 11 SDS Sharepoint - Finance code selectors validation firing when trying to remove linked services.
    ''' Mo Tahir 22/07/2010 D11877 - Res Care to Service group mapping
    ''' MikeVO  27/01/2010  D11649 - added FinanceCode fields.
    ''' MikeVO  15/12/2009  A4WA#5967 - fix to RemoveAssociationDomService to correctly filter on DomServiceType.
    ''' MoTahir 23/09/2009  A5797
    ''' MoTahir 24/08/2009  D11671 - added service groups
    ''' MikeVO  12/05/2009  D11549 - added reporting support.
    ''' MikeVO  05/01/2009  D11468 - added "one unit per order" support.
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MikeVO  18/08/2008  D11398 - Added VisitBased indicator.
    ''' </history>
    Partial Public Class ServiceTypes
        Inherits BasePage

        Const VIEWSTATE_KEY_DATA_SERVICE_TYPE As String = "NonResDataList"
        Const VIEWSTATE_KEY_COUNTER_SERVICE_TYPE As String = "NonResDataListCounter"
        Const ADD_LINKED_SERVICES_ERROR_MSG As String = "Please save the record entering service group, and description on the details tab, before adding linked services"

        Const CTRL_PREFIX_SERVICE As String = "service"
        Const CTRL_PREFIX_DUM As String = "dum"
        Const CTRL_PREFIX_DUM_HDN As String = "dumHdn"
        Const CTRL_PREFIX_REMOVED As String = "remove"
        Const CTRL_PREFIX_BUDGET_CATEGORY As String = "bc"
        Const CTRL_PREFIX_BUDGET_CATEGORY_NUMERATOR As String = "bcNum"
        Const CTRL_PREFIX_BUDGET_CATEGORY_DENOMINATOR As String = "bcDen"

        Const CTRL_PREFIX_ADMISSION As String = "admission"
        Const CTRL_PREFIX_ACCOMODATION As String = "accomodation"

        Const ROW_PREFIX As String = "row"

        Const UNIQUEID_PREFIX_NEW As String = "N"
        Const UNIQUEID_PREFIX_UPDATE As String = "U"
        Const UNIQUEID_PREFIX_DELETE As String = "D"

        Private _stdBut As StdButtonsBase
        Private _inUseByContractVisitServiceType As Boolean
        Private _newServiceIDCounter As Integer
        Private _services As DomServiceCollection = Nothing
        Private _dums As DomUnitsOfMeasureCollection = Nothing
        Private _admissionStatuses As vwAdmissionStatusCollection = Nothing
        Private _accommodationTypes As vwAccommodationTypeCollection = Nothing
        Private _serviceTypeID As Integer = 0
        Private _serviceGroupID As Integer = 0
        Private _careType As Integer = 0
        Private _type As DomServiceType
        Private _deleteFindID As Integer = 0
        Private _inEditMode As Boolean = False
        Private _isDeleted As Boolean = False

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim msg As New ErrorMessage()
            Dim js As StringBuilder = New StringBuilder()
            Dim css As StringBuilder = New StringBuilder()
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceTypes"), "Service Types")
            Me.ShowValidationSummary = True

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceTypes.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceTypes.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceTypes.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                    .Add("Abbreviation", "Abbreviation")
                End With
                .EditableControls.Add(tabDetails.Controls)
                .EditableControls.Add(tabLinkedServices.Controls)
                .GenericFinderTypeID = GenericFinderType.DomServiceType
                .AuditLogTableNames.Add("DomServiceType")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ServiceTypes")
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            ' disable validation on finance code selectors
            With CType(txtFinanceCode, Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceFinanceCodeSelector)
                .Required = False
            End With

            With CType(txtFinanceCode2, Target.Abacus.Web.Apps.InPlaceSelectors.InPlaceFinanceCodeSelector)
                .Required = False
            End With

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add("ServiceTypes.js")

            If _stdBut.SelectedItemID > 0 Then
                ' if we have an id

                Dim type As New DomServiceType(Me.DbConnection, SecurityBL.GetCurrentUser().ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                Dim servGroup As New ServiceGroup(conn:=Me.DbConnection, auditUserName:=String.Empty, AuditLogTitle:=String.Empty)

                ' fetch the service type
                msg = type.Fetch(_stdBut.SelectedItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' fetch the service group
                msg = servGroup.Fetch(type.ServiceGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' set _isVisitBased indicator

            End If

            ' re-create the list of Services (from view state)
            Dim list As List(Of String) = GetUniqueIDsFromViewState(VIEWSTATE_KEY_DATA_SERVICE_TYPE, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)
            For Each id As String In list
                OutputNonResidentialControls(id, Nothing, js)
            Next

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)

            With css
                .Append("td.domService {background-color:#ffddac;} ")
                .Append("td.budgetCategories {background-color:#fffea6;} ")
                .Append("td.centre {align:center;} ")
                Me.AddExtraCssStyle(.ToString())
            End With

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage

            ClearViewState(e, VIEWSTATE_KEY_DATA_SERVICE_TYPE)
            ClearViewState(e, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)

            msg = populateConvertServiceTypeDropDown()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            txtDescription.Text = String.Empty
            txtAbbreviation.Text = String.Empty

            CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText = String.Empty
            CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText = String.Empty
            chkRedundant.CheckBox.Checked = False
            chkDoubleNoCarers.CheckBox.Checked = False
            chkHalveDuration.CheckBox.Checked = False

            phLinkedServices.Controls.Clear()
            CType(txtServiceGroup, InPlaceServiceGroupSelector).ServiceGroupText = String.Empty
            _careType = 0
            hidServiceGroup.Value = String.Empty
            tabLinkedServices.Visible = False
        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            _inEditMode = True
            FindClicked(e)
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim svcList As DomServiceCollection = New DomServiceCollection
            Dim dumList As DomUnitsOfMeasureCollection = New DomUnitsOfMeasureCollection
            Dim list As List(Of String)
            Dim js As StringBuilder
            Dim servGroup As ServiceGroup
            Dim servGroupClass As ServiceGroupClassification

            msg = populateConvertServiceTypeDropDown()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _type = New DomServiceType(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            With _type
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _serviceTypeID = e.ItemID
                CType(txtServiceGroup, InPlaceServiceGroupSelector).ServiceGroupID = .ServiceGroupID
                txtDescription.Text = .Description
                txtAbbreviation.Text = .Abbreviation
                CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode1
                CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText = .FinanceCode2
                chkRedundant.CheckBox.Checked = .Redundant

                chkDoubleNoCarers.CheckBox.Checked = .DoubleNoOfCarers
                chkHalveDuration.CheckBox.Checked = .HalveDuration

                cboConvertToServiceType.DropDownList.SelectedValue = .ConvertToServiceTypeID
            End With

            If CheckServiceTypeInUse(e.ItemID) Then
                hidServiceGroup.Value = CType(txtServiceGroup, InPlaceServiceGroupSelector).ServiceGroupID
            End If

            hidServiceGroup.Value = CType(txtServiceGroup, InPlaceServiceGroupSelector).ServiceGroupID

            msg = DomService.FetchList(Me.DbConnection, svcList, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault, e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = DomUnitsOfMeasure.FetchList(conn:=Me.DbConnection, list:=dumList, auditUserName:=String.Empty, auditLogTitle:=String.Empty)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' refresh the list of existing bands and save in view state
            ClearViewState(e, VIEWSTATE_KEY_DATA_SERVICE_TYPE)
            list = GetUniqueIDsFromViewState(VIEWSTATE_KEY_DATA_SERVICE_TYPE, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)

            js = New StringBuilder()
            With js
                .Append("serviceTypes_services=new Collection();")
            End With

            With js
                .Append("serviceTypes_dum=new Collection();")
            End With

            servGroup = New ServiceGroup(conn:=Me.DbConnection, _
                                           auditUserName:=String.Empty, AuditLogTitle:=String.Empty)

            servGroupClass = New ServiceGroupClassification(conn:=Me.DbConnection, _
                                                            auditUserName:=String.Empty, AuditLogTitle:=String.Empty)
            With servGroup
                msg = .Fetch(_type.ServiceGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End With

            With servGroupClass
                msg = .Fetch(servGroup.ServiceGroupClassificationID)
                _careType = .CareType
            End With

            ' set _isVisitBased indicator
            For Each service As DomService In svcList
                Dim id As String = GetUniqueServiceID(service)
                OutputNonResidentialControls(id, service, js)
                list.Add(id)
            Next
            PersistUniqueIDsToViewState(list, VIEWSTATE_KEY_DATA_SERVICE_TYPE, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)

            ViewState("type") = _type

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                ClearViewState(e, VIEWSTATE_KEY_DATA_SERVICE_TYPE)
                ClearViewState(e, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)
                txtDescription.Text = String.Empty
                txtAbbreviation.Text = String.Empty
                CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText = String.Empty
                CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText = String.Empty
                chkRedundant.CheckBox.Checked = False
                CType(txtServiceGroup, InPlaceServiceGroupSelector).ServiceGroupText = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim transRolledBack As Boolean = False
            Dim svcList As DomServiceCollection = New DomServiceCollection


            ' check to see if its in use
            msg = DomContractBL.ServiceTypeInUseByContractVisitServiceType(Me.DbConnection, e.ItemID, _inUseByContractVisitServiceType)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                'Try and remove the Service Type from any expenditure account group records
                Dim svcTypeList As ExpenditureAccountGroupDomServiceTypeCollection = New ExpenditureAccountGroupDomServiceTypeCollection
                msg = ExpenditureAccountGroupDomServiceType.FetchList(trans, svcTypeList, 0, e.ItemID)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    trans.Rollback()
                    transRolledBack = True
                End If
                For Each DomServiceType As ExpenditureAccountGroupDomServiceType In svcTypeList
                    msg = ExpenditureAccountGroupDomServiceType.Delete(trans, DomServiceType.ID)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        trans.Rollback()
                        transRolledBack = True
                    End If
                Next

                'deassociate any services
                msg = RemoveAssociationDomService(trans, e.ItemID)
                If Not msg.Success Then
                    trans.Rollback()
                    transRolledBack = True
                    If msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_SERVICE_TYPE Then
                        lblError.Text = msg.Message
                    Else
                        WebUtils.DisplayError(msg)
                    End If

                End If

                If Not transRolledBack Then
                    msg = DomServiceType.Delete(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                    If Not msg.Success Then
                        lblError.Text = msg.Message
                        trans.Rollback()
                        transRolledBack = True
                    End If
                End If

                If transRolledBack Then
                    e.Cancel = True
                    FindClicked(e)
                Else
                    trans.Commit()
                    ClearViewState(e, VIEWSTATE_KEY_DATA_SERVICE_TYPE)
                    ClearViewState(e, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)
                    txtDescription.Text = String.Empty
                    txtAbbreviation.Text = String.Empty
                    CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText = String.Empty
                    CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText = String.Empty
                    chkRedundant.CheckBox.Checked = False
                    phLinkedServices.Controls.Clear()
                    CType(txtServiceGroup, InPlaceServiceGroupSelector).ServiceGroupText = String.Empty
                    _careType = 0
                End If



            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                e.Cancel = True
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                _isDeleted = Not e.Cancel
            End Try

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim resServicesToDelete As List(Of Integer) = New List(Of Integer)
            Dim serviceList As List(Of String) = New List(Of String)
            Dim resServiceList As List(Of String) = New List(Of String)
            Dim resCareServices As ArrayList = New ArrayList
            Dim addStatusToSave As ArrayList = New ArrayList
            Dim accTypeToSave As ArrayList = New ArrayList
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim transRolledBack As Boolean = False
            Dim prevServiceID As Integer
            Dim errorOccurred As Boolean = False
            Dim svcList As DomServiceCollection = New DomServiceCollection
            Dim serviceGroupID As Integer = 0
            Dim counter As Integer
            Dim servGroup As ServiceGroup = Nothing
            Dim servGroupClass As ServiceGroupClassification
            Dim findResult As Integer = 0

            If Me.IsPostBack Then
                msg = populateConvertServiceTypeDropDown()
                If Not msg.Success Then WebUtils.DisplayError(msg)
                cboConvertToServiceType.SelectPostBackValue()
            End If


            If CheckServiceTypeInUse(e.ItemID) Then
                serviceGroupID = Convert.ToInt32(hidServiceGroup.Value)
                CType(txtServiceGroup, InPlaceServiceGroupSelector).ServiceGroupID = Convert.ToInt32(hidServiceGroup.Value)
            Else
                serviceGroupID = Request.Form(CType(txtServiceGroup, InPlaceServiceGroupSelector).HiddenFieldUniqueID)
            End If

            If hidServiceGroup.Value <> String.Empty Then
                serviceGroupID = Convert.ToInt32(hidServiceGroup.Value)
                CType(txtServiceGroup, InPlaceServiceGroupSelector).ServiceGroupID = Convert.ToInt32(hidServiceGroup.Value)
            End If

            If Me.IsValid Then
                _type = New DomServiceType(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With _type
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    ' check to see if its in use
                    msg = DomContractBL.ServiceTypeInUseByContractVisitServiceType(Me.DbConnection, e.ItemID, _inUseByContractVisitServiceType)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' get care type
                    servGroup = New ServiceGroup(conn:=Me.DbConnection, _
                               auditUserName:=String.Empty, AuditLogTitle:=String.Empty)

                    servGroupClass = New ServiceGroupClassification(conn:=Me.DbConnection, _
                                                                    auditUserName:=String.Empty, AuditLogTitle:=String.Empty)

                    With servGroup
                        msg = .Fetch(_type.ServiceGroupID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    With servGroupClass
                        msg = .Fetch(servGroup.ServiceGroupClassificationID)
                        _careType = .CareType
                    End With

                ElseIf hidServiceGroup.Value <> String.Empty Or serviceGroupID <> 0 Then
                    ' get care type
                    servGroup = New ServiceGroup(conn:=Me.DbConnection, _
                               auditUserName:=String.Empty, AuditLogTitle:=String.Empty)

                    servGroupClass = New ServiceGroupClassification(conn:=Me.DbConnection, _
                                                                    auditUserName:=String.Empty, AuditLogTitle:=String.Empty)

                    With servGroup
                        msg = .Fetch(serviceGroupID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    With servGroupClass
                        msg = .Fetch(servGroup.ServiceGroupClassificationID)
                        _careType = .CareType
                    End With
                End If

                Try

                    Dim preSaveServiceList As DomServiceCollection = Nothing
                    Dim preBudgetCategories As List(Of Integer) = Nothing
                    Dim preBudgetCategoriesInUse As List(Of Integer) = Nothing
                    Dim budgetCategoriesToSaveList As New List(Of Integer)()
                    Dim currentServiceToSave As DomService = Nothing
                    Dim servicesToSaveList As New List(Of DomService)()
                    Dim servicesToReconsider As New HashSet(Of Integer)()

                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    ' fetch the service list
                    If _type.ID <> 0 Then
                        msg = DomService.FetchList(trans:=trans, list:=svcList, domServiceTypeID:=_type.ID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If

                    If Not svcList Is Nothing AndAlso svcList.Count > 0 Then
                        ' if we have some existing service

                        Dim distinctBudgetCategories As List(Of Integer) = (From svc In svcList.ToArray() Where svc.BudgetCategoryID > 0 Select svc.BudgetCategoryID).Distinct().ToList()

                        If Not distinctBudgetCategories Is Nothing AndAlso distinctBudgetCategories.Count > 0 Then
                            ' if we have some budget categories check if they are in use

                            Dim bcIsInUse As Boolean = False

                            preBudgetCategoriesInUse = New List(Of Integer)()

                            For Each bcId As Integer In distinctBudgetCategories
                                ' loop each bc and check if in use

                                msg = BudgetCategoryBL.GetIfBudgetCategoryInUse(trans, bcId, bcIsInUse)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                                If bcIsInUse Then preBudgetCategoriesInUse.Add(bcId)

                            Next

                        End If

                        preBudgetCategories = distinctBudgetCategories

                    End If

                    For Each uniqueID As String In GetUniqueIDsFromViewState(VIEWSTATE_KEY_DATA_SERVICE_TYPE, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)
                        If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                            currentServiceToSave = New DomService()
                            With currentServiceToSave
                                .ID = CType(phLinkedServices.FindControl(CTRL_PREFIX_SERVICE & uniqueID), InPlaceServiceSelector).ItemID
                                .DomUnitsOfMeasureID = Utils.ToInt32(CType(phLinkedServices.FindControl(CTRL_PREFIX_DUM_HDN & uniqueID), HiddenField).Value)
                                .BudgetCategoryID = CType(phLinkedServices.FindControl(CTRL_PREFIX_BUDGET_CATEGORY & uniqueID), InPlaceBudgetCategorySelector).ItemID
                                .ConversionFactorNumerator = Utils.ToInt32(CType(phLinkedServices.FindControl(CTRL_PREFIX_BUDGET_CATEGORY_NUMERATOR & uniqueID), TextBoxEx).Value)
                                .ConversionFactorDenominator = Utils.ToInt32(CType(phLinkedServices.FindControl(CTRL_PREFIX_BUDGET_CATEGORY_DENOMINATOR & uniqueID), TextBoxEx).Value)
                                If .BudgetCategoryID > 0 AndAlso budgetCategoriesToSaveList.Contains(.BudgetCategoryID) = False Then
                                    budgetCategoriesToSaveList.Add(.BudgetCategoryID)
                                End If
                            End With
                            servicesToSaveList.Add(currentServiceToSave)
                        End If
                    Next

                    ' get any items that were in the pre list but not in the pro i.e. deleted items
                    servicesToReconsider.UnionWith((From pre In svcList.ToArray() _
                                                        Where _
                                                                (From post In servicesToSaveList.ToArray() _
                                                                    Where post.ID > 0 _
                                                                        Select post.ID).Contains(pre.ID) = False _
                                                                AndAlso pre.ID > 0 _
                                                        Select pre.ID).ToList())

                    With _type
                        .ServiceGroupID = serviceGroupID
                        .Description = txtDescription.Text
                        .Abbreviation = IIf(txtAbbreviation.Text.Trim().Length = 0, Nothing, txtAbbreviation.Text)
                        .FinanceCode1 = CType(txtFinanceCode, InPlaceFinanceCodeSelector).FinanceCodeText
                        .FinanceCode2 = CType(txtFinanceCode2, InPlaceFinanceCodeSelector).FinanceCodeText
                        .ServiceCategory = servGroup.ServiceCategory
                        .Permanent = servGroup.Permanent
                        .Redundant = chkRedundant.CheckBox.Checked

                        .ConvertToServiceTypeID = Utils.ToInt32(cboConvertToServiceType.DropDownList.SelectedValue)
                        .DoubleNoOfCarers = chkDoubleNoCarers.CheckBox.Checked
                        .HalveDuration = chkHalveDuration.CheckBox.Checked

                        ' save
                        msg = DomContractBL.SaveServiceType(trans, _type)
                        If Not msg.Success Then
                            If msg.Number = DomContractBL.ERR_RATE_CATEGORY_ABBREV_INVALID Or _
                                    msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_SERVICE_TYPE Then
                                ' rate category abbrev invalid
                                lblError.Text = msg.Message
                                SqlHelper.RollbackTransaction(trans)
                                transRolledBack = True
                            Else
                                SqlHelper.RollbackTransaction(trans)
                                transRolledBack = True
                                WebUtils.DisplayError(msg)
                            End If
                        End If
                        e.ItemID = .ID
                    End With

                    'Associate/deassociate services to this service type.
                    If Not transRolledBack Then
                        msg = RemoveAssociationDomService(trans, _type.ID, servicesToSaveList)
                        If Not msg.Success Then
                            trans.Rollback()
                            transRolledBack = True
                            errorOccurred = True
                            If msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_SERVICE_TYPE Then
                                lblError.Text = msg.Message
                            Else
                                WebUtils.DisplayError(msg)
                            End If
                        End If

                        If Not errorOccurred Then

                            For Each service As DomService In servicesToSaveList
                                If prevServiceID = service.ID Then
                                    lblError.Text = "Cannot have duplicate services entered."
                                    errorOccurred = True
                                    trans.Rollback()
                                    transRolledBack = True
                                    Exit For
                                End If
                                prevServiceID = service.ID
                            Next
                        End If

                        If Not errorOccurred Then
                            Dim service As DomService = New DomService(trans)
                            Dim serviceType As DomServiceType = Nothing
                            Dim reconsiderService As Boolean = False
                            Dim preService As DomService = Nothing
                            counter = 0
                            errorOccurred = True
                            For Each serviceToSave As DomService In servicesToSaveList
                                reconsiderService = False
                                msg = service.Fetch(serviceToSave.ID)
                                If Not msg.Success Then
                                    trans.Rollback()
                                    transRolledBack = True
                                    WebUtils.DisplayError(msg)
                                End If
                                'Check to see if the service is already associated to another service Type
                                If Utils.ToInt32(service.DomServiceTypeID) <> 0 AndAlso service.DomServiceTypeID <> _type.ID Then
                                    serviceType = New DomServiceType(trans, String.Empty, String.Empty)
                                    With serviceType
                                        msg = .Fetch(service.DomServiceTypeID)
                                        If Not msg.Success Then
                                            trans.Rollback()
                                            transRolledBack = True
                                            WebUtils.DisplayError(msg)
                                        End If
                                    End With
                                    lblError.Text = String.Format("The service '{0}' is already associated to the Service Type '{1}'", service.Title, serviceType.Description)
                                    trans.Rollback()
                                    transRolledBack = True
                                End If
                                'Associate the service Type to the service.
                                service.DomServiceTypeID = _type.ID
                                service.DomUnitsOfMeasureID = serviceToSave.DomUnitsOfMeasureID
                                ' determine if the service existed in the list of items before
                                preService = (From tmpService In svcList.ToArray() Where tmpService.ID = service.ID Select tmpService).FirstOrDefault()
                                ' if the bc of the service has changed or the service did not exist in the list before
                                If preService Is Nothing _
                                    OrElse (preService IsNot Nothing AndAlso preService.BudgetCategoryID <> serviceToSave.BudgetCategoryID) Then
                                    reconsiderService = True
                                End If
                                service.BudgetCategoryID = serviceToSave.BudgetCategoryID
                                service.ConversionFactorNumerator = serviceToSave.ConversionFactorNumerator
                                service.ConversionFactorDenominator = serviceToSave.ConversionFactorDenominator
                                counter += 1
                                msg = service.Save
                                If Not msg.Success Then
                                    trans.Rollback()
                                    transRolledBack = True
                                    WebUtils.DisplayError(msg)
                                End If
                                _careType = CareType.NonResidential
                                ' add the service into the list of services to reconsider
                                If reconsiderService Then
                                    servicesToReconsider.Add(service.ID)
                                End If
                            Next
                            errorOccurred = False
                        End If

                        If Not errorOccurred AndAlso Not budgetCategoriesToSaveList Is Nothing Then
                            ' if no errors and we have some budget categories to save then do so

                            Dim postBudgetCategoriesInUse As New List(Of Integer)()
                            Dim missingInUseBudgetCategories As New List(Of Integer)()

                            If Not servicesToSaveList Is Nothing AndAlso servicesToSaveList.Count > 0 Then
                                ' if we have some services

                                postBudgetCategoriesInUse = (From svc In servicesToSaveList Select svc.BudgetCategoryID).Distinct().ToList()

                            End If

                            If Not preBudgetCategoriesInUse Is Nothing AndAlso preBudgetCategoriesInUse.Count > 0 Then

                                ' get any budget categories that are required to remain linked to this service type cos they are in use somewhere
                                missingInUseBudgetCategories = (From pre In preBudgetCategoriesInUse _
                                                                    Where _
                                                                            (From post In postBudgetCategoriesInUse _
                                                                                Where post > 0 _
                                                                                    Select post).Contains(pre) = False _
                                                                            AndAlso pre > 0 _
                                                                    Select pre).ToList()

                            End If

                            If Not missingInUseBudgetCategories Is Nothing AndAlso missingInUseBudgetCategories.Count > 0 Then
                                ' if we have at least one in use budget category that is no longer in use with this service type then moan

                                Dim currentBudgetCategory As BudgetCategory = Nothing
                                Dim sbMissingBudgetCategories As New StringBuilder()

                                sbMissingBudgetCategories.Append("The following budget categories must be linked to this service type:<ol>")

                                For Each missingBcId As Integer In missingInUseBudgetCategories
                                    ' loop each budget category and append to string

                                    msg = BudgetCategoryBL.Fetch(trans, missingBcId, currentBudgetCategory, String.Empty, String.Empty)
                                    If Not msg.Success Then WebUtils.DisplayError(msg)

                                    sbMissingBudgetCategories.AppendFormat("<li>{0} : {1}</li>", currentBudgetCategory.Reference, currentBudgetCategory.Description)

                                Next

                                sbMissingBudgetCategories.Append("</ol>")

                                trans.Rollback()
                                transRolledBack = True
                                lblError.Text = sbMissingBudgetCategories.ToString()

                            Else

                                Dim currentBudgetCategory As BudgetCategory = Nothing
                                Dim missingBudgetCategories As New List(Of Integer)()

                                If Not preBudgetCategories Is Nothing AndAlso preBudgetCategories.Count > 0 Then

                                    ' get the budget cats that existed before this save but have been deleted i.e missing budget categories
                                    missingBudgetCategories = (From pre In preBudgetCategories _
                                                                    Where _
                                                                            (From post In budgetCategoriesToSaveList _
                                                                                Where post > 0 _
                                                                                    Select post).Contains(pre) = False _
                                                                            AndAlso pre > 0 _
                                                                    Select pre).ToList()

                                End If

                                ' add in the missing budget categories
                                budgetCategoriesToSaveList.AddRange(missingBudgetCategories)

                                For Each bcId As Integer In budgetCategoriesToSaveList
                                    ' loop each budget category and associate this service type 

                                    If bcId > 0 Then
                                        ' if we have a budget category to save 

                                        ' get the bc to alter
                                        msg = BudgetCategoryBL.Fetch(trans, bcId, currentBudgetCategory, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                                        If Not msg.Success Then WebUtils.DisplayError(msg)

                                        With currentBudgetCategory
                                            If missingBudgetCategories.Contains(bcId) Then
                                                ' we have removed this bc from the service type so blank out service type and category
                                                .DomServiceTypeID = Nothing
                                                .ServiceCategory = Nothing
                                            Else
                                                ' we have added/updated this budget category so update service type and category
                                                .DomServiceTypeID = _type.ID
                                                .ServiceCategory = _type.ServiceCategory
                                            End If
                                        End With

                                        ' save the changes to budget category and report error if one exists
                                        msg = currentBudgetCategory.Save(False)
                                        If Not msg.Success Then WebUtils.DisplayError(msg)

                                    End If

                                Next

                            End If

                        End If
                    End If

                    If Not errorOccurred AndAlso servicesToReconsider.Count > 0 Then
                        ' if we have at least one item to reconsider then do so if no errors occurred earlier on...

                        ServiceUserContributionBL.FlagWeeksForReconsiderationByDomServiceOrder(trans, servicesToReconsider)

                    End If

                    If Not transRolledBack Then

                        trans.Commit()

                    Else

                        e.Cancel = True

                    End If

                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                    e.Cancel = True
                    WebUtils.DisplayError(msg)
                Finally
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                    End If
                End Try


            Else
                e.Cancel = True
            End If

        End Sub

        Private Function RemoveAssociationDomService(ByVal trans As SqlTransaction, ByVal domServiceTypeID As Integer, Optional ByVal servicesToSave As List(Of DomService) = Nothing) As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage
            Dim rateCatList As DomRateCategoryCollection = New DomRateCategoryCollection
            Dim svcList As DomServiceCollection = New DomServiceCollection
            Dim service As DomService = New DomService(trans)

            msg = DomService.FetchList(trans:=trans, list:=svcList, domServiceTypeID:=domServiceTypeID)
            If Not msg.Success Then Return msg

            For Each domSvc As DomService In svcList
                Dim deleteService As Boolean = True
                'Check if service is in the list of services on the screen
                If Not servicesToSave Is Nothing Then
                    For Each svc As DomService In servicesToSave
                        If svc.ID = domSvc.ID Then deleteService = False
                    Next
                End If

                If deleteService Then
                    msg = DomRateCategory.FetchList(trans, rateCatList, String.Empty, String.Empty, domSvc.ID, domServiceTypeID)
                    If Not msg.Success Then Return msg

                    If rateCatList.Count > 0 Then
                        msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_SERVICE_TYPE
                        msg.Message = String.Format("The service '{0}' cannot be removed as it is used on a rate category.", domSvc.Title)
                        msg.Success = False
                        Return msg
                    Else
                        'Remove association with service
                        msg = service.Fetch(domSvc.ID)
                        If Not msg.Success Then Return msg

                        service.DomServiceTypeID = Nothing
                        msg = service.Save
                        If Not msg.Success Then Return msg

                    End If
                End If
            Next

            msg.Success = True
            Return msg
        End Function

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If _type Is Nothing AndAlso _stdBut.SelectedItemID > 0 AndAlso _isDeleted = False Then
                ' if we havent got a type but have an id that isnt deleted then fetch from db

                Dim msg As New ErrorMessage()

                _type = New DomServiceType(Me.DbConnection, String.Empty, String.Empty)
                msg = _type.Fetch(_stdBut.SelectedItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

            End If

            If Not _type Is Nothing Then
                ' if we have a type i.e. this is an existing record

                tabLinkedServices.Visible = True

                ' setup visibility of the linked services tab and its content
                With _type
                    Select Case CType(.ServiceCategory, ServiceCategory)
                        Case ServiceCategory.NonResidential
                            tabLinkedServices.Visible = True
                            pnlLinkedServicesList.Visible = True
                        Case Else
                            tabLinkedServices.Visible = False
                    End Select
                End With

            Else
                ' else a new type so no linked services available for view

                tabLinkedServices.Visible = False

            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim msg As ErrorMessage
            Dim serviceList As List(Of String) = New List(Of String)
            Dim servicesToSave As ArrayList = New ArrayList
            Dim js As StringBuilder
            Dim servGroup As ServiceGroup
            Dim servGroupClass As ServiceGroupClassification
            Dim svcGroups As Target.Web.Apps.Security.Collections.ServiceGroupCollection = Nothing
            Dim counts As Integer = tabStrip.Tabs.Count
            Dim frameworkTypes As New FrameworkTypeCollection()

            ' select the current tab
            tabStrip.SetActiveTabByHeaderText(hidSelectedTab.Value)

            js = New StringBuilder()
            With js
                .Append("serviceTypes_services=new Collection();")
            End With

            With js
                .Append("serviceTypes_dum=new Collection();")
            End With

            ' check to see if its in use
            msg = DomContractBL.ServiceTypeInUseByContractVisitServiceType(Me.DbConnection, _stdBut.SelectedItemID, _inUseByContractVisitServiceType)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            serviceList = GetUniqueIDsFromViewState(VIEWSTATE_KEY_DATA_SERVICE_TYPE, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)
            For Each uniqueID As String In serviceList
                If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                    ' add to the collection
                    Dim serviceID As Integer = Utils.ToInt32(CType(phLinkedServices.FindControl(CTRL_PREFIX_SERVICE & uniqueID), InPlaceServiceSelector).ItemID)
                    servicesToSave.Add(serviceID)

                    With js
                        .AppendFormat("serviceTypes_services.add({0},{1});", _
                                              String.Format("'{0}_cboDropDownList'", CType(phLinkedServices.FindControl(CTRL_PREFIX_SERVICE & uniqueID), InPlaceServiceSelector).ClientID), _
                                              serviceID)
                    End With

                    Dim dumID As Integer = Utils.ToInt32(CType(phLinkedServices.FindControl(CTRL_PREFIX_DUM_HDN & uniqueID), HiddenField).Value)

                    With js
                        .AppendFormat("serviceTypes_dum.add({0},{1});", _
                                              String.Format("'{0}_cboDropDownList'", CType(phLinkedServices.FindControl(CTRL_PREFIX_DUM & uniqueID), DropDownListEx).ClientID), _
                                              dumID)
                    End With

                End If
            Next

            js.AppendFormat("mode={0};careType='{1}';lblServiceCategoryID='{2}'; hidServiceGroupID='{3}'; selectedServiceTypeID = {4};", Convert.ToInt32(_stdBut.ButtonsMode), _careType, lblServiceCategory.ClientID, hidServiceGroup.ClientID, _stdBut.SelectedItemID)
            js.AppendFormat("inEditMode = {0};", _inEditMode.ToString().ToLower())

            ' fetch service groups to store in an array client side for dynamic behaviour
            msg = ServiceGroup.FetchList(conn:=DbConnection, list:=svcGroups, auditUserName:=String.Empty, auditLogTitle:=String.Empty)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' fetch the framework types
            msg = FrameworkType.FetchList(conn:=DbConnection, list:=frameworkTypes, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Not svcGroups Is Nothing AndAlso svcGroups.Count > 0 Then
                ' if we have some svc groups then output info as javascript

                Dim svcGroupIndex As Integer = 0
                Dim serviceTypeFrameworkType As FrameworkType = Nothing

                If Not _type Is Nothing Then _serviceTypeID = _type.ID

                msg = DomContractBL.FetchFrameWorkTypeByServiceType(Me.DbConnection, _serviceTypeID, serviceTypeFrameworkType)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                For Each svcGroup As ServiceGroup In svcGroups
                    ' loop each service group and add some js to store in page

                    If Not serviceTypeFrameworkType Is Nothing Then
                        js.AppendFormat("serviceGroupCollection[{0}] = new ServiceGroup({1}, {2}, {3});", _
                                    svcGroupIndex, _
                                    svcGroup.ID, _
                                    svcGroup.ServiceCategory, _
                                    IIf(svcGroup.Permanent = TriState.True, "true", "false"))

                        svcGroupIndex += 1

                    End If

                Next

            End If

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
                                      js.ToString(), _
                                      True)

            If CheckServiceTypeInUse(_stdBut.SelectedItemID) Then
                CType(txtServiceGroup, InPlaceServiceGroupSelector).Enabled = False
            Else
                CType(txtServiceGroup, InPlaceServiceGroupSelector).Required = True
                CType(txtServiceGroup, InPlaceServiceGroupSelector).ValidationGroup = "Save"
            End If

            If _stdBut.SelectedItemID > 0 Then
                CType(txtServiceGroup, InPlaceServiceGroupSelector).Enabled = False
            End If

            If _type Is Nothing Then
                _type = ViewState("type")
            End If

            If Not _type Is Nothing And _careType <> 0 Then
                servGroup = New ServiceGroup(conn:=Me.DbConnection, _
                   auditUserName:=String.Empty, AuditLogTitle:=String.Empty)

                servGroupClass = New ServiceGroupClassification(conn:=Me.DbConnection, _
                                                                auditUserName:=String.Empty, AuditLogTitle:=String.Empty)
                With servGroup
                    If _type.ServiceGroupID > 0 Then
                        msg = .Fetch(_type.ServiceGroupID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If
                End With

                With servGroupClass
                    If servGroup.ServiceGroupClassificationID > 0 Then
                        msg = .Fetch(servGroup.ServiceGroupClassificationID)
                        _careType = .CareType
                    End If
                End With
            End If

        End Sub
        Public Function CheckServiceTypeInUse(Optional ByVal serviceTypeId As Integer = 0) As Boolean
            Dim msg As ErrorMessage
            Dim tbServiceType As DomRateCategoryCollection = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim tbDomService As DomServiceCollection = Nothing

            If serviceTypeId > 0 And _careType = CareType.NonResidential Then
                msg = DomRateCategory.FetchList(Me.DbConnection, tbServiceType, String.Empty, String.Empty, , serviceTypeId)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If tbServiceType.Count > 0 Then
                    tbServiceType = Nothing
                    Return True
                End If
            End If

            If _careType = CareType.NonResidential Then
                If serviceTypeId > 0 Then
                    msg = DomService.FetchList(Me.DbConnection, tbDomService, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault, serviceTypeId)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    If tbDomService.Count > 0 Then
                        tbDomService = Nothing
                        Return True
                    End If
                End If
            End If

            Return False
        End Function


        Private Function populateConvertServiceTypeDropDown() As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage
            Dim svcTypes As DomServiceTypeCollection = Nothing
            With cboConvertToServiceType

                msg = DomServiceType.FetchList(conn:=Me.DbConnection, _
                                               auditLogTitle:=String.Empty, _
                                               auditUserName:=String.Empty, _
                                               list:=svcTypes, _
                                               redundant:=TriState.False)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                With .DropDownList
                    .Items.Clear()
                    .DataSource = svcTypes
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

            msg.Success = True
            Return msg
        End Function

#Region " Linked Services Table Code "

#Region "           ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs, ByVal key As String)
            ViewState.Remove(key)

            If key = VIEWSTATE_KEY_DATA_SERVICE_TYPE Then
                phLinkedServices.Controls.Clear()
                _services = Nothing
                _dums = Nothing
            End If

            ViewState.Remove("type")

        End Sub

#End Region

#Region "           btnAddLinkedServices_Click "

        Private Sub btnAddLinkedServices_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddLinkedServices.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState(VIEWSTATE_KEY_DATA_SERVICE_TYPE, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)
            Dim service As DomService = New DomService(Me.DbConnection)
            Dim js As StringBuilder
            Dim serviceList As List(Of String) = New List(Of String)
            Dim msg As ErrorMessage
            Dim uomService As DomService = New DomService(Me.DbConnection)
            Dim servGroup As New ServiceGroup(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
            Dim frameWrkType As FrameworkType = Nothing

            js = New StringBuilder()
            With js
                .Append("serviceTypes_services=new Collection();")
            End With

            With js
                .Append("serviceTypes_dum=new Collection();")
            End With

            serviceList = GetUniqueIDsFromViewState(VIEWSTATE_KEY_DATA_SERVICE_TYPE, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)

            For Each uniqueID As String In serviceList
                If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                    ' add to the collection
                    Dim serviceID As Integer = Utils.ToInt32(CType(phLinkedServices.FindControl(CTRL_PREFIX_SERVICE & uniqueID), InPlaceServiceSelector).ItemID)

                    If serviceID > 0 Then

                        With js
                            .AppendFormat("serviceTypes_services.add({0},{1});", _
                                                  String.Format("'{0}_cboDropDownList'", CType(phLinkedServices.FindControl(CTRL_PREFIX_SERVICE & uniqueID), InPlaceServiceSelector).ItemID), _
                                                  serviceID)
                        End With

                        With uomService
                            msg = .Fetch(serviceID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            With js
                                .AppendFormat("serviceTypes_dum.add({0},{1});", _
                                                      String.Format("'{0}_cboDropDownList'", CType(phLinkedServices.FindControl(CTRL_PREFIX_DUM & uniqueID), DropDownListEx).ClientID), _
                                                      uomService.DomUnitsOfMeasureID)
                            End With
                        End With

                    End If

                End If

            Next

            _type = New DomServiceType(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            If _stdBut.SelectedItemID > 0 Then
                With _type
                    msg = .Fetch(_stdBut.SelectedItemID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With
                msg = servGroup.Fetch(_type.ServiceGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                service.VisitBasedReturn = TriState.False
            Else
                lblError.Text = ADD_LINKED_SERVICES_ERROR_MSG
            End If

            If _stdBut.SelectedItemID > 0 Then
                ' add a new row to the Service list
                id = GetUniqueServiceID(service)
                ' create the controls
                OutputNonResidentialControls(id, service, js)
                ' persist the data into view state
                list.Add(id)
                PersistUniqueIDsToViewState(list, VIEWSTATE_KEY_DATA_SERVICE_TYPE, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)
            End If


            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)
            _careType = CareType.NonResidential

            If hidServiceGroup.Value <> String.Empty Then
                CType(txtServiceGroup, InPlaceServiceGroupSelector).ServiceGroupID = Convert.ToInt32(hidServiceGroup.Value)
            End If

            _inEditMode = True

        End Sub

#End Region

#Region "           OutputNonResidentialControls "

        Private Sub OutputNonResidentialControls(ByVal uniqueID As String, ByVal service As DomService, ByRef js As StringBuilder)

            Dim row As TableRow
            Dim msg As New ErrorMessage()
            Dim cell As TableCell
            Dim removeButton As HtmlInputImage
            Dim ipService As InPlaceServiceSelector
            Dim ipBudgetCategory As InPlaceBudgetCategorySelector
            Dim cboDomUnitsOfMeasure As DropDownListEx
            Dim hdnDomUnitsOfMeasure As New HiddenField()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim lblMeasuredIn As Label
            Dim txtNumerator As TextBoxEx
            Dim txtDenominator As TextBoxEx
            Dim genTxtNumeratorContainer As HtmlGenericControl
            Dim genTxtDenominator As HtmlGenericControl
            Dim genSplitter As HtmlGenericControl
            Dim genConversionFactorEditableContainer As HtmlGenericControl
            Dim genConversionFactorReadOnlyContainer As HtmlGenericControl
            Dim validationGroup As String = "Save"

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

                row = New TableRow()
                row.ID = ROW_PREFIX & uniqueID
                phLinkedServices.Controls.Add(row)

                ' Service
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.CssClass = "domService"
                cell.Style.Add("vertical-align", "top")
                ipService = CType(LoadControl("~/AbacusWeb/Apps/InPlaceSelectors/InPlaceServiceSelector.ascx"), InPlaceServiceSelector)
                With ipService
                    .ID = CTRL_PREFIX_SERVICE & uniqueID
                    .Required = True
                    .RequiredValidatorErrorMessage = "Please Select a Service."
                    .RequiredValidatorValidationGroup = validationGroup
                    If Not service Is Nothing AndAlso service.ID > 0 Then
                        .ItemID = service.ID
                    End If
                    .TextWidth = "80%"
                End With
                cell.Controls.Add(ipService)

                ' Dom Units of Measure
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.CssClass = "domService"
                cell.Style.Add("vertical-align", "top")
                cboDomUnitsOfMeasure = New DropDownListEx()
                With cboDomUnitsOfMeasure
                    .ID = CTRL_PREFIX_DUM & uniqueID
                    .ValidationGroup = validationGroup
                    .Required = True
                    .RequiredValidatorErrMsg = "Please Enter a Unit of Measure."
                    If Not service Is Nothing AndAlso service.DomUnitsOfMeasureID > 0 Then .DropDownList.SelectedValue = service.DomUnitsOfMeasureID
                    .Width = Unit.Percentage(95)
                End With
                cell.Controls.Add(cboDomUnitsOfMeasure)
                With hdnDomUnitsOfMeasure
                    .ID = CTRL_PREFIX_DUM_HDN & uniqueID
                    .Value = cboDomUnitsOfMeasure.DropDownList.SelectedValue
                End With
                cell.Controls.Add(hdnDomUnitsOfMeasure)

                If Not service Is Nothing AndAlso service.ID <> 0 Then
                    With js
                        .AppendFormat("serviceTypes_services.add({0},{1});", _
                                              String.Format("'{0}_cboDropDownList'", ipService), _
                                              service.ID)
                    End With

                    With js
                        .AppendFormat("serviceTypes_dum.add({0},{1});", _
                                              String.Format("'{0}_cboDropDownList'", cboDomUnitsOfMeasure.ClientID), _
                                              service.DomUnitsOfMeasureID)
                    End With
                End If

                ' Budget Category
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.CssClass = "budgetCategories"
                cell.Style.Add("vertical-align", "top")
                ipBudgetCategory = CType(LoadControl("~/AbacusWeb/Apps/InPlaceSelectors/InPlaceBudgetCategorySelector.ascx"), InPlaceBudgetCategorySelector)
                With ipBudgetCategory
                    .ID = CTRL_PREFIX_BUDGET_CATEGORY & uniqueID
                    .Required = False
                    .RequiredValidatorErrorMessage = "Please Select a Budget Category."
                    .RequiredValidatorValidationGroup = validationGroup
                    If Not service Is Nothing AndAlso service.ID > 0 Then
                        .ItemID = service.BudgetCategoryID
                    End If
                    .TextWidth = "80%"
                End With
                cell.Controls.Add(ipBudgetCategory)

                ' Measure In
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.CssClass = "budgetCategories"
                cell.Style.Add("vertical-align", "top")
                lblMeasuredIn = New Label()
                With lblMeasuredIn
                    .ID = "bcUom" & uniqueID
                    .Text = "&nbsp;"
                    If Not service Is Nothing AndAlso service.BudgetCategoryID > 0 Then
                        ' if we have a bc to fetch then do so

                        Dim bc As New BudgetCategory(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)
                        Dim uom As New DomUnitsOfMeasure(conn:=DbConnection, auditLogTitle:=String.Empty, auditUserName:=String.Empty)

                        msg = bc.Fetch(service.BudgetCategoryID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        If bc.DomUnitsOfMeasureID > 0 Then

                            msg = uom.Fetch(bc.DomUnitsOfMeasureID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            .Text = uom.Description

                        End If

                    End If
                End With
                cell.Controls.Add(lblMeasuredIn)

                ' Conversion Factor
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.CssClass = "budgetCategories"
                cell.Style.Add("vertical-align", "top")

                ' create the two top level divs for controls
                genConversionFactorEditableContainer = New HtmlGenericControl("div")
                genConversionFactorReadOnlyContainer = New HtmlGenericControl("div")

                ' setup read only div
                genConversionFactorReadOnlyContainer.InnerText = "Automatic"
                genConversionFactorReadOnlyContainer.Style.Add("display", "none")

                ' create a div to hold the converion numerator in
                genTxtNumeratorContainer = New HtmlGenericControl("div")

                With genTxtNumeratorContainer
                    .Style.Add("float", "left")
                End With

                ' create the numerator field
                txtNumerator = New TextBoxEx()
                With txtNumerator
                    .ID = CTRL_PREFIX_BUDGET_CATEGORY_NUMERATOR & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                    .MinimumValue = 1
                    .MaximumValue = 255
                    .ValidationGroup = validationGroup
                    .Width = New Unit(5, UnitType.Em)
                    If Not service Is Nothing Then
                        .Text = service.ConversionFactorNumerator
                    End If
                End With

                ' add the numerator into the div and then the div into the cell
                genTxtNumeratorContainer.Controls.Add(txtNumerator)
                genConversionFactorEditableContainer.Controls.Add(genTxtNumeratorContainer)
                genConversionFactorEditableContainer.Style.Add("display", "none")

                ' create a splitter for the numerator and denominator and add into cell
                genSplitter = New HtmlGenericControl("div")

                With genSplitter
                    .Style.Add("float", "left")
                End With

                genSplitter.InnerText = "/"
                genConversionFactorEditableContainer.Controls.Add(genSplitter)

                ' create a div to hold the converion denominator in
                genTxtDenominator = New HtmlGenericControl("div")

                With genTxtNumeratorContainer
                    .Style.Add("float", "left")
                End With

                ' create the denominator field
                txtDenominator = New TextBoxEx()
                With txtDenominator
                    .ID = CTRL_PREFIX_BUDGET_CATEGORY_DENOMINATOR & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                    .MinimumValue = 1
                    .MaximumValue = 255
                    .ValidationGroup = validationGroup
                    .Width = New Unit(5, UnitType.Em)
                    If Not service Is Nothing Then
                        .Text = service.ConversionFactorDenominator
                    End If
                End With

                ' add the denominator into the div and then the div into the cell
                genTxtDenominator.Controls.Add(txtDenominator)
                genConversionFactorEditableContainer.Controls.Add(genTxtDenominator)

                cell.Controls.Add(genConversionFactorEditableContainer)
                cell.Controls.Add(genConversionFactorReadOnlyContainer)

                ' remove button
                cell = New TableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                removeButton = New HtmlInputImage()
                With removeButton
                    .ID = CTRL_PREFIX_REMOVED & uniqueID
                    .Src = WebUtils.GetVirtualPath("images/delete.png")
                    .Alt = "Remove this line"
                    AddHandler .ServerClick, AddressOf Remove_Click
                End With

                ' add edit controls into cell
                cell.Controls.Add(removeButton)

                cboDomUnitsOfMeasure.DropDownList.Attributes.Add("onchange", String.Format("cboDomUnitsOfMeasure_Change('{0}_cboDropDownList')", cboDomUnitsOfMeasure.ClientID))

            End If

        End Sub

#End Region

#Region "           Remove_Click "

        Private Sub Remove_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewState(VIEWSTATE_KEY_DATA_SERVICE_TYPE, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_PREFIX_REMOVED, String.Empty)

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
            For index As Integer = 0 To phLinkedServices.Controls.Count - 1
                If phLinkedServices.Controls(index).ID.Replace(ROW_PREFIX, String.Empty) = id Then
                    phLinkedServices.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState(list, VIEWSTATE_KEY_DATA_SERVICE_TYPE, VIEWSTATE_KEY_COUNTER_SERVICE_TYPE)
            _careType = CareType.NonResidential

            If hidServiceGroup.Value <> String.Empty Then
                CType(txtServiceGroup, InPlaceServiceGroupSelector).ServiceGroupID = Convert.ToInt32(hidServiceGroup.Value)
            End If

            _inEditMode = True

        End Sub

#End Region

#Region "           GetUniqueServiceID "

        Private Function GetUniqueServiceID(ByVal service As DomService) As String

            Dim id As String

            If service.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW & _newServiceIDCounter
                _newServiceIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE & service.ID
            End If

            Return id

        End Function

#End Region

#Region "           PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String), ByVal viewStateKeyData As String, _
                                                   ByVal viewStateKeyCounter As String)
            ViewState.Add(viewStateKeyData, list)
            ViewState.Add(viewStateKeyCounter, _newServiceIDCounter)
        End Sub

#End Region

#Region "           GetUniqueIDsFromViewState "

        Private Function GetUniqueIDsFromViewState(ByVal viewStateKeyData As String, _
                                                   ByVal viewStateKeyCounter As String) As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(viewStateKeyData) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(viewStateKeyData), List(Of String))
            End If
            If ViewState.Item(viewStateKeyCounter) Is Nothing Then
                _newServiceIDCounter = 0
            Else
                _newServiceIDCounter = CType(ViewState.Item(viewStateKeyCounter), Integer)
            End If

            Return list

        End Function

#End Region

#Region "           LoadServicesDropdown "

        Private Sub LoadServicesDropdown(ByVal dropdown As DropDownListEx)

            Dim msg As ErrorMessage
            msg = DomService.FetchList(Me.DbConnection, _services, TriState.UseDefault, TriState.True, TriState.False)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _services.Sort(New CollectionSorter("Title", SortDirection.Ascending))

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = _services
                .DataTextField = "Title"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region "           LoadAdmissionStatusDropdown "

        Private Sub LoadAdmissionStatusDropdown(ByVal dropdown As DropDownListEx)

            Dim msg As ErrorMessage
            Dim _admissionStatuses1 As vwAdmissionStatusCollection = Nothing

            msg = vwAdmissionStatus.FetchList(conn:=Me.DbConnection, list:=_admissionStatuses, _
                                              infoString:="PERMANENT")
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = vwAdmissionStatus.FetchList(conn:=Me.DbConnection, list:=_admissionStatuses1, _
                                  infoString:="TEMPORARY")
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _admissionStatuses.AddRange(_admissionStatuses1)

            _admissionStatuses.Sort(New CollectionSorter("Description", SortDirection.Ascending))

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = _admissionStatuses
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region "           LoadAccommodationTypeDropdown "

        Private Sub LoadAccommodationTypeDropdown(ByVal dropdown As DropDownListEx)

            Dim msg As ErrorMessage
            msg = vwAccommodationType.FetchList(conn:=Me.DbConnection, list:=_accommodationTypes)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _accommodationTypes.Sort(New CollectionSorter("Description", SortDirection.Ascending))

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = _accommodationTypes
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region "           FindID "
        Private Function FindID(ByVal id As Integer) As Boolean
            If id = _deleteFindID Then
                Return True
            Else
                Return False
            End If
        End Function
#End Region

#End Region

    End Class


End Namespace