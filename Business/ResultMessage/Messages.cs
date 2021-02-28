using System;
using System.Collections.Generic;
using System.Text;

namespace Business.ResultMessage
{
    public static class Messages
    {
        public static string ProductAdded = "Ürün Eklendi";
        public static string ProductNameInvalid = "Ürün ismi geçersiz";
        public static string ProductsListed = "Ürünler Listelendi";
        public static string ProductCountOfCategoryError = "Aynı kategorideki ürün sayısı fazla";
        public static string ProductNameAlreadyExists = "Bu isimde zaten başka bir ürün var.";

        public static string CategoryOfCountLimit = "Kategori sayısı 15 ten fazla olduğu için ürün eklenemez";
    }
}
