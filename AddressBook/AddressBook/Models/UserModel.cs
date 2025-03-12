using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models
{
    public class UserModel
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Enter User Name!")]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter Mobile No.!")]
        [StringLength(50)]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Enter Email Address!")]
        [EmailAddress]
        public string EmailID { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }
    }

    public class UserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
    public class UserRegisterModel
    {
        public int? UserID { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        public string MobileNo { get; set; }

    }
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Username / Email / MobileNo is required.")]
        public string Credential { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }

}
