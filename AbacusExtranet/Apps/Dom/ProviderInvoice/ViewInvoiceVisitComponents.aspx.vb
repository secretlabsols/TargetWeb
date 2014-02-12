Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.ProviderInvoice

    ''' <summary>
    ''' Screen to allow a user to view the details of an individual visit.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      19/10/2009  D11546 - added more header fields and Unit Cost column.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewInvoiceVisitComponents
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim visitID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchID"))
            Dim invoiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("invoiceID"))
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            Dim style As New StringBuilder

            CType(pSchedule, Apps.UserControls.PaymentScheduleHeader).SingleLiner = True

            Const SP_NAME_FETCH_INVOICEVISIT As String = "spxDomProviderInvoiceVisit_FetchComponentsForView"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ProviderInvoiceEnquiry"), "Provider Invoice - Visit Components")

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProformaInvoice/ViewInvoiceVisitComponents.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProformaInvoice/CommentDialog.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            style.Append("label.label { float:left; width:10em; font-weight:bold; }")
            style.Append("span.label { float:left; width:10em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            If Not Me.IsPostBack Then
                Me.CustomNavAdd(False)
            End If

            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_INVOICEVISIT, False)
                spParams(0).Value = visitID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_INVOICEVISIT, spParams)

                While reader.Read
                    lblProvider.Text = String.Format("{0}/{1}", reader("ProviderReference"), reader("ProviderName"))
                    lblContract.Text = String.Format("{0}/{1}", reader("ContractNumber"), reader("ContractTitle"))
                    If Not IsDBNull(reader("ClientName")) Then
                        lblServiceUser.Text = String.Format("{0}/{1}", reader("ClientReference"), reader("ClientName"))
                    End If
                    If Not IsDBNull(reader("Weekending")) Then lblWeekending.Text = Convert.ToDateTime(reader("Weekending")).ToString("dd MMM yyyy")
                    If Not IsDBNull(reader("VisitDate")) Then lblVisitDate.Text = Convert.ToDateTime(reader("VisitDate")).ToString("dd MMM yyyy")
                    If Not IsDBNull(reader("ActualStartTime")) Then lblStartTime.Text = Convert.ToDateTime(reader("ActualStartTime")).ToString("HH:mm")

                    If Not IsDBNull(reader("DurationClaimed")) Then lblDurationClaimed.Text = Convert.ToDateTime(reader("DurationClaimed")).ToString("H\h m\m")
                    If Not IsDBNull(reader("DurationPaid")) Then
                        lblDurationPaid.Text = String.Format("{0}h {1}m", Math.Floor(reader("DurationPaid") / 60).ToString, (reader("DurationPaid") Mod 60).ToString)
                    End If
                    If Not IsDBNull(reader("CalculatedPayment")) Then lblPayment.Text = Convert.ToDecimal(reader("CalculatedPayment")).ToString("c")
                    If Not IsDBNull(reader("PaymentRef")) Then lblPaymentRef.Text = reader("PaymentRef")
                    If Not IsDBNull(reader("VisitCode")) Then lblVisitCode.Text = reader("VisitCode")
                    If Not IsDBNull(reader("NumberOfCarers")) Then lblNumberOfCarers.Text = reader("NumberOfCarers")
                    If Not IsDBNull(reader("SecondaryVisit")) Then lblSecondaryVisit.Text = Utils.BooleanToYesNo(reader("SecondaryVisit"))
                End While
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEVISIT, "ViewInvoiceVisitComponents.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            'Populate the components table
            PopulateComponantsTable(visitID)


        End Sub

        Private Sub PopulateComponantsTable(ByVal VisitID As Long)
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_COMPONENTS As String = "spxDomProviderInvoiceDetailVisit_FetchListForView"
            ' grab the list of titles
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_COMPONENTS, False)
                spParams(0).Value = VisitID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_COMPONENTS, spParams)
                rptComponents.DataSource = reader
                rptComponents.DataBind()

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_COMPONENTS, "ViewInvoiceVisitComponents.PopulateComponantsTable")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub

        Private Sub btnBack_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.ServerClick
            Me.CustomNavRemoveLast()
            Me.CustomNavGoBack()
        End Sub

    End Class
End Namespace