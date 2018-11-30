using System.IO;

public class FilePath
{
    public FileLocation Location { get; private set; }
    public string FullFilePath { get; private set; }
    private DataType _type;
    private bool _localUser;

    public FilePath(bool localUser, DataType type)
    {
        if (localUser && type == DataType.Shop)
        {
            Location = FileLocation.StreamingAssets;
        }
        else
        {
            Location = FileLocation.PersistentDataPath;
        }

        _localUser = localUser;
        _type = type;
        FullFilePath = GeneratePath();
    }

    public string GeneratePath()
    {
        string fileName = string.Empty;
        string fullFilePath = GetPathByFileLocation(Location);

        if (Location != FileLocation.StreamingAssets && !Directory.Exists(fullFilePath))
        {
            Directory.CreateDirectory(fullFilePath);
        }

        switch (_type)
        {
            case DataType.Duels:
                fileName = "1.dat";
                break;
            case DataType.Shop:
                if (_localUser)
                {
                    fileName = "2l.dat";
                }
                else
                {
                    fileName = "2.dat";
                }
                break;
            case DataType.Friends:
                fileName = "3.dat";
                break;
            case DataType.Suits:
                fileName = "4.dat";
                break;
            case DataType.Trades:
                fileName = "5.dat";
                break;
            case DataType.UserInfo:
                fileName = "6.dat";
                break;
        }

        return Path.Combine(fullFilePath, fileName);
    }

    private string GetPathByFileLocation(FileLocation location)
    {
        switch (location)
        {
            case FileLocation.PersistentDataPath:
                return Path.Combine(GameController.PersistentDataPath, "Data");
            case FileLocation.StreamingAssets:
                return GameController.StreamingAssetsPath;
        }
        return string.Empty;
    }
}

public enum FileLocation
{
    StreamingAssets, PersistentDataPath
}