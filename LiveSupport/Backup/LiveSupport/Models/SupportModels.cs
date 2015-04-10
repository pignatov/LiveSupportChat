using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LiveSupport.Models
{
    public class EmailModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Question")]
        public string Question { get; set; }

        [Required]
        [Display(Name = "Company")]
        public string Company { get; set; }
    }

    public class OperatorModel
    {
        [Required]
        [Display(Name = "User name")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Question")]
        public string Question { get; set; }
    }

}