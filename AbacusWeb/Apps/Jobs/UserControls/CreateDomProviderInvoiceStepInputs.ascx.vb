
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
    ''' User control that provides custom inputs for the create domiciliary provider invoice job step.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF      15/09/2011   D12142A - Added/repositioned "Payment Request" batch type
    ''' PaulW      29/06/2010   D11795 - SDS, Generic Contracts and Service Orders
    '''</history>
    Partial Public Class CreateDomProviderInvoiceStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Const CTRL_PREFIX_BATCH_TYPE As String = "chkBatchType"

        Private _batchTypeCheckboxes As List(Of CheckBoxEx)

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            thePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/Jobs/UserControls/CreateDomProviderInvoiceStepInputs.js"))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_STARTUP As String = "Startup"

            Dim msg As ErrorMessage = Nothing
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Dim script As StringBuilder
            Dim scriptString As String
            Dim dataPartitioned As Boolean

            'data partitioned ?
            msg = AdministrativeAreasBL.IsPartitionDataByAdministrativeAreaOn(Nothing, CType(Me.Page, BasePage).DbConnection, dataPartitioned)
            If Not Msg.Success Then WebUtils.DisplayError(Msg)

            thePage.AddExtraCssStyle(".chkBoxStyle { float:left; margin-right:2em; }")

            PopulateScreen()

            cboContractType.DropDownList.Attributes.Add("onchange", "cboContractType_Change();")
            cboContractGroup.DropDownList.Attributes.Add("onchange", "cboContractGroup_Change();")

            ' output array of batch type checkbox IDs
            script = New StringBuilder()
            For Each chk As CheckBoxEx In _batchTypeCheckboxes
                script.AppendFormat("'{0}',", chk.ClientID)
            Next
            scriptString = script.ToString()
            If scriptString.Length > 0 Then scriptString = scriptString.Substring(0, scriptString.Length - 1)
            Me.Page.ClientScript.RegisterArrayDeclaration("batchTypeIDs", scriptString)

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                    String.Format("Edit_domContractID='{0}';cboContractTypeID='{1}';cboContractGroupID='{2}';dataPartitioned={3};intWebSecurityUserID={4};", _
                        domContract.ClientID, cboContractType.ClientID, cboContractGroup.ClientID, dataPartitioned.ToString.ToLower, SecurityBL.GetCurrentUser.ID), _
                    True _
                )
            End If

        End Sub

        Private Sub PopulateScreen()

            Dim fs As HtmlGenericControl, legend As HtmlGenericControl
            Dim msg As ErrorMessage
            Dim groups As GenericContractGroupCollection = Nothing

            ' batch type
            fs = New HtmlGenericControl("FIELDSET")
            legend = New HtmlGenericControl("LEGEND")
            legend.InnerText = "Batch Type"
            fs.Controls.Add(legend)
            _batchTypeCheckboxes = New List(Of CheckBoxEx)
            For Each t As DomProformaInvoiceBatchType In [Enum].GetValues(GetType(DomProformaInvoiceBatchType))
                Dim chk As CheckBoxEx = New CheckBoxEx()
                With chk
                    .ID = String.Format("{0}{1}", CTRL_PREFIX_BATCH_TYPE, Convert.ToInt32(t))
                    .Text = Utils.SplitOnCapitals([Enum].GetName(GetType(DomProformaInvoiceBatchType), t))
                    .CheckBoxCssClass = "chkBoxStyle"
                    ' only prime the checkbox if we haven't just submitted the values
                    If Request.Form(hidCreatingJob.UniqueID) Is Nothing Then .CheckBox.Checked = True
                    _batchTypeCheckboxes.Add(chk)
                    .CheckBox.TextAlign = TextAlign.Right
                End With
                If chk.Text = "Payment Request" Then
                    '++ This checkbox must appear out of sequence with the normal flow of the 
                    '++ governing enum, so insert it at a specific position..
                    fs.Controls.AddAt(3, chk)
                Else
                    fs.Controls.Add(chk)
                End If
            Next
            '++ Also, the checkboxes then need to occupy two rows due to word-wrapping..
            fs.Controls.AddAt(6, New LiteralControl("<br/>"))
            fs.Controls.AddAt(6, New LiteralControl("<br/>"))
            phBatchType.Controls.Add(fs)

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

        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim batchTypes As DomProformaInvoiceBatchType
            Dim batchTypesDesc As StringBuilder = New StringBuilder()
            Dim msg As ErrorMessage
            Dim estab As Establishment
            Dim contractGroup As GenericContractGroup
            Dim contract As DomContract
            Dim providerID As Integer, contractTypeID As Integer, contractGroupID As Integer, contractID As Integer
            Dim value As String
            Dim isPartitionDataOn As Boolean

            ' batch types
            For Each t As DomProformaInvoiceBatchType In [Enum].GetValues(GetType(DomProformaInvoiceBatchType))
                Dim id As String = String.Format("{0}{1}", CTRL_PREFIX_BATCH_TYPE, Convert.ToInt32(t))
                Dim chkBatchType As CheckBoxEx = phBatchType.FindControl(id)
                value = Request.Form(String.Format("{0}$chkCheckBox", chkBatchType.UniqueID))
                If Not value Is Nothing Then
                    batchTypes += Convert.ToInt32(id.Replace(CTRL_PREFIX_BATCH_TYPE, String.Empty))
                    batchTypesDesc.AppendFormat("{0}, ", Utils.SplitOnCapitals([Enum].GetName(GetType(DomProformaInvoiceBatchType), t)))
                End If
            Next
            result.Add(New Triplet(jobStepTypeID, "FilterBatchTypeID", batchTypes))
            result.Add(New Triplet(jobStepTypeID, "FilterBatchTypeDesc", batchTypesDesc.ToString(0, batchTypesDesc.Length - 1)))

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

