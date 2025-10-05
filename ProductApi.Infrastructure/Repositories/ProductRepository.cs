using System.Linq.Expressions;
using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                var product = await GetByAsync(p => p.Name == entity.Name);
                if (product is not null && !string.IsNullOrEmpty(product.Name))
                {
                    return new Response(false, $"This product {entity.Name} alreay added");
                }
                var currentProduct = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currentProduct is not null && currentProduct.Id > 0)
                {
                    return new Response(true, $"This product {entity.Name} has been created.");
                }else
                return new Response(false, "Error ocuured while adding new product");

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error ocuured while adding new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product is null)
                {
                    return new Response(false, $"This product {entity.Name} is not found");

                }
                context.Products.Remove(entity);
                await context.SaveChangesAsync();
                return new Response(true,$"This product {entity.Name} has been deleted");
                
            
           }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error ocuured while deleting new product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                if (product is not null)
                    return product;
                else
                {
                    return null!;
                } 
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error ocuured while retrieving new product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error ocuured while retrieving new products");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync();
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error ocuured while retrieving new products");
                
            }
        }

        public Task<Response> UpdateAsync(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
