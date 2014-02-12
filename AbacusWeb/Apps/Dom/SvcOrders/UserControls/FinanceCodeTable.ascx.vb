Imports Target.Abacus.Library
Imports Target.Web.Apps
Imports Target.Library
Imports Target.Abacus.Library.ServiceOrder
Imports System.Collections.Generic
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Web.Apps.InPlaceSelectors

Namespace Apps.Dom.SvcOrders
    Partial Public Class FinanceCodeTable
        Inherits System.Web.UI.UserControl

#Region " Variables "
        ' consts
        Private _SelectorName As String = "FinanaceCodeTable"
        Private _apportionActualServiceEqually As Boolean
        Private _fundingDetail As List(Of ServiceOrderFundingDetail) = New List(Of ServiceOrderFundingDetail)
        Private _serviceType As String
        Private _svcTypeID As Integer
        Private _isDefault As Boolean
        Private _sessionStateName As String = "Temp" 'this is just a safeguard, a blank sessionStateName will break the session database.
        Public Event RowAdded()

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

        Public Property sessionStateName() As String
            Get
                Return _sessionStateName
            End Get
            Set(ByVal value As String)
                _sessionStateName = value
            End Set
        End Property


        Public Property GetAllFundingDetail() As List(Of ServiceOrderFundingDetail)
            Get
                Return IIf(Session("fundingDetail") IsNot Nothing, Session("fundingDetail"), Nothing)
            End Get
            Set(ByVal value As List(Of ServiceOrderFundingDetail))
                Session("fundingDetail") = value
            End Set
        End Property

        Public Property FundingDetails() As List(Of ServiceOrderFundingDetail)
            Get
                Dim allDetails As List(Of ServiceOrderFundingDetail) = GetAllFundingDetail
                Dim details As List(Of ServiceOrderFundingDetail) = Nothing
                If Not allDetails Is Nothing Then
                    details = allDetails.FindAll(AddressOf FindForServiceType)

                    For Each item As ServiceOrderFundingDetail In details
                        item.ControlID = Me.ClientID
                    Next

                    If details Is Nothing OrElse details.Count = 0 Then
                        details = allDetails.FindAll(AddressOf FindDefaultItems)
                    End If
                End If
                Return details
            End Get
            Set(ByVal value As List(Of ServiceOrderFundingDetail))

                Dim details As List(Of ServiceOrderFundingDetail) = GetAllFundingDetail
                details = details.FindAll(AddressOf FindNonMatchingItems)
                details.AddRange(value)
                Session("fundingDetail") = details
            End Set

        End Property

        Public Property ServiceType() As String
            Get
                Return _serviceType
            End Get
            Set(ByVal value As String)
                _serviceType = value
                sessionStateName = String.Format("tableData{0}", value)
            End Set
        End Property

        Public Property svcTypeID() As Integer
            Get
                Return _svcTypeID
            End Get
            Set(ByVal value As Integer)
                _svcTypeID = value
            End Set
        End Property


        Public Property apportionActualServiceEqually() As Boolean
            Get
                If hidFinCodeApportionEqually.Value = String.Empty Then
                    Return False
                Else
                    Return Utils.ToBoolean(hidFinCodeApportionEqually.Value) '_apportionActualServiceEqually
                End If

            End Get
            Set(ByVal value As Boolean)
                hidFinCodeApportionEqually.Value = value.ToString
            End Set
        End Property

        Public Property IsDefault() As Boolean
            Get
                Return _isDefault
            End Get
            Set(ByVal value As Boolean)
                _isDefault = value
            End Set
        End Property

        Private _createNewFromDefaults As Boolean
        Public Property createNewFromDefaults() As Boolean
            Get
                Return _createNewFromDefaults
            End Get
            Set(ByVal value As Boolean)
                _createNewFromDefaults = value
            End Set
        End Property

#End Region

#Region " Page Events "

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ' add the web service type
            AjaxPro.Utility.RegisterTypeForAjax(GetType(ServiceOrder.ServiceOrderService))

            MyBasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("AbacusWeb/Apps/Dom/SvcOrders/UserControls/FinanceCodeTable.js", _SelectorName)))

            dataBindTable()

        End Sub

#End Region

#End Region

#Region " dataBindTable "

        Public Sub dataBindTable()
            Dim details As List(Of ServiceOrderFundingDetail) = FundingDetails

            gvFinanceCodes.DataSource = details
            gvFinanceCodes.DataBind()
        End Sub

#End Region

#Region " btnAdd_Click "

        Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
            Dim index As Integer = 0
            Dim msg As ErrorMessage
            Dim details As List(Of ServiceOrderFundingDetail) = Nothing
            Dim newitem As ServiceOrderFundingDetail = Nothing
            Dim addRow As Boolean = True

            If FundingDetails Is Nothing Then
                Session("fundingDetail") = New List(Of ServiceOrderFundingDetail)
                IsDefault = True

                Dim newItems As List(Of ServiceOrderFundingDetail) = New List(Of ServiceOrderFundingDetail)

                newitem = New ServiceOrderFundingDetail
                newitem.ServiceType = ServiceType
                newitem.UseAsDefault = IsDefault
                newitem.rowIdentifier = FundingDetails.Count

                If FundingDetails.Count = 0 Then
                    newitem.balancing = True
                End If
                newItems.Add(newitem)
                FundingDetails = newItems

                gvFinanceCodes.DataSource = FundingDetails
                gvFinanceCodes.DataBind()

                addRow = False
            End If

            msg = GetFundingDetailsForCurrentSvcType(details)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            'If we are adding a row to a service type that currently only uses the default items, 
            'we need to take a copy of the default items and add these to the collection.
            If details Is Nothing OrElse details.Count = 0 Then
                createNewFromDefaults = True

            End If

            msg = constructServiceDetailFromTableRows()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = GetFundingDetailsForCurrentSvcType(details)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            createNewFromDefaults = False

            If addRow Then
                newitem = New ServiceOrderFundingDetail
                newitem.ServiceType = ServiceType
                newitem.UseAsDefault = IsDefault
                newitem.rowIdentifier = FundingDetails.Count

                If FundingDetails.Count = 0 Then
                    newitem.balancing = True
                End If
                details.Add(newitem)
                FundingDetails = details
            End If

            For Each item As ServiceOrderFundingDetail In FundingDetails
                item.proportion = Math.Round(100 / FundingDetails.Count, 2)
                item.rowIdentifier = index
                index += 1
            Next

            gvFinanceCodes.DataSource = FundingDetails
            gvFinanceCodes.DataBind()

            System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "anyname", "reselectItem();", True)

        End Sub

#End Region

#Region " DeleteRow "

        Protected Sub DeleteRow(sender As Object, e As CommandEventArgs)
            Dim msg As ErrorMessage = Nothing
            Dim details As List(Of ServiceOrderFundingDetail) = Nothing
            Dim newitem As ServiceOrderFundingDetail = Nothing

            'msg = constructServiceDetailFromTableRows()
            'If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = GetFundingDetailsForCurrentSvcType(details)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If details Is Nothing OrElse details.Count = 0 Then
                Dim defaultDetails As List(Of ServiceOrderFundingDetail) = GetAllFundingDetail.FindAll(AddressOf FindDefaultItems)
                details = New List(Of ServiceOrderFundingDetail)()
                For Each item As ServiceOrderFundingDetail In defaultDetails
                    newitem = item.Clone
                    newitem.ServiceType = ServiceType
                    newitem.UseAsDefault = False
                    newitem.DSOFundingID = 0
                    newitem.DSOFundingDetailID = 0
                    details.Add(newitem)
                Next
            End If

            details.Remove(details(e.CommandArgument))

            For Each item As ServiceOrderFundingDetail In details
                item.proportion = Math.Round(100 / details.Count, 2)
            Next

            gvFinanceCodes.DataSource = details
            gvFinanceCodes.DataBind()

            msg = constructServiceDetailFromTableRows()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            gvFinanceCodes.DataSource = FundingDetails
            gvFinanceCodes.DataBind()

            Session("RecreateControls") = True

            System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "anyname", "reselectItem();", True)

        End Sub

#End Region

#Region " constructServiceDetailFromTableRows "

        Private Function constructServiceDetailFromTableRows() As ErrorMessage
            Dim msg As ErrorMessage = Nothing
            Dim fundDetails As List(Of ServiceOrderFundingDetail) = New List(Of ServiceOrderFundingDetail)


            For Each row As GridViewRow In gvFinanceCodes.Rows
                Dim item As ServiceOrderFundingDetail = New ServiceOrderFundingDetail
                Dim txtProp As Label = row.FindControl("txtProportion")
                Dim optBalancing As RadioButton = row.FindControl("optBalancing")
                Dim ddlCallOff As DropDownList = row.FindControl("ddlCallOff")
                Dim ddlFundedBy As DropDownList = row.FindControl("ddlFundedBy")
                Dim expAccountType As HtmlInputControl = row.FindControl("hidExpAccType")
                Dim DSOFunding_ID As HtmlInputControl = row.FindControl("hidDSOFundingID")
                Dim DSOFundingDetail_ID As HtmlInputControl = row.FindControl("hidDSOFundingDetailID")
                Dim hidBalancing As HtmlInputControl = row.FindControl("hidoptBalancing")

                With item
                    .ControlID = Me.ClientID

                    .ServiceType = ServiceType
                    .ServiceTypeID = svcTypeID
                    .balancing = optBalancing.Checked  'hidBalancing.Value
                    .proportion = Convert.ToDecimal(txtProp.Text.Replace("%", ""))
                    .callOffOrder = Utils.ToInt32(ddlCallOff.Text)
                    .ExpenditureAccountType = expAccountType.Value
                    .expenditureAccountGroupID = CType(row.FindControl("expenditureAccount"), InPlaceSelectors.InPlaceExpenditureAccountSelector).ExpenditureAccountGroupID
                    .financeCodeID = CType(row.FindControl("financeCode"), InPlaceSelectors.InPlaceFinanceCodeSelector).FinanceCodeID
                    .financeCodeText = CType(row.FindControl("financeCode"), InPlaceSelectors.InPlaceFinanceCodeSelector).FinanceCodeText
                    .FundedBy = ddlFundedBy.Text
                    .rowIdentifier = row.RowIndex

                    If createNewFromDefaults Then
                        .UseAsDefault = False
                        .DSOFundingID = 0
                        .DSOFundingDetailID = 0
                    Else
                        .UseAsDefault = IsDefault
                        If DSOFunding_ID.Value = String.Empty Then
                            .DSOFundingID = 0
                        Else
                            .DSOFundingID = DSOFunding_ID.Value
                        End If
                        If DSOFundingDetail_ID.Value = String.Empty Then
                            .DSOFundingDetailID = 0
                        Else
                            .DSOFundingDetailID = DSOFundingDetail_ID.Value
                        End If

                    End If
                End With
                fundDetails.Add(item)
            Next

            FundingDetails = fundDetails

            msg = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

#End Region

#Region " gvFinanceCodes_RowDataBound "

        Private Sub gvFinanceCodes_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvFinanceCodes.RowDataBound
            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim itemList As List(Of ListItem) = New List(Of ListItem)
                itemList.Add(New ListItem(" ", 0))
                For index As Integer = 1 To FundingDetails.Count
                    itemList.Add(New ListItem(index.ToString, index))
                Next

                Dim ddlCallOff As DropDownList = e.Row.FindControl("ddlCallOff")
                ddlCallOff.DataSource = itemList
                ddlCallOff.DataTextField = "Text"
                ddlCallOff.DataValueField = "Value"
                ddlCallOff.DataBind()
                Dim value As String = DirectCast(DirectCast(e.Row.DataItem, System.Object), Target.Abacus.Library.ServiceOrder.ServiceOrderFundingDetail).callOffOrder.ToString
                ddlCallOff.SelectedValue = value

                Dim hidExpAccType As HtmlInputText = e.Row.FindControl("hidExpAccType")
                Dim ddlFundedBy As DropDownList = e.Row.FindControl("ddlFundedBy")
                Dim msg As ErrorMessage = Nothing
                Dim expAccGroup As ExpenditureAccountGroup = New ExpenditureAccountGroup(MyBasePage.DbConnection)
                Dim expAccGroupID As Integer = DirectCast(DirectCast(e.Row.DataItem, System.Object), Target.Abacus.Library.ServiceOrder.ServiceOrderFundingDetail).expenditureAccountGroupID


                CType(e.Row.FindControl("financeCode"), InPlaceFinanceCodeSelector).ExpenditureAccountGroupID = expAccGroupID

                If expAccGroupID <> 0 Then
                    msg = expAccGroup.Fetch(expAccGroupID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ddlFundedBy.Items.Clear()
                    Select Case expAccGroup.Type
                        Case ExpenditureAccountGroupType.Council
                            ddlFundedBy.Items.Add("Service User")
                        Case ExpenditureAccountGroupType.ClinicalCommissioningGroup
                            ddlFundedBy.Items.Add("CCG")
                        Case ExpenditureAccountGroupType.ClientSpecificThirdParty
                            ddlFundedBy.Items.Add("Third Party")
                        Case ExpenditureAccountGroupType.OtherLocalAuthority
                            ddlFundedBy.Items.Add("OLA")
                        Case Else
                            ddlFundedBy.Items.Add("Other Organisation")
                            ddlFundedBy.Items.Add("Service User")
                    End Select

                    ddlFundedBy.Text = DirectCast(DirectCast(e.Row.DataItem, System.Object), Target.Abacus.Library.ServiceOrder.ServiceOrderFundingDetail).FundedBy
                End If

            End If
        End Sub

#End Region

#Region " enableBalancing "

        Protected Function enableBalancing() As Boolean
            Return (apportionActualServiceEqually = True)
        End Function

#End Region

#Region " enableCallOff "

        Protected Function enableCallOff() As Boolean
            Return (apportionActualServiceEqually = False)
        End Function

#End Region

#Region " enableDefaultButton "

        Protected Function enableDefaultButton() As Boolean
            Return (FundingDetails.Count > 1)
        End Function

#End Region

#Region " GetProportionText "

        Protected Function GetProportionText(ByVal Proportion As Decimal) As String
            Return String.Format("{0}%", Proportion.ToString)
        End Function

#End Region

#Region " FindForServiceType "

        Private Function FindForServiceType(ByVal item As ServiceOrderFundingDetail) As Boolean
            Return (item.ServiceType = ServiceType)
        End Function

#End Region

#Region " FindNonMatchingItems "

        Private Function FindNonMatchingItems(ByVal item As ServiceOrderFundingDetail) As Boolean
            Return (item.ServiceType <> ServiceType)
        End Function

#End Region

#Region " FindDefaultItems "

        Private Function FindDefaultItems(ByVal item As ServiceOrderFundingDetail) As Boolean
            Return (item.UseAsDefault = True)
        End Function

#End Region

#Region " btnSetDefault_Click "

        Private Sub btnSetDefault_Click(sender As Object, e As System.EventArgs) Handles btnSetDefault.Click
            Dim msg As ErrorMessage = Nothing

            msg = constructServiceDetailFromTableRows()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            gvFinanceCodes.DataSource = FundingDetails
            gvFinanceCodes.DataBind()

            System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "anyname", "SetSvcTypeAsDefault();", True)
        End Sub

#End Region

#Region " btnAmendProportions_Click "

        Private Sub btnAmendProportions_Click(sender As Object, e As System.EventArgs) Handles btnAmendProportions.Click
            Dim msg As ErrorMessage = Nothing

            msg = constructServiceDetailFromTableRows()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            gvFinanceCodes.DataSource = FundingDetails
            gvFinanceCodes.DataBind()

            System.Web.UI.ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "anyname", "ShowProportions();", True)
        End Sub

#End Region

#Region " UpdateSessionState "

        Public Function UpdateSessionState() As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage

            msg = constructServiceDetailFromTableRows()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg.Success = True
            Return msg
        End Function

#End Region

#Region " GetFundingDetailsForCurrentSvcType "

        Public Function GetFundingDetailsForCurrentSvcType(ByRef details As List(Of ServiceOrderFundingDetail)) As ErrorMessage
            Dim msg As ErrorMessage = New ErrorMessage

            details = GetAllFundingDetail.FindAll(AddressOf FindForServiceType)

            msg.Success = True
            Return msg
        End Function

#End Region

        Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
            If FundingDetails Is Nothing OrElse FundingDetails.Count = 0 Then
                btnSetDefault.Enabled = False
                btnAmendProportions.Enabled = False
            End If
        End Sub
    End Class
End Namespace