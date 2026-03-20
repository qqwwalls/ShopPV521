using Shop.App.Services;

namespace Shop.App
{
    public enum MenuOption
    {
        ShowAll = 1,
        Add,
        UpdatePrice,
        Delete,
        Exit
    }

    public class ShopManager
    {
        private readonly ProductService _productService;

        public ShopManager(ProductService productService)
        {
            _productService = productService;
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("\n--- MENU ---");
                Console.WriteLine("1. Show All\n2. Add Product\n3. Update Price\n4. Delete\n5. Exit");
                Console.Write("Choice: ");

                if (!Enum.TryParse(Console.ReadLine(), out MenuOption choice)) continue;
                if (choice == MenuOption.Exit) break;

                Execute(choice);
            }
        }

        private void Execute(MenuOption choice)
        {
            switch (choice)
            {
                case MenuOption.ShowAll:
                    _productService.ShowAllProducts();
                    break;
                case MenuOption.Add:
                    Console.Write("Name: ");
                    string name = Console.ReadLine() ?? "New Item";
                    _productService.AddNewProduct(name, 50.00m, 10);
                    break;
                case MenuOption.UpdatePrice:
                    Console.Write("ID: ");
                    int id = int.Parse(Console.ReadLine() ?? "0");
                    _productService.ChangePrice(id, 75.00m);
                    break;
                case MenuOption.Delete:
                    Console.Write("ID: ");
                    int delId = int.Parse(Console.ReadLine() ?? "0");
                    _productService.RemoveProduct(delId);
                    break;
            }
        }
    }
}