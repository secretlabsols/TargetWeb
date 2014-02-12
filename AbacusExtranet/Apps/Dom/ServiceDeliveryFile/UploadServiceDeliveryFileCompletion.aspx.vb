Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Library
Imports System.Collections.Generic
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Jobs.Core
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.Core

Namespace Apps.Dom.ServiceDeliveryFile

    ''' <summary>
    ''' Screen to signify successful upload of a service delivery file.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Waqas   22/03/2011  D12083
    ''' MikeVO  15/06/2009  D11515 - added support for email notifications.
    ''' MikeVO  13/03/2009  D11297B - changed to correctly use the new JobTypes enum.
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MikeVO  24/11/2008  A4WA#5033 - security fixes.
    ''' </history>
    Partial Class UploadServiceDeliveryFileCompletion
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim trans As SqlTransaction = Nothing
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage
            Dim reader As SqlDataReader = Nothing
            Dim interfaceFileID As Integer = Utils.ToInt32(Session("DomProviderInterfaceFileID"))
            Dim valCon_Original As String = Request.QueryString("valcon")
            Dim valCon_Current As String = ""
            Dim fileReference As String = ""
            Dim domProviderInterfaceFileID_Existing As Integer
            Dim domProviderInterfaceFileID_New As Integer
            Dim exceptionsFound As Boolean = False

            Const SP_NAME_FETCH_FILE As String = "spxDomProviderInterfaceFile_FetchByUserIdAndFileID"

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ServiceDeliveryFileEnquiry"), "Upload Service Delivery File Completion")
            Try
                'Start Transaction
                trans = SqlHelper.GetTransaction(Me.DbConnection)
                'Get File Reference from DomProviderInterfaceFile table
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(trans.Connection, SP_NAME_FETCH_FILE, False)
                spParams(0).Value = user.ExternalUserID
                spParams(1).Value = interfaceFileID
                reader = SqlHelper.ExecuteReader(trans, CommandType.StoredProcedure, SP_NAME_FETCH_FILE, spParams)
                While reader.Read
                    fileReference = reader("Reference")
                    domProviderInterfaceFileID_New = reader("ID")
                End While
                reader.Close()

                'Lock user Row
                msg = DomContractBL.ValidateServiceDeliveryFileAgainstUploadedFiles(trans, user.ExternalUserID, fileReference, domProviderInterfaceFileID_Existing, True)
                If Not msg.Success Then
                    If msg.Number = DomContractBL.ERR_SERVICE_DELIVERY_FILE_3B_MET Or msg.Number = DomContractBL.ERR_SERVICE_DELIVERY_FILE_3C_MET Then
                        'Condition 3b has been met but was not in previous step.
                        If msg.Number = DomContractBL.ERR_SERVICE_DELIVERY_FILE_3B_MET And valCon_Original <> DomContractBL.ERR_SERVICE_DELIVERY_FILE_3B_MET Then
                            lblError.Text = String.Format("A file with the same reference ({0}) has been uploaded since the file was validated.  Please start the file upload process again.", fileReference)
                            exceptionsFound = True
                        End If
                        'Condition 3c has been met but was not in previous step.
                        If msg.Number = DomContractBL.ERR_SERVICE_DELIVERY_FILE_3C_MET And valCon_Original <> DomContractBL.ERR_SERVICE_DELIVERY_FILE_3C_MET Then
                            lblError.Text = String.Format("A previously uploaded file with the same reference ({0}) has been amended since this file was validated.  Please start the file upload process again.", fileReference)
                            exceptionsFound = True
                        End If
                    Else
                        lblError.Text = msg.Message
                        exceptionsFound = True
                    End If
                End If

                If Not exceptionsFound Then
                    If domProviderInterfaceFileID_Existing <> 0 Then
                        'rows were located in the Dom Provider Interface File table
                        'mark DomProviderInterfaceFile table as deleted and delete all child information
                        msg = DomContractBL.DeleteDomProviderInterfaceChildInformation(trans, domProviderInterfaceFileID_Existing, user.ExternalUsername)
                        If Not msg.Success Then
                            SqlHelper.RollbackTransaction(trans)
                            WebUtils.DisplayError(msg)
                        End If
                    End If

                    'Mark DomProviderinterfaceFile record as confirmed
                    Dim domProvInterfaceFile As DataClasses.DomProviderInterfaceFile = New DataClasses.DomProviderInterfaceFile(trans)
                    With domProvInterfaceFile
                        msg = .Fetch(domProviderInterfaceFileID_New)
                        If Not msg.Success Then
                            SqlHelper.RollbackTransaction(trans)
                            WebUtils.DisplayError(msg)
                        End If
                        .Confirmed = TriState.True
                        msg = .Save()
                        If Not msg.Success Then
                            SqlHelper.RollbackTransaction(trans)
                            WebUtils.DisplayError(msg)
                        End If
                    End With

                    'Audit logging

                    'Add Job to the job Service que.
                    msg = CreateJob(interfaceFileID, trans)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If

                    trans.Commit()

                    lblconfirmation.Text = "Congratulations, the file has been successfully uploaded and will be processed in due course. " & _
                                            "Use the link below to review the progress of this file.<br /><br />"
                    lnkServiceDelEnq.Visible = True
                    lnkServiceDelEnq.Text = "Service Delivery File Enquiry"
                    lnkServiceDelEnq.NavigateUrl = "ServiceDeliveryFileEnquiry.aspx"

                    Dim backUrl As String = String.Empty
                    If Not Session("backurl") Is Nothing Then
                        backUrl = Server.UrlEncode(Session("backurl"))
                        ' remove from session 
                        Session.Remove("backurl")
                    End If

                    Dim serviceDeliveryfile As String = Session("DomProviderInterfaceFileID")
                    Session("DomProviderInterfaceFileID") = 0
                    Response.Redirect(String.Format("ViewServiceDeliveryFile.aspx?id={0}&backUrl={1}", serviceDeliveryfile, backUrl))

                Else
                    SqlHelper.RollbackTransaction(trans)
                End If
            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                WebUtils.DisplayError(msg)
            End Try
            

        End Sub

        Private Function CreateJob(ByVal fileID As Integer, ByRef trans As SqlTransaction) As ErrorMessage
            Dim msg As ErrorMessage = Nothing
            Dim currentUser As WebSecurityUser
            Dim input As vwJobStepInputs
            Dim inputs As vwJobStepInputsCollection = Nothing
            Dim newInputs As List(Of Triplet)

            Dim jobID As Integer

            currentUser = SecurityBL.GetCurrentUser()

            Me.Validate()

            If Me.IsValid Then

                Try

                    ' get the list of inputs with their default values for this job type
                    msg = JobServiceBL.GetNewJobInputs(trans, JobTypes.ProcessServiceDeliveryFile, inputs)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ' build the inputs
                    newInputs = New List(Of Triplet)
                    input = inputs(0)
                    With input
                        newInputs.Add(New Triplet(.JobStepTypeID, .InputName, fileID))
                    End With


                    ' create the job with the new inputs
                    msg = JobServiceBL.CreateNewJob(trans, _
                                                    JobTypes.ProcessServiceDeliveryFile, _
                                                    Nothing, _
                                                    currentUser.ExternalUsername, _
                                                    DateTime.Now, _
                                                    Nothing, _
                                                    currentUser.ExternalUserID, _
                                                    currentUser.Email, _
                                                    newInputs, _
                                                    jobID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)


                Catch ex As Exception
                    msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected

                End Try

            End If
            Return msg
        End Function

    End Class
End Namespace