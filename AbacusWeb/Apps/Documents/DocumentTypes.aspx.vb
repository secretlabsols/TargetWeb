Imports Target.Web.Apps
Imports Target.Library.Web.UserControls
Imports Target.Library
Imports Target.Web.Apps.Security
Imports DAL = Target.Abacus.Library.DataClasses
Imports Docs = Target.Abacus.Library.Documents
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library.Documents
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library
Imports Target.Library.Web.Adapters
Imports System.Collections.Generic


Namespace Apps.Documents

    ''' <summary>
    ''' Manages the document types (which are core part of the document framework)
    ''' Each created document belongs to a document type e.g. PERSONAL_BUDGET_STATEMENT,
    ''' DEBTOR_INVOICE_SDS_V2 etc. 
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Mo Tahir    21/04/2011  SDS-Issue-522  
    ''' IHS         29/03/2011  D11960 - populated printer section
    ''' IHS         11/03/2011  D11915 - added comments
    ''' </history>
    Partial Public Class DocumentTypes
        Inherits BasePage

#Region " Private Members "

        Private _isSystemType As Boolean = False
        Private _stdBut As StdButtonsBase
        Private _InvalidIcon As String = Target.Library.Web.Utils.GetVirtualPath("Images/Blocked.png")
        Private _ValidIcon As String = Target.Library.Web.Utils.GetVirtualPath("Images/Complete.png")

#End Region

#Region " Public Members "

        Public ReadOnly Property ValidIcon() As String
            Get
                Return _ValidIcon
            End Get
        End Property

        Public ReadOnly Property InvalidIcon() As String
            Get
                Return _InvalidIcon
            End Get
        End Property

        Public ReadOnly Property EditMode() As StdButtonsMode
            Get
                Return (_stdBut.ButtonsMode = StdButtonsMode.Edit Or _stdBut.ButtonsMode = StdButtonsMode.AddNew)
            End Get
        End Property

        Public documentPrinterID As Integer = -1
        Public paperSourceID As Integer = -1
        Public paperSizeID As Integer = -1
        Public printOnID As Integer = -1

#End Region

#Region " Event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Documents.ReferenceData.DocumentTypes"), "Document Types")
            Me.UseJQuery = True

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DocumentType.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DocumentType.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DocumentType.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsDetails.Controls)
                .EditableControls.Add(fsPrinting.Controls)
                .GenericFinderTypeID = GenericFinderType.DocumentType
                .AuditLogTableNames.Add("DocumentType")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DocumentTypes")
            End With

            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked
            AddHandler _stdBut.NewClicked, AddressOf NewClicked

            ' add fancy drop down list jquery script
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/ImageDropdown/ImageDropdown.js"))
            ' add fancy drop down css
            Me.CssLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/jQuery/Plugins/ImageDropdown/ImageDropdown.css"))
            ' add help utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add("DocumentTypes.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Documents))

            cboPrinter.DropDownList.Attributes.Add("onchange", "setDropDownValue = false; cboPrinter_OnChange()")
        End Sub

        Private Sub txtFileNamePattern_AfterTextBoxControlAdded(ByVal sender As Target.Library.Web.Controls.TextBoxEx) Handles txtFileNamePattern.AfterTextBoxControlAdded

            ' add the 'popup' help icon (shows question mark)

            Dim anchor As HtmlAnchor = New HtmlAnchor
            Dim image As HtmlImage = New HtmlImage
            Dim space As Literal = New Literal

            space.Text = "&nbsp;"
            sender.Controls.Add(space)

            With image
                .Src = "~/Images/help16.png"
            End With

            With anchor
                .HRef = "javascript:ShowHelp();"
                .Controls.Add(image)
            End With

            sender.Controls.Add(anchor)

        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            FindClicked(e)

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            Me.Clear()

            txtSystemType.Text = Utils.SplitOnCapitals(DocumentSystemType.UserDefined.ToString)

            txtFileNamePattern.Visible = False

            chkRedundant.CheckBox.Checked = False

            chkPublish.CheckBox.Checked = False
            chkUpload.CheckBox.Checked = False

            txtDescription.SetFocus = True

            BindPrinters()
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim docType As DAL.DocumentType
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            docType = New DAL.DocumentType(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            With docType
                msg = DocumentTypeBL.Fetch(e.ItemID, docType)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                txtFileNamePattern.Text = .FileNamePattern
                txtRePrintWatermark.Text = .RePrintWatermarkText
                _isSystemType = (.SystemType = DocumentOrigin.SystemGenerated)

                ' if system type is larger than 0 then is a system type
                If _isSystemType Then
                    txtSystemType.Text = Utils.SplitOnCapitals(DocumentSystemType.SystemDefined.ToString)
                    txtFileNamePattern.Visible = True
                Else
                    txtSystemType.Text = Utils.SplitOnCapitals(DocumentSystemType.UserDefined.ToString)
                    txtFileNamePattern.Visible = False
                    chkRedundant.CheckBox.Checked = .Redundant
                End If
                chkPublish.CheckBox.Checked = .PublishToExtranet
                chkUpload.CheckBox.Checked = .UploadViaExtranet

                ' system types cannot be deleted so deny access to delete button
                SetDeleteButton()

                documentPrinterID = .DocumentPrinterID
                paperSourceID = .DocumentPrinterPaperSourceID
                paperSizeID = .DocumentPrinterPaperSizeID
                printOnID = .DuplexSetting

                ' populate printers drop down list
                BindPrinters()
            End With
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                Me.Clear()
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim list As New DocumentCollection
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' check if this document type is in use...
            msg = Document.FetchList(Me.DbConnection, list:=list, documentTypeID:=e.ItemID)

            If (list.Count > 0) Then
                Dim docType As New DAL.DocumentType(Me.DbConnection, String.Empty, String.Empty)
                DocumentTypeBL.Fetch(e.ItemID, docType)
                msg.Success = False
                msg.Message = String.Format("Document type ""{0}"" is in-use and cannot be deleted.", docType.Description)
                WebUtils.DisplayError(msg)
            End If

            msg = DocumentTypeBL.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)

            If msg.Success Then
                e.ItemID = 0
                _stdBut.SelectedItemID = 0
            Else
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim docType As DAL.DocumentType
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Me.ResetValidators()

            Me.Validate("Save")

            If Not Me.IsValid Then
                e.Cancel = True
                Exit Sub
            End If

            docType = New DAL.DocumentType(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            If e.ItemID > 0 Then
                '++ Update of existing record..
                With docType
                    msg = DocumentTypeBL.Fetch(e.ItemID, docType)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With
            End If

            With docType
                ' set values depending on whether Document Type is System Type 
                If .SystemType = DocumentOrigin.SystemGenerated Then
                    txtDescription.Text = .Description
                    chkRedundant.CheckBox.Checked = .Redundant
                    txtDescription.Enabled = False
                    chkRedundant.CheckBox.Enabled = False
                Else
                    .Description = txtDescription.Text
                    .FileNamePattern = txtFileNamePattern.Text
                    .Redundant = chkRedundant.CheckBox.Checked
                    .SystemType = DocumentOrigin.UserUploaded
                    txtDescription.Enabled = True
                    chkRedundant.CheckBox.Enabled = True
                End If

                .PublishToExtranet = chkPublish.CheckBox.Checked
                .UploadViaExtranet = chkUpload.CheckBox.Checked

                .FileNamePattern = txtFileNamePattern.Text
                .RePrintWatermarkText = txtRePrintWatermark.Text

                SetDropDownValue(.DocumentPrinterID, cboPrinter)
                SetDropDownValue(.DocumentPrinterPaperSourceID, cboPaperTray)
                SetDropDownValue(.DocumentPrinterPaperSizeID, cboPaperSize)
                SetDropDownValue(.DuplexSetting, cboPrintOn)

                documentPrinterID = .DocumentPrinterID
                paperSourceID = .DocumentPrinterPaperSourceID
                paperSizeID = .DocumentPrinterPaperSizeID
                printOnID = .DuplexSetting

                BindPrinters()

                msg = DocumentTypeBL.Save(Me.DbConnection, docType)

                If msg.Success Then
                    lblError.Text = ""
                Else
                    lblError.Text = msg.Message
                    e.Cancel = True
                End If
                e.ItemID = .ID
            End With

        End Sub

        ''' <summary>
        ''' Adds item to valid/invalid drop down option group 
        ''' and also prefixes description with valid/invalid icon
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        ''' <history>
        ''' Iftikhar  24/03/2011  Created (D11960)
        ''' </history>
        Private Sub cboPrinter_DataBound(ByVal sender As Object, ByVal e As System.EventArgs)
            For Each item As ListItem In cboPrinter.DropDownList.Items
                If item.Text.StartsWith("1_") Then
                    ' add item to "Valid" drop down option group
                    item.Attributes(DropDownListAdapter.OPTGROUP_ATTRIBUTE_NAME) = "Valid"
                    item.Attributes("title") = _ValidIcon
                Else
                    ' add item to "Not Valid" drop down option group
                    item.Attributes(DropDownListAdapter.OPTGROUP_ATTRIBUTE_NAME) = "Not Valid"
                    item.Attributes("title") = _InvalidIcon
                End If

                ' remove the "1_" or "0_" from beginning of printer name
                item.Text = item.Text.Remove(0, 2)

                item.Selected = (item.Value = CType(documentPrinterID, String))
            Next
        End Sub

        Private Sub DocumentTypes_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            If _stdBut.ButtonsMode = StdButtonsMode.Edit Or _stdBut.ButtonsMode = StdButtonsMode.AddNew Then

                txtDescription.Enabled = Not _isSystemType
                txtFileNamePattern.Enabled = _isSystemType
                chkRedundant.CheckBox.Enabled = Not _isSystemType


                If _isSystemType Then
                    txtFileNamePattern.SetFocus = True
                Else
                    txtDescription.SetFocus = True
                End If

            End If

            If _stdBut.ButtonsMode = StdButtonsMode.Fetched Then
                ' system types cannot be deleted so deny access to delete button
                SetDeleteButton()
            End If
        End Sub

#End Region

#Region " Private Methods "

        ' clear all fields
        Private Sub Clear()
            txtDescription.Text = String.Empty
            txtSystemType.Text = String.Empty
            txtFileNamePattern.Text = String.Empty
            txtRePrintWatermark.Text = String.Empty
            chkRedundant.CheckBox.Checked = False
            cboPrinter.DropDownList.SelectedIndex = -1
        End Sub

        Private Function BindPrinters() As ErrorMessage
            Try
                Dim msg As New ErrorMessage
                Dim printers As New DocumentPrinterCollection
                Dim listPrinters As New List(Of ListItem)
                Dim listItem As ListItem
                Dim desc As String

                AddHandler cboPrinter.DropDownList.DataBound, AddressOf cboPrinter_DataBound

                msg = DocumentPrinterBL.FetchList(Me.DbConnection, printers)
                If Not msg.Success Then Return msg

                For Each printer As DAL.DocumentPrinter In printers
                    ' prefix item desc with "1_" or "0_" depending if its valid or invalid
                    ' (this prefix gets removed in cboPrinter_DataBound event)
                    desc = String.Format("{0}{1}", IIf(printer.IsValid, "1_", "0_"), printer.PrinterName)

                    listItem = New ListItem(desc, printer.ID)

                    listPrinters.Add(listItem)
                Next

                cboPrinter.DropDownList.DataTextField = "Text"
                cboPrinter.DropDownList.DataValueField = "Value"
                cboPrinter.DropDownList.DataSource = listPrinters
                cboPrinter.DropDownList.DataBind()

                ' insert empty item at top - to enforce printer selection
                cboPrinter.DropDownList.Items.Insert(0, New ListItem("&nbsp;", String.Empty))

                Return msg
            Catch ex As Exception
                Return Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
            End Try
        End Function

        Private Sub ResetValidators()
            Dim printerSelected As Boolean

            ' enable/disable paper tray, size and duplex drop downs based on if printer is selected
            ' and also if drop down had no value selected at client side

            printerSelected = (Utils.ToInt32(cboPrinter.GetPostBackValue()) > 0)

            cboPaperTray.RequiredValidator.Enabled = printerSelected AndAlso Not cboPaperTray.GetPostBackValue() > 0
            cboPaperSize.RequiredValidator.Enabled = printerSelected AndAlso Not cboPaperSize.GetPostBackValue() > 0
            cboPrintOn.RequiredValidator.Enabled = printerSelected AndAlso Not (cboPrintOn.GetPostBackValue() > 0 OrElse _
                                                                                cboPrintOn.GetPostBackValue() = -1)

        End Sub

        Private Sub SetDropDownValue(ByRef setTo As Integer, ByVal cboObject As Target.Library.Web.Controls.DropDownListEx)
            If (Utils.ToInt32(cboObject.GetPostBackValue()) > 0) Then
                setTo = Utils.ToInt32(cboObject.GetPostBackValue())
            Else
                setTo = Nothing
            End If
        End Sub

        Private Sub SetDropDownValue(ByRef setTo As Short, ByVal cboObject As Target.Library.Web.Controls.DropDownListEx)
            Dim tmpSetTo As Short

            If Short.TryParse(cboObject.GetPostBackValue(), tmpSetTo) Then
                setTo = tmpSetTo
            Else
                setTo = Nothing
            End If
        End Sub

        Private Sub SetDeleteButton()
            _stdBut.AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DocumentType.Delete"))
            If _stdBut.AllowDelete Then
                ' system types cannot be deleted so deny access to delete button
                _stdBut.AllowDelete = Not _isSystemType
            End If
        End Sub

#End Region

    End Class


End Namespace
