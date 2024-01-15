using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnLTWeb;

namespace OnLTWeb.Controllers
{
    public class SanPhamsController : Controller
    {
        private QLBanHangQuanAoEntities db = new QLBanHangQuanAoEntities();

        // GET: SanPhams
        public ActionResult Index()
        {
            var sanPhams = db.SanPhams.Include(s => s.PhanLoai).Include(s => s.PhanLoaiPhu);
            ViewBag.PhanLoaiChinh = new SelectList(db.PhanLoais, "MaPhanLoai", "PhanLoaiChinh");
            return View(sanPhams.ToList());
        }

        public ActionResult GetProductsByCategory(string cateId)
        {
            var products = db.SanPhams.Where(p => p.MaPhanLoaiPhu == cateId).ToList();

            // tránh lỗi tham chiếu vòng tròn
            var _products = products.Select(p => new
            {
                MaSanPham = p.MaSanPham,
                TenSanPham = p.TenSanPham,
                MaPhanLoai = p.MaPhanLoai,
                GiaNhap = p.GiaNhap,
                DonGiaBanNhoNhat = p.DonGiaBanNhoNhat,
                DonGiaBanLonNhat = p.DonGiaBanLonNhat,
                TrangThai = p.TrangThai,
                MoTaNgan = p.MoTaNgan,
                AnhDaiDien = p.AnhDaiDien,
                NoiBat = p.NoiBat,
                MaPhanLoaiPhu = p.MaPhanLoaiPhu,
            }).ToList();

            return Json(new { products = _products }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RenderProductByCatId(int CatId)
        {
            //List<SanPham> listSanPham = db.SanPhams.Where(a =>a.MaPhanLoai.Contains(CatId.ToString())).ToList();

            string id = CatId.ToString();

            List<SanPham> listSanPham = db.SanPhams.Where(a => a.MaPhanLoaiPhu.Contains(id)).ToList();

            return PartialView("MainContent", listSanPham);
        }

        // GET: SanPhams/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        // GET: SanPhams/Create
        public ActionResult Create()
        {
            ViewBag.MaPhanLoai = new SelectList(db.PhanLoais, "MaPhanLoai", "PhanLoaiChinh");
            ViewBag.MaPhanLoaiPhu = new SelectList(db.PhanLoaiPhus, "MaPhanLoaiPhu", "TenPhanLoaiPhu");
            return View();
        }

        // POST: SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaSanPham,TenSanPham,MaPhanLoai,GiaNhap,DonGiaBanNhoNhat,DonGiaBanLonNhat,TrangThai,MoTaNgan,NoiBat,MaPhanLoaiPhu")] SanPham sanPham, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                // Lưu hình ảnh vào thư mục
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                    file.SaveAs(path);

                    // Lưu đường dẫn của ảnh vào model
                    sanPham.AnhDaiDien = fileName;
                }

                // Lưu sản phẩm vào cơ sở dữ liệu và chuyển hướng
                db.SanPhams.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, lấy lại danh sách phân loại để hiển thị trong dropdown
            ViewBag.MaPhanLoai = new SelectList(db.PhanLoais, "MaPhanLoai", "PhanLoaiChinh", sanPham.MaPhanLoai);
            ViewBag.MaPhanLoaiPhu = new SelectList(db.PhanLoaiPhus, "MaPhanLoaiPhu", "TenPhanLoaiPhu", sanPham.MaPhanLoaiPhu);
            return View(sanPham);
        }

        // GET: SanPhams/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaPhanLoai = new SelectList(db.PhanLoais, "MaPhanLoai", "PhanLoaiChinh", sanPham.MaPhanLoai);
            ViewBag.MaPhanLoaiPhu = new SelectList(db.PhanLoaiPhus, "MaPhanLoaiPhu", "TenPhanLoaiPhu", sanPham.MaPhanLoaiPhu);
            return View(sanPham);
        }

        // POST: SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaSanPham,TenSanPham,MaPhanLoai,GiaNhap,DonGiaBanNhoNhat,DonGiaBanLonNhat,TrangThai,MoTaNgan,AnhDaiDien,NoiBat,MaPhanLoaiPhu")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaPhanLoai = new SelectList(db.PhanLoais, "MaPhanLoai", "PhanLoaiChinh", sanPham.MaPhanLoai);
            ViewBag.MaPhanLoaiPhu = new SelectList(db.PhanLoaiPhus, "MaPhanLoaiPhu", "TenPhanLoaiPhu", sanPham.MaPhanLoaiPhu);
            return View(sanPham);
        }

        // GET: SanPhams/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }
            return View(sanPham);
        }

        // POST: SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            SanPham sanPham = db.SanPhams.Find(id);
            db.SanPhams.Remove(sanPham);
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
