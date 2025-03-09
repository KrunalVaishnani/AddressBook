using System;
using System.ComponentModel.DataAnnotations;

namespace addressbook_app.Models
{
    public class ContactCategoryModel
    {
        public int ContactCategoryID { get; set; }

        [Required(ErrorMessage = "Contact Category Name is required.")] // Specifies the field is mandatory
        [StringLength(100, ErrorMessage = "Contact Category Name cannot exceed 100 characters.")]
        public string? ContactCategoryName { get; set; }

        [Required(ErrorMessage = "User ID is required.")] // Specifies the field is mandatory
        public int UserID { get; set; }

        [Required] // Specifies the field is mandatory
        [DataType(DataType.DateTime)] // Specifies the data type for formatting purposes
        public DateTime CreationDate { get; set; }
    }
}