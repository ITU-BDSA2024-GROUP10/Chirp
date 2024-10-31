using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages.BindingModels;

public class MessageModel {
    [BindProperty]
    public string Message {get; set;}
}