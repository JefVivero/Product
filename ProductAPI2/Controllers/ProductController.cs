using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Models.DTO;
using ProductAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllProduct()
        {
            var products= productRepository.GetAllProduct();
            return Ok(mapper.Map<List<ProductDTO>>(products));
        }

        [HttpGet("{id}", Name ="GetProduct")]
        public IActionResult GetProduct(int id)
        {
            var product = productRepository.GetProduct(id);
            if (product==null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<ProductDTO>(product));
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] ProductDTO productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);         

            if (productRepository.ProductExist(productDto.Name))
            {
                ModelState.AddModelError("Response", $"Ya existe un producto con el nombre {productDto.Name}");
                return StatusCode(404, ModelState);
            }

            var product = mapper.Map<Product>(productDto);
            if (!productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("Response", $"Ha ocurrido un error al intentar guardar el producto {productDto.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);            
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductDTO productDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != productDTO.Id) return BadRequest(ModelState);            

            var product = mapper.Map<Product>(productDTO);
            if (!productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("Response", $"Ha ocurrido un error al intentar actualizar el producto {productDTO.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            if (!productRepository.ProductExist(id)) return NotFound();

            var product = productRepository.GetProduct(id);
            if (!productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("Response", $"Ha ocurrido un error al intentar eliminar el producto {product.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
          
        }
    }
}
