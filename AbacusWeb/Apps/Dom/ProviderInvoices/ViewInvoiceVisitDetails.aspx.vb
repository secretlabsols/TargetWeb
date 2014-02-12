Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.Web
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Imports LibUtils = Target.Library.Utils

Namespace Apps.Dom.ProviderInvoices

    ''' <summary>
    ''' Screen to allow a user to view the details of an individual visit.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir           12/12/2011  BTI607 - After having viewed an Invoiced Visit, pressing 'Back' returns to step 1 of the wizard
    '''     Paul Wheaver      14/05/2009  D11613 Invoiced Visits Enquiry.
    ''' </history>
    Partial Class ViewInvoiceVisitDetails
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim visitID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage
            Dim style As New StringBuilder
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))

            If currentPage <= 0 Then currentPage = 1

            Const SP_NAME_FETCH_INVOICEVISIT As String = "spxDomProviderInvoiceVisit_FetchComponentsForView"

            Me.InitPage(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ViewInvoiceVisitDetails"), "View Invoiced Visit")

            Me.JsLinks.Add("TargetWeb\Library\JavaScript\Utils.js")
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            'AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            style.Append("label.label { float:left; width:13em; font-weight:bold; }")
            style.Append("span.label { float:left; width:13em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

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
                    If Not IsDBNull(reader("OrderReference")) Then lblOrderReference.Text = reader("OrderReference")
                    If Not IsDBNull(reader("Weekending")) Then lblWeekending.Text = Convert.ToDateTime(reader("Weekending")).ToString("dd MMM yyyy")
                    If Not IsDBNull(reader("ServiceType")) Then lblServiceType.Text = reader("ServiceType")
                    If Not IsDBNull(reader("VisitDate")) Then lblVisitDate.Text = Convert.ToDateTime(reader("VisitDate")).ToString("dd MMM yyyy")
                    If Not IsDBNull(reader("StartTimeClaimed")) Then lblStartTimeClaimed.Text = Convert.ToDateTime(reader("StartTimeClaimed")).ToString("HH:mm")
                    If Not IsDBNull(reader("DurationClaimed")) Then lblDurationClaimed.Text = Convert.ToDateTime(reader("DurationClaimed")).ToString("H\h m\m")
                    If Not IsDBNull(reader("VisitCode")) Then lblVisitCode.Text = reader("VisitCode")
                    If Not IsDBNull(reader("NumberOfCarers")) Then lblNumberOfCarers.Text = reader("NumberOfCarers")
                    If Not IsDBNull(reader("SecondaryVisit")) Then lblSecondaryVisit.Text = LibUtils.BooleanToYesNo(reader("SecondaryVisit"))
                    If Not IsDBNull(reader("SecondaryVisitAutoSet")) Then lblSecondaryVisitAutoSet.Text = LibUtils.BooleanToYesNo(reader("SecondaryVisitAutoSet"))
                    If Not IsDBNull(reader("ManuallyAmended")) Then lblManuallyAmended.Text = reader("ManuallyAmended")
                    If Not IsDBNull(reader("ActualStartTime")) Then lblActualStartTime.Text = Convert.ToDateTime(reader("ActualStartTime")).ToString("HH:mm")
                    If Not IsDBNull(reader("ActualDuration")) Then lblActualDuration.Text = Convert.ToDateTime(reader("ActualDuration")).ToString("H\h m\m")

                End While
            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_INVOICEVISIT, "ViewInvoiceVisitDetails.Page_Load")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try

            'Populate Carers Table
            PopulateCarersTable(visitID)

            Me.JsLinks.Add("ViewInvoiceVisitDetails.js")

        End Sub

        Private Sub PopulateCarersTable(ByVal VisitID As Long)
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_CARERS As String = "spxDomProviderInvoiceVisit_CareWorker_FetchListForView"
            ' grab the list of titles
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_CARERS, False)
                spParams(0).Value = VisitID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_CARERS, spParams)
                rptCarers.DataSource = reader
                rptCarers.DataBind()

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_CARERS, "ViewInvoiceVisitDetails.PopulateCarersTable")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub

    End Class
End Namespace