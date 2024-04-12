using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace SuperbrainManagement.Models
{
    public partial class ModelDbContext : DbContext
    {
        public ModelDbContext()
            : base("name=ModelDbContext")
        {
        }

        public virtual DbSet<AchievementEmployee> AchievementEmployees { get; set; }
        public virtual DbSet<ApproveLeave> ApproveLeaves { get; set; }
        public virtual DbSet<AssignTask> AssignTasks { get; set; }
        public virtual DbSet<BankAccountOfBranch> BankAccountOfBranches { get; set; }
        public virtual DbSet<Branch> Branches { get; set; }
        public virtual DbSet<BranchCommission> BranchCommissions { get; set; }
        public virtual DbSet<BranchGroup> BranchGroups { get; set; }
        public virtual DbSet<CertificateCategory> CertificateCategories { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Competition> Competitions { get; set; }
        public virtual DbSet<CompetitionRank> CompetitionRanks { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseBranch> CourseBranches { get; set; }
        public virtual DbSet<CourseOnline> CourseOnlines { get; set; }
        public virtual DbSet<DataPaymentOnline> DataPaymentOnlines { get; set; }
        public virtual DbSet<DataSuggest> DataSuggests { get; set; }
        public virtual DbSet<Debit> Debits { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeCheckin> EmployeeCheckins { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventCheckin> EventCheckins { get; set; }
        public virtual DbSet<EventJoin> EventJoins { get; set; }
        public virtual DbSet<FAQ> FAQs { get; set; }
        public virtual DbSet<Feed> Feeds { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<FileType> FileTypes { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<GroupChat> GroupChats { get; set; }
        public virtual DbSet<Inform> Informs { get; set; }
        public virtual DbSet<JoinCourseOnlineLog> JoinCourseOnlineLogs { get; set; }
        public virtual DbSet<LessonCourseOnline> LessonCourseOnlines { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<LoginLog> LoginLogs { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MessageGroup> MessageGroups { get; set; }
        public virtual DbSet<MessageStudent> MessageStudents { get; set; }
        public virtual DbSet<MKTCampaign> MKTCampaigns { get; set; }
        public virtual DbSet<NewsFeed> NewsFeeds { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderStatu> OrderStatus { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionCategory> PermissionCategories { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<PProductRecieptionDetail> PProductRecieptionDetails { get; set; }
        public virtual DbSet<PrivateProduct> PrivateProducts { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductCourse> ProductCourses { get; set; }
        public virtual DbSet<ProductPromotion> ProductPromotions { get; set; }
        public virtual DbSet<ProductReceiptionDetail> ProductReceiptionDetails { get; set; }
        public virtual DbSet<Program> Programs { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<ReactionNewsFeed> ReactionNewsFeeds { get; set; }
        public virtual DbSet<Registration> Registrations { get; set; }
        public virtual DbSet<RegistrationCourse> RegistrationCourses { get; set; }
        public virtual DbSet<RegistrationOther> RegistrationOthers { get; set; }
        public virtual DbSet<RegistrationProduct> RegistrationProducts { get; set; }
        public virtual DbSet<RegistrationTraining> RegistrationTrainings { get; set; }
        public virtual DbSet<ReportForm> ReportForms { get; set; }
        public virtual DbSet<ResultCourse> ResultCourses { get; set; }
        public virtual DbSet<RevenueReference> RevenueReferences { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<SalaryEmployee> SalaryEmployees { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Schoolarship> Schoolarships { get; set; }
        public virtual DbSet<SchoolarshipForm> SchoolarshipForms { get; set; }
        public virtual DbSet<Shipping> Shippings { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentAdvise> StudentAdvises { get; set; }
        public virtual DbSet<StudentCertification> StudentCertifications { get; set; }
        public virtual DbSet<StudentCheckin> StudentCheckins { get; set; }
        public virtual DbSet<StudentInputTest> StudentInputTests { get; set; }
        public virtual DbSet<StudentJoinClass> StudentJoinClasses { get; set; }
        public virtual DbSet<StudentVoucher> StudentVouchers { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TrainingAttendance> TrainingAttendances { get; set; }
        public virtual DbSet<TrainingCourse> TrainingCourses { get; set; }
        public virtual DbSet<TrainingDocument> TrainingDocuments { get; set; }
        public virtual DbSet<TrainingResult> TrainingResults { get; set; }
        public virtual DbSet<TrainingType> TrainingTypes { get; set; }
        public virtual DbSet<TraningPayment> TraningPayments { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAvatar> UserAvatars { get; set; }
        public virtual DbSet<UserJoinCourseOnline> UserJoinCourseOnlines { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }
        public virtual DbSet<UserMood> UserMoods { get; set; }
        public virtual DbSet<UserPermission> UserPermissions { get; set; }
        public virtual DbSet<VacationSchedule> VacationSchedules { get; set; }
        public virtual DbSet<VideoCourseOnline> VideoCourseOnlines { get; set; }
        public virtual DbSet<WarehouseReceiption> WarehouseReceiptions { get; set; }
        public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<Hash> Hashes { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobParameter> JobParameters { get; set; }
        public virtual DbSet<JobQueue> JobQueues { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<Schema> Schemata { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<State> States { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.ApproveLeaves)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.BankAccountOfBranches)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.BranchCommissions)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.CertificateCategories)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Classes)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Competitions)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Coupons)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.CourseBranches)
                .WithRequired(e => e.Branch)
                .HasForeignKey(e => e.IdBranch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.DataSuggests)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Debits)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Employees)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Events)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Feedbacks)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Files)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Folders)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.MessageStudents)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.MKTCampaigns)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Notifications)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.PrivateProducts)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Projects)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Promotions)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Registrations)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.RevenueReferences)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Rooms)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.SalaryEmployees)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Students)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Tasks)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.TraningPayments)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Transactions)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.Users)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.VacationSchedules)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<Branch>()
                .HasMany(e => e.WarehouseReceiptions)
                .WithOptional(e => e.Branch)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<BranchCommission>()
                .Property(e => e.Money)
                .HasPrecision(18, 0);

            modelBuilder.Entity<BranchGroup>()
                .Property(e => e.PhoneInvestor)
                .IsUnicode(false);

            modelBuilder.Entity<BranchGroup>()
                .HasMany(e => e.Branches)
                .WithOptional(e => e.BranchGroup)
                .HasForeignKey(e => e.IdGroup);

            modelBuilder.Entity<CertificateCategory>()
                .HasMany(e => e.StudentCertifications)
                .WithOptional(e => e.CertificateCategory)
                .HasForeignKey(e => e.IdCertificate);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.ResultCourses)
                .WithOptional(e => e.Class)
                .HasForeignKey(e => e.IdClass);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.Schedules)
                .WithRequired(e => e.Class)
                .HasForeignKey(e => e.IdClass)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.StudentCheckins)
                .WithOptional(e => e.Class)
                .HasForeignKey(e => e.IdClass);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.StudentJoinClasses)
                .WithOptional(e => e.Class)
                .HasForeignKey(e => e.IdClass);

            modelBuilder.Entity<Configuration>()
                .Property(e => e.UsageFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Configuration>()
                .Property(e => e.AccountFee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Coupon>()
                .Property(e => e.Value)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Course>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseBranches)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.IdCourse)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.ProductCourses)
                .WithOptional(e => e.Course)
                .HasForeignKey(e => e.IdCourse);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.RegistrationCourses)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.IdCourse)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.ResultCourses)
                .WithOptional(e => e.Course)
                .HasForeignKey(e => e.IdCourse);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.StudentJoinClasses)
                .WithOptional(e => e.Course)
                .HasForeignKey(e => e.IdCourse);

            modelBuilder.Entity<CourseBranch>()
                .Property(e => e.PriceCourse)
                .HasPrecision(18, 0);

            modelBuilder.Entity<CourseBranch>()
                .Property(e => e.PriceAccount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<CourseBranch>()
                .Property(e => e.PriceTest)
                .HasPrecision(18, 0);

            modelBuilder.Entity<CourseBranch>()
                .Property(e => e.DiscountPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<CourseOnline>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<CourseOnline>()
                .Property(e => e.DiscountPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<CourseOnline>()
                .HasMany(e => e.LessonCourseOnlines)
                .WithOptional(e => e.CourseOnline)
                .HasForeignKey(e => e.IdCourse);

            modelBuilder.Entity<CourseOnline>()
                .HasMany(e => e.UserJoinCourseOnlines)
                .WithRequired(e => e.CourseOnline)
                .HasForeignKey(e => e.IdCourse)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseOnline>()
                .HasMany(e => e.VideoCourseOnlines)
                .WithOptional(e => e.CourseOnline)
                .HasForeignKey(e => e.IdCourse);

            modelBuilder.Entity<Debit>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Debit>()
                .Property(e => e.Payment)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.BasicSalary)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.AchievementEmployees)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.IdEmployee);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.RegistrationTrainings)
                .WithRequired(e => e.Employee)
                .HasForeignKey(e => e.IdEmployee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.SalaryEmployees)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.IdEmployee);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Schedules)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.IdEmployee);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.TrainingResults)
                .WithRequired(e => e.Employee)
                .HasForeignKey(e => e.IdEmpoyee)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Users)
                .WithOptional(e => e.Employee)
                .HasForeignKey(e => e.IdEmployee);

            modelBuilder.Entity<Event>()
                .Property(e => e.Fee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventCheckins)
                .WithOptional(e => e.Event)
                .HasForeignKey(e => e.IdEvent);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventJoins)
                .WithRequired(e => e.Event)
                .HasForeignKey(e => e.IdEvent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<File>()
                .HasMany(e => e.TrainingDocuments)
                .WithRequired(e => e.File)
                .HasForeignKey(e => e.IdDocument)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FileType>()
                .HasMany(e => e.Files)
                .WithOptional(e => e.FileType)
                .HasForeignKey(e => e.IdType);

            modelBuilder.Entity<Folder>()
                .HasMany(e => e.Files)
                .WithOptional(e => e.Folder)
                .HasForeignKey(e => e.IdFolder);

            modelBuilder.Entity<GroupChat>()
                .HasMany(e => e.MessageGroups)
                .WithOptional(e => e.GroupChat)
                .HasForeignKey(e => e.IdGroup);

            modelBuilder.Entity<GroupChat>()
                .HasMany(e => e.NewsFeeds)
                .WithOptional(e => e.GroupChat)
                .HasForeignKey(e => e.ToGroup);

            modelBuilder.Entity<LessonCourseOnline>()
                .HasMany(e => e.JoinCourseOnlineLogs)
                .WithOptional(e => e.LessonCourseOnline)
                .HasForeignKey(e => e.IdLesson);

            modelBuilder.Entity<Log>()
                .HasMany(e => e.UserLogs)
                .WithOptional(e => e.Log)
                .HasForeignKey(e => e.IdLog);

            modelBuilder.Entity<MKTCampaign>()
                .Property(e => e.Fee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<MKTCampaign>()
                .HasMany(e => e.Students)
                .WithOptional(e => e.MKTCampaign)
                .HasForeignKey(e => e.IdBranch);


            modelBuilder.Entity<Student>()
               .HasOptional(s => s.User)  // Optional relationship
               .WithMany()
               .HasForeignKey(s => s.IdUser);  // Foreign key column

            modelBuilder.Entity<NewsFeed>()
                .HasMany(e => e.ReactionNewsFeeds)
                .WithRequired(e => e.NewsFeed)
                .HasForeignKey(e => e.IdNewsFeed);

            modelBuilder.Entity<Order>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderDetails)
                .WithRequired(e => e.Order)
                .HasForeignKey(e => e.IdOrder)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderStatus)
                .WithRequired(e => e.Order)
                .HasForeignKey(e => e.IdOrder)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Transactions)
                .WithOptional(e => e.Order)
                .HasForeignKey(e => e.IdOrder);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Permission>()
                .HasMany(e => e.UserPermissions)
                .WithRequired(e => e.Permission)
                .HasForeignKey(e => e.IdPermission);

            modelBuilder.Entity<PermissionCategory>()
                .HasMany(e => e.Permissions)
                .WithOptional(e => e.PermissionCategory)
                .HasForeignKey(e => e.IdPermissionCategory);

            modelBuilder.Entity<Position>()
                .HasMany(e => e.Employees)
                .WithOptional(e => e.Position)
                .HasForeignKey(e => e.IdPosition);

            modelBuilder.Entity<PProductRecieptionDetail>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PProductRecieptionDetail>()
                .Property(e => e.Discount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PProductRecieptionDetail>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PrivateProduct>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PrivateProduct>()
                .HasMany(e => e.PProductRecieptionDetails)
                .WithRequired(e => e.PrivateProduct)
                .HasForeignKey(e => e.IdProduct)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .Property(e => e.DiscountPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductCourses)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.IdProduct);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductPromotions)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.IdProduct)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductReceiptionDetails)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.IdProduct)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.RegistrationProducts)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.IdProduct)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(e => e.PrivateProducts)
                .WithOptional(e => e.ProductCategory)
                .HasForeignKey(e => e.IdCategory);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.ProductCategory)
                .HasForeignKey(e => e.IdCategory);

            modelBuilder.Entity<ProductReceiptionDetail>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ProductReceiptionDetail>()
                .Property(e => e.Discount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<ProductReceiptionDetail>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Program>()
                .HasMany(e => e.Courses)
                .WithOptional(e => e.Program)
                .HasForeignKey(e => e.IdProgram);

            modelBuilder.Entity<Program>()
                .HasMany(e => e.CourseOnlines)
                .WithOptional(e => e.Program)
                .HasForeignKey(e => e.IdProgram);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Tasks)
                .WithOptional(e => e.Project)
                .HasForeignKey(e => e.IdProject);

            modelBuilder.Entity<Promotion>()
                .Property(e => e.Value)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Promotion>()
                .HasMany(e => e.ProductPromotions)
                .WithRequired(e => e.Promotion)
                .HasForeignKey(e => e.IdPromotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Promotion>()
                .HasMany(e => e.RegistrationProducts)
                .WithOptional(e => e.Promotion)
                .HasForeignKey(e => e.IdPromotion);

            modelBuilder.Entity<Registration>()
                .Property(e => e.Amount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Registration>()
                .Property(e => e.Discount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Registration>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Registration>()
                .HasMany(e => e.Debits)
                .WithOptional(e => e.Registration)
                .HasForeignKey(e => e.IdRegistration);

            modelBuilder.Entity<Registration>()
                .HasMany(e => e.RegistrationCourses)
                .WithRequired(e => e.Registration)
                .HasForeignKey(e => e.IdRegistration)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Registration>()
                .HasMany(e => e.RegistrationOthers)
                .WithRequired(e => e.Registration)
                .HasForeignKey(e => e.IdRegistration)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Registration>()
                .HasMany(e => e.RegistrationProducts)
                .WithRequired(e => e.Registration)
                .HasForeignKey(e => e.IdRegistration)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Registration>()
                .HasMany(e => e.ResultCourses)
                .WithOptional(e => e.Registration)
                .HasForeignKey(e => e.IdRegistration);

            modelBuilder.Entity<Registration>()
                .HasMany(e => e.StudentJoinClasses)
                .WithOptional(e => e.Registration)
                .HasForeignKey(e => e.IdRegistration);

            modelBuilder.Entity<Registration>()
                .HasMany(e => e.Transactions)
                .WithOptional(e => e.Registration)
                .HasForeignKey(e => e.IdRegistration);

            modelBuilder.Entity<RegistrationCourse>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RegistrationCourse>()
                .Property(e => e.Discount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RegistrationCourse>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RegistrationOther>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RegistrationOther>()
                .Property(e => e.Discount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RegistrationOther>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RegistrationOther>()
                .Property(e => e.Status)
                .IsFixedLength();

            modelBuilder.Entity<RegistrationProduct>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RegistrationProduct>()
                .Property(e => e.Discount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RegistrationProduct>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RevenueReference>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RevenueReference>()
                .Property(e => e.Discount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<RevenueReference>()
                .HasMany(e => e.RegistrationOthers)
                .WithRequired(e => e.RevenueReference)
                .HasForeignKey(e => e.IdReference)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Room>()
                .HasMany(e => e.Schedules)
                .WithOptional(e => e.Room)
                .HasForeignKey(e => e.IdRoom);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.BasicSalary)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.TeachSalary)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.Overtime)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.Allowance)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.CommissionAllowance)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.TotalSalary)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.SalaryDeduction)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.TotalDeduction)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.ActualBalance)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.Tax)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.SocialSecurity)
                .HasPrecision(18, 0);

            modelBuilder.Entity<SalaryEmployee>()
                .Property(e => e.HealthInsurance)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Schedule>()
                .Property(e => e.IdWeek)
                .IsFixedLength();

            modelBuilder.Entity<Schoolarship>()
                .Property(e => e.Value)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Schoolarship>()
                .HasMany(e => e.SchoolarshipForms)
                .WithRequired(e => e.Schoolarship)
                .HasForeignKey(e => e.IdSchoolarship)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Shipping>()
                .Property(e => e.Fee)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Shipping>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Shipping)
                .HasForeignKey(e => e.IdShipping);

            modelBuilder.Entity<Student>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Student>()
                .Property(e => e.IdMKT)
                .IsFixedLength();

            modelBuilder.Entity<Student>()
                .Property(e => e.Balance)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.Competitions)
                .WithRequired(e => e.Student)
                .HasForeignKey(e => e.IdStudent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.Feedbacks)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.MessageStudents)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.Registrations)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.ResultCourses)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.StudentAdvises)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.StudentCertifications)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.StudentCheckins)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.StudentInputTests)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.StudentJoinClasses)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.StudentVouchers)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<Student>()
                .HasMany(e => e.Transactions)
                .WithOptional(e => e.Student)
                .HasForeignKey(e => e.IdStudent);

            modelBuilder.Entity<StudentVoucher>()
                .Property(e => e.Voucher)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Supplier>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Supplier>()
                .HasMany(e => e.PProductRecieptionDetails)
                .WithOptional(e => e.Supplier)
                .HasForeignKey(e => e.IdSupplier);

            modelBuilder.Entity<Supplier>()
                .HasMany(e => e.ProductReceiptionDetails)
                .WithOptional(e => e.Supplier)
                .HasForeignKey(e => e.IdSupplier);

            modelBuilder.Entity<Task>()
                .HasMany(e => e.AssignTasks)
                .WithRequired(e => e.Task)
                .HasForeignKey(e => e.IdTask)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingCourse>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TrainingCourse>()
                .HasMany(e => e.RegistrationTrainings)
                .WithRequired(e => e.TrainingCourse)
                .HasForeignKey(e => e.IdTraining)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingCourse>()
                .HasMany(e => e.TrainingAttendances)
                .WithRequired(e => e.TrainingCourse)
                .HasForeignKey(e => e.IdTraining)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingCourse>()
                .HasMany(e => e.TrainingDocuments)
                .WithRequired(e => e.TrainingCourse)
                .HasForeignKey(e => e.IdTraining)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingCourse>()
                .HasMany(e => e.TrainingResults)
                .WithRequired(e => e.TrainingCourse)
                .HasForeignKey(e => e.IdTraining)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingCourse>()
                .HasMany(e => e.TraningPayments)
                .WithOptional(e => e.TrainingCourse)
                .HasForeignKey(e => e.IdTraining);

            modelBuilder.Entity<TrainingType>()
                .HasMany(e => e.TrainingCourses)
                .WithOptional(e => e.TrainingType)
                .HasForeignKey(e => e.IdType);

            modelBuilder.Entity<TraningPayment>()
                .Property(e => e.Amount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TraningPayment>()
                .Property(e => e.Descript)
                .IsFixedLength();

            modelBuilder.Entity<Transaction>()
                .Property(e => e.Amount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Transaction>()
                .Property(e => e.Discount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Transaction>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Transaction>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Transaction>()
                .HasMany(e => e.StudentVouchers)
                .WithOptional(e => e.Transaction)
                .HasForeignKey(e => e.IdTransaction);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AchievementEmployees)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ApproveLeaves)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ApproveLeaves1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.ApproveBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AssignTasks)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.AssignTo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AssignTasks1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.CertificateCategories)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Classes)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Competitions)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Competitions1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.UserCheckin);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Configurations)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Coupons)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Courses)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.DataPaymentOnlines)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.DataSuggests)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Debits)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.EmployeeCheckins)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.ApproveBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.EmployeeCheckins1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Events)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.EventCheckins)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.EventJoins)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.IdUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FAQs)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Feeds)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Feedbacks)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Files)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Folders)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.GroupChats)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Informs)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.JoinCourseOnlineLogs)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.LessonCourseOnlines)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.LoginLogs)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.FromUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Messages1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.ToUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.MessageGroups)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.MessageStudents)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.MKTCampaigns)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdBranch);

            modelBuilder.Entity<User>()
                .HasMany(e => e.NewsFeeds)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Notifications)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.PrivateProducts)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ProductCourses)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Programs)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Projects)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Projects1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.ProjectLeader);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Projects2)
                .WithOptional(e => e.User2)
                .HasForeignKey(e => e.Observer);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Promotions)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ReactionNewsFeeds)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.IdUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Registrations)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ReportForms)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.ApproveBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ReportForms1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ReportForms2)
                .WithOptional(e => e.User2)
                .HasForeignKey(e => e.Observer);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ResultCourses)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.RevenueReferences)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Rooms)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SalaryEmployees)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Schedules)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Schoolarships)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SchoolarshipForms)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SchoolarshipForms1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.ApproveBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Shippings)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.StudentAdvises)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.StudentCheckins)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.StudentInputTests)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.StudentJoinClasses)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.StudentVouchers)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Tasks)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Tasks1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Tasks2)
                .WithOptional(e => e.User2)
                .HasForeignKey(e => e.Observer);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrainingAttendances)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.IdUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrainingCourses)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrainingDocuments)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrainingResults)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrainingTypes)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TraningPayments)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Transactions)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserJoinCourseOnlines)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.ApproveBy);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserJoinCourseOnlines1)
                .WithRequired(e => e.User1)
                .HasForeignKey(e => e.IdUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserLogs)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserPermissions)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.IdUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.VacationSchedules)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.VideoCourseOnlines)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<User>()
                .HasMany(e => e.WarehouseReceiptions)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.IdUser);

            modelBuilder.Entity<WarehouseReceiption>()
                .Property(e => e.Debit)
                .HasPrecision(18, 0);

            modelBuilder.Entity<WarehouseReceiption>()
                .Property(e => e.Credit)
                .HasPrecision(18, 0);

            modelBuilder.Entity<WarehouseReceiption>()
                .Property(e => e.TotalAmount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<WarehouseReceiption>()
                .HasMany(e => e.PProductRecieptionDetails)
                .WithRequired(e => e.WarehouseReceiption)
                .HasForeignKey(e => e.IdReceiption)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WarehouseReceiption>()
                .HasMany(e => e.ProductReceiptionDetails)
                .WithRequired(e => e.WarehouseReceiption)
                .HasForeignKey(e => e.IdReceiption)
                .WillCascadeOnDelete(false);
        }
    }
}
