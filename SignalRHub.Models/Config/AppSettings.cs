
namespace SignalRHub.Models.Config
{
    public class AppSettings
    {
        public AppSettings()
        {
            this.Redis = new Redis();
        }

        public Redis Redis { get; }
    }
}
