using EIUSmartWarehouse.Models;
using EIUSmartWarehouse.Models.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EIUSmartWarehouse.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly DBContext _DBContext;
        private readonly ILogger<AdminController> _logger;
        public AdminController(DBContext context, ILogger<AdminController> logger)
        {
            _DBContext = context;
            _logger = logger;
        }
        public IActionResult ManageProduct()
        {
            var products = _DBContext.StoredProduct
                                        .Include(sp => sp.Customer)
                                        .Include(sp => sp.Warehouse)
                                        .ToList();
            ViewBag.StoredProducts = products;
            return View();
        }
        public IActionResult HomeAdmin()
        {
            var warehouseList = _DBContext.Warehouse.ToList();
            ViewBag.WarehouseList = warehouseList;
            return View();
        }
        public IActionResult ManageCustomer()
        {
            var customers = _DBContext.Customer
                                        .ToList();
            ViewBag.Customers = customers;
            return View();
        }
        public IActionResult ManageWarehouse()
        {
            var warehouses = _DBContext.Warehouse
                                        .ToList();
            ViewBag.Warehouses = warehouses;
            return View();
        }
        public IActionResult ManageStaff()
        {
            var staffs = _DBContext.Staff
                                        .ToList();
            ViewBag.Staffs = staffs;
            return View();
        }
        public IActionResult CreateProduct()
        {
            ViewBag.Customers = _DBContext.Customer.ToList();
            ViewBag.Warehouses = _DBContext.Warehouse.ToList();
            ViewBag.Staffs = _DBContext.Staff.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult CreateProduct(StoredProduct newProduct)
        {
            try
            {
                // 1. Find a warehouse with Location_status = "empty"
                var emptyWarehouse = _DBContext.Warehouse
                    .FirstOrDefault(w => w.Location_status == "empty");

                if (emptyWarehouse == null)
                {
                    ViewBag.Customers = _DBContext.Customer.ToList();
                    ViewBag.Warehouses = _DBContext.Warehouse.ToList();
                    ViewBag.Staffs = _DBContext.Staff.ToList();
                    return View(newProduct);
                }

                // 2. Populate the fields of the new product
                newProduct.RFID = emptyWarehouse.RFID;  // Assign the warehouse RFID
                newProduct.InTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"); // Set InTime as current time
                newProduct.OutTime = ""; // OutTime is empty by default
                if(newProduct.Description == null)
                {
                    newProduct.Description = "";
                }

                // 3. Add the new product to the database
                _DBContext.StoredProduct.Add(newProduct);

                // 4. Update the warehouse's Location_status to "occupied"
                emptyWarehouse.Location_status = "occupied";

                // 5. Save both changes (product and warehouse)
                _DBContext.SaveChanges();

                return RedirectToAction("ManageProduct");
            }
            catch (Exception ex)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError(error.ErrorMessage);
                }
            }
            // Repopulate dropdowns in case of validation errors
            ViewBag.Customers = _DBContext.Customer.ToList();
            ViewBag.Warehouses = _DBContext.Warehouse.ToList();
            ViewBag.Staffs = _DBContext.Staff.ToList();

            return View(newProduct);
        }

        public IActionResult EditProduct(int productID)
        {
            var product = _DBContext.StoredProduct
                .Include(sp => sp.Customer)
                .Include(sp => sp.Warehouse)
                .Include(sp => sp.Staff)
                .FirstOrDefault(sp => sp.ProductID == productID);

            if (product == null)
            {
                _logger.LogError($"No product found with ProductID: {productID}");
                return NotFound();
            }

            ViewBag.Customers = _DBContext.Customer.ToList();
            ViewBag.Warehouses = _DBContext.Warehouse.ToList();
            ViewBag.Staffs = _DBContext.Staff.ToList();

            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(StoredProduct updatedProduct)
        {
            /*if (ModelState.IsValid)
            {*/
                var product = _DBContext.StoredProduct
                    .Include(sp => sp.Warehouse)  // Ensure warehouse is included
                    .FirstOrDefault(sp => sp.ProductID == updatedProduct.ProductID);

                if (product != null)
                {
                    var previousRFID = product.RFID;

                    // Update the product fields
                    product.ProductCode = updatedProduct.ProductCode;
                    product.ProductName = updatedProduct.ProductName; 
                    product.ProductUnit = updatedProduct.ProductUnit;

                    // Update Customer and Staff references
                    product.CustomerID = updatedProduct.CustomerID;
                    product.StaffID = updatedProduct.StaffID;
                    product.RFID = updatedProduct.RFID;

                    // Handle warehouse update when RFID is provided
                    if (!string.IsNullOrEmpty(updatedProduct.RFID))
                    {
                        var previousWarehouse = _DBContext.Warehouse.FirstOrDefault(w => w.RFID == previousRFID);
                        if (previousWarehouse != null)
                        {
                            previousWarehouse.Location_status = "empty";
                        }
                        var warehouse = _DBContext.Warehouse.FirstOrDefault(w => w.RFID == updatedProduct.RFID);
                        if (warehouse != null)
                        {
                            warehouse.Location_status = "occupied"; // Update warehouse status
                        }
                    }

                    // Update optional fields
                    product.Status = updatedProduct.Status;
                    product.InTime = updatedProduct.InTime;
                    product.OutTime = updatedProduct.OutTime ?? product.OutTime; // Null check
                    product.Description = updatedProduct.Description ?? product.Description; // Null check

                    // Save changes to the database
                    _DBContext.SaveChanges();

                    return RedirectToAction("ManageProduct");
                }
                else
                {
                    _logger.LogError($"Product with ID {updatedProduct.ProductID} not found.");
                    return NotFound();
                }
            /*}
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError(error.ErrorMessage);
                }
            }*/

            // Ensure the dropdown lists are available in the view when there are validation errors
            ViewBag.Customers = _DBContext.Customer.ToList();
            ViewBag.Warehouses = _DBContext.Warehouse.ToList();
            ViewBag.Staffs = _DBContext.Staff.ToList();

            return View(updatedProduct);  // Return the view with the updated product
        }
        [HttpPost]
        public IActionResult DeleteProduct(int productID)
        {
            _logger.LogInformation($"ProductID passed: {productID}");
            var product = _DBContext.StoredProduct
                .Include(sp => sp.Warehouse)
                .FirstOrDefault(sp => sp.ProductID == productID);

            if (product != null)
            {
                var warehouseRFID = product.Warehouse.RFID;
                _DBContext.StoredProduct.Remove(product);
                if (!string.IsNullOrEmpty(warehouseRFID))
                {
                    var warehouse = _DBContext.Warehouse.FirstOrDefault(w => w.RFID == warehouseRFID);
                    if (warehouse != null)
                    {
                        warehouse.Location_status = "empty";
                    }
                }
                _DBContext.SaveChanges();
                TempData["msg"] = "Delete Successful";
            }
            else
            {
                TempData["msg"] = "Delete Unsuccessful: Product not found.";
            }
            return RedirectToAction("ManageProduct");
        }
    }
}
