Imports System.Collections.Generic
Imports System.Text
Imports Target.Web.Apps
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Abacus.Library.SDS
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls
Imports Target.Abacus.Library
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess


Namespace Apps.ServiceUsers.Enquiry
    ''' <summary>
    '''  New Statement Screen
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD   SDS Issue 507 21/04/2010 - Changes to btnGenerate_Clicked to handle sds transaction reconisderation exceptions/warnings when calling PersonalBudgetStatementBL.CreatePersonalBudgetStatementDocument.
    '''     Iftikhar   10/03/11  D11854 - View button is renamed to Preview and Generate statement button has been introduced
    '''     ColinD   SDS Issue 370 23/11/2010 - Changes to reconsider transactions for a selected client budget period in js
    '''     ColinD   SDS Issue 370 11/11/2010 - Changes to indicate on client side if a selected client budget period needs reconsidering
    '''     ColinD   SDS Issue 298 27/10/2010 - Set the current years budget period if present in the drop down list
    '''     Mo Tahir D11814 20/09/2010
    ''' </history>
    Partial Public Class NewStatement
        Inherits Target.Web.Apps.BasePage

        Private _clientID As Integer
        Private _budgetPeriods As List(Of ViewableClientBudgetPeriod) = Nothing
        Private _personalBudgetStandardReportID As Integer = 0


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.EnableTimeout = False

            _personalBudgetStandardReportID = CType(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.PersonalBudgetStandard"), Integer)

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUserEnquiry"), "Service User")
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID

            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/Date.js"))
            Me.JsLinks.Add("NewStatement.js")

            If Utils.ToInt32(Request.QueryString("clientID")) > 0 Then
                _clientID = Utils.ToInt32(Request.QueryString("clientID"))
            End If

            With CType(serviceUserHeader1, Target.Abacus.Web.Apps.UserControls.ServiceUserHeader)
                .InitControl(Me.Page, Me.DbConnection, _clientID)
            End With

            With CType(btnPreview, IReportsButton)
                .ReportID = _personalBudgetStandardReportID
                .ReportToExcel = False
                .ReportToView = False
                .Position = SearchableMenu.SearchableMenuPosition.TopLeft
            End With

            cboBudgetPeriods.DropDownList.Attributes.Add("onchange", "cboBudgetPeriods_OnChange();")
            cboStatementLayout.DropDownList.Attributes.Add("onchange", "cboStatementLayout_OnChange();")

            If Not IsPostBack Then
                LoadBudgetPeriods(cboBudgetPeriods)
                LoadStatements(cboStatementLayout)
            End If

            Me.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                  "Target.Abacus.Web.Apps.ServiceUsers.Enquiry.NewStatement.Startup", _
                                                  String.Format("NewStatement_btnViewID='{0}';NewStatement_btnGenerateID='{1}';", _
                                                                btnPreview.ClientID, btnGenerate.ClientID), True)

            ' register the sds transactions web service for use in js
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.SdsTransactions))

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim sbJavascript As New StringBuilder()

            If Not _budgetPeriods Is Nothing AndAlso _budgetPeriods.Count > 0 Then
                ' if we have some budget periods then output them to js

                Dim cbPeriodIdx As Integer = 0

                For Each cbPeriod As ViewableClientBudgetPeriod In _budgetPeriods
                    ' loop each budget period and add to js string builder

                    sbJavascript.AppendFormat("NewStatement_ClientBudgetPeriods[{0}] = new ClientBudgetPeriod({1}, {2});", _
                                              cbPeriodIdx, _
                                              cbPeriod.ID, _
                                              cbPeriod.HasSdsTransactionReconsiderations.ToString().ToLower())
                    cbPeriodIdx += 1

                Next

            End If

            Me.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                  "Target.Abacus.Web.Apps.ServiceUsers.Enquiry.NewStatement.Startup2", _
                                                  sbJavascript.ToString, True)

        End Sub

        ''' <summary>
        ''' Handles the Click event of the btnGenerate button. By clicking the Generate button  
        ''' a personal budget statement is produced for the pre-selected service user for the 
        ''' selected budget period.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        ''' <history>
        '''     Iftikhar      02/03/2011  D11854 - Created
        ''' </history>
        Protected Sub btnGenerate_Clicked(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGenerate.Click

            Dim msg As New ErrorMessage

            Using trans As SqlTransaction = SqlHelper.GetTransaction(Me.DbConnection)
                ' create a transaction

                Try

                    Dim clientBudgetPeriodID As Integer = CType(cboBudgetPeriods.DropDownList.SelectedValue(), Integer)
                    Dim userName As String = Target.Web.Apps.Security.SecurityBL.GetCurrentUser().ExternalUsername
                    Dim clientAuditTitle As String = AuditLogging.GetAuditLogTitle(PageTitle, Settings)
                    Dim personalBudgetStatementID As Integer = 0, documentID As Integer
                    Dim reconsiderationExceptions As List(Of SdsTransactions.SdsTransactionReconsiderationException) = Nothing
                    Dim reconsiderationWarnings As List(Of SdsTransactions.SdsTransactionReconsiderationWarning) = Nothing

                    msg = PersonalBudgetStatementBL.CreatePersonalBudgetStatementDocument(trans, _clientID, _
                                                                                          clientBudgetPeriodID, _
                                                                                          personalBudgetStatementID, _
                                                                                          Nothing, userName, _
                                                                                          clientAuditTitle, False, _
                                                                                          documentID, _
                                                                                          reconsiderationExceptions, reconsiderationWarnings)

                    If msg.Success Then
                        ' if successful then commit transaction

                        trans.Commit()

                        Me.ClientScript.RegisterStartupScript(Me.GetType(), _
                                          "Target.Abacus.Web.Apps.ServiceUsers.Enquiry.NewStatement.CloseWindow", _
                                          String.Format("RefreshParentAndClose({0});", personalBudgetStatementID), True)

                    ElseIf msg.Number = PersonalBudgetStatementBL.ERROR_SDS_TRANSACTION_EXCEPTIONS_OR_WARNINGS Then
                        ' else we have encountered sds transaciton reconsideration errors so display on this screen
                        lblError.Text = msg.Message

                    Else
                        ' else unknown error so use generic error handler

                        WebUtils.DisplayError(msg)

                    End If

                Catch ex As Exception
                    ' always rollback transaction if hit exception

                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))

                End Try

            End Using

        End Sub

#Region "           LoadBudgetPeriods "

        ''' <summary>
        ''' Loads the budget periods into drop down list.
        ''' </summary>
        ''' <param name="dropdown">The dropdown to load budget periods into.</param>
        Private Sub LoadBudgetPeriods(ByVal dropdown As DropDownListEx)

            Dim msg As ErrorMessage
            Dim cbPeriodListItem As ListItem = Nothing
            Dim selectedDefaultPeriod As Boolean = False

            ' get the budget periods
            msg = ServiceUserBL.GetBudgetPeriods(Me.DbConnection, _clientID, _budgetPeriods)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            For Each cbPeriod As ViewableClientBudgetPeriod In _budgetPeriods.OrderBy(Function(cb As ViewableClientBudgetPeriod) cb.DateFrom)
                ' loop each budget period and add into drop down list

                cbPeriodListItem = New ListItem(cbPeriod.DateFrom & " - " & cbPeriod.DateTo, cbPeriod.ID)

                If DateTime.Now >= cbPeriod.DateFrom AndAlso DateTime.Now <= cbPeriod.DateTo Then
                    ' if this item is in the current period then set it as the default

                    cbPeriodListItem.Selected = True
                    selectedDefaultPeriod = True

                End If

                ' add the item into the list
                dropdown.DropDownList.Items.Add(cbPeriodListItem)

            Next

            If selectedDefaultPeriod = False Then
                ' if we havent selected a default period then select the last one in list

                dropdown.DropDownList.SelectedIndex = (dropdown.DropDownList.Items.Count - 1)

            End If

        End Sub

#End Region

#Region "           LoadStatements "

        Private Sub LoadStatements(ByVal dropdown As DropDownListEx)

            Dim msg As ErrorMessage
            Dim format As PersonalBudgetStatementBL.Formats

            With dropdown.DropDownList.Items
                .Clear()
                For Each layout As String In [Enum].GetNames(GetType(PersonalBudgetStatementBL.Formats))
                    Dim tmpLayout As PersonalBudgetStatementBL.Formats = [Enum].Parse(GetType(PersonalBudgetStatementBL.Formats), layout)
                    .Add(New ListItem(Utils.SplitOnCapitals(tmpLayout.ToString()), Convert.ToInt32(tmpLayout)))
                Next
            End With

            msg = PersonalBudgetStatementBL.GetFormat(Me.DbConnection, Nothing, _clientID, format)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            WebUtils.SetDropdownListValue(cboStatementLayout.DropDownList, Convert.ToInt32(format))

        End Sub

#End Region

    End Class

End Namespace
