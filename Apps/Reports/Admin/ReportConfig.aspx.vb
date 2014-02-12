
Imports System.ComponentModel
Imports System.Collections.Specialized
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Reports
Imports Target.Web.Apps.Reports.Collections
Imports Microsoft.Reporting.WebForms

Namespace Apps.Reports.Admin

    ''' <summary>
    ''' Admin page used to maintain report configuration.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      22/10/2009  D11546 - created.
    ''' </history>
    Partial Public Class ReportConfig
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _webReport As WebReport
        Private _btnAuditDetails As HtmlInputButton = New HtmlInputButton("button")
        Private _lnkTest As HyperLink = New HyperLink()
        Private _ssrsConfig As NameValueCollection
        Private _ssrsBaseUrl As String
        Private _ssrsBasePath As String

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            ' prime SSRS configuation
            _ssrsConfig = CType(ConfigurationManager.GetSection("reportServer"), NameValueCollection)
            _ssrsBaseUrl = _ssrsConfig("BaseUrl").Trim()
            _ssrsBasePath = _ssrsConfig("BasePath").Trim()

            If String.IsNullOrEmpty(_ssrsBaseUrl) OrElse String.IsNullOrEmpty(_ssrsBasePath) Then
                lblConfigWarning.Visible = True
            End If

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            With _stdBut
                AddHandler .AddCustomControls, AddressOf StdButtons_AddCustomControls
            End With

        End Sub

#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Reporting.ReportConfiguration"), "Report Configuration")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Reporting.ReportConfiguration.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Reporting.ReportConfiguration.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Reporting.ReportConfiguration.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.ReportConfiguration
                ' add extra params
                ' current application ID
                .GenericFinderExtraParams.Add(Convert.ToInt32(Me.Settings.CurrentApplicationID).ToString())
                ' UserDefined
                .GenericFinderExtraParams.Add(Boolean.TrueString)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebReport.ReportConfiguration")
                .ReportButtonParameters.Add("intApplicationID", Convert.ToInt32(Me.Settings.CurrentApplicationID))
                .ReportButtonParameters.Add("blnUserDefined", Boolean.TrueString)

                AddHandler .FindClicked, AddressOf FindClicked
                AddHandler .EditClicked, AddressOf FindClicked
                AddHandler .SaveClicked, AddressOf SaveClicked
                AddHandler .CancelClicked, AddressOf CancelClicked
                AddHandler .DeleteClicked, AddressOf DeleteClicked
                AddHandler .NewClicked, AddressOf NewClicked
            End With

            Me.JsLinks.Add("ReportConfig.js")

            txtServerConfig.Text = String.Format("{0}{1}", _ssrsBaseUrl, _ssrsBasePath)
            lnkBrowseServer.NavigateUrl = String.Format("{0}?{1}&rs:Command=ListChildren", _ssrsBaseUrl, _ssrsBasePath)

            PopulateAuditDetails(Me.DbConnection, Nothing)

        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            _webReport = New WebReport(Me.DbConnection)
            With _webReport
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                txtReportPath.Text = .Path
            End With
            PopulateCategories(_webReport.ID)
            PopulateAuditDetails(Me.DbConnection, Nothing)

        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                NewClicked(e)
            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region " NewClicked "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            txtDescription.Text = String.Empty
            txtReportPath.Text = String.Empty
            PopulateCategories(0)
        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = ReportsBL.DeleteReportConfiguration(Me.DbConnection, e.ItemID)
            If Not msg.Success Then
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim selected As vwWebReport_WebReportCategoryCollection = Nothing
            Dim cat As vwWebReport_WebReportCategory
            Dim trans As SqlTransaction = Nothing

            If Me.IsValid Then

                Try
                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    _webReport = New WebReport(trans)
                    With _webReport
                        If e.ItemID > 0 Then
                            ' update
                            msg = .Fetch(e.ItemID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            .AmendedByID = currentUser.ID
                            .AmendedDate = DateTime.Now
                        Else
                            ' new
                            .CreatedByID = currentUser.ID
                            .CreatedDate = DateTime.Now
                        End If
                        .Description = txtDescription.Text
                        .Path = txtReportPath.Text

                        .ApplicationID = Convert.ToInt32(Me.Settings.CurrentApplicationID)
                        .ProcessingMode = ProcessingMode.Remote
                        .UserDefined = TriState.True

                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        e.ItemID = .ID

                        PopulateAuditDetails(trans.Connection, trans)

                        ' process categories
                        ' get currently selected categories
                        msg = ReportsBL.GetSelectedReportCategories(Nothing, trans, _webReport.ID, selected)
                        If Not msg.Success Then WebUtils.DisplayError(msg)

                        ' deselect any categroies that are not now selected
                        For Each cat In selected
                            Dim revoke As Boolean = True
                            For Each item As ListItem In dlCategories.DestList.Items
                                If Utils.ToInt32(item.Value) = cat.WebReportCategoryID Then
                                    revoke = False
                                    Exit For
                                End If
                            Next
                            If revoke Then
                                msg = ReportsBL.DeselectReportCategory(trans, cat.ID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next

                        ' select any new categories that weren't there before
                        For Each item As ListItem In dlCategories.DestList.Items
                            Dim grant As Boolean = True
                            For Each cat In selected
                                If cat.WebReportCategoryID = Utils.ToInt32(item.Value) Then
                                    grant = False
                                    Exit For
                                End If
                            Next
                            If grant Then
                                msg = ReportsBL.SelectReportCategory(trans, _webReport.ID, Utils.ToInt32(item.Value))
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                        Next

                    End With

                    trans.Commit()

                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                    SqlHelper.RollbackTransaction(trans)
                    WebUtils.DisplayError(msg)
                End Try

            Else
                e.Cancel = True
            End If

        End Sub

#End Region

#Region " PopulateAuditDetails "

        Private Sub PopulateAuditDetails(ByVal conn As SqlConnection, ByVal trans As SqlTransaction)

            Dim user As WebSecurityUser
            Dim msg As ErrorMessage

            If Not _webReport Is Nothing AndAlso _webReport.ID > 0 Then
                With CType(auditDetails, IBasicAuditDetails)
                    .Collapsed = True

                    If Not trans Is Nothing Then
                        user = New WebSecurityUser(trans)
                    Else
                        user = New WebSecurityUser(conn)
                    End If
                    msg = user.Fetch(_webReport.CreatedByID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    .EnteredBy = String.Format("{0} {1}", user.FirstName, user.Surname)
                    .DateEntered = _webReport.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss")

                    If Utils.IsDate(_webReport.AmendedDate) Then
                        If _webReport.CreatedByID <> _webReport.AmendedByID Then
                            If Not trans Is Nothing Then
                                user = New WebSecurityUser(trans)
                            Else
                                user = New WebSecurityUser(conn)
                            End If
                            msg = user.Fetch(_webReport.AmendedByID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End If
                        .LastAmendedBy = String.Format("{0} {1}", user.FirstName, user.Surname)
                        .DateLastAmended = _webReport.AmendedDate.ToString("dd/MM/yyyy HH:mm:ss")
                    End If

                End With

                _btnAuditDetails.Visible = True
                auditDetails.Visible = True

            Else
                _btnAuditDetails.Visible = False
                auditDetails.Visible = False
            End If

        End Sub

#End Region

#Region " PopulateCategories "

        Private Sub PopulateCategories(ByVal webReportID As Integer)

            Dim currentApp As ApplicationName = Me.Settings.CurrentApplicationID
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim selected As vwWebReport_WebReportCategoryCollection = Nothing
            Dim available As WebReportCategoryCollection = Nothing
            Dim availableCat As WebReportCategory
            Dim cat As vwWebReport_WebReportCategory
            Dim alreadySelected As New ArrayList()
            Dim index As Integer

            ' selected
            msg = ReportsBL.GetSelectedReportCategories(Me.DbConnection, Nothing, webReportID, selected)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With dlCategories
                .DestList.Items.Clear()
                For Each cat In selected
                    .DestList.Items.Add(New ListItem(cat.WebReportCategoryDescription, cat.WebReportCategoryID))
                Next
            End With

            ' available
            msg = WebReportCategory.FetchList(Me.DbConnection, available)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' remove those already selected
            For Each cat In selected
                For Each availableCat In available
                    If availableCat.ID = cat.WebReportCategoryID Then
                        alreadySelected.Add(available.IndexOf(availableCat))
                    End If
                Next
            Next

            index = alreadySelected.Count - 1
            While index >= 0
                available.RemoveAt(alreadySelected(index))
                index -= 1
            End While

            With dlCategories.SrcList.Items
                .Clear()
                For Each availableCat In available
                    .Add(New ListItem(availableCat.Description, availableCat.ID))
                Next
            End With

        End Sub
#End Region

#Region " StdButtons_AddCustomControls "

        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)

            With _btnAuditDetails
                .ID = "btnAuditDetails"
                .Value = "Audit Details"
            End With
            controls.Add(_btnAuditDetails)

            With CType(auditDetails, IBasicAuditDetails)
                .ToggleControlID = _btnAuditDetails.ClientID
            End With

        End Sub

#End Region

#Region " txtReportPath_AfterTextBoxControlAdded "

        Private Sub txtReportPath_AfterTextBoxControlAdded(ByVal sender As TextBoxEx) Handles txtReportPath.AfterTextBoxControlAdded

            With _lnkTest
                .ID = "lnkText"
                .Style.Add("margin-left", "0.5em")
                .Text = "[Test]"
                .Target = "_blank"
            End With

            sender.Controls.Add(_lnkTest)

        End Sub

#End Region

#Region " ReportConfig_PreRenderComplete "

        Private Sub ReportConfig_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            With lnkBrowseServer
                .Enabled = True
                .CssClass = .CssClass.Replace("disabled", "")
            End With
            With _lnkTest
                .Enabled = True
                .CssClass = .CssClass.Replace("disabled", "")
            End With

            Me.ClientScript.RegisterStartupScript( _
                Me.GetType(), "Startup", _
                String.Format("ssrsBaseUrl='{0}';ssrsBasePath='{1}';txtReportPathID='{2}';lnkTestID='{3}';", _
                              _ssrsBaseUrl, _
                              _ssrsBasePath, _
                              txtReportPath.ClientID, _
                              _lnkTest.ClientID _
                ), _
                True _
            )

        End Sub

#End Region

    End Class

End Namespace