using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using Google.Protobuf.WellKnownTypes;
using Mysqlx.Crud;
using PagedList.Mvc;
using PagedList;
using SuperbrainManagement.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace SuperbrainManagement.Controllers
{
    public class StudentsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Students
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string idBranch)
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

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var students = db.Students.Include(x=>x.User).ToList();

            if (!string.IsNullOrEmpty(idBranch))
            {
                students = students.Where(x => x.IdBranch == int.Parse(idBranch)).ToList();
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Name).ToList();
                    break;
                case "date":
                    students = students.OrderBy(s => s.Id).ToList();
                    break;
                case "name":
                    students = students.OrderBy(s => s.Name).ToList();
                    break;
                default:
                    students = students.OrderByDescending(s => s.Id).ToList();
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);


            var pagedData = students.ToPagedList(pageNumber, pageSize);

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

        public ActionResult AddCourseProgramOfStudents(int IdStudent)
        {
            Student student = Connect.SelectSingle<Student>("select * from Student where Id='" + IdStudent + "'");
            Session["infoUser"] = student;
            return View(student);
        }
       
        public ActionResult RegistrationPrints()
        {
            Student student = Session["infoUser"] as Student;
            int idregistration = Convert.ToInt32(Session["IdRegistration"]);
           
            Registration registration = Connect.SelectSingle<Registration>("Select * from Registration where Id = '" + idregistration + "'");
            DataTable datatable = Connect.SelectAll("select course.Name,course.Price,rescourse.TotalAmount from Registration  res\r\ninner join RegistrationCourse rescourse\r\non rescourse.IdRegistration = res.Id\r\ninner join Course course \r\non course.Id = rescourse.IdCourse\r\nwhere res.Id = '" + idregistration+"'");
            var model = new
            {
                DataTableLoad = datatable
            };
            if (student != null)
            {
                Session["Name"] = student.Name;
                Session["code"] = registration.Code;
                Session["Datecreate"] = registration.DateCreate;
                Session["totalamount"] = registration.TotalAmount;
                Session["datatable"] = datatable;
            }
          
            return View(model);
        }
        [HttpPost] // Use POST for actions that modify data
        public ActionResult Deletes(int IdCourse, int IdRegistration)
        {
            try
            {
                var deleteItem = db.RegistrationCourses.FirstOrDefault(x => x.IdCourse == IdCourse && x.IdRegistration == IdRegistration);
                if (deleteItem != null)
                {
                    db.RegistrationCourses.Remove(deleteItem);
                    db.SaveChanges();
                    return Json(deleteItem.IdRegistration);

                }
                else
                {
                    return Json(new { success = false, message = "Item not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Item not found" });
            }
        }
        public string GenerateCode()
        {
            string prefix = "DK_HQ";

            // Generate a random number
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999); // Adjust range as needed

            // Generate the new code
            string newCode = $"{prefix}_{randomNumber}";

            return newCode;
        }
   

        public ActionResult getData(string IdRegistration)
        {
            DataTable dataTableCourse = Connect.SelectAll("SELECT cour.Name AS NameCourse, rescourse.IdCourse, res.Id, rescourse.Price, pro.Name AS NameProgram, res.Amount, rescourse.TotalAmount, res.Code, res.DateCreate, res.Discount FROM Registration res INNER JOIN RegistrationCourse rescourse ON rescourse.IdRegistration = res.Id INNER JOIN Course cour ON cour.Id = rescourse.IdCourse INNER JOIN Program pro ON pro.Id = cour.IdProgram WHERE res.Id = '" + IdRegistration + "'");
            DataTable dataTableProduct = Connect.SelectAll("SELECT resproduct.Discount,pro.Name, resproduct.Price, resproduct.TotalAmount, res.Id, resproduct.IdProduct FROM Registration res INNER JOIN RegistrationProduct resproduct ON res.Id = resproduct.IdRegistration INNER JOIN Product pro ON pro.Id = resproduct.IdProduct WHERE res.Id = '" + IdRegistration + "'");
            DataTable dataTableOther = Connect.SelectAll("select revenue.Name,revenue.Price,revenue.Discount,other.Amount,other.TotalAmount,res.Id,other.IdReference from Registration res inner join RegistrationOther other on other.IdRegistration = res.Id inner join RevenueReference revenue on revenue.Id = other.IdReference where other.IdRegistration = '"+IdRegistration+"'");
            Registration registration = Connect.SelectSingle<Registration>("SELECT * FROM Registration WHERE Id = '" + IdRegistration + "'");

            var data = new StringBuilder();
            var totalAmount = 0;
            var i = 0;

            List<DataRow> allRows = new List<DataRow>();
            allRows.AddRange(dataTableCourse.AsEnumerable());
            allRows.AddRange(dataTableProduct.AsEnumerable());
            allRows.AddRange(dataTableOther.AsEnumerable());

            foreach (DataRow row in allRows)
            {
                i++;
                string amountString = "";
                string discount = "";
                string name = "";
                string total = "";
                var idobject = 0;
                if (row.Table == dataTableCourse)
                {
                    amountString = string.Format("{0:N0} VND", row["TotalAmount"]);
                    discount = string.Format("{0:N0} VND", row["Discount"]);
                    name = row["NameProgram"].ToString() + "<hr>" + row["NameCourse"].ToString();
                    idobject = Convert.ToInt32(row["IdCourse"]);
                    total = amountString;
                }
                else if (row.Table == dataTableProduct)
                {
                    amountString = string.Format("{0:N0} VND", row["Price"]);
                    discount = string.Format("{0:N0} VND", row["Discount"]);
                    total = string.Format("{0:N0} VND", row["TotalAmount"]);
                    idobject = Convert.ToInt32(row["IdProduct"]);
                    name = row["Name"].ToString();
                }else if(row.Table == dataTableOther)
                {
                    amountString = string.Format("{0:N0} VND", row["Price"]);
                    discount = string.Format("{0:N0} VND", row["Discount"]);
                    total = string.Format("{0:N0} VND", row["TotalAmount"]);
                    idobject = Convert.ToInt32(row["IdReference"]);
                    name = row["Name"].ToString();
                }

                var newRow = "<tr>" +
                    "<td style='text-align:center;'>" + i + "</td>" +
                    "<td style='text-align:left;'>" + name + "</td>" +
                    "<td style='text-align:center;'>1</td>" +
                    "<td style='text-align:right;'>" + amountString + "</td>" +
                    "<td style='text-align:center;'>" + discount + "</td>" +
                    "<td style='text-align:right;'>" + total + "</td>" +
                    "<td style='text-align:center;'>" +
                    "<a href='#' class='btn btn-sm btn-danger ti-trash' onclick=\"deleteitem('" + idobject + "','" + row["Id"] + "')\" data-courseid='" + idobject + "' data-registrationid='" + row["Id"] + "'>" +
                    "<i class='bx bx-trash-alt font-size-18'></i>" +
                    "</a>" +
                    "</td>" +
                    "</tr>";

                data.Append(newRow);

                if (row.Table == dataTableCourse)
                {
                    totalAmount += Convert.ToInt32(row["TotalAmount"]);
                }
                else if (row.Table == dataTableProduct)
                {
                    totalAmount += Convert.ToInt32(row["TotalAmount"]);
                }else if(row.Table == dataTableOther)
                {
                    totalAmount += Convert.ToInt32(row["TotalAmount"]);
                }
            }

            var result = new
            {
                datalist = data.ToString(),
                TotalAmount = totalAmount,
                DateCreate = Convert.ToDateTime(registration.DateCreate).ToString("dd/MM/yyyy"),
                idRegistrations = registration.Id,
                Code = registration.Code
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class listProduct
        {
           public int isChecked {  get; set; }
           public int idpro {  get; set; }
        }
    
        [HttpPost] 
        public ActionResult SaveRegistration(int? IdRegistration,int type, int? IdObject,int? price,int? totalamount,int? amount,string Description,int? Discount,List<listProduct> listProduct)
        {
            //get Iduser and Idstudents
            MD5Hash md5 = new MD5Hash();
            string iduser = System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString();
            iduser = md5.Decrypt(iduser.ToString());
            Student student = Session["infoUser"] as Student;
            //check registration 
            Registration registration = new Registration();
            registration = Connect.SelectSingle<Registration>("select * from Registration where Id='" + IdRegistration + "'");
             // Create new Registration
            if(registration == null)
            {
                Registration NewRegistration = new Registration();
                NewRegistration.IdStudent = student.Id;
                NewRegistration.IdUser = Convert.ToInt32(iduser);
                NewRegistration.Amount = amount;
                NewRegistration.Code = GenerateCode();
                NewRegistration.TotalAmount = totalamount;
                NewRegistration.DateCreate = DateTime.Now;
                NewRegistration.Status = true;
                NewRegistration.Enable = true;
                NewRegistration.Description = Description;
                NewRegistration.IdBranch = student.IdBranch;
                db.Registrations.Add(NewRegistration);
                db.SaveChanges();
                registration = NewRegistration;
                Session["IdRegistration"] = registration.Id;
            }
            //Check type luồng Product,Course,Order
            switch (type)
            {
                case 1:
                    RegistrationProduct registrationProduct = Connect.SelectSingle<RegistrationProduct>("select * from RegistrationProduct where IdProduct = '"+IdObject+"'  and IdRegistration = '"+registration.Id+"'");
                    if(registrationProduct != null)
                    {
                        return Json(null);

                    }else
                    {
                        RegistrationProduct NewregistrationProduct = new RegistrationProduct();
                        NewregistrationProduct.IdRegistration = registration.Id;
                        NewregistrationProduct.IdProduct = Convert.ToInt32(IdObject);
                        NewregistrationProduct.Status = true;
                        NewregistrationProduct.Price = price;
                        NewregistrationProduct.Amount = amount;
                        NewregistrationProduct.TotalAmount = totalamount;
                        NewregistrationProduct.Discount = Discount;
                        db.RegistrationProducts.Add(NewregistrationProduct);
                        db.SaveChanges();
                        return Json(NewregistrationProduct.IdRegistration);
                    }
                case 2:
                    RegistrationCourse registrationCourse = Connect.SelectSingle<RegistrationCourse>("select * from RegistrationCourse  where IdRegistration = '"+registration.Id+"'  AND IdCourse = '"+IdObject+"'"); 
                    if(registrationCourse != null)
                    {
                        return Json(null);
                    }else
                    {
                        RegistrationCourse NewregistrationCourse = new RegistrationCourse();
                        NewregistrationCourse.Status = true;
                        NewregistrationCourse.Price = price;
                        NewregistrationCourse.Amount = amount;
                        NewregistrationCourse.TotalAmount = totalamount;
                        NewregistrationCourse.IdRegistration = registration.Id;
                        NewregistrationCourse.IdCourse  = Convert.ToInt32(IdObject);
                        NewregistrationCourse.Discount = Discount;
                        db.RegistrationCourses.Add(NewregistrationCourse);
                        db.SaveChanges();
                        //if(listProduct.Count > 0)
                        //{
                        //    foreach(var itempro in listProduct)
                        //    {
                        //        if(itempro.isChecked == 1)
                        //        {
                        //            Product product = Connect.SelectSingle<Product>("select * from Product where Id='" + itempro.idpro + "'");
                        //            ProductCourse NewproductCourse = new ProductCourse();
                        //            NewproductCourse.Status = true;
                        //            NewproductCourse.IdCourse = IdObject;
                        //            NewproductCourse.IdProduct = itempro.idpro;
                        //            NewproductCourse.Amount = Convert.ToInt32(product.Price);
                        //            NewproductCourse.DateCreate = DateTime.Now;
                        //            db.ProductCourses.Add(NewproductCourse);
                        //            db.SaveChanges();

                        //        }
                             
                        //    }
                           
                        //}
                        return Json(NewregistrationCourse.IdRegistration);
                    }
                case 3:
                    RegistrationOther RegistrationOther = Connect.SelectSingle<RegistrationOther>("select * from RegistrationOther where IdRegistration = '"+registration.Id+"' and IdReference = '"+IdObject+"'");
                    if(RegistrationOther != null)
                    {
                        return Json(null);
                    }else
                    {
                        RegistrationOther NewRegistrationOther = new RegistrationOther();
                        NewRegistrationOther.IdRegistration = registration.Id;
                        NewRegistrationOther.IdReference = Convert.ToInt32(IdObject);
                        NewRegistrationOther.Discount = Discount;
                        NewRegistrationOther.Status = "1";
                        NewRegistrationOther.Price = price;
                        NewRegistrationOther.Amount = amount;
                        NewRegistrationOther.TotalAmount = totalamount;
                        db.RegistrationOthers.Add(NewRegistrationOther);
                        db.SaveChanges();
                        return Json(NewRegistrationOther.IdRegistration);
                    }
                default:
                    return Json(null);
                    break;
            }

        }
        public ActionResult getDataComboxOther(int? IdOther)
        {
            List<RevenueReference> revenueReferences = Connect.Select<RevenueReference>("select * from RevenueReference");

            var str = "";
            var number = 0;
            var totalamount = 0;
            if (IdOther != 0)
            {
                RevenueReference product = Connect.SelectSingle<RevenueReference>("select * from RevenueReference where Id = '" + IdOther + "'");
                totalamount = Convert.ToInt32(product.Price);
                
            }
            else
            {
                foreach (var items in revenueReferences)
                {
                    str += "<option value='" + items.Id + "' data-name='" + items.Name + "'>" + items.Name + "</option>";
                }
                totalamount = Convert.ToInt32(revenueReferences[0].Price);
                
            }

            var item = new
            {
                str,
                totalamount
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_profile(int? idStudent) 
        {  
            List<object> dskhoadangky = new List<object>();
            string str = "";
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            int count = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select s.Id,re.Id as IdRegistration,re.Code as Code,rec.IdCourse,course.Name as NameCourse ,re.DateCreate,rec.Status, rec.TotalAmount,rec.StatusExchangeCourse,rec.StatusJoinClass,rec.StatusExtend,rec.StatusReserve from Student s inner join registration re on re.IdStudent= s.id inner join RegistrationCourse rec on rec.IdRegistration = re.id, Course course where course.id= rec.IdCourse and s.id=" + idStudent;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var item = new
                    {
                        Code = reader["Code"].ToString(),
                        IdCourse = reader["IdCourse"].ToString(),
                        NameCourse = reader["NameCourse"].ToString(),
                        Price = reader["TotalAmount"].ToString(),
                        DateCreate = reader["DateCreate"].ToString(),
                        Status = reader["status"],
                        StatusExchangeCourse = reader["statusexchangecourse"].ToString(),
                        StatusJoinClass= reader["StatusJoinClass"].ToString(),
                        StatusExtend = reader["StatusExtend"].ToString()
                    };
                    count++;

                    str +="<tr>"
                        +"<td>"+count+"</td>"
                        +"<td>"+item.Code+"</td>"
                        +"<td>"+item.NameCourse+"</td>"
                        +"<td>"+item.DateCreate+"</td>"
                        +"<td>"+ item.Price + "</td>"
                        +"<td class='text-center'>"+ (Convert.ToBoolean(reader["Status"]) ? "<i class='ti ti-circle-check text-success'></i>" : "Chưa thanh toán") + "</td>"
                        +"<td class='text-center'>"+ (Convert.ToBoolean(reader["StatusJoinClass"]) ? "" : "Chờ xét lớp") + "</td>"
                        + "<td class='text-end'>"
                        + "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">"
                        +"<i class=\"ti ti-dots-vertical\"></i>"
                        +"</a>"
                        +"<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">"
                        +"<li><a class=\"dropdown-item\" href=''><i class=\"ti ti-eye-check\"></i> Xét vào lớp</a></li>"
                        +"</ul>"
                        + "</td>"
                        + "</tr>";
                    dskhoadangky.Add(item);
                }
                reader.Close();
            }
            var data = new { dskhoadangky,str};
            return Json(data,JsonRequestBehavior.AllowGet); 
        }
        public ActionResult getDataComboxProduct(int? idproduct)
        {
            List<Product> products = Connect.Select<Product>("select * from Product");

            var str = "";
            var strTable = "";
            var number = 0;
            var totalamount = 0;
            if(idproduct != 0)
            {
               Product product = Connect.SelectSingle<Product>("select * from Product where Id = '"+idproduct+"'");
                totalamount = Convert.ToInt32(product.Price);
                number = Convert.ToInt32(product.NumberOfPackage);
              
            }
            else
            {
                foreach (var items in products)
                {

                    str += "<option value='" + items.Id + "' data-name='" + items.Name + "' data-inventory='" + ProductReceiptionDetailsController.GetInventory(items.Id) + "'>" + items.Name + "</option>";
                }
               
                totalamount = Convert.ToInt32(products[0].Price);
                number = Convert.ToInt32(products[0].NumberOfPackage);
            }
           
            var item = new
            {
                str,
                totalamount,
                number
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDataCombobox(int? IdProgram,int? IdCourse,int? type)
        {
            int idBranch = int.Parse(CheckUsers.idBranch());
            var item = new
            {
                courses = new List<Course>(),
                programs = new List<Program>()
            };

            // Lấy danh sách tất cả các chương trình
            List<Program> programs = Connect.Select<Program>("SELECT * FROM Program");
            List<Promotion> promotions = Connect.Select<Promotion>("Select * from Promotion where idBranch='"+idBranch+"' and todate>getdate() and active='1'");
            List<Product> products = Connect.Select<Product>("select * from Product");
            var priceCourse = 0;
            // Nếu không có IdProgram được chọn, lấy danh sách khóa học của chương trình đầu tiên
            if (IdProgram == 0 && type == 1)
            {
                // Lấy danh sách khóa học của chương trình đầu tiên
                    if (programs.Count > 0)
                    {
                        List<Course> courses = Connect.Select<Course>("SELECT * FROM Course WHERE IdProgram = '" + programs[0].Id + "'");
                        priceCourse = Convert.ToInt32(courses[0].Price);
                        item.programs.AddRange(programs);
                        item.courses.AddRange(courses);
                    }
                
            }
            else if(IdProgram != 0 && type == 1)
            {
           
                    // Nếu có IdProgram được chọn, lấy danh sách khóa học của chương trình tương ứng
                    List<Course> courses = Connect.Select<Course>("SELECT * FROM Course WHERE IdProgram = '" + IdProgram + "'");
                    priceCourse = Convert.ToInt32(courses[0].Price);
                    item.courses.AddRange(courses);
                
            
            }else if (type == 2) 
            {
                if (IdCourse != 0)
                {
                    Course course = Connect.SelectSingle<Course>("select * from Course where Id = '" + IdCourse + "'");
                    priceCourse = Convert.ToInt32(course.Price);
                }
            }
          
            // Tạo chuỗi HTML cho các dropdown chương trình
            var strpro = "";
            if (item.programs.Count > 0)
            {
                foreach (var pro in item.programs)
                {
                    strpro += "<option value='" + pro.Id + "' data-name='" + pro.Name + "'>" + pro.Name + "</option>";
                }
            }
            var strcour = "";
            foreach (var course in item.courses)
            {
                strcour += "<option value='" + course.Id + "' data-name='" + course.Name + "'>" + course.Name + "</option>";
            }
            var strpromotion = "";
            foreach(var promotion in promotions)
            {
                strpromotion += "<option value='" + promotion.Id + "' data-name='" + promotion.Name + "'>" + promotion.Name + "</option>";
            }
            var strTable = "";
            foreach (var itemtable in products)
            {
                strTable += "<tr class='listproduct'>"
                      + "<th><input class='idproduct' onclick='updateCheckboxValue(this)'  type='checkbox' value=''/><input class='idpro' value='"+itemtable.Id+"' hidden/></th>"
                    + "<th>" + itemtable.Name + "</th>"
                     + "<th>" + itemtable.Price + "</th>"
                       + "<th>" + itemtable.Price + "</th>"
                    + "</tr>";
            }
            // Tạo đối tượng để chứa chuỗi HTML của dropdown chương trình và khóa học
            var strcombobox = new
            {
                strpro,
                strcour,
                strTable,
                strpromotion,
                priceCourse =  priceCourse,
                type = type
                // Đây là để giữ chỗ, bạn có thể thêm logic để tạo chuỗi HTML cho dropdown khóa học tương ứng
            };

            return Json(strcombobox, JsonRequestBehavior.AllowGet);
        }



        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo");
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Image,Code,DateOfBirth,Sex,Username,Password,Enable,School,Class,Description,ParentName,Phone,Email,ParentDateOfBirth,City,District,Address,Relationship,Job,Facebook,Hopeful,Known,IdMKT,IdBranch,PowerScore,Balance,Presenter,Status,Power,StatusStudy")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", student.IdBranch);
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code", student.IdBranch);
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", student.IdBranch);
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code", student.IdBranch);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Image,Code,DateOfBirth,Sex,Username,Password,Enable,School,Class,Description,ParentName,Phone,Email,ParentDateOfBirth,City,District,Address,Relationship,Job,Facebook,Hopeful,Known,IdMKT,IdBranch,PowerScore,Balance,Presenter,Status,Power,StatusStudy")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", student.IdBranch);
            ViewBag.IdBranch = new SelectList(db.MKTCampaigns, "Id", "Code", student.IdBranch);
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
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
