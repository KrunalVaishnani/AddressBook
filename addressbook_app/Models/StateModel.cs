using System.ComponentModel.DataAnnotations;

namespace addressbook_app.Models
{
    public class StateModel
    {
        public int StateID { get; set; }

        [Required]
        [Display(Name = "State name")]
        public string StateName { get; set; }

        [Required]
        [Display(Name = "State code")]
        public string StateCode { get; set; }

        [Required]
        [Display(Name = "State capital")]
        public string StateCapital { get; set; }

        [Required]
        [Display(Name = "Country Name")]
        public int CountryID { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public int UserID { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class StateDropDownModel
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }
}
