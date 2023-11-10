using MyClass.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClass.DAO
{
    public class ProductsDAO//copy nội dung của class SUPPLIERS, thay thế Suppliers bằng Products
    {
        private MyDBContext db = new MyDBContext();

        //SELECT * FROM ...
        public List<Products> getList()
        {
            return db.Products.ToList();
        }

        //Index chi voi status 1,2        
        public List<Products> getList(string status = "ALL")//status 0,1,2
        {
            List<Products> list = null;
            switch (status)
            {
                case "Index"://1,2
                    {
                        list = db.Products.Where(m => m.Status != 0).ToList();
                        break;
                    }
                case "Trash"://0
                    {
                        list = db.Products.Where(m => m.Status == 0).ToList();
                        break;
                    }
                default:
                    {
                        list = db.Products.ToList();
                        break;
                    }
            }
            return list;
        }
        //details
        public Products getRow(int? id)
        {
            if (id == null)
            {
                return null;
            }
            else
            {
                return db.Products.Find(id);
            }
        }

        //tao moi mau tin
        public int Insert(Products row)
        {
            db.Products.Add(row);
            return db.SaveChanges();
        }

        //cap nhat mau tin
        public int Update(Products row)
        {
            db.Entry(row).State = EntityState.Modified;
            return db.SaveChanges();
        }

        //Xoa mau tin
        public int Delete(Products row)
        {
            db.Products.Remove(row);
            return db.SaveChanges();//thành công => return 1
        }
    }
}
