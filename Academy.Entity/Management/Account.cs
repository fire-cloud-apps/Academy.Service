using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Academy.Entity.Management;


 public class Account
    {
        /// <summary>
        /// A Unique Id to get account details.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Registered name or a name to identity the business
        /// </summary>
        [Required]
        [StringLength(150, ErrorMessage = "Name length can't be more than 150.")]
        public string BusinessName { get; set; }

        /// <summary>
        /// Business Details or some description about the account
        /// </summary>
        [StringLength(300, ErrorMessage = "Description length can't be more than 300.")]
        public string Description { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string Email { get; set; }
        /// <summary>
        /// Customer Contact Number
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// The exact full domain for the application it can be custom domain url or our own domain from netlify.
        /// </summary>
        [Required]
        public string? ServiceDomain { get; set; }

        /// <summary>
        /// Full database name eg. "AVS-DB"
        /// </summary>
        [Required]
        public string? DBName { get; set; }

        /// <summary>
        /// Full connection string value with the formated one
        /// eg. mongodb+srv://fc_client_admin:fc.clients.mongo@cluster0.acxm4.mongodb.net/{0}?retryWrites=true&w=majority&connect=replicaSet
        /// </summary>
        [Required]
        public string? ConnectionString { get; set; }

        /// <summary>
        /// In which country the business is located.
        /// </summary>

        public Country WhichCountry { get; set; }
        
        public FileMetaData AccountLogo { get; set; }
    }


    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Iso3 { get; set; }
        public string Iso2 { get; set; }
        public int Numeric_Code { get; set; }
        public string phone_code { get; set; }
        public string capital { get; set; }
        public string currency { get; set; }
        public string currency_name { get; set; }
        public string currency_symbol { get; set; }
        public string tld { get; set; }
        public string native { get; set; }
        public string region { get; set; }
        public string subregion { get; set; }
        public string timezones { get; set; }
        //public List<Timezone> timezones { get; set; }
        //public Translations translations { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string emoji { get; set; }
        public string emojiU { get; set; }
    }