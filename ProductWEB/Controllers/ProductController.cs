using Microsoft.AspNetCore.Mvc;
using ProductWEB.Models;
using ProductWEB.Repository.IRepository;
using ProductWEB.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProductWEB.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly Util<Product> util;

        public ProductController(IProductRepository productRepository, IHttpClientFactory httpClientFactory)
        {
            this.productRepository = productRepository;
            util = new Util<Product>(httpClientFactory);
        }
        public async Task<IActionResult> Index()
        {

            return View(await util.GetAllAsync(Resource.ProductAPIURL));
        }

        public IActionResult Create()
        {
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count>0)
                {
                    byte[] imgBytes = null;
                    using (Stream stream= files[0].OpenReadStream())
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            imgBytes = memoryStream.ToArray();
                        }
                    }
                    product.Image = imgBytes;
                }
                var modelStateError= await util.CreateAsync(Resource.ProductAPIURL, product);
                if (modelStateError.Response.Errors.Count>0)
                {
                    foreach (var item in modelStateError.Response.Errors)
                    {
                        product.Errors.Add(item);
                    }
                    return View(product);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await util.GetAsync(Resource.ProductAPIURL, id.GetValueOrDefault());
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] imgBytes = null;
                    using (Stream stream = files[0].OpenReadStream())
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            imgBytes = memoryStream.ToArray();
                        }
                    }
                    product.Image = imgBytes;
                }
                var modelStateError = await util.UpdateAsync(Resource.ProductAPIURL +product.Id, product);
                if (modelStateError.Response.Errors.Count > 0)
                {
                    foreach (var item in modelStateError.Response.Errors)
                    {
                        product.Errors.Add(item);
                    }
                    return View(product);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await util.GetAsync(Resource.ProductAPIURL, id.GetValueOrDefault());
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await util.GetAsync(Resource.ProductAPIURL, id);
            if (product == null) return NotFound();

            var modelstateError = await util.DeleteAsync(Resource.ProductAPIURL, product.Id);
            if(modelstateError.Response.Errors.Count > 0)
            {
                foreach (var item in modelstateError.Response.Errors)
                {
                    product.Errors.Add(item);
                }
                return View(product);
            }

            return RedirectToAction(nameof(Index));


        }
    }
}
