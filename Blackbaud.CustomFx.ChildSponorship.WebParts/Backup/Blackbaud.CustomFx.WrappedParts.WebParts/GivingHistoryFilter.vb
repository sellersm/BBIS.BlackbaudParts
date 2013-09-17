<Serializable()> _
Public Class GivingHistoryFilter
    Private mAll As Boolean
    Private mBBCFATypes As ArrayList

    Public Property All() As Boolean
        Get
            Return mAll
        End Get
        Set(ByVal value As Boolean)
            mAll = value
        End Set
    End Property

    Public Property BBCFATypes() As ArrayList
        Get
            Return mBBCFATypes
        End Get
        Set(ByVal value As ArrayList)
            mBBCFATypes = value
        End Set
    End Property

    Public Sub New()
        mAll = True
        mBBCFATypes = New ArrayList
    End Sub

    Public Sub New(ByVal bAll As Boolean, ByVal bbcfaTypes As ArrayList)
        mAll = bAll
        mBBCFATypes = bbcfaTypes
    End Sub

End Class
