using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swappy_V2.Models;
namespace Swappy_V2.Tests.Generators
{
    class DataGenerator
    {
        private string[] Names = {  "Alik", "Timur", "Kevin", "Vladislav", "Ruslan", "Robert", "Nikita", "Alexandr", "Damir" };
        private string[] Surnames = { "Khilazhev", "Handa", "Khazhiev", "Kurenkov", "Tushov", "Dolgushev", "Gazizov", "Mustafin", "Sakhapov" };
        private string[] Cities = { "Уфа", "Казань", "Иннополис", "Москва", "Ульяновск" };
        private string[] ItemTitles = { "Lumia 520", "iPhone 5", "Galaxy S5", "xBox 360", "Lumia 620",
                                          "LG G2", "PS 3", "Dell Inspirion n5110", "Lumia 920", "iPhone 3", "Fly N55", "Nokia 5s", "Srs", "Premium Sound" };

        private string[] Descriptions = { "Самы крутой__________________", "Ништяковый__________________", 
                                            "Классный такой__________________", "Недорогой__________________", "Быстрый__________________",
                                            "Современный__________________", "Нормальный__________________", "Top 1337__________________" };
        Random rand = new Random();
        public void Generate(int userCnt, int dealCntPerUser, int itemCntPerDeal, out List<AppUserModel> users, out List<DealModel> deals, out List<ItemModel> items)
        {
            users = new List<AppUserModel>();
            deals = new List<DealModel>();
            items = new List<ItemModel>();
            int itemId = 0;
            int dealId = 0;
            for(int i = 0;i < userCnt;i++)
            {
                var user = new AppUserModel()
                {
                    City = Cities[rand.Next(Cities.Length)],
                    Email = "sample@examp.le",
                    Id = i,
                    Name = Names[rand.Next(Names.Length)],
                    Surname = Surnames[rand.Next(Surnames.Length)],
                    PhoneNumber = GeneratePhoneNumber()
                };
                users.Add(user);
                for (int j = 0; j < dealCntPerUser; j++)
                {
                    var deal = new DealModel()
                    {
                        AnotherVariants = true,
                        Id = dealId++,
                        City = users[i].City,
                        AppUserId = i,
                    };
                    var itemes = new List<ItemModel>();
                    for(int k = 0;k < itemCntPerDeal;k++)
                    {
                        itemes.Add(new ItemModel() {
                            Title = ItemTitles[rand.Next(ItemTitles.Length)],
                            Description = Descriptions[rand.Next(Descriptions.Length)],
                            Id = itemId++,
                            ImageUrl = "",
                            DealModel = deal
                        });
                    }
                    
                    deal.Title = ItemTitles[rand.Next(ItemTitles.Length)];
                    deal.Description = Descriptions[rand.Next(Descriptions.Length)];
                    deal.Variants = itemes;
                    deals.Add(deal);
                }

            }

        }
        private string GeneratePhoneNumber()
        {
            return String.Format("+79{0}{1}{2}", rand.Next(100, 999), rand.Next(100, 999), rand.Next(100, 999));
        }
    }
}
