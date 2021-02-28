using Business.Abstract;
using Business.CCS;
using Business.ResultMessage;
using Business.ValidationRules.FluentValidation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.BusinessIsKurallariMotoru;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        ILogger _logger;
        //ProductManager içinde başka bir Maneger yazılamaz. Ama service yazılabilir.
        ICategoryService _categoryService;
        public ProductManager(IProductDal productDal, ILogger logger,ICategoryService categoryService)
        {
            _productDal = productDal;
            _logger = logger;

            //service eklememezin sebebi kategori tablosuyla ilgili bir kural gelirse diye(kategori sayısı 15 ten fazla ise ürün eklenemez gibi.)
            _categoryService = categoryService;
        }

        //[ValidationAspect(typeof(ProductValidator))]
        public IResult Add(Product product)
        {
            _logger.Log();
            try
            {
                ValidationTool.Validate(new ProductValidator(), product);

                //Aşağıda yazdığımız iş kuralları metodları IResult döndürdüğü için, kurallara bir iş kuralı motoru yazıldı
                
                IResult errorResult= BusinessRules.Run(CheckIfProductCountOfCategoryCorrect(product.CategoryId), 
                    CheckIfProductNameExists(product.ProductName),
                    CheckIfCategoryLimit());

                if (errorResult!=null)
                {
                    return errorResult;
                }
                _productDal.Add(product);
                return new SuccessResult(Messages.ProductAdded);
                
                //ayrı bir method yazıldı, aşağıda. Her kural için ayrı method(iş parçacığı) yazılır.
                //Yukarıda iş kuralları motoru yazıldığı için bu kısma gerek kalmadı.
                //if (CheckIfProductCountOfCategoryCorrect(product.CategoryId).Success)
                //{
                //    if (CheckIfProductNameExists(product.ProductName).Success)
                //    {
                //        _productDal.Add(product);
                //        return new SuccessResult(Messages.ProductAdded);
                //    }
                //}
                //return new ErrorResult();
            }
            catch (Exception exception)
            {
                //hata oluştuğunda hata logu atabiliriz.
                _logger.Log();
            }
            return new ErrorResult();
        }

        public IDataResult<List<Product>> GetAll()
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(), Messages.ProductsListed);
        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(a => a.CategoryId == id));
        }

        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(a => a.ProductId == productId));
        }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(a => a.UnitPrice >= min && a.UnitPrice <= max));
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails());
        }

        public IResult Update(Product product)
        {
            throw new NotImplementedException();
        }

        private IResult CheckIfProductCountOfCategoryCorrect(int categoryId)
        {
            var result = _productDal.GetAll(a => a.CategoryId == categoryId).Count;
            if (result >= 15)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult();
        }

        private IResult CheckIfProductNameExists(string productName)
        {
            var result = _productDal.GetAll(a => a.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }

        private IResult CheckIfCategoryLimit()
        {
            var result = _categoryService.GetAll().Data.Count;
            if (result>15)
            {
                return new ErrorResult(Messages.CategoryOfCountLimit);
            }
            return new SuccessResult();
        }
    }
}
