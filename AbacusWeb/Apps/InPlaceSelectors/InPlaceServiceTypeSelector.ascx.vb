Imports System.Text
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Class representing a selector tool for DomServiceType records
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''    Waqas    16/05/2013 Updated #7848 VisitBased property removed
    '''   ColinD   23/11/2010 D11964A - Created
    ''' </history>
    Partial Public Class InPlaceServiceTypeSelector
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' fields
        Private _Enabled As Boolean = True
        Private _ItemText As String
        Private _RequiredValidatorErrorMsg As String

        ' constants
        Private Const _InPlaceSelectorJavascriptLibraryKey As String = "InPlaceServiceTypeSelectorJavascriptLibrary"
        Private Const _InPlaceSelectorJavaScriptPath As String = "AbacusWeb/Apps/InPlaceSelectors/InPlaceServiceTypeSelector.js"
        Private Const _InPlaceSelectorName As String = "InPlaceServiceTypeSelector"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the base page.
        ''' </summary>
        ''' <value>The base page.</value>
        Private ReadOnly Property BasePage() As BasePage
            Get
                Return CType(Me.Page, BasePage)
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="InPlaceServiceTypeSelector" /> is enabled.
        ''' </summary>
        ''' <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        Public Property Enabled() As Boolean
            Get
                Return _Enabled
            End Get
            Set(ByVal value As Boolean)
                _Enabled = value
                WebUtils.RecursiveDisable(Controls, Not value)
            End Set
        End Property

        ''' <summary>
        ''' Gets the hidden field unique ID.
        ''' </summary>
        ''' <value>The hidden field unique ID.</value>
        Public ReadOnly Property HiddenFieldUniqueID() As String
            Get
                Return hidID.UniqueID
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the item ID for this in place selector.
        ''' </summary>
        ''' <value>The item ID.</value>
        Public Property ItemID() As Integer
            Get
                Return Target.Library.Utils.ToInt32(hidID.Value)
            End Get
            Set(ByVal value As Integer)
                hidID.Value = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the item text for this in place selector.
        ''' </summary>
        ''' <value>The item text.</value>
        Public Property ItemText() As String
            Get
                Return txtName.Value
            End Get
            Set(ByVal value As String)
                txtName.Value = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether this <see cref="InPlaceServiceTypeSelector" /> is required.
        ''' </summary>
        ''' <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        Public Property Required() As Boolean
            Get
                Return valRequired.Visible
            End Get
            Set(ByVal value As Boolean)
                valRequired.Visible = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the required validator error message.
        ''' </summary>
        ''' <value>The required validator error message.</value>
        Public Property RequiredValidatorErrorMessage() As String
            Get
                Return _RequiredValidatorErrorMsg
            End Get
            Set(ByVal value As String)
                _RequiredValidatorErrorMsg = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the required validator validation group i.e. Save
        ''' </summary>
        ''' <value>The required validator validation group.</value>
        Public Property RequiredValidatorValidationGroup() As String
            Get
                Return valRequired.ValidationGroup
            End Get
            Set(ByVal value As String)
                valRequired.ValidationGroup = value
            End Set
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Loads the item.
        ''' </summary>
        Private Sub LoadItem()

            Dim item As ViewableServiceType = New ViewableServiceType()
            Dim js As New StringBuilder()
            Dim jsServiceObjectVar As String = String.Format("{0}_{1}", _InPlaceSelectorName, Me.ClientID) ' a unique object name in client side to hold the item
            Dim msg As New ErrorMessage()

            If ItemID > 0 Then
                ' if we have an id to work with

                ' fetch the item and throw an error if not successful
                msg = AbacusClassesBL.FetchServiceTypeWithDetails(BasePage.DbConnection, ItemID, item)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' set the controls on the selector to that of the fetched item
                txtName.Value = item.ServiceTypeDescription
                hidID.Value = item.ID

            Else
                ' else just set the controls to blank

                txtName.Value = String.Empty
                hidID.Value = String.Empty

            End If

            ' set the controls on the selector to that of the fetched item
            js.AppendFormat("{0} = new {1}_ServiceType();", jsServiceObjectVar, _InPlaceSelectorName)
            js.AppendFormat("{0}.ID = {1};", jsServiceObjectVar, item.ID)
            js.AppendFormat("{0}.ServiceTypeDescription = '{1}';", jsServiceObjectVar, item.ServiceTypeDescription)
            js.AppendFormat("{0}.ServiceCategoryDescription = '{1}';", jsServiceObjectVar, item.ServiceCategoryDescription)
            js.AppendFormat("{0}.ServiceCategoryID = {1};", jsServiceObjectVar, item.ServiceCategoryID)
            js.AppendFormat("{0}.ServiceClassificationGroupDescription = '{1}';", jsServiceObjectVar, item.ServiceClassificationGroupDescription)
            js.AppendFormat("{0}.ServiceClassificationGroupID = {1};", jsServiceObjectVar, item.ServiceClassificationGroupID)
            js.AppendFormat("{0}.ServiceGroupDescription = '{1}';", jsServiceObjectVar, item.ServiceGroupDescription)
            js.AppendFormat("{0}.ServiceGroupID = {1};", jsServiceObjectVar, item.ServiceGroupID)
            js.AppendFormat("{0}.Permanent = {1};", jsServiceObjectVar, item.Permanent.ToString().ToLower())
            js.AppendFormat("{0}.Redundant = {1};", jsServiceObjectVar, item.Redundant.ToString().ToLower())
            js.AppendFormat("{2}_Selections['{0}'] = {1};", Me.ClientID, jsServiceObjectVar, _InPlaceSelectorName)

            ' register the script above in client, do so once using a unique key based on the selector name and client id 
            BasePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                           String.Format("Target.Abacus.Web.Apps.UserControls.{0}.Startup.{1}", _InPlaceSelectorName, Me.ClientID), _
                                                           Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            btnFind.Attributes.Add("onclick", String.Format("{0}_FindClicked('{1}');", _InPlaceSelectorName, Me.ClientID))
            txtName.Attributes.Add("onchange", String.Format("{0}_ClearStoredID('{1}');", _InPlaceSelectorName, Me.ClientID))

            If Not Page.ClientScript.IsClientScriptIncludeRegistered(Me.GetType(), _InPlaceSelectorJavascriptLibraryKey) Then
                ' check that the js for this control isnt already registered, if not then register script 

                Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), _InPlaceSelectorJavascriptLibraryKey, WebUtils.GetVirtualPath(_InPlaceSelectorJavaScriptPath))

            End If

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If Required Then
                ' if this is a required selector setup validator

                With valRequired
                    .ControlToValidate = txtName.ID
                    .Display = ValidatorDisplay.Dynamic
                    .ErrorMessage = RequiredValidatorErrorMessage
                End With

            End If

            ' load the item i.e. setup controls with values from db
            LoadItem()

        End Sub

#End Region

    End Class

End Namespace
