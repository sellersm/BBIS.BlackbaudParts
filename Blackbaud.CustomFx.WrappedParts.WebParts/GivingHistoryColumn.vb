Imports System

<Serializable(), Xml.Serialization.XmlInclude(GetType(GivingHistoryColumn.SortType)), System.Runtime.Serialization.DataContract()> _
Public Class GivingHistoryColumn
    Public Sub New(ByVal tableName As String, ByVal columnName As String, ByVal displayOrder As Integer, ByVal fieldID As Integer)
        Me.TableName = tableName
        Me.ColumnName = columnName
        Me.DisplayOrder = displayOrder
        Me.FieldID = fieldID
    End Sub

    Public Sub New()
    End Sub
    <Xml.Serialization.XmlAttribute(), System.Runtime.Serialization.DataMember()> _
    Public TableName As String

    <Xml.Serialization.XmlAttribute(), System.Runtime.Serialization.DataMember()> _
    Public ColumnName As String

    <Xml.Serialization.XmlAttribute(), System.Runtime.Serialization.DataMember()> _
    Public ColumnSortType As GivingHistoryColumn.SortType

    <System.Runtime.Serialization.OptionalField(), System.Runtime.Serialization.DataMember()> _
    Public FieldLocalizationGuid As Guid

    <Xml.Serialization.XmlAttribute(), System.Runtime.Serialization.DataMember()> _
    Public DisplayOrder As Integer

    <Xml.Serialization.XmlAttribute(), System.Runtime.Serialization.DataMember()> _
    Public FieldID As Integer

    <System.Runtime.Serialization.DataMember()> _
    Public DefaultName As String

    ' Maps to [dbo].[GivingHistorySchemaFields].[SortType]
    '    via [CK_GivingHistorySchemaFields_SortType]
    Public Enum SortType
        DateTime = 0
        Numeric = 1
        [String] = 2
        Currency = 3
    End Enum
End Class

<System.Runtime.Serialization.DataContract()> _
Public Class GivingHistoryColumns
    <System.Runtime.Serialization.DataMember()> _
    Public Columns As Generic.List(Of GivingHistoryColumn)
End Class
