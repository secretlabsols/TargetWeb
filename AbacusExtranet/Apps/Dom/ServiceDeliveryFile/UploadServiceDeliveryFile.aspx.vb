Imports System.Data.SqlClient
Imports Target.Web.Apps.FileStore
Imports Target.Library.ApplicationBlocks.DataAccess
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports Target.Library
Imports Target.Abacus.Library
Imports System.Xml
Imports System.IO


Namespace Apps.Dom.ServiceDeliveryFile

    ''' <summary>
    ''' Scree to allow the upload of service delivery files.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Waqas   22/03/2011  D12083
    ''' MikeVO  10/03/2009  A4WA#5287 - change EncodeXmlDoc() to use InnerText.
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MikeVO  24/11/2008  A4WA#5033 - security fixes.
    ''' MikeVO  24/11/2008  A4WA#5030 - security fixes.
    ''' </history>
    Partial Class UploadServiceDeliveryFile
        Inherits Target.Web.Apps.BasePage

        Private Sub UploadServiceDeliveryFile_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ServiceDeliveryFileEnquiry"), "Upload Service Delivery File")
            Me.Form.Enctype = "multipart/form-data"
            Me.JsLinks.Add("UploadServiceDeliveryFile.js")
        End Sub

        Protected Sub UploadServiceDeliveryFile_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim Msg As ErrorMessage = Nothing
            If IsPostBack Then
                Msg = UploadServiceDeliveryFile(Me.DbConnection)
                If Not Msg.Success Then
                    WebUtils.DisplayError(Msg)
                End If
            End If



            If (Not Request.QueryString("backUrl") Is Nothing) Then
                Session.Add("backUrl", Request.QueryString("backUrl"))
            End If

        End Sub
        ''' <summary>
        ''' Procedure to upload and validate Service delivery file
        ''' </summary>
        ''' <param name="conn"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  15/10/2009     D11546 - added support for DomProviderInterfaceFile.FileUploadedByID
        ''' PaulW   13-Feb-2008    Initial Version
        ''' </history>
        Private Function UploadServiceDeliveryFile(ByVal conn As SqlConnection) As ErrorMessage
            Dim msg As ErrorMessage = Nothing
            Dim scannedDocIDs As Integer()
            Dim newDataFileID As Integer
            Dim interfaceFileID As Integer
            Dim tempData As WebFileStoreTempUpload
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim queryString As String = ""
            Dim interfaceFile As MemoryStream = Nothing
            Dim xmlDoc As XmlDocument = New XmlDocument


            ' get the uploaded doc ID
            scannedDocIDs = FileStoreBL.GetUploadedTempFileIDs()

            tempData = New WebFileStoreTempUpload(Me.DbConnection)
            If scannedDocIDs.Length > 0 Then
                msg = tempData.Fetch(scannedDocIDs(0))
                If Not msg.Success Then Return msg
            End If

            If tempData.Data Is Nothing Then
                msg = New ErrorMessage
                msg.Number = "E0517"
                tempData.Delete()
                Return msg
            End If

            Try
                interfaceFile = New MemoryStream(tempData.Data)
                xmlDoc.Load(interfaceFile)

                EncodeXmlDoc(xmlDoc.DocumentElement)

                msg = DomContractBL.ValidateServiceDeliveryfile(Me.DbConnection, xmlDoc, user.ExternalUserID)
                If msg.Success Or msg.Number = DomContractBL.ERR_SERVICE_DELIVERY_FILE_3B_MET Or msg.Number = DomContractBL.ERR_SERVICE_DELIVERY_FILE_3C_MET Then
                    'Still need extra confirmation.
                    'Determine the Query string for the redirection
                    If msg.Number = DomContractBL.ERR_SERVICE_DELIVERY_FILE_3B_MET Then
                        queryString = String.Format("?valcon={0}", DomContractBL.ERR_SERVICE_DELIVERY_FILE_3B_MET)
                    ElseIf msg.Number = DomContractBL.ERR_SERVICE_DELIVERY_FILE_3C_MET Then
                        queryString = String.Format("?valcon={0}", DomContractBL.ERR_SERVICE_DELIVERY_FILE_3C_MET)
                    End If

                    'move Tmp file to actual Abacus table and Delete Tmp Slick Upload file
                    trans = SqlHelper.GetTransaction(Me.DbConnection)
                    msg = FileStoreBL.MoveFromTempStoreToDataStore(trans, scannedDocIDs(0), newDataFileID)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        Return msg
                    End If
                    'Create row in DomProviderInterfaceFile
                    msg = DomContractBL.CreateDomProviderInterfaceFileRow( _
                        trans, _
                        xmlDoc, _
                        user.ExternalUserID, _
                        String.Format("{0} {1}", user.FirstName, user.Surname), _
                        newDataFileID, _
                        user.ID, _
                        interfaceFileID _
                    )
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        Return msg
                    End If

                    'Add an entry in DomProviderInterfaceFile_DomContract for each contract listed in the interface file
                    msg = DomContractBL.InsertDomProviderInterfaceFileDomContractRowFromFile(trans, newDataFileID, interfaceFileID)
                    If Not msg.Success Then
                        SqlHelper.RollbackTransaction(trans)
                        WebUtils.DisplayError(msg)
                    End If

                    'Commit Database transaction
                    trans.Commit()

                    ' store file ID in session variable
                    Session("DomProviderInterfaceFileID") = interfaceFileID

                    Response.Redirect(String.Format("{0}{1}", "UploadServiceDeliveryFileConfirmation.aspx", queryString))
                Else
                    'Delete Tmp Slick Upload file
                    WebFileStoreTempUpload.Delete(Me.DbConnection, scannedDocIDs(0))
                    'Display Error Message to the screen
                    lblError.Text = msg.Message
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                Return msg
            End Try
            msg = New ErrorMessage
            msg.Success = True
            Return msg
        End Function

        Private Sub EncodeXmlDoc(ByRef node As XmlNode)

            If node.HasChildNodes Then
                For Each n As XmlNode In node.ChildNodes
                    EncodeXmlDoc(n)
                Next
            Else
                node.InnerText = WebUtils.EncodeOutput(node.InnerText)
            End If

        End Sub

    End Class
End Namespace