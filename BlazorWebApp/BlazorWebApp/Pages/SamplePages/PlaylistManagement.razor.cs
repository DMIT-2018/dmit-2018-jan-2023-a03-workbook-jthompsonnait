
using Microsoft.AspNetCore.Components;
using PlaylistManagementSystem.BLL;
using PlaylistManagementSystem.Paginator;
using PlaylistManagementSystem.ViewModels;


namespace BlazorWebApp.Pages.SamplePages
{
    public partial class PlaylistManagement
    {
        #region Inject
        // we are injecting our service into our class using the [Inject] attribute
        [Inject]
        protected PlaylistManagementService? PlaylistManagementService { get; set; }
        #endregion

        #region fields

        private string searchPattern { get; set; } = "Deep";
        // preset the search type so that the radio button has a default of "Artist"
        private string searchType { get; set; } = "Artist";
        private string playlistName { get; set; } = "HansenB1";
        private string userName { get; set; } = "HansenB";
        private int playlistId { get; set; }
        private string feedback { get; set; }
        #endregion

        protected List<PlaylistTrackView> Playlists { get; set; } = new();

        #region Paginator
        // Desired current page size
        private const int PAGE_SIZE = 10;

        // sort column used with the paginator
        protected string SortField { get; set; } = "Owner";
        
        // sort direction for the paginator
        protected string Direction { get; set; } = "desc";

        //  current page for the paginator
        protected int CurrentPage { get; set; } = 1;

        //paginator collection of tracks selection view
        protected PagedResult<TrackSelectionView> PaginatorTrackSelection { get; set; } = new();

        #endregion


        private async Task FetchArtistOrAlbumTracks()
        {
            //  we would normal check if the user has enter in a value into the search
            //      pattern but we will let the service do the error checking
        }
    }
}
