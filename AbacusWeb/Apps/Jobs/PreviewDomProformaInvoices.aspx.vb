
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Web.Apps.Security

Namespace Apps.Jobs

    ''' <summary>
    ''' Screen that allows a user to preview the domiciliary proforma invoices that match the criteria
    ''' entered for the create dom provider invoices job.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Paul W      29/06/2010  D11795 - SDS, Generic Contracts and Service Orders
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class PreviewDomProformaInvoices
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.CreateNewJob"), "Preview Proforma Invoices")

            Const SP_NAME As String = "spxDomProformaInvoice_PreviewForCreateProviderInvoice"

            Dim batchTypes As DomProformaInvoiceBatchType = [Enum].Parse(GetType(DomProformaInvoiceBatchType), Utils.ToInt32(Request.QueryString("batchTypes")))
            Dim providerID As Integer = Utils.ToInt32(Request.QueryString("providerID"))
            Dim contractType As DomContractType = [Enum].Parse(GetType(DomContractType), Utils.ToInt32(Request.QueryString("ctID")))
            Dim contractGroupID As Integer = Utils.ToInt32(Request.QueryString("cgID"))
            Dim contractID As Integer = Utils.ToInt32(Request.QueryString("contractID"))
            Dim dataPartitioned As Boolean = Boolean.Parse(Request.QueryString("dataPartitioned"))
            Dim websecurityUserID As Integer = Utils.ToInt32(Request.QueryString("intWebSecurityUserID"))
            Dim css As StringBuilder
            Dim batchTypesDesc As StringBuilder
            Dim msg As ErrorMessage
            Dim provider As Establishment
            Dim contractTypeDesc As String = Nothing
            Dim contractGroup As GenericContractGroup
            Dim contract As DomContract
            Dim spParams As SqlParameter()
            Dim reader As SqlDataReader = Nothing

            css = New StringBuilder()
            With css
                .Append("@media print {	div.header { display:none; } div.main { margin:0em; } }")
                .Append("@media screen { div.header { padding-top:1em; } div.main { margin:0.5em; } }")
            End With
            Me.AddExtraCssStyle(css.ToString())

            ' output filters
            ' batch types
            batchTypesDesc = New StringBuilder()
            For Each t As DomProformaInvoiceBatchType In [Enum].GetValues(GetType(DomProformaInvoiceBatchType))
                If t And batchTypes Then
                    batchTypesDesc.AppendFormat("{0}, ", Utils.SplitOnCapitals([Enum].GetName(GetType(DomProformaInvoiceBatchType), t)))
                End If
            Next
            ' remove trailing ", " chars
            txtBatchTypes.Text = batchTypesDesc.ToString(0, batchTypesDesc.Length - 2)

            ' provider
            If providerID > 0 Then
                provider = New Establishment(Me.DbConnection)
                With provider
                    msg = .Fetch(providerID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    txtProvider.Text = String.Format("{0}/{1}", .AltReference, .Name)
                End With
            End If

            ' contract type
            If contractType <> DomContractType.Unknown Then
                contractTypeDesc = [Enum].GetName(GetType(DomContractType), contractType)
                txtContractType.Text = Utils.SplitOnCapitals(contractTypeDesc)
            End If

            ' contract group
            If contractGroupID > 0 Then
                contractGroup = New GenericContractGroup(Me.DbConnection, String.Empty, String.Empty)
                With contractGroup
                    msg = .Fetch(contractGroupID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    txtContractGroup.Text = .Description
                End With
            End If

            ' contract
            If contractID > 0 Then
                contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                With contract
                    msg = .Fetch(contractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    txtContract.Text = String.Format("{0}/{1}", .Number, .Title)
                End With
            End If

            ' date/time
            txtNow.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")

            ' get the list of invoices
            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                spParams(0).Value = batchTypes
                If providerID > 0 Then spParams(1).Value = providerID
                If Not contractTypeDesc Is Nothing Then spParams(2).Value = contractTypeDesc
                If contractGroupID > 0 Then spParams(3).Value = contractGroupID
                If contractID > 0 Then spParams(4).Value = contractID
                spParams(5).Value = dataPartitioned
                If websecurityUserID > 0 Then spParams(6).Value = websecurityUserID

                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                rptInvoices.DataSource = reader
                rptInvoices.DataBind()

            Catch ex As Exception
                WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
            Finally
                SqlHelper.CloseReader(reader)
            End Try

        End Sub

        Protected Function GetCalculatedPayment(ByVal value As Object) As String
            If Not Convert.IsDBNull(value) Then
                Return Convert.ToDecimal(value).ToString("c")
            Else
                Return String.Empty
            End If
        End Function

    End Class

End Namespace