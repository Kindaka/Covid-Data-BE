using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace ODataCovid.Models
{
    [Table("CountryRegion")]
    public class CountryRegion
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
		public float? latitude { get; set; }
		public float? longitude { get; set; }
		public string? countryName { get; set; }
        //public Point Location { get; set; }
        public  ICollection<CovidDaily>? CovidDailies { get; set; }
    }
}

