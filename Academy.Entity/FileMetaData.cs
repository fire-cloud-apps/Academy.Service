using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using Academy.Entity.Management;

namespace Academy.Entity;

public class FileMetaData
{
    /// <summary>
    /// File Id for the same.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Just a GUID obtained once we upload the file into S3 Bucket.
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// Who is owning this File, Eg. StudentId, SchoolId, ParentId, StaffId etc
    /// Association Id or foreign Key id.
    /// </summary>
    public QuickView AssociatedWith { get; set; }
    /// <summary>
    /// File Name which is optional
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Folder Structure exists in the S3 Bucket Eg. 'account/png'
    /// </summary>
    public string FolderStructure { get; set; }

    /// <summary>
    /// Generated File Key after file upload. Used to delete the file.
    /// </summary>
    public string FileKey { get; set; } = string.Empty;
    /// <summary>
    /// Client name, which will be used as S3-Bucket Name
    /// </summary>
    public string ClientName { get; set; } = string.Empty;
    /// <summary>
    /// Module name, eg. User, Student, etc.
    /// </summary>
    public string ModuleName { get; set; } = string.Empty;
    /// <summary>
    /// File extension, Eg. .png, .doc, .pdf
    /// </summary>
    public string Extension { get; set; } = string.Empty;
    /// <summary>
    /// Generates the dynamic guid, if set to true else uses file name. 
    /// </summary>
    public bool DynamicName { get; set; } = true;
    /// <summary>
    /// S3 Uploaded URL. Eg. https://fc-demo-s3-storage.s3.ap-south-1.amazonaws.com/account/png/66e1c63a-d566-4a05-9486-ecd68f4ba24c.png
    /// </summary>
    public string S3URL { get; set; } = string.Empty;
    /// <summary>
    /// Original File Size
    /// </summary>
    public long FileSize { get; set; }

    static readonly string[] SizeSuffixes =
              { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    private string _fileSizeSuffix;
    /// <summary>
    /// Returns files size in readable format eg 12.5 KB or 50.23 MB etc.
    /// </summary>
    public string FileSizeSuffix
    {
        get
        {
            return _fileSizeSuffix;
        }
        set
        {
            _fileSizeSuffix = SizeSuffix(FileSize);
        }
    }

    public static string SizeSuffix(long value)
    {
        if (value < 0) { return "-" + SizeSuffix(-value); }

        int i = 0;
        decimal dValue = (decimal)value;
        while (Math.Round(dValue / 1024) >= 1)
        {
            dValue /= 1024;
            i++;
        }

        return string.Format("{0:n1} {1}", dValue, SizeSuffixes[i]);
    }

    /// <summary>
    /// Sets the file is public or private. 0 - Public, 1 - Private
    /// </summary>
    public FileAccessibility AccessType { get; set; } = FileAccessibility.Public;

    /// <summary>
    /// Indicates during initial file upload, the file is set to junk until, the record is updated in DB. Once it is updated in DB this is not set to junk. This is primarily used to clear Junk files from S3 Bucket.
    /// </summary>
    //public bool IsJunk { get; set; } = true;

    [JsonIgnore]
    public bool UploadStatus { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

}

/// <summary>
/// File FileAccessibility 'Public', 'Private'
/// </summary>
public enum FileAccessibility
{
    Public,
    Private
}