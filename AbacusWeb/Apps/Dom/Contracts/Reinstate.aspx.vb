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
    ''' PW   08/01/2008  D11470 - Initial version.
    ''' </history>
    Partial Class Reinstate
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryContractsReinstate"), "Re-instate Domiciliary Contract")

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

            style.Append("label.label { float:left; width:12em; font-weight:bold; }")
            style.Append("span.label { float:left; width:12em; padding-right:1em; font-weight:bold; }")
            style.Append(".Amendment {padding-left:2em; color:red; font-style:italic; )")
            Me.AddExtraCssStyle(style.ToString)

            If contractID > 0 Then
                contract = New DomContract(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                With contract
                    msg = .Fetch(contractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    lblNumber.Text = .Number
                    lblTitle.Text = .Title
                    lblDescription.Text = .Description
                    lblStartDate.Text = .StartDate.ToString("dd MMM yyyy")
                    lblEndDate.Text = .EndDate.ToString("dd MMM yyyy")
                    contractEndReason = New ContractEndReason(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    msg = contractEndReason.Fetch(.ContractEndReasonID)
                    If msg.Success Then
                        lblEndReason.Text = contractEndReason.Description
                    End If
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

            Me.JsLinks.Add("Reinstate.js")

        End Sub

        Private Sub btnReinstate_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReinstate.ServerClick
            Dim _auditLogtitle As String = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)
            Dim _auditUserName As String = SecurityBL.GetCurrentUser().ExternalUsername
            Dim msg As ErrorMessage = Nothing
            Dim trans As SqlTransaction = Nothing
            Dim contractID As Integer = Utils.ToInt32(Request.QueryString("id"))

            Try
                Me.Validate("Save")
                If Me.IsValid Then
                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    msg = DomContractBL.DomContractReinstate(trans, _
                                                             contractID, _
                                                             _auditUserName, _
                                                             _auditLogtitle)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    trans.Commit()

                End If

                msg = New ErrorMessage
                msg.Success = True

                Response.Redirect(Request.QueryString("backUrl"))

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub
    End Class

End Namespace