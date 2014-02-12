
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Data.SqlClient

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Outputs an in-place, budget holder selector.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class InPlaceBudgetHolderSelector
        Inherits System.Web.UI.UserControl

#Region " Private variables & properties "

        Private _budgetHolderID As Integer

        Public Property BudgetHolderID() As Integer
            Get
                Return _budgetHolderID
            End Get
            Set(ByVal value As Integer)
                _budgetHolderID = value
                LoadBudgetHolder()
            End Set
        End Property

        Private _FilterShowRedundant As Nullable(Of Boolean)

        ''' <summary>
        ''' Gets or sets the whether to show redundant records.
        ''' </summary>
        ''' <value>Whether to show redundant records.</value>
        Public Property FilterShowRedundant() As Nullable(Of Boolean)
            Get
                Return _FilterShowRedundant
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                _FilterShowRedundant = value
            End Set
        End Property

        Public Property Required() As Boolean
            Get
                Return valRequired.Visible
            End Get
            Set(ByVal value As Boolean)
                valRequired.Visible = value
            End Set
        End Property

        Public Property ValidationGroup() As String
            Get
                Return valRequired.ValidationGroup
            End Get
            Set(ByVal value As String)
                valRequired.ValidationGroup = value
            End Set
        End Property

        Public ReadOnly Property HiddenFieldUniqueID() As String
            Get
                Return hidID.UniqueID
            End Get
        End Property

        ''' <summary>
        ''' Gets the budget holder description.
        ''' </summary>
        ''' <value>
        ''' The budget holder description.
        ''' </value>
        Public ReadOnly Property BudgetHolderDescription As String
            Get
                Dim description As String = String.Empty
                If BudgetHolderID > 0 Then
                    ' if has an id then construct description
                    description = String.Format("{0}: {1}", txtReference.Value, txtName.Value)
                End If
                Return description
            End Get
        End Property

        Private _FilterServiceUserID As Nullable(Of Integer) = Nothing

        ''' <summary>
        ''' Gets or sets the filter service user ID.
        ''' </summary>
        ''' <value>
        ''' The filter service user ID.
        ''' </value>
        Public Property FilterServiceUserID() As Nullable(Of Integer)
            Get
                Return _FilterServiceUserID
            End Get
            Set(ByVal value As Nullable(Of Integer))
                _FilterServiceUserID = value
            End Set
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_LIBRARY As String = "Library"

            txtReference.Attributes.Add("onchange", String.Format("InPlaceBudgetHolderSelector_ClearStoredID('{0}');", Me.ClientID))
            txtName.Attributes.Add("onchange", String.Format("InPlaceBudgetHolderSelector_ClearStoredID('{0}');", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceBudgetHolderSelector_btnFind_Click('{0}');", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusWeb/Apps/InPlaceSelectors/InPlaceBudgetHolderSelector.js"))
            End If

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Required Then
                With valRequired
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = "Please select a budget holder"
                End With
            End If
        End Sub

        Private Sub LoadBudgetHolder()
            Const SP_NAME As String = "spxThirdPartyBudgetHolder_FetchDetails"

            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If _budgetHolderID > 0 Then
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(thePage.DbConnection, SP_NAME, False)
                Dim reader As SqlDataReader = Nothing
                spParams(0).Value = _budgetHolderID
                reader = SqlHelper.ExecuteReader(thePage.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)
                If reader.HasRows Then
                    While reader.Read()
                        txtReference.Value = reader("Reference")
                        If reader("OrganisationName").ToString.Trim <> "" Then
                            txtName.Value = reader("OrganisationName").ToString.Trim
                        Else
                            txtName.Value = reader("BudgetHolderName").ToString.Trim
                        End If
                    End While
                End If
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then reader.Close()
                hidID.Value = _budgetHolderID
            Else
                txtReference.Value = String.Empty
                txtName.Value = String.Empty
                hidID.Value = String.Empty
            End If
            If FilterShowRedundant IsNot Nothing AndAlso FilterShowRedundant.HasValue Then
                hidRedundant.Value = FilterShowRedundant.Value.ToString().ToLower()
            Else
                hidRedundant.Value = "null"
            End If
            If FilterServiceUserID IsNot Nothing AndAlso FilterServiceUserID.HasValue Then
                hidServiceUser.Value = FilterServiceUserID.Value.ToString().ToLower()
            Else
                hidServiceUser.Value = "null"
            End If
        End Sub

        Public Function GetPostBackValue() As String
            Return Request.Form(hidID.UniqueID)
        End Function

    End Class

End Namespace