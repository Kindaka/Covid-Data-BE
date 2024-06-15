using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ODataCovid.Models
{
    [Table("CovidDaily")]
    public class CovidDaily
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? day { get; set; }
		public long? personConfirmed { get; set; }
		public long? personDeath { get; set; }
		public long? personRecovered { get; set; }
		public long ? personActive { get; set; }
		public long CountryRegionId { get; set; }
		public  CountryRegion? CountryRegion { get; set; }
    }
}

