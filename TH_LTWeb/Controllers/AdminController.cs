using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TH_LTWeb.Models;
using PagedList.Mvc;
using PagedList;
using System.Data.Entity.Validation;
using System.IO;
using System.Net;
using System.Web.Helpers;


using System.Data.Entity;
using System.Threading.Tasks; // Thêm thư viện hỗ trợ async/await

namespace TH_LTWeb.Controllers
{
    public class AdminController : Controller
    {
        // Khởi tạo DbContext
        private QLBANXEGANMAYEntities3 db = new QLBANXEGANMAYEntities3();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Xe (Hiển thị danh sách xe)
        public ActionResult Xe(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            return View(db.XEGANMAYs.ToList().OrderBy(n => n.MaXe).ToPagedList(pageNumber, pageSize));
        }

        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "Phải nhập tên đăng nhập");
            }
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "Phải nhập mật khẩu");
            }

            if (ModelState.IsValid)
            {
                var admin = db.Admins.SingleOrDefault(a => a.UserAdmin == username && a.PassAdmin == password);
                if (admin != null)
                {
                    Session["Taikhoanadmin"] = admin;
                    return RedirectToAction("Index", "Admin");
                }
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            }
            return View();
        }

        [HttpGet]
        public ActionResult ThemmoiXe()
        {
            ViewBag.MaLX = new SelectList(db.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
            ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");

            return View();
        }

        [HttpPost]

        [ValidateInput(false)]

        public ActionResult ThemmoiXe(XEGANMAY xeganmay, HttpPostedFileBase fileUpload)
        {
            //Dua du lieu vao dropdownload
            ViewBag.MaLX = new SelectList(db.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe");
            ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP");
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";

                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {

                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/Content/Images/XE/Yamaha/"), fileName); //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    //Luu hinh anh vao duong dan
                    xeganmay.Anhbia = fileName;
                    //Luu vào CSDL
                    db.XEGANMAYs.Add(xeganmay);
                    db.SaveChanges();
                }
                else
                {
                    return Json(new {mesage = "ádasd"}, JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Xe");
            }
        }

        public ActionResult ChitietXe(int id)
        {
            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(xe);
        }

        [HttpGet]

        public ActionResult XoaXe(int id)
        {
            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(n => n.MaXe == id);
            ViewBag.MaXe = xe.MaXe;
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(xe);

        }
        [HttpPost, ActionName("XoaXe")]
        public ActionResult Xacnhanxoa(int id)
        {
            XEGANMAY xe = db.XEGANMAYs.SingleOrDefault(m => m.MaXe == id);
            if (xe == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.XEGANMAYs.Remove(xe);
            db.SaveChanges();
            return RedirectToAction("Xe");
        }

        [HttpGet]
        public ActionResult SuaXe(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XEGANMAY xe = db.XEGANMAYs.Include(s => s.LOAIXE).FirstOrDefault(s => s.MaXe == id);
            if (xe == null)
            {
                //return HttpNotFound();
                Response.StatusCode = 404;
                return null;
            }
            var chude = db.LOAIXEs.Select(c => new { c.MaLX, c.TenLoaiXe }).ToList();
            ViewData["MaLX"] = new SelectList(chude, "MaLX", "TenLoaiXe", xe.MaLX);
            ViewData["MaNPP"] = new SelectList(db.NHAPHANPHOIs, "MaNPP", "TenNPP", xe.MaNPP);

            return View(xe);

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suaxe(XEGANMAY xeganmay, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaLX = new SelectList(db.LOAIXEs.ToList().OrderBy(n => n.TenLoaiXe), "MaLX", "TenLoaiXe", xeganmay.MaLX);
            ViewBag.MaNPP = new SelectList(db.NHAPHANPHOIs.ToList().OrderBy(n => n.TenNPP), "MaNPP", "TenNPP", xeganmay.MaNPP);

            if (ModelState.IsValid)
            {
                var existingCar = db.XEGANMAYs.FirstOrDefault(c => c.MaXe == xeganmay.MaXe);
                if (existingCar == null)
                {
                    ViewBag.Announce = "Car not found.";
                    return Json(new { mess = "a" }, JsonRequestBehavior.AllowGet);
                }

                existingCar.TenXe = xeganmay.TenXe;
                existingCar.Giaban = xeganmay.Giaban;
                existingCar.Mota = xeganmay.Mota;
                existingCar.Ngaycapnhat = xeganmay.Ngaycapnhat;
                existingCar.Soluongton = xeganmay.Soluongton;
                existingCar.MaLX = xeganmay.MaLX;
                existingCar.MaNPP = xeganmay.MaNPP;

                if (fileUpload != null)
                {
                    var filename = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/images"), filename);

                    if (!System.IO.File.Exists(path))
                    {
                        fileUpload.SaveAs(path);
                        existingCar.Anhbia = filename;
                    }
                    else
                    {
                        ViewBag.Announce = "Picture already exists.";
                        return Json(new { mess = "b" }, JsonRequestBehavior.AllowGet);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Xe", "Admin");
            }

            ViewBag.Announce = "Model state is invalid.";
            return Json(new { mess = "b" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ThongKeXe()
        {
            var thongKe = db.XEGANMAYs
                            .GroupBy(x => x.MaLX)
                            .Select(g => new
                            {
                                LoaiXe = g.FirstOrDefault().LOAIXE.TenLoaiXe,
                                SoLuong = g.Count()
                            })
                            .ToList();

            // Dữ liệu biểu đồ
            var chart = new Chart(width: 800, height: 400)
                        .AddTitle("Thống kê số lượng xe theo loại")
                        .AddLegend("Loại xe")
                        .AddSeries(
                            chartType: "Column",
                            xValue: thongKe.Select(x => x.LoaiXe).ToArray(),
                            yValues: thongKe.Select(x => x.SoLuong).ToArray()
                        )
                        .Write();

            return null;
        }

    }
}
