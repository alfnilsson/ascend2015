<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

var start = DateTime.Now;

Console.WriteLine("Start");

System.Net.WebClient webClientPlan1 = new System.Net.WebClient();
webClientPlan1.DownloadStringCompleted += (sender, args) => {
	if (args.Error != null) { Console.WriteLine("Something went wrong"); return; }
	Console.WriteLine((DateTime.Now - start).ToString("s\\.fff") +  " Done alloy-plan");
};

System.Net.WebClient webClientPlan2 = new System.Net.WebClient();
webClientPlan2.DownloadStringCompleted += (sender, args) => {
	if (args.Error != null) { Console.WriteLine("Something went wrong"); return; }
	Console.WriteLine((DateTime.Now - start).ToString("s\\.fff") +  " Done alloy-plan");
};

System.Net.WebClient webClientPlan3 = new System.Net.WebClient();
webClientPlan3.DownloadStringCompleted += (sender, args) => {
	if (args.Error != null) { Console.WriteLine("Something went wrong"); return; }
	Console.WriteLine((DateTime.Now - start).ToString("s\\.fff") +  " Done alloy-plan which now is cached");
};

Task.WaitAll( new [] {
webClientPlan1.DownloadStringTaskAsync(new Uri("http://localhost:4247/alloy-plan")),
webClientPlan2.DownloadStringTaskAsync(new Uri("http://localhost:4247/alloy-plan"))
});
Console.WriteLine(string.Empty);
Console.WriteLine("Waiting ... ");
Console.WriteLine("4 ...");
Thread.Sleep(1000);
Console.WriteLine("3 ...");
Thread.Sleep(1000);
Console.WriteLine("2 ...");
Thread.Sleep(1000);
Console.WriteLine("1 ...");
Thread.Sleep(1000);

Console.WriteLine(string.Empty);
Console.WriteLine("0 Start alloy-plan");
start = DateTime.Now;
webClientPlan3.DownloadStringTaskAsync(new Uri("http://localhost:4247/alloy-plan"));