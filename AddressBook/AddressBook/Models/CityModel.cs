using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models
{
    public class CityModel
    {
        [Key]
        public int CityID { get; set; }

        [Required(ErrorMessage = "Enter State!")]
        public int StateID { get; set; }

        [Required(ErrorMessage = "Enter Country!")]
        public int CountryID { get; set; }

        [Required(ErrorMessage = "Enter City Name!")]
        [StringLength(100)]
        public string CityName { get; set; }

        [Required(ErrorMessage = "Enter City's STD Code!")]
        [StringLength(50)]
        public string STDCode { get; set; }

        [Required(ErrorMessage = "Enter City PinCode!")]
        [StringLength(6)]
        public string PinCode { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int UserID { get; set; }
    }
}
