using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using SuperbrainManagement.Models;
using System.Web.Services.Description;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SuperbrainManagement.Controllers
{
    public class CoursesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Courses
        public ActionResult Index()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name");
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Name");
            return View();
        }
        public ActionResult Loadlist(string sortOrder, string searchString, string IdBranch)
        {
            string str = "";
            string querysort = "";
            string querysearch = "";
            if (string.IsNullOrEmpty(IdBranch))
            {
                IdBranch = CheckUsers.idBranch();
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                querysearch = " and p.Name like N'" + searchString + "' or p.Code like N'" + searchString + "'";
            }
            if (!string.IsNullOrEmpty(sortOrder))
            {
                switch (sortOrder)
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
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryCat = "SELECT * from Program where enable=1";
                SqlCommand commandCat = new SqlCommand(queryCat, connection);
                connection.Open();
                SqlDataReader readerCat = commandCat.ExecuteReader();
                int count = 0;
                while (readerCat.Read())
                {
                    str += "<tr>"
                            + "<td class='text-center text-success fw-bolder'>" + readerCat["Code"].ToString() + "</td>"
                            + "<td class='text-success fw-bolder' colspan=7>" + readerCat["Name"].ToString() + "</td>"
                            + "</tr>";
                    string query = "select c.Id,c.Code,c.Name,cb.PriceCourse,cb.PriceTest,cb.PriceAccount,DiscountPrice,cb.StatusDiscount,cb.Sessons"
                                        +" from Course c inner join CourseBranch cb on c.Id = cb.IdCourse" 
                                        +" where c.IdProgram=" + readerCat["Id"] +" and cb.IdBranch="+IdBranch +querysearch +querysort;
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        double amount = Double.Parse(reader["PriceCourse"].ToString(), 0);
                        double amountTest = Double.Parse(reader["PriceTest"].ToString(),0);
                        double amountAccount = Double.Parse(reader["PriceAccount"].ToString(),0);

                        //double priceCourse = Double.Parse(reader["PriceCourse"].ToString());
                        //double priceTest = Double.Parse(reader["PriceTest"].ToString());
                        //double priceAccount = Double.Parse(reader["PriceAccount"].ToString());
                        count++;
                        str += "<tr>"
                            + "<td class='text-center'>" +count+ "</td>"
                            + "<td class=''>" + reader["Code"].ToString() + "</td>"
                            + "<td class=''>" + reader["Name"].ToString() + "</td>"
                            + "<td class='text-center'>" + string.Format("{0:N0} đ", amount) + "</td>"
                            + "<td class='text-center'>" + string.Format("{0:N0} đ", amountTest) + "</td>"
                            + "<td class='text-center'>" + string.Format("{0:N0} đ", amountAccount) + "</td>"
                            + "<td class='text-center'>" + reader["Sessons"].ToString() + "</td>"
                            +"<td class='text-end'>" 
                            + "<a href='javascript:void(0)' class=\"me-1\"><i class=\"ti ti-edit text-primary\"></i></a>"
                            + "</td>"
                            + "</tr>";
                    }
                }
                readerCat.Close();
            }
            var item = new {
            str
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_ProductCourse(int IdCourse,int IdProduct) 
        {
            string status = "ok";
            var productCourse = db.ProductCourses.SingleOrDefault(x=>x.IdCourse == IdCourse&&x.IdProduct==IdProduct);
            if (productCourse == null)
            {
                var pc = new ProductCourse()
                {
                    DateCreate = DateTime.Now,
                    IdCourse = IdCourse,
                    IdProduct = IdProduct,
                    IdUser = int.Parse(CheckUsers.iduser()),
                    Amount = 1,
                    Enable = true,
                    Status = true
                };
                db.ProductCourses.Add(pc);
                db.SaveChanges();
                status = "ok";
            }
            else
            {
                status = "Đã tồn tại vật tư này!";
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Loadlist_ProductCourse(string IdCourse) 
        {
            string str = "";
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryCat = "select pc.Id,p.Code,p.Name,pc.Amount,p.Unit,p.Price from ProductCourse pc inner join Product p on pc.IdProduct = p.Id where pc.IdCourse="+IdCourse;
                SqlCommand commandCat = new SqlCommand(queryCat, connection);
                connection.Open();
                SqlDataReader readerCat = commandCat.ExecuteReader();
                int count = 0;
                while (readerCat.Read())
                {
                    count++;
                    str += "<tr>"
                            + "<td class='text-center '>" + count + "</td>"
                            + "<td class='' >" + readerCat["Name"].ToString() + "</td>"
                            + "<td class='' >" + readerCat["Amount"].ToString() + "</td>"
                            + "<td class='text-end'><a href=\"javascript:Delete_ProductCourse(" + readerCat["Id"] +","+IdCourse+")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a></td>"
                            + "</tr>";
                    
                }
                readerCat.Close();
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> Delete_ProductCourse(int id)
        {
            var pc = await db.ProductCourses.FindAsync(id);
            if (pc == null)
            {
                return HttpNotFound();
            }

            db.ProductCourses.Remove(pc);
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }
        [HttpPost]
        public async Task<ActionResult> Delete_Course(int id)
        {
            var c = await db.Courses.FindAsync(id);
            if (c == null)
            {
                return HttpNotFound();
            }

            db.Courses.Remove(c);
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Name");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Hour,Levels,DisplayOrder,Price,IdProgram,Description,FormulaMidterm,SpeedMidterm,FormulaEndterm,SpeedEndterm,DevelopRoute,ScorePass,DateCreate,IdUser,Sessions,Hours")] Course course)
        {
            if (ModelState.IsValid)
            {
                course.DateCreate = DateTime.Now;
                course.IdUser = int.Parse(CheckUsers.iduser());
                db.Courses.Add(course);
                db.SaveChanges();
                return Redirect("/Programs");
            }

            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Name", course.IdProgram);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", course.IdUser);
            return View(course);
        }
        public ActionResult Submit_CourseBranch(int IdBranch, int IdCourse, decimal PriceCourse, decimal PriceOnline, decimal PriceTest, int Sessons)
        {
            string status = "ok";
            var cb = db.CourseBranches.SingleOrDefault(x => x.IdBranch == IdBranch && x.IdCourse == IdCourse);
            if (cb == null)
            {
                var courseBranch = new CourseBranch()
                {
                    IdCourse = IdCourse,
                    IdBranch = IdBranch,
                    PriceCourse = PriceCourse,
                    PriceAccount = PriceOnline,
                    PriceTest = PriceTest,
                    Sessons = Sessons
                };
                db.CourseBranches.Add(courseBranch);
                db.SaveChanges();
                status = "ok";
            }
            else
            {
                status = "Cơ sở đã tồn tại khóa học này.";
            }
            var item = new
            {
                status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Name", course.IdProgram);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", course.IdUser);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Hour,Levels,DisplayOrder,Price,IdProgram,Description,FormulaMidterm,SpeedMidterm,FormulaEndterm,SpeedEndterm,DevelopRoute,ScorePass,DateCreate,IdUser,Sessions,Hours")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("/Programs");
            }
            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Name", course.IdProgram);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", course.IdUser);
            return View(course);
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
