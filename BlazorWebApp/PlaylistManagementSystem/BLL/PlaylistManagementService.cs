#nullable disable
using System.Security.Cryptography.X509Certificates;
using PlaylistManagementSystem.ViewModels;
using PlaylistManagementSystem.DAL;
using PlaylistManagementSystem.Paginator;
using PlaylistManagementSystem.Entities;
using System.Diagnostics;

namespace PlaylistManagementSystem.BLL
{
    public class PlaylistManagementService
    {
        #region Fields

        private readonly PlaylistManagementContext _playlistManagementContext;

        #endregion

        internal PlaylistManagementService(PlaylistManagementContext playlistManagementContext)
        {
            _playlistManagementContext = playlistManagementContext;
        }

        //  get working version data
        public WorkingVersionView GetWorkingVersion()
        {
            return _playlistManagementContext.WorkingVersions
                .Select(x => new WorkingVersionView
                {
                    VersionId = x.VersionId,
                    Major = x.Major,
                    Minor = x.Minor,
                    Build = x.Build,
                    Revision = x.Revision,
                    AsOfDate = x.AsOfDate,
                    Comments = x.Comments

                }
                ).FirstOrDefault();
        }

        //  fetch playlist
        public async Task<List<PlaylistTrackView>> FetchPlaylist(string userName, string playlistName)
        {
            //	Business Rules
            //	thesee are processing rules that need to be satisfied for valid data.
            //		rule:	playlist nane cannot be empty
            //		rule:	playlist must exist in the database (will be handle on webpage)

            if (string.IsNullOrWhiteSpace(playlistName))
            {
                throw new ArgumentNullException("Playlist name is missing");
            }

            return _playlistManagementContext.PlaylistTracks
                .Where(x => x.Playlist.Name == playlistName)
                .Select(x => new PlaylistTrackView
                {
                    TrackId = x.TrackId,
                    TrackNumber = x.TrackNumber,
                    SongName = x.Track.Name,
                    Milliseconds = x.Track.Milliseconds
                }).OrderBy(x => x.TrackNumber).ToList();
        }

        //  fetch artist or album tracks
        public Task<PagedResult<TrackSelectionView>> FetchArtistOrAlbumTracks(string searchType,
            string searchValue, int page, int pageSize, string sortColumn, string direction)
        {
            //  Business Rules:
            //  These are processing rules that need to be satisfied for valid data.
            //      rule:   search value cannot be empty
            if (string.IsNullOrWhiteSpace(searchValue))
            {
                throw new ArgumentNullException("Search name is missing");
            }

            //  Task.FromResult() creates a finished Task that holds a value in its Result
            //      property
            return Task.FromResult(_playlistManagementContext.Tracks
                .Where(x => searchType == "Artist"
                    ? x.Album.Artist.Name.Contains(searchValue)
                    : x.Album.Title.Contains(searchValue))
                .Select(x => new TrackSelectionView
                {
                    TrackId = x.TrackId,
                    SongName = x.Name,
                    AlbumTitle = x.Album.Title,
                    ArtistName = x.Album.Artist.Name,
                    Milliseconds = x.Milliseconds,
                    Price = x.UnitPrice
                }).AsQueryable()
                .OrderBy(sortColumn, direction)
                .ToPagedResult(page, pageSize));
        }

        //  add track
        public void AddTrack(string userName, string playlistName, int trackId)
        {
            // create local variables
            //  Check to ensure that the track has not been remove from the catelog/library
            bool trackExist = false;
            Playlist playlist = null;
            int trackNumber = 0;
            bool playlistTrackExist = false;
            PlaylistTrack playlistTrack = null;

            #region Business Logic and Parameter Exceptions

            //	create a list<Exception> to contain all discovered errors
            List<Exception> errorList = new List<Exception>();

            //	Business Rules
            //	These are processing rules that need to be satisfied for valid data
            //		rule:	a track can only exist once on a playlist
            //		rule:	each track on a playlist is assigned a continous track number
            //		rule:	playlist nane cannot be empty
            //		rule:	track must exist in the tracks table
            //
            //	If the business rules are passed, consider the data valid:
            //		a)	stage your transaction work (Adds, Updates, Deletes)
            //		b)	execute a SINGLE .SaveChanges() - commits to database.

            //	We could assume that user name and track ID will always be valid.

            // parameter validation
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("User name is missing");
            }

            if (string.IsNullOrWhiteSpace(playlistName))
            {
                throw new ArgumentNullException("Playlist name is missing");
            }

            #endregion

            //  check that the incoming data exists
            trackExist = _playlistManagementContext.Tracks
                .Where(x => x.TrackId == trackId)
                .Select(x => x.TrackId)
                .Any();
            if (!trackExist)
            {
                throw new ArgumentNullException("Selected track no longer is on the system.  Refresh track table");
            }

            //  Business Process
            //  Check if the playlist exist
            playlist = _playlistManagementContext.Playlists
                .Where(x => x.Name == playlistName
                            && x.UserName == userName)
                .FirstOrDefault();

            //	does not exist
            if (playlist == null)
            {
                playlist = new Playlist()
                {
                    Name = playlistName,
                    UserName = userName
                };
                //  stage (only in memory)
                _playlistManagementContext.Playlists.Add(playlist);
                trackNumber = 1;
            }
            else
            {
                //  playlist already exist
                //	rule:	unique tracks on the playlist
                playlistTrackExist = _playlistManagementContext.PlaylistTracks
                    .Any(x => x.Playlist.Name == playlistName
                              && x.Playlist.UserName == userName
                              && x.TrackId == trackId);
                if (playlistTrackExist)
                {
                    var songName = _playlistManagementContext.Tracks
                        .Where(x => x.TrackId == trackId)
                        .Select(x => x.Name)
                        .FirstOrDefault();
                    // rule violation
                    errorList.Add(new Exception($"Selected track ({songName}) is already on the playlist"));
                }
                else
                {
                    trackNumber = _playlistManagementContext.PlaylistTracks
                        .Where(x => x.Playlist.Name == playlistName
                                    && x.Playlist.UserName == userName)
                        .Count();
                    //  increment this by 1
                    trackNumber++;
                }
            }

            //  add the track to the playlist
            //	create an instance for the playlist track

            playlistTrack = new PlaylistTrack();

            //	load values
            playlistTrack.TrackId = trackId;
            playlistTrack.TrackNumber = trackNumber;

            //	What about the second part of the primary key:  PlaylistID?
            //	If playlist exists, then we know the id:  playlist.PlaylistId
            //	But if the playlist is NEW, we DO NOT KNOW the ID

            //	In the sitution of a NEW playlist, even though we have created the
            //		playlist instance (see above), it is ONLY staged!!!
            //	This means that the actual SQL record has NOT uet been created.
            //	This means that the IDENTITY value for the new playlist DOES NOT
            //		yet exists.
            //	The value on the playlist instance (playlist) is zero (0)
            //		Therefore we have a serious problem.

            //	Solution
            //	It is build into the Entity Framework software and is based using
            //		the navigational property in the Playlist point to it's "child"

            //	Staging a typical Add in the past was to refernce the entity and 
            //		use the entity.Add(xxx)
            //	If you use this statement, the playlistId would be zero (0)
            //		causing your transaction to ABORT.
            //	Why?	PKeys cannot be zero (0) (FKey to PKey problem)

            //	INSTEAD, do the stating using the "parent.navChildProperty.Add(xxx)
            playlist.PlaylistTracks.Add(playlistTrack);

            //	Staging is complete.
            //	Commit the work (Transaction)
            //	Committing the work needs a .SaveChanges()
            //	A tranaction has ONLY ONE .SaveChanges()
            //	But, what if you have discoved errors during the bussiness process???
            if (errorList.Count > 0)
            {
                //  we need to clear the "track changes" otherwise we leave our entity system in flux
                _playlistManagementContext.ChangeTracker.Clear();
                //  throw the list of business processing error(s)
                throw new AggregateException("Unable to add new track. Check conerns", errorList);
            }
            else
            {
                //  consider data valid
                //	hass passed business processing rules
                _playlistManagementContext.SaveChanges();
            }
        }

        //  remove track(s)
        public void RemoveTracks(int playlistId, List<int> trackIds)
        {
            {
                //	local	variables
                PlaylistTrack playlistTrackToRemove = null;
                PlaylistTrack playlistTrackToRenumber = null;
                int tracknumber = 1;

                //	we need a container to hold x number of Exception messages
                List<Exception> errorList = new();

                if (playlistId == null || playlistId == 0)
                {
                    throw new ArgumentNullException("Playlist id was not provided");
                }

                //var count = trackIds.Count();
                //if (count == 0)
                if (trackIds.Count() == 0)
                {
                    throw new ArgumentNullException("No list of tracks were submitted");
                }

                //	obtain the tracks to keep
                //	create a query to extrack the "keep" tracks from the incoming track list
                //	we want all playlist tracks that are part of playlist and not in the collection
                //		of tracks that we are removing

                var keeplist = _playlistManagementContext.PlaylistTracks
                    .AsEnumerable()
                    .Where(x => x.PlaylistId == playlistId &&
                                trackIds.All(tid => tid != x.TrackId))
                    .OrderBy(x => x.TrackNumber).ToList();

                foreach (var id in trackIds)
                {
                    playlistTrackToRemove = _playlistManagementContext.PlaylistTracks
                        .Where(x => x.PlaylistId == playlistId
                                    && x.TrackId == id)
                        .FirstOrDefault();
                    if (playlistTrackToRemove != null)
                    {
                        _playlistManagementContext.PlaylistTracks.Remove(playlistTrackToRemove);
                    }
                }

                foreach (var item in keeplist)
                {
                    playlistTrackToRenumber = _playlistManagementContext.PlaylistTracks
                        .Where(x => x.PlaylistId == playlistId
                                    && x.TrackId == item.TrackId)
                        .FirstOrDefault();
                    if (playlistTrackToRenumber != null)
                    {
                        playlistTrackToRenumber.TrackNumber = tracknumber;
                        _playlistManagementContext.PlaylistTracks.Update(playlistTrackToRenumber);

                        //	this library is not directly accessable by LinqPAD
                        //	EntityEntry<PlaylistTracks> updating = _context.Entry(playlistTrackToRenumber);
                        //	updating.state = <Microsoft.EntityFrameworkCore.EntityState.Modify;

                        //	get ready for next track
                        tracknumber++;
                    }
                    else
                    {
                        var songName = _playlistManagementContext.Tracks
                            .Where(x => x.TrackId == item.TrackId)
                            .Select(x => x.Name)
                            .SingleOrDefault();
                        errorList.Add(new Exception($"The track ({songName}) is no longer on file. Please remove"));
                    }
                }

                if (errorList.Count() > 0)
                {
                    //  we need to clear the "track changes" otherwise we leave our entity system in flux
                    _playlistManagementContext.ChangeTracker.Clear();
                    throw new AggregateException("Unable to remove request tracks.  Check Concerns", errorList);
                }
                else
                {
                    //	all work has been staged
                    _playlistManagementContext.SaveChanges();
                }
            }
        }

        //  move tracks
        public void MoveTracks(int playlistId, List<MoveTrackView> moveTracks)
        {

        }






    }
}
