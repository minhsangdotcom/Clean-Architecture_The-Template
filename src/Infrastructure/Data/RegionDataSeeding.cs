using System.Text.Json;
using Domain.Aggregates.Regions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure.Data;

public class RegionDataSeeding
{
    public static async Task SeedingAsync(IServiceProvider provider)
    {
        IDbContext dbContext = provider.GetRequiredService<IDbContext>();

        var provinces = dbContext.Set<Province>();
        var districts = dbContext.Set<District>();
        var communes = dbContext.Set<Commune>();

        if(await provinces.AnyAsync() && await districts.AnyAsync() && await communes.AnyAsync())
        {
            return;
        }

        string path = Directory.GetParent(Directory.GetCurrentDirectory())!.FullName!;
        string fullPath = Path.Combine(path, "Infrastructure", "Data", "Seeds");

        string provinceFilePath = Path.Combine(fullPath, "Provinces.json");
        IEnumerable<Province>? provinceData = Read<Province>(provinceFilePath);
        await provinces.AddRangeAsync(provinceData ?? []);

        string districtFilePath = Path.Combine(fullPath, "Districts.json");
        IEnumerable<District>? districtData = Read<District>(districtFilePath);
        await districts.AddRangeAsync(districtData ?? []);

        string communeFilePath = Path.Combine(fullPath, "Wards.json");
        IEnumerable<Commune>? communeData = Read<Commune>(communeFilePath);
        await communes.AddRangeAsync(communeData ?? []);
        Log.Information("Seeding region data has finished....");
        await dbContext.SaveChangesAsync();
    }

    private static List<T>? Read<T>(string path)
        where T : class
    {
        using FileStream json = File.OpenRead(path);
        List<T>? datas = JsonSerializer.Deserialize<List<T>>(json);
        return datas;
    }
}
