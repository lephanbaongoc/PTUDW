using System;
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
    public class ProductController : Controller
    {
        ProductsDAO productsDAO = new ProductsDAO();
        CategoriesDAO categoriesDAO = new CategoriesDAO();
        SuppliersDAO suppliersDAO = new SuppliersDAO();

        //////////////////////////////////////////////////////////////////////////////////////
        //INDEX
        // GET: Admin/Product
        public ActionResult Index()
        {
            return View(productsDAO.getList("Index"));
        }


        //////////////////////////////////////////////////////////////////////////////////////
        //DETAIL
        // GET: Admin/Product/Details/5
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(products);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //CREATE
        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatId - truy vấn bằng Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");// sai SupplierId truy vấn bằng Suppliers
            //Dùng để lựa chọn từ danh sách droplist như bảng Categories: ParentID và Suppliers: ParentID
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Products products)
        {
            if (ModelState.IsValid)
            {

                //xử lý tự động cho các trường: Slug, CreateAt/By, UpdateAt/By, Order
                //Xử lý tự động: CreateAt
                products.CreateAt = DateTime.Now;
                //Xử lý tự động: UpdateAt
                products.UpdateAt = DateTime.Now;
                //Xử lý tự động: CreateBy
                products.CreateBy = Convert.ToInt32(Session["UserID"]);
                //Xử lý tự động: UpdateBy
                products.UpdateBy = Convert.ToInt32(Session["UserID"]);
                //Xử lý tự động: Slug
                products.Slug = XString.Str_Slug(products.Name);

                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;
                        //upload hinh
                        string PathDir = "~/Public/img/product";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //lưu vào DB
                productsDAO.Insert(products);
                //Thông báo tạo mẫu tin thành công 
                TempData["message"] = new XMessage("success", "Tạo mới sản phẩm thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatId - truy vấn bằng Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");// sai SupplierId truy vấn bằng Suppliers
            return View(products);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //EDIT
        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatId - truy vấn bằng Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");// sai SupplierId truy vấn bằng Suppliers
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Products products)
        {
            if (ModelState.IsValid)
            {
                //xử lý tự động cho các trường: Slug, CreateAt/By, UpdateAt/By, Order
                //Xử lý tự động: UpdateAt
                products.UpdateAt = DateTime.Now;
                //Xử lý tự động: Slug
                products.Slug = XString.Str_Slug(products.Name);

                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                string PathDir = "~/Public/img/product";
                if (img.ContentLength != 0)
                {
                    //Xu ly cho muc xoa hinh anh
                    if (products.Img != null)
                    {
                        string DelPath = Path.Combine(Server.MapPath(PathDir), products.Img);
                        System.IO.File.Delete(DelPath);
                    }

                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;
                        //upload hinh
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }

                }//ket thuc phan upload hinh anh


                //Cập nhật mẫu tin vào DB
                productsDAO.Update(products);
                //Thông báo tạo mẫu tin thành công 
                TempData["message"] = new XMessage("success", "Cập nhật sản phẩm thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatId - truy vấn bằng Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");// sai SupplierId truy vấn bằng Suppliers
            return View(products);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //EDIT
        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(products);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //DELETE
        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = productsDAO.getRow(id);
            //xu ly cho phan upload hình ảnh
            var img = Request.Files["img"];//lay thong tin file
            string PathDir = "~/Public/img/product";
            if (productsDAO.Delete(products) == 1)
            {
                //Xu ly cho muc xoa hinh anh
                if (products.Img != null)
                {
                    string DelPath = Path.Combine(Server.MapPath(PathDir), products.Img);
                    System.IO.File.Delete(DelPath);
                }
            }
            //Thông báo xóa mẫu tin thành công 
            TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
            return RedirectToAction("Trash");
        }

        //Phát sinh thêm 1 số Action: Status, Trash, DelTrash, Undo
        //////////////////////////////////////////////////////////////////////////////////////
        //STATUS
        // GET: Admin/Product/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            //Truy vấn dòng có Id = Id yêu cầu
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }

            else
            {
                //Chuyển đổi trạng thái của status từ 1<->2
                products.Status = (products.Status == 1) ? 2 : 1;

                //Cập nhật giá trị UpdateAt 
                products.UpdateAt = DateTime.Now;

                //Cập nhật lại database
                productsDAO.Update(products);

                //Thông báo cập nhật trạng thái thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
                return RedirectToAction("Index");
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //DELTRASH
        // GET: Admin/Product/DelTrash/5
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
                return RedirectToAction("Index");
            }
            //Truy vấn dòng có Id = Id yêu cầu
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tìm thấy nhà cung cấp");
                return RedirectToAction("Index");
            }

            else
            {
                //Chuyển đổi trạng thái của status từ 1,2  -> 0: Không hiển thị ở Index
                products.Status = 0;

                //Cập nhật giá trị UpdateAt 
                products.UpdateAt = DateTime.Now;

                //Cập nhật lại database
                productsDAO.Update(products);

                //Thông báo cập nhật trạng thái thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");
                return RedirectToAction("Index");
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //INDEX
        // GET: Admin/Product
        public ActionResult Trash()
        {
            return View(productsDAO.getList("Trash"));
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //RECOVER
        // GET: Admin/Product/Recover/5
        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            //Truy vấn dòng có Id = Id yêu cầu
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }

            else
            {
                //Chuyển đổi trạng thái của status từ 0 -> 2: Không xuất bản
                products.Status = 2;

                //Cập nhật giá trị UpdateAt 
                products.UpdateAt = DateTime.Now;

                //Cập nhật lại database
                productsDAO.Update(products);

                //Thông báo phục hồi dữ liệu thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Phục hồi mẫu tin thành công");
                return RedirectToAction("Index");
            }
        }
    }
}
