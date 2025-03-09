using System.ComponentModel.DataAnnotations;

namespace addressbook_app.Models
{
    public class ContactModel
    {
        
        public int ContactID { get; set; }


        [Required]
        [Display(Name = "Country ID")]
        public int CountryID { get; set; }

        [Required]
        [Display(Name = "State ID")]
        public int StateID { get; set; }

        [Required]
        [Display(Name = "City ID")]
        public int CityID { get; set; }

        [Required]
        [Display(Name = "Country name")]
        public string Name { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string MobileNo { get; set; }

        [Required]
        public string WhatsAppNo { get; set; }

        [Required]
        public string EmailID { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        public DateTime DoB { get; set; }

        [Required]
        public string FaceBookID { get; set; }

        [Required]
        public string InstagramID { get; set; }

        [Required]
        public string BloodGroup { get; set; }

        [Display(Name = "creation date")]
        public string CreationDate { get; set; }

    }
}
