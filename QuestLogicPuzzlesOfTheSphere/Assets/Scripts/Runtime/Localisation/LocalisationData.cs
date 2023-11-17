[System.Serializable]
public class LocalisationData
{
    public LocalisationItem[] Items;
}

[System.Serializable]
public class LocalisationItem
{
    public string Key;
    public string Value;
}