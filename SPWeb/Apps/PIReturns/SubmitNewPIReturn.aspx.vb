Imports System.Collections.Specialized
Imports Target.Web.Apps.Security
Imports Target.SP.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.SP.Library
Imports Target.SP.Library.SPClassesBL
Imports Target.Library.Web
Imports Target.Library
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Web.Apps.FileStore
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Net.SourceForge.Koogra
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.PIReturns

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.PIReturns.SubmitNewPIReturn
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Screen to allow users to submit a new PI return.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO  29/03/2007  Changed error handling so that invalid workbook errors are displayed nicely.
    '''     MikeVO  29/03/2007  Fixed issue where temp upload file was not being deleted
    '''                         when ValidatePIWorkbookDoc() cannot read the workbook.
    '''     MikeVO  26/03/2007  Changed error logic when validating a workbook.
    ''' 	[paul]	??/??/??	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class SubmitNewPIReturn
        Inherits Target.Web.Apps.BasePage

        Protected SelectorWizard1 As Target.Library.Web.UserControls.SelectorWizardBase

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_FINANCIALYEAR As String = "finYr"
        Const QS_SERVICEID As String = "serviceID"
        Const QS_QUARTER As String = "qtr"
        Const STEP_FIN_YR_QTR As Integer = 2
        Const STEP_INDEX_ATTACH_DOC As Integer = 3
        Const STEP_INDEX_COMPLETE As Integer = 4
        Private _addFileStep As NewPIReturnAttachDocStep

        Private Sub Page_init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            Me.InitPage(ConstantsManager.GetConstant("webSecurityItemPIReturnsSubmitNew"), "Submit New PI Return")

            Dim currentStep As Integer = Target.Library.Utils.ToInt32(Request.QueryString(QS_CURRENTSTEP))
            Dim provStep As ProviderStep
            Dim svcStep As ServiceStep
            Dim enterAttachDocStep As New NewPIReturnAttachDocStep
            Dim finYrQtrStep As New FinancialYrQtrStep

            ' setup the steps
            provStep = New ProviderStep
            provStep.Required = False
            svcStep = New ServiceStep
            svcStep.Required = True
            _addFileStep = New NewPIReturnAttachDocStep
            _addFileStep.Required = True
            finYrQtrStep = New FinancialYrQtrStep
            finYrQtrStep.Required = True

            With SelectorWizard1
                ' add the steps
                .Steps.Add(provStep)
                .Steps.Add(svcStep)
                .Steps.Add(finYrQtrStep)
                .Steps.Add(_addFileStep)
                .Steps.Add(New NewPIReturnCompleteStep)

                ' set the display of the new button
                .NewButton.Value = "New PI Return"
                .NewButton.Attributes("title") = "Click here to Submit a New PI Return."

                ' hide the finish button when not on the attach doc step
                If currentStep <> STEP_INDEX_ATTACH_DOC Then .FinishButton.Visible = False
                If currentStep = STEP_INDEX_COMPLETE Then
                    .BackButton.Visible = False
                    .ShowHeaderControls = False
                End If
                .InitControl()
            End With

            If currentStep = STEP_INDEX_ATTACH_DOC Then Me.Form.Enctype = "multipart/form-data"

        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim msg As ErrorMessage
            Dim workbookValidated As Boolean

            If IsPostBack Then
                Validate()
                If IsValid Then
                    ' upload the scanned doc (if specified) and submit for processing
                    msg = UploadPIWorkbookDoc(Me.DbConnection, workbookValidated)
                    If Not msg.Success Then
                        WebUtils.DisplayError(msg)
                    ElseIf workbookValidated Then
                        MyRedirect(STEP_INDEX_COMPLETE)
                    End If
                End If
            End If
        End Sub

    Private Sub MyRedirect(ByVal stepID As Integer)
            Dim newQS As NameValueCollection = New NameValueCollection(Request.QueryString)
            newQS.Remove(QS_CURRENTSTEP)
            newQS.Add(QS_CURRENTSTEP, stepID)
            Response.Redirect(String.Format("SubmitNewPIReturn.aspx?{0}", WebUtils.GetNameValueCollectionAsString(newQS)))
        End Sub

        Private Function UploadPIWorkbookDoc(ByVal conn As SqlConnection, ByRef workbookValidated As Boolean) As ErrorMessage
            Dim msg As ErrorMessage = Nothing
            Dim scannedDocIDs As Integer()
            Dim newDataFileID As Integer
            Dim webSPPISubQueue As WebSPPISubmissionQueue
            Dim trans As SqlTransaction = Nothing
            Dim tempData As WebFileStoreTempUpload
            Dim financialYear As String = Request.QueryString(QS_FINANCIALYEAR)
            Dim quarter As String = Request.QueryString(QS_QUARTER)
            Dim serviceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString(QS_SERVICEID))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim valResults As StringBuilder
            Dim newPISubQueueID As Integer
            Dim periodFrom As Date, periodTo As Date

            Try
                trans = SqlHelper.GetTransaction(conn)

                ' get the uploaded doc ID
                scannedDocIDs = FileStoreBL.GetUploadedTempFileIDs()

                'validate here
                tempData = New WebFileStoreTempUpload(trans)
                If scannedDocIDs.Length > 0 Then
                    msg = tempData.Fetch(scannedDocIDs(0))
                    If Not msg.Success Then Return msg
                End If

                valResults = New StringBuilder

                If tempData.Data Is Nothing Then
                    msg = New ErrorMessage
                    msg.Number = "E0517"
                    tempData.Delete()
                    trans.Commit()
                    _addFileStep.ValResults.Text = "File does not exist or is not a valid file."
                    Return msg
                End If

                msg = ValidatePIWorkbookDoc(tempData.Data, ConnectionStrings("Abacus").ConnectionString, financialYear, _
                                                            quarter, serviceID, workbookValidated, valResults)

                If Not msg.Success Then
                    If msg.Number = "E2002" Then
                        ' could not read workbook
                        ' output nice error message to screen rather than a "kaboom" error
                        _addFileStep.ValResults.Text = msg.Message
                        msg = New ErrorMessage
                        msg.Success = True
                    End If
                    ' delete the temp upload file
                    tempData.Delete()
                    trans.Commit()
                    Return msg
                End If

                If workbookValidated Then
                    ' move the uploaded doc to proper storage
                    If scannedDocIDs.Length > 0 Then
                        msg = FileStoreBL.MoveFromTempStoreToDataStore(trans, scannedDocIDs(0), newDataFileID)
                        If Not msg.Success Then Return msg
                    End If

                    'Create Record WebSPPISubmissionQueue
                    webSPPISubQueue = New WebSPPISubmissionQueue
                    With webSPPISubQueue
                        .DbConnection = Me.DbConnection
                        .DbTransaction = trans
                        .SPServiceID = serviceID
                        .FinancialYear = financialYear
                        .Quarter = quarter
                        msg = GetPeriodCoveredForFinYrQtr(ConnectionStrings("Abacus").ConnectionString, financialYear, quarter, periodFrom, periodTo)
                        If Not msg.Success Then Return msg
                        .PeriodCoveredFrom = periodFrom
                        .PeriodCoveredTo = periodTo
                        .Comments = _addFileStep.txtComment.Text
                        .SubmittedByUserID = user.ID
                        .SubmissionDate = Now
                        .Status = 2
                        .WebFileStoreDataID = newDataFileID

                        msg = .Save()
                        If Not msg.Success Then Return msg
                        newPISubQueueID = .ID
                    End With

                    ' commit
                    trans.Commit()

                    msg = New ErrorMessage
                    msg.Success = True

                Else
                    tempData.Delete()
                    _addFileStep.ValResults.Text = valResults.ToString
                    trans.Commit()
                    msg = New ErrorMessage
                    msg.Success = True
                End If

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0502", "UploadPIWorkbookDoc")   ' error whilst saving data
            Finally
                If Not msg.Success AndAlso Not trans Is Nothing AndAlso Not trans.Connection Is Nothing Then trans.Rollback()
            End Try

            Return msg

        End Function

    End Class
End Namespace