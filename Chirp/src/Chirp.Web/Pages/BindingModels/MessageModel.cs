using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages.BindingModels;

public class MessageModel {
    [BindProperty]
    [Required]
    [StringLength(160, ErrorMessage = "Maximum length needs to be {0}")]
    public string? Message {get; set;}
}