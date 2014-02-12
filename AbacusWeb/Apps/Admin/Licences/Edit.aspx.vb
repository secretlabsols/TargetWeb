Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Licensing
Imports Target.Web.Apps.Licensing.Collections
Imports System.Reflection
Imports System.Text
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports AjaxControlToolkit


Namespace Apps.Admin.Licences
    ''' <summary>
    ''' Screen used to manage licences.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir  23/11/2011  BTI302 - Deleting the license for Intranet SDS v1 breaks Roles Maintenance 
    ''' MikeVO   14/04/2011  SDS issue #593 - support for revoking unlicensed menu items.
    ''' Mo Tahir 15/06/2010  D11829.
    ''' </history>
    Partial Public Class Edit
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Const CTRL_PREFIX_AUTHORISATION_CODE As String = "authorisationcode_"
        Const CTRL_PREFIX_HCHANGED As String = "hchanged"
        Const CTRL_PREFIX_ISSYSTEMPROTECTED As String = "hsystem"
        Const VIEWSTATE_KEY_DATA As String = "DataList"
        Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"
        Const CTRL_ROW_SUFFIX As String = "_Row"
        Const TEXT_BOX_ROW_SIZE As Integer = 3
        Const NUMBER_OF_LICENCES As Integer = 1
        Private _newCodeIDCounter As Integer

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim js As StringBuilder = New StringBuilder()
            Dim list As List(Of ViewableLicence) '= New List(Of ViewableLicence)

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.SystemSettings.Licences"), "Module Licensing")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.SystemSettings.Licences.Edit"))

                If IsPostBack Then
                    .AllowDelete = True
                Else
                    .AllowDelete = False
                End If
                With .SearchBy
                    .Add("Name", "Name")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.WebModule
                .AuditLogTableNames.Add("WebModuleLicence")
            End With

            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)

            list = GetLicencesFromViewState()

            OutputLicenceHeaders()
            OutputLicenceInfo(list)

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            ClearViewState()
            FindClicked(e)
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim wmLicences As WebModuleLicenceCollection = Nothing
            Dim trans As SqlTransaction = Nothing

            Try
                msg = WebModuleLicence.FetchList(conn:=Me.DbConnection, list:=wmLicences, _
                                             auditUserName:=String.Empty, auditLogTitle:=String.Empty, _
                                             webModuleID:=e.ItemID)

                If wmLicences.Count = NUMBER_OF_LICENCES And _
                        Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.SystemSettings.Licences.Delete")) Then

                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    msg = WebModuleLicence.Delete(trans:=trans, auditUserName:=currentUser.ExternalUsername, _
                                                  auditLogTitle:=AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                                  id:=wmLicences(0).ID, auditLogOverriddenParentID:=e.ItemID)

                    If msg.Success Then
                        msg = ModuleLicence.RevokeUnlicensedMenuItems(trans, e.ItemID)
                        trans.Commit()
                        ' clear cached security/menu items
                        SecurityBL.ClearCacheByUser(currentUser.ID)
                        Navigation.NavigationBL.ClearCacheByUser(currentUser.ID)
                    Else
                        SqlHelper.RollbackTransaction(trans)
                    End If

                End If

                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    SqlHelper.RollbackTransaction(trans)
                Else
                    ModuleLicence.RefreshCacheLicences(ConnectionStrings("Abacus").ConnectionString, True)
                End If

                FindClicked(e)

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0503", "Licences Edit.aspx.vb DeleteClicked")     ' could not delete
                WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim licenceList As List(Of ViewableLicence)
            Dim wmLicence As WebModuleLicence
            Dim transRolledBack As Boolean
            Dim vwLicence As New ViewableLicence

            If Me.IsValid Then
                Try
                    ' get list of licenses to update 
                    licenceList = GetLicencesFromViewState()

                    For Each licence As ViewableLicence In licenceList
                        If ((licence.AuthorisationCode <> _
                        Utils.ToString(CType(phLicences.FindControl(CTRL_PREFIX_AUTHORISATION_CODE & licence.ID), TextBoxEx).TextBox.Text.Trim)) _
                        And (Utils.ToString(CType(phLicences.FindControl(CTRL_PREFIX_AUTHORISATION_CODE & licence.ID), TextBoxEx).TextBox.Text.Trim) <> _
                        String.Empty)) Then

                            licence.AuthorisationCode = Utils.ToString(CType(phLicences.FindControl(CTRL_PREFIX_AUTHORISATION_CODE & licence.ID), TextBoxEx).TextBox.Text.Trim)
                            trans = SqlHelper.GetTransaction(Me.DbConnection)
                            wmLicence = New WebModuleLicence(trans, currentUser.ExternalUsername, Me.PageTitle)
                            msg = wmLicence.Fetch(licence.WebModuleLicenceID)
                            If Not msg.Success Then
                                wmLicence.WebModuleID = licence.ID
                            End If

                            Utils.SetProperties(licence.GetType.GetProperties, _
                                                vwLicence.GetType.GetProperties, licence, vwLicence)
                            msg = ModuleLicence.ReadAuthorisationCode(vwLicence)
                            If Not msg.Success Then
                                lblError.Text = msg.Message
                                e.Cancel = True
                                Exit For
                            End If

                            msg = ModuleLicence.ValidateAuthorisationCode(trans, vwLicence)
                            If Not msg.Success Then
                                trans.Rollback()
                                transRolledBack = True
                                lblError.Text = msg.Message
                                e.Cancel = True
                            Else
                                ModuleLicence.CreateLicenceCode(vwLicence)
                                With wmLicence
                                    .LicenceCode = vwLicence.LicenceCode
                                    .AuditLogOverriddenParentID = e.ItemID
                                    msg = .Save
                                    If Not msg.Success Then
                                        trans.Rollback()
                                        transRolledBack = True
                                        WebUtils.DisplayError(msg)
                                    End If
                                End With
                            End If
                        Else
                            'Can not delete a licence if its system Protected
                            If Utils.ToString(CType(phLicences.FindControl(CTRL_PREFIX_AUTHORISATION_CODE & licence.ID), TextBoxEx).TextBox.Text.Trim) = String.Empty And _
                                CType(phLicences.FindControl(CTRL_PREFIX_ISSYSTEMPROTECTED & licence.ID), HiddenField).Value = Boolean.TrueString.ToString Then
                                msg.Success = False
                                lblError.Text = "This module is system protected and, as a result, the licence can not be removed."
                                e.Cancel = True
                                Exit For
                            End If

                            If Utils.ToString(CType(phLicences.FindControl(CTRL_PREFIX_AUTHORISATION_CODE & licence.ID), TextBoxEx).TextBox.Text.Trim) = String.Empty _
                            Then DeleteClicked(e)
                        End If
                    Next

                    If Not transRolledBack And Not trans Is Nothing And msg.Success Then
                        trans.Commit()
                        ModuleLicence.RefreshCacheLicences(ConnectionStrings("Abacus").ConnectionString)
                    End If

                Catch ex As Exception
                    msg = Utils.CatchError(ex, "E0001 Module Licence Save")     ' unexpected
                    e.Cancel = True
                Finally
                    If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                End Try
            Else
                e.Cancel = True
            End If

            If _stdBut.ButtonsMode = StdButtonsMode.Edit Then
                With _stdBut
                    .AllowDelete = False
                End With
            End If

            If Not e.Cancel Then
                ClearViewState()
                FindClicked(e)
            End If

        End Sub

        Private Sub OutputLicenceHeaders()
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell

            phLicencesHeading.Controls.Clear()

            row = New HtmlTableRow()
            row.ID = "LicenceHeader"

            phLicencesHeading.Controls.Add(row)

            ' Module Name
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Module<br/>Name"

            ' Module Reference
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Module<br/>Reference"

            ' Description
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Description"

            ' Licenced
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Licenced ?"

            ' Expiry Date
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Expiry<br/>Date"

            ' Authorisation Code
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Authorisation<br/>Code"
        End Sub

        Private Sub OutputLicenceInfo(ByVal list As List(Of ViewableLicence))

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim txtAuthoriationCode As TextBoxEx
            Dim hidChanged As HiddenField
            Dim hidIsSystemProtected As HiddenField

            phLicences.Controls.Clear()

            For Each webMod As ViewableLicence In list
                row = New HtmlTableRow()
                row.ID = webMod.ID & CTRL_ROW_SUFFIX
                phLicences.Controls.Add(row)

                ' Module Name
                cell = New HtmlTableCell()
                cell.InnerHtml = webMod.Name
                row.Controls.Add(cell)

                ' Module Reference
                cell = New HtmlTableCell()
                cell.InnerHtml = webMod.Reference
                row.Controls.Add(cell)

                ' Module Description
                cell = New HtmlTableCell()
                cell.InnerHtml = webMod.Description
                row.Controls.Add(cell)

                ' Licenced
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                If webMod.Licenced Then
                    cell.InnerHtml = "YES"
                Else
                    cell.InnerHtml = "NO"
                End If

                cell = New HtmlTableCell()
                With cell
                    If Not webMod.ExpiryDate Is Nothing Then
                        .InnerHtml = ModuleLicence.FormatLicenceDate(webMod.ExpiryDate)
                        .BgColor = ModuleLicence.GetExpiryDateColour(webMod.ExpiryDate)
                    Else
                        .InnerHtml = "&nbsp"
                    End If
                End With
                row.Controls.Add(cell)

                ' Authorisation Code
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                txtAuthoriationCode = New TextBoxEx()
                hidChanged = New HiddenField

                hidIsSystemProtected = New HiddenField
                hidIsSystemProtected.Value = webMod.IsSystemProtected.ToString
                hidIsSystemProtected.ID = CTRL_PREFIX_ISSYSTEMPROTECTED & webMod.ID
                cell.Controls.Add(hidIsSystemProtected)

                With txtAuthoriationCode
                    .ID = CTRL_PREFIX_AUTHORISATION_CODE & webMod.ID
                    .Width = New Unit(20, UnitType.Em)
                    If Not webMod.AuthorisationCode Is Nothing Then .Text = webMod.AuthorisationCode
                End With
                cell.Controls.Add(txtAuthoriationCode)
                txtAuthoriationCode.TextBox.TextMode = TextBoxMode.MultiLine
                txtAuthoriationCode.TextBox.Rows = TEXT_BOX_ROW_SIZE
            Next

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim list As New List(Of ViewableLicence)

            msg = ModuleLicence.getLicenceModules(Me.DbConnection, list, e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            ClearViewState()
            PersistLicencesToViewState(list)
            OutputLicenceHeaders()
            OutputLicenceInfo(list)

            If _stdBut.ButtonsMode = StdButtonsMode.Edit Or _
            _stdBut.ButtonsMode = StdButtonsMode.Initial Then
                With _stdBut
                    .AllowDelete = False
                End With
            End If
        End Sub

#Region " PersistLicencesToViewState "

        Private Sub PersistLicencesToViewState(ByVal list As List(Of ViewableLicence))
            ViewState.Add(VIEWSTATE_KEY_DATA, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER, _newCodeIDCounter)
        End Sub

#End Region

#Region " GetLicencesFromViewState "

        Private Function GetLicencesFromViewState() As List(Of ViewableLicence)

            Dim list As List(Of ViewableLicence)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA) Is Nothing Then
                list = New List(Of ViewableLicence)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA), List(Of ViewableLicence))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER) Is Nothing Then
                _newCodeIDCounter = 0
            Else
                _newCodeIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER), Integer)
            End If

            Return list

        End Function

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState()
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phLicences.Controls.Clear()
            phLicencesHeading.Controls.Clear()
        End Sub

#End Region


        Private Sub Page_PreRenderComplete1(sender As Object, e As System.EventArgs) Handles Me.PreRenderComplete
            Target.Library.Web.Utils.RecursiveDisable(phLicencesHeading.Controls, False)
        End Sub
    End Class
End Namespace
