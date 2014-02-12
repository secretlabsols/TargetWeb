
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.SP.Web
    ''' Class	 : SP.Web.Apps.UserControls.NewSUNotifEnterDetails
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the New SU Notification Enter Details step controls.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      07/11/2008  Fix after .NET 2.0 port.
    '''     MikeVO      05/03/2007  Added Provider Ref, Service Level & Unit Cost.
    '''                             Made DoB and NINo optional.
    '''                             Made properties writeable.
    ''' 	[MikeVO]	16/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class NewSUNotifEnterDetails
        Inherits System.Web.UI.UserControl

#Region " Properties "

        Public Property ExpectedStartDate() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtExpectedStartDate.UniqueID))
                Else
                    Return txtExpectedStartDate.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtExpectedStartDate.Text = Value
            End Set
        End Property

        Public Property TenancySupportAgreement() As Boolean
            Get
                If Me.IsPostBack Then
                    Return IIf(Request.Form(String.Format("{0}$chkCheckBox", chkTenancySupport.UniqueID)).ToLower() = "on", True, False)
                Else
                    Return chkTenancySupport.CheckBox.Checked
                End If
            End Get
            Set(ByVal Value As Boolean)
                chkTenancySupport.CheckBox.Checked = Value
            End Set
        End Property

        Public Property YourReference() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtYourReference.UniqueID))
                Else
                    Return txtYourReference.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtYourReference.Text = Value
            End Set
        End Property

        Public Property ServiceLevel() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtServiceLevel.UniqueID))
                Else
                    Return txtServiceLevel.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtServiceLevel.Text = Value
            End Set
        End Property

        Public Property UnitCost() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtUnitCost.UniqueID))
                Else
                    Return txtUnitCost.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtUnitCost.Text = Value
            End Set
        End Property

        Public Property PrimaryTitle() As Integer
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$cboDropDownList", cboPrimaryTitle.UniqueID))
                Else
                    Return IIf(cboPrimaryTitle.DropDownList.SelectedValue.Length = 0, 0, cboPrimaryTitle.DropDownList.SelectedValue)
                End If
            End Get
            Set(ByVal Value As Integer)
                If cboPrimaryTitle.DropDownList.Items.Count > 0 AndAlso Value > 0 Then cboPrimaryTitle.DropDownList.SelectedValue = Value
            End Set
        End Property

        Public Property PrimaryFirstNames() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtPrimaryFirstNames.UniqueID))
                Else
                    Return txtPrimaryFirstNames.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtPrimaryFirstNames.Text = Value
            End Set
        End Property

        Public Property PrimarySurname() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtPrimarySurname.UniqueID))
                Else
                    Return txtPrimarySurname.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtPrimarySurname.Text = Value
            End Set
        End Property

        Public Property PrimaryNINo() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtPrimaryNINo.UniqueID))
                Else
                    Return txtPrimaryNINo.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtPrimaryNINo.Text = Value
            End Set
        End Property

        Public Property PrimaryDoB() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtPrimaryDoB.UniqueID))
                Else
                    Return txtPrimaryDoB.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtPrimaryDoB.Text = Value
            End Set
        End Property

        Public Property Address() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtAddress.UniqueID))
                Else
                    Return txtAddress.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtAddress.Text = Value
            End Set
        End Property

        Public Property Postcode() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtPostcode.UniqueID))
                Else
                    Return txtPostcode.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtPostcode.Text = Value
            End Set
        End Property

        Public Property SecondaryTitle() As Integer
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$cboDropDownList", cboSecondaryTitle.UniqueID))
                Else
                    Return IIf(cboSecondaryTitle.DropDownList.SelectedValue.Length = 0, 0, cboSecondaryTitle.DropDownList.SelectedValue)
                End If
            End Get
            Set(ByVal Value As Integer)
                If cboSecondaryTitle.DropDownList.Items.Count > 0 AndAlso Value > 0 Then cboSecondaryTitle.DropDownList.SelectedValue = Value
            End Set
        End Property

        Public Property SecondaryFirstNames() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtSecondaryFirstNames.UniqueID))
                Else
                    Return txtSecondaryFirstNames.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtSecondaryFirstNames.Text = Value
            End Set
        End Property

        Public Property SecondarySurname() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtSecondarySurname.UniqueID))
                Else
                    Return txtSecondarySurname.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtSecondarySurname.Text = Value
            End Set
        End Property

        Public Property SecondaryNINo() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtSecondaryNINo.UniqueID))
                Else
                    Return txtSecondaryNINo.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtSecondaryNINo.Text = Value
            End Set
        End Property

        Public Property SecondaryDoB() As String
            Get
                If Me.IsPostBack Then
                    Return Request.Form(String.Format("{0}$txtTextBox", txtSecondaryDoB.UniqueID))
                Else
                    Return txtSecondaryDoB.Text
                End If
            End Get
            Set(ByVal Value As String)
                txtSecondaryDoB.Text = Value
            End Set
        End Property

#End Region

        Public Sub InitControl(ByVal thePage As BasePage)

            Const SP_NAME_FETCH_TITLES As String = "pr_FetchTitles"

            Dim msg As ErrorMessage
            Dim dt As DataTable

            With txtAddress.TextBox
                .TextMode = TextBoxMode.MultiLine
                .Rows = 5
                .Columns = 1
            End With

            ' grab the list of titles
            Try
                dt = SqlHelper.ExecuteDataset(thePage.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_TITLES).Tables(0)

                With cboPrimaryTitle.DropDownList
                    .DataSource = dt
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    .Items.Insert(0, New ListItem("", ""))
                End With

                With cboSecondaryTitle.DropDownList
                    .DataSource = dt
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    .Items.Insert(0, New ListItem("", ""))
                End With

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_TITLES, "NewSUNotifEnterDetails.InitControl()")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            End Try

        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)

            lblPrimaryTitle.AssociatedControlID = cboPrimaryTitle.ID
            lblSecondaryTitle.AssociatedControlID = cboSecondaryTitle.ID

        End Sub

    End Class

End Namespace
