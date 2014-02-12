
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
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MikeVO  18/09/2008  D11402 - added DateTo.
    ''' </history>
    Partial Public Class PlannedCarePkg
        Inherits BasePage

#Region " Private variables "

        Private _stdBut As StdButtonsBase
        Private _packageID As Integer, _clientID As Integer

#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.PlannedCarePackageEnquiry"), "Planned Care Package")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("id")) > 0 Then
                _packageID = Utils.ToInt32(Request.QueryString("id"))
            End If
            _clientID = Utils.ToInt32(Request.QueryString("clientID"))

            ' setup buttons
            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PlannedCarePackageEnquiry.AddNew"))
                .ShowNew = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PlannedCarePackageEnquiry.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PlannedCarePackageEnquiry.Delete"))
                .EditableControls.Add(fsControls.Controls)
                .AllowFind = False
                .AllowBack = True
                .AuditLogTableNames.Add("PlannedCarePackage")
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

            Dim msg As ErrorMessage
            Dim package As PlannedCarePackage

            package = New PlannedCarePackage(Me.DbConnection, String.Empty, String.Empty)
            With package
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _packageID = .ID
                _clientID = .ClientID

                PopulateClient()

                dteDateFrom.Text = .DateFrom
                If .DateTo = DataUtils.MAX_DATE Then
                    dteDateTo.Text = String.Empty
                Else
                    dteDateTo.Text = .DateTo
                End If
                txtValue.Text = .PackageValue.ToString("F2")
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

            msg = PlannedCarePackage.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim package As PlannedCarePackage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If Me.IsValid Then
                package = New PlannedCarePackage(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With package
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        _packageID = .ID
                        _clientID = .ClientID
                    End With
                Else
                    package.ClientID = _clientID
                End If

                PopulateClient()

                With package
                    .DateFrom = dteDateFrom.Text
                    If dteDateTo.Text.Trim().Length = 0 Then
                        .DateTo = DataUtils.MAX_DATE
                    Else
                        .DateTo = dteDateTo.Text
                    End If
                    .PackageValue = txtValue.Text

                    ' save package
                    msg = SdsBL.SavePlannedCarePackage(Me.DbConnection, package)
                    If Not msg.Success Then
                        If msg.Number = SdsBL.ERR_COULD_NOT_SAVE_PLANNED_CARE_PACKAGE Then
                            lblError.Text = msg.Message
                            e.Cancel = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    Else
                        e.ItemID = .ID
                        _packageID = .ID
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

            ' set client details
            client = New ClientDetail(Me.DbConnection)
            With client
                msg = .Fetch(_clientID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtClient.Text = String.Format("{0}: {1}", .Reference, .Name)
            End With

        End Sub

#End Region

    End Class

End Namespace