using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Entity.Management
{
    public class AcademicalYear
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime YearStart { get; set; }
        public DateTime YearEnd { get; set; }
        public bool IsActive { get; set; } = true;

        #region Internal Property
        private bool _isCurrent;
        public bool IsCurrent
        {
            get
            {
                return _isCurrent;
            }
            set
            {
                _isCurrent = DateTime.Now.Ticks > YearStart.Ticks && DateTime.Now.Ticks < YearEnd.Ticks;
            }
        }
        public bool IsDeleted { get; set; } = false;
        #endregion

    }
}
