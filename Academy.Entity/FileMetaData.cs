using System.Text.Json.Serialization;

namespace Academy.Entity;

public class FileMetaData
    {
        /// <summary>
        /// File Id for the same.
        /// </summary>
        public string FileId { get; set; } = string.Empty;
        /// <summary>
        /// File Name which is optional
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        public string FolderStructure { get; set; }

        /// <summary>
        /// Generated File Key after file upload
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
        /// File extension
        /// </summary>
        public string Extension { get; set; } = string.Empty;
        /// <summary>
        /// Generates the dynamic guid, if set to true else uses file name. 
        /// </summary>
        public bool DynamicName { get; set; } = true;
        /// <summary>
        /// S3 Uploaded URL
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

        public string SizeSuffix(long value)
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
        public FileAccessability AccesssType { get; set; } = FileAccessability.Public;

        /// <summary>
        /// Indicates during initial file upload, the file is set to junk until, the record is updated in DB. Once it is updated in DB this is not set to junk. This is primarily used to clear Junk files from S3 Bucket.
        /// </summary>
        public bool IsJunk { get; set; } = true;

        [JsonIgnore]
        public bool UploadStatus { get; set; }

    }

    /// <summary>
    /// File accessabilty 'Public', 'Private'
    /// </summary>
    public enum FileAccessability
    {
        Public,
        Private
    }