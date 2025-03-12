using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models
{
    public class CountryModel
    {
        [Key]
        public int CountryID { get; set; }

        [Required(ErrorMessage = "Enter Country Name!")]
        [StringLength(100)]
        public string CountryName { get; set; }

        [Required(ErrorMessage = "Enter Country Code!")]
        [StringLength(50)]
        public string CountryCode { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int UserID { get; set; }
    }
    public class CountryDropDownModel
    {
        public int CountryID { get;set; }
        public string CountryName { get; set; }
    }
}
