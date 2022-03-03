namespace WeDontNeedRequired;
public class Program
{
    public static void Main(string[] args)
    {
        IHost host = Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => 
                {
                    builder.UseStartup<Startup>();
                }).Build();

        host.Run();
    }
}
