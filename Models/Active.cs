using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ODataCovid.Models
{
	public class Active
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? day { get; set; }
        public float? value { get; set; }
        public long CountryRegionId { get; set; }
        public virtual CountryRegion? CountryRegion { get; set; }
    }
}

