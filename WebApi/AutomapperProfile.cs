using AutoMapper;
using Business.Models;
using Data.Entities;
using System.Linq;

namespace WebApi
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Receipt, ReceiptModel>()
                .ForMember(rm => rm.ReceiptDetailsIds, r => r.MapFrom(x => x.ReceiptDetails.Select(rd => rd.Id)))
                .ReverseMap();

           
            CreateMap<Product, ProductModel>()
                .ForMember(pm => pm.ReceiptDetailIds, p => p.MapFrom(x => x.ReceiptDetails.Select(rd => rd.Id)))
                .ForMember(pm => pm.CategoryName, p => p.MapFrom(x => x.Category.CategoryName))
                .ReverseMap();


           
            CreateMap<ReceiptDetail, ReceiptDetailModel>()
                .ReverseMap();


            CreateMap<Customer, CustomerModel>()
                .ForMember(cm => cm.ReceiptsIds, c => c.MapFrom(x => x.Receipts.Select(r => r.Id)))
                .ForMember(cm => cm.Name, c => c.MapFrom(x => x.Person.Name))
                .ForMember(cm => cm.Surname, c => c.MapFrom(x => x.Person.Surname))
                .ForMember(cm => cm.BirthDate, c => c.MapFrom(x => x.Person.BirthDate))
                .ReverseMap();
                //.ForPath(c => c.Person.Name, cm => cm.MapFrom(x => x.Name))
                //.ForPath(c => c.Person.Surname, cm => cm.MapFrom(x => x.Surname))
                //.ForPath(c => c.Person.BirthDate, cm => cm.MapFrom(x => x.BirthDate));


            //CreateMap<CustomerModel, Customer>()
            //    .ForMember(c => c.Receipts, c => c.Ignore())
            //    .ForPath(cm => cm.Person.Name, c => c.MapFrom(x => x.Name))
            //    .ForPath(cm => cm.Person.Surname, c => c.MapFrom(x => x.Surname))
            //    .ForPath(cm => cm.Person.BirthDate, c => c.MapFrom(x => x.BirthDate))
            //    .ReverseMap();



            CreateMap<ProductCategory, ProductCategoryModel>()
                .ForMember(pcm => pcm.ProductIds, pc => pc.MapFrom(x => x.Products.Select(p => p.Id)))
                .ReverseMap();
        }
    }
}