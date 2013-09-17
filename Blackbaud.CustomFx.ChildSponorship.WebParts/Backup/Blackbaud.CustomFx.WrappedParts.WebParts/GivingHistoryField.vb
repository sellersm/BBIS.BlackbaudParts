Public Class GivingHistoryField

    Private _tableName As String
    Public Property TableName() As String
        Get
            Return _tableName
        End Get
        Set(ByVal value As String)
            _tableName = value
        End Set
    End Property

    Private _columnName As String
    Public Property ColumnName() As String
        Get
            Return _columnName
        End Get
        Set(ByVal value As String)
            _columnName = value
        End Set
    End Property

    Private _customName As String
    Public Property CustomName() As String
        Get
            Return _customName
        End Get
        Set(ByVal value As String)
            _customName = value
        End Set
    End Property

End Class
