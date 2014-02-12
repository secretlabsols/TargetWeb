
Imports System.Collections.Generic
Imports System.Text
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

Namespace Apps.Sds

    ''' <summary>
    ''' Screen used to maintain an SDS personal budget.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Public Class PersonalBudget
        Inherits BasePage

#Region " Private variables "

        Private _stdBut As StdButtonsBase
        Private _budgetID As Integer, _clientID As Integer

#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.PersonalBudgetEnquiry"), "Personal Budget")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("id")) > 0 Then
                _budgetID = Utils.ToInt32(Request.QueryString("id"))
            End If
            _clientID = Utils.ToInt32(Request.QueryString("clientID"))

            ' setup buttons
            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PersonalBudgetEnquiry.AddNew"))
                .ShowNew = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PersonalBudgetEnquiry.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PersonalBudgetEnquiry.Delete"))
                .EditableControls.Add(fsControls.Controls)
                .AllowFind = False
                .AllowBack = True
                .AuditLogTableNames.Add("RASPersonalBudget")
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add page JS
            Me.JsLinks.Add("PersonalBudget.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Sds))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

        End Sub

#End Region

#Region " NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            PopulateClient()
            PopulateDropdowns()
        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim budget As RASPersonalBudget
            Dim rt As RASType
            Dim reason As RASOverrideReason

            budget = New RASPersonalBudget(Me.DbConnection, String.Empty, String.Empty)
            With budget
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _budgetID = .ID
                _clientID = .ClientID

                PopulateClient()
                PopulateDropdowns()

                ' ras type
                If cboRasType.DropDownList.Items.FindByValue(.RASTypeID) Is Nothing Then
                    ' the current ras type is redundant so we need to add it in
                    rt = New RASType(Me.DbConnection, String.Empty, String.Empty)
                    msg = rt.Fetch(.RASTypeID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    cboRasType.DropDownList.Items.Add(New ListItem(rt.Description, rt.ID))
                End If
                cboRasType.DropDownList.SelectedValue = .RASTypeID

                dteRasDate.Text = .RASDate
                If Not .RASRevision Is Nothing Then txtRasRevision.Text = .RASRevision
                dteEffectiveDate.Text = .DateFrom
                If .DateTo < DataUtils.MAX_DATE Then dteEndDate.Text = .DateTo

                txtPointScore.Text = .PointScore
                If .OverriddenPointScoreReasonID > 0 Then
                    chkOverridePointScore.Checked = True
                    txtOverriddenPointScore.Text = .OverriddenPointScore
                    ' override reason
                    If cboOverriddenPointScoreReason.DropDownList.Items.FindByValue(.OverriddenPointScoreReasonID) Is Nothing Then
                        ' the current reason is redundant so we need to add it in
                        reason = New RASOverrideReason(Me.DbConnection, String.Empty, String.Empty)
                        msg = reason.Fetch(.OverriddenPointScoreReasonID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        cboOverriddenPointScoreReason.DropDownList.Items.Add(New ListItem(reason.Description, reason.ID))
                    End If
                    cboOverriddenPointScoreReason.DropDownList.SelectedValue = .OverriddenPointScoreReasonID
                End If

                txtBudget.Text = .Budget.ToString("F2")
                If .OverriddenBudgetReasonID > 0 Then
                    chkOverrideBudget.Checked = True
                    txtOverriddenBudget.Text = .OverriddenBudget.ToString("F2")
                    ' override reason
                    If cboOverriddenBudgetReason.DropDownList.Items.FindByValue(.OverriddenBudgetReasonID) Is Nothing Then
                        ' the current reason is redundant so we need to add it in
                        reason = New RASOverrideReason(Me.DbConnection, String.Empty, String.Empty)
                        msg = reason.Fetch(.OverriddenBudgetReasonID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        cboOverriddenBudgetReason.DropDownList.Items.Add(New ListItem(reason.Description, reason.ID))
                    End If
                    cboOverriddenBudgetReason.DropDownList.SelectedValue = .OverriddenBudgetReasonID
                End If

            End With



        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                PopulateClient()
                PopulateDropdowns()

            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = RASPersonalBudget.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim budget As RASPersonalBudget
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' as we are not using viewstate because it fecks other things up, dropdowns need a bit of extra work to pass validation
            PopulateDropdowns()
            cboRasType.SelectPostBackValue()
            cboOverriddenPointScoreReason.SelectPostBackValue()
            cboOverriddenBudgetReason.SelectPostBackValue()

            txtOverriddenPointScore.RequiredValidator.Enabled = chkOverridePointScore.Checked
            cboOverriddenPointScoreReason.RequiredValidator.Enabled = chkOverridePointScore.Checked
            txtOverriddenBudget.RequiredValidator.Enabled = chkOverrideBudget.Checked
            cboOverriddenBudgetReason.RequiredValidator.Enabled = chkOverrideBudget.Checked
            Me.Validate("Save")

            If Me.IsValid Then
                budget = New RASPersonalBudget(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With budget
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        _budgetID = .ID
                        _clientID = .ClientID
                    End With
                Else
                    budget.ClientID = _clientID
                End If

                PopulateClient()

                With budget
                    .RASTypeID = Utils.ToInt32(cboRasType.DropDownList.SelectedValue)
                    .RASDate = dteRasDate.Text
                    If txtRasRevision.Text.Trim().Length = 0 Then
                        .RASRevision = Nothing
                    Else
                        .RASRevision = txtRasRevision.Text
                    End If
                    .DateFrom = dteEffectiveDate.Text
                    If Utils.IsDate(dteEndDate.Text) Then
                        .DateTo = dteEndDate.Text
                    Else
                        .DateTo = DataUtils.MAX_DATE
                    End If
                    .PointScore = txtPointScore.Text
                    If chkOverridePointScore.Checked Then
                        .OverriddenPointScore = txtOverriddenPointScore.Text
                        .OverriddenPointScoreReasonID = cboOverriddenPointScoreReason.DropDownList.SelectedValue
                    Else
                        .OverriddenPointScore = Nothing
                        .OverriddenPointScoreReasonID = Nothing
                    End If

                    msg = SdsBL.CalculatePersonalBudget(Me.DbConnection, .RASTypeID, .RASDate, .PointScore, .Budget)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    txtBudget.Text = .Budget.ToString("F2")

                    If chkOverrideBudget.Checked Then
                        .OverriddenBudget = txtOverriddenBudget.Text
                        .OverriddenBudgetReasonID = cboOverriddenBudgetReason.DropDownList.SelectedValue
                    Else
                        .OverriddenBudget = Nothing
                        .OverriddenBudgetReasonID = Nothing
                    End If

                    ' save budget
                    msg = SdsBL.SavePersonalBudget(Me.DbConnection, budget)
                    If Not msg.Success Then
                        If msg.Number = SdsBL.ERR_COULD_NOT_SAVE_PERSONAL_BUDGET Then
                            lblError.Text = msg.Message
                            e.Cancel = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    Else
                        e.ItemID = .ID
                        _budgetID = .ID
                    End If

                End With

            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " PopulateClient "

        Private Sub PopulateClient()

            Dim msg As ErrorMessage
            Dim client As ClientDetail
            
            ' set client details
            client = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
            With client
                msg = .Fetch(_clientID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtClient.Text = String.Format("{0}: {1}", .Reference, .Name)
            End With

        End Sub

#End Region

#Region " PopulateDropdowns "

        Private Sub PopulateDropdowns()

            Dim msg As ErrorMessage
            Dim rasTypes As RASTypeCollection = Nothing
            Dim reasons As RASOverrideReasonCollection = Nothing

            ' get non-redundant types
            msg = RASType.FetchList(Me.DbConnection, rasTypes, String.Empty, String.Empty, TriState.False)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With cboRasType.DropDownList
                .Items.Clear()
                .DataSource = rasTypes
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                ' insert a blank at the top
                .Items.Insert(0, New ListItem(String.Empty))
            End With

            ' get non-redundant override reasons
            msg = RASOverrideReason.FetchList(Me.DbConnection, reasons, String.Empty, String.Empty, TriState.False)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With cboOverriddenPointScoreReason.DropDownList
                .Items.Clear()
                .DataSource = reasons
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                ' insert a blank at the top
                .Items.Insert(0, New ListItem(String.Empty))
            End With
            With cboOverriddenBudgetReason.DropDownList
                .Items.Clear()
                .DataSource = reasons
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                ' insert a blank at the top
                .Items.Insert(0, New ListItem(String.Empty))
            End With


        End Sub

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim js As StringBuilder = New StringBuilder()

            ' hook up client-side events
            cboRasType.DropDownList.Attributes.Add("onchange", "CalculateBudget();")
            chkOverridePointScore.Attributes.Add("onclick", "chkOverridePointScore_Click();")
            chkOverrideBudget.Attributes.Add("onclick", "chkOverrideBudget_Click();")

            ' output startup javascript
            js.AppendFormat("txtOverriddenPointScore_ClientID='{0}';", txtOverriddenPointScore.ClientID)
            js.AppendFormat("cboOverriddenPointScoreReason_ClientID='{0}';", cboOverriddenPointScoreReason.ClientID)
            js.AppendFormat("txtOverriddenBudget_ClientID='{0}';", txtOverriddenBudget.ClientID)
            js.AppendFormat("cboOverriddenBudgetReason_ClientID='{0}';", cboOverriddenBudgetReason.ClientID)
            js.AppendFormat("mode={0};", Convert.ToInt32(_stdBut.ButtonsMode))

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

        End Sub

#End Region

#Region " Page_PreRenderComplete "

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            ' disable override fields as appropriate
            txtOverriddenPointScore.TextBox.Enabled = chkOverridePointScore.Checked
            cboOverriddenPointScoreReason.DropDownList.Enabled = chkOverridePointScore.Checked
            txtOverriddenBudget.TextBox.Enabled = chkOverrideBudget.Checked
            cboOverriddenBudgetReason.DropDownList.Enabled = chkOverrideBudget.Checked

            ' disable calc-ed budget field
            txtBudget.TextBox.Enabled = False

        End Sub

#End Region

    End Class

End Namespace