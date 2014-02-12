
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

Namespace Apps.Sds

    ''' <summary>
    ''' Screen used to maintain an SDS planned care package.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD  11/10/2010  D11918 - renamed from PlannedCarePkg to IndicativeBudget
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MikeVO  18/09/2008  D11402 - added DateTo.
    ''' </history>
    Partial Public Class IndicativeBudget
        Inherits BasePage

#Region " Private variables "

        Private _stdBut As StdButtonsBase
        Private _ID As Integer, _clientID As Integer

#End Region

#Region " Page_Load "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            With CType(Me.ipClient, InPlaceSelectors.InPlaceClientSelector)
                .hideDebtorRef = True
            End With
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.IndicativeBudgetEnquiry"), "Indicative Budget")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("id")) > 0 Then
                _ID = Utils.ToInt32(Request.QueryString("id"))
            End If
            _clientID = Utils.ToInt32(Request.QueryString("clientID"))

            ' setup buttons
            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.IndicativeBudgetEnquiry.AddNew"))
                .ShowNew = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.IndicativeBudgetEnquiry.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.IndicativeBudgetEnquiry.Delete"))
                .EditableControls.Add(fsControls.Controls)
                .AllowFind = False
                .AllowBack = True
                .AuditLogTableNames.Add("IndicativeBudget")
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

        End Sub

#End Region

#Region " NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            PopulateClient()
        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = Nothing
            Dim budget As New Target.Abacus.Library.DataClasses.IndicativeBudget(Me.DbConnection, String.Empty, String.Empty)

            With budget
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _ID = .ID
                _clientID = .ClientID
                PopulateClient()
                dteDateFrom.Text = .DateFrom
                If .DateTo = DataUtils.MAX_DATE Then
                    dteDateTo.Text = String.Empty
                Else
                    dteDateTo.Text = .DateTo
                End If
                txtValue.Text = .Value.ToString("F2")
            End With

        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                PopulateClient()
            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = SdsBL.DeleteIndicativeBudget(DbConnection, e.ItemID, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim budget As Target.Abacus.Library.DataClasses.IndicativeBudget
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim queryClientID As String = Utils.ToInt32(Request.QueryString("clientID"))

            If Me.IsValid Then
                budget = New Target.Abacus.Library.DataClasses.IndicativeBudget(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With budget
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        _ID = .ID
                    End With
                Else
                    If queryClientID > 0 Then
                        ' query string provided client id
                        budget.ClientID = queryClientID
                    Else
                        ' get client id from the in place selector
                        budget.ClientID = Utils.ToInt32(Request.Form(CType(ipClient, InPlaceClientSelector).HiddenFieldUniqueID))
                    End If
                End If

                _clientID = budget.ClientID

                PopulateClient()

                With budget
                    .DateFrom = dteDateFrom.Text
                    If dteDateTo.Text.Trim().Length = 0 Then
                        .DateTo = DataUtils.MAX_DATE
                    Else
                        .DateTo = dteDateTo.Text
                    End If
                    .Value = txtValue.Text

                    ' save indicative budget
                    msg = SdsBL.SaveIndicativeBudget(Me.DbConnection, budget)
                    If Not msg.Success Then
                        If msg.Number = SdsBL.ERR_COULD_NOT_SAVE_INDICATIVE_BUDGET Then
                            lblError.Text = msg.Message
                            e.Cancel = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    Else
                        e.ItemID = .ID
                        _ID = .ID
                    End If

                End With

            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " PopulateClient "

        Private Sub PopulateClient()

            Dim msg As ErrorMessage
            Dim client As ClientDetail

            If _clientID > 0 Then

                client = New ClientDetail(DbConnection, String.Empty, String.Empty)
                With client
                    msg = .Fetch(_clientID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With

                With CType(ipClient, InPlaceSelectors.InPlaceClientSelector)
                    .ClientDetailID = _clientID
                    .Required = True
                    .ValidationGroup = "Save"
                End With

            End If

        End Sub

#End Region

#Region "Page_PreRenderComplete"

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            If _clientID > 0 Then

                WebUtils.RecursiveDisable(ipClient.Controls, True)

            Else

                WebUtils.RecursiveDisable(ipClient.Controls, False)

            End If

        End Sub

#End Region

        
    End Class

End Namespace