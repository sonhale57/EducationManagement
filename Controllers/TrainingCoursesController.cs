using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using SuperbrainManagement.Helpers;
using SuperbrainManagement.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SuperbrainManagement.Controllers
{
    public class TrainingCoursesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: TrainingCourses
        public ActionResult Index()
        {
            int IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            ViewBag.IdEmployee = new SelectList(db.Employees.Where(x=>x.IdBranch==IdBranch), "Id", "Name");
            var trainingCourses = db.TrainingCourses.Include(t => t.TrainingType).Include(t => t.User);
            return View(trainingCourses.ToList());
        }
        public ActionResult Loadlist()
        {
            string str = "";
            string querybranch = "";
            int idbranch = int.Parse(CheckUsers.idBranch());
            if (!CheckUsers.CheckHQ())
            {
                querybranch = " and IdBranch=" + idbranch;
            }
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select tc.Id,tc.Code,tc.Name,tc.ResgistrationDeadline,tc.Fromdate,tc.Todate,tc.Number,cat.Name as NameCat,(select COUNT(IdEmployee) from RegistrationTraining where IdTraining=tc.Id " + querybranch+") as Soluongdk"
                            +" from TrainingCourse tc"
                            + " join TrainingType cat on cat.Id=tc.IdType"
                            + " where tc.Enable=1";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count++;
                    string strStatus = "";
                    string strbtn = "";

                    querybranch = " and IdBranch=" + idbranch;

                    //Chưa bắt đầu
                    if (DateTime.Parse(reader["Fromdate"].ToString()) > DateTime.Now)
                    {
                        strStatus = "<span class='text-muted'>Chưa diễn ra</span>";
                        // Kiểm tra HQ
                        if (CheckUsers.CheckHQ())
                        {
                            strbtn = "<a href='/trainingcourses/edit/" + reader["Id"] + "' class='me-1'><i class='ti ti-edit text-primary'></i></a>" +
                                    "<a href='javascript:Delete(" + reader["Id"] + ")' class='me-1'><i class='ti ti-trash text-danger'></i></a>" +
                                    "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">" +
                                        "<i class=\"ti ti-dots-vertical\"></i>" +
                                    "</a>" +
                                    "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">" +
                                        "<li>"+
                                        "<a href=\"javascript:Load_dangky(" + reader["Id"] + ")\" class=\"dropdown-item\">" +
                                            "<i class=\"ti ti-receipt\"></i> Đăng ký tham gia" +
                                        "</a>" +
                                        "</li>" +
                                    "</ul>";
                        }
                        else
                        {
                            strbtn ="<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">" +
                                        "<i class=\"ti ti-dots-vertical\"></i>" +
                                    "</a>" +
                                    "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">" +
                                        "<li>"+
                                        "<a href=\"javascript:Load_dangky(" + reader["Id"] + ")\" class=\"dropdown-item\">" +
                                            "<i class=\"ti ti-receipt\"></i> Đăng ký tham gia" +
                                        "</a>" +
                                        "</li>" +
                                    "</ul>";
                        }
                    }
                    else
                    {
                        if (DateTime.Parse(reader["Todate"].ToString()) < DateTime.Now)
                        {
                            strStatus = "<span class='text-danger'>Đã kết thúc</span>";
                            // Kiểm tra HQ
                            if (CheckUsers.CheckHQ())
                            {
                                strbtn = "<a href='/trainingcourses/edit/" + reader["Id"] + "' class='me-1'><i class='ti ti-edit text-primary'></i></a>" +
                                        "<a href='javascript:Delete(" + reader["Id"] + ")' class='me-1'><i class='ti ti-trash text-danger'></i></a>" +
                                        "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">" +
                                            "<i class=\"ti ti-dots-vertical\"></i>" +
                                        "</a>" +
                                        "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">" +
                                            "<li>" +
                                            "<a href=\"javascript:Rating_registration(" + reader["Id"] + ")\" class=\"dropdown-item\">" +
                                                "<i class=\"ti ti-receipt\"></i> Đánh giá khóa đào tạo" +
                                            "</a>" +
                                            "</li>" +
                                        "</ul>";
                            }
                            else
                            {
                                strbtn = "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">" +
                                            "<i class=\"ti ti-dots-vertical\"></i>" +
                                        "</a>" +
                                        "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">" +
                                            "<li>" +
                                            "<a href=\"javascript:Rating_registration(" + reader["Id"] + ")\" class=\"dropdown-item\">" +
                                                "<i class=\"ti ti-receipt\"></i> Xem kết quả khóa đào tạo" +
                                            "</a>" +
                                            "</li>" +
                                        "</ul>";
                            }
                        }
                        else
                        {
                            strStatus = "<span class='text-success'>Đang diễn ra</span>";
                            // Kiểm tra HQ
                            if (CheckUsers.CheckHQ())
                            {
                                strbtn = "<a href='/trainingcourses/edit/" + reader["Id"] + "' class='me-1'><i class='ti ti-edit text-primary'></i></a>" +
                                        "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">" +
                                            "<i class=\"ti ti-dots-vertical\"></i>" +
                                        "</a>" ;
                            }
                        }
                    }

                    str += "<tr>"
                            + "<td class='text-center align-content-center'>" + count + "</td>"
                            + "<td class='align-content-center'>" + reader["name"].ToString() + "<br/> <small class='fst-italic text-primary'>" + reader["NameCat"] +"</small></td>"
                            + "<td class='text-center align-content-center'>" + DateTime.Parse(reader["ResgistrationDeadline"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                            + "<td class='text-center align-content-center'>" + DateTime.Parse(reader["Fromdate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                            + "<td class='text-center align-content-center'>" + DateTime.Parse(reader["Todate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                            + "<td class='text-center align-content-center'><a href='javascript:View_registration(" + reader["Id"] +")' class='fw-bolder'>" + reader["Soluongdk"].ToString() + "</a></td>"
                            + "<td class='text-center align-content-center'>" + strStatus + "</td>"
                            + "<td class='text-end align-content-center'>" + strbtn + "</td>"
                            + "</tr>";
                }
                reader.Close();
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Load_registration(int id)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            string str = "";
            var join = db.RegistrationTrainings.Where(x=>x.IdTraining==id);
            if (!CheckUsers.CheckHQ())
            {
                join = db.RegistrationTrainings.Where(x => x.IdTraining == id && x.IdBranch == idbranch);
            }
            int count = 0;
            var train = db.TrainingCourses.Find(id);
            double tongtien = 0;
            foreach(var ite in join)
            {
                tongtien += Double.Parse(train.Price.ToString());
                count++;    
                str += "<tr>"
                    +"<td class='text-center'>"+count+"</td>"
                    +"<td class='text-left'>"+ite.Employee.Name+"</td>"
                    +"<td class='text-center'>"+ite.Employee.Phone+"</td>"
                    + "<td>" + ite.Employee.Email + "</td>"
                    + "<td class='text-center'>" + ite.Employee.Branch.Name + "</td>"
                    + "<td class='text-center'>" + (ite.StatusPayment==true?"<span class='text-success'>Đã thanh toán</span>":"Chưa thanh toán") + "</td>"
                    + "<td class='text-center'>" + (ite.IsRegisteStay==true?"<span class='text-success'>Đăng ký</span>":"Không đăng ký") + "</td>"
                    +"</tr>";
            }
            str += "<tr><td colspan=7 class='text-end'>Tổng tiền: <b>"+string.Format("{0:N0} đ",tongtien)+"</b></td></tr>";
            var item = new
            {
                str,
                name = train.Name,
                price =train.Price,
                description = train.Description,
                deadline= train.ResgistrationDeadline.Value.ToString("dd/MM/yyyy"),
                from = train.Fromdate.Value.ToString("dd/MM/yyyy"),
                to = train.Todate.Value.ToString("dd/MM/yyyy"),
                tongtien = tongtien
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_infoByEmployee(int id)
        {
            var employee = db.Employees.Find(id);
            string phone = employee.Phone;
            string email =employee.Email;
            string position = (employee.IdPosition ==null ?"":employee.Position.Name);
            string time = (employee.DateStart ==null?"":employee.DateStart.Value.ToString("dd/MM/yyyy"));
            var item = new
            {
                phone,
                email,
                position,
                time,
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_infoByTrainingCourse(int id)
        {
            var tra = db.TrainingCourses.Find(id);
            string name = tra.Name;
            string price = string.Format("{0:N0}",tra.Price);
            string deadline = (tra.ResgistrationDeadline==null?"":"Hạn đăng ký: "+tra.ResgistrationDeadline.Value.ToString("dd/MM/yyyy"));
            var item = new
            {
                name,
                price,
                deadline
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Submit_registration(int IdEmployee, int IdTrainingCourse, string Phone,string Email,bool Luutru)
        {
            string status = "ok";
            var join = db.RegistrationTrainings.SingleOrDefault(x => x.IdTraining == IdTrainingCourse && x.IdEmployee == IdEmployee);
            if (join == null)
            {
                var emp = db.Employees.Find(IdEmployee);
                emp.Phone = Phone;
                emp.Email = Email;
                var reg = new RegistrationTraining()
                {
                    IdEmployee = IdEmployee,
                    IdTraining = IdTrainingCourse,
                    Updatetime = DateTime.Now,
                    IdBranch = emp.IdBranch,
                    StatusPayment = false,
                    IsRegisteStay = Luutru,
                    IdUser = Convert.ToInt32(CheckUsers.iduser())
                };
                db.Entry(emp);
                db.RegistrationTrainings.Add(reg);
                db.SaveChanges();
            }
            else
            {
                status = "Nhân sự này đã được đăng ký tham gia!";
            }
            var item = new
            {
                status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_payment(int id)
        {
            string str = "",strName="",strStatus="";
            int count_chuadongphi = 0;
            double price = 0;
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            var tr = db.TrainingCourses.Find(id);
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select tc.Id,tc.Name as NameCourse,tc.Price,tjoin.IdEmployee,e.Email,e.Name as NameEmployee,e.Phone,e.DateOfBirth,b.Name as NameBranch,tjoin.StatusPayment,tjoin.IdBranch" +
                    " from TrainingCourse tc" +
                    " join RegistrationTraining tjoin on tc.Id = tjoin.IdTraining" +
                    " join Employee e on tjoin.IdEmployee = e.Id" +
                    " join Branch b on b.Id = e.IdBranch" +
                    " where tjoin.IdTraining = " + id + " and tjoin.IdBranch = " + idbranch;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    price = Double.Parse(reader["Price"].ToString());
                    strName = reader["NameCourse"].ToString();
                    count++;
                    str += "<tr>"
                        + "<td class='text-center'>" + count + "</td>"
                        + "<td>" + reader["NameEmployee"] + "</td>"
                        + "<td class='text-center'> " + reader["Phone"] + "</td>"
                        + "<td class='text-center'> " + reader["Email"] + "</td>"
                        + "<td>" + reader["NameBranch"] + "</td>"
                        + "<td class='text-center'>" + (reader["StatusPayment"].ToString() == "False" ? "<span class='text-danger'>Chưa đóng phí</span>" : "<span class='text-success'>Đã đóng phí</span>") + "</td>"
                        + "</tr>";
                    if (reader["StatusPayment"].ToString() == "False")
                    {
                        count_chuadongphi++;
                    }
                }
                reader.Close();
            }
            double tong = price * count_chuadongphi;
            string strphi = string.Format("{0:N0}", price);
            string strtongtien = string.Format("{0:N0}", tong);
            if(count_chuadongphi > 0)
            {
                strStatus = "<span class='text-danger fw-bold'>Có "+count_chuadongphi+" chưa đóng phí.</span>";
            }
            else
            {
                strStatus = "<span class='text-success'>Đã đóng phí.</span>";
            }
            if (tr == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy khóa đào tạo này!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { str,strtongtien,count_chuadongphi,strphi,strName,strStatus}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Submit_createtype(string Code, string Name, string Description)
        {
            string status = "ok";
            var type = db.TrainingTypes.SingleOrDefault(x => x.Code == Code);
            if (type == null)
            {
                var emp = new TrainingType()
                {
                    Code = Code,
                    Name =Name, 
                    Description = Description,
                    Enable = true,
                    Active = true,
                    DateCreate = DateTime.Now,
                    IdUser = Convert.ToInt32(CheckUsers.iduser())
                };
                db.TrainingTypes.Add(emp);
                db.SaveChanges();
            }
            else
            {
                status = "Đã tồn tại mã này!";
            }
            var item = new
            {
                status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_ratingregistration(int id)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            string str = "";
            var join = db.RegistrationTrainings.Where(x => x.IdTraining == id);
            if (!CheckUsers.CheckHQ())
            {
                join = db.RegistrationTrainings.Where(x => x.IdTraining == id && x.IdBranch == idbranch);
            }
            int count = 0;
            var train = db.TrainingCourses.Find(id);
            double tongtien = 0;
            foreach (var ite in join)
            {
                tongtien += Double.Parse(train.Price.ToString());
                count++;
                str += "<tr>"
                    + "<td class='text-center'>" + count + "</td>"
                    + "<td class='text-left'>" + ite.Employee.Name + "</td>"
                    + "<td class='text-center'>" + (ite.Employee.DateOfBirth == null ? "-" : ite.Employee.DateOfBirth.Value.ToString("dd/MM/yyyy")) + "</td>"
                    + "<td class='text-center'>" + ite.Employee.Phone + "</td>"
                    + "<td class='text-center'>" + ite.Employee.Email + "</td>"
                    + "<td class='text-left'>" + ite.Employee.Branch.Name + "</td>"
                    + "<td class='text-center'>" + Get_resultTraninning(ite.Employee.Id, id) + "</td>";
                if (CheckUsers.CheckHQ())
                {
                    str += "<td class='text-end'><a href='javascript:Rating_Employee(" + id + "," + ite.Employee.Id + ")' class='btn btn-sm btn-success me-1'><i class='ti ti-check text-white'></i> Đánh giá</a></td>";
                }
                str += "</tr>";
            }
            string strinfo = "<p><b>Khóa đào tạo:</b> "+train.Name+"</p>"
                            +"<p><b>Ghi chú:</b> " + train.Description + "</p>"
                            + "<p><b>Phí đào tạo:</b> " + string.Format("{0:N0} đ",train.Price) + "</p>"
                            + "<p><b>Thời gian:</b> " + train.Fromdate.Value.ToString("dd/MM/yyyy") + " - "+train.Todate.Value.ToString("dd/MM/yyyy")+"</p>";
            var item = new
            {
                str,
                strinfo
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_infoRating(int IdTrainning,int IdEmployee) 
        {
            var t = db.TrainingCourses.Find(IdTrainning);
            var e = db.Employees.Find(IdEmployee);
            var item = new { 
                strNameT= t.Name,
                strNameE = e.Name
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Submit_rating(int IdTrainingCourse,int IdEmployee,bool Result,int TotalScore,int NumberCertification, string SuperbrainScore,string BrandScore,string TeachScore,string SaleScore,string MindsetScore,string SorobanScore,string OnlineScore,string CompleteScore,string ParticipationScore,string DemeanorScore,string ProactiveScore, string Description)
        {
            string status = "ok";
            var type = db.TrainingResults.SingleOrDefault(x => x.IdTraining == IdTrainingCourse&&x.IdEmpoyee==IdEmployee);
            var e = db.Employees.Find(IdEmployee);
            var reg = db.RegistrationTrainings.SingleOrDefault(x=>x.IdTraining==IdTrainingCourse&& x.IdEmployee==IdEmployee);
            if (type == null)
            {
                var rate = new TrainingResult()
                {
                    IdTraining = IdTrainingCourse,
                    IdEmpoyee = IdEmployee,
                    Result = Result,
                    TotalScore = TotalScore,
                    NumberCertification = NumberCertification,
                    SaleScore = SaleScore,
                    SorobanScore = SorobanScore,
                    SuperbrainScore = SuperbrainScore,
                    BrandScore = BrandScore,
                    TeachScore = TeachScore,
                    MindsetScore = MindsetScore,
                    CompleteScore = CompleteScore,
                    DemeanorScore = DemeanorScore,
                    ParticipationScore = ParticipationScore,
                    ProactiveScore = ProactiveScore,
                    OnlineScore = OnlineScore,
                    Description = Description,
                    DateCreate = DateTime.Now,
                    IdUser = Convert.ToInt32(CheckUsers.iduser())
                };
                db.TrainingResults.Add(rate);
                reg.IsPass = Result;
                reg.Result = TotalScore;
                db.Entry(reg);
                db.SaveChanges();

                if (Result == true)
                {
                    e.IsOfficial = true;
                    e.CertificateNumber = NumberCertification;
                    db.Entry(e);
                    db.SaveChanges();
                    Create_Account(IdEmployee);
                }
            }
            else
            {
                status = "Giáo viên này đã được đánh giá trước đó!";
            }
            var item = new
            {
                status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public static string GetLastName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return string.Empty;
            }

            string[] nameParts = fullName.Split(' ');
            return nameParts[nameParts.Length - 1];
        }
        public static string GetLastThreeDigits(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 3)
            {
                return phoneNumber;
            }

            return phoneNumber.Substring(phoneNumber.Length - 3);
        }

        public void Create_Account(int IdEmployee) {
            ConvertText convi = new ConvertText();

            var e = db.Employees.Find(IdEmployee);
            var user = db.Users.SingleOrDefault(x => x.IdEmployee == IdEmployee);
            string Username = "GV_" + convi.fixtex(GetLastName(e.Name)) + GetLastThreeDigits(e.Phone);
            var IdBranch = e.IdBranch;
            MD5Hash md5 = new MD5Hash();
            string pass = md5.GetMD5Working("taptrung");
            string Password = md5.mahoamd5(pass.Replace("&^%$", ""));
            if (user == null)
            {
                var us = new User() {
                    Name = e.Name,
                    Username = Username,
                    Password = Password,
                    Active = true,
                    Enable =true,
                    Expire = DateTime.Now.AddMonths(6),
                    DateCreate = DateTime.Now,
                    IdBranch = IdBranch,
                    Createby = Convert.ToInt32(CheckUsers.iduser()),
                    IdEmployee = IdEmployee
                };
                db.Users.Add(us);
                db.SaveChanges();
            }
        }
        public string Get_resultTraninning(int IdEmployee,int IdTrainning) {
            var result = db.RegistrationTrainings.SingleOrDefault(x=>x.IdTraining ==IdTrainning && x.IdEmployee == IdEmployee);
            if (result == null)
            {
                return "-";
            }
            else
            {
                if (result.IsPass == null)
                {
                    return "Chưa đánh giá";
                }
                else
                {
                    if (result.IsPass == true)
                    {
                        return "<span class='text-success'>Đã đạt </span>";
                    }
                    else
                    {
                        return "<span class='text-danger'>Chưa đạt</span>";
                    }
                }
            }
        }

        #region DefaultController

        // GET: TrainingCourses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCourse trainingCourse = db.TrainingCourses.Find(id);
            if (trainingCourse == null)
            {
                return HttpNotFound();
            }
            return View(trainingCourse);
        }

        // GET: TrainingCourses/Create
        public ActionResult Create()
        {
            ViewBag.IdType = new SelectList(db.TrainingTypes, "Id", "Name");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: TrainingCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Description,Fromdate,Todate,Enable,Active,ResgistrationDeadline,Price,Number,IdType,IdUser,DateCreate")] TrainingCourse trainingCourse)
        {
            if (ModelState.IsValid)
            {
                trainingCourse.Active = true;
                trainingCourse.Enable= true;
                trainingCourse.IdUser = Convert.ToInt32(CheckUsers.iduser());
                trainingCourse.DateCreate = DateTime.Now;
                db.TrainingCourses.Add(trainingCourse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdType = new SelectList(db.TrainingTypes, "Id", "Name", trainingCourse.IdType);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", trainingCourse.IdUser);
            return View(trainingCourse);
        }

        // GET: TrainingCourses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCourse trainingCourse = db.TrainingCourses.Find(id);
            if (trainingCourse == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdType = new SelectList(db.TrainingTypes, "Id", "Name", trainingCourse.IdType);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", trainingCourse.IdUser);
            return View(trainingCourse);
        }

        // POST: TrainingCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Description,Fromdate,Todate,Enable,Active,ResgistrationDeadline,Price,Number,IdType,IdUser,DateCreate")] TrainingCourse trainingCourse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingCourse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdType = new SelectList(db.TrainingTypes, "Id", "Name", trainingCourse.IdType);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", trainingCourse.IdUser);
            return View(trainingCourse);
        }

        // GET: TrainingCourses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCourse trainingCourse = db.TrainingCourses.Find(id);
            if (trainingCourse == null)
            {
                return HttpNotFound();
            }
            return View(trainingCourse);
        }

        // POST: TrainingCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TrainingCourse trainingCourse = db.TrainingCourses.Find(id);
            db.TrainingCourses.Remove(trainingCourse);
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
        #endregion
    }
}
