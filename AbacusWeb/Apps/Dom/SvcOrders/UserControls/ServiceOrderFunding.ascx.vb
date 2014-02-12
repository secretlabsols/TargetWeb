#Region " Imports "
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.FinanceCodes
Imports Target.Abacus.Library.ServiceOrder
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils
#End Region

Namespace Apps.Dom.SvcOrders
    Partial Public Class ServiceOrderFunding
        Inherits System.Web.UI.UserControl

#Region " Variables "
        Private _svcOrderFundingDetails As List(Of ServiceOrderFundingDetail)
        Private _fundingRevision As DomServiceOrderFundingRevision
        Private _mostRecentRevision As DomServiceOrderFundingRevision
        Private _revisionCount As Integer
        Private _svcOrder As DomServiceOrder
        Private _saveServiceTypeID As Integer
        Private _earliestRevisionID As Integer
        ' consts
        Private _SelectorName As String = "ServiceOrderFunding"
#End Region

#Region " Properties "
        ''' <summary>
        ''' Gets base page.
        ''' </summary>
        ''' <value>Base page.</value>
        Private ReadOnly Property MyBasePage() As BasePage
            Get
                Return CType(Page, BasePage)
            End Get
        End Property

        Public Property ButtonClicked() As String
            Get
                Return hid_ButtonClicked.Value
            End Get
            Set(ByVal value As String)
                hid_ButtonClicked.Value = value
            End Set
        End Property


        Public Property MostRecentRevision() As DomServiceOrderFundingRevision
            Get
                Return _mostRecentRevision
            End Get
            Set(ByVal value As DomServiceOrderFundingRevision)
                _mostRecentRevision = value
            End Set
        End Property


        Public Property FundingRevision() As DomServiceOrderFundingRevision
            Get
                Return _fundingRevision
            End Get
            Set(ByVal value As DomServiceOrderFundingRevision)
                _fundingRevision = value
            End Set
        End Property

        Public Property RevisionCount() As Integer
            Get
                Return _revisionCount
            End Get
            Set(ByVal value As Integer)
                _revisionCount = value
            End Set
        End Property

        Public Property svcOrderID() As Integer
            Get
                If hidFundingDSOID.Value = "" Then
                    Return 0
                Else
                    Return hidFundingDSOID.Value
                End If

            End Get
            Set(ByVal value As Integer)
                hidFundingDSOID.Value = value
            End Set
        End Property

        Public Property SvcOrder() As DomServiceOrder
            Get
                Return _svcOrder
            End Get
            Set(ByVal value As DomServiceOrder)
                _svcOrder = value
            End Set
        End Property

        Public Property svcOrderFundingDetails() As List(Of ServiceOrderFundingDetail)

            Get
                Return IIf(Session("fundingDetail") IsNot Nothing, Session("fundingDetail"), Nothing)
            End Get
            Set(ByVal value As List(Of ServiceOrderFundingDetail))
                Session("fundingDetail") = value
            End Set
        End Property

        Public Property RevisionID() As Integer
            Get
                If hidRevisionID.Value = String.Empty Then
                    Return 0
                Else
                    Return hidRevisionID.Value
                End If
            End Get
            Set(ByVal value As Integer)
                Dim msg As ErrorMessage

                hidRevisionID.Value = value

                If value <> 0 Then
                    FundingRevision = New DomServiceOrderFundingRevision(MyBasePage.DbConnection)
                    msg = FundingRevision.Fetch(value)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                Else
                    FundingRevision = Nothing
                End If

            End Set
        End Property

        Private _differentRevisionSelected As Boolean
        Public Property differentRevisionSelected() As Boolean
            Get
                Return _differentRevisionSelected
            End Get
            Set(ByVal value As Boolean)
                _differentRevisionSelected = value
            End Set
        End Property

        Public Property earliestRevisionID() As Integer
            Get
                Return _earliestRevisionID
            End Get
            Set(ByVal value As Integer)
                _earliestRevisionID = value
            End Set
        End Property



#End Region

#Region " defaultServiceType "

        Private Function defaultServiceType() As String
            Dim details As List(Of ServiceOrderFundingDetail) = IIf(Session("fundingDetail") IsNot Nothing, Session("fundingDetail"), Nothing)
            Dim svcType As String = String.Empty

            If Not details Is Nothing Then
                details = details.FindAll(AddressOf FindDefaultItems)

                For Each item As ServiceOrderFundingDetail In details
                    svcType = item.ServiceType
                    Exit For
                Next
            End If

            Return svcType

        End Function

#End Region

#Region " populateFromSvcOrderScreen "

        Public Function populateFromSvcOrderScreen(ByVal DSO_ID As Integer) As ErrorMessage
            Dim msg As ErrorMessage

            msg = populateControl(DSO_ID)
            If Not msg.Success Then Return msg
            loadFundindDataIntoSession()
            msg = createControls()
            If Not msg.Success Then WebUtils.DisplayError(msg)
            DataBindFinanceCodeTables()

            msg = New ErrorMessage
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " populateControl "

        Public Function populateControl(ByVal domServiceOrderID As Integer) As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage
            Dim revisions As DomServiceOrderFundingRevisionCollection = Nothing

            If domServiceOrderID = 0 Then
                msg.Success = True
                Return msg
            End If

            svcOrderID = domServiceOrderID
            SvcOrder = New DomServiceOrder(MyBasePage.DbConnection, String.Empty, String.Empty)
            msg = SvcOrder.Fetch(domServiceOrderID)
            If Not msg.Success Then Return msg

            msg = populateEffectiveDateDropDown()
            If Not msg.Success Then Return msg

            'Work out the maximum DateFrom Value
            Dim dateFroms As List(Of Date) = New List(Of Date)

            dateFroms.Add(SvcOrder.DateFrom)
            If Not MostRecentRevision Is Nothing Then
                'if there is a revision then we want the most recent effective date + one day
                dateFroms.Add(MostRecentRevision.EffectiveDate.AddDays(6))
            End If
            Dim maxDateFrom As Date = (From d As Date In dateFroms Select d).Max()

            fundingEffectiveDate.MinimumValue = maxDateFrom.ToShortDateString
            fundingEffectiveDate.MaximumValue = SvcOrder.DateTo.ToShortDateString

            'If Not MyBasePage.IsPostBack OrElse ButtonClicked = "Cancel" Then
            '    If Not FundingRevision Is Nothing And ButtonClicked <> "New" Then

            If (Not MyBasePage.IsPostBack OrElse ButtonClicked = "Cancel") Or _
                MyBasePage.StdButtonsMode = Target.Library.Web.UserControls.StdButtonsMode.Fetched Or _
                MyBasePage.StdButtonsMode = Target.Library.Web.UserControls.StdButtonsMode.Edit Then
                If (Not FundingRevision Is Nothing And ButtonClicked <> "New") And _
                    (Not FundingRevision Is Nothing And _
                     MyBasePage.StdButtonsMode = Target.Library.Web.UserControls.StdButtonsMode.Fetched) Or _
                    (Not FundingRevision Is Nothing And MyBasePage.StdButtonsMode = Target.Library.Web.UserControls.StdButtonsMode.Edit) Then


                    FundingRevision = MostRecentRevision
                    If Not FundingRevision Is Nothing Then
                        cbofundingEffectiveDate.DropDownList.SelectedValue = FundingRevision.ID
                        CType(fundingCareManager, InPlaceCareManagerSelector).CareManagerID = FundingRevision.CareManagerID
                        CType(fundingTeam, InPlaceTeamSelector).TeamID = FundingRevision.TeamID
                        CType(fundingClientGroup, InPlaceClientGroupSelector).ClientGroupID = FundingRevision.ClientGroupID
                        CType(fundingClientSubGroup, InPlaceClientSubGroupSelector).ClientSubGroupID = FundingRevision.ClientSubGroupID
                        chkUserGenerated.Checked = FundingRevision.UserRevision
                        msg = setFundingType(FundingRevision)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        fundingEffectiveDate.Text = FundingRevision.EffectiveDate.ToShortDateString
                        RevisionID = FundingRevision.ID
                    End If
                End If

            End If

            msg.Success = True
            Return msg

        End Function

#End Region

#Region " populateEffectiveDateDropDown  "

        Private Function populateEffectiveDateDropDown() As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage
            Dim revisions As DomServiceOrderFundingRevisionCollection = Nothing
            Dim revisionList As List(Of DomServiceOrderFundingRevision) = Nothing

            MostRecentRevision = Nothing
            msg = DomServiceOrderFundingRevision.FetchList(MyBasePage.DbConnection, revisions, svcOrderID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            RevisionCount = revisions.Count

            'Order revisions by effective date
            revisionList = revisions.ToArray().OrderBy(Function(tmpitem As DomServiceOrderFundingRevision) tmpitem.EffectiveDate).ToList()

            cbofundingEffectiveDate.DropDownList.AutoPostBack = True
            earliestRevisionID = 0

            With cbofundingEffectiveDate.DropDownList
                .Items.Clear()
                For Each revision As DomServiceOrderFundingRevision In revisionList
                    Dim item As ListItem = New ListItem(revision.EffectiveDate.ToShortDateString, revision.ID)
                    .Items.Add(item)

                    'Store the Ealiest RevisionID
                    If earliestRevisionID = 0 Then
                        earliestRevisionID = revision.ID
                        System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "earliestRevision", String.Format("earliestRevisionID = {0};", revision.ID), True)
                    End If

                    If MostRecentRevision Is Nothing OrElse revision.EffectiveDate > MostRecentRevision.EffectiveDate Then
                        MostRecentRevision = revision
                    End If

                    If revision.ID = RevisionID Or (RevisionID = 0 And ButtonClicked <> "New") Then
                        FundingRevision = revision
                        cbofundingEffectiveDate.DropDownList.SelectedValue = revision.ID
                    End If
                Next
            End With

            msg.Success = True
            Return msg

        End Function

#End Region

#Region " createControls "

        Private Function createControls() As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage
            Dim allServiceTypes As List(Of ViewablePair) = New List(Of ViewablePair)
            Dim dsoServiceTypes As List(Of ViewablePair) = New List(Of ViewablePair)
            Dim effectiveDate As Date = Date.Now

            phFinanceCodes.Controls.Clear()
            plannedSTList.Controls.Clear()
            otherSTList.Controls.Clear()

            If RevisionID <> 0 Then
                '++ Added check, since FundingRevision object may not be primed at this point..
                If FundingRevision Is Nothing Then RevisionID = RevisionID

                cbofundingEffectiveDate.DropDownList.SelectedValue = FundingRevision.ID
                effectiveDate = FundingRevision.EffectiveDate
                fundingEffectiveDate.Text = effectiveDate.ToShortDateString
            Else
                If fundingEffectiveDate.Text <> String.Empty Then
                    effectiveDate = fundingEffectiveDate.Text
                End If
            End If


            msg = ServiceOrder.ServiceOrderBL.FetchServiceTypesUsedOnOrder(MyBasePage.DbConnection, _
                                                                           svcOrderID, _
                                                                           effectiveDate, _
                                                                           dsoServiceTypes)
            If Not msg.Success Then Return msg

            If Not SvcOrder Is Nothing Then
                msg = DomContractBL.FetchServiceTypesAvailableToContract(MyBasePage.DbConnection, SvcOrder.DomContractID, allServiceTypes)
                If Not msg.Success Then Return msg

            End If

            For Each svcType As ViewablePair In dsoServiceTypes
                Dim isDefault As Boolean = (svcType.Text = defaultServiceType())

                msg = CreateSvcTypeControls(svcType, plannedSTList)
                If Not msg.Success Then Return msg

                msg = CreateFinanceCodeTableControls(svcType, isDefault)
                If Not msg.Success Then Return msg
            Next


            For Each svcType As ViewablePair In allServiceTypes
                If Not dsoServiceTypes.Contains(svcType) Then
                    Dim isDefault As Boolean = (svcType.Text = defaultServiceType())

                    msg = CreateSvcTypeControls(svcType, otherSTList)
                    If Not msg.Success Then Return msg

                    msg = CreateFinanceCodeTableControls(svcType, isDefault)
                    If Not msg.Success Then Return msg
                End If
            Next

            System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "anyname", String.Format("fcTable_DefaultServiceType='{0}';", defaultServiceType()), True)
            System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "anyname", "reselectItem();", True)


            msg.Success = True
            Return msg

        End Function

#End Region

#Region " CreateSvcTypeControls "

        Function CreateSvcTypeControls(ByVal svcType As ViewablePair, ByVal list As HtmlGenericControl) As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage

            Dim li As HtmlGenericControl = New HtmlGenericControl("li")
            Dim img As System.Web.UI.WebControls.Image = New System.Web.UI.WebControls.Image
            li.InnerText = svcType.Text
            li.ID = String.Format("imgDefault_{0}", svcType.Text.Replace(" ", "").Replace(")", "").Replace("(", ""))
            li.Attributes.Add("class", "svcTypeItem scvTypeDefault")

            list.Controls.Add(li)

            msg.Success = True
            Return msg

        End Function

#End Region

#Region " CreateFinanceCodeTableControls "

        Function CreateFinanceCodeTableControls(ByVal svcType As ViewablePair, _
                                                ByVal isDefault As Boolean) As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage

            Dim div As HtmlGenericControl = New HtmlGenericControl("div")
            div.ID = String.Format("div_{0}", svcType.Text.Replace(" ", "").Replace(")", "").Replace("(", ""))
            div.Style.Add("width", "100%")
            phFinanceCodes.Controls.Add(div)

            Dim tbl As UserControl = LoadControl("FinanceCodeTable.ascx")
            tbl.ID = String.Format("FinanceCodeTable_{0}", svcType.Text.Replace(" ", "").Replace(")", "").Replace("(", ""))
            div.Controls.Add(tbl)

            CType(tbl, FinanceCodeTable).apportionActualServiceEqually = (optApportion.Checked = True)
            CType(tbl, FinanceCodeTable).ServiceType = svcType.Text
            CType(tbl, FinanceCodeTable).svcTypeID = svcType.Value
            CType(tbl, FinanceCodeTable).IsDefault = isDefault
            CType(tbl, FinanceCodeTable).dataBindTable()

            msg.Success = True
            Return msg

        End Function

#End Region

#Region " Page Events "

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim msg As ErrorMessage = New ErrorMessage
            Dim appSetting As ApplicationSetting = Nothing

            MyBasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("AbacusWeb/Apps/Dom/SvcOrders/UserControls/ServiceOrderFunding.js", _SelectorName)))

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomServiceOrderFunding))

            'check if derive from matrix aplication setting present and configured.
            'msg = FinanceCodeMatrixBL.GetDeriveFromMatrixApplicationSetting(MyBasePage.DbConnection, Nothing, appSetting)
            'If Not msg.Success Then WebUtils.DisplayError(msg)

            'If appSetting.SettingValue = "1" Then
            '    chkUserGenerated.Visible = True
            'Else
            '    chkUserGenerated.Visible = False
            'End If

            If Me.IsPostBack Then
                msg = populateEffectiveDateDropDown()
                If Not msg.Success Then
                    WebUtils.DisplayError(msg)
                End If

                cbofundingEffectiveDate.SelectPostBackValue()

                msg = populateControl(svcOrderID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If RevisionCount > 0 And ButtonClicked <> "New" Then
                    If RevisionID <> cbofundingEffectiveDate.DropDownList.SelectedValue Then
                        differentRevisionSelected = True
                        RevisionID = cbofundingEffectiveDate.DropDownList.SelectedValue
                    End If
                End If

                'because of complications with event order, we have to listen to this session state value
                'to determine if we need to recreate the dymamic created controls.
                If Session("RecreateControls") = True Then
                    msg = createControls()
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    InstructPostbackToRecreateControls(False)
                End If

            End If


        End Sub

#Region "      Page_PreRender "

        Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

            fundingRevisionDropDown.Visible = False
            fundingRevisionDatePicker.Visible = False
            If ButtonClicked = "Edit" Or ButtonClicked = "New" Then
                WebUtils.RecursiveDisable(ServiceOrderFundingContainer.Controls, False)
                btnFundingNew.Visible = False
                btnFundingDelete.Visible = False
                btnFundingEdit.Visible = False
                btnFundingCancel.Visible = True
                btnFundingSave.Visible = True
                fundingRevisionDatePicker.Visible = True

                If ButtonClicked = "Edit" Then
                    'if there is more than one revision and we are editing the most recent 
                    If Not MostRecentRevision Is Nothing AndAlso RevisionCount > 1 AndAlso MostRecentRevision.ID = FundingRevision.ID Then
                        RestrictEffectiveDatesToWeekCommencing()
                    Else
                        fundingEffectiveDate.Enabled = False
                    End If
                Else
                    If RevisionCount = 0 Then
                        fundingEffectiveDate.Enabled = False
                    Else
                        RestrictEffectiveDatesToWeekCommencing()
                    End If
                End If

            ElseIf RevisionCount = 0 Then
                WebUtils.RecursiveDisable(ServiceOrderFundingContainer.Controls, True)
                btnFundingDelete.Visible = False
                btnFundingEdit.Visible = False
                btnFundingCancel.Visible = False
                btnFundingSave.Visible = False
            Else
                WebUtils.RecursiveDisable(ServiceOrderFundingContainer.Controls, True)
                btnFundingNew.Visible = True
                btnFundingDelete.Visible = True
                btnFundingEdit.Visible = True
                btnFundingCancel.Visible = False
                btnFundingSave.Visible = False
                fundingRevisionDropDown.Visible = True
            End If
            WebUtils.RecursiveDisable(svcTypeMenuContainer.Controls, False)
            lblUserGenerated.Enabled = True
            chkUserGenerated.Visible = False

            If chkUserGenerated.Checked Then
                lblUserGenerated.Text = "User Entered Finance Code"
            Else
                lblUserGenerated.Text = "System Generated Finance Code"
            End If

        End Sub

#End Region

#End Region

#Region " loadFundindDataIntoSession "

        Private Sub loadFundindDataIntoSession()
            Dim msg As ErrorMessage = New ErrorMessage
            If Not Me.IsPostBack Or differentRevisionSelected Then
                Session("fundingDetail") = New List(Of ServiceOrderFundingDetail)
                If RevisionCount > 0 Then

                    msg = ServiceOrderFundingBL.FetchServiceOrderFundingDetails(MyBasePage.DbConnection, FundingRevision.ID, svcOrderFundingDetails)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

            End If
        End Sub

#End Region

#Region " loadMostRecentRevisionDataIntoSession "

        Private Sub loadMostRecentRevisionDataIntoSession(ByVal mostRecentRevisionID As Integer)
            Dim msg As ErrorMessage = New ErrorMessage
            Dim details As List(Of ServiceOrderFundingDetail) = Nothing
            Dim newDetails As List(Of ServiceOrderFundingDetail) = New List(Of ServiceOrderFundingDetail)
            Dim newitem As ServiceOrderFundingDetail = Nothing

            If mostRecentRevisionID > 0 Then
                msg = ServiceOrderFundingBL.FetchServiceOrderFundingDetails(MyBasePage.DbConnection, mostRecentRevisionID, details)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            For Each item As ServiceOrderFundingDetail In details
                newitem = item.Clone
                newitem.DSOFundingID = 0
                newitem.DSOFundingDetailID = 0
                newDetails.Add(newitem)
            Next

            Session("fundingDetail") = newDetails

        End Sub

#End Region

#Region " FindDefaultItems "

        Private Function FindDefaultItems(ByVal item As ServiceOrderFundingDetail) As Boolean
            Return (item.UseAsDefault = True)
        End Function

#End Region

#Region " Button Event Logic  "

#Region " btnFundingNew_Click "

        Private Sub btnFundingNew_Click(sender As Object, e As System.EventArgs) Handles btnFundingNew.Click
            Dim msg As ErrorMessage = New ErrorMessage

            ButtonClicked = "New"

            RevisionID = 0
            cbofundingEffectiveDate.DropDownList.SelectedValue = -1
            Session("fundingDetail") = Nothing
            If RevisionCount = 0 Then
                fundingEffectiveDate.Text = SvcOrder.DateFrom.ToShortDateString
                CType(fundingCareManager, InPlaceCareManagerSelector).CareManagerID = SvcOrder.CareManagerID
                CType(fundingTeam, InPlaceTeamSelector).TeamID = SvcOrder.TeamID
                CType(fundingClientGroup, InPlaceClientGroupSelector).ClientGroupID = SvcOrder.ClientGroupID
                CType(fundingClientSubGroup, InPlaceClientSubGroupSelector).ClientSubGroupID = SvcOrder.ClientSubGroupID
                optApportion.Checked = True
            Else
                fundingEffectiveDate.Text = String.Empty
                loadMostRecentRevisionDataIntoSession(MostRecentRevision.ID)
                msg = setFundingType(MostRecentRevision)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If
            msg = createControls()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            InstructPostbackToRecreateControls(True)

        End Sub

#End Region

#Region " btnFundingEdit_Click "

        Private Sub btnFundingEdit_Click(sender As Object, e As System.EventArgs) Handles btnFundingEdit.Click
            Dim msg As ErrorMessage
            ButtonClicked = "Edit"

            chkUserGenerated.Checked = True

            msg = createControls()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = setFundingType(FundingRevision)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            InstructPostbackToRecreateControls(True)
        End Sub

#End Region

#Region " btnFundingDelete_Click "

        Private Sub btnFundingDelete_Click(sender As Object, e As System.EventArgs) Handles btnFundingDelete.Click
            Dim trans As SqlTransaction = Nothing
            Dim msg As ErrorMessage = New ErrorMessage

            Try

                If Not FundingRevision Is Nothing Then

                    trans = SqlHelper.GetTransaction(MyBasePage.DbConnection)

                    If earliestRevisionID = FundingRevision.ID Then
                        'Delete All Revisions
                        msg = ServiceOrderFundingBL.DeleteAllDomServiceOrderFundingRevisions(trans, SvcOrder.ID)
                        If Not msg.Success Then
                            lblfundingError.Text = msg.Message
                            SqlHelper.RollbackTransaction(trans)
                            InstructPostbackToRecreateControls(True)
                            Exit Sub
                        End If
                    Else
                        msg = ServiceOrderFundingBL.DeleteDomServiceOrderFundingRevision(trans, FundingRevision.ID)
                        If Not msg.Success Then
                            lblfundingError.Text = String.Format("Unable to delete the revision with the effective date {0} <br/> {1}", _
                                                                     FundingRevision.EffectiveDate.ToShortDateString, _
                                                                     msg.Message)
                            SqlHelper.RollbackTransaction(trans)
                            InstructPostbackToRecreateControls(True)
                            Exit Sub
                        End If

                    End If

                    trans.Commit()

                End If

                RevisionID = 0

                differentRevisionSelected = True

                msg = populateControl(svcOrderID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                loadFundindDataIntoSession()

                msg = createControls()
                If Not msg.Success Then WebUtils.DisplayError(msg)


            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                WebUtils.DisplayError(msg)
                SqlHelper.RollbackTransaction(trans)
            End Try
        End Sub

#End Region

#Region " btnFundingSave_Click "

        Private Sub btnFundingSave_Click(sender As Object, e As System.EventArgs) Handles btnFundingSave.Click
            Dim msg As ErrorMessage = New ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim newRevisonID As Integer = 0
            Dim fundingRows As List(Of ServiceOrderFundingDetail) = Nothing

            MyBasePage.Validate("fundingSave")

            If MyBasePage.IsValid Then
                Try
                    trans = SqlHelper.GetTransaction(MyBasePage.DbConnection)

                    fundingRows = svcOrderFundingDetails

                    If fundingRows.Count = 0 Then

                        msg = New ErrorMessage
                        msg.Number = ServiceOrderFundingBL.ERR_COULD_NOT_SAVE_DSO_FUNDING
                        msg.Message = String.Format(msg.Message, "<br/>At least one finance code record must be entered.")
                        lblfundingError.Text = msg.Message
                        SqlHelper.RollbackTransaction(trans)
                        InstructPostbackToRecreateControls(True)
                        Exit Sub
                    End If

                    '**** Create the Revision ****
                    Dim revision As DomServiceOrderFundingRevision
                    If FundingRevision Is Nothing Then
                        revision = New DomServiceOrderFundingRevision()
                    Else
                        revision = FundingRevision
                    End If

                    With revision
                        .DomServiceOrderID = SvcOrder.ID
                        .EffectiveDate = fundingEffectiveDate.Text
                        .CareManagerID = Utils.ToInt32(Request.Form(CType(fundingCareManager, InPlaceCareManagerSelector).HiddenFieldUniqueID))
                        .ClientGroupID = Utils.ToInt32(Request.Form(CType(fundingClientGroup, InPlaceClientGroupSelector).HiddenFieldUniqueID))
                        .ClientSubGroupID = Utils.ToInt32(Request.Form(CType(fundingClientSubGroup, InPlaceClientSubGroupSelector).HiddenFieldUniqueID))
                        .TeamID = Utils.ToInt32(Request.Form(CType(fundingTeam, InPlaceTeamSelector).HiddenFieldUniqueID))

                        .UserRevision = True
                        chkUserGenerated.Checked = True
                    End With

                    msg = ServiceOrderFundingBL.SaveDomServiceOrderFundingRevision(trans, revision, newRevisonID)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        InstructPostbackToRecreateControls(True)
                        WebUtils.DisplayError(msg)
                    End If

                    '****** End Saving Revision *******

                    'Make sure the session has been updated with values from the screen, on all tables
                    For Each item As ServiceOrderFundingDetail In fundingRows
                        msg = UpdateSessionDataFromFinanceCodeTables(item.ControlID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    Next

                    fundingRows = svcOrderFundingDetails


                    Dim fundingRecords As List(Of DomServiceOrderFunding)
                    'Get list of service types from data we have entered on the Finance Code tables, these will be placed in the funding object.
                    fundingRecords = (From item In fundingRows _
                                Select New DomServiceOrderFunding With _
                                       {.ID = item.DSOFundingID, .DomServiceTypeID = item.ServiceTypeID, .UseAsDefaultForOtherServiceTypes = item.UseAsDefault}).ToList()
                    'Get a distinct list, as currently the list will be duplicated by the number of rows
                    fundingRecords = fundingRecords.GroupBy(Function(tmpItem As DomServiceOrderFunding) New With {Key tmpItem.DomServiceTypeID}).Select(Function(group) group.First()).ToList()


                    'Need to check if any funding records have been removed (All items removed and now using the default)
                    Dim allCurrentlySavedFunding As DomServiceOrderFundingCollection = Nothing
                    msg = DomServiceOrderFunding.FetchList(trans:=trans, list:=allCurrentlySavedFunding, domServiceOrderFundingRevisionID:=newRevisonID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    For Each savedItem As DomServiceOrderFunding In allCurrentlySavedFunding
                        Dim itemFound As Boolean = False
                        For Each unsavedItem As DomServiceOrderFunding In fundingRecords
                            If savedItem.ID = unsavedItem.ID Then
                                itemFound = True
                                Exit For
                            End If
                        Next
                        If Not itemFound Then
                            'this item must be deleted
                            msg = ServiceOrderFundingBL.DeleteFundingRecord(trans, savedItem.ID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End If
                    Next

                    '**** Save Funding Record(s) ****
                    For Each Funding As DomServiceOrderFunding In fundingRecords
                        If optApportion.Checked Then
                            Funding.Type = 1
                        ElseIf optCallOff.Checked Then
                            Funding.Type = 2
                        End If
                        Funding.DomServiceOrderFundingRevisionID = newRevisonID

                        msg = ServiceOrderFundingBL.SaveDomServiceOrderFunding(trans, Funding, currentUser.ExternalUsername, Funding.ID)
                        If Not msg.Success Then
                            If msg.Number = ServiceOrderFundingBL.ERR_COULD_NOT_SAVE_DSO_FUNDING Or msg.Number = ServiceOrderFundingBL.ERR_COULD_NOT_SAVE_DSO_FUNDING_DETAIL Then
                                lblfundingError.Text = msg.Message
                                trans.Rollback()
                                InstructPostbackToRecreateControls(True)
                                Exit Sub
                            Else
                                trans.Rollback()
                                WebUtils.DisplayError(msg)
                            End If
                        End If

                        msg = ServiceOrderFundingBL.DeleteAllFundingDetailRecords(trans, Funding.ID)
                        If Not msg.Success Then
                            SqlHelper.RollbackTransaction(trans)
                            InstructPostbackToRecreateControls(True)
                            WebUtils.DisplayError(msg)
                        End If

                        Dim DSOFDetails As DomServiceOrderFundingDetailCollection = New DomServiceOrderFundingDetailCollection
                        Dim itemsForSvcType As List(Of ServiceOrderFundingDetail) = GetItemsForServiceTypeID(Funding.DomServiceTypeID)
                        For Each item As ServiceOrderFundingDetail In itemsForSvcType
                            Dim DSOFD As DomServiceOrderFundingDetail = New DomServiceOrderFundingDetail
                            With DSOFD
                                .DomServiceOrderFundingID = Funding.ID

                                Dim expList As ExpenditureAccountCollection = Nothing
                                msg = ExpenditureAccount.FetchList(trans, expList, item.expenditureAccountGroupID, item.financeCodeID)
                                If Not msg.Success Then
                                    SqlHelper.RollbackTransaction(trans)
                                    InstructPostbackToRecreateControls(True)
                                    WebUtils.DisplayError(msg)
                                End If

                                .ExpenditureAccountID = expList(0).ID 'item.expenditureAccountGroupID
                                If Funding.Type = 2 Then 'optCallOff is checked
                                    .Callofforder = item.callOffOrder
                                    .RecieveBalancingAmount = False
                                Else
                                    .Callofforder = 0
                                    .RecieveBalancingAmount = item.balancing
                                End If

                                Select Case item.ExpenditureAccountType
                                    Case "Council"
                                        .ExpenditureAccountType = ExpenditureAccountGroupType.Council
                                    Case "Clinical Commissioning Group"
                                        .ExpenditureAccountType = ExpenditureAccountGroupType.ClinicalCommissioningGroup
                                    Case "Client-specific Third Party"
                                        .ExpenditureAccountType = ExpenditureAccountGroupType.ClientSpecificThirdParty
                                    Case "Other Local Authority (OLA)"
                                        .ExpenditureAccountType = ExpenditureAccountGroupType.OtherLocalAuthority
                                    Case "Other Organisation"
                                        .ExpenditureAccountType = ExpenditureAccountGroupType.Other
                                End Select

                                If item.FundedBy = "Service User" Then
                                    .ServiceUserFunded = True
                                Else
                                    .ServiceUserFunded = False
                                End If

                                '****** New to correct this
                                .Denominator = 100
                                .Numerator = item.proportion

                            End With

                            DSOFDetails.Add(DSOFD)

                        Next
                        'Save the detail records
                        msg = ServiceOrderFundingBL.SaveDomServiceOrderFundingDetails(trans, Funding.ID, DSOFDetails)
                        If Not msg.Success Then
                            If msg.Number = ServiceOrderFundingBL.ERR_COULD_NOT_SAVE_DSO_FUNDING Or msg.Number = ServiceOrderFundingBL.ERR_COULD_NOT_SAVE_DSO_FUNDING_DETAIL Then
                                lblfundingError.Text = msg.Message
                                trans.Rollback()
                                InstructPostbackToRecreateControls(True)
                                Exit Sub
                            Else
                                trans.Rollback()
                                InstructPostbackToRecreateControls(True)
                                WebUtils.DisplayError(msg)
                            End If
                        End If

                    Next

                    msg = ServiceOrderFundingBL.ValidateDomServiceOrderFundingRevision(trans, fundingRecords)
                    If Not msg.Success Then
                        If msg.Number = ServiceOrderFundingBL.ERR_COULD_NOT_SAVE_DSO_FUNDING Then
                            lblfundingError.Text = msg.Message
                            trans.Rollback()
                            InstructPostbackToRecreateControls(True)
                            Exit Sub
                        Else
                            trans.Rollback()
                            InstructPostbackToRecreateControls(True)
                            WebUtils.DisplayError(msg)
                        End If
                    End If

                    trans.Commit()

                    ButtonClicked = ""

                    differentRevisionSelected = True

                    msg = populateEffectiveDateDropDown()
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    DataBindFinanceCodeTables()

                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                    WebUtils.DisplayError(msg)
                    SqlHelper.RollbackTransaction(trans)

                End Try
            Else
                InstructPostbackToRecreateControls(True)
            End If
        End Sub

#End Region

#Region " btnFundingCancel_Click "

        Private Sub btnFundingCancel_Click(sender As Object, e As System.EventArgs) Handles btnFundingCancel.Click
            Dim msg As ErrorMessage
            ButtonClicked = "Cancel"

            RevisionID = 0

            differentRevisionSelected = True

            msg = populateControl(svcOrderID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            loadFundindDataIntoSession()

            msg = createControls()
            If Not msg.Success Then WebUtils.DisplayError(msg)
            ButtonClicked = ""
        End Sub

#End Region

#End Region

#Region " DataBindFinanceCodeTables "

        Private Function DataBindFinanceCodeTables() As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage
            For Each fcDiv As Control In phFinanceCodes.Controls
                For Each ctl As Control In fcDiv.Controls
                    CType(ctl, FinanceCodeTable).dataBindTable()
                Next
            Next

            msg.Success = True
            Return msg

        End Function

#End Region

#Region " UpdateSessionDataFromFinanceCodeTables "

        Private Function UpdateSessionDataFromFinanceCodeTables(ByVal controlID As String) As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage
            For Each fcDiv As Control In phFinanceCodes.Controls
                For Each ctl As Control In fcDiv.Controls
                    If ctl.ClientID = controlID Then
                        msg = CType(ctl, FinanceCodeTable).UpdateSessionState
                        If Not msg.Success Then Return msg
                    End If
                Next
            Next

            msg.Success = True
            Return msg

        End Function

#End Region

#Region " RestrictEffectiveDatesToWeekCommencing "

        Private Sub RestrictEffectiveDatesToWeekCommencing()
            Dim weekCommencingDate As DateTime = DomContractBL.GetWeekCommencingDate(CType(Me.Page, BasePage).DbConnection, Nothing)
            fundingEffectiveDate.AllowableDays = Integer.Parse(weekCommencingDate.DayOfWeek)
        End Sub

#End Region

#Region " GetItemsForServiceTypeID "

        Private Function GetItemsForServiceTypeID(ByVal serviceTypeID As Integer) As List(Of ServiceOrderFundingDetail)
            _saveServiceTypeID = serviceTypeID
            Return svcOrderFundingDetails.FindAll(AddressOf FindForServiceTypeID)
        End Function

#End Region

#Region " FindForServiceTypeID "

        Private Function FindForServiceTypeID(ByVal item As ServiceOrderFundingDetail) As Boolean
            Return (item.ServiceTypeID = _saveServiceTypeID)
        End Function

#End Region

#Region " cbofundingEffectiveDate_SelectedIndexChanged "

        Private Sub cbofundingEffectiveDate_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cbofundingEffectiveDate.SelectedIndexChanged
            Dim msg As ErrorMessage = New ErrorMessage
            differentRevisionSelected = True

            cbofundingEffectiveDate.SelectPostBackValue()
            RevisionID = cbofundingEffectiveDate.DropDownList.SelectedValue

            loadFundindDataIntoSession()
            msg = setFundingType(FundingRevision)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = createControls()
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

#End Region

#Region " InstructPostbackToRecreateControls "

        Private Sub InstructPostbackToRecreateControls(ByVal value As Boolean)
            Session("RecreateControls") = value
        End Sub

#End Region

#Region " GetFundingType "

        Private Function GetFundingType(ByVal revisionID As Integer, ByRef fundingType As Integer) As ErrorMessage
            Dim msg As ErrorMessage
            Dim fundingRecords As DomServiceOrderFundingCollection = Nothing

            msg = DomServiceOrderFunding.FetchList(conn:=Me.MyBasePage.DbConnection, list:=fundingRecords, domServiceOrderFundingRevisionID:=revisionID)
            If Not msg.Success Then Return msg

            If Not fundingRecords Is Nothing AndAlso fundingRecords.Count > 0 Then
                fundingType = fundingRecords(0).Type
            Else
                fundingType = 1
            End If

            msg = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

#End Region

#Region " setFundingType "

        Private Function setFundingType(ByVal revision As DomServiceOrderFundingRevision) As ErrorMessage
            Dim type As Integer
            Dim msg As ErrorMessage
            msg = GetFundingType(revision.ID, type)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If type = 1 Then
                optApportion.Checked = True
                optCallOff.Checked = False
            Else
                optApportion.Checked = False
                optCallOff.Checked = True
            End If

            msg = New ErrorMessage
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " hideButtons "

        Public Function hideButtons(ByVal hide As Boolean) As ErrorMessage
            Dim msg As ErrorMessage

            fldButtons.Visible = (hide = False)

            msg = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

#End Region



    End Class
End Namespace
