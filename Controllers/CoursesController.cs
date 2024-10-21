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
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x=>x.Enable==true), "Id", "Name");
            ViewBag.IdBranchForm = new SelectList(db.Branches.Where(x => x.Enable == true), "Id", "Name");
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Name");
            return View();
        }
        public ActionResult Loadlist(int? IdBranch,string searchString)
        {
            string str = "",strquery="";
            if (IdBranch==null)
            {
                IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                strquery = " and (c.Name like N'%"+searchString+"%' or c.Code like N'%"+searchString+"%')";
            }
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryCat = "SELECT * from Program where enable=1 order by DisplayOrder";
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
                    string query = "select c.Id,c.Code,c.Name,cb.Hour,cb.Sessons,cb.PriceCourse,cb.PriceTest,cb.PriceAccount,DiscountPrice,cb.StatusDiscount"
                                        + " from Course c inner join CourseBranch cb on c.Id = cb.IdCourse" 
                                        +" where c.IdProgram=" + readerCat["Id"] +" and cb.IdBranch="+IdBranch +strquery+ " order by c.DisplayOrder";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        double amount = Double.Parse(reader["PriceCourse"].ToString(), 0);
                        double amountTest = Double.Parse(reader["PriceTest"].ToString(),0);
                        double amountAccount = Double.Parse(reader["PriceAccount"].ToString(),0);
                        count++;
                        str += "<tr>"
                            + "<td class='text-center'>" + reader["Code"].ToString() + "</td>"
                            + "<td class=''>" + reader["Name"].ToString() + "</td>"
                            + "<td class='text-end'>" + string.Format("{0:N0} đ", amount) + "</td>"
                            + "<td class='text-end'>" + string.Format("{0:N0} đ", amountAccount) + "</td>"
                            + "<td class='text-center'>" + reader["Sessons"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["Hour"].ToString() + "</td>"
                            + "<td class='text-end'>" 
                            + "<a href='javascript:Edit_CourseBranch(" + IdBranch + "," + reader["Id"] +")' class=\"me-1\"><i class=\"ti ti-edit text-primary\"></i></a>"
                            + "<a href='javascript:Delete_CourseBranch(" + IdBranch + "," + reader["Id"] + ")' class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>"
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


        [HttpPost]
        public async Task<ActionResult> Delete_CourseBranch(int IdBranch,int IdCourse)
        {
            string status,message;
            var pc = await db.CourseBranches.Where(x=>x.IdBranch==IdBranch&&x.IdCourse==IdCourse).FirstOrDefaultAsync();
            if (pc == null)
            {
                status = "error";
                message = "Không tìm thấy khóa học của cơ sở này!";
                return HttpNotFound();
            }

            db.CourseBranches.Remove(pc);
            await db.SaveChangesAsync();
            status = "ok";
            message = "Đã xóa thành công!";
            var item = new { 
                status,
                message
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_EditCourseBranch(int IdBranch,int IdCourse) {
            var c = db.CourseBranches.Where(x=>x.IdBranch==IdBranch&&x.IdCourse==IdCourse).FirstOrDefault();
            var phi = c.PriceCourse;
            var phitest = c.PriceTest;
            var phitk = c.PriceAccount;
            var sobuoi = c.Sessons;
            var sogio = c.Hour;
            var item = new {
                phi, phitest, phitk,sobuoi, sogio
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
            var item = new { 
                status = status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
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
                            + "<td class='text-center' >" + readerCat["Amount"].ToString() + "</td>"
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
        public ActionResult Delete_Course(int id)
        {
            var c =  db.Courses.Find(id);
            if (c == null)
            {
                return Json(new {status="error",message="Không tìm thấy khóa học!"},JsonRequestBehavior.AllowGet);
            }
            if (db.RegistrationCourses.Any(x => x.IdCourse == id))
            {
                return Json(new { status = "error", message = "Không thể xóa học viên, Đã có học viên đăng kí khóa học này!" }, JsonRequestBehavior.AllowGet);
            }
            db.Courses.Remove(c);
            db.SaveChanges();
            return Json(new { status="ok",message = "Đã xóa khóa học thành công!" },JsonRequestBehavior.AllowGet);
        }
        public ActionResult Loadlist_Statistics(int? IdBranch, string month)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            if (IdBranch == null)
            {
                IdBranch = idbranch;
            }
            // Tách tháng và năm từ chuỗi tháng
            var monthYear = month.Split('/');
            int monthValue = int.Parse(monthYear[0]);
            int yearValue = int.Parse(monthYear[1]);

            string str = "";
            var query = from c in db.Courses
                        join cb in db.CourseBranches on c.Id equals cb.IdCourse
                        where cb.IdBranch==IdBranch
                        let countOnClass = db.StudentJoinClasses.Count(x => x.IdCourse == c.Id
                                 && x.Registration.IdBranch == IdBranch
                                 && x.Todate.Value.Month >= monthValue
                                 && x.Todate.Value.Year >= yearValue)
                        let countRegistration = db.RegistrationCourses.Count(x=>x.IdCourse==c.Id && x.Registration.IdBranch == IdBranch && x.Registration.DateCreate.Value.Month==monthValue&&x.Registration.DateCreate.Value.Year==yearValue)
                        orderby c.Program.DisplayOrder,c.DisplayOrder
                        select new{
                            NameProgram = c.Program.Name,
                            NameCourse = c.Name,
                            CountRegistration = countRegistration,
                            CountOnClass = countOnClass
                        };

            // Tạo một đối tượng để trả về kết quả
            if(query == null)
            {
                str = "<tr><td colspan=6>Không tìm thấy dữ liệu</td></tr>";
            }
            int stt = 0;
            foreach(var s in query)
            {
                stt++;
                str += "<tr>"
                    +"<td class='text-center'>"+stt + "</td>"
                    +"<td class='text-center'>"+s.NameCourse + "</td>"
                    +"<td class='text-center'>"+s.CountRegistration + "</td>"
                    +"<td class='text-center'>"+s.CountOnClass + "</td>"
                    + "</tr>";
            }
            return Json(new {str, data = query.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Statistics()
        {
            var config = db.Configurations;
            var list = new List<string>();
            if(config != null)
            {
                foreach(var c in config)
                {
                    list.Add(c.TimePayment.Value.ToString("MM/yyyy"));
                }    
            }
            ViewBag.Month = new SelectList(list);
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x=>x.Enable==true), "Id", "Name");
            return View();
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
        public ActionResult Submit_CourseBranch(int IdBranch, int IdCourse, decimal PriceCourse, decimal PriceOnline, decimal PriceTest,string Action,int Sessons,int Hour)
        {

            string status = "ok";
            string message = "";
            var cb = db.CourseBranches.SingleOrDefault(x => x.IdBranch == IdBranch && x.IdCourse == IdCourse);
            if (Action == "create")
            {
                if (cb == null)
                {
                    var courseBranch = new CourseBranch()
                    {
                        IdCourse = IdCourse,
                        IdBranch = IdBranch,
                        PriceCourse = PriceCourse,
                        PriceAccount = PriceOnline,
                        PriceTest = PriceTest,
                        Sessons = Sessons,
                        Hour = Hour
                    };
                    db.CourseBranches.Add(courseBranch);
                    db.SaveChanges();
                    status = "ok";
                    message = "Đã thêm thành công!";
                }
                else
                {
                    status = "error";
                    message = "Cơ sở đã tồn tại khóa học này!";
                }
            }
            else
            {
                if (cb == null)
                {
                    status = "error";
                    message = "Không tìm thấy khóa học này của cơ sở!";
                }
                else
                {
                    cb.PriceAccount = PriceOnline;
                    cb.PriceTest = PriceTest;
                    cb.PriceCourse = PriceCourse;
                    cb.Sessons = Sessons;
                    cb.Hour = Hour;
                    db.Entry(cb);
                    db.SaveChanges();
                    status="ok";
                    message = "Đã cập nhật thành công!";
                }
            }
            var item = new
            {
                status,message
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
