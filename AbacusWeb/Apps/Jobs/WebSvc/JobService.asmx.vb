
Imports System.Reflection.Assembly
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web.Services
Imports System.Xml
Imports System.Xml.Xsl
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Jobs.Core
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.Core

Namespace Apps.Jobs.WebSvc

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.JobService
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Web Service to allow interaction with the Job Service.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' MoTahir     09/02/2011  D11934 - Password Maintenance
    ''' MikeVO      11/01/2010  D11435 - user-job security
    ''' MvO         19/03/2008  Added CopyJob() method.
    ''' Phil Walker 04/10/2006  D10883 - Add new Job Type for SWIFT.
    ''' MikeVO      29/08/2006  D10921 - support for config settings in database.
    ''' MikeVO      17/02/2006  Added CreateNewJob().
    ''' [Mikevo]	24/01/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Web.Services.WebService(Namespace:="http://targetsys.co.uk/Abacus/JobService")> _
    Public Class JobService
        Inherits System.Web.Services.WebService

#Region " Web Services Designer Generated Code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Web Services Designer.
            InitializeComponent()

            'Add your own initialization code after the InitializeComponent() call

        End Sub

        'Required by the Web Services Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Web Services Designer
        'It can be modified using the Web Services Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            components = New System.ComponentModel.Container
        End Sub

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            'CODEGEN: This procedure is required by the Web Services Designer
            'Do not modify it using the code editor.
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region

#Region " FetchJobList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Returns a page of jobs, filtered by status and user.
        ''' </summary>
        ''' <param name="createdBy">The user whose jobs to return.</param>
        ''' <param name="jobStatusID">The status of jobs to return.</param>
        ''' <param name="jobTypeID">The type of the jobs to return.</param>
        ''' <param name="page">The page of jobs to return.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' MvO  11/01/2010  D11435 - Added webSecurityUserID parameter
        ''' MvO  13/02/2008  Added jobTypeID filter
        ''' MvO  24/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchJobList(ByVal createdBy As String, ByVal jobStatusID As Integer, _
            ByVal jobTypeID As Integer, ByVal page As Integer) As FetchJobListResult

            Dim jobList As vwJobListCollection = Nothing
            Dim msg As ErrorMessage
            Dim result As FetchJobListResult = New FetchJobListResult
            Dim conn As SqlConnection = Nothing
            Dim pageSize As Integer = 10
            Dim totalRecords As Integer
            Dim currentUser As WebSecurityUser

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                currentUser = SecurityBL.GetCurrentUser()

                ' get the list of jobs
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                msg = JobServiceBL.FetchJobList(conn, _
                                                createdBy, _
                                                jobStatusID, _
                                                jobTypeID, _
                                                page, _
                                                pageSize, _
                                                currentUser.ID, _
                                                totalRecords, _
                                                jobList _
                )
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Jobs = jobList
                    .PagingLinks = Target.Library.Web.Utils.BuildPagingLinks( _
                        "<a href=""javascript:FetchJobList(jobListCreatedBy, jobListStatusID, jobTypeID, {0})"" title=""{2}"">{1}</a>&nbsp;", _
                        page, Math.Ceiling(totalRecords / pageSize))
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " FetchJobStepList "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Returns a list of job steps for the specified job.
        ''' </summary>
        ''' <param name="jobID">The job to return the steps for.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	26/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchJobStepList(ByVal jobID As Integer) As FetchJobStepListResult

            Dim steps As vwJobStepListCollection = Nothing
            Dim msg As ErrorMessage
            Dim result As FetchJobStepListResult = New FetchJobStepListResult
            Dim conn As SqlConnection = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get the list of job steps
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                msg = vwJobStepList.FetchList(conn, steps, 0, jobID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Steps = steps
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " CancelJob "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Attempts to cancel a job.
        ''' </summary>
        ''' <param name="jobID">The ID of the job to cancel.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        '''     MikeVO      16/07/2009  D11627 - additional security.
        '''     MikeVO      01/12/2008  D11444 - security overhaul.
        ''' 	[Mikevo]	26/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CancelJob(ByVal jobID As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim canCancelJobs As Boolean
            Dim currentUser As WebSecurityUser
            Dim settings As SystemSettings

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then Return msg

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' check security
                currentUser = SecurityBL.GetCurrentUser()
                settings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                canCancelJobs = _
                    SecurityBL.UserHasMenuItemCommand(conn, _
                                           currentUser.ID, _
                                           Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ViewJobs.CancelJob"), _
                                           settings.CurrentApplicationID)
                If Not canCancelJobs Then
                    msg = New ErrorMessage()
                    msg.Number = "E3020"    ' insufficient rights
                    Return msg
                End If

                ' attempt to cancel the job
                msg = JobServiceBL.CancelJob(conn, jobID)

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg

        End Function

#End Region

#Region " DeleteJob "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Attempts to delete a job.
        ''' </summary>
        ''' <param name="jobID">The ID of the job to delete.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        '''     MikeVO      16/07/2009  D11627 - additional security.
        '''     MikeVO      01/12/2008  D11444 - security overhaul.
        ''' 	[Mikevo]	26/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function DeleteJob(ByVal jobID As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim canDeleteJobs As Boolean
            Dim currentUser As WebSecurityUser
            Dim settings As SystemSettings

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then Return msg

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' check security
                currentUser = SecurityBL.GetCurrentUser()
                settings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                canDeleteJobs = _
                    SecurityBL.UserHasMenuItemCommand(conn, _
                                           currentUser.ID, _
                                           Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ViewJobs.DeleteJob"), _
                                           settings.CurrentApplicationID)
                If Not canDeleteJobs Then
                    msg = New ErrorMessage()
                    msg.Number = "E3020"    ' insufficient rights
                    Return msg
                End If

                ' attempt to delete the job
                msg = JobServiceBL.DeleteJob(conn, jobID)

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return msg

        End Function

#End Region

#Region " FetchJobStepXml "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Returns the formatted job step Xml information for display.
        ''' </summary>
        ''' <param name="jobStepID">The step to get info for.</param>
        ''' <param name="whichXml">Which Xml we are getting.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     MikeVO      06/04/2010  A4WA#6158 - defaulted xml content such that transform is always performed.
        '''     MikeVO      19/06/2008  Added XsltExtensionObject to transformation.
        ''' 	[Mikevo]	30/01/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchJobStepXml(ByVal jobStepID As Integer, ByVal whichXml As Integer) As StringResult

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim result As StringResult = New StringResult
            Dim theStep As JobStep
            Dim jobStepTypeClassName As String
            Dim steps As vwJobStepListCollection = Nothing
            Dim xmlData As String = String.Empty, transformedData As String = String.Empty, xsltPath As String = Nothing

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' get step
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)
                theStep = New JobStep(conn)
                With theStep
                    msg = .Fetch(jobStepID)
                    If Not msg.Success Then
                        result.ErrMsg = msg
                        Return result
                    End If
                End With

                ' get the step via the view to get its JobStepType.ClassName
                msg = vwJobStepList.FetchList(conn, steps, jobStepID, theStep.JobID)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If
                jobStepTypeClassName = steps(0).JobStepTypeClass

                ' decide which XML and XSLT to use
                Select Case whichXml
                    Case JobStepXml.Input
                        xmlData = theStep.InputXML
                        xsltPath = String.Format("AbacusWeb/Apps/Jobs/WebSvc/Xslt/{0}.Input.xslt", jobStepTypeClassName)

                    Case JobStepXml.Progress
                        xmlData = theStep.ProgressXML
                        xsltPath = "AbacusWeb/Apps/Jobs/WebSvc/Xslt/JobStepProgress.xslt"

                    Case JobStepXml.Output
                        xmlData = theStep.OutputXML
                        xsltPath = String.Format("AbacusWeb/Apps/Jobs/WebSvc/Xslt/{0}.Output.xslt", jobStepTypeClassName)

                End Select

                ' format the data for display
                If xmlData Is Nothing Then xmlData = "<root />"
                xsltPath = Server.MapPath(Target.Library.Web.Utils.GetVirtualPath(xsltPath))
                If File.Exists(xsltPath) Then
                    ' transform using the XSLT
                    Dim args As XsltArgumentList = New XsltArgumentList
                    ' add the extension utility object
                    Dim obj As XsltExtensionUtil = New XsltExtensionUtil
                    args.AddExtensionObject("urn:ext-util", obj)
                    transformedData = Utils.TransformXML(xmlData, xsltPath, args)
                End If

                With result
                    .ErrMsg = New ErrorMessage
                    .ErrMsg.Success = True
                    .Data = transformedData
                End With

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function


#End Region

#Region " FetchAbout "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Returns information about the browser and software.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	25/05/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function FetchAbout() As AboutResult

            Dim msg As ErrorMessage
            Dim result As AboutResult = New AboutResult
            Dim appInfo As ApplicationInfo = New ApplicationInfo
            Dim section As ApplicationInfoSection
            Dim request As HttpRequest = HttpContext.Current.Request
            Dim conn As SqlConnection = Nothing
            Dim sysInfos As SystemInfoCollection = Nothing
            Dim missingAssemblies As ArrayList = New ArrayList
            Dim loadedAssemblies As SortedList = New SortedList

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' browser info
                section = appInfo.AddSection("Browser")
                If request.Browser.Browser = "Gecko" Then section.AddDetail("NOTE", _
                    "Gecko based browsers (e.g. Mozilla, Firefox, Netscape 6+) will display ""Gecko"" in the ""Browser"" " & _
                    "property and static version information of ""5.0"". The actual browser and version is displayed in " & _
                    "the ""Type"" property.")
                section.AddDetail("ActiveXControls", request.Browser.ActiveXControls())
                section.AddDetail("AOL", request.Browser.AOL())
                section.AddDetail("BackgroundSounds", request.Browser.BackgroundSounds())
                section.AddDetail("Beta", request.Browser.Beta())
                section.AddDetail("Browser", request.Browser.Browser())
                section.AddDetail("CDF", request.Browser.CDF())
                section.AddDetail("ClrVersion", request.Browser.ClrVersion().ToString())
                section.AddDetail("Cookies", request.Browser.Cookies())
                section.AddDetail("Crawler", request.Browser.Crawler())
                section.AddDetail("EcmaScriptVersion", request.Browser.EcmaScriptVersion().ToString())
                section.AddDetail("Frames", request.Browser.Frames())
                section.AddDetail("JavaApplets", request.Browser.JavaApplets())
                section.AddDetail("JavaScript", request.Browser.EcmaScriptVersion.ToString())
                section.AddDetail("MajorVersion", request.Browser.MajorVersion())
                section.AddDetail("MinorVersion", request.Browser.MinorVersion())
                section.AddDetail("MSDomVersion", request.Browser.MSDomVersion().ToString())
                section.AddDetail("Platform", request.Browser.Platform())
                section.AddDetail("Tables", request.Browser.Tables())
                section.AddDetail("TagWriter", request.Browser.TagWriter().ToString())
                section.AddDetail("Type", request.Browser.Type())
                section.AddDetail("VBScript", request.Browser.VBScript())
                section.AddDetail("Version", request.Browser.Version())
                section.AddDetail("W3CDomVersion", request.Browser.W3CDomVersion().ToString())
                section.AddDetail("Win16", request.Browser.Win16())
                section.AddDetail("Win32", request.Browser.Win32())

                ' open db connection
                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' get system info
                msg = SystemInfo.FetchList(conn, sysInfos)
                If Not msg.Success Then
                    result.ErrMsg = msg
                    Return result
                End If

                ' Abacus info
                section = appInfo.AddSection("Abacus for Windows Database")
                section.AddDetail("Database Server", conn.DataSource)
                section.AddDetail("Database Name", conn.Database)
                section.AddDetail("Licence No", sysInfos(0).LicenceNo)
                section.AddDetail("Version", sysInfos(0).CurrentBuild)

                ' close db connection
                conn.Close()

                ' web server environment
                section = appInfo.AddSection("Abacus Web Server")
                section.AddDetail("Server Name", Environment.MachineName)
                section.AddDetail("OS", Environment.OSVersion.ToString())
                section.AddDetail(".NET Version", Environment.Version.ToString())

                ' assemblies
                Utils.LoadAssemblies(GetExecutingAssembly().GetName(), missingAssemblies, loadedAssemblies)
                If missingAssemblies.Count > 0 Then
                    section = appInfo.AddSection("Missing Assemblies")
                    For Each assem As String In missingAssemblies
                        section.AddDetail(assem)
                    Next
                End If
                If loadedAssemblies.Count > 0 Then
                    section = appInfo.AddSection("Loaded Assemblies")
                    For Each assem As String In loadedAssemblies.Values
                        section.AddDetail(assem)
                    Next
                End If


                ' return result
                result.AppInfo = appInfo

                msg = New ErrorMessage
                msg.Success = True
                result.ErrMsg = msg

            Catch ex As Exception
                result.ErrMsg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)    ' unexpected
            Finally
                If Not conn Is Nothing Then conn.Close()
            End Try

            Return result

        End Function

#End Region

#Region " CopyJob "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Copies and existing job and submits it ready for processing.
        ''' </summary>
        ''' <param name="jobID">The ID of the job to copy.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' MikeVO  16/07/2009  D11627 - additional security.
        ''' MikeVO  15/06/2009  D11515 - added support for email notifications.
        ''' MikeVO  01/12/2008  D11444 - security overhaul.
        ''' MvO  19/03/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <WebMethod(EnableSession:=True), AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.Read)> _
        Public Function CopyJob(ByVal jobID As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim conn As SqlConnection = Nothing
            Dim currentUser As WebSecurityUser
            Dim canCopyJobs As Boolean
            Dim settings As SystemSettings
            Dim a4wUser As Target.Abacus.Library.DataClasses.Users

            Try
                ' check user is logged in
                msg = SecurityBL.ValidateLogin
                If Not msg.Success Then Return msg

                conn = SqlHelper.GetConnection(ConnectionStrings("Abacus").ConnectionString)

                ' check security
                currentUser = SecurityBL.GetCurrentUser()
                settings = SystemSettings.GetCachedSettings(ConnectionStrings("Abacus").ConnectionString, SystemSettings.CACHE_DEFAULT_EXPIRATION)
                canCopyJobs = _
                    SecurityBL.UserHasMenuItemCommand(conn, _
                                           currentUser.ID, _
                                           Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ViewJobs.CopyJob"), _
                                           settings.CurrentApplicationID)
                If Not canCopyJobs Then
                    msg = New ErrorMessage()
                    msg.Number = "E3020"    ' insufficient rights
                    Return msg
                End If

                ' copy the job
                a4wUser = New Target.Abacus.Library.DataClasses.Users(conn)
                msg = a4wUser.Fetch(currentUser.ExternalUserID)
                If Not msg.Success Then Return msg

                msg = JobServiceBL.CopyJob(conn, _
                                           jobID, _
                                           currentUser.ExternalUsername, _
                                           currentUser.ExternalUserID, _
                                           a4wUser.EMail)
                If Not msg.Success Then Return msg

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)   ' unexpected
            Finally
                SqlHelper.CloseConnection(conn)
            End Try

            Return msg

        End Function

#End Region

    End Class

End Namespace