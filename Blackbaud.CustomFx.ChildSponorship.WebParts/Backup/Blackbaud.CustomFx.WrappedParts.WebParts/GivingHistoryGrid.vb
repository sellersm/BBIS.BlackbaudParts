Imports System.Collections.Generic

Namespace GivingHistory
  Public Class GivingHistoryGrid
        Inherits System.Web.UI.WebControls.Repeater

        Private _viewStateUniqueId As String
        Private Const GIFTSAMOUNTKEY As String = "GIFTS__AMOUNT"

        Private _gridSubtotalText As String
        Public Property GridSubtotalText() As String
            Get
                Return _gridSubtotalText
            End Get
            Set(ByVal value As String)
                _gridSubtotalText = value
            End Set
        End Property

    Public Property ViewStateUniqueId() As String
      Get
        Return _viewStateUniqueId
      End Get
      Set(ByVal value As String)
        _viewStateUniqueId = value
      End Set
    End Property

    Private _columns As System.Web.UI.WebControls.DataControlFieldCollection
    Public Property Columns() As System.Web.UI.WebControls.DataControlFieldCollection
      Get
        If _columns Is Nothing Then
          _columns = New System.Web.UI.WebControls.DataControlFieldCollection()
        End If

        Return _columns
      End Get
      Set(ByVal value As System.Web.UI.WebControls.DataControlFieldCollection)
        _columns = value
      End Set
    End Property

    Private _allowPaging As Boolean
    Public Property AllowPaging() As Boolean
      Get
        Return _allowPaging
      End Get
      Set(ByVal value As Boolean)
        _allowPaging = value
      End Set
    End Property

    Public Property PageSize() As Integer
      Get
        Dim o = ViewState("PageSize")

        If o IsNot Nothing Then
          Return CInt(o)
        Else
          Return 10
        End If
      End Get
      Set(ByVal value As Integer)
        ViewState("PageSize") = value
      End Set
    End Property

    Public Property CurrentPageIndex() As Integer
      Get
        Dim o = ViewState("CurrentPageIndex" & ViewStateUniqueId)

        If o IsNot Nothing Then
          Return CInt(o)
        Else
          Return 0
        End If
      End Get
      Set(ByVal value As Integer)
        ViewState("CurrentPageIndex" & ViewStateUniqueId) = value
      End Set
    End Property

    Public ReadOnly Property IsFirstPage() As Boolean
      Get
        If Me.AllowPaging AndAlso Me.DataSource IsNot Nothing AndAlso Me.DataSource.GetType() Is GetType(PagedDataSource) Then
          Return DirectCast(Me.DataSource, PagedDataSource).IsFirstPage
        Else
          Return True
        End If
      End Get
    End Property

    Private _gridAttributes As GivingHistoryGridAttributes
    Private ReadOnly Property GridAttributes() As GivingHistoryGridAttributes
      Get
        If _gridAttributes Is Nothing Then
          _gridAttributes = New GivingHistoryGridAttributes()
        End If

        Return _gridAttributes
      End Get
    End Property

    Public Property Summary() As String
      Get
        Return Me.GridAttributes.TableSummary
      End Get
      Set(ByVal value As String)
        Me.GridAttributes.TableSummary = value
      End Set
    End Property

    Public ReadOnly Property IsLastPage() As Boolean
      Get
        If Me.AllowPaging AndAlso Me.DataSource IsNot Nothing AndAlso Me.DataSource.GetType() Is GetType(PagedDataSource) Then
          Return DirectCast(Me.DataSource, PagedDataSource).IsLastPage
        Else
          Return True
        End If
      End Get
    End Property

    Private _colspanAll As Boolean
    Public Property ColspanAll() As Boolean
      Get
        Return _colspanAll
      End Get
      Set(ByVal value As Boolean)
        _colspanAll = value
      End Set
    End Property

    Private _GroupHeaders As Collections.Generic.Dictionary(Of Integer, String)
    Public Property GroupHeaders() As Collections.Generic.Dictionary(Of Integer, String)
      Get
        If _GroupHeaders Is Nothing Then
          _GroupHeaders = New Dictionary(Of Integer, String)
        End If
        Return _GroupHeaders
      End Get
      Set(ByVal value As Collections.Generic.Dictionary(Of Integer, String))
        _GroupHeaders = value
      End Set
        End Property

        Private _SubTotalHeaders As Collections.Generic.Dictionary(Of Integer, String)
        Public Property SubTotalHeaders() As Collections.Generic.Dictionary(Of Integer, String)
            Get
                If _SubTotalHeaders Is Nothing Then
                    _SubTotalHeaders = New Dictionary(Of Integer, String)
                End If
                Return _SubTotalHeaders
            End Get
            Set(ByVal value As Collections.Generic.Dictionary(Of Integer, String))
                _SubTotalHeaders = value
            End Set
        End Property

    Public Overrides Sub DataBind()
      Me.HeaderTemplate = New GivingHistoryGridTemplate(ListItemType.Header, Me.Columns, Me.HeaderCells, Me.GridAttributes)

      If Me.ColspanAll Then
        Me.ItemTemplate = New GivingHistoryInfoTemplate(Me.Columns)
      Else
        Me.ItemTemplate = New GivingHistoryGridTemplate(ListItemType.Item, Me.Columns, Me.HeaderCells, Me.GridAttributes)
      End If

      Me.FooterTemplate = New GivingHistoryGridTemplate(ListItemType.Footer, Me.Columns, Me.HeaderCells, Me.GridAttributes)

      If Me.AllowPaging Then
        Dim pagedData As New PagedDataSource()
        pagedData.DataSource = DirectCast(Me.DataSource, IEnumerable) 'IEnumerable
        pagedData.AllowPaging = True
        pagedData.PageSize = Me.PageSize
        pagedData.CurrentPageIndex = Me.CurrentPageIndex

        Me.DataSource = pagedData
      End If
      ItemsBound = If(Me.AllowPaging, Me.PageSize * Me.CurrentPageIndex, 0)
      MyBase.DataBind()
    End Sub


    Private ItemsBound As Integer = 0

    Protected Overrides Sub OnItemDataBound(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
      If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                If SubTotalHeaders.ContainsKey(ItemsBound) Then
                    'Find the amount column
                    Dim amountIndex As Integer = 0
                    For j As Integer = 0 To Columns.Count
                        Dim cItem As BoundField = DirectCast(Columns(j), BoundField)
                        If cItem.DataField.ToLower() = GIFTSAMOUNTKEY.ToLower() Then
                            amountIndex = j + 1
                            Exit For
                        End If
                    Next

                    Dim separatorRow As New TableRow()
                    'Need the number of columns in this row control
                    Dim numcolumns = e.Item.Controls(0).Controls.Count

                    For j As Integer = 0 To numcolumns
                        Dim td As TableCell = New TableCell()
                        td.CssClass = "TransactionManagerGridCell TransactionManagerGridCell0"
                        If j = (amountIndex - 1) Then
                            td.Text = String.Format("<b>{0}</b>", GridSubtotalText)
                        End If

                        If j = amountIndex Then
                            td.Text = SubTotalHeaders.Item(ItemsBound)
                        End If
                        separatorRow.Controls.Add(td)
                    Next


                    e.Item.Controls.AddAt(e.Item.Controls.Count, separatorRow)
                End If

                If GroupHeaders.ContainsKey(ItemsBound) Then
                    Dim separatorRow As New TableRow()
                    'Need the number of columns in this row control
                    Dim numcolumns = e.Item.Controls(0).Controls.Count
                    Dim td As New TableCell With {.CssClass = "TransactionManagerSeparatorCell TransactionManagerSeparatorCell", .Text = GroupHeaders.Item(ItemsBound), .ColumnSpan = numcolumns}
                    separatorRow.Controls.Add(td)
                    e.Item.Controls.AddAt(0, separatorRow)
                End If

                
        ItemsBound += 1
      End If
      MyBase.OnItemDataBound(e)
    End Sub

    ' TODO: Is this the best way to sync the other rows with the header row?
    Private _headerCells As List(Of TableCell)
    Private ReadOnly Property HeaderCells() As List(Of TableCell)
      Get
        If _headerCells Is Nothing Then
          _headerCells = New List(Of TableCell)
        End If

        Return _headerCells
      End Get
    End Property

    Private Sub GivingHistoryGrid_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles Me.ItemCreated
      If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
        ' BUG: This check needs to be removed, it is only here so that
        '       paging doesn't blow up. The "headers" attribute still gets set incorrectly
        '       on any page besides the first.
        If e.Item.Controls.Count > 0 Then
          Dim row = DirectCast(e.Item.Controls(0), TableRow)
          Dim cells = row.Cells

          For i = 1 To cells.Count - 1
            cells(i).Attributes("headers") = HeaderCells(i - 1).ClientID ' The HeaderCells colletion is zero based, but the actual cells begin at 1-based in the table
          Next
        End If
      End If
    End Sub

    Private Class GivingHistoryGridAttributes

      Private _tableSummary As String
      Public Property TableSummary() As String
        Get
          Return _tableSummary
        End Get
        Set(ByVal value As String)
          _tableSummary = value
        End Set
      End Property

    End Class

    Private Class GivingHistoryInfoTemplate
      Implements System.Web.UI.ITemplate

      Private _columns As System.Web.UI.WebControls.DataControlFieldCollection
      Public Property Columns() As System.Web.UI.WebControls.DataControlFieldCollection
        Get
          Return _columns
        End Get
        Set(ByVal value As System.Web.UI.WebControls.DataControlFieldCollection)
          _columns = value
        End Set
      End Property

      Public Sub New(ByVal columns As System.Web.UI.WebControls.DataControlFieldCollection)
        Me.Columns = columns
      End Sub

            Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
                Dim tr As New TableRow
                Dim td As New TableCell
                td.ColumnSpan = Me.Columns.Count + 1
                td.CssClass = "TransactionManagerGridNotificationCell"
                tr.Cells.Add(td)
                container.Controls.Add(tr)
            End Sub
        End Class

    Private Class GivingHistoryGridTemplate
      Implements System.Web.UI.ITemplate

      Private _templateType As ListItemType
      Private Property TemplateType() As ListItemType
        Get
          Return _templateType
        End Get
        Set(ByVal value As ListItemType)
          _templateType = value
        End Set
      End Property


      Private _columns As System.Web.UI.WebControls.DataControlFieldCollection
      Public Property Columns() As System.Web.UI.WebControls.DataControlFieldCollection
        Get
          Return _columns
        End Get
        Set(ByVal value As System.Web.UI.WebControls.DataControlFieldCollection)
          _columns = value
        End Set
      End Property

      Private _headerCells As List(Of TableCell)
      Private Property HeaderCells() As List(Of TableCell)
        Get
          Return _headerCells
        End Get
        Set(ByVal value As List(Of TableCell))
          _headerCells = value
        End Set
      End Property


      Private _gridAttributes As GivingHistoryGridAttributes
      Public Property GridAttributes() As GivingHistoryGridAttributes
        Get
          Return _gridAttributes
        End Get
        Set(ByVal value As GivingHistoryGridAttributes)
          _gridAttributes = value
        End Set
      End Property


      Public Sub New(ByVal type As ListItemType, ByVal columns As System.Web.UI.WebControls.DataControlFieldCollection, ByVal headerCells As List(Of TableCell), ByVal gridAttributes As GivingHistoryGridAttributes)
        Me.TemplateType = type
        Me.Columns = columns
        Me.HeaderCells = headerCells
        Me.GridAttributes = gridAttributes
      End Sub

      ' The Repeater's DataBind passes in the container of type "RepeaterItem"
      Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
        ' Open the table
        If Me.TemplateType = ListItemType.Header Then
          Dim tableOpen As New Literal()
          ' TODO: Make summary an editable field in the editor
          tableOpen.Text = String.Format("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""BBFormTable TransactionManagerGrid"" summary=""{0}"">", Me.GridAttributes.TableSummary)
          container.Controls.Add(tableOpen)
        End If

        If Me.TemplateType <> ListItemType.Footer Then
          Dim mainRow As TableRow

          If Me.TemplateType = ListItemType.Header Then
            mainRow = New TableHeaderRow()
            mainRow.ID = "headerRow"
          Else
            mainRow = New TableRow()
            mainRow.ID = "mainRow"
          End If

          Dim td As TableCell

          ' ====================================
          '  Add expand-button column
          ' ====================================
          If Me.TemplateType = ListItemType.Header Then
            td = New TableHeaderCell()
            td.CssClass = "TransactionManagerGridHeaderCell TransactionManagerGridHeaderCell0"
          Else
            td = New TableCell()
            td.CssClass = "TransactionManagerGridCell TransactionManagerGridCell0"
          End If

          mainRow.Cells.Add(td)

          ' ====================================

          Dim column As DataControlField

          For i = 0 To Columns.Count - 1
            column = Columns(i)

            If Me.TemplateType = ListItemType.Header Then
              td = New DataControlFieldHeaderCell(column)
              td.Attributes("scope") = "col"
              td.ID = column.HeaderText.Replace(" ", "_")
              td.CssClass = "TransactionManagerGridHeaderCell TransactionManagerGridHeaderCell" & (i + 1)
              Me.HeaderCells.Add(td)
            Else
              td = New DataControlFieldCell(column)
              td.CssClass = "TransactionManagerGridCell TransactionManagerGridCell" & (i + 1)
            End If

            mainRow.Cells.Add(td)
          Next

          container.Controls.Add(mainRow)

          ' Add details row to repeater items
          If Me.TemplateType <> ListItemType.Header Then
            Dim detailsCell As New TableCell()
            detailsCell.ColumnSpan = mainRow.Cells.Count
            detailsCell.ID = "detailsCell"
            detailsCell.CssClass = "TransactionManagerGridDetailCell"

            Dim detailsRow As New TableRow()
            detailsRow.ID = "detailsRow"
            detailsRow.Cells.Add(detailsCell)

            container.Controls.Add(detailsRow)
          End If
        End If

        ' Close the table
        If Me.TemplateType = ListItemType.Footer Then
          Dim tableClose As New Literal()
          tableClose.Text = "</table>"
          container.Controls.Add(tableClose)
        End If

        AddHandler container.DataBinding, New EventHandler(AddressOf Bind)
      End Sub

      Private Sub Bind(ByVal sender As Object, ByVal e As EventArgs)

        Dim ri = DirectCast(sender, RepeaterItem)

        If Me.TemplateType = ListItemType.Header Then
          For Each th As TableCell In DirectCast(ri.Controls(1), TableHeaderRow).Cells
            If th.GetType() Is GetType(DataControlFieldHeaderCell) Then
              Dim FieldName = DirectCast(DirectCast(th, DataControlFieldHeaderCell).ContainingField, BoundField).HeaderText
              If FieldName IsNot Nothing Then
                th.Text = FieldName.ToString
              Else
                th.Text = String.Empty
              End If
            End If
          Next
        End If

        If Me.TemplateType = ListItemType.Item Then
          For Each td As TableCell In DirectCast(ri.Controls(0), TableRow).Cells
            If td.GetType() Is GetType(DataControlFieldCell) Then
              Dim FieldName = DirectCast(DirectCast(td, DataControlFieldCell).ContainingField, BoundField).DataField
              Dim FieldValue = DataBinder.Eval(ri.DataItem, FieldName)
              If FieldValue IsNot Nothing Then
                td.Text = FieldValue.ToString
              Else
                td.Text = String.Empty
              End If
            End If
          Next
        End If

        ' TODO: raise event itemdatabound after done?
      End Sub
    End Class
  End Class
End Namespace