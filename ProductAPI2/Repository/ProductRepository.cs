using ProductAPI.Context;
using ProductAPI.Models;
using ProductAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext context;

        public ProductRepository(ApplicationDBContext context)
        {
            this.context = context;
        }
        public bool CreateProduct(Product product)
        {
            context.Products.Add(product);
            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            context.Products.Remove(product);
            return Save();
        }

        public IEnumerable<Product> GetAllProduct()
        {
            return context.Products.ToList();
        }

        public Product GetProduct(int id)
        {
            return context.Products.FirstOrDefault(x => x.Id == id);
        }

        public bool ProductExist(string name)
        {
            return context.Products.Any(x => x.Name.ToLower().Trim().Equals(name.ToLower().Trim()));
        }

        public bool ProductExist(int id)
        {
            return context.Products.Any(x => x.Id == id);
        }

        public bool Save()
        {
            return context.SaveChanges() > 0 ? true : false;
        }

        public bool UpdateProduct(Product product)
        {
            context.Products.Update(product);
            return Save();
        }
    }
}
