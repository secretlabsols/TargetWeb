
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security
Imports System.Text
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Data.SqlClient

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Main container screen used to terminate domiciliary contracts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF   22/09/2010   Ensure end reason are of usage Dom Contracts only (D11801)
    ''' PW   08/01/2008  D11470 - Initial version.
    ''' </history>
    Partial Class Terminate
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryContractsTerminate"), "Terminate Domiciliary Contract")

            Dim contractID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim builder As Target.Library.Web.UriBuilder = New Target.Library.Web.UriBuilder(Request.Url)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim style As New StringBuilder
            Dim contract As DomContract = Nothing
            Dim provider As Establishment = Nothing
            Dim client As ClientDetail = Nothing
            Dim contractEndReason As ContractEndReason = Nothing
            Dim reasons As ContractEndReasonCollection = Nothing
            Dim msg As ErrorMessage

            builder.QueryItems.Remove("backUrl")

            Style.Append("label.label { float:left; width:12em; font-weight:bold; }")
            Style.Append("span.label { float:left; width:12em; padding-right:1em; font-weight:bold; }")
            Style.Append(".Amendment {padding-left:2em; color:red; font-style:italic; )")
            Me.AddExtraCssStyle(Style.ToString)

            contractEndReason = New ContractEndReason(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            msg = ContractEndReason.FetchList(Me.DbConnection, reasons, String.Empty, String.Empty, TriState.False)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With cboEndReason
                With .DropDownList
                    .Items.Clear()
                    For Each reason As ContractEndReason In reasons
                        If (reason.Usage And ContractEndReasonUsage.DomContracts) <> 0 Then
                            .Items.Add(New ListItem(reason.Description, reason.ID))
                        End If
                    Next
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty, String.Empty))
                    cboEndReason.SelectPostBackValue()
                End With
            End With


            If contractID > 0 Then
                contract = New DomContract(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                With contract
                    msg = .Fetch(contractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    lblNumber.Text = .Number
                    lblTitle.Text = .Title
                    lblDescription.Text = .Description
                    lblStartDate.Text = .StartDate.ToString("dd MMM yyyy")
                    provider = New Establishment(Me.DbConnection)
                    With provider
                        msg = .Fetch(contract.ProviderID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        lblProvider.Text = String.Format("{0} : {1}", .AltReference, .Name)
                    End With
                    lblContractType.Text = .ContractType
                    If .ClientID > 0 Then
                        client = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
                        With client
                            msg = .Fetch(contract.ClientID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            lblServiceUser.Text = String.Format("{0} : {1}", .Reference, .Name)
                        End With
                    End If

                    ' store MRU Contract
                    Dim mruManager As Target.Library.Web.MostRecentlyUsedManager = New Target.Library.Web.MostRecentlyUsedManager(HttpContext.Current)
                    mruManager("DOM_CONTRACTS")(contractID.ToString()) = String.Format("{0}: {1}", .Number, .Title)
                    mruManager.Save(HttpContext.Current)
                End With
            End If


            Me.JsLinks.Add("Terminate.js")

        End Sub

        Private Sub btnTerminate_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTerminate.ServerClick
            Dim _auditLogtitle As String = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)
            Dim _auditUserName As String = SecurityBL.GetCurrentUser().ExternalUsername
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage = Nothing
            Dim trans As SqlTransaction = Nothing
            Dim contractID As Integer = Utils.ToInt32(Request.QueryString("id"))

            Me.Validate("Save")
            If Me.IsValid Then

                Try

                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    msg = DomContractBL.DomContractTermination(trans, _
                                                               contractID, _
                                                               cboEndReason.DropDownList.SelectedValue, _
                                                               dteTerminateDate.Value, _
                                                               _auditLogtitle, _
                                                               _auditUserName)
                    If msg.Success Then

                        trans.Commit()

                        Response.Redirect(Request.QueryString("backUrl"))

                    Else
                        If msg.Number = DomContractBL.ERR_CAN_NOT_TERMINATE_CONTRACT Then
                            ' could not save DSO or could not categorise visits
                            lblError.Text = msg.Message.Replace(vbCrLf, "<br />")
                            'msg = New ErrorMessage
                            'msg.Success = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    End If

                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                Finally
                    If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                End Try

                'If Not msg.Success Then WebUtils.DisplayError(msg)

            End If

        End Sub
    End Class

End Namespace