using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FridgeStoreApp
{
    [Serializable]
    public class Check
    {
        public string Costumer { get; set; }
        public string Seller { get; set; }
        public DateTime Date { get; set; }
        public List<Product> Products { get; set; }

        public Check() { }

        public Check(List<Product> products)
        {
            Products = products;
        }

        public void PrintProducts()
        {
            if (Products != null)
            {
                foreach(Product item in Products)
                { 
                    Console.WriteLine("Товар: {0}, Цена: {1}", item.Name , item.Price);
                }                
            }
            else
            {
                Console.WriteLine("Нет продуктов!!!");
            }
        }
    }
}
