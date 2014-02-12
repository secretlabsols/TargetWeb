
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.AddressContact
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Control that encapsulates the display of address/contact information.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      05/10/2007  Ported to use AjaxToolkit Tab control.
    '''     MikeVO      23/04/2007  Fix to DisableAddressTab() & DisableContactTab() (SPBG-313).
    '''     MikeVO      04/10/2006  Added DisableAddressTab() & DisableContactTab().
    '''     MikeVO      03/10/2006  Added SetAddressIDs() & SetContactIDs().
    ''' 	[Mikevo]	02/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------

    Partial Class AddressContact
        Inherits System.Web.UI.UserControl

#Region " Properties "

        ' properties that provide access to the controls on the user control

        Public Property Address() As TextBoxEx
            Get
                Return txtAddress
            End Get
            Set(ByVal Value As TextBoxEx)
                txtAddress = Value
            End Set
        End Property

        Public Property Postcode() As TextBoxEx
            Get
                Return txtPostcode
            End Get
            Set(ByVal Value As TextBoxEx)
                txtPostcode = Value
            End Set
        End Property

        Public Property AdminAuthority() As TextBoxEx
            Get
                Return txtAdminAuthority
            End Get
            Set(ByVal Value As TextBoxEx)
                txtAdminAuthority = Value
            End Set
        End Property

        Public Property District() As TextBoxEx
            Get
                Return txtDistrict
            End Get
            Set(ByVal Value As TextBoxEx)
                txtDistrict = Value
            End Set
        End Property

        Public Property Ward() As TextBoxEx
            Get
                Return txtWard
            End Get
            Set(ByVal Value As TextBoxEx)
                txtWard = Value
            End Set
        End Property

        Public Property Directions() As TextBoxEx
            Get
                Return txtDirections
            End Get
            Set(ByVal Value As TextBoxEx)
                txtDirections = Value
            End Set
        End Property

        Public Property UPRN() As TextBoxEx
            Get
                Return txtUPRN
            End Get
            Set(ByVal Value As TextBoxEx)
                txtUPRN = Value
            End Set
        End Property

        Public Property USRN() As TextBoxEx
            Get
                Return txtUSRN
            End Get
            Set(ByVal Value As TextBoxEx)
                txtUSRN = Value
            End Set
        End Property

        Public Property Confidential() As Label
            Get
                Return litConfidential
            End Get
            Set(ByVal Value As Label)
                litConfidential = Value
            End Set
        End Property

        Public Property DisabledAccess() As DropDownListEx
            Get
                Return cboDisabledAccess
            End Get
            Set(ByVal Value As DropDownListEx)
                cboDisabledAccess = Value
            End Set
        End Property

        Public Property AlsoUsedAs() As CheckedListBox
            Get
                Return chklstAlsoUsedAs
            End Get
            Set(ByVal Value As CheckedListBox)
                chklstAlsoUsedAs = Value
            End Set
        End Property

        Public Property ContactType() As DropDownListEx
            Get
                Return cboContactType
            End Get
            Set(ByVal Value As DropDownListEx)
                cboContactType = Value
            End Set
        End Property

        Public Property ContactOrganisation() As TextBoxEx
            Get
                Return txtContactOrganisation
            End Get
            Set(ByVal Value As TextBoxEx)
                txtContactOrganisation = Value
            End Set
        End Property

        Public Property ContactTitle() As DropDownListEx
            Get
                Return cboContactTitle
            End Get
            Set(ByVal Value As DropDownListEx)
                cboContactTitle = Value
            End Set
        End Property

        Public Property ContactForenames() As TextBoxEx
            Get
                Return txtContactForenames
            End Get
            Set(ByVal Value As TextBoxEx)
                txtContactForenames = Value
            End Set
        End Property

        Public Property ContactSurname() As TextBoxEx
            Get
                Return txtContactSurname
            End Get
            Set(ByVal Value As TextBoxEx)
                txtContactSurname = Value
            End Set
        End Property

        Public Property ContactPosition() As TextBoxEx
            Get
                Return txtContactPosition
            End Get
            Set(ByVal Value As TextBoxEx)
                txtContactPosition = Value
            End Set
        End Property

        Public Property ContactTel() As TextBoxEx
            Get
                Return txtContactTel
            End Get
            Set(ByVal Value As TextBoxEx)
                txtContactTel = Value
            End Set
        End Property

        Public Property ContactFax() As TextBoxEx
            Get
                Return txtContactFax
            End Get
            Set(ByVal Value As TextBoxEx)
                txtContactFax = Value
            End Set
        End Property

        Public Property ContactMobile() As TextBoxEx
            Get
                Return txtContactMobile
            End Get
            Set(ByVal Value As TextBoxEx)
                txtContactMobile = Value
            End Set
        End Property

        Public Property ContactPager() As TextBoxEx
            Get
                Return txtContactPager
            End Get
            Set(ByVal Value As TextBoxEx)
                txtContactPager = Value
            End Set
        End Property

        Public Property ContactEmail() As TextBoxEx
            Get
                Return txtContactEmail
            End Get
            Set(ByVal Value As TextBoxEx)
                txtContactEmail = Value
            End Set
        End Property

        Public Property ContactWeb() As TextBoxEx
            Get
                Return txtContactWeb
            End Get
            Set(ByVal Value As TextBoxEx)
                txtContactWeb = Value
            End Set
        End Property

        Public Property ContactAlsoUsedAs() As CheckedListBox
            Get
                Return chklstContactAlsoUsedAs
            End Get
            Set(ByVal Value As CheckedListBox)
                chklstContactAlsoUsedAs = Value
            End Set
        End Property

#End Region

#Region " InitControl "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Initialises the user control.
        ''' </summary>
        ''' <param name="conn">An already open database connection.</param>
        ''' <param name="tabStripID">The ID to give the tab strip control.</param>
        ''' <param name="displayAlsoUsedAs">Whether the Also Used As fileds should be displayed.</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	03/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Sub InitControl(ByVal conn As SqlConnection, ByVal tabStripID As String, ByVal displayAlsoUsedAs As Boolean)

            Const SP_NAME_FETCH_TITLES As String = "pr_FetchTitles"

            Dim thePage As BasePage = DirectCast(Me.Page, BasePage)
            Dim msg As ErrorMessage
            Dim reader As SqlDataReader = Nothing

            thePage.AddExtraCssStyle(".addressContactContent { width:70%; float:left; } .addressContactLabel { width:15em; font-weight:bold; float:left; }")

            ' address
            With txtAddress.TextBox
                .TextMode = TextBoxMode.MultiLine
                .Rows = 4
                .Columns = 1
            End With
            With txtDirections.TextBox
                .TextMode = TextBoxMode.MultiLine
                .Rows = 4
                .Columns = 1
            End With
            If Not Me.Page.IsPostBack Then
                With cboDisabledAccess.DropDownList.Items
                    .Add(New ListItem(" ", " "))
                    .Add(New ListItem("Yes", "Y"))
                    .Add(New ListItem("No", "N"))
                End With
            End If
            If Not displayAlsoUsedAs Then
                lblAddressAlsoUsedAs.Visible = False
                chklstAlsoUsedAs.Visible = False
            End If

            ' contact
            If Not Me.Page.IsPostBack Then
                With cboContactType.DropDownList.Items
                    .Add(New ListItem("Person", "P"))
                    .Add(New ListItem("Role", "R"))
                    .Add(New ListItem("Organisation", "O"))
                End With

                ' grab the list of titles
                Try
                    reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, SP_NAME_FETCH_TITLES)

                    With cboContactTitle.DropDownList
                        .DataSource = reader
                        .DataTextField = "Description"
                        .DataValueField = "Description"
                        .DataBind()
                        .Items.Insert(0, New ListItem("", ""))
                    End With

                Catch ex As Exception
                    msg = Utils.CatchError(ex, "E0501", SP_NAME_FETCH_TITLES, "AddressContact.InitControl()")   ' could not read data
                    Target.Library.Web.Utils.DisplayError(msg)
                Finally
                    If Not reader Is Nothing Then reader.Close()
                End Try
            End If
            If Not displayAlsoUsedAs Then
                lblContactAlsoUsedAs.Visible = False
                chklstContactAlsoUsedAs.Visible = False
            End If

        End Sub

#End Region

#Region " SetAddressIDs "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sets the record IDs on the relevant address controls.
        ''' </summary>
        ''' <param name="addressRecordID">The ID of the address record.</param>
        ''' <param name="parentRecordID">The ID of the parent record that uses the address/contact</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	03/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Sub SetAddressIDs(ByVal addressRecordID As Integer, ByVal parentRecordID As Integer)

            txtAddress.RecordID = addressRecordID
            txtAddress.ParentRecordID = parentRecordID
            txtPostcode.RecordID = addressRecordID
            txtPostcode.ParentRecordID = parentRecordID
            txtAdminAuthority.RecordID = addressRecordID
            txtAdminAuthority.ParentRecordID = parentRecordID
            txtDistrict.RecordID = addressRecordID
            txtDistrict.ParentRecordID = parentRecordID
            txtWard.RecordID = addressRecordID
            txtWard.ParentRecordID = parentRecordID
            txtDirections.RecordID = addressRecordID
            txtDirections.ParentRecordID = parentRecordID
            txtUPRN.RecordID = addressRecordID
            txtUPRN.ParentRecordID = parentRecordID
            txtUSRN.RecordID = addressRecordID
            txtUSRN.ParentRecordID = parentRecordID
            cboDisabledAccess.RecordID = addressRecordID
            cboDisabledAccess.ParentRecordID = parentRecordID
            chklstAlsoUsedAs.RecordID = addressRecordID
            chklstAlsoUsedAs.ParentRecordID = parentRecordID

        End Sub

#End Region

#Region " SetContactIDs "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sets the record IDs on the relevant contact controls.
        ''' </summary>
        ''' <param name="contactRecordID">The ID of the contact record.</param>
        ''' <param name="parentRecordID">The ID of the parent record that uses the address/contact</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        ''' 	[Mikevo]	03/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Sub SetContactIDs(ByVal contactRecordID As Integer, ByVal parentRecordID As Integer)

            cboContactType.RecordID = contactRecordID
            cboContactType.ParentRecordID = parentRecordID
            txtContactOrganisation.RecordID = contactRecordID
            txtContactOrganisation.ParentRecordID = parentRecordID
            cboContactTitle.RecordID = contactRecordID
            cboContactTitle.ParentRecordID = parentRecordID
            txtContactForenames.RecordID = contactRecordID
            txtContactForenames.ParentRecordID = parentRecordID
            txtContactSurname.RecordID = contactRecordID
            txtContactSurname.ParentRecordID = parentRecordID
            txtContactPosition.RecordID = contactRecordID
            txtContactPosition.ParentRecordID = parentRecordID
            txtContactTel.RecordID = contactRecordID
            txtContactTel.ParentRecordID = parentRecordID
            txtContactFax.RecordID = contactRecordID
            txtContactFax.ParentRecordID = parentRecordID
            txtContactMobile.RecordID = contactRecordID
            txtContactMobile.ParentRecordID = parentRecordID
            txtContactPager.RecordID = contactRecordID
            txtContactPager.ParentRecordID = parentRecordID
            txtContactEmail.RecordID = contactRecordID
            txtContactEmail.ParentRecordID = parentRecordID
            txtContactWeb.RecordID = contactRecordID
            txtContactWeb.ParentRecordID = parentRecordID
            chklstContactAlsoUsedAs.RecordID = contactRecordID
            chklstContactAlsoUsedAs.ParentRecordID = parentRecordID

        End Sub

#End Region

#Region " DisableAddressTab "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Disables all of the controls on the Address tab.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        '''     MikeVO      23/04/2007  Changed to use RecursiveDisable().
        ''' 	[Mikevo]	04/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Sub DisableAddressTab()
            addressPanel.Enabled = False
        End Sub

#End Region

#Region " DisableContactTab "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Disables all of the controls on the Contact tab.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history><![CDATA[
        '''     MikeVO      23/04/2007  Changed to use RecursiveDisable().
        ''' 	[Mikevo]	04/10/2006	Created
        ''' ]]></history>
        ''' -----------------------------------------------------------------------------
        Public Sub DisableContactTab()
            contactPanel.Enabled = False
        End Sub

#End Region

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)

            lblDisabledAccess.AssociatedControlID = cboDisabledAccess.ID
            lblContactType.AssociatedControlID = cboContactType.ID
            lblContactTitle.AssociatedControlID = cboContactTitle.ID

        End Sub

    End Class

End Namespace

