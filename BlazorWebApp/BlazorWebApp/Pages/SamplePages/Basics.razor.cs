using Microsoft.AspNetCore.Components;
using PlaylistManagementSystem.BLL;
using PlaylistManagementSystem.ViewModels;

namespace BlazorWebApp.Pages.SamplePages
{
    public partial class Basics
    {
        #region Injections
        //  We are now injecting our service into our class using the [Inject] attribute.
        //  Before, we would have used the page constructor to add it.
        //  public Basic(PlaylistManagementService playlistManagementService)
        //  {
        //      _playlistManagementSystem = playlistManagementSystem;
        //  }
        [Inject]
        protected PlaylistManagementService? PlaylistManagementService { get; set; }
        #endregion

        #region Fields
        private string myName;
        private int oddEven;
        //  working version view
        private WorkingVersionView workingVersionView = new();
        #endregion

        //  Method invoked when the component is ready to start,
        //      having received it initial parameters from it parent in the render tree
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            RandomValue();

        }

        //  method for retrieving our version information
        private async Task GetDatabase()
        {
            workingVersionView = PlaylistManagementService.GetWorkingVersion();
            //  wait for the data to be retreived before we update the label on the page
            await InvokeAsync(StateHasChanged);
        }

        private void RandomValue()
        {
            Random rnd = new Random();
            oddEven = rnd.Next(0, 25);
            if (oddEven % 2 == 0)
            {
                myName = $"James is even {oddEven}";
            }
            else
            {
                myName = null;
            }
        }

    }
}
