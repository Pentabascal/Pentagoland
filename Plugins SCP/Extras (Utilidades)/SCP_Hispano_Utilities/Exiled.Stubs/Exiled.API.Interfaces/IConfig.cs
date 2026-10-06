namespace Exiled.API.Interfaces
{
    public interface IConfig
    {
        bool IsEnabled { get; set; }
        bool Debug { get; set; }
    }
}
