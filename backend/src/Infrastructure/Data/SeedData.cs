using MealCycle.Domain.MealPlans;
using MealCycle.Domain.Recipes;

namespace MealCycle.Infrastructure.Data;

internal static class SeedData
{
    public static IReadOnlyList<Recipe> Recipes { get; } =
    [
        new Recipe(
            Guid.Parse("3e9e6ba9-7199-4f4d-a2f3-7388f699a531"),
            "Weeknight Chili",
            [
                new RecipeIngredient("Ground turkey", "500g"),
                new RecipeIngredient("Black beans", "2 cans"),
                new RecipeIngredient("Crushed tomatoes", "1 can"),
                new RecipeIngredient("Onion", "1 diced")
            ],
            [
                "Brown turkey with diced onion.",
                "Add beans and tomatoes.",
                "Simmer for 20 minutes and season to taste."
            ]),
        new Recipe(
            Guid.Parse("80e3f6ef-f229-4e02-a2b1-f6be777ed6f4"),
            "Lemon Chickpea Pasta",
            [
                new RecipeIngredient("Pasta", "400g"),
                new RecipeIngredient("Chickpeas", "1 can"),
                new RecipeIngredient("Lemon", "1 zested and juiced"),
                new RecipeIngredient("Garlic", "2 cloves")
            ],
            [
                "Cook pasta until al dente.",
                "Saute garlic and chickpeas in olive oil.",
                "Toss pasta with lemon juice, zest, and chickpeas."
            ]),
        new Recipe(
            Guid.Parse("e4b9ecce-af6e-435d-a89f-a703c09157b9"),
            "Crispy Tofu Rice Bowls",
            [
                new RecipeIngredient("Firm tofu", "400g"),
                new RecipeIngredient("Cooked rice", "3 cups"),
                new RecipeIngredient("Broccoli", "1 head"),
                new RecipeIngredient("Soy sauce", "3 tbsp")
            ],
            [
                "Press and cube tofu, then pan-fry until crisp.",
                "Steam broccoli until bright green.",
                "Serve tofu and broccoli over rice with soy sauce."
            ]),
        new Recipe(
            Guid.Parse("4cbac8df-cce0-4d8f-9072-681f4e6fbe59"),
            "Baked Salmon and Potatoes",
            [
                new RecipeIngredient("Salmon fillets", "4"),
                new RecipeIngredient("Baby potatoes", "700g"),
                new RecipeIngredient("Olive oil", "2 tbsp"),
                new RecipeIngredient("Dill", "1 tbsp")
            ],
            [
                "Roast potatoes until nearly tender.",
                "Season salmon and place on tray with potatoes.",
                "Bake until salmon flakes and finish with dill."
            ])
    ];

    public static IReadOnlyList<MealPlanItem> MealPlanItems { get; } =
    [
        new MealPlanItem(
            Guid.Parse("2de8e48e-8df2-4d28-abf7-bd765b2c4b2d"),
            Guid.Parse("3e9e6ba9-7199-4f4d-a2f3-7388f699a531"),
            "Monday",
            "Weeknight Chili",
            0),
        new MealPlanItem(
            Guid.Parse("f1783191-cba6-4f3f-aec7-d5fd5d4dc2cc"),
            Guid.Parse("80e3f6ef-f229-4e02-a2b1-f6be777ed6f4"),
            "Monday",
            "Lemon Chickpea Pasta",
            1),
        new MealPlanItem(
            Guid.Parse("73f6824d-b2b6-4bc0-b16c-765cdf3e0706"),
            Guid.Parse("e4b9ecce-af6e-435d-a89f-a703c09157b9"),
            "Tuesday",
            "Crispy Tofu Rice Bowls",
            0),
        new MealPlanItem(
            Guid.Parse("0a61b1f9-0ea1-45ca-bfaa-f122cfc08fce"),
            Guid.Parse("4cbac8df-cce0-4d8f-9072-681f4e6fbe59"),
            "Wednesday",
            "Baked Salmon and Potatoes",
            0)
    ];
}
