#nullable disable
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
        private int playlistId { get; set; } = 13;
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

        private async void Sort(string column)
        {
            Direction = SortField == column ? Direction == "asc" ? "desc"
                : "asc" : "asc";
            SortField = column;
            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                await FetchArtistOrAlbumTracks();
            }
        }

        //  sets css class to display up and down arrows
        private string GetSortColumn(string x)
        {
            return x == SortField ? Direction == "desc" ? "desc" : "asc" : "";
        }

        private async Task FetchArtistOrAlbumTracks()
        {
            try
            {
                //  we would normal check if the user has enter in a value into the search
                //      pattern but we will let the service do the error checking
                PaginatorTrackSelection = await PlaylistManagementService.FetchArtistOrAlbumTracks(
                    searchType, searchPattern, CurrentPage, PAGE_SIZE, SortField, Direction);
                //  At this moment, Blazor does not recognize the state has change and will not
                //      refresh the UI
                await InvokeAsync(StateHasChanged);
            }

            #region catch all exceptions
            catch (AggregateException ex)
            {
                foreach (var error in ex.InnerExceptions)
                {
                    feedback = error.Message;
                }
            }

            catch (ArgumentNullException ex)
            {
                feedback = GetInnerException(ex).Message;
            }

            catch (Exception ex)
            {
                feedback = GetInnerException(ex).Message;
            }
            #endregion
        }

        private async Task FetchPlaylist()
        {

            try
            {
                Playlists = await PlaylistManagementService.FetchPlaylist(userName, playlistName);
                await InvokeAsync(StateHasChanged);
            }

            #region catch all exceptions
            catch (AggregateException ex)
            {
                foreach (var error in ex.InnerExceptions)
                {
                    feedback = error.Message;
                }
            }

            catch (ArgumentNullException ex)
            {
                feedback = GetInnerException(ex).Message;
            }

            catch (Exception ex)
            {
                feedback = GetInnerException(ex).Message;
            }
            #endregion
        }

        private async Task AddTrackToPlaylist(int trackId)
        {
            try
            {
                PlaylistManagementService.AddTrack(userName, playlistName, trackId);
                await FetchPlaylist();
            }

            #region catch all exceptions
            catch (AggregateException ex)
            {
                foreach (var error in ex.InnerExceptions)
                {
                    feedback = error.Message;
                }
            }

            catch (ArgumentNullException ex)
            {
                feedback = GetInnerException(ex).Message;
            }

            catch (Exception ex)
            {
                feedback = GetInnerException(ex).Message;
            }
            #endregion
        }

        private async Task RemoveTracks()
        {
            try
            {
                List<int> removeTracks = new();
                foreach (var playlist in Playlists)
                {
                    if (playlist.Remove)
                    {
                        removeTracks.Add(playlist.TrackId);
                    }
                }
                PlaylistManagementService.RemoveTracks(playlistId, removeTracks);
                await FetchPlaylist();
            }

            #region catch all exceptions
            catch (AggregateException ex)
            {
                foreach (var error in ex.InnerExceptions)
                {
                    feedback = error.Message;
                }
            }

            catch (ArgumentNullException ex)
            {
                feedback = GetInnerException(ex).Message;
            }

            catch (Exception ex)
            {
                feedback = GetInnerException(ex).Message;
            }
            #endregion
        }

        private async Task ReorderTracks()
        {
            try
            {
                List<MoveTrackView> moveTracks = new();
                foreach (var playlist in Playlists)
                {
                    if (playlist.NewTrackNumber > 0)
                    {
                        moveTracks.Add(new MoveTrackView()
                        {
                            TrackId = playlist.TrackId,
                            TrackNumber = playlist.NewTrackNumber
                        });
                    }
                }
                PlaylistManagementService.MoveTracks(playlistId, moveTracks);
                await FetchPlaylist();

            }

            #region catch all exceptions
            catch (AggregateException ex)
            {
                foreach (var error in ex.InnerExceptions)
                {
                    feedback = error.Message;
                }
            }

            catch (ArgumentNullException ex)
            {
                feedback = GetInnerException(ex).Message;
            }

            catch (Exception ex)
            {
                feedback = GetInnerException(ex).Message;
            }
            #endregion
        }
        #region Methods
        private Exception GetInnerException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
        #endregion
    }
}
