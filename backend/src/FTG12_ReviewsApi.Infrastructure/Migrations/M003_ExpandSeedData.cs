using FluentMigrator;

namespace FTG12_ReviewsApi.Infrastructure.Migrations;

/// <summary>
/// Expands seed data with 25 additional products and fills in all missing user-product review
/// combinations so that every non-admin user has exactly one review per product (87 total reviews).
/// This ensures pagination can be tested with pageSize=10.
/// </summary>
[Migration(3)]
public class M003_ExpandSeedData : Migration
{
    public override void Up()
    {
        var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        // ── 25 new products (IDs 5–29) ──────────────────────────────────────
        Insert.IntoTable("Products").Row(new { Name = "Dyson V15 Detect Vacuum" });
        Insert.IntoTable("Products").Row(new { Name = "Bosch Serie 6 Dishwasher" });
        Insert.IntoTable("Products").Row(new { Name = "Philips 3200 Espresso Machine" });
        Insert.IntoTable("Products").Row(new { Name = "iRobot Roomba j7+ Robot Vacuum" });
        Insert.IntoTable("Products").Row(new { Name = "KitchenAid Artisan Stand Mixer" });
        Insert.IntoTable("Products").Row(new { Name = "De'Longhi Magnifica Evo Coffee Machine" });
        Insert.IntoTable("Products").Row(new { Name = "Sony WH-1000XM5 Headphones" });
        Insert.IntoTable("Products").Row(new { Name = "Ninja Foodi Air Fryer" });
        Insert.IntoTable("Products").Row(new { Name = "Instant Pot Duo Plus" });
        Insert.IntoTable("Products").Row(new { Name = "Vitamix A3500 Blender" });
        Insert.IntoTable("Products").Row(new { Name = "Bose SoundLink Revolve+ Speaker" });
        Insert.IntoTable("Products").Row(new { Name = "Samsung QN90B Neo QLED TV" });
        Insert.IntoTable("Products").Row(new { Name = "LG C3 OLED TV" });
        Insert.IntoTable("Products").Row(new { Name = "Apple AirPods Pro 2" });
        Insert.IntoTable("Products").Row(new { Name = "Garmin Venu 3 Smartwatch" });
        Insert.IntoTable("Products").Row(new { Name = "Dyson Purifier Hot+Cool" });
        Insert.IntoTable("Products").Row(new { Name = "Shark Navigator Lift-Away Vacuum" });
        Insert.IntoTable("Products").Row(new { Name = "Cuisinart TOA-65 Air Fryer Toaster Oven" });
        Insert.IntoTable("Products").Row(new { Name = "Nespresso Vertuo Next Coffee Machine" });
        Insert.IntoTable("Products").Row(new { Name = "Weber Spirit II E-310 Gas Grill" });
        Insert.IntoTable("Products").Row(new { Name = "Philips Sonicare DiamondClean Toothbrush" });
        Insert.IntoTable("Products").Row(new { Name = "Ecovacs Deebot X2 Omni Robot Vacuum" });
        Insert.IntoTable("Products").Row(new { Name = "Tineco Floor One S5 Wet Dry Vacuum" });
        Insert.IntoTable("Products").Row(new { Name = "JBL Charge 5 Bluetooth Speaker" });
        Insert.IntoTable("Products").Row(new { Name = "Anker Soundcore Liberty 4 Earbuds" });

        // ── Missing reviews for original products (IDs 1–4) ─────────────────
        // M002 already has: U2→P1, U3→P1, U2→P2, U4→P2, U3→P3, U4→P3, U2→P4, U3→P4
        // Missing: U4→P1, U3→P2, U2→P3, U4→P4

        Insert.IntoTable("Reviews").Row(new { ProductId = 1, UserId = 4, StatusId = 2, Rating = 4, Text = "Solid fridge, plenty of room for a large family. The water dispenser works great.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 2, UserId = 3, StatusId = 2, Rating = 5, Text = "Incredibly quiet and efficient. Love the steam cycle for delicates.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 3, UserId = 2, StatusId = 2, Rating = 4, Text = "Great microwave for the price. Sensor cooking feature is very handy.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 4, UserId = 4, StatusId = 2, Rating = 5, Text = "Premium kettle that heats water fast. Variable temperature is perfect for tea.", CreatedAt = now });

        // ── Reviews for new products (IDs 5–29) from User1 (2), User2 (3), User3 (4) ──
        // Mix of statuses: mostly Approved (2), some Pending (1), a few Rejected (3)

        // Product 5 — Dyson V15 Detect Vacuum
        Insert.IntoTable("Reviews").Row(new { ProductId = 5, UserId = 2, StatusId = 2, Rating = 5, Text = "Best vacuum I have ever used. The laser dust detection is brilliant.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 5, UserId = 3, StatusId = 2, Rating = 4, Text = "Powerful suction and lightweight. Battery life could be a bit better.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 5, UserId = 4, StatusId = 2, Rating = 5, Text = "Amazing technology. Shows you exactly how much dust you are picking up.", CreatedAt = now });

        // Product 6 — Bosch Serie 6 Dishwasher
        Insert.IntoTable("Reviews").Row(new { ProductId = 6, UserId = 2, StatusId = 2, Rating = 4, Text = "Very quiet and cleans dishes perfectly. Third rack is super useful.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 6, UserId = 3, StatusId = 1, Rating = 3, Text = "Good dishwasher but the drying could be more effective.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 6, UserId = 4, StatusId = 2, Rating = 4, Text = "Reliable and energy efficient. Installation was straightforward.", CreatedAt = now });

        // Product 7 — Philips 3200 Espresso Machine
        Insert.IntoTable("Reviews").Row(new { ProductId = 7, UserId = 2, StatusId = 2, Rating = 5, Text = "Makes cafe quality espresso at home. The milk frother is excellent.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 7, UserId = 3, StatusId = 2, Rating = 4, Text = "Great espresso machine. Easy to clean and maintain.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 7, UserId = 4, StatusId = 3, Rating = 2, Text = "Machine broke down after three months. Customer support was unhelpful.", CreatedAt = now });

        // Product 8 — iRobot Roomba j7+
        Insert.IntoTable("Reviews").Row(new { ProductId = 8, UserId = 2, StatusId = 2, Rating = 4, Text = "Smart obstacle avoidance works well. Self-emptying base is convenient.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 8, UserId = 3, StatusId = 2, Rating = 5, Text = "Set it and forget it. Keeps our floors clean with minimal effort.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 8, UserId = 4, StatusId = 2, Rating = 4, Text = "Good robot vacuum for pet owners. Handles pet hair without tangling.", CreatedAt = now });

        // Product 9 — KitchenAid Artisan Stand Mixer
        Insert.IntoTable("Reviews").Row(new { ProductId = 9, UserId = 2, StatusId = 2, Rating = 5, Text = "A kitchen essential. Makes baking so much easier and more enjoyable.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 9, UserId = 3, StatusId = 2, Rating = 5, Text = "Built like a tank. Have had mine for years and it still runs perfectly.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 9, UserId = 4, StatusId = 1, Rating = 4, Text = "Heavy but powerful. The attachments expand its versatility significantly.", CreatedAt = now });

        // Product 10 — De'Longhi Magnifica Evo
        Insert.IntoTable("Reviews").Row(new { ProductId = 10, UserId = 2, StatusId = 2, Rating = 4, Text = "Consistent coffee quality. The bean-to-cup process is very convenient.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 10, UserId = 3, StatusId = 2, Rating = 3, Text = "Decent machine but the water tank is too small for daily use.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 10, UserId = 4, StatusId = 2, Rating = 5, Text = "Perfect morning coffee every day. My favorite kitchen appliance.", CreatedAt = now });

        // Product 11 — Sony WH-1000XM5 Headphones
        Insert.IntoTable("Reviews").Row(new { ProductId = 11, UserId = 2, StatusId = 2, Rating = 5, Text = "Industry-leading noise cancellation. Incredibly comfortable for long sessions.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 11, UserId = 3, StatusId = 2, Rating = 4, Text = "Great sound quality. Wish they still folded flat like the XM4.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 11, UserId = 4, StatusId = 2, Rating = 5, Text = "Worth every penny. The multipoint connection is seamless.", CreatedAt = now });

        // Product 12 — Ninja Foodi Air Fryer
        Insert.IntoTable("Reviews").Row(new { ProductId = 12, UserId = 2, StatusId = 2, Rating = 4, Text = "Crispy results without the oil. Easy to use and clean.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 12, UserId = 3, StatusId = 3, Rating = 2, Text = "Basket coating started peeling after a few months. Disappointed.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 12, UserId = 4, StatusId = 2, Rating = 4, Text = "Great for quick meals. The dual zone feature is really useful.", CreatedAt = now });

        // Product 13 — Instant Pot Duo Plus
        Insert.IntoTable("Reviews").Row(new { ProductId = 13, UserId = 2, StatusId = 2, Rating = 5, Text = "Replaced multiple kitchen appliances. The pressure cooking is a game changer.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 13, UserId = 3, StatusId = 2, Rating = 4, Text = "Very versatile. Slow cook, pressure cook, saute — all in one.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 13, UserId = 4, StatusId = 2, Rating = 5, Text = "Best kitchen purchase I have made. Perfect for busy weeknights.", CreatedAt = now });

        // Product 14 — Vitamix A3500 Blender
        Insert.IntoTable("Reviews").Row(new { ProductId = 14, UserId = 2, StatusId = 2, Rating = 5, Text = "Smoothest smoothies ever. Can even make hot soup from raw ingredients.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 14, UserId = 3, StatusId = 2, Rating = 4, Text = "Powerful but loud. The preset programs are very convenient.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 14, UserId = 4, StatusId = 1, Rating = 5, Text = "Professional grade blender. Worth the premium price for daily use.", CreatedAt = now });

        // Product 15 — Bose SoundLink Revolve+ Speaker
        Insert.IntoTable("Reviews").Row(new { ProductId = 15, UserId = 2, StatusId = 2, Rating = 4, Text = "360-degree sound is impressive. Great for outdoor gatherings.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 15, UserId = 3, StatusId = 2, Rating = 5, Text = "Rich and balanced sound. Battery lasts all day.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 15, UserId = 4, StatusId = 2, Rating = 4, Text = "Solid build quality and excellent sound. A bit heavy to carry around.", CreatedAt = now });

        // Product 16 — Samsung QN90B Neo QLED TV
        Insert.IntoTable("Reviews").Row(new { ProductId = 16, UserId = 2, StatusId = 2, Rating = 5, Text = "Stunning picture quality. Mini LED technology makes a huge difference.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 16, UserId = 3, StatusId = 2, Rating = 4, Text = "Great TV for a bright room. Anti-reflective coating works well.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 16, UserId = 4, StatusId = 2, Rating = 5, Text = "Perfect for gaming. Low input lag and VRR support are excellent.", CreatedAt = now });

        // Product 17 — LG C3 OLED TV
        Insert.IntoTable("Reviews").Row(new { ProductId = 17, UserId = 2, StatusId = 2, Rating = 5, Text = "OLED blacks are unmatched. Movie nights have never been better.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 17, UserId = 3, StatusId = 1, Rating = 4, Text = "Incredible picture but concerned about burn-in over time.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 17, UserId = 4, StatusId = 2, Rating = 5, Text = "Best TV on the market. webOS is smooth and intuitive.", CreatedAt = now });

        // Product 18 — Apple AirPods Pro 2
        Insert.IntoTable("Reviews").Row(new { ProductId = 18, UserId = 2, StatusId = 2, Rating = 4, Text = "Great ANC and transparency mode. Spatial audio is fun for movies.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 18, UserId = 3, StatusId = 2, Rating = 5, Text = "Seamless Apple ecosystem integration. Best earbuds for iPhone users.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 18, UserId = 4, StatusId = 2, Rating = 4, Text = "Comfortable fit and good sound. USB-C charging is a welcome addition.", CreatedAt = now });

        // Product 19 — Garmin Venu 3 Smartwatch
        Insert.IntoTable("Reviews").Row(new { ProductId = 19, UserId = 2, StatusId = 2, Rating = 4, Text = "Excellent fitness tracking. Battery lasts much longer than Apple Watch.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 19, UserId = 3, StatusId = 2, Rating = 5, Text = "Best smartwatch for runners. GPS accuracy is top notch.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 19, UserId = 4, StatusId = 3, Rating = 3, Text = "Good hardware but the app ecosystem is limited compared to competitors.", CreatedAt = now });

        // Product 20 — Dyson Purifier Hot+Cool
        Insert.IntoTable("Reviews").Row(new { ProductId = 20, UserId = 2, StatusId = 2, Rating = 4, Text = "Three-in-one device works well. Air quality display is informative.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 20, UserId = 3, StatusId = 2, Rating = 3, Text = "Works as advertised but very expensive for what it does.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 20, UserId = 4, StatusId = 2, Rating = 5, Text = "Helped significantly with my allergies. Quiet mode is actually quiet.", CreatedAt = now });

        // Product 21 — Shark Navigator Lift-Away Vacuum
        Insert.IntoTable("Reviews").Row(new { ProductId = 21, UserId = 2, StatusId = 2, Rating = 4, Text = "Excellent suction power. The lift-away feature is great for stairs.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 21, UserId = 3, StatusId = 2, Rating = 4, Text = "Good value vacuum. Heavy but gets the job done well.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 21, UserId = 4, StatusId = 1, Rating = 3, Text = "Decent vacuum but the cord could be longer. Suction is strong though.", CreatedAt = now });

        // Product 22 — Cuisinart TOA-65 Air Fryer Toaster Oven
        Insert.IntoTable("Reviews").Row(new { ProductId = 22, UserId = 2, StatusId = 2, Rating = 5, Text = "Replaced our toaster and air fryer. Saves counter space and works great.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 22, UserId = 3, StatusId = 2, Rating = 4, Text = "Versatile appliance. Toast, bake, broil, and air fry all in one.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 22, UserId = 4, StatusId = 2, Rating = 4, Text = "Cooks evenly and quickly. Interior is easy to wipe clean.", CreatedAt = now });

        // Product 23 — Nespresso Vertuo Next
        Insert.IntoTable("Reviews").Row(new { ProductId = 23, UserId = 2, StatusId = 2, Rating = 3, Text = "Good coffee but the proprietary pods are expensive over time.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 23, UserId = 3, StatusId = 3, Rating = 2, Text = "Machine stopped working after six months. Build quality is poor.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 23, UserId = 4, StatusId = 2, Rating = 4, Text = "Convenient and fast. Perfect for a quick morning coffee.", CreatedAt = now });

        // Product 24 — Weber Spirit II E-310 Gas Grill
        Insert.IntoTable("Reviews").Row(new { ProductId = 24, UserId = 2, StatusId = 2, Rating = 5, Text = "Excellent grill for the price. Even heat distribution across the grates.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 24, UserId = 3, StatusId = 2, Rating = 4, Text = "Sturdy construction and easy to assemble. Grills like a dream.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 24, UserId = 4, StatusId = 2, Rating = 5, Text = "Weekend BBQs have never been better. Great sear on steaks.", CreatedAt = now });

        // Product 25 — Philips Sonicare DiamondClean Toothbrush
        Insert.IntoTable("Reviews").Row(new { ProductId = 25, UserId = 2, StatusId = 2, Rating = 4, Text = "Teeth feel dentist-clean every time. Battery lasts two weeks.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 25, UserId = 3, StatusId = 2, Rating = 5, Text = "Best electric toothbrush I have tried. The app tracking is a bonus.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 25, UserId = 4, StatusId = 2, Rating = 4, Text = "Great cleaning power. The pressure sensor protects your gums.", CreatedAt = now });

        // Product 26 — Ecovacs Deebot X2 Omni
        Insert.IntoTable("Reviews").Row(new { ProductId = 26, UserId = 2, StatusId = 2, Rating = 4, Text = "Mops and vacuums at the same time. The docking station does everything.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 26, UserId = 3, StatusId = 1, Rating = 4, Text = "Good robot vacuum and mop combo. Navigation is very accurate.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 26, UserId = 4, StatusId = 2, Rating = 5, Text = "Hands-free floor cleaning. The square design reaches corners better.", CreatedAt = now });

        // Product 27 — Tineco Floor One S5
        Insert.IntoTable("Reviews").Row(new { ProductId = 27, UserId = 2, StatusId = 2, Rating = 5, Text = "Vacuums and washes floors simultaneously. Game changer for hard floors.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 27, UserId = 3, StatusId = 2, Rating = 4, Text = "Effective wet-dry cleaning. Self-cleaning function saves time.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 27, UserId = 4, StatusId = 2, Rating = 4, Text = "Works well on tile and hardwood. Not great on carpets though.", CreatedAt = now });

        // Product 28 — JBL Charge 5
        Insert.IntoTable("Reviews").Row(new { ProductId = 28, UserId = 2, StatusId = 2, Rating = 4, Text = "Loud and clear sound with deep bass. IP67 waterproof is a plus.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 28, UserId = 3, StatusId = 2, Rating = 5, Text = "Perfect pool party speaker. Battery lasts over 20 hours.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 28, UserId = 4, StatusId = 1, Rating = 4, Text = "Great portable speaker. The power bank feature is handy on trips.", CreatedAt = now });

        // Product 29 — Anker Soundcore Liberty 4
        Insert.IntoTable("Reviews").Row(new { ProductId = 29, UserId = 2, StatusId = 2, Rating = 4, Text = "Excellent value earbuds. Sound quality rivals brands costing twice as much.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 29, UserId = 3, StatusId = 2, Rating = 4, Text = "Comfortable fit for all-day wear. ANC is surprisingly effective.", CreatedAt = now });
        Insert.IntoTable("Reviews").Row(new { ProductId = 29, UserId = 4, StatusId = 2, Rating = 5, Text = "Best budget earbuds available. The spatial audio feature is impressive.", CreatedAt = now });
    }

    public override void Down()
    {
        // Remove reviews added by this migration (IDs 9–87)
        for (var i = 87; i >= 9; i--)
        {
            Delete.FromTable("Reviews").Row(new { Id = i });
        }

        // Remove new products (IDs 5–29)
        for (var i = 29; i >= 5; i--)
        {
            Delete.FromTable("Products").Row(new { Id = i });
        }
    }
}
