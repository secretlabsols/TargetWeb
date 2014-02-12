Imports System.Text
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Web.Apps.Security
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports System.Collections.Generic
Imports System.Web.Script.Serialization


Namespace Apps.Dom.ServiceDeliveryFile

    ''' <summary>
    ''' Screen to view a single service delivery file.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Waqas   22/03/2011  D12083
    ''' ColinD  05/05/2010  D11755 - added total items label and tweaks for status of file = 'Failed' i.e. flashing
    ''' MikeVO  15/10/2009  D11546 - top level button tweaks.
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MikeVO  24/11/2008  A4WA#5033 - security fixes.
    ''' </history>
    Partial Class ViewServiceDeliveryFile
        Inherits Target.Web.Apps.BasePage

        Private _interfaceFileID As Integer
        Private _userID As Integer
        Private _fileID As Integer
        Private _exceptionFileID As Integer
        Private _canViewDomContract As Boolean
        Private _failed As Boolean
        Private _backUrl As String = String.Empty

#Region "Property"

        Private _disablePaymentschedule As Boolean
        Public Property disablePaymentschedule() As Boolean
            Get
                Return _disablePaymentschedule
            End Get
            Set(ByVal value As Boolean)
                _disablePaymentschedule = value
            End Set
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ViewServiceDeliveryFile"), "Service Delivery File")
            _interfaceFileID = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            _backUrl = HttpUtility.UrlEncode(Request.Url.ToString())
            PopulateServiceDeliveryFileInfo()

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Dom.ServiceDeliveryFile.ViewServiceDeliveryFile.Startup", _
               Target.Library.Web.Utils.WrapClientScript(String.Format( _
                   "serviceDeliveryFileId={0};disablePaymentschedule={1}", _
                    _interfaceFileID, _
                    disablePaymentschedule.ToString().ToLower() _
               )) _
           )
        End Sub


        Private Sub btnDelete_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.ServerClick

            Dim fileId As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim trans As SqlTransaction = Nothing
            Dim msg As ErrorMessage = New ErrorMessage
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim isServiceDeliveryFileValidForDelete As Boolean = True

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                '' disconnect the service delivery file with batches and invoices
                msg = DomContractBL.UnHookServicedliveryFilesFromBatchsAndInvoices(Nothing, trans, fileId)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    Return
                End If

                '' get list of payment schedules attached with this service delivery file
                Dim pScheduleInterfaceList As DataClasses.Collections.PaymentSchedule_DomProviderInterfaceFileCollection = _
                New DataClasses.Collections.PaymentSchedule_DomProviderInterfaceFileCollection()
                msg = New ErrorMessage
                msg = DataClasses.PaymentSchedule_DomProviderInterfaceFile.FetchList(trans, pScheduleInterfaceList, Nothing, fileId)
                If Not msg.Success Then
                    lblError.Text = msg.Message
                    Return
                End If

                '' get all payment schedules attached to this file
                '' validate each PaymentSchedule for delete.
                '' if a paymetn schedule can be deleted then delete that 
                '' if a payment schedule cannot be deleted then disconnect it from the service delivery file
                For Each pScheduleInterfaceFile As DataClasses.PaymentSchedule_DomProviderInterfaceFile In pScheduleInterfaceList
                    msg = DomContractBL.ValidatePaymentScheduleDelete(Nothing, trans, pScheduleInterfaceFile.PaymentScheduleID, True)
                    If msg.Success Then
                        msg = New ErrorMessage
                        msg = DomContractBL.DeletePaymentSchedule(Nothing, trans, pScheduleInterfaceFile.PaymentScheduleID, True)
                        If Not msg.Success Then
                            lblError.Text = msg.Message
                            Exit For
                        End If
                    Else
                        ' this will delete only the bridge table record.
                        msg = New ErrorMessage
                        pScheduleInterfaceFile.DbTransaction = trans
                        msg = pScheduleInterfaceFile.Delete()
                        If Not msg.Success Then
                            lblError.Text = msg.Message
                            Exit For
                        End If
                    End If

                    msg = New ErrorMessage
                    msg = DomContractBL.PaymentscheduleRecalculateCountsAttributesAndNetValues(trans, pScheduleInterfaceFile.PaymentScheduleID)
                Next


                If msg.Success Then
                    msg = New ErrorMessage
                    '' change status of service delivery file to be set as deleted 
                    Dim serviceDeliveryfile As DataClasses.DomProviderInterfaceFile = New DataClasses.DomProviderInterfaceFile(trans)
                    serviceDeliveryfile.Fetch(fileId)
                    serviceDeliveryfile.Deleted = TriState.True
                    serviceDeliveryfile.DeletedBy = user.ExternalFullName
                    msg = serviceDeliveryfile.Save()
                End If
                If Not msg.Success Then
                    lblError.Text = msg.Message
                End If

                '    'delete the file and info from child tables
                '    msg = DomContractBL.DeleteDomProviderInterfaceChildInformation(trans, Target.Library.Utils.ToInt32(Request.QueryString("id")), user.ExternalUsername)
                '    If Not msg.Success Then
                '        SqlHelper.RollbackTransaction(trans)
                '        lblError.Text = msg.Message
                '    Else
                '        trans.Commit()
                '    End If
                '    'Repopulate the screen
                '    PopulateScreen()
                If msg.Success Then
                    trans.Commit()
                End If
            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                trans.Rollback()
                '    WebUtils.DisplayError(msg)
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

            PopulateScreen()

        End Sub

        Private Sub PopulateScreen()

            Dim interfaceFile As DataClasses.DomProviderInterfaceFile = New DataClasses.DomProviderInterfaceFile(Me.DbConnection)
            Dim msg As ErrorMessage
            Dim strStyle As New StringBuilder
            Const SP_NAME_CONFIRMATION As String = "spxServiceDeliveryFileUploadConfirmation_Fetch"
            Dim reader As SqlDataReader = Nothing
            Dim user As DataClasses.Users = New DataClasses.Users(Me.DbConnection)

            Try
                'strStyle.Append("label.label, label.label2 { float:left; font-weight:bold; }")
                'strStyle.Append("label.label2 { width:16em }")
                strStyle.Append(".ServiceDeliveryFileDownload { background-repeat:no-repeat; background-position:5px 50%; text-indent:28px; background-image:url(save16.png); }")
                Me.AddExtraCssStyle(strStyle.ToString)

                ' add in the jquery library
                Me.UseJQuery = True
                ' add in the jquery ui library
                Me.UseJqueryUI = True
                ' add the jquery searchable menu
                Me.UseJquerySearchableMenu = True
                Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ServiceDeliveryFile/ViewServiceDeliveryFile.js"))

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
                    lblExceptions.Text = WebUtils.EncodeOutput(interfaceFile.ExceptionCount)
                End If

                If interfaceFile.Deleted Then
                    lblDeleted.Text = "Yes"
                    lblDeletedBy.Text = WebUtils.EncodeOutput(interfaceFile.DeletedBy)
                Else
                    lblDeleted.Text = "No"
                End If

                'Work out the current status of the uploaded file
                'btnViewInvoiceBatches.Disabled = True
                disablePaymentschedule = True
                If interfaceFile.Deleted Then
                    lblStatus.Text = "Deleted"
                    btnDelete.Disabled = True
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
                    disablePaymentschedule = False
                    lnkStatus.Visible = False
                Else
                    lblStatus.Text = "Awaiting Processing"
                    lnkStatus.Visible = True
                End If

                If interfaceFile.WorkInProgress = TriState.True Then
                    btnDelete.Disabled = True
                End If

                'Populate list of contracts included in file
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_CONFIRMATION, False)
                spParams(0).Value = _interfaceFileID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_CONFIRMATION, spParams)
                rptProviderContracts.DataSource = reader
                rptProviderContracts.DataBind()
                reader.Close()

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                WebUtils.DisplayError(msg)
            Finally
                If Not reader.IsClosed Then reader.Close()
            End Try
        End Sub

        Protected Function GetContractLink(ByVal contractID As Integer, ByVal text As String) As String
            If _canViewDomContract Then
                Return String.Format("<a href=""../Contracts/ViewDomiciliaryContract.aspx?id={0}&backUrl={1}"">{2}</a>", contractID, HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString()), text)
            Else
                Return text
            End If
        End Function

        Protected Sub PopulateServiceDeliveryFileInfo()
            Const SP_CAN_VIEW As String = "spxDomProviderInterfaceFile_CanView"

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim spParams As SqlParameter()
            Dim startupScript As StringBuilder = New StringBuilder()
            Dim menuItems As List(Of ServiceDeliveryFileDownloadMenuItem) = New List(Of ServiceDeliveryFileDownloadMenuItem)(2)
            Dim menuItem As ServiceDeliveryFileDownloadMenuItem
            Dim jsSerializer As JavaScriptSerializer = New JavaScriptSerializer()
            Dim jsonMenuItems As String = "[]"

            _canViewDomContract = _
                SecurityBL.UserHasMenuItem(Me.DbConnection, _
                                            currentUser.ID, _
                                            Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ViewDomiciliaryContract"), _
                                            Me.Settings.CurrentApplicationID)

            ' ensure user is permitted to view the requested file
            Try
                spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_CAN_VIEW, False)
                spParams(0).Value = _interfaceFileID
                spParams(1).Value = currentUser.ExternalUserID
                spParams(2).Direction = ParameterDirection.InputOutput

                SqlHelper.ExecuteNonQuery(Me.DbConnection, CommandType.StoredProcedure, SP_CAN_VIEW, spParams)

                If Not Convert.ToBoolean(spParams(2).Value) Then
                    Response.Redirect("~/Library/Errors/404.aspx")
                End If

            Catch ex As Exception
                WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))
            End Try


            PopulateScreen()

            ' setup the top buttons
            btnDelete.Visible = Me.UserHasMenuItemCommand( _
                Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItemCommand.ViewServiceDeliveryFile.Delete") _
            )

            If _fileID <> 0 Then
                menuItem = New ServiceDeliveryFileDownloadMenuItem()
                With menuItem
                    .description = "Original File"
                    .cssClass = "ServiceDeliveryFileDownload"
                    .fileID = _fileID
                End With
                menuItems.Add(menuItem)
            End If

            'btnViewInvoiceBatches.Attributes.Add("onclick", String.Format("document.location.href='../ProformaInvoice/InvoiceEnquiry.aspx?fileID={0}&currentStep=3';", _interfaceFileID))
            'btnPaymentSchedules.Attributes.Add("onclick", String.Format("document.location.href='ServiceDeliveryFilePaymentSchedule.aspx?fileID={0}&id={1}&currentStep=3'&backUrl={2};", _interfaceFileID, _interfaceFileID, _backUrl))

            If _exceptionFileID <> 0 Then
                menuItem = New ServiceDeliveryFileDownloadMenuItem()
                With menuItem
                    .description = "Exceptions File"
                    .cssClass = "ServiceDeliveryFileDownload"
                    .fileID = _exceptionFileID
                End With
                menuItems.Add(menuItem)

                btnViewExceptions.Attributes.Add("onclick", String.Format("javascript:btnViewExceptions_Click({0})", _exceptionFileID))
                btnViewExceptions.Disabled = False
                startupScript.AppendFormat( _
                                    "FlashObject('{0}', 'flashedInput');FlashObject('{1}', 'flashedText');FlashObject('{2}', 'flashedText');", _
                                    btnViewExceptions.ClientID, _
                                    lblExceptionsLabel.ClientID, _
                                    lblExceptions.ClientID _
                                )


            Else
                btnViewExceptions.Disabled = True
            End If

            If _failed Then

                startupScript.AppendFormat("FlashObject('{0}', 'flashedText');", lblStatus.ClientID)

            End If

            If startupScript.Length > 0 Then
                'startupScript.Length = 0
                startupScript.AppendFormat("highlightExceptionsScript=""{0}"";", startupScript.ToString())
            End If

            ' serialize the object to a json string
            With jsSerializer
                If Not menuItems Is Nothing Then
                    jsonMenuItems = .Serialize(menuItems.ToArray())
                End If
            End With
            startupScript.AppendFormat("var {0}_menuItems = {1};btnDownloadID=""{0}""; btnPaymentSchedulesID=""btnPaymentSchedules""", Me.btnDownload.ClientID, jsonMenuItems)

            ClientScript.RegisterStartupScript( _
                    Me.GetType(), _
                    "startupScript", _
                    startupScript.ToString(), _
                    True _
                )

        End Sub
    End Class

End Namespace