using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using DataAccess.Models;
using BusinessLogic.ProductServices;
using BusinessLogic.CategoryServices;

namespace Ghafar_Tajhiz_Admin.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductsController(ProductService productService, CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }



        // GET: Products
        public async Task<IActionResult> Index()
        {
            var data =await _productService.GetProductsWithCategory();
            return View(data);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _productService.GetProductsWithCategory(p=>p.ProductId==id);
            var product = products.FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetCategories(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductDescription,Price,StockQuantity,ImageUrl,IsAvailable,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreateDate = DateTime.Now;
                await _productService.CreateProduct(product);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductById(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescription,Price,StockQuantity,ImageUrl,IsAvailable,CategoryId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProduct(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetCategories(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _productService.GetProductsWithCategory(p => p.ProductId == id);
            var product = products.FirstOrDefault();
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _productService.GetProductsWithCategory(p => p.ProductId == id);
            var product = products.FirstOrDefault();
            if (product != null)
            {
                await _productService.DeleteProduct(product);
            }

            return RedirectToAction(nameof(Index));
        }

        
    }
}
