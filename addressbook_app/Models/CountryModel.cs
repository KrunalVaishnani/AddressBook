using System.ComponentModel.DataAnnotations;

namespace addressbook_app.Models
{
    public class CountryModel
    {
        public int CountryID { get; set; }

        [Required]
        [Display(Name = "Country Name")]
        public string CountryName { get; set; }

        [Required]
        [Display(Name = "Country code")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Country capital must not be null")]
        public string CountryCapital {  get; set; }

        [Required]
        [Display(Name = "User Name")]
        public int UserID { get; set; }

        [Required]
        [Display(Name = "Creation date")]
        public DateTime CreationDate { get; set; }
    }

    public class CountryDropDownModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
    }
}
