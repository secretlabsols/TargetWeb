Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports Target.Abacus.Library.SdsTransactions
Imports Target.Library
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Abacus.Library.DataClasses
Imports Target.Web.Apps

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control providing inputs for managing sds transactions
    ''' from the job service.
    ''' </summary>
    ''' <history>
    ''' Colin Daly   D11799 Created 06/09/2010
    ''' ColinDaly    D11799 Updated 10/11/2010 SDS Issue 377 - Added filter field FilterForceReconsideration
    ''' </history>
    Partial Public Class ManageSdsTransactionsStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

#Region "Fields"

        ' constants
        Private Const _GeneralErrorCode As String = ErrorMessage.GeneralErrorNumber

        ' locals
        Private _SelectableSdsTransactionTypes As List(Of SdsTransactionBL.SdsTransactionType) = Nothing

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' populate the transaction types
            PopulateSdsTransactionTypes()

            ' set the client detail id of the client selector control again
            InPlaceClientSelectorControl.ClientDetailID = Utils.ToInt32(InPlaceClientSelectorControl.GetPostBackValue())

            ' setup js links
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/CheckedListBox.js"))

        End Sub

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
        ''' Gets the in place client selector control.
        ''' </summary>
        ''' <value>The in place client selector control.</value>
        Private ReadOnly Property InPlaceClientSelectorControl() As InPlaceClientSelector
            Get
                Return CType(client, InPlaceClientSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the selectable SDS transaction types.
        ''' </summary>
        ''' <value>The selectable SDS transaction types.</value>
        Private ReadOnly Property SelectableSdsTransactionTypes() As List(Of SdsTransactionBL.SdsTransactionType)
            Get
                If _SelectableSdsTransactionTypes Is Nothing Then
                    ' if we havent fetched the selectable transaaction types then do so

                    Dim msg As New ErrorMessage()
                    Dim transactionTypes As List(Of SdsTransactionBL.SdsTransactionType) = Nothing

                    _SelectableSdsTransactionTypes = New List(Of SdsTransactionBL.SdsTransactionType)()

                    ' get all of the non excluded sds transaction types
                    msg = SdsTransactionBL.GetSdsTransactionTypesFromEnum(0, True, transactionTypes)
                    If Not msg.Success Then Throw New Exception(msg.ToString())

                    ' return a list of transaction types ordered by name
                    _SelectableSdsTransactionTypes = (From tmpTransactionType In transactionTypes _
                                                        Order By tmpTransactionType.ToString() _
                                                            Select tmpTransactionType).ToList()

                End If
                Return _SelectableSdsTransactionTypes
            End Get
        End Property

        ''' <summary>
        ''' Gets the selected client ID.
        ''' </summary>
        ''' <value>The selected client ID.</value>
        Private ReadOnly Property SelectedClientID() As Integer
            Get
                Return InPlaceClientSelectorControl.ClientDetailID
            End Get
        End Property

        ''' <summary>
        ''' Gets the selected transaction types as a summed integer.
        ''' </summary>
        ''' <value>The selected transaction types.</value>
        Private ReadOnly Property SelectedTransactionTypes() As Integer
            Get
                ' return the sum of all selected items in the checkbox list (PopulateTransactionTypes)
                ' must be called before this to reinstate view state to check box list control else wont work
                Return (From selectedItem As ListItem In cblTransactionTypes.Items _
                          Where selectedItem.Selected = True _
                               Select CType(selectedItem.Value, Integer)).Sum()
            End Get
        End Property

        ''' <summary>
        ''' Gets the selected transaction types from the request.
        ''' </summary>
        ''' <value>The selected transaction types from request.</value>
        Private ReadOnly Property SelectedTransactionTypesFromRequest() As List(Of SdsTransactionBL.SdsTransactionType)
            Get
                Dim selectedTransactionTypes As New List(Of SdsTransactionBL.SdsTransactionType)()
                Dim selectedTransactionTypeKeys As New List(Of String)()
                Dim transactionTypes As New List(Of SdsTransactionBL.SdsTransactionType)()

                ' get the existing transaction types, with exclusions applied
                transactionTypes = SelectableSdsTransactionTypes

                ' get all the keys from the request that start with the unique id of the check box list control
                ' the way the control works is to store each selected item in the request with the selected index
                ' after the last instance of a $ sign
                selectedTransactionTypeKeys = (From tmpRequestData In Request.Form.AllKeys _
                                                   Where tmpRequestData.StartsWith(cblTransactionTypes.UniqueID) _
                                                    Select tmpRequestData).ToList()

                If Not selectedTransactionTypeKeys Is Nothing AndAlso selectedTransactionTypeKeys.Count > 0 Then
                    ' if we have some transaction types selected

                    Dim selectedTransactionKeyIndex As Integer = 0

                    ' get the applicable transaction types
                    transactionTypes = SelectableSdsTransactionTypes

                    For Each selectedTransactionTypeKey As String In selectedTransactionTypeKeys
                        ' loop each key and get its value

                        ' get the index of the selected transaction type from the key
                        selectedTransactionKeyIndex = Integer.Parse(selectedTransactionTypeKey.Split("$").Last)

                        ' get the transaction type from the collection at the selected index
                        selectedTransactionTypes.Add(transactionTypes(selectedTransactionKeyIndex))

                    Next

                End If

                Return selectedTransactionTypes

            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether [selected force reconsideration].
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if [selected force reconsideration]; otherwise, <c>false</c>.
        ''' </value>
        Public ReadOnly Property SelectedForceReconsideration() As Boolean
            Get
                Dim cbForceReconsiderationRequestData As String = Request.Form(cbForceReconsideration.UniqueID)
                If String.IsNullOrEmpty(cbForceReconsiderationRequestData) = False _
                    AndAlso cbForceReconsiderationRequestData.Trim().Length > 0 Then
                    ' if we have some data then the check box was checked
                    Return True
                Else
                    ' else no data so not checked
                    Return False
                End If
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Populates the SDS transaction types.
        ''' </summary>
        Private Sub PopulateSdsTransactionTypes()

            Dim selectedTransactionTypes As List(Of SdsTransactionBL.SdsTransactionType) = SelectedTransactionTypesFromRequest

            ' get the selected transaction types from the request, we will use this to reset this data
            selectedTransactionTypes = SelectedTransactionTypesFromRequest

            If selectedTransactionTypes.Count = 0 Then
                ' if we have no selected transaction types

                cbSelectAllTransactionTypes.Checked = True

            End If

            ' loop each transaction type and add to check box list
            For Each transactionType As SdsTransactionBL.SdsTransactionType In SelectableSdsTransactionTypes

                Dim transationTypeItem As New ListItem(Utils.SplitOnCapitals(transactionType.ToString()), _
                                                       CType(transactionType, Integer))

                ' set the selected property based on whether this item was previosuly selected
                transationTypeItem.Selected = (selectedTransactionTypes.Count = 0 OrElse selectedTransactionTypes.Contains(transactionType))

                ' add the item to the checkbox list
                cblTransactionTypes.Items.Add(transationTypeItem)

            Next

            cbSelectAllTransactionTypes.Attributes.Add("onclick", _
                                                       String.Format("(this.checked == true) ? CheckedListBox_SelectAll('{0}') : CheckedListBox_SelectNone('{0}');", _
                                                                     cblTransactionTypes.ClientID))

        End Sub

#End Region

#Region "ICustomJobStepInputs"

        ''' <summary>
        ''' Gets the custom inputs.
        ''' </summary>
        ''' <param name="conn">The conn.</param>
        ''' <param name="trans">The trans.</param>
        ''' <param name="jobStepTypeID">The job step type ID.</param>
        ''' <returns></returns>
        Public Function GetCustomInputs(ByVal conn As System.Data.SqlClient.SqlConnection, ByVal trans As System.Data.SqlClient.SqlTransaction, ByVal jobStepTypeID As Integer) As System.Collections.Generic.List(Of System.Web.UI.Triplet) Implements Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim msg As New ErrorMessage()
            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim serviceUsr As ClientDetail = Nothing
            Dim sumOfSelectableTransactionTypes As Integer = 0
            Dim sumOfTransactionTypes As Integer = 0

            ' get the sum of all selectable transaction types, we will use this to compare with selected types
            ' this will then allow us to determine if the user has selected all available transaction types
            sumOfSelectableTransactionTypes = SelectableSdsTransactionTypes.Sum(Function(transactionType) CType(transactionType, Integer))

            If sumOfSelectableTransactionTypes = SelectedTransactionTypes Then
                ' the user has selected all available transaction types
                ' use a value of 0 to indicate we should process all, this
                ' renders All on the job steps instead of a large list of transaction types...tidier

                sumOfTransactionTypes = 0

            Else
                ' the user has only selected some of the transaction types

                sumOfTransactionTypes = SelectedTransactionTypes

            End If

            ' get the service user filter criteria
            result.Add(New Triplet(jobStepTypeID, "FilterServiceUserID", SelectedClientID))

            If SelectedClientID > 0 Then
                ' if we have a client then fetch and populate some
                ' invisible fields, used later for viewing purposes

                If Not trans Is Nothing Then
                    ' use transaction
                    serviceUsr = New ClientDetail(trans, String.Empty, String.Empty)
                Else
                    ' use connection
                    serviceUsr = New ClientDetail(conn, String.Empty, String.Empty)
                End If

                ' get the service user and add details to the result to be used later
                msg = serviceUsr.Fetch(SelectedClientID)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterServiceUserReferenceAndName", String.Format("({0}) {1}", serviceUsr.Reference, serviceUsr.Name)))

            End If

            ' get the service user filter criteria
            result.Add(New Triplet(jobStepTypeID, "FilterTransactionTypes", sumOfTransactionTypes))

            If sumOfTransactionTypes > 0 Then
                ' if we have some selected transaction types then set a descriptive 
                ' invisible field, used later for viewing purposes

                Dim selectedTransactionTypesAsString As String = String.Empty

                ' get the types in a string format and set a result to be used later
                msg = SdsTransactionBL.GetSdsTransactionTypesAsStringFromEnum(CType(sumOfTransactionTypes, SdsTransactionBL.SdsTransactionType), " AND ", selectedTransactionTypesAsString)
                If Not msg.Success Then Target.Library.Web.Utils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterSdsTransactionTypes", selectedTransactionTypesAsString))

            End If

            result.Add(New Triplet(jobStepTypeID, "FilterForceReconsideration", SelectedForceReconsideration))

            Return result

        End Function

#End Region

    End Class

End Namespace
