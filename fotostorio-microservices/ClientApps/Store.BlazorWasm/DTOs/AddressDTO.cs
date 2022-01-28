using System.ComponentModel.DataAnnotations;

namespace Store.BlazorWasm.DTOs;

public class AddressDTO
{
    [Required(ErrorMessage = "First Name is required")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Your Street is required")]
    [Display(Name = "Street")]
    public string Street { get; set; }

    public string SecondLine { get; set; }

    [Required(ErrorMessage = "Your Town/City is required")]
    [Display(Name = "Town/City")]
    public string City { get; set; }

    [Required(ErrorMessage = "Your County is required")]
    [Display(Name = "County")]
    public string County { get; set; }

    [Required(ErrorMessage = "Your Post Code is required")]
    [Display(Name = "Post Code")]
    public string PostCode { get; set; }
}
