Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Abacus.Web.Apps.Documents.UserControls
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Abacus.Library
Imports Target.Abacus.Library.CreditorPayments
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security



Namespace Apps.CreditorPayments

    ''' <summary>
    ''' View a creditors details
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir    19/04/2011  D11974 - SDS Generic Creditor Notes
    '''     PaulW      15/03/2011  D11974 - View Creditor Screen
    ''' </history>
    Partial Public Class ViewCreditor
        Inherits Target.Web.Apps.BasePage

        Private _stdBut As StdButtonsBase
        Private _documentsTabAllowed As Boolean = False
        Private _creditorID As Integer

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ' init the page
            InitPage(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.CreditorPayments"), "Creditor")

            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            _documentsTabAllowed = SecurityBL.UserHasMenuItem(Me.DbConnection, User.ID, _
                                   Target.Library.Web.ConstantsManager.GetConstant( _
                                   "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"), _
                                   Settings.CurrentApplicationID)

            _creditorID = Target.Library.Utils.ToInt32(Request.QueryString("id"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            Me.AddExtraCssStyle("span.roLabel {float:left;} ")

            ' setup buttons
            With _stdBut
                '.EditableControls.Add(tabDetails.Controls)
                .AllowNew = False
                .ShowNew = False
                .AllowFind = False
                .AllowBack = True
                .AllowEdit = False
                .AllowDelete = False
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked

            Me.JsLinks.Add("ViewCreditor.js")

            InitialiseDocumentSelector()
            InitialiseNotesSelector()
        End Sub

#End Region

#Region " Page_PreRenderComplete "

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            tabDocuments.Visible = _documentsTabAllowed
        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim creditor As ViewableGenericCreditor = Nothing

            msg = CreditorPaymentsBL.GetGenericCreditor(Me.DbConnection, _creditorID, creditor)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With creditor
                txtType.Text = .TypeDescription
                txtCreditorReference.Text = .CreditorReference
                txtName.Text = .Name
                txtAddress.Text = .Address
            End With


            txtAddress.Label.Style.Add("float", "left")
            txtAddress.ReadOnlyContentCssClass = "roLabel"

            'populate document tab
            If _documentsTabAllowed Then CType(docSelector, DocumentSelector).InitControl(Me.Page, _creditorID)

        End Sub

#End Region

#Region " InitialiseDocumentSelector "

        Private Sub InitialiseDocumentSelector()

            If Not _documentsTabAllowed Then Exit Sub

            CType(docSelector, DocumentSelector).ServiceUserType = Abacus.Library.DocumentAssociationType.Creditor

            CType(docSelector, DocumentSelector).Show_Buttons = (ShowButtons.New + ShowButtons.View + ShowButtons.Properties)

            CType(docSelector, DocumentSelector).Show_Filters = ( _
                            ShowFilters.Created + ShowFilters.CreatedBy + ShowFilters.CreatedFrom + _
                            ShowFilters.CreatedTo + ShowFilters.DocumentType + ShowFilters.Origin + _
                            ShowFilters.PrintStatus + ShowFilters.PrintStatusBy + _
                            ShowFilters.PrintStatusCheckBoxes + ShowFilters.PrintStatusFrom + _
                            ShowFilters.PrintStatusTo)

            CType(docSelector, DocumentSelector).InitControl(Me.Page, _creditorID)
        End Sub

#End Region

#Region "InitialiseNotesSelector"
        Private Sub InitialiseNotesSelector()
            'check if there is a client id
            If _creditorID > 0 Then
                'load the notes control
                With CType(Notes1, Target.Abacus.Web.Apps.UserControls.NotesSelector)
                    .FilterNoteType = Abacus.Library.Notes.NoteTypes.GenericCreditor
                    .FilterNoteTypeChildID = _creditorID
                    .ViewNoteInNewWindow = True
                    .InitControl(Me.Page)
                End With
            End If

        End Sub
#End Region


    End Class

End Namespace