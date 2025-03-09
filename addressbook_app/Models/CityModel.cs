using System.ComponentModel.DataAnnotations;

namespace addressbook_app.Models
{
    public class CityModel
    {
        public int CityID { get; set; }

        [Required]
        [Display(Name = "City name")]
        public string CityName { get; set; }

        [Required]
        [Display(Name = "City code")]
        public string CityCode { get; set; }

        [Required]
        [Display(Name = "State name")]
        public int StateID { get; set; }

        [Required]
        [Display(Name = "Country name")]
        public int CountryID { get; set; }

        [Required]
        [Display(Name = "User name")]
        public int UserID { get; set; }
        public DateTime CreationDate { get; set; }
    }
    public class CityDropDownModel
    {
        public int CityID { get; set; }
        public string CityName { get; set; }
    }
}
