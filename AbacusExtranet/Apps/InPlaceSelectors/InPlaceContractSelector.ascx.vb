Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports System.Text

Namespace Apps.InPlaceSelectors
    Partial Public Class InPlaceContractSelector
        Inherits System.Web.UI.UserControl

        Private _requiredErrMsg As String = "Select contract"
        Private _enabled As Boolean = True

        Public Property ContractID() As String
            Get
                Return txtHidselectedDomContractId.Value
            End Get
            Set(ByVal value As String)
                txtHidselectedDomContractId.Value = value
                LoadContract()
            End Set
        End Property

        Public Property SelectedContractID() As String
            Get
                Return txtHidselectedDomContractId.Value
            End Get
            Set(ByVal value As String)
                txtHidselectedDomContractId.Value = value
                LoadContract()
            End Set
        End Property

        Public Property SelectedContractTitle() As String
            Get
                Return txtContractTitle.Value
            End Get
            Set(ByVal value As String)
                txtContractTitle.Value = value
            End Set
        End Property

        Public Property SelectedContractNumber() As String
            Get
                Return txtContractNumber.Value
            End Get
            Set(ByVal value As String)
                txtContractNumber.Value = value
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

        Public Property Enabled() As Boolean
            Get
                Return _enabled
            End Get
            Set(ByVal value As Boolean)
                _enabled = value
                WebUtils.RecursiveDisable(Me.Controls, Not _enabled)
            End Set
        End Property

        Public WriteOnly Property TxtContractNumber_Enabled() As Boolean
            Set(ByVal value As Boolean)
                txtContractNumber.Disabled = Not value
            End Set
        End Property


        Public WriteOnly Property TxtContractTitle_Enabled() As Boolean
            Set(ByVal value As Boolean)
                txtContractTitle.Disabled = Not value
            End Set
        End Property

        Public WriteOnly Property btnFind_Enabled() As Boolean
            Set(ByVal value As Boolean)
                btnFind.Disabled = Not value
            End Set
        End Property

        Public WriteOnly Property ContractSelector_Enabled() As Boolean
            Set(ByVal value As Boolean)
                txtContractNumber.Disabled = Not value
                txtContractTitle.Disabled = Not value
                btnFind.Disabled = Not value
            End Set
        End Property


        Private Sub LoadContract()
            Dim msg As ErrorMessage
            Dim contract As DomContract
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            If Me.SelectedContractID > 0 Then
                contract = New DomContract(thePage.DbConnection, "", "")
                msg = contract.Fetch(Me.SelectedContractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtContractNumber.Value = contract.Number
                txtContractTitle.Value = contract.Title
                txtHidselectedDomContractId.Value = Me.SelectedContractID
                If contract.DomRateFrameworkID > 0 Then
                    ' if we have a rate framework

                    Dim rateFramework As New DomRateFramework(auditUserName:=String.Empty, auditLogTitle:=String.Empty, conn:=thePage.DbConnection)
                    Dim rateframeworkType As New FrameworkType(auditUserName:=String.Empty, auditLogTitle:=String.Empty, conn:=thePage.DbConnection)

                    ' get the framework
                    msg = rateFramework.Fetch(contract.DomRateFrameworkID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' get the framework type
                    msg = rateframeworkType.Fetch(rateFramework.FrameworkTypeId)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' set the abbr
                    txtHidFrameworkTypeAbbr.Value = rateframeworkType.Abbreviation

                End If
            Else
                txtContractNumber.Value = String.Empty
                txtContractTitle.Value = String.Empty
                txtHidselectedDomContractId.Value = String.Empty
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_LIBRARY As String = "Library"

            txtContractTitle.Attributes.Add("onchange", String.Format("InPlaceDomContractSelector_ClearStoredID(""{0}"");;", Me.ClientID))
            txtContractNumber.Attributes.Add("onchange", String.Format("InPlaceDomContractSelector_ClearStoredID(""{0}"");", Me.ClientID))
            btnFind.Attributes.Add("onclick", String.Format("InPlaceDomContractSelector_btnFind_Click(""{0}"");", Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), SCRIPT_LIBRARY) Then
                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), SCRIPT_LIBRARY, WebUtils.GetVirtualPath("AbacusExtranet/Apps/InPlaceSelectors/InPlaceContractSelector.js"))
            End If

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim js As New StringBuilder()

            ' setup js to be output to client
            js.AppendFormat("InPlaceDomContractSelector_Init(""{0}"");", Me.ClientID)

            ' output js to client using an unique key for this control
            Page.ClientScript.RegisterStartupScript(Me.GetType(), String.Format("InPlaceDomContractSelector_{0}", Me.ClientID), js.ToString(), True)

            If Me.Required Then
                With valRequired
                    .ControlToValidate = txtContractNumber.ID
                    .ControlToValidate = txtContractTitle.ID
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = _requiredErrMsg
                End With
            End If
            txtContractNumber.Disabled = Not Me.Enabled
            txtContractTitle.Disabled = Not Me.Enabled
            btnFind.Disabled = Not Me.Enabled

        End Sub

    End Class

End Namespace