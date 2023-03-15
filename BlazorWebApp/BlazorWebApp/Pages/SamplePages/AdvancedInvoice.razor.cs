#nullable disable
using ChinookSystem.ViewModel;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorWebApp.Pages.SamplePages
{
    public partial class AdvancedInvoice
    {
        #region Fields
        private InvoiceView invoice;
        private string feedBack;
        private int counter = 1;

        //  Holds metadata related to a data editing process,
        //      such as flags to indicate which fields have been modified.
        //      and the current set of validation messages
        private EditContext? editContext;

        //  Used to store the validation messages
        private ValidationMessageStore? messageStore;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            invoice = new InvoiceView();
            Random rnd = new Random();
            invoice.InvoiceNo = rnd.Next(1000, 2000);
        }

        //  Handles the validation requested.  This allows for custom validation
        //      outside of using the DataAnnotationsValidator
        private void HandleValidationRequested(object? sender,
            ValidationRequestedEventArgs args)
        {

        }

        private void HandleSubmit()
        {
            feedBack = $"Submitted Press - {counter++}";
        }

        private void HandleValidSubmit()
        {
            feedBack = $"Valid Submit - {counter++}";
        }
        private void HandleInValidSubmit()
        {
            feedBack = $"InValid Submit - {counter++}";
        }
    }
}
