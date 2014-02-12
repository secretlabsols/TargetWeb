Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library
Imports System.Collections.Specialized
Imports System.Collections.Generic
Imports System.Text
Imports Target.Library
Imports System.Reflection
Imports WebUtils = Target.Library.Web.Utils

''' <summary>
''' Represents values to filter document list
''' </summary>
''' <remarks></remarks>
''' <history>
''' IHS  11/03/2011  D11915 - added comments
''' </history>
Public Enum DocumentFilterList
    FilterDocumentTypes = 1
    FilterOrigin = 2
    FilterDescription = 3
    FilterCreatedFromDate = 4
    FilterCreatedToDate = 5
    FilterCreatedBy = 6
    FilterRecipientReference = 7
    FilterRecipientName = 8
    FilterNeverQueued = 9
    FilterQueued = 10
    FilterBatched = 11
    FilterSentToPrinter = 12
    FilterRemovedFromQueue = 13
    FilterPrintStatusFromDate = 14
    FilterPrintStatusToDate = 15
    FilterPrintStatusBy = 16
End Enum

''' <summary>
''' Helper class to assist with document filter logic
''' </summary>
''' <remarks></remarks>
''' <history>
''' IHS  11/03/2011  D11915 - added comments
''' </history>
Public Class DocumentFilterHelper

#Region " Private/Local Variables "
    Private _documentTypes As New List(Of Integer)

    Private _origin As DocumentOrigin

    Private _description As String

    Private _createdFromDate As Nullable(Of DateTime)
    Private _createdToDate As Nullable(Of DateTime)
    Private _createdBy As String

    Private _recipientReference As String
    Private _recipientName As String

    Private _neverQueued As Nullable(Of Boolean)
    Private _queued As Nullable(Of Boolean)
    Private _batched As Nullable(Of Boolean)
    Private _sentToPrinter As Nullable(Of Boolean)

    Private _printStatusFromDate As Nullable(Of DateTime)
    Private _printStatusToDate As Nullable(Of DateTime)
    Private _printStatusBy As String

    Private _queryString As NameValueCollection
    Const DateFormat As String = "dd/MM/yyyy"
    Const SqlDateFormat As String = "yyyyMMdd"
#End Region

#Region " Properties "
    Public ReadOnly Property DocumentTypes() As List(Of Integer)
        Get
            Return _documentTypes
        End Get
    End Property

    Public Property Origin() As DocumentOrigin
        Get
            Return _origin
        End Get
        Set(ByVal value As DocumentOrigin)
            _origin = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Property CreatedFromDate() As Nullable(Of DateTime)
        Get
            Return _createdFromDate
        End Get
        Set(ByVal value As Nullable(Of DateTime))
            _createdFromDate = value
        End Set
    End Property

    Public Property CreatedToDate() As Nullable(Of DateTime)
        Get
            Return _createdToDate
        End Get
        Set(ByVal value As Nullable(Of DateTime))
            _createdToDate = value
        End Set
    End Property

    Public Property CreatedBy() As String
        Get
            Return _createdBy
        End Get
        Set(ByVal value As String)
            _createdBy = value
        End Set
    End Property

    Public Property RecipientReference() As String
        Get
            Return _recipientReference
        End Get
        Set(ByVal value As String)
            _recipientReference = value
        End Set
    End Property

    Public Property RecipientName() As String
        Get
            Return _recipientName
        End Get
        Set(ByVal value As String)
            _recipientName = value
        End Set
    End Property

    Public Property NeverQueued() As Nullable(Of Boolean)
        Get
            Return _neverQueued
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            If value.HasValue AndAlso value.Value Then _neverQueued = value
        End Set
    End Property

    Public Property Queued() As Nullable(Of Boolean)
        Get
            Return _queued
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            If value.HasValue AndAlso value.Value Then _queued = value
        End Set
    End Property

    Public Property Batched() As Nullable(Of Boolean)
        Get
            Return _batched
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            If value.HasValue AndAlso value.Value Then _batched = value
        End Set
    End Property

    Public Property SentToprinter() As Nullable(Of Boolean)
        Get
            Return _sentToPrinter
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            If value.HasValue AndAlso value.Value Then _sentToPrinter = value
        End Set
    End Property

    Public Property PrintStatusFromDate() As Nullable(Of DateTime)
        Get
            Return _printStatusFromDate
        End Get
        Set(ByVal value As Nullable(Of DateTime))
            _printStatusFromDate = value
        End Set
    End Property

    Public Property PrintStatusToDate() As Nullable(Of DateTime)
        Get
            Return _printStatusToDate
        End Get
        Set(ByVal value As Nullable(Of DateTime))
            _printStatusToDate = value
        End Set
    End Property

    Public Property PrintStatusBy() As String
        Get
            Return _printStatusBy
        End Get
        Set(ByVal value As String)
            _printStatusBy = value
        End Set
    End Property
#End Region

#Region " Constructor "
    Public Sub New(ByRef queryStringValues As NameValueCollection)
        _queryString = queryStringValues
    End Sub

    Public Sub New()

    End Sub
#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Gets value for filter item
    ''' </summary>
    ''' <param name="filterItem"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Function qsVal(ByVal filterItem As DocumentFilterList) As String
        Return HttpUtility.UrlDecode(_queryString(qsKey(filterItem)))
    End Function

    ''' <summary>
    ''' Returns the filter key/name for a filter e.g. f1, f2 etc.
    ''' </summary>
    ''' <param name="filterItem"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Function qsKey(ByVal filterItem As DocumentFilterList) As String
        Return String.Format("f{0}", CType(filterItem, Integer))
    End Function

    ''' <summary>
    ''' Adds a filter to the filter list
    ''' </summary>
    ''' <param name="filterItem"></param>
    ''' <param name="filterValue"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Sub AddFilter(ByVal filterItem As DocumentFilterList, ByVal filterValue As String)
        _queryString.Add(qsKey(filterItem), HttpUtility.UrlEncode(filterValue))
    End Sub

    ''' <summary>
    ''' Populates this object based on a provided QueryString object
    ''' </summary>
    ''' <param name="queryString"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Sub SetFiltersFromQueryString(ByRef queryString As NameValueCollection)
        _queryString = queryString
        SetFiltersFromQueryString()
    End Sub

    ''' <summary>
    ''' Populates this object based on a provided QueryString object
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Function SetFiltersFromQueryString() As ErrorMessage
        Dim msg As New ErrorMessage

        Try
            Dim intTmp As Integer
            Dim qsDocTypes() As String = _queryString.GetValues(qsKey(DocumentFilterList.FilterDocumentTypes))

            If Not qsDocTypes Is Nothing Then
                For Each DocType As String In qsDocTypes
                    If Integer.TryParse(DocType, intTmp) Then
                        _documentTypes.Add(intTmp)
                    End If
                Next
            End If

            _origin = DocumentOrigin.NotSet
            If Not qsVal(DocumentFilterList.FilterOrigin) Is Nothing Then
                _origin = [Enum].Parse(GetType(DocumentOrigin), qsVal(DocumentFilterList.FilterOrigin), True)
            End If

            _description = qsVal(DocumentFilterList.FilterDescription)

            _createdFromDate = Nothing
            If Not qsVal(DocumentFilterList.FilterCreatedFromDate) Is Nothing Then
                _createdFromDate = CType(qsVal(DocumentFilterList.FilterCreatedFromDate), DateTime)
            End If

            _createdToDate = Nothing
            If Not qsVal(DocumentFilterList.FilterCreatedToDate) Is Nothing Then
                Dim tmpDateTime As DateTime

                If DateTime.TryParseExact(qsVal(DocumentFilterList.FilterCreatedToDate), "dd/MM/yyyy HH:mm", _
                                          Globalization.CultureInfo.CurrentCulture, Globalization.DateTimeStyles.None, _
                                          tmpDateTime) Then
                    _createdToDate = New Nullable(Of DateTime)(tmpDateTime)
                End If
            End If

            _createdBy = qsVal(DocumentFilterList.FilterCreatedBy)

            _recipientReference = qsVal(DocumentFilterList.FilterRecipientReference)
            _recipientName = qsVal(DocumentFilterList.FilterRecipientName)

            _neverQueued = Nothing
            If Not qsVal(DocumentFilterList.FilterNeverQueued) Is Nothing Then
                _neverQueued = (qsVal(DocumentFilterList.FilterNeverQueued) = "1")
            End If

            _queued = Nothing
            If Not qsVal(DocumentFilterList.FilterQueued) Is Nothing Then
                _queued = (qsVal(DocumentFilterList.FilterQueued) = "1")
            End If

            _batched = Nothing
            If Not qsVal(DocumentFilterList.FilterBatched) Is Nothing Then
                _batched = (qsVal(DocumentFilterList.FilterBatched) = "1")
            End If

            _sentToPrinter = Nothing
            If Not qsVal(DocumentFilterList.FilterSentToPrinter) Is Nothing Then
                _sentToPrinter = (qsVal(DocumentFilterList.FilterSentToPrinter) = "1")
            End If

            _printStatusFromDate = Nothing
            If Not qsVal(DocumentFilterList.FilterPrintStatusFromDate) Is Nothing Then
                _printStatusFromDate = CType(qsVal(DocumentFilterList.FilterPrintStatusFromDate), DateTime)
            End If

            _printStatusToDate = Nothing
            If Not qsVal(DocumentFilterList.FilterPrintStatusToDate) Is Nothing Then
                _printStatusToDate = CType(qsVal(DocumentFilterList.FilterPrintStatusToDate), DateTime)
            End If

            _printStatusBy = qsVal(DocumentFilterList.FilterPrintStatusBy)

            msg.Success = True
        Catch ex As Exception
            msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
        End Try

        Return msg
    End Function

    ''' <summary>
    ''' Returns a QueryString string populated with filter values
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Function PrepareFiltersQueryString() As String
        RemoveFiltersFromQueryString()

        ' reflect to readonly property
        Dim isreadonly As PropertyInfo = GetType(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance Or BindingFlags.NonPublic)

        ' make collection editable
        isreadonly.SetValue(_queryString, False, Nothing)

        If Not _documentTypes Is Nothing Then
            For Each intDocType As Integer In _documentTypes
                AddFilter(DocumentFilterList.FilterDocumentTypes, intDocType)
            Next
        End If

        If Not (_origin = DocumentOrigin.NotSet) Then
            AddFilter(DocumentFilterList.FilterOrigin, _origin)
        End If

        If Not String.IsNullOrEmpty(_description) Then
            AddFilter(DocumentFilterList.FilterDescription, _description)
        End If

        Dim tmp As String = _queryString.ToString()

        If _createdFromDate.HasValue Then
            AddFilter(DocumentFilterList.FilterCreatedFromDate, _createdFromDate.Value.ToString(DateFormat))
        End If

        If _createdToDate.HasValue Then
            ' get CreatedToDate with time format: 23:59
            Dim tmpDateTime As DateTime = _createdToDate.Value.Date.AddDays(1).AddMinutes(-1)
            AddFilter(DocumentFilterList.FilterCreatedToDate, tmpDateTime.ToString("dd/MM/yyyy HH:mm"))
        End If

        If Not String.IsNullOrEmpty(_createdBy) Then
            AddFilter(DocumentFilterList.FilterCreatedBy, _createdBy)
        End If

        If Not String.IsNullOrEmpty(_recipientReference) Then
            AddFilter(DocumentFilterList.FilterRecipientReference, _recipientReference)
        End If

        If Not String.IsNullOrEmpty(_recipientName) Then
            AddFilter(DocumentFilterList.FilterRecipientName, _recipientName)
        End If

        If _neverQueued.HasValue Then
            AddFilter(DocumentFilterList.FilterNeverQueued, "1")
        End If

        If _queued.HasValue Then
            AddFilter(DocumentFilterList.FilterQueued, "1")
        End If

        If _batched.HasValue Then
            AddFilter(DocumentFilterList.FilterBatched, "1")
        End If

        If _sentToPrinter.HasValue Then
            AddFilter(DocumentFilterList.FilterSentToPrinter, "1")
        End If

        If _printStatusFromDate.HasValue Then
            AddFilter(DocumentFilterList.FilterPrintStatusFromDate, _printStatusFromDate.Value.ToString(DateFormat))
        End If

        If _printStatusToDate.HasValue Then
            AddFilter(DocumentFilterList.FilterPrintStatusToDate, _printStatusToDate.Value.ToString(DateFormat))
        End If

        If Not String.IsNullOrEmpty(_printStatusBy) Then
            AddFilter(DocumentFilterList.FilterPrintStatusBy, _printStatusBy)
        End If

        ' make collection readonly again
        isreadonly.SetValue(_queryString, True, Nothing)

        Return _queryString.ToString()
    End Function

    ''' <summary>
    ''' Removes filters from the QueryString property
    ''' </summary>
    ''' <param name="queryString"></param>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Sub RemoveFiltersFromQueryString(ByRef queryString As NameValueCollection)
        _queryString = queryString

        RemoveFiltersFromQueryString()
    End Sub

    ''' <summary>
    ''' Removes filters from the QueryString property
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Sub RemoveFiltersFromQueryString()
        ' reflect to readonly property
        Dim isreadonly As PropertyInfo = GetType(System.Collections.Specialized.NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance Or BindingFlags.NonPublic)

        ' make collection editable
        isreadonly.SetValue(_queryString, False, Nothing)

        For Each filterItem As Integer In [Enum].GetValues(GetType(DocumentFilterList))
            _queryString.Remove(qsKey(filterItem))
        Next

        ' make collection readonly again
        isreadonly.SetValue(_queryString, True, Nothing)
    End Sub

    ''' <summary>
    ''' Returns the JavaScript variable name for a filter item
    ''' </summary>
    ''' <param name="filterItem"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Function GetJSvar(ByVal filterItem As DocumentFilterList) As String
        Return String.Format("list{0}", filterItem)
    End Function

    ''' <summary>
    ''' Creates the XML string for the Document Types
    ''' (needed in SP spxDocument_FetchListWithPaging)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Function GetDocTypeXML() As String
        Dim strBldr As New StringBuilder

        If _documentTypes.Count = 0 Then Return Nothing

        strBldr.Append("<documentType>")

        For Each intDocType As Integer In _documentTypes
            strBldr.AppendFormat("<documentTypeID>{0}</documentTypeID>", intDocType)
        Next

        strBldr.Append("</documentType>")

        Return strBldr.ToString()
    End Function

    ''' <summary>
    ''' Returns a string containing JavaScript filter variables 
    ''' populated with filter values
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS  11/03/2011  D11915 - added comments
    ''' </history>
    Public Function GetJavaScriptFilterVars() As String
        Dim strBuilder As New StringBuilder

        If _documentTypes.Count = 0 Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterDocumentTypes)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = ""{1}"";", GetJSvar(DocumentFilterList.FilterDocumentTypes), GetDocTypeXML()).AppendLine()
        End If

        If (_origin = DocumentOrigin.NotSet) Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterOrigin)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = {1};", GetJSvar(DocumentFilterList.FilterOrigin), CType(_origin, Integer)).AppendLine()
        End If

        If String.IsNullOrEmpty(_description) Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterDescription)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = ""{1}"";", GetJSvar(DocumentFilterList.FilterDescription), _description).AppendLine()
        End If

        If Not _createdFromDate.HasValue Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterCreatedFromDate)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = {1};", GetJSvar(DocumentFilterList.FilterCreatedFromDate), WebUtils.GetDateAsJavascriptString(_createdFromDate)).AppendLine()
        End If

        If Not _createdToDate.HasValue Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterCreatedToDate)).AppendLine()
        Else
            Dim jsStrDateFormat As String = _createdToDate.Value.ToString("MMMM dd, yyyy HH:mm:ss")
            Dim tmpStrDateCreated As String = String.Format("new Date(""{0}"")", jsStrDateFormat)
            strBuilder.AppendFormat("{0} = {1};", GetJSvar(DocumentFilterList.FilterCreatedToDate), tmpStrDateCreated).AppendLine()
        End If

        If String.IsNullOrEmpty(_createdBy) Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterCreatedBy)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = ""{1}"";", GetJSvar(DocumentFilterList.FilterCreatedBy), _createdBy).AppendLine()
        End If

        If String.IsNullOrEmpty(_recipientReference) Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterRecipientReference)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = ""{1}"";", GetJSvar(DocumentFilterList.FilterRecipientReference), _recipientReference).AppendLine()
        End If

        If String.IsNullOrEmpty(_recipientName) Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterRecipientName)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = ""{1}"";", GetJSvar(DocumentFilterList.FilterRecipientName), _recipientName).AppendLine()
        End If

        If Not _neverQueued.HasValue Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterNeverQueued)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = true;", GetJSvar(DocumentFilterList.FilterNeverQueued)).AppendLine()
        End If

        If Not _queued.HasValue Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterQueued)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = true;", GetJSvar(DocumentFilterList.FilterQueued)).AppendLine()
        End If

        If Not _batched.HasValue Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterBatched)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = true;", GetJSvar(DocumentFilterList.FilterBatched)).AppendLine()
        End If

        If Not _sentToPrinter.HasValue Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterSentToPrinter)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = true;", GetJSvar(DocumentFilterList.FilterSentToPrinter)).AppendLine()
        End If

        If Not _printStatusFromDate.HasValue Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterPrintStatusFromDate)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = {1};", GetJSvar(DocumentFilterList.FilterPrintStatusFromDate), WebUtils.GetDateAsJavascriptString(_printStatusFromDate)).AppendLine()
        End If

        If Not _printStatusToDate.HasValue Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterPrintStatusToDate)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = {1};", GetJSvar(DocumentFilterList.FilterPrintStatusToDate), WebUtils.GetDateAsJavascriptString(_printStatusToDate)).AppendLine()
        End If

        If String.IsNullOrEmpty(_printStatusBy) Then
            strBuilder.AppendFormat("{0} = null;", GetJSvar(DocumentFilterList.FilterPrintStatusBy)).AppendLine()
        Else
            strBuilder.AppendFormat("{0} = ""{1}"";", GetJSvar(DocumentFilterList.FilterPrintStatusBy), _printStatusBy).AppendLine()
        End If

        Return strBuilder.ToString()
    End Function

#End Region

End Class
