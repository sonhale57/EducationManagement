using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
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
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x=>x.Enable==true), "Id", "Name",IdBranch);
            var trainingCourses = db.TrainingCourses.Include(t => t.TrainingType).Include(t => t.User);
            return View(trainingCourses.ToList());
        }
        public ActionResult Loadlist(string searchString)
        {
            string str = "";
            string querybranch = "",querySearch="";
            int idbranch = int.Parse(CheckUsers.idBranch());
            if (!CheckUsers.CheckHQ())
            {
                querybranch = " and IdBranch=" + idbranch;
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                querySearch = " and tc.Name like N'%"+searchString+"%' ";
            }
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select tc.Id,tc.Code,tc.Name,tc.ResgistrationDeadline,tc.Fromdate,tc.Todate,tc.Number,cat.Name as NameCat,(select COUNT(IdEmployee) from RegistrationTraining where IdTraining=tc.Id " + querybranch+") as Soluongdk"
                            +" from TrainingCourse tc"
                            + " join TrainingType cat on cat.Id=tc.IdType"
                            + " where tc.Enable=1 "+querySearch;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count++;
                    string strStatus = "";
                    string strbtn = "", btn = "";
                    //Chưa bắt đầu
                    if (DateTime.Parse(reader["Fromdate"].ToString()) > DateTime.Now)
                    {
                        strStatus = "<span class='text-muted'>Chưa diễn ra</span>";
                        btn += "<li>" +
                                    "<a href=\"javascript:Load_dangky(" + reader["Id"] + ")\" class=\"dropdown-item\">" +
                                        "<i class=\"ti ti-receipt\"></i> Đăng ký tham gia" +
                                    "</a></li>" +
                                        "<li><a href=\"javascript:Load_paymentFee(" + reader["Id"] + ")\" class=\"dropdown-item\">" +
                                            "<i class=\"ti ti-credit-card\"></i> Thanh toán khóa đào tạo" +
                                        "</a></li>";
                    }
                    else
                    {
                        if (DateTime.Parse(reader["Todate"].ToString()) < DateTime.Now)
                        {
                            strStatus = "<span class='text-danger'>Đã kết thúc</span>";
                        }
                        else
                        {
                            strStatus = "<span class='text-success'>Đang diễn ra</span>";
                        }
                    }
                    if (CheckUsers.CheckHQ())
                    {
                        strbtn += "<a href='/trainingcourses/edit/" + reader["Id"] + "' class='me-1'><i class='ti ti-edit text-primary'></i></a>" +
                                        "<a href='javascript:Delete_trainning(" + reader["Id"] + ")' class='me-1'><i class='ti ti-trash text-danger'></i></a>";
                    }
                    strbtn += "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">"
                                        + "<i class=\"ti ti-dots-vertical\"></i>"
                                        + "</a>"
                                        + "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">"
                                        + btn
                                        + "<li>"
                                        + "<a href=\"javascript:View_registration(" + reader["Id"] + ")\" class=\"dropdown-item\">"
                                        + "<i class=\"ti ti-list\"></i> Danh sách đăng ký"
                                        + "</a>"
                                        + "</li>"
                                        + "</ul>";
                    str += "<tr>"
                            + "<td class='text-center align-content-center'>" + count + "</td>"
                            + "<td class='align-content-center'>" + reader["name"].ToString() + "<br/> <small class='fst-italic text-primary'>" + reader["NameCat"] + "</small></td>"
                            + "<td class='text-center align-content-center'>" + DateTime.Parse(reader["ResgistrationDeadline"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                            + "<td class='text-center align-content-center'>" + DateTime.Parse(reader["Fromdate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                            + "<td class='text-center align-content-center'>" + DateTime.Parse(reader["Todate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                            + "<td class='text-center align-content-center'><a href='javascript:View_registration(" + reader["Id"] + ")' class='fw-bolder'>" + reader["Soluongdk"].ToString() + "</a></td>"
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
                string btn = "";

                //Chưa bắt đầu
                if (train.Fromdate > DateTime.Now)
                {
                    if (ite.StatusJoin == true)
                    { 
                        btn += "<a class='btn btn-sm btn-outline-danger mt-1 ms-1' href='javascript:Cancel_reg(" + ite.IdTraining + "," + ite.IdEmployee + ")'><i class='ti ti-trash'></i> Hủy đăng ký</a>";
                        if (CheckUsers.CheckHQ())
                        {
                            btn += "<a class='btn btn-sm btn-outline-success mt-1 ms-1' href='javascript:Return_reg(" + ite.IdTraining + "," + ite.IdEmployee + ")'><i class='ti ti-arrow-back'></i> Hủy xác nhận</a>";
                        }
                    }
                    else
                    {
                       if (CheckUsers.CheckHQ())
                        {
                            btn += "<a class='btn btn-sm btn-success mt-1 ms-1' href='javascript:Confirm_reg(" + ite.IdTraining + "," + ite.IdEmployee + ")'><i class='ti ti-checks'></i> Xác nhận</a>";
                        }
                    }
                }
                else
                {
                    if (train.Todate < DateTime.Now)
                    {
                        // = "<span class='text-danger'>Đã kết thúc</span>";
                        if (CheckUsers.CheckHQ())
                        {
                            btn += "<a href='javascript:loadRatingModal(" + ite.IdTraining + "," + ite.IdEmployee + ",1)' class='btn btn-sm btn-success me-1'><i class='ti ti-checks'></i> Đánh giá</a>";
                        }
                        else
                        {
                            btn += "<a href='javascript:loadRatingModal(" + ite.IdTraining + "," + ite.IdEmployee + ",0)' class='btn btn-sm btn-success me-1'> Xem kết quả</a>";
                        }
                    }
                }
                var ResultTraining = db.TrainingResults
                                  .Where(x => x.IdTraining == ite.IdTraining && x.IdEmpoyee == ite.IdEmployee)
                                  .Select(x => x.Result ? "1" : "0")
                                  .FirstOrDefault() ?? "Chưa đánh giá";
                str += "<tr>"
                    + "<td class='text-center align-content-center'>" + count + "</td>"
                    + "<td class='text-left align-content-center'>" + ite.Employee.Name + "</td>"
                    + "<td class='text-center align-content-center'>" + ite.Employee.Phone + "</td>"
                    + "<td class='text-center align-content-center'>" + ite.Employee.Branch.Name + "</td>"
                    + "<td class='text-center align-content-center'>" + (ite.IsRegisteStay == true ? "Đăng ký" : "Không đăng ký") + "</td>"
                    + "<td class='text-center align-content-center'>" + (ite.StatusJoin == true ? "<span class='text-success'>Đã xác nhận</span>" : (ite.StatusPayment==true?"<span class='text-success'>Đã đóng phí</span>":"Chưa xác nhận")) + "</td>"
                    + "<td class='align-content-center text-center'>" + (ResultTraining == "1" ? "<span class='text-success'>Đã đạt</span>" : (ResultTraining == "0" ? "<span class='text-danger'>Chưa đạt</span>" : ResultTraining)) + "</td>"
                    + "<td class='align-content-center text-center'>" + btn + "</td>"
                    + "</tr>";
            }
            str += "<tr><td colspan=8 class='text-end'>Tổng tiền: <b>"+string.Format("{0:N0} đ",tongtien)+"</b></td></tr>";
            var item = new
            {
                str,
                name = train.Name,
                price =string.Format("{0:N0}",train.Price),
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
                return Json(new {status="ok",message="Đã đăng ký thành công!"}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "error", message = "Nhân sự này đã được đăng ký tham gia!" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Load_payment(int id)
        {
            string str = "",strStatus="",strDiscount="";
            int count_chuadongphi = 0;
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            var branch = db.Branches.Find(idbranch);
            int discount = branch.FreeTrainning==null?0:(int)branch.FreeTrainning;
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
                    count++;
                    str += "<tr>"
                        + "<td class='text-center'>" + count + "</td>"
                        + "<td>" + reader["NameEmployee"] + "</td>"
                        + "<td class='text-center'> " + reader["Phone"] + "</td>"
                        + "<td class='text-center'> " + reader["Email"] + "</td>"
                        + "<td class='text-center'>" + reader["NameBranch"] + "</td>"
                        + "<td class='text-center'>" + (reader["StatusPayment"].ToString() == "False" ? "<span class='text-danger'>Chưa đóng phí</span>" : "<span class='text-success'>Đã đóng phí</span>") + "</td>"
                        + "</tr>";
                    if (reader["StatusPayment"].ToString() == "False")
                    {
                        count_chuadongphi++;
                    }
                }
                reader.Close();
            }
            if (branch.FreeTrainning>0)
            {
                if (discount >= count_chuadongphi)
                {
                    count_chuadongphi = 0;
                    discount = count_chuadongphi;
                }
                else
                {
                    count_chuadongphi = count_chuadongphi - discount;
                }
                strDiscount = "<span class='text-muted fst-italic'>** Cơ sở đang còn ưu đãi phí đào tạo cho " + discount + " nhân sự.</span>";
            }
            decimal price = (decimal)tr.Price;
            
            decimal tong = price * count_chuadongphi;
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
            return Json(new { str,strtongtien,count_chuadongphi,discount,strphi,strName=tr.Name,strStatus,strDiscount}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Confirm_reg(int IdTrainning,int IdEmployee) { 
            var reg = db.RegistrationTrainings.FirstOrDefault(x=>x.IdTraining == IdTrainning&&x.IdEmployee==IdEmployee);
            if(reg== null)
            {
                return Json(new { status = "error", message = "Không tìm thấy nhân sự này!" },JsonRequestBehavior.AllowGet);
            }
            reg.StatusJoin = true;
            db.Entry(reg).State=EntityState.Modified;
            db.SaveChanges();
            return Json(new { status="ok",message="Đã xác nhận cho nhân sự thành công!" },JsonRequestBehavior.AllowGet);
        }
        public ActionResult Return_reg(int IdTrainning, int IdEmployee)
        {
            var reg = db.RegistrationTrainings.FirstOrDefault(x => x.IdTraining == IdTrainning && x.IdEmployee == IdEmployee);
            if (reg == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy nhân sự này!" }, JsonRequestBehavior.AllowGet);
            }
            reg.StatusJoin = false;
            db.Entry(reg).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { status = "ok", message = "Đã hủy xác nhận cho nhân sự thành công!" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Cancel_reg(int IdTrainning, int IdEmployee)
        {
            var reg = db.RegistrationTrainings.FirstOrDefault(x => x.IdTraining == IdTrainning && x.IdEmployee == IdEmployee);
            if (reg == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy nhân sự này!" }, JsonRequestBehavior.AllowGet);
            }
            db.RegistrationTrainings.Remove(reg);
            db.SaveChanges();
            return Json(new { status = "ok", message = "Đã hủy tham gia cho nhân sự thành công!" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete_trainning(int id)
        {
            var tr = db.TrainingCourses.Find(id);
            if (tr == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy khóa đào tạo này!" }, JsonRequestBehavior.AllowGet);
            }
            var reg = db.RegistrationTrainings.Any(x => x.IdTraining==id);
            if (reg == null)
            {
                db.TrainingCourses.Remove(tr);
                db.SaveChanges();
                return Json(new { status = "ok", message = "Đã xóa khóa đào tạo thành công!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                tr.Enable = false;
                tr.Active = false;
                db.Entry(tr).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "ok", message = "Khóa đào tạo đã được ẩn thành công!" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Submit_paymentFee(int? IdTrainingCourse, int? Number, int? Discount, decimal? Tongtien, string Description, HttpPostedFileBase file)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            int iduser = Convert.ToInt32(CheckUsers.iduser());
            string fileName = "";

            // Kiểm tra IdTrainingCourse không null trước khi xử lý
            if (IdTrainingCourse == null || Number == null)
            {
                return Json(new { status = "error", message = "Thông tin khóa đào tạo hoặc số lượng không hợp lệ!" }, JsonRequestBehavior.AllowGet);
            }
            var reg = db.RegistrationTrainings.Where(x => x.IdTraining == IdTrainingCourse && x.IdBranch == idbranch);
            if (reg == null)
            {
                return Json(new { status = "error",message="Cơ sở đang không giáo viên tham gia!" }, JsonRequestBehavior.AllowGet);
            }

            // Khởi tạo đối tượng TraningPayment
            var payment = new TraningPayment()
            {
                IdTraining = IdTrainingCourse.Value, // Sử dụng .Value vì đã kiểm tra không null
                Amount = Number.Value,               // Sử dụng .Value cho Number
                IdBranch = idbranch,
                IdUser = iduser,
                Updatetime = DateTime.Now,
                Number = Number.Value,
                Status = true,
                Descript = Description
            };

            // Nếu có file tải lên
            if (file != null && file.ContentLength > 0)
            {
                // Generate a unique file name
                fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";

                // Specify the path to save the file
                string _path = Path.Combine(Server.MapPath("~/Uploads/Images"), fileName);
                file.SaveAs(_path);

                // Lưu đường dẫn file vào đối tượng payment
                payment.Image = "/Uploads/Images/" + fileName;
            }

            // Thêm payment vào database
            db.TraningPayments.Add(payment);

            foreach (var item in reg)
            {
                item.StatusPayment = true;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();

            // Cập nhật transaction
            Update_Transaction(Tongtien, 0, IdTrainingCourse.Value, "/Uploads/Images/" + fileName);

            // Nếu có Discount, cập nhật chi nhánh
            if (Discount.HasValue && Discount.Value > 0)
            {
                var branch = db.Branches.Find(idbranch);
                if (branch != null)
                {
                    branch.FreeTrainning = branch.FreeTrainning - Discount.Value;
                    db.Entry(branch).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            // Trả về kết quả thành công
            return Json(new { status = "ok", message = "Đã thanh toán phí thành công!" }, JsonRequestBehavior.AllowGet);
        }

        string Getcode_transaction(bool type)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            string loai = "";
            int nextCode = 0;
            if (type == false)
            {
                loai = "PC";
                nextCode = db.Transactions.Where(x => x.IdBranch == idbranch && x.Type == false).Count() + 1;
            }
            else if (type == true)
            {
                loai = "PT";
                nextCode = db.Transactions.Where(x => x.IdBranch == idbranch && x.Type == true).Count() + 1;
            }
            string code = nextCode.ToString().PadLeft(5, '0');
            var cn = db.Branches.Find(idbranch);
            string str = cn.Code + loai + DateTime.Now.Year + code;

            return str;
        }
        public void Update_Transaction(decimal? Tongtien,decimal Discount,int IdTrain,string Image)
        {
            var trainingCourse = db.TrainingCourses.Find(IdTrain);
            int IdUser = int.Parse(CheckUsers.iduser());
            int IdBranch = int.Parse(CheckUsers.idBranch());
            var transaction = new Transaction()
            {
                Code = Getcode_transaction(false),
                Type = false,
                IdUser = IdUser,
                DateCreate = DateTime.Now,
                Amount = Tongtien,
                IdBranch = IdBranch,
                Status = true,
                Description = "Thanh toán phí đào tạo " +trainingCourse.Name,
                PaymentMethod = "chuyenkhoan",
                Name = "Head Quater",
                Image = Image,
                Discount = Discount,
                TotalAmount = Tongtien
            };
            db.Transactions.Add(transaction);
            db.SaveChanges();
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
                    str += "<td class='text-end'><a href='javascript:loadRatingModal(" + id + "," + ite.Employee.Id + ")' class='btn btn-sm btn-outline-success me-1'> Đánh giá</a></td>";
                }
                str += "</tr>";
            }
            string strinfo = "<p><b>Khóa đào tạo:</b> "+train.Name+"</p>"
                            +"<p><b>Loại:</b> " + train.TrainingType.Name + "</p>"
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
        public ActionResult LoadRating(int IdTrainingCourse, int IdEmployee)
        {
            var e = db.Employees.Find(IdEmployee) as Employee;
            var tr = db.TrainingCourses.Find(IdTrainingCourse);
            var result = db.TrainingResults.SingleOrDefault(x => x.IdTraining == IdTrainingCourse && x.IdEmpoyee == IdEmployee);

            var data = new
            {
                IdTrainingCourse = IdTrainingCourse,
                IdEmployee = IdEmployee,
                Result = result?.Result ?? false, // Mặc định là false nếu chưa có
                TotalScore = result?.TotalScore ?? 0, // Mặc định là 0 nếu chưa có
                NumberCertification = result? .NumberCertification ?? 0, // Mặc định chuỗi rỗng
                SuperbrainScore = result?.SuperbrainScore ?? "",
                BrandScore = result?.BrandScore ?? "",
                TeachScore = result?.TeachScore ?? "",
                SaleScore = result?.SaleScore ?? "",
                MindsetScore = result?.MindsetScore ?? "",
                SorobanScore = result?.SorobanScore ?? "",
                OnlineScore = result?.OnlineScore ?? "",
                CompleteScore = result?.CompleteScore ?? "",
                ParticipationScore = result?.ParticipationScore ?? "",
                DemeanorScore = result?.DemeanorScore ?? "",
                ProactiveScore = result?.ProactiveScore ?? "",
                Description = result?.Description ?? ""
            };

            return Json(new { status = "ok",nameEmployee=e.Name,nameCourse=tr.Name, data }, JsonRequestBehavior.AllowGet);;
        }
        [HttpPost]
        public ActionResult Submit_rating(int IdTrainingCourse, int IdEmployee, bool Result, int TotalScore, int NumberCertification,
                                  string SuperbrainScore, string BrandScore, string TeachScore, string SaleScore,
                                  string MindsetScore, string SorobanScore, string OnlineScore, string CompleteScore,
                                  string ParticipationScore, string DemeanorScore, string ProactiveScore, string Description)
        {
            string status = "ok";

            // Tìm bản ghi đánh giá của giáo viên trong khóa đào tạo
            var type = db.TrainingResults.SingleOrDefault(x => x.IdTraining == IdTrainingCourse && x.IdEmpoyee == IdEmployee);
            var e = db.Employees.Find(IdEmployee);  // Tìm giáo viên
            var reg = db.RegistrationTrainings.SingleOrDefault(x => x.IdTraining == IdTrainingCourse && x.IdEmployee == IdEmployee);  // Tìm bản ghi đăng ký khóa học

            if (type == null)
            {
                // Nếu chưa có đánh giá, thêm mới
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
                    IdUser = Convert.ToInt32(CheckUsers.iduser())  // Lấy id người dùng hiện tại
                };

                db.TrainingResults.Add(rate);  // Thêm vào cơ sở dữ liệu
                reg.IsPass = Result;
                reg.Result = TotalScore;
                db.Entry(reg).State = EntityState.Modified;
                db.SaveChanges();

                // Nếu kết quả đạt, cập nhật thông tin chính thức của giáo viên
                if (Result == true)
                {
                    var user = db.Users.FirstOrDefault(x => x.IdEmployee == e.Id);
                    // Kiểm tra xem user có tồn tại không
                    if (user != null && e != null)
                    {
                        // Cập nhật thông tin nhân sự và người dùng
                        e.IsOfficial = true;
                        e.CertificateNumber = NumberCertification;
                        user.Expire = DateTime.Now.AddMonths(6);

                        // Đánh dấu rằng thông tin của user và nhân sự đã bị thay đổi
                        db.Entry(user).State = EntityState.Modified;
                        db.Entry(e).State = EntityState.Modified;

                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();
                    }
                    else
                    {
                        Create_Account(IdEmployee);  // Tạo tài khoản cho giáo viên
                    }
                }
            }
            else
            {
                // Nếu đã có đánh giá, cập nhật kết quả mới
                type.Result = Result;
                type.TotalScore = TotalScore;
                type.NumberCertification = NumberCertification;
                type.SaleScore = SaleScore;
                type.SorobanScore = SorobanScore;
                type.SuperbrainScore = SuperbrainScore;
                type.BrandScore = BrandScore;
                type.TeachScore = TeachScore;
                type.MindsetScore = MindsetScore;
                type.CompleteScore = CompleteScore;
                type.DemeanorScore = DemeanorScore;
                type.ParticipationScore = ParticipationScore;
                type.ProactiveScore = ProactiveScore;
                type.OnlineScore = OnlineScore;
                type.Description = Description;

                db.Entry(type).State = EntityState.Modified;
                db.SaveChanges();

                reg.IsPass = Result;
                reg.Result = TotalScore;
                db.Entry(reg).State = EntityState.Modified;
                db.SaveChanges();

                if (Result == true && e.IsOfficial == false)
                {
                    e.IsOfficial = true;
                    e.CertificateNumber = NumberCertification;
                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();
                }
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
        public ActionResult Statistics()
        {
            ViewBag.IdTrainingCourse = new SelectList(db.TrainingCourses.Where(x=>x.Enable==true),"Id","Name");
            return View();
        }
        public ActionResult Loadlist_statistic(int IdTrainingCourse)
        {
            var list = from tc in db.TrainingCourses
                       join reg in db.RegistrationTrainings on tc.Id equals reg.IdTraining
                       join e in db.Employees on reg.IdEmployee equals e.Id
                       where reg.IdTraining ==IdTrainingCourse
                       select new { 
                            NameCourse = tc.Name,
                            NameEmployee = e.Name,
                            Phone = e.Phone,
                           // Kiểm tra null cho TrainingResults
                           ResultTraining = db.TrainingResults
                                  .Where(x => x.IdTraining == tc.Id && x.IdEmpoyee == e.Id)
                                  .Select(x => x.Result ? "1" : "0")
                                  .FirstOrDefault() ?? "Chưa đánh giá",
                           NumberCertificate = db.TrainingResults
                                     .Where(x => x.IdTraining == tc.Id && x.IdEmpoyee == e.Id)
                                     .Select(x => x.NumberCertification)
                                     .FirstOrDefault() ?? null,

                           Exp = db.TrainingResults
                                  .Where(x => x.IdTraining == tc.Id && x.IdEmpoyee == e.Id)
                                  .Select(x => x.DateCreate)
                                  .FirstOrDefault() ?? null,
                           NameBranch = e.Branch.Name
                       };
            string str = "";int stt = 0;
            if(!list.Any())
            {
                str = "<tr><td colspan=7 class='text-center'>Chưa có dữ liệu đăng ký khóa đào tạo</td></tr>";
            }
            foreach(var res in list)
            {
                stt++;
                str += "<tr>"
                    +"<td class='text-center'>"+stt+"</td>"
                    +"<td class='text-start'>"+res.NameEmployee+"</td>"
                    +"<td class='text-center'>"+res.Phone+"</td>"
                    +"<td class='text-center'>"+(res.ResultTraining =="1" ? "<span class='text-success'>Đã đạt</span>":(res.ResultTraining=="0"?"<span class='text-danger'>Chưa đạt</span>":res.ResultTraining))+"</td>"
                    +"<td class='text-center'>"+res.NumberCertificate + "</td>"
                    +"<td class='text-center'>"+(res.Exp.HasValue ? res.Exp.Value.AddMonths(6).ToString("dd/MM/yyyy"):"-") + "</td>"
                    +"<td class='text-start'>"+res.NameBranch + "</td>"
                    + "</tr>";
            }
            return Json(new {str},JsonRequestBehavior.AllowGet);
        }
        public  ActionResult PaymentStatistics()
        {
            ViewBag.IdTrainingCourse = new SelectList(db.TrainingCourses.Where(x=>x.Enable==true),"Id","Name");
            return View();
        }
        public ActionResult Loadlist_paymentstatistic(int IdTraining)
        {
            var registrations =  db.RegistrationTrainings
            .Where(rt => rt.IdTraining == IdTraining) // Lọc theo IdTraining
            .GroupBy(rt => rt.IdBranch) // Nhóm theo IdBranch
            .Select(g => new
            {
                IdBranch = g.Key,
                Name = db.TrainingCourses.FirstOrDefault(x=>x.Id==IdTraining).Name,
                BranchName = db.Branches.FirstOrDefault(b => b.Id == g.Key).Name??"Không xác định",
                RegistrationCount = g.Count(),
                SupportCount = db.TraningPayments.FirstOrDefault(x=>x.IdTraining==IdTraining && x.IdBranch ==g.Key).NumberSupport ?? 0,
                PaymentCount = db.TraningPayments.Where(x=>x.IdTraining ==IdTraining && x.IdBranch ==g.Key).Sum(x=>x.Number) ?? 0,
                ConfirmCount = db.RegistrationTrainings.Where(x=>x.IdTraining==IdTraining && x.IdBranch ==g.Key).Count(x=>x.StatusJoin==true)
            })
            .ToList();
            string str = "";int stt = 1;
            if (registrations == null)
            {
                str = "<tr><td colspan=7 class='text-center'> Không thấy dữ liệu đăng ký khóa đào tạo</td></tr>";
            }
            foreach(var item in registrations)
            {
                stt++;
                str += "<tr>" +
                    "<td class='text-center'>"+stt+"</td>" +
                    "<td class='text-center'>"+item.Name+"</td>" +
                    "<td class='text-center'>"+item.BranchName+"</td>" +
                    "<td class='text-center'>"+item.RegistrationCount+"</td>" +
                    "<td class='text-center'>"+item.SupportCount+"</td>" +
                    "<td class='text-center'>"+item.PaymentCount+"</td>" +
                    "<td class='text-center'>" +item.ConfirmCount + "</td>" +
                    "</tr>";
            }
            return Json(new {str},JsonRequestBehavior.AllowGet);
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
