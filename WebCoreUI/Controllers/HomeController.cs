using Business.Concrete;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreUI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            CategoryManager categoryManager = new CategoryManager(new EfCategoryDal());
            var result=categoryManager.GetAll();
            List<Category> categoryList=null;
            if (result.Success)
            {
                categoryList = result.Data;
            }
            
            return View(categoryList);
        }
    }
}
