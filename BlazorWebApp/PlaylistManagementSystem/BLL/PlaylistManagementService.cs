using PlaylistManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistManagementSystem.BLL
{
    public class PlaylistManagementService
    {
        //  fetch playlist
        public List<PlaylistTrackView> FetchPlaylist(string userName, string playlistName)
        {
            return null;
        }

        //  fetch artist or album tracks
        public List<TrackSelectionView> FetchArtistOrAlbumTracks(string searchType,
            string searchValue)
        {
            return null;
        }

        //  add track
        public void AddTrack(string userName, string playlistName, int trackId)
        {

        }

        //  remove track(s)
        public void RemoveTracks(int playlistId, List<int> trackIds)
        {

        }

        //  move tracks
        public void MoveTracks(int playlistId, List<MoveTrackView> moveTracks)
        {

        }






    }
}
