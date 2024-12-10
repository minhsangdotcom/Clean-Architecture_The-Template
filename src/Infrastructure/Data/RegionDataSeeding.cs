using System.Text.Json;
using Application.Common.Interfaces.UnitOfWorks;
using Domain.Aggregates.Regions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure.Data;

public class RegionDataSeeding
{
    public static async Task SeedingAsync(IServiceProvider provider)
    {
        IUnitOfWork unitOfWork = provider.GetRequiredService<IUnitOfWork>();

        if (
            await unitOfWork.Repository<Province>().AnyAsync()
            && await unitOfWork.Repository<District>().AnyAsync()
            && await unitOfWork.Repository<Commune>().AnyAsync()
        )
        {
            return;
        }

        string path = Directory.GetCurrentDirectory();
        string fullPath = Path.Combine(path, "Data", "Seeds");

        string provinceFilePath = Path.Combine(fullPath, "Provinces.json");
        IEnumerable<Province>? provinces = Read<Province>(provinceFilePath);
        await unitOfWork.Repository<Province>().AddRangeAsync(provinces ?? []);

        string districtFilePath = Path.Combine(fullPath, "Districts.json");
        IEnumerable<District>? districts = Read<District>(districtFilePath);
        await unitOfWork.Repository<District>().AddRangeAsync(districts ?? []);

        string communeFilePath = Path.Combine(fullPath, "Wards.json");
        IEnumerable<Commune>? communes = Read<Commune>(communeFilePath);
        await unitOfWork.Repository<Commune>().AddRangeAsync(communes ?? []);
        Log.Information("Seeding region data has finished....");

        await unitOfWork.SaveAsync();
    }

    private static List<T>? Read<T>(string path)
        where T : class
    {
        using FileStream json = File.OpenRead(path);
        List<T>? datas = JsonSerializer.Deserialize<List<T>>(json);
        return datas;
    }
}
