using System.Text.Json;
using Application.Common.Interfaces.Services;
using Domain.Aggregates.Regions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure.Data;

public class RegionDataSeeding
{
    public static async Task SeedingAsync(IServiceProvider provider)
    {
        IRegionService regionService = provider.GetRequiredService<IRegionService>();

        if (
            await regionService.AnyAsync<Province>()
            && await regionService.AnyAsync<District>()
            && await regionService.AnyAsync<Commune>()
        )
        {
            return;
        }

        string path = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName!;
        string fullPath = Path.Combine(path, "Infrastructure", "Data", "Seeds");

        string provinceFilePath = Path.Combine(fullPath, "Provinces.json");
        IEnumerable<Province>? provinceData = Read<Province>(provinceFilePath);
        await regionService.CreateRangeAsync(provinceData ?? []);

        string districtFilePath = Path.Combine(fullPath, "Districts.json");
        IEnumerable<District>? districtData = Read<District>(districtFilePath);
        await regionService.CreateRangeAsync(districtData ?? []);

        string communeFilePath = Path.Combine(fullPath, "Wards.json");
        IEnumerable<Commune>? communeData = Read<Commune>(communeFilePath);
        await regionService.CreateRangeAsync(communeData ?? []);
        Log.Information("Seeding region data has finished....");
    }

    private static List<T>? Read<T>(string path)
        where T : class
    {
        using FileStream json = File.OpenRead(path);
        List<T>? datas = JsonSerializer.Deserialize<List<T>>(json);
        return datas;
    }
}
