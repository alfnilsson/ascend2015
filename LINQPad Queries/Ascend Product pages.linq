<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var start = DateTime.Now;

Console.WriteLine("Start");

System.Net.WebClient webClientPlan = new System.Net.WebClient();
webClientPlan.DownloadStringCompleted += (sender, args) => {
	if (args.Error != null) { Console.WriteLine("Something went wrong"); return; }
	Console.WriteLine((DateTime.Now - start).ToString("ss\\.fff") +  " Done alloy-plan");
};

System.Net.WebClient webClientMeet = new System.Net.WebClient();
webClientMeet.DownloadStringCompleted += (sender, args) => {	
	if (args.Error != null) { Console.WriteLine("Something went wrong"); return; }
	Console.WriteLine((DateTime.Now - start).ToString("ss\\.fff") +  " Done alloy-meet");
};

System.Net.WebClient webClientTrack = new System.Net.WebClient();
webClientTrack.DownloadStringCompleted += (sender, args) => {
	if (args.Error != null) { Console.WriteLine("Something went wrong"); return; }
	Console.WriteLine((DateTime.Now - start).ToString("ss\\.fff") +  " Done alloy-track");
};

Task.WaitAll( new [] {
webClientPlan.DownloadStringTaskAsync(new Uri("http://localhost:4247/alloy-plan")),
webClientMeet.DownloadStringTaskAsync(new Uri("http://localhost:4247/alloy-meet")),
webClientTrack.DownloadStringTaskAsync(new Uri("http://localhost:4247/alloy-track"))
});

Console.WriteLine(string.Empty);
Console.WriteLine((DateTime.Now - start).ToString("ss\\.fff") +  " done");