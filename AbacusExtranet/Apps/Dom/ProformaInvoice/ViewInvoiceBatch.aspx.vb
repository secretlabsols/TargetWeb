Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Namespace Apps.Dom.ProformaInvoice

    ''' <summary>
    ''' Screen to allow a user to view the details of a domiciliary pro forma invoice batch.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewInvoiceBatch
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SP_NAME_FETCH_INVOICEBATCH As String = "spxDomProformaInvoiceBatch_FetchForView"

            Dim domProformaBatchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reader As SqlDataReader = Nothing
            Dim currentStatus As Short
            Dim batchTypeID As Short
            Dim js As String
            Dim style As New StringBuilder
            Dim msg As ErrorMessage
            Dim invoiceBatch As DomProformaInvoiceBatch = Nothing

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "View Domiciliary Pro forma Invoice Batch")

            btnVerify.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.ProformaInvoice.Verify"))
            btnDelete.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.PaymentSchedules.ProformaInvoice.Delete"))
            btnViewInvoices.Visible = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                           user.ID, _
                                           Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), _
                                           Me.Settings.CurrentApplicationID)

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add("ViewInvoiceBatch.js")
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProformaInvoiceBatchType))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProformaInvoiceBatchStatus))
            style.Append("label.label { float:left; width:14.5em; font-weight:bold; }")
            style.Append("span.label { float:left; width:14.5em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_INVOICEBATCH, False)
                spParams(0).Value = domProformaBatchID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_INVOICEBATCH, spParams)

                While reader.Read
                    lblProvider.Text = String.Format("{0}/{1}", reader("ProviderReference"), reader("ProviderName"))
                    lblContract.Text = String.Format("{0}/{1}", reader("ContractNumber"), reader("ContractTitle"))
                    If Not IsDBNull(reader("ClientName")) Then
                        lblServiceUser.Text = String.Format("{0}/{1}", reader("ClientReference"), reader("ClientName"))
                    End If
                    lblDateCreated.Text = WebUtils.EncodeOutput(reader("DateCreated"))
                    lblCreatedBy.Text = WebUtils.EncodeOutput(reader("CreatedBy"))
                    currentStatus = reader("DomProformaInvoiceBatchStatusID")
                    batchTypeID = reader("DomProformaInvoiceBatchTypeID")
                    lblStatus.Text = Utils.SplitOnCapitals([Enum].GetName(GetType(DomProformaInvoiceBatchStatus), currentStatus))
                    lblStatusDate.Text = WebUtils.EncodeOutput(reader("StatusDate"))
                    lblLastActionBy.Text = WebUtils.EncodeOutput(reader("StatusChangedBy"))
                    lblInvoiceCount.Text = WebUtils.EncodeOutput(reader("ItemCount"))
                    lblNetPayment.Text = Convert.ToDecimal(reader("TotalPayment")).ToString("c")
                    lblItemsQueried.Text = reader("QueryCount")
                End While
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEBATCH, "ViewInvoiceBatch.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            js = String.Format("currentStatus={0};selectedInvoiceBatchID={1};batchTypeID={2};btnVerifyID=""{3}"";btnDeleteID=""{4}"";Init();", _
                               currentStatus, domProformaBatchID, batchTypeID, btnVerify.ClientID, btnDelete.ClientID)

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

    End Class
End Namespace