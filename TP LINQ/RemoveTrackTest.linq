<Query Kind="Program">
  <Connection>
    <ID>6908ebe3-e58f-4ab6-b1c6-6d010450e634</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>.</Server>
    <Database>ChinookSept2018</Database>
    <DisplayName>ChinookEntity</DisplayName>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
      <EFVersion>6.0.10</EFVersion>
      <TrustServerCertificate>True</TrustServerCertificate>
    </DriverData>
  </Connection>
</Query>

#load ".\ViewModels\*.cs"
using Chinook;
void Main()
{
	try
	{
		//	This is the DRIVER area.
		//  The command method will receive no model but will reveive
		//		individual arguments:  playlistId, List<trackId>

		//	793 A Castle Full of Rascals
		//	822	A Twist In The Tail
		//	543	Burn
		//	756	Child In Time

		string userName = "HansenB";
		string playlistName = "Jan23A03";
		int playlistId = Playlists
							.Where(x => x.UserName == userName
									&& x.Name == playlistName)
							.Select(x => x.PlaylistId).FirstOrDefault();
		if (playlistId == 0)
		{
			throw new ArgumentNullException($"No playlist exists for {playlistName}");
		}

		List<int> trackIds = new List<int>();  // = new();
											   trackIds.Add(822);
											   //trackIds.Add(543);

		//	show that we have playlist with our tracks in it
		Console.WriteLine("Before removing tracks");
		PlaylistTrackServices_FetchPlaylist(userName, playlistName).Dump();

		//	Remove the tracks
		PlaylistTrackServices_RemoveTracks(playlistId, trackIds);

		//	shaw that tracks has been removed and that our data has been reorder
		Console.WriteLine("After removing tracks");
		PlaylistTrackServices_FetchPlaylist(userName, playlistName).Dump();
	}

	#region  catch all exceptions
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}

	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}

	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	#endregion
}

#region Method
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}
#endregion



public void PlaylistTrackServices_RemoveTracks(int playlistId, List<int> trackIds)
{
	//	local	variables
	PlaylistTracks playlistTrackToRemove = null;
	PlaylistTracks playlistTrackToRenumber = null;
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

	var keeplist = PlaylistTracks
						.AsEnumerable()
						.Where(x => x.PlaylistId == playlistId &&
						trackIds.All(tid => tid != x.TrackId))
						.OrderBy(x => x.TrackNumber).ToList();

	foreach (var id in trackIds)
	{
		playlistTrackToRemove = PlaylistTracks
									.Where(x => x.PlaylistId == playlistId
											&& x.TrackId == id)
									.FirstOrDefault();
		if (playlistTrackToRemove != null)
		{
			PlaylistTracks.Remove(playlistTrackToRemove);
		}
	}

	foreach (var item in keeplist)
	{
		playlistTrackToRenumber = PlaylistTracks
									.Where(x => x.PlaylistId == playlistId
									&& x.TrackId == item.TrackId)
									.FirstOrDefault();
		if (playlistTrackToRenumber != null)
		{
			playlistTrackToRenumber.TrackNumber = tracknumber;
			PlaylistTracks.Update(playlistTrackToRenumber);

			//	this library is not directly accessable by LinqPAD
			//	EntityEntry<PlaylistTracks> updating = _context.Entry(playlistTrackToRenumber);
			//	updating.state = <Microsoft.EntityFrameworkCore.EntityState.Modify;

			//	get ready for next track
			tracknumber++;
		}
		else
		{
			var songName = Tracks
							.Where(x => x.TrackId == item.TrackId)
							.Select(x => x.Name)
							.SingleOrDefault();
			errorList.Add(new Exception($"The track ({songName}) is no longer on file. Please remove"));
		}
	}
	
	if (errorList.Count() > 0)
	{
		throw new AggregateException("Unable to remove request tracks.  Check Concerns", errorList);
	}
	else
	{
		//	all work has been staged
		SaveChanges();
	}
}


public List<PlaylistTrackView> PlaylistTrackServices_FetchPlaylist(string userName, string playlistName)
{
	//	Business Rules
	//	thesee are processing rules that need to be satisfied for valid data.
	//		rule:	search parttern value cannot be empty
	//		rule:	playlist must exist in the database (will be handle on webpage)

	if (string.IsNullOrWhiteSpace(playlistName))
	{
		throw new ArgumentNullException("Playlist name is missing");
	}

	return PlaylistTracks
	.Where(x => x.Playlist.Name == playlistName)
	.Select(x => new PlaylistTrackView
	{
		TrackId = x.TrackId,
		TrackNumber = x.TrackNumber,
		SongName = x.Track.Name,
		Milliseconds = x.Track.Milliseconds
	}).OrderBy(x => x.TrackNumber).ToList();
}
