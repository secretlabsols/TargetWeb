
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls

Namespace Library.UserControls

    ''' <summary>
    ''' Displays the results of a generic finder search.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO   31/10/2011 BTI128 - added support for dynamic column widths
    ''' </history>
    Partial Public Class GenericFinderResults
        Inherits GenericFinderResultsBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim th As TableHeaderCell
            Dim tr As TableRow
            Dim td As TableCell
            Dim rdo As HtmlInputRadioButton

            ' setup the header row
            With phHeaderCells
                For Each col As DataColumn In Me.Results.Columns
                    If col.ColumnName <> "ID" Then
                        th = New TableHeaderCell
                        th.Text = Utils.SplitOnCapitals(col.ColumnName)
                        th.Style.Add("white-space", "nowrap")
                        th.Style.Add("vertical-align", "top")
                        If Me.ColumnWidths.ContainsKey(th.Text) Then
                            th.Style.Add("width", String.Format("{0}%", Me.ColumnWidths(th.Text)))
                        End If
                        .Controls.Add(th)
                    End If
                Next
            End With

            ' output the results
            For Each row As DataRow In Me.Results.Rows
                tr = New TableRow
                ' add radio button
                td = New TableCell
                rdo = New HtmlInputRadioButton
                rdo.Name = "rdoSelect"
                ' add the value to the rdo in the loop below
                td.Controls.Add(rdo)
                tr.Cells.Add(td)

                For Each col As DataColumn In Me.Results.Columns
                    If col.ColumnName = "ID" Then
                        rdo.Attributes.Add("onclick", String.Format("rdoSelect_Click({0})", row(col.Ordinal)))
                    Else
                        td = New TableCell
                        td.Attributes.Add("valign", "top")
                        td.Style.Add("text-overflow", "ellipsis")
                        td.Style.Add("overflow", "hidden")
                        td.Style.Add("white-space", "nowrap")
                        If Convert.IsDBNull(row(col.Ordinal)) Then
                            ' do nothing
                        ElseIf col.DataType Is GetType(Boolean) Then
                            td.Text = IIf(row(col.Ordinal), "Yes", "No")
                        ElseIf col.DataType Is GetType(Date) Then
                            td.Text = CDate(row(col.Ordinal)).ToString("dd/MM/yyyy")
                        Else
                            td.Text = row(col.Ordinal)
                        End If
                        If td.Text.Length = 0 Then td.Text = "&nbsp;"
                        tr.Cells.Add(td)
                    End If
                Next
                phDataRows.Controls.Add(tr)
            Next

            divPagingLinks.InnerHtml = Me.PagingLinks

        End Sub

    End Class

End Namespace