using System;

[Serializable()]
public class ChildMergeData
{
    private string _name;
    private string _location;
    private string _gender;
    private string _dob;

    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
        }
    }

    public string Location
    {
        get
        {
            return _location;
        }
        set
        {
            _location = value;
        }
    }

    public string Gender
    {
        get
        {
            return _gender;
        }
        set
        {
            _gender = value;
        }
    }

    public string DOB
    {
        get
        {
            return _dob;
        }
        set
        {
            _dob = value;
        }
    }
}