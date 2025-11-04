using Microsoft.EntityFrameworkCore;
using Shop_app.Models;

/*
 INSERT INTO Products (Name, Price, Description) VALUES ('Apple iPhone 14', 799.99, 'The latest iPhone model with 5G capability, A15 Bionic chip, and improved camera system.');
INSERT INTO Products (Name, Price, Description) VALUES ('Samsung Galaxy S23', 999.99, 'Flagship smartphone from Samsung featuring a dynamic AMOLED display and top-notch performance.');
INSERT INTO Products (Name, Price, Description) VALUES ('Sony WH-1000XM5', 399.99, 'Industry-leading noise-canceling wireless headphones with premium sound quality.');
INSERT INTO Products (Name, Price, Description) VALUES ('Dell XPS 13 Laptop', 1199.99, '13-inch ultra-thin laptop with InfinityEdge display and powerful Intel i7 processor.');
INSERT INTO Products (Name, Price, Description) VALUES ('Apple MacBook Pro 16"', 2499.99, 'High-performance laptop with M1 Pro chip, Retina display, and long battery life.');
INSERT INTO Products (Name, Price, Description) VALUES ('Google Pixel 7', 599.99, 'The latest Google phone with an advanced AI-powered camera and seamless integration with Google services.');
INSERT INTO Products (Name, Price, Description) VALUES ('Sony PlayStation 5', 499.99, 'Next-generation gaming console with 4K gaming and ultra-fast SSD for load times.');
INSERT INTO Products (Name, Price, Description) VALUES ('Microsoft Xbox Series X', 499.99, 'Powerful gaming console with 12 teraflops of processing power and 4K gameplay.');
INSERT INTO Products (Name, Price, Description) VALUES ('Bose QuietComfort Earbuds II', 299.99, 'Premium true wireless earbuds with advanced noise-canceling technology and crisp sound.');
INSERT INTO Products (Name, Price, Description) VALUES ('Apple AirPods Pro 2', 249.99, 'Second-generation wireless earbuds with active noise cancellation and transparency mode.');
INSERT INTO Products (Name, Price, Description) VALUES ('Fitbit Charge 5', 179.99, 'Advanced health and fitness tracker with built-in GPS, heart rate monitor, and stress tracking.');
INSERT INTO Products (Name, Price, Description) VALUES ('Nikon Z7 II Camera', 2999.99, 'Full-frame mirrorless camera with 45.7 MP resolution, dual processors, and 4K video recording.');
INSERT INTO Products (Name, Price, Description) VALUES ('Canon EOS R6', 2499.99, 'Mirrorless camera with 20 MP resolution, advanced autofocus system, and 4K video support.');
INSERT INTO Products (Name, Price, Description) VALUES ('Dyson V15 Detect Vacuum', 749.99, 'Powerful cordless vacuum cleaner with laser dust detection and advanced filtration system.');
INSERT INTO Products (Name, Price, Description) VALUES ('Instant Pot Duo 7-in-1', 99.99, 'Multifunctional pressure cooker with seven cooking modes, including slow cooking and steaming.');
INSERT INTO Products (Name, Price, Description) VALUES ('KitchenAid Stand Mixer', 499.99, 'Classic stand mixer with 10 speeds and durable build, perfect for baking and cooking tasks.');
INSERT INTO Products (Name, Price, Description) VALUES ('GoPro HERO11 Black', 499.99, 'Waterproof action camera with 5.3K video recording, 27 MP photos, and HyperSmooth stabilization.');
INSERT INTO Products (Name, Price, Description) VALUES ('Nintendo Switch OLED', 349.99, 'Hybrid gaming console with a 7-inch OLED screen, detachable controllers, and extensive game library.');
INSERT INTO Products (Name, Price, Description) VALUES ('Logitech MX Master 3', 99.99, 'Wireless ergonomic mouse with precision scrolling, customizable buttons, and multi-device support.');
INSERT INTO Products (Name, Price, Description) VALUES ('Razer DeathAdder V2', 69.99, 'High-precision gaming mouse with 20K DPI sensor, optical switches, and ergonomic design.');
 */

namespace Shop_app.Services
{
    public interface IServiceProducts
    {
        Task<Product?> CreateAsync(Product? product); // Асинхронний метод створення продукту
        Task<IEnumerable<Product>> ReadAsync(); // Асинхронний метод отримання всіх продуктів
        Task<Product?> GetByIdAsync(int id); // Асинхронний метод отримання продукту з його ID
        Task<Product?> UpdateAsync(int id, Product? product); // Асинхронний метод оновлення продукту
        Task<bool> DeleteAsync(int id); // Асинхронний метод видалення продукту по ID
    }

    public class ServiceProducts : IServiceProducts
    {
        private readonly ShopContext _shopContext; // Хранит контекст базы данных для работы с продуктами
        private readonly ILogger<ServiceProducts> _logger; // Логгер для записи событий и ошибок

        // Конструктор класса, который принимает контекст и логгер через внедрение зависимостей
        public ServiceProducts(ShopContext productContext, ILogger<ServiceProducts> logger)
        {
            _shopContext = productContext; // Инициализация контекста базы данных
            _logger = logger; // Инициализация логгера
        }

        // Метод для создания нового продукта
        public async Task<Product?> CreateAsync(Product? product)
        {
            // Проверка, является ли продукт нулевым
            if (product == null)
            {
                _logger.LogWarning("Попытка создать продукт с нулевым значением."); // Логирование предупреждения
                return null; // Возврат нуля, если продукт нулевой
            }

            // Добавление продукта в контекст базы данных
            await _shopContext.Products.AddAsync(product);
            // Сохранение изменений в базе данных
            await _shopContext.SaveChangesAsync();
            return product; // Возврат созданного продукта
        }

        // Метод для удаления продукта по его ID
        public async Task<bool> DeleteAsync(int id)
        {
            // Поиск продукта в базе данных по его ID
            var product = await _shopContext.Products.FindAsync(id);
            // Если продукт не найден, вернуть false
            if (product == null)
            {
                return false;
            }

            // Удаление продукта из контекста базы данных
            _shopContext.Products.Remove(product);
            // Сохранение изменений в базе данных
            await _shopContext.SaveChangesAsync();
            return true; // Возврат true, если продукт успешно удален
        }

        // Метод для получения продукта по его ID
        public async Task<Product?> GetByIdAsync(int id)
        {
            // Поиск продукта в базе данных по его ID
            return await _shopContext.Products.FindAsync(id);
        }

        // Метод для получения всех продуктов из базы данных
        public async Task<IEnumerable<Product>> ReadAsync()
        {
            // Возврат списка всех продуктов
            return await _shopContext.Products.ToListAsync();
        }

        // Метод для обновления существующего продукта
        public async Task<Product?> UpdateAsync(int id, Product? product)
        {
            // Проверка, является ли продукт нулевым или идентификатор не совпадает
            if (product == null || id != product.Id)
            {
                _logger.LogWarning($"Несоответствие идентификатора продукта. Ожидался {id}, получен {product?.Id}."); // Логирование предупреждения
                return null; // Возврат нуля, если есть несоответствие
            }

            try
            {
                // Обновление продукта в контексте базы данных
                _shopContext.Products.Update(product);
                // Сохранение изменений в базе данных
                await _shopContext.SaveChangesAsync();
                return product; // Возврат обновленного продукта
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Логирование ошибки при обновлении продукта
                _logger.LogError(ex, "Ошибка при обновлении продукта с идентификатором {Id}.", id);
                return null; // Возврат нуля в случае ошибки
            }
        }

    }
}
