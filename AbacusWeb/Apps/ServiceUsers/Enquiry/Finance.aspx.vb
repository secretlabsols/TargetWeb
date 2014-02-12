Imports System.Text
Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Controls
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Library.Web.Extensions.AjaxControlToolkit
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Abacus.Library.DebtorInvoices

Namespace Apps.ServiceUsers.Enquiry
    ''' <summary>
    ''' Screen used to maintain service user finance details.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD        20/11/2012  D12310 - updated so that buttons are not displayed for creditor payment records to authorise, suspend etc as new functionality functions differently.
    '''     MikeVO        24/05/2011  SDS issue #654 - improve debtor invoice performance.
    '''     Iftikhar      02/03/2011  D11854 - user control DocumentSelector is replaced by StatementsSelector
    '''     Iftikhar      01/03/2011  D11966 - passing show/hide New button flag to DebtorInvoiceResults.InitControl
    '''     ColinD        22/12/2010  D11852C - Removed Spend Plans panel, moved to Services tab instead.
    '''     Mo Tahir      27/08/2010  D11814 - Service User Enquiry.
    ''' </history>
    Partial Public Class Finance
        Inherits Target.Web.Apps.BasePage

        Private _clientID As Integer
        Private _qsParser As WizardScreenParameters

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUserEnquiry"), "Service User")
            Me.UseJQuery = True
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID
            Dim personalBudgetStatementID As Integer = Utils.ToInt32(Request.QueryString("personalbudgetstatementid"))

            ' get the controls
            genericCreditorPayment1 = cpPayments.FindControl("genericCreditorPayment1")
            debtorInvoice1 = cpInvoices.FindControl("debtorInvoice1")
            ucStatementSelector = cpStatements.FindControl("ucStatementSelector")

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("clientid")) > 0 Then
                _clientID = Utils.ToInt32(Request.QueryString("clientid"))
            End If

            With CType(genericCreditorPayment1, Target.Abacus.Web.Apps.UserControls.GenericCreditorPaymentSelector)
                .FilterServiceUserID = _clientID
                .ShowMruList = False
                .ShowNewButton = False
                .ShowServiceUserColumn = False
                .ShowButtons = False
                .WindowOpenMode = GenericCreditorPaymentSelector.WindowOpenModes.Popup
                .InitControl()
            End With

            With CType(debtorInvoice1, Target.Abacus.Web.Apps.UserControls.DebtorInvoiceResults)
                'set up the required parameters
                _qsParser = New WizardScreenParameters
                _qsParser.InvoiceRes = "true"
                _qsParser.InvoiceDom = "true"
                _qsParser.ClientID = _clientID
                _qsParser.InvoiceLD = "true"
                _qsParser.InvoiceClient = "true"
                _qsParser.InvoiceTP = "true"
                _qsParser.InvoiceProp = "true"
                _qsParser.InvoiceOLA = "true"
                _qsParser.InvoicePenColl = "true"
                _qsParser.InvoiceHomeColl = "true"
                _qsParser.InvoiceStd = "true"
                _qsParser.InvoiceMan = "true"
                _qsParser.InvoiceSDS = "true"
                _qsParser.InvoiceActual = "true"
                _qsParser.InvoiceProvisional = "false"
                _qsParser.InvoiceRetracted = "true"
                _qsParser.InvoiceViaRetract = "true"
                _qsParser.InvoiceZeroValue = "true"
                .InitControl(Me.Page, _qsParser, False, False, True)
            End With

            cpStatements.Visible = IntranetSDSv2Licensed(Me.DbConnection)
            cpStatements.Expanded = (personalBudgetStatementID > 0)

            If cpStatements.Visible Then
                CType(ucStatementSelector, StatementsSelector).InitControl(Me.Page, personalBudgetStatementID, _
                                                                           _clientID, True)
            End If

        End Sub

        ''' <summary>
        ''' Checks if system is licensed for Intranet SDS v2
        ''' </summary>
        ''' <param name="conn"></param>
        ''' <returns>Returns True if system is licensed for Intranet SDS v2 else False</returns>
        ''' <remarks></remarks>
        Private Function IntranetSDSv2Licensed(ByVal conn As SqlClient.SqlConnection) As Boolean
            Dim msg As ErrorMessage
            Dim isLicensed As Boolean

            msg = Licensing.ModuleLicence.AreAnyModulesLicensed(conn, _
                  New Licensing.ModuleLicenses() {Licensing.ModuleLicenses.IntranetSDSv2}, _
                  isLicensed)

            Return isLicensed
        End Function

    End Class
End Namespace
