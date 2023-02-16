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
		//  coded and test the AddTrack
		//	The command method will receive no collection bu will receive individual arguments
		//	userName, playlistName, trackID

		//	793 A Castle Full of Rascals
		//	822	A Twist In The Tail
		//	543	Burn
		//	756	Child In Time

		string userName = "HansenB";
		string playlistName = "Jan23A03";
		int trackId = 793;
		//	showing that both the playlist and track does not exist
		Console.WriteLine("Before adding Track");
		PlaylistTrackServices_FetchPlaylist(userName, playlistName);
		PlaylistTrackServices_AddTrack(userName, playlistName, trackId);
		//	showing that both the playist and tracks now exist
		Console.WriteLine("After adding Track");
		PlaylistTrackServices_FetchPlaylist(userName, playlistName);
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


//	check if the incoming data is completed(all parameter exists)
//if a problem exists, throw an ArgumentNullException for missing incoming values
//check that track exists
//does not exist, throws ArgumentException on the trackID
//check to see if playlist exists
//no(new playlist)
//create a new playlist record
//set track number to 1
//yes(appending to existing playlist)
//check if the track already exists on the playlist
//yes
//throw an exception that tracks already exist
//no
//determine the next track number
//add track to playlist tracks
//check for any errors
//yes: throw the list of all collected exceptions
//no: save all work to the database

public void PlaylistTrackServices_AddTrack(string userName, string playlistName, int trackId)
{
	// create local variables
	//  Check to ensure that the track has not been remove from the catelog/library
	bool trackExist = false;
	Playlists playlist = null;
	int trackNumber = 0;
	bool playlistTrackExist = false;
	PlaylistTracks playlistTracks = null;

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
	if(string.IsNullOrWhiteSpace(userName))
	{
		throw new ArgumentNullException("User name is missing");
	}
	if (string.IsNullOrWhiteSpace(playlistName))
	{
		throw new ArgumentNullException("Playlist name is missing");
	}
	#endregion
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