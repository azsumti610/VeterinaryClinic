
using Microsoft.EntityFrameworkCore;
using VeterinaryClinic.Models;

namespace VeterinaryClinic.Data
{
    public static class AnimalDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var animalData = new Dictionary<string, string[]>
            {
                ["Куче"] = new[]
                {
                    "Немска овчарка", "Лабрадор",
                    "Голдън ретривър", "Хъски",
                    "Чихуахуа", "Пудел",
                    "Булдог", "Джак Ръсел",
                    "Йоркширски териер", "Смесена порода"
                },
                ["Котка"] = new[]
                {
                    "Персийска", "Сиамска",
                    "Британска късокосместа",
                    "Мейн Куун", "Сфинкс",
                    "Бенгалска", "Смесена порода"
                },
                ["Папагал"] = new[]
                {
                    "Вълнист папагал", "Корела",
                    "Жако", "Ара", "Амазона"
                },
                ["Заек"] = new[]
                {
                    "Холандски", "Лъвска глава",
                    "Мини лоп"
                },
                ["Хамстер"] = new[]
                {
                    "Сирийски", "Джунгарски",
                    "Роборовски"
                },
                ["Костенурка"] = new[]
                {
                    "Червеноуха", "Средиземноморска"
                }
            };

            foreach (var item in animalData)
            {
                var animalType = await context.AnimalTypes
                    .FirstOrDefaultAsync(t => t.Name == item.Key);

                if (animalType == null)
                {
                    animalType = new AnimalType
                    {
                        Name = item.Key
                    };

                    context.AnimalTypes.Add(animalType);
                    await context.SaveChangesAsync();
                }

                foreach (var breedName in item.Value)
                {
                    bool exists = await context.Breeds.AnyAsync(
                        b => b.AnimalTypeId == animalType.AnimalTypeId
                             && b.Name == breedName);

                    if (!exists)
                    {
                        context.Breeds.Add(new Breed
                        {
                            Name = breedName,
                            AnimalTypeId = animalType.AnimalTypeId
                        });
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
