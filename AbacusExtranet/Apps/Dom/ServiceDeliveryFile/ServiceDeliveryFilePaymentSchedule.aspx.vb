Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library.Web.UserControls
Imports Constants = Target.Library.Web
Imports System.Text
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Web.Apps.Security
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.Dom.ServiceDeliveryFile

    Partial Public Class ServiceDeliveryFilePaymentSchedule
        Inherits Target.Web.Apps.BasePage



        Private _interfaceFileID As Integer
        Private _userID As Integer
        Private _fileID As Integer
        Private _exceptionFileID As Integer
        Private _canViewDomContract As Boolean
        Private _failed As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ViewServiceDeliveryFile"), "Service Delivery File - Payment Schedules")
            _interfaceFileID = Target.Library.Utils.ToInt32(Request.QueryString("fileID"))
            PopulateScreen()

            CType(pScheduleSelector, Apps.UserControls.PaymentScheduleSelector).InitControl(thePage, 1, _interfaceFileID)

        End Sub

        Private Sub PopulateScreen()


            Dim interfaceFile As DataClasses.DomProviderInterfaceFile = New DataClasses.DomProviderInterfaceFile(Me.DbConnection)
            Dim msg As ErrorMessage
            Dim strStyle As New StringBuilder
            Dim user As DataClasses.Users = New DataClasses.Users(Me.DbConnection)

            Try
                strStyle.Append("label.label, label.label2 { float:left; font-weight:bold; }")
                strStyle.Append("label.label2 { width:14em }")
                Me.AddExtraCssStyle(strStyle.ToString)
                'Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ServiceDeliveryFile/ViewServiceDeliveryFile.js"))
                'fetch interfacefile details
                msg = interfaceFile.Fetch(_interfaceFileID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                'output file details to the page
                lblFileRef.Text = interfaceFile.Reference
                lblFileDesc.Text = interfaceFile.Description
                lblFileCreatedOn.Text = WebUtils.EncodeOutput(interfaceFile.FileCreatedDate)
                lblUploadedOn.Text = WebUtils.EncodeOutput(interfaceFile.FileUploadedDate)
                lblUploadedBy.Text = interfaceFile.FileUploadedBy

                'fetch details about the user who uploaded the file
                _userID = interfaceFile.UserID
                'Store FileId for use later
                _fileID = interfaceFile.OriginalFileID

                'Store Exception FileID for use later.
                _exceptionFileID = interfaceFile.ExceptionFileID

                msg = user.Fetch(interfaceFile.UserID)
                If msg.Success Then
                    lblProvOrgan.Text = user.Name
                End If

                lblTotalItems.Text = WebUtils.EncodeOutput(interfaceFile.ServiceDeliveryHeaderCount)

                If interfaceFile.ProcessedDate <> Date.MinValue Then
                    lblProcessedOn.Text = WebUtils.EncodeOutput(interfaceFile.ProcessedDate)
                    lblacceptedItems.Text = WebUtils.EncodeOutput(interfaceFile.AcceptedCount)
                    'lblExceptions.Text = WebUtils.EncodeOutput(interfaceFile.ExceptionCount)
                End If

                If interfaceFile.Deleted Then
                    'lblDeleted.Text = "Yes"
                    'lblDeletedBy.Text = WebUtils.EncodeOutput(interfaceFile.DeletedBy)
                Else
                    'lblDeleted.Text = "No"
                End If

                'Work out the current status of the uploaded file
                'btnViewInvoiceBatches.Disabled = True

                If interfaceFile.Deleted Then
                    lblStatus.Text = "Deleted"
                    lnkStatus.Visible = False
                ElseIf interfaceFile.WorkInProgress Then
                    lblStatus.Text = "Work in Progress"
                    lnkStatus.Visible = True
                ElseIf interfaceFile.Failed Then
                    lblStatus.Text = "Failed, please contact the system administrator."
                    lblStatus.Font.Bold = True
                    lnkStatus.Visible = False
                    _failed = True
                ElseIf interfaceFile.ProcessedDate <> Date.MinValue Then
                    lblStatus.Text = "Processed"
                    'btnViewInvoiceBatches.Disabled = False
                    lnkStatus.Visible = False
                Else
                    lblStatus.Text = "Awaiting Processing"
                    lnkStatus.Visible = True
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                WebUtils.DisplayError(msg)
            Finally

            End Try
        End Sub


    End Class

End Namespace