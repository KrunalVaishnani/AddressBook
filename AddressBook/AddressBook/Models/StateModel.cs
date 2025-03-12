using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models
{
    public class StateModel
    {
        [Key]
        public int StateID { get; set; }

        [Required(ErrorMessage = "Enter Country!")]
        public int CountryID { get; set; }

        [Required(ErrorMessage = "Enter State Name!")]
        [StringLength(100)]
        public string StateName { get; set; }

        [Required(ErrorMessage = "Enter State Code!")]
        [StringLength(50)]
        public string StateCode { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int UserID { get; set; }
    }
    public class StateDropDownModel
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }
}
