using System;

[Serializable()]
public class ConfirmationEmailOptions
{
    #region Fields
    private string _fromAddress;
    private string _fromName;
    private string _html;
    private string _replyAddress;
    private string _subject;
    private int _templateID;
    #endregion

    #region Properties
    public string FromAddress
    {
        get
        {
            return _fromAddress;
        }
        set
        {
            _fromAddress = value;
        }
    }

    public string FromName
    {
        get
        {
            return _fromName;
        }
        set
        {
            _fromName = value;
        }
    }

    public string HTML
    {
        get
        {
            return _html;
        }
        set
        {
            _html = value;
        }
    }

    public string ReplyAddress
    {
        get
        {
            return _replyAddress;
        }
        set
        {
            _replyAddress = value;
        }
    }

    public string Subject
    {
        get
        {
            return _subject;
        }
        set
        {
            _subject = value;
        }
    }

    public int TemplateID
    {
        get
        {
            return _templateID;
        }
        set
        {
            _templateID = value;
        }
    }
    #endregion
}