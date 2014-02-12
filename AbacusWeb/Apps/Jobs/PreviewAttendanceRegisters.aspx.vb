
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
    '''     PaulW     06/11/2009  Created.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class PreviewAttendanceRegisters
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.CreateNewJob"), "Preview Domiciliary Proforma Invoices")

            Const SP_NAME As String = "spxRegister_PreviewForCreateProviderInvoice"

            Dim newRegisters As Boolean = Convert.ToBoolean(Request.QueryString("nRegisters"))
            Dim amendedRegisters As Boolean = Convert.ToBoolean(Request.QueryString("aRegisters"))
            Dim providerID As Integer = Utils.ToInt32(Request.QueryString("providerID"))
            Dim contractID As Integer = Utils.ToInt32(Request.QueryString("contractID"))
            Dim css As StringBuilder
            Dim msg As ErrorMessage
            Dim provider As Establishment
            Dim contract As DomContract
            Dim spParams As SqlParameter()
            Dim reader As SqlDataReader = Nothing

            css = New StringBuilder()
            With css
                .Append("@media print {	div.header { display:none; } div.main { margin:0em; } }")
                .Append("@media screen { div.header { padding-top:1em; } div.main { margin:0.5em; } }")
            End With
            Me.AddExtraCssStyle(css.ToString())

            If newRegisters Then
                txtNewRegister.Text = "Yes"
            Else
                txtNewRegister.Text = "No"
            End If

            If amendedRegisters Then
                txtAmendedRegister.Text = "Yes"
            Else
                txtAmendedRegister.Text = "No"
            End If

            ' provider
            If providerID > 0 Then
                provider = New Establishment(Me.DbConnection)
                With provider
                    msg = .Fetch(providerID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    txtProvider.Text = String.Format("{0}/{1}", .AltReference, .Name)
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
                If providerID > 0 Then spParams(0).Value = providerID
                If contractID > 0 Then spParams(1).Value = contractID
                spParams(2).Value = newRegisters
                spParams(3).Value = amendedRegisters

                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)

                rptRegisters.DataSource = reader
                rptRegisters.DataBind()

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