Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Screen to allow a user to view the details of a domiciliary contract.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewDomiciliaryContract
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim domContractID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim style As New StringBuilder
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            Const SP_NAME_FETCH_DOMCONTRACT As String = "spxDomContractView_Fetch"
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ViewDomiciliaryContract"), "View Contract")
            Me.JsLinks.Add("TargetWeb\Library\JavaScript\Utils.js")
            Me.JsLinks.Add("ViewDomiciliaryContract.js")
            style.Append("label.label { float:left; width:14.5em; font-weight:bold; }")
            style.Append("span.label { float:left; width:14.5em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            btnViewRates.Visible = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                            user.ID, _
                                            Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ViewDomiciliaryContractRates"), _
                                            Me.Settings.CurrentApplicationID)

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_DOMCONTRACT, False)
                spParams(0).Value = domContractID
                spParams(1).Value = user.ExternalUserID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_DOMCONTRACT, spParams)

                If reader.Read() Then
                    lblProviderRef.Text = WebUtils.EncodeOutput(reader("ProviderReference"))
                    lblProviderName.Text = WebUtils.EncodeOutput(reader("ProviderName"))
                    lblContractNo.Text = WebUtils.EncodeOutput(reader("Number"))
                    lblContractTitle.Text = WebUtils.EncodeOutput(reader("Title"))
                    lblContractDesc.Text = WebUtils.EncodeOutput(reader("Description"))
                    lblServiceUserRef.Text = WebUtils.EncodeOutput(reader("ClientReference"))
                    lblServiceUserName.Text = WebUtils.EncodeOutput(reader("ClientName"))
                    lblStartDate.Text = WebUtils.EncodeOutput(reader("StartDate"))
                    lblEndDate.Text = WebUtils.EncodeOutput(reader("EndDate"))
                    If Not Convert.IsDBNull(reader("DcrDescription")) Then
                        lblDcr.Text += WebUtils.EncodeOutput(reader("DcrDescription"))
                    End If
                    If Not Convert.IsDBNull(reader("DcrReference")) Then
                        lblDcr.Text += String.Format("({0})", WebUtils.EncodeOutput(reader("DcrReference")))
                    End If
                    If Convert.IsDBNull(reader("DcrReference")) And Convert.IsDBNull(reader("DcrReference")) Then
                        lblDcr.Text = "None"
                    End If


                Else
                    WebUtils.DisplayAccessDenied()
                End If

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_DOMCONTRACT, "ViewDomiciliaryContract.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

        End Sub

    End Class

End Namespace