using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    public class CategoryController : Controller
    {
        CategoriesDAO categoriesDAO = new CategoriesDAO();

        //////////////////////////////////////////////////////////////////////////////////////
        //INDEX
        // GET: Admin/Category
        public ActionResult Index()
        {
            return View(categoriesDAO.getList("Index"));
        }


        //////////////////////////////////////////////////////////////////////////////////////
        //DETAILS
        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tồn tại loại sản phẩm");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại loại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(categories);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //CREATE
        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Categories categories)
        {
            if (ModelState.IsValid)
            {
                //Xử lý tự động: CreateAt
                categories.CreateAt = DateTime.Now;
                //Xử lý tự động: UpdateAt
                categories.UpdateAt = DateTime.Now;
                //Xử lý tự động: ParentId
                if (categories.ParentID == null)
                {
                    categories.ParentID = 0;
                }
                //Xử lý tự động: Order
                if (categories.Order == null)
                {
                    categories.Order = 1;
                }
                else
                {
                    categories.Order += 1;
                }
                //Xử lý tự động: Slug
                categories.Slug = XString.Str_Slug(categories.Name);


                //Chèn thêm dòng cho data base
                categoriesDAO.Insert(categories);
                //thông báo thêm danh mục sản phẩm thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Tạo mới loại sản phẩm thành công");
                //Trở về Index
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //EDIT
        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại
                TempData["message"] = TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //Thông báo thất bại
                TempData["message"] = TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Categories categories)
        {
            if (ModelState.IsValid)
            {
                //Xử lý tự động: Slug
                categories.Slug = XString.Str_Slug(categories.Name);
                //Xử lý tự động: ParentID
                if (categories.ParentID == null)
                {
                    categories.ParentID = 0;
                }
                //Xử lý tự động: Order
                if (categories.Order == null)
                {
                    categories.Order = 1;
                }
                else
                {
                    categories.Order += 1;
                }
                //Xử lý tự động: UpdateAt
                categories.UpdateAt = DateTime.Now;

                //Cập nhật mẫu tin
                categoriesDAO.Update(categories);

                //Thông báo thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Cập nhật mẫu tin thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //DELETE
        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //thong bao that bai 
                TempData["message"] = new XMessage("danger", "Xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //thong bao that bai 
                TempData["message"] = new XMessage("danger", "Xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            return View(categories);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Categories categories = categoriesDAO.getRow(id);
            categoriesDAO.Delete(categories);

            //thong bao thanh cong
            TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");
            return RedirectToAction("Trash");
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //STATUS
        // GET: Admin/Category/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            //Truy vấn dòng có Id = Id yêu cầu
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }

            else
            {
                //Chuyển đổi trạng thái của status từ 1<->2
                categories.Status = (categories.Status == 1) ? 2 : 1;

                //Cập nhật giá trị UpdateAt 
                categories.UpdateAt = DateTime.Now;

                //Cập nhật lại database
                categoriesDAO.Update(categories);

                //Thông báo cập nhật trạng thái thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
                return RedirectToAction("Index");
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //DELTRASH
        // GET: Admin/Category/DelTrash/5
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            //Truy vấn dòng có Id = Id yêu cầu
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }

            else
            {
                //Chuyển đổi trạng thái của status từ 1,2  -> 0: Không hiển thị ở Index
                categories.Status = 0;

                //Cập nhật giá trị UpdateAt 
                categories.UpdateAt = DateTime.Now;

                //Cập nhật lại database
                categoriesDAO.Update(categories);

                //Thông báo cập nhật trạng thái thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");
                return RedirectToAction("Index");
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //INDEX
        // GET: Admin/Category
        public ActionResult Trash()
        {
            return View(categoriesDAO.getList("Trash"));
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //RECOVER
        // GET: Admin/Category/Recover/5
        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            //Truy vấn dòng có Id = Id yêu cầu
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //Thông báo thất bại 
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }

            else
            {
                //Chuyển đổi trạng thái của status từ 0 -> 2: Không xuất bản
                categories.Status = 2;

                //Cập nhật giá trị UpdateAt 
                categories.UpdateAt = DateTime.Now;

                //Cập nhật lại database
                categoriesDAO.Update(categories);

                //Thông báo phục hồi dữ liệu thành công
                TempData["message"] = TempData["message"] = new XMessage("success", "Phục hồi mẫu tin thành công");
                return RedirectToAction("Index");
            }
        }
    }
}
