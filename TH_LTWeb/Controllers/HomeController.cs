
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TH_LTWeb.Models;
using PagedList;
using PagedList.Mvc;
using System.Web.UI;

namespace TH_LTWeb.Controllers
{
    public class HomeController : Controller

    {
        // GET: Home

        QLBANXEGANMAYEntities3 data = new QLBANXEGANMAYEntities3();

        private List<XEGANMAY> layxeganmaymoi(int count)
        {
            return data.XEGANMAYs.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }

        public ActionResult Index(int ? page)
        {
            var xemoi = layxeganmaymoi(5);
            int pageSize = 5;
            int pageNum = (page ?? 1);
            return View(xemoi.ToPagedList(pageNum, pageSize));

        }
        public ActionResult LoaiXe()
        {
            var loaixe = data.LOAIXEs.ToList(); // Lấy danh sách từ bảng LOAIXE
            return PartialView(loaixe);
        }

        public ActionResult NhaPhanPhoi()
        {
            var loaixe = data.NHAPHANPHOIs.ToList(); // Lấy danh sách từ bảng LOAIXE
            return PartialView(loaixe);
        }

        public ActionResult SPTheoLoaixe(int id)
        {
            var xeganmay = from s in data.XEGANMAYs
                           where s.MaXe == id
                           select s;
            return View(xeganmay.ToList()); // Truyền dữ liệu ra view
        }
        public ActionResult SPTheoNPP(int id)
        {
            var xeganmays = from s in data.XEGANMAYs
                            where s.MaXe == id
                            select s;
            return View(xeganmays.ToList());
        }

        public ActionResult Details(int id)
        {
            var xeganmay = data.XEGANMAYs.FirstOrDefault(s => s.MaXe == id);
            if (xeganmay == null)
            {
                return HttpNotFound();
            }
            return View(xeganmay);
        }

    }
}
