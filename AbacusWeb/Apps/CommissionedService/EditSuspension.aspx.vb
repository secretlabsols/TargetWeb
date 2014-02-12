Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.Dom.SvcOrders

    ''' <summary>
    ''' Screen used to maintain a Service Order Suspensions.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO    15/12/2009  A4WA#5967 - fix to re-construction of redirect url (encoding backUrl parameter)
    '''     Paul      14/01/2009  D11472 - Service order Suspensions
    ''' </history>
    Partial Public Class EditSuspension
        Inherits BasePage

        Private _stdBut As StdButtonsBase

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .hideCreditorRef = True
            End With
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOrderSuspensions"), "Service Order Suspensions")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            ' setup buttons
            With _stdBut
                .EditableControls.Add(fsHeader.Controls)
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderSuspensions.AddNew"))
                .ShowNew = False
                .AllowFind = False
                .AllowBack = True
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderSuspensions.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderSuspensions.Delete"))
                .AuditLogTableNames.Add("ServiceOrderSuspensionPeriod")
                .AuditLogTableNames.Add("DomServiceOrderSuspensionPeriod")
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            'AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            ' output javascript
            'Me.JsLinks.Add("Edit.js")
            Me.JsLinks.Add("EditSuspension.js")
            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))

            Me.UseJQuery = True

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

            PopulateCombos()

        End Sub

        Private Sub PopulateCombos()

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim reasons As ServiceOrderSuspensionReasonCollection = Nothing

            msg = ServiceOrderSuspensionReason.FetchList(Me.DbConnection, reasons, String.Empty, String.Empty)
            If Not Msg.Success Then WebUtils.DisplayError(Msg)
            With cboSuspensionReason
                With .DropDownList
                    .Items.Clear()
                    .DataSource = reasons
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty, String.Empty))
                    cboSuspensionReason.SelectPostBackValue()
                End With
            End With
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim suspensionID As Integer = Utils.ToInt32(Request.QueryString("id"))

            PopulateForm(suspensionID)

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            Dim suspensionID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim clientID As Integer = Utils.ToInt32(Request.QueryString("clientID"))

            SetupClientSelector(clientID)

            PopulateForm(suspensionID)

        End Sub


        Private Sub PopulateForm(ByVal suspensionID As Integer)
            Dim suspensionHeader As ServiceOrderSuspensionPeriod = Nothing
            Dim msg As ErrorMessage

            If suspensionID > 0 Then
                suspensionHeader = New ServiceOrderSuspensionPeriod(Me.DbConnection, String.Empty, String.Empty)
                msg = suspensionHeader.Fetch(suspensionID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                With suspensionHeader

                    'if this was created by an interface we can not edit or delete
                    If .CreatedByInterface Then
                        _stdBut.AllowEdit = False
                        _stdBut.AllowDelete = False
                    End If

                    SetupClientSelector(.ClientID)

                    dteDateFrom.Text = .DateFrom
                    If .DateTo <> DataUtils.MAX_DATE Then
                        dteDateTo.Text = .DateTo
                    End If
                    cboSuspensionReason.DropDownList.SelectedValue = .ServiceOrderSuspensionReasonID
                End With

                PopulateOrdersTable(suspensionID)
            End If
        End Sub

        Private Sub PopulateOrdersTable(ByVal SuspensionID As Integer)
            Dim ds As DataSet = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_ORDERS As String = "spxDomServiceOrderSuspensionPeriod_FetchForSuspension"
            ' grab the list of Orders
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_ORDERS, False)
                spParams(0).Value = SuspensionID
                ds = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_ORDERS, spParams)
                rptSuspensions.DataSource = ds.Tables(0)
                rptSuspensions.DataBind()

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_ORDERS, "EditSuspension.PopulateOrdersTable")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            End Try
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim suspensionID As Integer = Utils.ToInt32(Request.QueryString("id"))

            msg = ServiceOrderSuspensionsBL.DeleteDomServiceOrderSuspensionPeriod(Me.DbConnection, suspensionID, currentUser.ExternalUsername, "Service Order Suspensions")
            If msg.Success Then
                Response.Redirect(Request.QueryString("backUrl"))
            Else
                WebUtils.DisplayError(msg)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim suspension As ServiceOrderSuspensionPeriod = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage = Nothing

            suspension = New ServiceOrderSuspensionPeriod(Me.DbConnection, String.Empty, String.Empty)
            If e.ItemID > 0 Then
                msg = suspension.Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If
            suspension.DateFrom = dteDateFrom.Text
            If dteDateTo.Text = "" Then
                suspension.DateTo = DataUtils.MAX_DATE
            Else
                suspension.DateTo = dteDateTo.Text
            End If
            suspension.ServiceOrderSuspensionReasonID = cboSuspensionReason.GetPostBackValue
            suspension.ClientID = Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID))

            msg = ServiceOrderSuspensionsBL.SaveServiceOrderSuspension(Me.DbConnection, suspension, currentUser.ExternalUsername, "Service Order Suspensions")
            If Not msg.Success Then
                If msg.Number = ServiceOrderSuspensionsBL.ERR_CAN_NOT_SAVE_SERVICE_ORDER_SUSPENSION Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    Exit Sub
                Else
                    WebUtils.DisplayError(msg)
                End If
            End If


            Response.Redirect(String.Format("EditSuspension.aspx?id={0}&clientID={1}&backUrl={2}&mode=1", _
                                            suspension.ID, _
                                            Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID)), _
                                            HttpUtility.UrlEncode(Request.QueryString("backUrl")) _
                                ) _
            )

        End Sub

        Private Sub btnSuspend_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSuspend.ServerClick
            Dim suspensionID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As Integer = Utils.ToInt32(Request.Form("Radio1"))
            Dim orderSuspend As DomServiceOrderSuspensionPeriod = New DomServiceOrderSuspensionPeriod(Me.DbConnection, currentUser.ExternalUsername, "Service Order Suspensions")
            Dim msg As ErrorMessage

            msg = orderSuspend.Fetch(id)
            If msg.Success Then
                orderSuspend.Suspended = Not orderSuspend.Suspended
                msg = orderSuspend.Save()
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            PopulateForm(suspensionID)
        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim js As String
            js = String.Format("ServiceOrder_btnViewID='{0}';ServiceOrder_btnSuspendID='{1}';ServiceOrder_mode={2};", _
                                    btnView.ClientID, btnSuspend.ClientID, Convert.ToInt32(_stdBut.ButtonsMode))
            ClientScript.RegisterStartupScript(Me.GetType(), "EditSuspension.Startup", _
                            Target.Library.Web.Utils.WrapClientScript(js))
        End Sub

        Private Sub SetupClientSelector(ByVal clientID As Integer)
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .ClientDetailID = clientID
                .Required = True
                .ValidationGroup = "Save"
                '.RequiredValidator = "A Service User must be entered."
            End With
        End Sub

        Protected Function GetOrderReference() As String
            Dim datatPartioned As String = Eval("DataPartitioned")
            If datatPartioned = "No" Then
                Return "<a href=""javascript:ViewOrder(" & Eval("ServiceOrderID") & ");"" class=""transBg"">" & Eval("OrderReference") & "&nbsp;</a>"
            Else
                Return Eval("OrderReference") & "&nbsp;"
            End If
        End Function

    End Class



End Namespace