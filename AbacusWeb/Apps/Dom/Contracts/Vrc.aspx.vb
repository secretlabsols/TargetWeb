
Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Dom.Contracts

	''' <summary>
	''' Screen used to maintain a domiciliary contract visit rate category.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' CD      29/04/2010  A4WA#6252 - prevented refresh of tree if delete failed (DeleteClicked), so user can see error message
    ''' MvO     15/12/2009  A4WA#5967 - fix when deleting immediately after creating and control re-initialisation.
    ''' MvO     07/04/2009  D11537 - need to suppress Csrf check due to use of iframe.
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MikeVO  18/08/2008  D11398 - support for visit service types.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class Vrc
        Inherits BasePage

        Private _contract As DomContract
        Private _contractID As Integer
        Private _vrcID As Integer
        Private _refreshTree As Boolean
        Private _currentTimeBandID As Integer
        Private _stdBut As StdButtonsBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Contract Visit Rate Category")

            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _vrcID = Utils.ToInt32(Request.QueryString("id"))

            Dim msg As ErrorMessage
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            ' fetch the contract
            _contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = _contract.Fetch(_contractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With _stdBut
                .AllowNew = canEdit
                .ShowNew = False
                .AllowFind = False
                .AllowEdit = canEdit
                .AllowDelete = canEdit
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
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
            ' add the page JS
            Me.JsLinks.Add("Vrc.js")

            cboDayCategory.DropDownList.Attributes.Add("onchange", "FetchTimeBands();RefreshRateCategories();")
            cboServiceType.DropDownList.Attributes.Add("onchange", "RefreshRateCategories();")
            cboTimeBand.DropDownList.Attributes.Add("onchange", "RefreshRateCategories();")

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup1", _
              Target.Library.Web.Utils.WrapClientScript(String.Format( _
              "frameworkID={0};", _contract.DomRateFrameworkID) _
              ) _
             )

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            PopulateDropdowns(0, 0, 0)
            _currentTimeBandID = 0
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim vrc As DomContractVisitRateCategory

            vrc = New DomContractVisitRateCategory(Me.DbConnection, String.Empty, String.Empty)
            With vrc
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                PopulateDropdowns(.DomDayCategoryID, .DomServiceTypeID, .DomTimeBandID)

                If .DomDayCategoryID > 0 Then cboDayCategory.DropDownList.SelectedValue = .DomDayCategoryID
                _currentTimeBandID = Utils.ToInt32(.DomTimeBandID)
                If .DomTimeBandID > 0 Then cboTimeBand.DropDownList.SelectedValue = .DomTimeBandID
                If .DomServiceTypeID > 0 Then cboServiceType.DropDownList.SelectedValue = .DomServiceTypeID
                If .CareWorkers > 0 Then cboCareWorkers.DropDownList.SelectedValue = .CareWorkers
                txtMinutesFrom.Text = .MinutesFrom
                txtMinutesTo.Text = .MinutesTo
                cboEntireVisitUnitRounding.DropDownList.SelectedValue = .EntireVisitUnitRounding
                cboEntireVisitUnitConversion.DropDownList.SelectedValue = .EntireVisitUnitConversion
                cboEntireVisitRateCategory.DropDownList.SelectedValue = .EntireVisitRateCategoryID
                cboPartVisitUnitRounding.DropDownList.SelectedValue = .PartVisitUnitRounding
                cboPartVisitUnitConversion.DropDownList.SelectedValue = .PartVisitUnitConversion
                cboPartVisitRateCategory.DropDownList.SelectedValue = .PartVisitRateCategoryID

                Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", _
                 Target.Library.Web.Utils.WrapClientScript(String.Format( _
                 "entireVisitDomRateCategoryID={0};partVisitDomRateCategoryID={1}", .EntireVisitRateCategoryID, .PartVisitRateCategoryID) _
                 ) _
                )

            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                PopulateDropdowns(0, 0, 0)
                cboDayCategory.DropDownList.SelectedValue = String.Empty
                cboTimeBand.DropDownList.SelectedValue = String.Empty
                cboServiceType.DropDownList.SelectedValue = String.Empty
                cboCareWorkers.DropDownList.SelectedValue = Convert.ToInt32(DomContractVisitRateCategoryCareWorker.All)
                txtMinutesFrom.Text = String.Empty
                txtMinutesTo.Text = String.Empty
                cboEntireVisitUnitRounding.DropDownList.SelectedValue = String.Empty
                cboEntireVisitUnitConversion.DropDownList.SelectedValue = String.Empty
                cboEntireVisitRateCategory.DropDownList.SelectedValue = String.Empty
                cboPartVisitUnitRounding.DropDownList.SelectedValue = String.Empty
                cboPartVisitUnitConversion.DropDownList.SelectedValue = String.Empty
                cboPartVisitRateCategory.DropDownList.SelectedValue = String.Empty
                _currentTimeBandID = 0
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = DomContractBL.DeleteContractVisitRateCategory(Me.DbConnection, e.ItemID, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            If Not msg.Success Then
                If msg.Number = "E3009" Then    ' could not delete vrc
                    lblError.Text = msg.Message
                    e.Cancel = True
                Else
                    WebUtils.DisplayError(msg)
                End If
            Else
                _vrcID = 0
                _refreshTree = True
                NewClicked(Nothing)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim vrc As DomContractVisitRateCategory
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim dayCategoryID As Integer, timeBandID As Integer, serviceTypeID As Integer

            ' as we are not using viewstate because it fecks other things up, dropdowns need a bit of extra work to pass validation
            dayCategoryID = Utils.ToInt32(cboDayCategory.GetPostBackValue())
            timeBandID = Utils.ToInt32(cboTimeBand.GetPostBackValue())
            serviceTypeID = Utils.ToInt32(cboServiceType.GetPostBackValue())

            PopulateDropdowns(dayCategoryID, serviceTypeID, timeBandID)

            cboDayCategory.SelectPostBackValue()
            cboTimeBand.SelectPostBackValue()
            cboServiceType.SelectPostBackValue()
            cboCareWorkers.SelectPostBackValue()
            cboEntireVisitUnitRounding.SelectPostBackValue()
            cboEntireVisitUnitConversion.SelectPostBackValue()
            cboEntireVisitRateCategory.SelectPostBackValue()
            cboPartVisitUnitRounding.SelectPostBackValue()
            cboPartVisitUnitConversion.SelectPostBackValue()
            cboPartVisitRateCategory.SelectPostBackValue()

            Me.Validate("Save")

            If Me.IsValid Then
                vrc = New DomContractVisitRateCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With vrc
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                Else
                    vrc.DomContractID = _contractID
                End If
                With vrc
                    .AuditLogOverriddenParentID = vrc.DomContractID
                    .DomDayCategoryID = cboDayCategory.DropDownList.SelectedValue
                    .DomTimeBandID = cboTimeBand.DropDownList.SelectedValue
                    _currentTimeBandID = .DomTimeBandID
                    .DomServiceTypeID = cboServiceType.DropDownList.SelectedValue
                    .CareWorkers = cboCareWorkers.DropDownList.SelectedValue
                    .MinutesFrom = txtMinutesFrom.Text
                    .MinutesTo = txtMinutesTo.Text
                    .EntireVisitUnitRounding = cboEntireVisitUnitRounding.DropDownList.SelectedValue
                    .EntireVisitUnitConversion = cboEntireVisitUnitConversion.DropDownList.SelectedValue
                    .EntireVisitRateCategoryID = cboEntireVisitRateCategory.DropDownList.SelectedValue
                    .PartVisitUnitRounding = cboPartVisitUnitRounding.DropDownList.SelectedValue
                    .PartVisitUnitConversion = cboPartVisitUnitConversion.DropDownList.SelectedValue
                    .PartVisitRateCategoryID = cboPartVisitRateCategory.DropDownList.SelectedValue

                    msg = DomContractBL.SaveContractVisitRateCategory(Me.DbConnection, vrc, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    If Not msg.Success Then
                        If msg.Number = "E3010" Then    ' could not save vrc
                            lblError.Text = msg.Message
                            e.Cancel = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    Else
                        e.ItemID = .ID
                        _vrcID = .ID
                        _refreshTree = True
                    End If
                End With
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub PopulateDropdowns(ByVal dayCategoryID As Integer, ByVal serviceTypeID As Integer, ByVal timeBandID As Integer)

            Dim msg As ErrorMessage
            Dim list As List(Of ViewablePair) = Nothing

            With cboDayCategory
                ' get a list of day categories available to the contract
                msg = DomContractBL.FetchDayCategoriesAvailableToContract(Me.DbConnection, _contractID, list, dayCategoryID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .DataSource = list
                    .DataTextField = "Text"
                    .DataValueField = "Value"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

            With cboTimeBand
                ' get a list of time bands available to the contract
                msg = DomContractBL.FetchTimeBandsAvailableToContract(Me.DbConnection, _contractID, list)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .DataSource = list
                    .DataTextField = "Text"
                    .DataValueField = "Value"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

            With cboServiceType
                ' get a list of service types available to the contract
                msg = DomContractBL.FetchServiceTypesAvailableToVrc(Me.DbConnection, _contractID, list)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .DataSource = list
                    .DataTextField = "Text"
                    .DataValueField = "Value"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

            With cboCareWorkers
                With .DropDownList.Items
                    .Clear()
                    For Each value As DomContractVisitRateCategoryCareWorker In [Enum].GetValues(GetType(DomContractVisitRateCategoryCareWorker))
                        If value = DomContractVisitRateCategoryCareWorker.All Then
                            .Add(New ListItem(String.Empty, Convert.ToInt32(value)))
                        Else
                            .Add(New ListItem([Enum].GetName(GetType(DomContractVisitRateCategoryCareWorker), value), Convert.ToInt32(value)))
                        End If
                    Next
                End With
            End With

            ' get a list of rate categories available to this vrc
            msg = DomContractBL.FetchRateCategoriesAvailableToVrc(Me.DbConnection, _contract.DomRateFrameworkID, dayCategoryID, serviceTypeID, timeBandID, list)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With cboEntireVisitUnitRounding
                With .DropDownList.Items
                    .Clear()
                    For Each value As DomContractVisitRateCategoryUnitRounding In [Enum].GetValues(GetType(DomContractVisitRateCategoryUnitRounding))
                        .Add(New ListItem(Utils.SplitOnCapitals([Enum].GetName(GetType(DomContractVisitRateCategoryUnitRounding), value)), Convert.ToInt32(value)))
                    Next
                    .Insert(0, String.Empty)
                End With
            End With

            With cboEntireVisitUnitConversion
                With .DropDownList.Items
                    .Clear()
                    For Each value As DomContractVisitRateCategoryUnitConversion In [Enum].GetValues(GetType(DomContractVisitRateCategoryUnitConversion))
                        .Add(New ListItem(Utils.SplitOnCapitals([Enum].GetName(GetType(DomContractVisitRateCategoryUnitConversion), value)), Convert.ToInt32(value)))
                    Next
                    .Insert(0, String.Empty)
                End With
            End With

            With cboEntireVisitRateCategory
                With .DropDownList
                    .Items.Clear()
                    .DataSource = list
                    .DataTextField = "Text"
                    .DataValueField = "Value"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

            With cboPartVisitUnitRounding
                With .DropDownList.Items
                    .Clear()
                    For Each value As DomContractVisitRateCategoryUnitRounding In [Enum].GetValues(GetType(DomContractVisitRateCategoryUnitRounding))
                        .Add(New ListItem(Utils.SplitOnCapitals([Enum].GetName(GetType(DomContractVisitRateCategoryUnitRounding), value)), Convert.ToInt32(value)))
                    Next
                    .Insert(0, String.Empty)
                End With
            End With

            With cboPartVisitUnitConversion
                With .DropDownList.Items
                    .Clear()
                    For Each value As DomContractVisitRateCategoryUnitConversion In [Enum].GetValues(GetType(DomContractVisitRateCategoryUnitConversion))
                        .Add(New ListItem(Utils.SplitOnCapitals([Enum].GetName(GetType(DomContractVisitRateCategoryUnitConversion), value)), Convert.ToInt32(value)))
                    Next
                    .Insert(0, String.Empty)
                End With
            End With

            With cboPartVisitRateCategory
                With .DropDownList
                    .Items.Clear()
                    .DataSource = list
                    .DataTextField = "Text"
                    .DataValueField = "Value"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            ClientScript.RegisterStartupScript(Me.GetType(), "Startup3", String.Format("currentTimeBandID={0};", _currentTimeBandID), True)
            If _refreshTree Then
                ClientScript.RegisterStartupScript(Me.GetType(), "Startup4", String.Format("window.parent.RefreshTree({0}, 'vrc', {1});currentTimeBandID={2};", _contractID, _vrcID, _currentTimeBandID), True)
            End If

        End Sub

        Private Sub txtMinutes_AfterTextBoxControlAdded(ByVal sender As TextBoxEx) Handles txtMinutesFrom.AfterTextBoxControlAdded, txtMinutesTo.AfterTextBoxControlAdded

            ' add custom controls to the minute ranges controls
            With sender.Controls

                Dim lit As Literal = New Literal()
                lit.Text = "&nbsp;"
                .Add(lit)

                Dim link As HyperLink = New HyperLink()
                link.Style.Add("font-size", "x-small")
                link.Text = IIf(sender.ID = "txtMinutesFrom", "[Minimum]", "[Maximum]")
                link.NavigateUrl = String.Format("javascript:SetMinutes('{0}',{1});", _
                    sender.ClientID, IIf(sender.ID = "txtMinutesFrom", DomContractBL.VRC_MIN_MINUTES, DomContractBL.VRC_MAX_MINUTES))
                .Add(link)

            End With

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            _stdBut.Visible = (_contract.EndDate = DataUtils.MAX_DATE)
        End Sub

    End Class

End Namespace