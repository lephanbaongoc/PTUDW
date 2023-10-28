﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.DAO;
using MyClass.Model;
using PTUDW.Library;
using UDW.Library;

namespace PTUDW.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {
        SuppliersDAO suppliersDAO = new SuppliersDAO();

        //////////////////////////////////////////////////////////////////////////////////////
        //INDEX
        // GET: Admin/Supplier
        public ActionResult Index()
        {
            return View(suppliersDAO.getList("Index"));
        }

        //////////////////////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            return View(suppliers);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //CREATE
        // GET: Admin/Supplier/Create
        public ActionResult Create()
        {
            ViewBag.ListOrder = new SelectList(suppliersDAO.getList("Index"), "Order", "Name");
            return View();
        }

        //////////////////////////////////////////////////////////////////////////////////////
        // POST: Admin/Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                //xử lý tự động cho các trường: Slug, CreateAt/By, UpdateAt/By, Order
                //Xử lý tự động: CreateAt
                suppliers.CreateAt = DateTime.Now;
                //Xử lý tự động: UpdateAt
                suppliers.UpdateAt = DateTime.Now;
                //Xử lý tự động: CreateBy
                suppliers.CreateBy = Convert.ToInt32(Session["UserID"]);
                //Xử lý tự động: UpdateBy
                suppliers.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //Xử lý tự động: Order
                if (suppliers.Order == null)
                {
                    suppliers.Order = 1;
                }
                else
                {
                    suppliers.Order += 1;
                }
                //Xử lý tự động: Slug
                suppliers.Slug = XString.Str_Slug(suppliers.Name);

                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = suppliers.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + suppliers.Id + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        suppliers.Image = imgName;
                        //upload hinh
                        string PathDir = "~/Public/img/supplier";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //chèn mẫu tin vào DB
                suppliersDAO.Insert(suppliers);
                //Thông báo tạo mẫu tin thành công 
                TempData["message"] = new XMessage("success", "Tạo mới nhà cung cấp thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListOrder = new SelectList(suppliersDAO.getList("Index"), "Order", "Name");
            return View(suppliers);
        }
        //////////////////////////////////////////////////////////////////////////////////////
        // GET: Admin/Supplier/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            ViewBag.ListOrder = new SelectList(suppliersDAO.getList("Index"), "Order", "Name");
            return View(suppliers);
        }

        // POST: Admin/Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                //xử lý tự động cho các trường: Slug, CreateAt/By, UpdateAt/By, Order
                //Xử lý tự động: UpdateAt
                suppliers.UpdateAt = DateTime.Now;
                //Xử lý tự động: Order
                if (suppliers.Order == null)
                {
                    suppliers.Order = 1;
                }
                else
                {
                    suppliers.Order += 1;
                }
                //Xử lý tự động: Slug
                suppliers.Slug = XString.Str_Slug(suppliers.Name);
                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                string PathDir = "~/Public/img/supplier";
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = suppliers.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + suppliers.Id + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        suppliers.Image = imgName;
                        //upload hinh
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //Xử lý cho mục xóa hình ảnh
                if(suppliers.Image != null)
                {
                    string DelPath = Path.Combine(Server.MapPath(PathDir), suppliers.Image);
                    System.IO.File.Delete(DelPath);
                }

                //Cập nhật mẫu tin vào DB
                suppliersDAO.Update(suppliers);
                //Thông báo tạo mẫu tin thành công 
                TempData["message"] = new XMessage("success", "Cập nhật nhà cung cấp thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListOrder = new SelectList(suppliersDAO.getList("Index"), "Order", "Name");
            return View(suppliers);
        }

        // GET: Admin/Supplier/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            return View(suppliers);
        }

        // POST: Admin/Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Suppliers suppliers = suppliersDAO.getRow(id);
            //Xóa mẫu tin khỏi DB
            suppliersDAO.Delete(suppliers);
            //Thông báo xóa mẫu tin thành công 
            TempData["message"] = new XMessage("success", "Xóa nhà cung cấp thành công");
            return RedirectToAction("Index");
        }

        //Phát sinh thêm 1 số Action: Status, Trash, DelTrash, Undo
        //////////////////////////////////////////////////////////////////////////////////////
        //STATUS
        // GET: Admin/Supplier/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            //Truy vấn dòng có Id = Id yêu cầu
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }

            else
            {
                //Chuyển đổi trạng thái của status từ 1<->2
                suppliers.Status = (suppliers.Status == 1) ? 2 : 1;

                //Cập nhật giá trị UpdateAt 
                suppliers.UpdateAt = DateTime.Now;

                //Cập nhật lại database
                suppliersDAO.Update(suppliers);

                //Thông báo cập nhật trạng thái thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
                return RedirectToAction("Index");
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //DELTRASH
        // GET: Admin/Supplier/DelTrash/5
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
                return RedirectToAction("Index");
            }
            //Truy vấn dòng có Id = Id yêu cầu
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
                return RedirectToAction("Index");
            }

            else
            {
                //Chuyển đổi trạng thái của status từ 1,2  -> 0: Không hiển thị ở Index
                suppliers.Status = 0;

                //Cập nhật giá trị UpdateAt 
                suppliers.UpdateAt = DateTime.Now;

                //Cập nhật lại database
                suppliersDAO.Update(suppliers);

                //Thông báo cập nhật trạng thái thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");
                return RedirectToAction("Index");
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //INDEX
        // GET: Admin/Supplier
        public ActionResult Trash()
        {
            return View(suppliersDAO.getList("Trash"));
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //RECOVER
        // GET: Admin/Suppliery/Recover/5
        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            //Truy vấn dòng có Id = Id yêu cầu
            Suppliers suppliers = suppliersDAO.getRow(id);
            if (suppliers == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }

            else
            {
                //Chuyển đổi trạng thái của status từ 0 -> 2: Không xuất bản
                suppliers.Status = 2;

                //Cập nhật giá trị UpdateAt 
                suppliers.UpdateAt = DateTime.Now;

                //Cập nhật lại database
                suppliersDAO.Update(suppliers);

                //Thông báo phục hồi dữ liệu thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Phục hồi mẫu tin thành công");
                return RedirectToAction("Index");
            }
        }
     }
}
