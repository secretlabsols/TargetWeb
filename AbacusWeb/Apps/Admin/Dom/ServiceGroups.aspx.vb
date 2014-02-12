
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Controls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Text

Namespace Apps.Admin.Dom
    ''' <summary>
    ''' Admin page used to maintain domiciliary service groups.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir  01/11/2012  D12343 - Remove Framework Type from Service Group.
    ''' ColinD   09/08/2011  D11965 - Updated to order rate frameowrks by sort order.
    ''' ColinD   22/10/2010  D11924A - Changes to account for addition of Permanent field on ServiceGroup table
    ''' ColinD   11/10/2010  D11918 - numerous changes mainly for outputting client side script
    ''' Mo Tahir 22/07/2010  D11877 - Res Care to Service group mapping
    ''' Mo Tahir 26/04/2010  A4WA#6256
    ''' Mo Tahir 15/03/2010  A4WA#6065
    ''' MikeVO   15/12/2009  A4WA#5967 - disable framework type when group is in use.
    ''' Mo Tahir 06/08/2009  D11671.
    ''' </history>
    Partial Public Class ServiceGroups
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _inUse As Boolean
        Private _javascriptServiceGroupClassifications As String = String.Empty
        Private _inEditMode As Boolean = False

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim js As StringBuilder = New StringBuilder()

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceGroups"), "Service Groups")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceGroups.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceGroups.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceGroups.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                    .Add("Abbreviation", "Abbreviation")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.ServiceGroup
                .AuditLogTableNames.Add("ServiceGroup")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ServiceGroups")
            End With

            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))
            Me.JsLinks.Add("ServiceGroups.js")

        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            _inEditMode = True
            FindClicked(e)
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim sgr As ServiceGroup
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            sgr = New ServiceGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With sgr
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                PopulateServiceGroupClassification(.ServiceGroupClassificationID)
                rblServiceCategory.SelectedValue = .ServiceCategory
                rblTemporaryOrPermanent.SelectedValue = IIf(.Permanent = TriState.True, "1", "0")
                chkRedundant.CheckBox.Checked = .Redundant
            End With

            msg = DomContractBL.ServiceGroupInUse(Me.DbConnection, e.ItemID, _inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim sgr As ServiceGroup
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim isNewSG As Boolean

            If e.ItemID > 0 Then
                msg = DomContractBL.ServiceGroupInUse(Me.DbConnection, e.ItemID, _inUse)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            If Me.IsPostBack Then
                PopulateServiceGroupClassification()
                cboServiceGroupClassification.SelectPostBackValue()
                cboServiceGroupClassification.RequiredValidator.Enabled = False
                reqServiceCategory.Enabled = False
            End If

            If Me.IsValid Then
                sgr = New ServiceGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With sgr
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                End If

                With sgr
                    .Description = txtDescription.Text
                    .Redundant = chkRedundant.CheckBox.Checked
                    If _inUse = False Then
                        .ServiceGroupClassificationID = cboServiceGroupClassification.DropDownList.SelectedValue
                        .ServiceCategory = Byte.Parse(hidServiceCategory.Value)
                        .Permanent = IIf(rblTemporaryOrPermanent.SelectedValue = "1", TriState.True, TriState.False)
                    End If
                    ' save
                    isNewSG = .IsNew
                    msg = .Save()
                    If Not msg.Success Then
                        WebUtils.DisplayError(msg)
                    End If

                    If isNewSG Then
                        lblError.Text = "The new Service Group must be associated with one or more Security " & _
                                        "Roles before it may be used elsewhere in the system"
                        lblError.ForeColor = Color.Orange
                    End If

                    e.ItemID = .ID
                End With
            Else
                e.Cancel = True
            End If

            FindClicked(e)
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                cboServiceGroupClassification.Text = String.Empty
                chkRedundant.CheckBox.Checked = False
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' check in use
            msg = DomContractBL.ServiceGroupInUse(Me.DbConnection, e.ItemID, _inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If _inUse Then
                lblError.Text = "The Service Group is in use and cannot be deleted."
                e.Cancel = True
                FindClicked(e)
                Exit Sub
            End If

            msg = ServiceGroup.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If

        End Sub

        Private Sub PopulateServiceGroupClassification(Optional ByVal serviceGroupClassificationId As Integer = 0)

            Dim msg As ErrorMessage
            Dim tbServiceGroupClassification As ServiceGroupClassificationCollection = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim javascriptServiceGroupClassifications As New StringBuilder()

            msg = ServiceGroupClassification.FetchList(Me.DbConnection, tbServiceGroupClassification, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), TriState.False)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With cboServiceGroupClassification.DropDownList
                .Items.Clear()
                .DataSource = tbServiceGroupClassification
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, New ListItem(String.Empty))
            End With

            If Not tbServiceGroupClassification Is Nothing AndAlso tbServiceGroupClassification.Count > 0 Then
                ' if we have some service group classification records

                Dim sgcIndex As Integer = 0

                For Each sgc As ServiceGroupClassification In tbServiceGroupClassification
                    ' loop each sgc and create a js object from it

                    javascriptServiceGroupClassifications.AppendFormat("serviceGroupClassificationCollection[{0}] = new ServiceGroupClassification({1}, {2});", sgcIndex, sgc.ID, sgc.CareType)
                    sgcIndex += 1

                Next

                If javascriptServiceGroupClassifications.Length > 0 Then
                    ' if we have some objects
                    _JavascriptServiceGroupClassifications = javascriptServiceGroupClassifications.ToString()

                End If

            End If

            If serviceGroupClassificationId <> 0 Then
                Dim itemFound As Boolean = False
                For Each tBand As ServiceGroupClassification In tbServiceGroupClassification
                    If tBand.ID = serviceGroupClassificationId Then
                        cboServiceGroupClassification.DropDownList.SelectedValue = serviceGroupClassificationId
                        itemFound = True
                        Exit For
                    End If
                Next

                If Not itemFound Then
                    Dim tBand As New ServiceGroupClassification(Me.DbConnection, String.Empty, String.Empty)
                    msg = tBand.Fetch(serviceGroupClassificationId)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    cboServiceGroupClassification.DropDownList.Items.Insert(cboServiceGroupClassification.DropDownList.Items.Count, New ListItem(tBand.Description, tBand.ID))
                    cboServiceGroupClassification.DropDownList.SelectedValue = serviceGroupClassificationId
                End If

            End If
        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            _inEditMode = True
            PopulateServiceGroupClassification()
        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim jsStartupScript As New StringBuilder()

            cboServiceGroupClassification.DropDownList.Attributes.Add("onchange", "cboServiceGroupClassification_Changed();")
            jsStartupScript.AppendFormat("cboServiceGroupClassificationID='{0}_cboDropDownList';", cboServiceGroupClassification.ClientID)
            jsStartupScript.AppendFormat("rblServiceCategoryID='{0}';", rblServiceCategory.ClientID)
            jsStartupScript.AppendFormat("lblServiceCategoryID='{0}';", lblServiceCategory.ClientID)
            jsStartupScript.AppendFormat("inEditMode = {0};", _inEditMode.ToString().ToLower())
            jsStartupScript.AppendFormat("hidServiceCategoryID='{0}';", hidServiceCategory.ClientID)
            jsStartupScript.AppendFormat("rblTemporaryOrPermanentID='{0}';", rblTemporaryOrPermanent.ClientID)
            jsStartupScript.AppendFormat("isServiceGroupInUse={0};", _inUse.ToString().ToLower())

            If Not String.IsNullOrEmpty(_JavascriptServiceGroupClassifications) Then
                ' if we have some classifications then create collection in js

                jsStartupScript.Append(_JavascriptServiceGroupClassifications)

            End If

            If Not ClientScript.IsStartupScriptRegistered(Me.GetType(), "StartUp") Then

                ClientScript.RegisterStartupScript(Me.GetType(), "StartUp", _
                                                    jsStartupScript.ToString(), _
                                                    True)

            End If

            cboServiceGroupClassification.RequiredValidator.Enabled = True
            reqServiceCategory.Enabled = True

        End Sub

    End Class

End Namespace

