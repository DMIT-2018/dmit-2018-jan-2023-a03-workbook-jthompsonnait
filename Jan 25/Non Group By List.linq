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
.OrderBy(x => x.ReleaseLabel)
.ThenBy(x => x.ReleaseYear)
.Select(x => new
{
	ReleaseLabel = x.ReleaseLabel == null ? "Unknown" : x.ReleaseLabel,
	ReleaseYear = x.ReleaseYear,
	Albums = Albums
			.Where(a =>a.ReleaseLabel == x.ReleaseLabel 
							&& a.ReleaseYear == x.ReleaseYear)
			.Select(a => new {
			Title = a.Title,
			Year = a.ReleaseYear,
			Count = a.Tracks.Count()
			})
}).Dump();

















