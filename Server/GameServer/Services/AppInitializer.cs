namespace GameServer.Services;

public class AppInitializer : BackgroundService
{
    private readonly AppState _appState;

    public AppInitializer(AppState state)
    {
        _appState = state;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(500, stoppingToken);

        Console.WriteLine("*** App initialization complete *** ");

        _appState.StartUp = true;
        _appState.Ready = true;
    }
}
