using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Entity.Management
{
    public class UserRoleMapping
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public QuickView UserId { get; set; }
        public QuickView RoleId { get; set; }

        #region Generic/Audit Fields
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        #endregion
    }
}
