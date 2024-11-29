using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TH_LTWeb.Models;

namespace TH_LTWeb.Controllers
{
    public class NguoidungController : Controller
    {
        // GET: Nguoidung
        //KHACHHANG db=new KHACHHANG();
        QLBANXEGANMAYEntities1 db = new QLBANXEGANMAYEntities1();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KHACHHANG kh)
        {
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            var matkhaunhaplai = collection["Matkhaunhaplai"];
            var diachi = collection["Diachi"];
            var email = collection["Email"];
            var dienthoai = collection["Dienthoai"];
            var ngaysinh = collection["Ngaysinh"];

            // Kiểm tra các trường nhập
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được để trống";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(matkhaunhaplai))
            {
                ViewData["Loi4"] = "Phải nhập lại mật khẩu";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Email không được bỏ trống";
            }
            else if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi6"] = "Phải nhập điện thoại";
            }
            else
            {
                // Kiểm tra ngày sinh
                DateTime parsedDate;
                if (!DateTime.TryParse(collection["Ngaysinh"], out parsedDate))
                {
                    ViewData["Loi7"] = "Ngày sinh không hợp lệ. Vui lòng nhập đúng định dạng MM/dd/yyyy.";
                    return this.Dangky();
                }

                // Gán giá trị cho đối tượng
                kh.HoTen = hoten;
                kh.Taikhoan = tendn;
                kh.Matkhau = matkhau;
                kh.Email = email;
                kh.DiachiKH = diachi;
                kh.DienthoaiKH = dienthoai;
                kh.Ngaysinh = parsedDate;

                // Thêm khách hàng vào cơ sở dữ liệu
                db.KHACHHANGs.Add(kh);
                db.SaveChanges();

                return RedirectToAction("Dangnhap");
            }

            return this.Dangky();
        }
        [HttpGet]   
        public ActionResult Dangnhap()
        {
            return View();          
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            // Gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];

            // Kiểm tra đầu vào
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loil"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                // Kiểm tra thông tin đăng nhập trong cơ sở dữ liệu
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn && n.Matkhau == matkhau);

                if (kh != null)
                {
                    // Đăng nhập thành công
                    ViewBag.Thongbao = "Chúc mừng bạn đã đăng nhập thành công!";
                    Session["Taikhoan"] = kh; // Lưu thông tin tài khoản vào session
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Đăng nhập thất bại
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!";
                }
            }

            return View();
        }

    }
}