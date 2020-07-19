using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AWSServerless_Google_Geocoding_Api.Domain
{
    public partial class Map
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public string ExcelPath { get; set; }
        public string FileName { get; set; }
        public string LastFileName { get; set; }
        public string PrevFileName { get; set; }
        public string BeforeFileName { get; set; }
        public int? FileCount { get; set; }
        public int? ModifyUserId { get; set; }
        public string ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? CreateUserId { get; set; }
        public string CreateHost { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? CreateDate { get; set; }

        public virtual User User { get; set; }
    }
}
