using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SuperbrainManagement.Models;
using PagedList.Mvc;
using PagedList;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Drawing.Drawing2D;

namespace SuperbrainManagement.Controllers.RegistrationStudent
{
    public class ProductsController : Controller
    {
        public ModelDbContext db = new ModelDbContext();

        // GET: Products
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ViewList() { return View(); }
        public ActionResult Loadlist(string sort, string searchString)
        {
            string str = "";
            string querysort = "";
            string querysearch = "";
            int IdBrand_HQ = db.Branches.SingleOrDefault(b => b.Code.ToLower() == "hq").Id;

            if (!string.IsNullOrEmpty(searchString))
            {
                querysearch = " and (p.Name like N'%" + searchString + "%' or p.Code like N'%" + searchString + "%')";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "name":
                        querysort = " order by p.Name";
                        break;
                    case "name_desc":
                        querysort = " order by p.Name desc";
                        break;
                    case "date_desc":
                        querysort = " order by p.Id desc";
                        break;
                    default:
                        querysort = " order by p.Id";
                        break;
                }
            }
            int count = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryCat = "SELECT * From ProductCategory where Active=1 and Enable=1";

                SqlCommand commandCat = new SqlCommand(queryCat, connection);
                connection.Open();
                SqlDataReader readerCat = commandCat.ExecuteReader();

                while (readerCat.Read())
                {
                    str += "<tr>"
                        + "<td class='text-center text-success fw-bolder'>" + readerCat["Code"] + "</td>"
                        + "<td class='text-success fw-bolder' colspan=10>" 
                        + readerCat["Name"] 
                        +"<a href=\"javascript:Edit_Category(" + readerCat["Id"] +")\" class=\"ms-2\"><i class=\"ti ti-edit text-primary fw-bolder\"></i></a>"
                        +"<a href=\"javascript:Delete_category(" + readerCat["Id"] +")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>"
                        + "</td>"
                        + "</tr>";
                    string query = "SELECT p.Id,p.Name,p.Description,p.Image,p.Unit,p.Price,p.Code,p.UnitOfPackage,NumberOfPackage,p.Active,p.Quota,COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '1' AND re.IdBranch = " + IdBrand_HQ + "), 0) -"
                                        + " COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '0' AND re.IdBranch = " + IdBrand_HQ + "), 0) AS Tonkho"
                                        + " FROM product p"
                                        + " where p.enable=1 and p.IdCategory=" + readerCat["Id"]
                                        + querysearch
                                        + querysort;
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        double amount = Double.Parse(reader["Price"].ToString(), 0);
                        count++;
                        str += "<tr>"
                                + "<td class='text-center'>" + count + "</td>"
                                + "<td>" + reader["code"].ToString() + "</td>"
                                + "<td>" 
                                    +"<div class=\"d-flex align-items-center\">" 
                                        +"<div class=\"flex-shrink-0 me-3\"><a href=\"/reback-react/ecommerce/products/10006\">" 
                                            +"<img src=\"" + reader["Image"] +"\" alt=\"\" class=\"rounded-2\" height='40'></a></div><div class=\"flex-grow-1\">" 
                                            +"<span class=\"mt-0 mb-0 fw-bolder\">" + reader["Name"] +"</span><br/>" 
                                            +"<span class='fs-3'><i class=\"ti ti-corner-down-right\"></i>" + reader["Description"] +"</span>" 
                                        +"</div>" 
                                    +"</div>" 
                                +"</td>"
                                + "<td>" + string.Format("{0:N0} đ", amount) + "</td>"
                                + "<td>" + reader["Unit"].ToString() + "</td>"
                                + "<td class='text-center'>" + reader["Tonkho"].ToString() + "</td>"
                                + "<td class='text-center'>" + reader["Quota"].ToString() + "</td>"
                                + "<td class='text-center'>" + reader["UnitOfPackage"].ToString() + "</td>"
                                + "<td class='text-center'>" + reader["NumberOfPackage"].ToString() + "</td>"
                                + "<td class='text-center'>" + (reader["Active"].ToString().ToLower() == "true" ? "<label class=\"custom-control ios-switch\"><input type=\"checkbox\" class=\"ios-switch-control-input\" onchange=\"javascript: ChangeStatus(this)\" data-id=\"" + reader["Id"] + "\" value=\"0\" checked><span class=\"ios-switch-control-indicator\"></span></label>" : "<label class=\"custom-control ios-switch\"><input type=\"checkbox\" class=\"ios-switch-control-input\" onchange=\"javascript: ChangeStatus(this)\" data-id=\"" + reader["Id"] + "\" value=\"1\"><span class=\"ios-switch-control-indicator\"></span></label>") + "</td>"
                                + "<td class='text-end'>"
                                    + "<a href=\"/products/edit/" + reader["Id"] + "\" class=\"me-1\"><i class=\"ti ti-edit text-primary\"></i></a>"
                                    + "<a href=\"/products/delete/" + reader["Id"] + "\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>"
                                    + "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">"
                                        + "<i class=\"ti ti-dots-vertical\"></i>"
                                    + "</a>"
                                    + "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">"
                                        + "<li><a href=\"javascript:void(0)\" data-id=\"" + reader["Id"] + "\" class=\"dropdown-item\"><i class=\"ti ti-user-plus\"></i> Cài đặt bán</a></li>"
                                    + "</ul>"
                                + "</td>"
                                + "</tr>";
                    }
                    reader.Close();
                }
                readerCat.Close();
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_EditCategory(int IdCategory)
        {
            var c = db.ProductCategories.Find(IdCategory);
            var name = c.Name;
            var code = c.Code;
            var displayorder = c.DisplayOrder;
            var description = c.Description;
            var item = new
            {
                code,
                name,
                description,
                displayorder
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_AddCategory(int? Id, string Code, string Name, string Description, string Action,int DisplayOrder)
        {

            string status = "ok";
            string message = "";
            var pc = db.ProductCategories.SingleOrDefault(x=> x.Code == Code&&x.Enable==true);
            if (Action == "create")
            {
                if (pc == null)
                {
                    var c = new ProductCategory()
                    {
                        Code = Code,
                        Name = Name,
                        Description = Description,
                        DisplayOrder = DisplayOrder,
                        Enable = true,
                        DateCreate = DateTime.Now,
                        Active = true,
                        IdUser = Convert.ToInt32(CheckUsers.iduser())
                    };
                    db.ProductCategories.Add(c);
                    db.SaveChanges();
                    status = "ok";
                    message = "Đã thêm thành công!"; 
                }
                else
                {
                    status = "error";
                    message = "Đã tồn tại mã danh mục này!";
                }
            }
            else
            {
                var cb = db.ProductCategories.Find(Id);
                if (cb == null)
                {
                    status = "error";
                    message = "Không tìm thấy danh mục này!";
                }
                else
                {
                    cb.Name = Name;
                    cb.Code = Code;
                    cb.Description = Description;
                    cb.DisplayOrder = DisplayOrder;
                    db.Entry(cb);
                    db.SaveChanges();
                    status = "ok";
                    message = "Đã cập nhật thành công!";
                }
            }
            var item = new
            {
                status,
                message
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Delete_Category(int IdCategory)
        {
            string status, message;
            var pc = db.ProductCategories.Find(IdCategory);
            if (pc == null)
            {
                status = "error";
                message = "Không tồn tại danh mục này!";
            }

            db.ProductCategories.Remove(pc);
            db.SaveChanges();
            status = "ok";
            message = "Đã xóa thành công!";
            var item = new { 
                status,
                message
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }
        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.IdCategory = new SelectList(db.ProductCategories.Where(x=>x.Enable==true), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Code,Image,Enable,Active,DateCreate,IdUser,Price,DiscountPrice,StatusDiscount,IdCategory,IsCore,Unit,Quota,NumberOfPackage,UnitOfPackage,Inventory,IsFixed,IsSale,IdSupplier,PowerScore")] Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                product.Active = true;
                product.Enable = true;
                product.DateCreate = DateTime.Now;
                product.IdUser = int.Parse(CheckUsers.iduser());
                // Handle image file upload
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Generate a unique file name
                    string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                    string extension = Path.GetExtension(imageFile.FileName);
                    fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";

                    // Specify the path to save the file
                    string path = Path.Combine(Server.MapPath("~/Uploads/Images"), fileName);

                    // Save the file
                    imageFile.SaveAs(path);

                    // Update the product's image property
                    product.Image = "/Uploads/Images/" + fileName;
                }
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdCategory = new SelectList(db.ProductCategories, "Id", "Name", product.IdCategory);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", product.IdUser);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCategory = new SelectList(db.ProductCategories.Where(x => x.Enable == true), "Id", "Name", product.IdCategory);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Code,Image,Price,DiscountPrice,StatusDiscount,IdCategory,IsCore,Unit,Quota,NumberOfPackage,UnitOfPackage,Inventory,IsFixed,IsSale,IdSupplier,PowerScore")] Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing product from the database
                    var existingProduct = await db.Products.FindAsync(product.Id);
                    if (existingProduct == null)
                    {
                        return HttpNotFound();
                    }

                    // Update the fields that are allowed to change
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Code = product.Code;
                    existingProduct.Price = product.Price;
                    existingProduct.DiscountPrice = product.DiscountPrice;
                    existingProduct.StatusDiscount = product.StatusDiscount;
                    existingProduct.IdCategory = product.IdCategory;
                    existingProduct.IsCore = product.IsCore;
                    existingProduct.Unit = product.Unit;
                    existingProduct.Quota = product.Quota;
                    existingProduct.NumberOfPackage = product.NumberOfPackage;
                    existingProduct.UnitOfPackage = product.UnitOfPackage;
                    existingProduct.Inventory = product.Inventory;
                    existingProduct.IsFixed = product.IsFixed;
                    existingProduct.IsSale = product.IsSale;
                    existingProduct.IdSupplier = product.IdSupplier;
                    existingProduct.PowerScore = product.PowerScore;

                    // Handle image file upload
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        // Generate a unique file name
                        string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                        string extension = Path.GetExtension(imageFile.FileName);
                        fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";

                        // Specify the path to save the file
                        string path = Path.Combine(Server.MapPath("~/Uploads/Images"), fileName);

                        // Save the file
                        imageFile.SaveAs(path);

                        // Update the product's image property
                        existingProduct.Image = "/Uploads/Images/" + fileName;
                    }

                    // Mark the entity as modified
                    db.Entry(existingProduct).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            ViewBag.IdCategory = new SelectList(db.ProductCategories, "Id", "Name", product.IdCategory);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult updateStatus(int id, int status)
        {
            Product product = db.Products.Find(id);
            if (product != null)
            {
                product.Active = status == 1;
                db.SaveChanges();
                return Json(product.Active);
            }
            else
            {
                return Json(false);
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
