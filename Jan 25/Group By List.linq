<Query Kind="Statements">
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

//  Use a multiple set of criteria (properties) to for the group 
//   also include a nested query to report on the "mini-collection" (smaller piles) 
//   of the grouped data. 

//  Display albums group by release label, and release year. 
//  Display the release year and the number of albums.   
//  List only the years with two or mor albums released 
//  For each album, display the title, year of release and count of tracks,. 

//  Original collection (large pile of data:  Albums 
//  Filtering cannot be decided until the groups are created. 
//  Grouping:  ReleaseLabel, ReleaseYear (anonymous key set: object) 
//  Now filtering can be done on the group:  group.Count >= 2 
//  Report the year and number of albums 
//  Nested query to report details per album: Title, Year, # of tracks. 

Albums
.GroupBy(x => new {x.ReleaseLabel, x.ReleaseYear})
.OrderBy(ag => ag.Key.ReleaseLabel)
.ThenBy(ag => ag.Key.ReleaseYear)
.Where(ag => ag.Count() >= 2)
.Select(ag => new
{
	ReleaseLabel = ag.Key.ReleaseLabel == null ? "Unknown" : ag.Key.ReleaseLabel,
	ReleaseYear = ag.Key.ReleaseYear,
	Count = ag.Count(),
	Albums = ag
			.Select(a => new
			{
				Title = a.Title,
				Year = a.ReleaseYear,
				Count = Tracks.Where(x => x.AlbumId == a.AlbumId).Count()
			})
}).Dump();

















