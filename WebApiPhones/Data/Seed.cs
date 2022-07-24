using Entities.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.MsSql;
using PhoneNumbers;
namespace WebApiPhones.Data
{
    public static class DbSetExtension
    {
        public static void ClearSave<T>(this DbSet<T> dbSet, PhonesDBContext dataContext) where T : class
        {
            if (dbSet.Any())
            {
                dbSet.RemoveRange(dbSet.ToList());
                dataContext.SaveChanges();
            }
        }
    }
    public class Seed
    {
        /// <summary>
        /// Хранит ссылку на генератор случайных чисел.
        /// </summary>
        public static readonly Random random = new();
        private static readonly PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
        private static PhoneNumber? GetRandomPhoneNumber() =>
            phoneUtil.Parse("+7" + new string(Enumerable.Range(2, random.Next(2, 11)).Select(x => (char)random.Next('0', '9' + 1)).ToArray()), "ru");
        private static Phone GetRandomPhone() => new() { Name = GetRandomString(5), PhoneNumder = GetRandomPhoneNumber() };

        /// <summary>
        /// Генерирует случайную строку из латинских букв нижнего регистра.
        /// </summary>
        /// <param name="length">Длина строки.</param>
        /// <returns></returns>
        private static string GetRandomString(int length) => new(Enumerable.Range(0, length).Select(x => (char)random.Next('a', 'z' + 1)).ToArray());
        private static List<Phone> GetRandomPhones(int v) => new(Enumerable.Range(0, v).Select(index => GetRandomPhone()).ToList());
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new PhonesDBContext(serviceProvider.GetRequiredService<DbContextOptions<PhonesDBContext>>());
            if (context == null) return;
            //var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            //context.Database.Migrate();
            //SeedUserDataAsync(userManager).Wait();
            // Чистим таблицы.
            context.Phones.ClearSave(context);
            // Заполняем таблицы случайными полями.
            context.Phones.AddRange(GetRandomPhones(5));
            // Сохраняем таблицы в базе.
            context.SaveChanges();
        }

        //public static async Task SeedUserDataAsync(UserManager<IdentityUser> userManager)
        //{
        //    if (!userManager.Users.Any())
        //    {
        //        var users = new List<IdentityUser>
        //                        {
        //                            new IdentityUser
        //                                {
        //                                    //Alias = "TestUserFirst",
        //                                    UserName = "TestUserFirst",
        //                                    Email = "testuserfirst@test.com"
        //                                },

        //                            new IdentityUser
        //                                {
        //                                    //Alias = "TestUserSecond",
        //                                    UserName = "TestUserSecond",
        //                                    Email = "testusersecond@test.com"
        //                                }
        //                        };

        //        foreach (var user in users)
        //        {
        //            await userManager.CreateAsync(user, "qazwsX123@");
        //        }
        //    }
        //}
    }
}
