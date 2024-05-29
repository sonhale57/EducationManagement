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

namespace SuperbrainManagement.Controllers
{
    public class CoursesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Courses
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page,string idBranch)
        {
            var branches = db.Branches.ToList();
            int idbranch = int.Parse(CheckUsers.idBranch());
            if (!CheckUsers.CheckHQ())
            {
                branches = db.Branches.Where(x => x.Id == idbranch).ToList();
            }
            if (string.IsNullOrEmpty(idBranch))
            {
                idBranch = branches.First().Id.ToString();
            }

            ViewBag.IdBranch = new SelectList(branches, "Id", "Name", idBranch);
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Name");

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var courses = db.CourseBranches.ToList();

            if (!string.IsNullOrEmpty(idBranch))
            {
                courses = courses.Where(x => x.IdBranch == int.Parse(idBranch)).ToList();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(x => x.Course.Name.ToLower().Contains(searchString.ToLower()) || x.Course.Code.ToLower().Contains(searchString.ToLower()) || x.Course.Program.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    courses = courses.OrderByDescending(s => s.Course.Name).ToList();
                    break;
                case "date":
                    courses = courses.OrderBy(s => s.Course.Id).ToList();
                    break;
                case "name":
                    courses = courses.OrderBy(s => s.Course.Name).ToList();
                    break;
                default:
                    courses = courses.OrderByDescending(s => s.Course.Id).ToList();
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            var pagedData = courses.ToPagedList(pageNumber, pageSize);
            var pagedListRenderOptions = new PagedListRenderOptions();
            pagedListRenderOptions.FunctionToTransformEachPageLink = (liTag, aTag) =>
            {
                liTag.AddCssClass("page-item");
                aTag.AddCssClass("page-link");
                return liTag;
            };
            ViewBag.PagedListRenderOptions = pagedListRenderOptions;
            return View(pagedData);
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
                        count++;
                        str += "<tr>"
                            + "<td class='text-center'>" +count+ "</td>"
                            + "<td class=''>" + reader["Code"].ToString() + "</td>"
                            + "<td class=''>" + reader["Name"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["PriceCourse"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["PriceTest"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["PriceAccount"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["Sessons"].ToString() + "</td>"
                            +"<td class='text-end'>" 
                            + "<a href=\"/courses/edit?IdBranch=" + IdBranch + "&IdCourse=" + reader["Id"] +"\" class=\"me-1\"><i class=\"ti ti-edit text-primary\"></i></a>"
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

        public ActionResult Loadlist_vattu(string IdCourse) 
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
                            + "<td class='' >" + readerCat["Unit"].ToString() + "</td>"
                            + "<td class='' >" + readerCat["Amount"].ToString() + "</td>"
                            + "<td class=''>" + readerCat["Price"].ToString() + "</td>"
                            + "<td class='text-end'><a href=\"/ProductCourses/delete/" + readerCat["Id"] +"\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a></td>"
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

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
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
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Code");
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
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Code", course.IdProgram);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", course.IdUser);
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCourseBranches([Bind(Include = "IdBranch,IdCourse,PriceCourse,PriceAccount,PriceTest,Hour,Sessons,DiscountPrice,StatusDiscount,FromdateDiscount,TodateDiscount")] CourseBranch courseBranch)
        {
            if (ModelState.IsValid)
            {
                db.CourseBranches.Add(courseBranch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", courseBranch.IdBranch);
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Code", courseBranch.IdCourse);
            return View("Index");
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
            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Code", course.IdProgram);
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
                return RedirectToAction("Index");
            }
            ViewBag.IdProgram = new SelectList(db.Programs, "Id", "Code", course.IdProgram);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", course.IdUser);
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
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
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            return RedirectToAction("Index");
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
