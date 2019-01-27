using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Xml.Serialization;


namespace FridgeStoreApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable sellers = new DataTable("seller");
            DataColumn pKeyCol = sellers.Columns.Add("sellerId", typeof(int));
            pKeyCol.AutoIncrement = true;
            pKeyCol.AutoIncrementStep = 1;
            pKeyCol.Unique = true;
            sellers.Columns.Add("name", typeof(string));
            sellers.Columns.Add("surname", typeof(string));

            DataTable products = new DataTable("product");
            pKeyCol = products.Columns.Add("productId", typeof(int));
            pKeyCol.AutoIncrement = true;
            pKeyCol.AutoIncrementStep = 1;
            pKeyCol.Unique = true;
            products.Columns.Add("name", typeof(string));
            products.Columns.Add("price", typeof(int));

            DataTable costumers = new DataTable("costumer");
            pKeyCol = costumers.Columns.Add("costId", typeof(int));
            pKeyCol.AutoIncrement = true;
            pKeyCol.AutoIncrementStep = 1;
            pKeyCol.Unique = true;
            costumers.Columns.Add("name", typeof(string));
            costumers.Columns.Add("surname", typeof(string));

            DataSet fridgeStore = new DataSet();
            fridgeStore.Tables.Add(sellers);
            fridgeStore.Tables.Add(products);
            fridgeStore.Tables.Add(costumers);

            fridgeStore.ReadXml("fridgeStore.xml");

            Check check = new Check(new List<Product>());

            while (true)
            {
                Console.WriteLine("1 - Выбрать продавца");
                Console.WriteLine("2 - Выбрать покупателя");
                Console.WriteLine("3 - Добавить товар");
                Console.WriteLine("4 - Сохранить чек");
                Console.WriteLine("5 - Сбросить");
                Console.WriteLine("6 - Exit");

                int key;
                int id;

                if (int.TryParse(Console.ReadLine(), out key))
                {
                    switch (key)
                    {
                        case 1:
                            if (check.Seller == null)
                            {
                                PrintValues(sellers, "Выберите продавца по ID:");
                                if (int.TryParse(Console.ReadLine(), out id))
                                {
                                    DataRow row = SelectItem(sellers, id);
                                    if (row != null)
                                    {
                                        check.Seller = row["name"] + " " + row["surname"];
                                        Console.WriteLine("Данные сохранены!!!");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Введен не верный ID!!!");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Ошибка ввода!!!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Продавец уже выбран: {0}", check.Seller);
                            }
                            break;
                        case 2:
                            if (check.Costumer == null)
                            {
                                SelectMenuCostumer(costumers, check);
                            }
                            else
                            {
                                Console.WriteLine("Покупатель уже выбран: {0}", check.Costumer);
                            }
                            break;
                        case 3:
                            Console.WriteLine("Выбранные товары: ");
                            check.PrintProducts();
                            PrintValues(products, "Выберите товар по ID:");
                            if (int.TryParse(Console.ReadLine(), out id))
                            {
                                DataRow row = SelectItem(products, id);
                               
                                if (row != null)
                                {
                                    check.Products.Add(new Product(row["name"].ToString(), 
                                        int.Parse(row["price"].ToString())));
                                    Console.WriteLine("Товар сохранен!!!");
                                }
                                else
                                {
                                    Console.WriteLine("Введен не верный ID!!!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Ошибка ввода!!!");
                            }
                            break;
                        case 4:
                            check.Date = DateTime.Now;
                            Console.Write("Введите путь где сохранить данные: ");
                            string path = Console.ReadLine();
                            try
                            {
                                SaveCheck(check, path);
                                Console.WriteLine("-----------------------------");
                                PrintCheckFromFile(path);
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            break;
                        case 5: check.Seller = null;
                            check.Costumer = null;
                            check.Products.Clear();
                            Console.WriteLine("Сброс произведен!!!");                        
                            break;
                        case 6:
                            fridgeStore.WriteXml("fridgeStore.xml");
                            return;
                        default: break;
                    }
                }
                Console.ReadLine();
                Console.Clear();
            }
        }
        
        static void PrintCheckFromFile(string path )
        {
            using (StreamReader sReader = new StreamReader(path))
            {
                while (!sReader.EndOfStream)
                {
                    Console.WriteLine(sReader.ReadLine());
                }
            }
        }

        static void SaveCheck(Check check, string path)
        {
            XmlSerializer sz = new XmlSerializer(typeof(Check));

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate|FileMode.Truncate))
            {
                sz.Serialize(fs, check);
                Console.WriteLine("Чек сохранен!!!");
            }
        }

        static void SelectMenuCostumer(DataTable costumers, Check check)
        {
            Console.WriteLine("1 - Выбрать покупателя из списка");
            Console.WriteLine("2 - Ввести данные нового покупателя");

            int key;

            if (int.TryParse(Console.ReadLine(), out key))
            {
                switch (key)
                {
                    case 1:
                        Console.Clear();
                        PrintValues(costumers, "Выберите покупателя по ID:");
                        int id;
                        if (int.TryParse(Console.ReadLine(), out id))
                        {
                            DataRow row = SelectItem(costumers, id);
                            if (row != null)
                            {
                                check.Costumer = row["name"] + " " + row["surname"];
                                Console.WriteLine("Данные сохранены!!!");
                            }
                            else
                            {
                                Console.WriteLine("Введен не верный ID!!!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ошибка ввода!!!");
                        }
                        break;
                    case 2:
                        Console.Write("Введите имя: ");
                        DataRow newRow = costumers.NewRow();
                        newRow["name"] = Console.ReadLine();
                        Console.Write("Введите фамилию: ");
                        newRow["surname"] = Console.ReadLine();
                        check.Costumer = newRow["name"] + " " + newRow["surname"];
                        costumers.Rows.Add(newRow);
                        Console.WriteLine("Данные сохранены!!!");
                        break;
                    default:break;
                }
            }
        }

        static DataRow SelectItem(DataTable table, int rowId)
        {
            DataRow[] rows = table.Select($"{table.Columns[0]} = {rowId}");

            if (rows.Length == 1)
            {
                return rows[0];
            }

            return null;
        }

        static void PrintValues(DataTable table, string tbName)
        {
            Console.WriteLine(tbName);

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    Console.Write("{0}: {1},  ",column.ColumnName, row[column]);
                }
                Console.WriteLine();
            }
        }
    }
}
