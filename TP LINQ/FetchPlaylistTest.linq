<Query Kind="Program">
  <Connection>
    <ID>3f77383b-7ae5-4f03-8c46-ddbe5b1af50d</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>.</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <DeferDatabasePopulation>true</DeferDatabasePopulation>
    <Database>ChinookSept2018</Database>
    <DisplayName>ChinookSept2018</DisplayName>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
</Query>

#load ".\ViewModels\*.cs"
using Chinook;
void Main()
{
	try
	{
		//  PlaylistTrackServices_FetchPlaylist
		//	PlaylistTrackServices is the BLL
		//	FetchPlaylist is the method name

		string userName = "HansenB";
		string playlistName = "HansenB11";
		List<PlaylistTrackView> tracks = PlaylistTrackServices_FetchPlaylist(userName, playlistName);
		if (tracks.Count == 0)
		{
			Console.WriteLine($"No playlist was found for {playlistName}");
		}
		else
		{
			tracks.Dump();
		}

	}

	//  catch all exceptions
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
}

#region Method
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}

#endregion




public List<PlaylistTrackView> PlaylistTrackServices_FetchPlaylist(string userName, string playlistName)
{
	//	Business Rules
	//	thesee are processing rules that need to be satisfied for valid data.
	//		rule:	playlist nane cannot be empty
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
