
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
Imports Target.Abacus.Library.AdministrativeSector
Imports Target.Web.Apps.Security

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control that provides custom inputs for the process domiciliary visit amendment requests job step.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' PaulW         29/06/2010   D11795 - SDS, Generic Contracts and Service Orders
    ''' </history> 
    Partial Public Class ProcessVisitAmendRequestStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            thePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/Jobs/UserControls/ProcessVisitAmendRequestStepInputs.js"))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_STARTUP As String = "Startup"

            PopulateScreen()

            cboContractType.DropDownList.Attributes.Add("onchange", "cboContractType_Change();")
            cboContractGroup.DropDownList.Attributes.Add("onchange", "cboContractGroup_Change();")

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                    String.Format("Edit_domContractID='{0}';cboContractTypeID='{1}';cboContractGroupID='{2}';chkCreateProformaID='{3}';chkCreateProviderID='{4}';", _
                        domContract.ClientID, cboContractType.ClientID, cboContractGroup.ClientID, chkCreateProforma.ClientID, chkCreateProvider.ClientID), _
                    True _
                )
            End If

        End Sub

        Private Sub PopulateScreen()

            Dim msg As ErrorMessage
            Dim groups As GenericContractGroupCollection = Nothing

            ' provider
            With CType(provider, InPlaceEstablishmentSelector)
                .Required = False
            End With

            ' contract type
            With cboContractType.DropDownList.Items
                .Clear()
                For Each value As DomContractType In [Enum].GetValues(GetType(DomContractType))
                    If value = DomContractType.Unknown Then
                        .Add(New ListItem(String.Empty, Convert.ToInt32(value)))
                    Else
                        .Add(New ListItem(Utils.SplitOnCapitals([Enum].GetName(GetType(DomContractType), value)), Convert.ToInt32(value)))
                    End If
                Next
            End With
            cboContractType.SelectPostBackValue()

            ' contract group
            With cboContractGroup
                ' get a list of non-redundant groups
                msg = GenericContractGroup.FetchList(CType(Me.Page, BasePage).DbConnection, groups, String.Empty, String.Empty, TriState.False)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .DataSource = groups
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty, 0))
                End With
            End With
            cboContractGroup.SelectPostBackValue()

            ' contract
            With CType(domContract, InPlaceDomContractSelector)
                .Required = False
            End With

            ' checkboxes - default to ticked if not already submitted
            If Request.Form(String.Format("{0}$chkCheckBox", chkCreateProforma.UniqueID)) Is Nothing Then
                chkCreateProforma.CheckBox.Checked = True
            End If
            If Request.Form(String.Format("{0}$chkCheckBox", chkCreateProvider.UniqueID)) Is Nothing Then
                chkCreateProvider.CheckBox.Checked = True
            End If
            chkCreateProforma.CheckBox.Attributes.Add("onclick", "chkCreateProforma_Click();")

        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim msg As ErrorMessage
            Dim estab As Establishment
            Dim contractGroup As GenericContractGroup
            Dim contract As DomContract
            Dim providerID As Integer, contractTypeID As Integer, contractGroupID As Integer, contractID As Integer
            Dim value As String
            Dim isPartitionDataOn As Boolean

            ' provider
            providerID = Utils.ToInt32(CType(provider, InPlaceEstablishmentSelector).GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "FilterProviderID", providerID))
            If providerID > 0 Then
                If Not trans Is Nothing Then
                    estab = New Establishment(trans)
                Else
                    estab = New Establishment(conn)
                End If
                msg = estab.Fetch(providerID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterProviderDesc", estab.Name))
            End If

            ' contract type
            contractTypeID = Utils.ToInt32(cboContractType.DropDownList.SelectedValue)
            If contractTypeID > 0 Then
                result.Add(New Triplet(jobStepTypeID, "FilterContractType", _
                                [Enum].GetName(GetType(DomContractType), contractTypeID)))
            End If

            ' contract group
            contractGroupID = Utils.ToInt32(cboContractGroup.DropDownList.SelectedValue)
            result.Add(New Triplet(jobStepTypeID, "FilterContractGroupID", contractGroupID))
            If contractGroupID > 0 Then
                If Not trans Is Nothing Then
                    contractGroup = New GenericContractGroup(trans, String.Empty, String.Empty)
                Else
                    contractGroup = New GenericContractGroup(conn, String.Empty, String.Empty)
                End If
                msg = contractGroup.Fetch(contractGroupID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterContractGroupDesc", contractGroup.Description))
            End If

            ' contract
            contractID = Utils.ToInt32(CType(domContract, InPlaceDomContractSelector).GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "FilterContractID", contractID))
            If contractID > 0 Then
                If Not trans Is Nothing Then
                    contract = New DomContract(trans, String.Empty, String.Empty)
                Else
                    contract = New DomContract(conn, String.Empty, String.Empty)
                End If
                msg = contract.Fetch(contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterContractDesc", String.Format("{0}/{1}", contract.Number, contract.Title)))
            End If

            ' create proforma invoices
            value = Request.Form(String.Format("{0}$chkCheckBox", chkCreateProforma.UniqueID))
            If value Is Nothing Then
                value = Boolean.FalseString
            ElseIf value = "on" Then
                value = Boolean.TrueString
            Else
                value = Boolean.FalseString
            End If
            result.Add(New Triplet(jobStepTypeID, "FilterCreateProforma", value))

            ' create provider invoices
            value = Request.Form(String.Format("{0}$chkCheckBox", chkCreateProvider.UniqueID))
            If value Is Nothing Then
                value = Boolean.FalseString
            ElseIf value = "on" Then
                value = Boolean.TrueString
            Else
                value = Boolean.FalseString
            End If
            result.Add(New Triplet(jobStepTypeID, "FilterCreateProvider", value))

            'data partitioned ?
            msg = AdministrativeAreasBL.IsPartitionDataByAdministrativeAreaOn(trans, conn, isPartitionDataOn)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If isPartitionDataOn Then
                result.Add(New Triplet(jobStepTypeID, "FilterPartitionData", isPartitionDataOn.ToString))
                result.Add(New Triplet(jobStepTypeID, "FilterWebSecurityUserID", SecurityBL.GetCurrentUser.ID))
            End If

            Return result

        End Function

    End Class

End Namespace

