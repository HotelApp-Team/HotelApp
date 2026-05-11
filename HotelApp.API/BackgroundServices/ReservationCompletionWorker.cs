using HotelApp.Domain.Services;

namespace HotelApp.API.BackgroundServices;

// ReservationCompletionWorker sinifinin amaci, check-out tarihi gecmis Confirmed rezervasyonlari
// periyodik olarak Completed durumuna gecirmektir.
// Is mantigi Domain'deki IReservationService.MarkPastReservationsCompletedAsync icinde; bu sinif
// sadece zamanlama, scope yonetimi ve hata yakalama gibi hosting kaygilarini ustlenir.
// SOLID: SRP kullanir; sadece "ne zaman tetiklenir" sorusundan sorumlu, "nasil tamamlanir" Domain'e ait.
public class ReservationCompletionWorker : BackgroundService
{
    // Iki tarama arasinda bekleme suresi. Saatlik tarama hem yeterince guncel hem CPU/DB dostu.
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    // Uygulama acilirken DB/migration hazirligi tamamlanmadan calismayalim diye ilk gecikme.
    private static readonly TimeSpan StartupDelay = TimeSpan.FromSeconds(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReservationCompletionWorker> _logger;

    // BackgroundService singleton; IReservationService scoped. Bu yuzden her tarama icin
    // IServiceScopeFactory uzerinden yeni bir scope acip oradan service'i cekiyoruz.
    public ReservationCompletionWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<ReservationCompletionWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReservationCompletionWorker baslatildi. Tarama araligi: {Interval}.", Interval);

        // Uygulama startup'inda DB henuz hazir olmayabilir; ilk taramayi biraz ertele.
        if (!await SafeDelayAsync(StartupDelay, stoppingToken))
            return;

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunOnceAsync(stoppingToken);

            // Sonraki tarama icin bekle. Cancellation gelirse temiz cikis.
            if (!await SafeDelayAsync(Interval, stoppingToken))
                return;
        }
    }

    // Tek bir tarama turunu yurutur. Exception fitlatsa bile worker durmaz; sadece loglanir.
    private async Task RunOnceAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

            var affected = await reservationService.MarkPastReservationsCompletedAsync();

            // Sifir kaydi her saat loglayarak gurultu yapmayalim; sadece is yapilinca loglariz.
            if (affected > 0)
            {
                _logger.LogInformation(
                    "ReservationCompletionWorker: check-out gecmis {Count} rezervasyon Completed durumuna gecirildi.",
                    affected);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Uygulama kapanisi sirasinda beklenen iptal; sessizce gec.
        }
        catch (Exception ex)
        {
            // Beklenmedik hata: worker'i oldurme, sadece logla ki bir sonraki cycle yine denesin.
            _logger.LogError(ex, "ReservationCompletionWorker tarama sirasinda hata aldi.");
        }
    }

    // Task.Delay'i cancellation token ile sarmalayip, iptal halinde "devam etme" sinyali doner.
    private static async Task<bool> SafeDelayAsync(TimeSpan delay, CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(delay, stoppingToken);
            return true;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }
}
