
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control that provides custom inputs for the recalculate domiciliary provider invoice job step.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class ReconsiderDPContractPaymentStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            thePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/Jobs/UserControls/ReconsiderDPContractPaymentStepInputs.js"))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Const SCRIPT_STARTUP As String = "Startup"
            Const UNIT_OF_SERVICE As Integer = 2

            Dim msg As New ErrorMessage()
            Dim budgetCats As BudgetCategoryCollection = Nothing

            '++ Client..
            With CType(ipClient, InPlaceClientSelector)
                .Required = False
            End With

            '++ Budget Holder..
            With CType(ipBudgetHolder, InPlaceBudgetHolderSelector)
                .Required = False
            End With

            '++ Budget Category..
            With cboBudgetCategory
                '++ Get a list of budget categories..
                msg = BudgetCategory.FetchList(CType(Me.Page, BasePage).DbConnection, budgetCats, String.Empty, String.Empty, TriState.False)
                If Not Msg.Success Then WebUtils.DisplayError(Msg)
                With .DropDownList
                    .Items.Clear()
                    For Each bc As BudgetCategory In budgetCats
                        If bc.Type = UNIT_OF_SERVICE Then
                            .Items.Add(New ListItem(bc.Description, bc.ID))
                        End If
                    Next
                    .Items.Insert(0, New ListItem(String.Empty, 0))
                End With
            End With
            cboBudgetCategory.SelectPostBackValue()

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                    String.Format("selClientID='{0}';selBudHolderID='{1}';selBudCategoryID='{2}';", _
                        ipClient.ClientID, ipBudgetHolder.ClientID, cboBudgetCategory.ClientID), _
                    True _
                )
            End If

        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim msg As ErrorMessage
            Dim client As ClientDetail
            Dim tpbh As ThirdPartyBudgetHolder
            Dim budCat As BudgetCategory
            Dim svcUserID As Integer, tpbhID As Integer
            Dim budCatID As Integer
            Dim value As String

            '++ Client..
            svcUserID = Utils.ToInt32(CType(ipClient, InPlaceClientSelector).GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "FilterServiceUserID", svcUserID))
            If svcUserID > 0 Then
                If Not trans Is Nothing Then
                    client = New ClientDetail(trans, String.Empty, String.Empty)
                Else
                    client = New ClientDetail(conn, String.Empty, String.Empty)
                End If
                msg = client.Fetch(svcUserID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterServiceUserDesc", String.Format("({0}) {1}", client.Reference, client.Name)))
            End If

            '++ (Third Party) Budget Holder..
            tpbhID = Utils.ToInt32(CType(ipBudgetHolder, InPlaceBudgetHolderSelector).GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "FilterBudgetHolderID", tpbhID))
            If tpbhID > 0 Then
                If Not trans Is Nothing Then
                    tpbh = New ThirdPartyBudgetHolder(trans, String.Empty, String.Empty)
                Else
                    tpbh = New ThirdPartyBudgetHolder(conn, String.Empty, String.Empty)
                End If
                msg = tpbh.Fetch(tpbhID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                Dim tpbhName As String
                tpbhName = String.Format(tpbh.TitleAndInitials, " ", tpbh.Surname).Trim
                If tpbhName <> "" Then
                    If tpbh.OrganisationName.Trim <> "" Then
                        tpbhName = String.Format("{0} - {1}", tpbhName, tpbh.OrganisationName)
                    End If
                Else
                    tpbhName = tpbh.OrganisationName
                End If
                result.Add(New Triplet(jobStepTypeID, "FilterBudgetHolderDesc", String.Format("({0}) {1}", tpbh.Reference, tpbhName)))
            End If

            '++ Budget Category..
            budCatID = Utils.ToInt32(cboBudgetCategory.DropDownList.SelectedValue)
            result.Add(New Triplet(jobStepTypeID, "FilterBudgetCategoryID", budCatID))
            If budCatID > 0 Then
                If Not trans Is Nothing Then
                    budCat = New BudgetCategory(trans, String.Empty, String.Empty)
                Else
                    budCat = New BudgetCategory(conn, String.Empty, String.Empty)
                End If
                msg = budCat.Fetch(budCatID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                result.Add(New Triplet(jobStepTypeID, "FilterBudgetCategoryDesc", String.Format("({0}) {1}", budCat.Reference, budCat.Description)))
            End If

            '++ Date From..
            value = Request.Form(String.Format("{0}$txtTextBox", dteDateFrom.UniqueID))
            If Not value Is Nothing AndAlso value.Trim().Length > 0 Then
                result.Add(New Triplet(jobStepTypeID, "FilterDateFrom", value))
            End If

            '++ Date To..
            value = Request.Form(String.Format("{0}$txtTextBox", dteDateTo.UniqueID))
            If Not value Is Nothing AndAlso value.Trim().Length > 0 Then
                result.Add(New Triplet(jobStepTypeID, "FilterDateTo", value))
            End If

            Return result

        End Function

    End Class

End Namespace

